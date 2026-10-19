"""Run the captured working-tree snapshot locally on the approved fly-server."""
import argparse
from pathlib import Path
import subprocess
import sys

BASE = Path('/data/typinghero-game-loop')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('profile', choices=['doctor','setup','quick','webgl','stress','soak','production'])
    args = parser.parse_args()
    archive = BASE / 'input-snapshot.tar.gz'
    if not sys.platform.startswith('linux') or not archive.is_file():
        parser.error('Capture this working tree with run.py on fly-server first')
    # server.py replaces only its task snapshot; preserve the captured inputs
    # across profiles instead of snapshotting Unity-generated modifications.
    completed = subprocess.run([sys.executable, str(BASE/'source/tools/game_loop/server.py'),
                                args.profile, '-', '1800'], input=archive.read_bytes())
    return completed.returncode


if __name__ == '__main__': sys.exit(main())
