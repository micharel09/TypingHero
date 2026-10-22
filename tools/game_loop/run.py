"""Snapshot this working tree and execute the isolated Ubuntu game-loop runner."""
import argparse
import hashlib
import io
import json
from pathlib import Path
import shlex
import subprocess
import sys
import tarfile

ROOT = Path(__file__).resolve().parents[2]
REMOTE = "/data/typinghero-game-loop"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("profile", choices=["doctor", "setup", "serialize", "quick", "webgl", "stress", "soak", "production", "windows"])
    parser.add_argument("--host", default="fly-server")
    parser.add_argument("--ssh-option", action="append", default=[], help="Verified per-run SSH option, passed with -o")
    parser.add_argument("--license", help="Optional existing server ULF; default uses the activated private Unity HOME")
    parser.add_argument("--wait-seconds", type=int, default=0, help="Wait up to this many seconds for the existing exclusive runner lock")
    parser.add_argument("--seconds", type=int, default=1800)
    parser.add_argument("--profile-allocations", action="store_true", help="Development QA player with per-frame allocation counters (WebGL only)")
    parser.add_argument("--browser-scenario", choices=["tutorial", "skeleton-art", "attack-sorting", "score-layout", "score-cluster", "pixel-sparks", "pixel-sparks-parry", "endless", "combat-r7", "parry-clash", "camera-impact"], help="Targeted WebGL acceptance using the existing isolated harness")
    args = parser.parse_args()
    if not 0 <= args.wait_seconds <= 3600:
        parser.error('--wait-seconds must be between 0 and 3600')
    if args.browser_scenario and args.profile != 'webgl':
        parser.error('--browser-scenario requires webgl')
    if args.profile_allocations and args.profile != 'webgl':
        parser.error('--profile-allocations requires webgl')
    if args.seconds < 1800:
        parser.error("soak acceptance requires at least 1800 real seconds")
    if not (ROOT / "Assets/Scripts/Core/TypingManager/TypingManager.cs").is_file():
        parser.error("not the TypingHero repository")
    # git's file list includes dirty tracked files and nonignored new files.
    names = subprocess.check_output(["git", "ls-files", "-z", "--cached", "--others", "--exclude-standard"], cwd=ROOT)
    archive = io.BytesIO()
    digest = hashlib.sha256()
    with tarfile.open(fileobj=archive, mode="w:gz") as tar:
        for name in sorted(set(names.decode().split("\0")) - {""}):
            path = ROOT / name
            # Only build inputs/tooling; never synchronize local secrets or design captures.
            if not (name.startswith(("Assets/", "Packages/", "ProjectSettings/", "tools/game_loop/"))):
                continue
            if path.is_symlink() or not path.is_file():
                continue
            if path.suffix.lower() in {".ulf", ".pem", ".key", ".pfx"} or path.name.startswith(".env"):
                continue
            data = path.read_bytes()
            digest.update(name.encode() + b"\0" + data)
            tar.add(path, arcname=name, recursive=False)
        marker = json.dumps({"sha256": digest.hexdigest()}).encode()
        info = tarfile.TarInfo("snapshot.json"); info.size = len(marker)
        tar.addfile(info, io.BytesIO(marker))
    # Server helper takes the lock before unpacking; concurrent clients cannot mix snapshots.
    helper = (ROOT / "tools/game_loop/server.py").read_text(encoding="utf-8")
    command = "python3 -c " + shlex.quote(helper) + " " + " ".join(map(shlex.quote, [args.profile, args.license or "-", str(args.seconds), args.browser_scenario or 'full', '1' if args.profile_allocations else '0', str(args.wait_seconds)]))
    ssh_options = [item for option in args.ssh_option for item in ("-o", option)]
    completed = subprocess.run(["ssh", "-o", "BatchMode=yes", "-o", "ConnectTimeout=10", *ssh_options, args.host, command], input=archive.getvalue())
    return completed.returncode


if __name__ == "__main__":
    sys.exit(main())
