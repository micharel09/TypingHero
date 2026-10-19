"""Runs through run.py over SSH. All mutations are confined to the task root."""
import fcntl
import io
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import sys
import tarfile
import time
import xml.etree.ElementTree as ET

BASE = Path("/data/typinghero-game-loop")
SOURCE = BASE / "source"


def call(args, timeout=60, **kwargs):
    return subprocess.run(args, check=True, timeout=timeout, **kwargs)


def container(args, timeout):
    name = f"typinghero-loop-{os.getpid()}-{time.time_ns()}"
    try:
        process = subprocess.Popen(["docker", "run", "--name", name, "--rm", "--init", *args])
        deadline = time.monotonic() + timeout
        try:
            while True:
                try:
                    result = process.wait(timeout=5)
                    if result: raise RuntimeError(f"Task container exited with code {result}")
                    break
                except subprocess.TimeoutExpired:
                    space(BASE, 1)
                    if time.monotonic() >= deadline: raise TimeoutError("Task container timed out")
        finally:
            if process.poll() is None:
                subprocess.run(["docker", "rm", "-f", name], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                process.wait(timeout=60)
    finally:
        # Also stop the task's container if the Docker client timed out.
        subprocess.run(["docker", "rm", "-f", name], stdout=subprocess.DEVNULL,
                       stderr=subprocess.DEVNULL, timeout=60)


def space(path, gib):
    available = shutil.disk_usage(path).free / 1024**3
    if available < gib:
        raise RuntimeError(f"{path}: {available:.1f} GiB free, need {gib} GiB (short {gib-available:.1f} GiB)")


def verify_xml(path):
    if not path.is_file():
        raise RuntimeError("Unity did not produce test XML")
    root = ET.parse(path).getroot()
    cases = root.findall(".//test-case")
    if root.get("result") != "Passed" or not cases or any(c.get("result") != "Passed" for c in cases):
        raise RuntimeError(f"Unity tests failed/skipped or zero tests ran: {len(cases)} cases")
    print(f"PASS: {len(cases)} Unity tests", flush=True)


def main():
    profile, license_path, seconds = sys.argv[1:4]
    scenario = sys.argv[4] if len(sys.argv) > 4 else 'full'
    profile_allocations = len(sys.argv) > 5 and sys.argv[5] == '1'
    if profile_allocations and profile != 'webgl': raise ValueError('Allocation profiling requires WebGL QA')
    if scenario not in ('full', 'skeleton-art', 'attack-sorting', 'score-layout', 'pixel-sparks', 'pixel-sparks-parry', 'endless', 'combat-r7', 'parry-clash') or (scenario != 'full' and profile != 'webgl'):
        raise ValueError('Unsupported browser scenario/profile')
    BASE.mkdir(parents=True, exist_ok=True)
    with (BASE / "runner.lock").open("w") as lock:
        wait_seconds = int(sys.argv[6]) if len(sys.argv) > 6 else 0
        if not 0 <= wait_seconds <= 600: raise ValueError('Invalid runner lock wait')
        if wait_seconds:
            def lock_timeout(signum, frame):
                raise TimeoutError('Timed out waiting for shared runner lock')
            previous_alarm = signal.signal(signal.SIGALRM, lock_timeout)
            signal.alarm(wait_seconds)
            print('Waiting for shared runner lock (existing jobs preserved)', flush=True)
            try: fcntl.flock(lock, fcntl.LOCK_EX)
            finally:
                signal.alarm(0)
                signal.signal(signal.SIGALRM, previous_alarm)
        else:
            fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        # The source directory is task-owned; replace source, retain Library and toolchain.
        staging = BASE / "incoming"
        if staging.exists(): shutil.rmtree(staging)
        staging.mkdir()
        payload = sys.stdin.buffer.read()
        archive_path = BASE / "input-snapshot.tar.gz"
        archive_path.write_bytes(payload)
        archive_path.chmod(0o600)
        with tarfile.open(fileobj=io.BytesIO(payload), mode="r|gz") as tar:
            for member in tar:
                destination = (staging / member.name).resolve()
                if not destination.is_relative_to(staging.resolve()) or not member.isfile():
                    raise RuntimeError("Unsafe snapshot member")
                space(BASE, 1 + member.size / 1024**3)
                tar.extract(member, staging, filter="data")
        if SOURCE.exists(): shutil.rmtree(SOURCE)
        staging.rename(SOURCE)
        config = json.loads((SOURCE / "tools/game_loop/toolchain.json").read_text())
        unity_image = config["unity"]
        if "@sha256:" not in unity_image: raise RuntimeError("Unity digest is required")
        image_id_file = BASE / "unity-image-id.txt"
        unity_reference = unity_image
        if image_id_file.is_file():
            record = json.loads(image_id_file.read_text())
            if record["manifest"] == unity_image: unity_reference = record["imageId"]
        license_file = Path(license_path) if license_path != "-" else None
        home = BASE / "unity-home"
        license_files = ([license_file] if license_file else
            [home / ".local/share/unity3d/Unity/Unity_lic.ulf", *home.glob(".config/unity3d/Unity/licenses/*.xml")])
        license_present = any(file.is_file() for file in license_files)
        if license_file and license_file.resolve().is_relative_to(SOURCE.resolve()):
            raise RuntimeError("License must be outside source snapshot")
        print(json.dumps({"profile": profile, "snapshot": json.loads((SOURCE / "snapshot.json").read_text()),
                          "unity": unity_image, "freeGiB": round(shutil.disk_usage(BASE).free / 1024**3, 1),
                          "licensePresent": license_present}), flush=True)
        call(["docker", "version", "--format", "{{.Server.Version}}"])
        if profile == "doctor":
            for phase, required in config["minimumFreeGiB"].items():
                print(f"{phase}: requires {required} GiB", flush=True)
            missing = []
            for image in [unity_reference, config["playwright"]]:
                if subprocess.run(["docker", "image", "inspect", image], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode:
                    missing.append(image)
            if missing: print("Missing images: " + ", ".join(missing), flush=True)
            if not license_present or missing:
                raise RuntimeError("Doctor prerequisites missing; provision license and run setup")
            space(BASE, config["minimumFreeGiB"]["unity"])
            return
        if profile == "setup":
            space(BASE, config["minimumFreeGiB"]["setup"])
            unity_present = subprocess.run(["docker", "image", "inspect", unity_reference],
                stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode == 0
            if not unity_present:
                sys.path.insert(0, str(SOURCE / "tools/game_loop"))
                from image_size import measure
                from load_image import load
                measurement = BASE / "image-size.json"
                layout = BASE / "image-layout"
                if not measurement.is_file():
                    data = measure(unity_image, layout)
                    measurement.write_text(json.dumps(data, indent=2))
                if json.loads(measurement.read_text())["image"] != unity_image:
                    raise RuntimeError("Measured image does not match the configured Unity digest")
                image_id = load(layout, measurement)
                image_id_file.write_text(json.dumps({"manifest": unity_image, "imageId": image_id}))
            if subprocess.run(["docker", "image", "inspect", config["playwright"]],
                stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode:
                call(["docker", "pull", config["playwright"]], timeout=1800)
            # Persist the resolved browser digest as well; no mutable tag during tests.
            browser = subprocess.check_output(["docker", "image", "inspect", "--format", "{{index .RepoDigests 0}}", config["playwright"]], text=True).strip()
            (BASE / "browser-image.txt").write_text(browser)
            tooling = BASE / "toolchain"; tooling.mkdir(exist_ok=True)
            container(["--cpus", "2", "--memory", "8g", "-v", f"{tooling}:/tools", "-w", "/tools", browser,
                  "npm", "install", "--save-exact", "playwright@" + config["playwrightPackage"]], timeout=600)
            print("Toolchain ready; " + ("license state present" if license_present else "Unity activation is still required"), flush=True)
            return
        space(BASE, config["minimumFreeGiB"]["unity"])
        if not license_present: raise RuntimeError("Unity activation state missing from private HOME" if not license_file else "Explicit Unity license file missing: " + str(license_file))
        for name in ["library", "cache", "results", "build-qa", "build-production", "build-windows", "unity-home"]:
            (BASE / name).mkdir(exist_ok=True)
        result = BASE / "results" / (time.strftime("%Y%m%dT%H%M%S") + "-" + profile)
        result.mkdir()
        shutil.copyfile(SOURCE / "snapshot.json", result / "snapshot.json")

        def unity(extra, qa=False, build="build-qa", modules=()):
            space(BASE, config["minimumFreeGiB"]["unity"])
            container(["--cpus", "2", "--memory", "8g",
                  "--hostname", config["containerHostname"], "--mac-address", config["containerMacAddress"],
                  "--user", f"{os.getuid()}:{os.getgid()}", "-e", "HOME=/home/runner",
                  "--network", "bridge", "-v", f"{SOURCE}:/project", "-v", f"{BASE / 'library'}:/project/Library",
                  "-v", f"{BASE / 'unity-home'}:/home/runner", "-v", f"{BASE / 'cache'}:/home/runner/.cache", "-v", f"{result}:/results",
                  "-v", f"{BASE / build}:/output",
                  *(["-v", f"{license_file.resolve()}:/home/runner/.local/share/unity3d/Unity/Unity_lic.ulf:ro"] if license_file else []),
                  "-e", "TYPINGHERO_QA=" + ("1" if qa else "0"), "-e", "TYPINGHERO_BUILD_PATH=/output",
                  "-e", "TYPINGHERO_PROFILE_ALLOCATIONS=" + ("1" if profile_allocations else "0"),
                  *modules, unity_reference,
                  "unity-editor", "-batchmode", "-nographics", "-projectPath", "/project",
                  "-logFile", "/results/unity.log", *extra], timeout=5400)

        if profile == "serialize":
            unity(["-quit", "-buildTarget", "WebGL", "-executeMethod", "PixelUIBuild.SerializeOnly"], qa=True)
            for relative in ('Assets/Scenes/SampleScene.unity', 'Assets/Art/UI/CombatR7', 'Assets/Art/UI/CombatR7.meta'):
                target = result / 'serialized' / relative
                target.parent.mkdir(parents=True, exist_ok=True)
                if (SOURCE / relative).is_dir(): shutil.copytree(SOURCE / relative, target)
                else: shutil.copy2(SOURCE / relative, target)
            print(f"PASS: repeat builder and serialized assets: {result}", flush=True)
            return
        if profile == "windows":
            module = Path.home() / "typinghero-game-loop/toolchain/windows-mono-2022.3.25f1/WindowsStandaloneSupport"
            if not (module / "Variations/win64_player_nondevelopment_mono").is_dir():
                raise RuntimeError("Pinned Windows Mono module missing: " + str(module))
            unity(["-quit", "-buildTarget", "Win64", "-executeMethod", "GameLoopBuild.Windows64"],
                  build="build-windows", modules=["-v", f"{module}:/opt/unity/Editor/Data/PlaybackEngines/WindowsStandaloneSupport:ro"])
            build = BASE / "build-windows"
            for name in ["CLACKBLADE.exe", "UnityPlayer.dll", "CLACKBLADE_Data/Managed/TypingHero.Runtime.dll"]:
                if not (build / name).is_file(): raise RuntimeError("Windows build missing " + name)
            assembly = (build / "CLACKBLADE_Data/Managed/TypingHero.Runtime.dll").read_bytes()
            if b"GameLoopProbe" in assembly: raise RuntimeError("QA probe present in Windows build")
            print("PASS: Windows64 build complete; QA probe excluded", flush=True)
            return
        if profile in ("quick", "stress"):
            unity(["-runTests", "-testPlatform", "PlayMode", "-testResults", "/results/tests.xml",
                   "-testCategory", "Stress" if profile == "stress" else "Quick"])
            verify_xml(result / "tests.xml")
            if profile == "quick": return
        production = profile == "production"
        build = BASE / ("build-production" if production else "build-qa")
        shutil.rmtree(build)
        build.mkdir()
        unity(["-quit", "-buildTarget", "WebGL", "-executeMethod", "GameLoopBuild.WebGL"],
              qa=not production, build="build-production" if production else "build-qa")
        if not (build / "index.html").is_file(): raise RuntimeError("Build missing index.html")
        marker = json.loads((build / "game-loop-build.json").read_text())
        if marker["qa"] == production: raise RuntimeError("Build QA flag mismatch")
        if production:
            for path in build.rglob("*.js"):
                if "typingHeroQA" in path.read_text(errors="replace"):
                    raise RuntimeError("QA bridge present in production JavaScript")
            print("PASS: normal WebGL build excludes QA bridge", flush=True)
            shutil.copytree(build, result / 'player')
            print(f"Production player preserved: {result / 'player'}", flush=True)
            return
        if scenario.startswith('pixel-sparks') or scenario in ('endless', 'combat-r7', 'parry-clash'):
            # Preserve this exact player before releasing the shared runner lock.
            # Native QA must not race another task replacing build-qa.
            immutable = result / 'player'
            shutil.copytree(build, immutable)
            for relative in ('Assets/Scenes/MainMenu.unity', 'Assets/Scenes/SampleScene.unity',
                             'Assets/Characters/Player/Player.prefab', 'Assets/Characters/Enermy/Skeleton.prefab',
                             'Assets/Art/UI/CombatR7', 'Assets/Art/UI/CombatR7.meta',
                             'Assets/Art/VFX/PixelSparks', 'Assets/Art/VFX/PixelSparks.meta',
                             'Assets/Art/Lighting', 'Assets/Art/Lighting.meta',
                             'Assets/Art/VFX/ParryClash', 'Assets/Art/VFX/ParryClash.meta'):
                target = result / 'serialized' / relative
                target.parent.mkdir(parents=True, exist_ok=True)
                if (SOURCE / relative).is_dir(): shutil.copytree(SOURCE / relative, target)
                else: shutil.copy2(SOURCE / relative, target)
            for meta in (SOURCE / 'Assets/Art').rglob('*.png.meta'):
                if b'_NormalMap' in meta.read_bytes():
                    target = result / 'serialized' / meta.relative_to(SOURCE)
                    target.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copy2(meta, target)
            build = immutable
            print(f"QA player preserved: {build}", flush=True)
        space(BASE, config["minimumFreeGiB"]["browser"])
        browser = (BASE / "browser-image.txt").read_text().strip()
        container(["--cpus", "2", "--memory", "8g", "--shm-size", config["browserSharedMemory"], "--network", "none",
              *(['-e', 'TYPINGHERO_SKELETON_ART_ONLY=1'] if scenario == 'skeleton-art' else []),
              *(['-e', 'TYPINGHERO_ENDLESS_ONLY=1'] if scenario == 'endless' else []),
              *(['-e', 'TYPINGHERO_HUD_R7_ONLY=1'] if scenario == 'combat-r7' else []),
              *(['-e', 'TYPINGHERO_SCORE_LAYOUT_ONLY=1'] if scenario == 'score-layout' else []),
              *(['-e', 'TYPINGHERO_ATTACK_SORTING_ONLY=1'] if scenario == 'attack-sorting' else []),
              *(['-e', 'TYPINGHERO_PIXEL_SPARKS_ONLY=1'] if scenario in ('pixel-sparks', 'pixel-sparks-parry', 'parry-clash') else []),
              *(['-e', 'TYPINGHERO_PARRY_CLASH_ONLY=1'] if scenario == 'parry-clash' else []),
              *(['-e', 'TYPINGHERO_PIXEL_PARRY_ONLY=1'] if scenario == 'pixel-sparks-parry' else []),
              "-v", f"{BASE / 'toolchain'}:/tools", "-v", f"{SOURCE / 'tools/game_loop'}:/runner:ro",
              "-v", f"{build}:/build:ro", "-v", f"{result}:/results", browser,
              "node", "/runner/browser.cjs", profile, seconds], timeout=int(seconds) + 900)
        report = result / "browser.json"
        if not report.is_file(): raise RuntimeError("Browser results missing")
        data = json.loads(report.read_text())
        if data.get("status") != "passed" or data.get("checks", 0) < 1:
            raise RuntimeError("Browser profile failed or zero checks")
        print(f"PASS {profile}: {report}", flush=True)


if __name__ == "__main__":
    try: main()
    except (Exception, KeyboardInterrupt) as error:
        print(f"FAIL: {error}", file=sys.stderr)
        sys.exit(1)
