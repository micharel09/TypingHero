"""Activate on fly-server inside a trusted, ephemeral Actions job using existing secrets."""
import json
import os
from pathlib import Path
import shutil
import subprocess

BASE = Path('/data/typinghero-game-loop')


def main():
    source = BASE / 'source'
    config = json.loads((source / 'tools/game_loop/toolchain.json').read_text())
    image = config['unity']
    image_id_file = BASE / 'unity-image-id.txt'
    if image_id_file.is_file():
        record = json.loads(image_id_file.read_text())
        if record['manifest'] != image: raise RuntimeError('Activated image must match the configured Unity manifest')
        image = record['imageId']
    home = BASE / 'unity-home'
    home.mkdir(mode=0o700, exist_ok=True)
    home.chmod(0o700)
    env = os.environ.copy()
    env['HOME'] = str(home)
    # GameCI's VCS middleware requires a repository even for activation. This
    # disposable snapshot contains no checkout metadata or source commits.
    if not (source / '.git').exists():
        subprocess.run(['git', 'init', '--quiet', str(source)], check=True)
        subprocess.run(['git', '-c', 'user.name=TypingHero automation', '-c',
                        'user.email=automation@localhost', 'commit', '--quiet',
                        '--allow-empty', '-m', 'Activation snapshot'], cwd=source, check=True)
    # ActivateCommand in the pinned CLI does not register build/test resource flags.
    # Constrain only Docker invocations launched by this activation subprocess.
    docker_binary = shutil.which('docker')
    if not docker_binary: raise RuntimeError('Docker CLI is missing')
    bin_dir = BASE / 'toolchain/activation-bin'
    bin_dir.mkdir(mode=0o700, exist_ok=True)
    wrapper = bin_dir / 'docker'
    wrapper.write_text('#!/usr/bin/python3\nimport os,sys\nargs=sys.argv[1:]\n'
                       f'if args and args[0]=="run": args=["run","--cpus","2","--memory","8g",'
                       f'"--hostname",{config["containerHostname"]!r},"--mac-address",{config["containerMacAddress"]!r}]+args[1:]\n'
                       f'os.execv({docker_binary!r},["docker",*args])\n')
    wrapper.chmod(0o700)
    env['PATH'] = str(bin_dir) + os.pathsep + env['PATH']
    if not env.get('UNITY_LICENSE') and not (env.get('UNITY_EMAIL') and env.get('UNITY_PASSWORD')):
        raise RuntimeError('Unity credentials are missing from the trusted Actions environment')
    try:
        subprocess.run([str(BASE / 'toolchain/game-ci'), 'activate', '.', '--custom-image', image,
                        '--chown-files-to', f'{os.getuid()}:{os.getgid()}'],
                       cwd=source, env=env, check=True, timeout=900)
    finally:
        subprocess.run(['sudo', '-n', 'chown', '-R', f'{os.getuid()}:{os.getgid()}', str(home)], check=True)
    # The CLI's Linux activation container mounts this dedicated HOME at /root.
    license_files = [home / '.local/share/unity3d/Unity/Unity_lic.ulf',
                     *home.glob('.config/unity3d/Unity/licenses/*.xml')]
    present = [file for file in license_files if file.is_file()]
    if not present: raise RuntimeError('Activation returned without persistent Unity licensing state')
    for file in present: file.chmod(0o600)
    print('Unity activated; license retained only in the private task HOME, outside source.')


if __name__ == '__main__':
    main()
