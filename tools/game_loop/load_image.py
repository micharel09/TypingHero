"""Import verified OCI layer cache without retaining a second compressed copy."""
import json
from pathlib import Path
import shutil
import subprocess
import tarfile


def load(layout, measurement, reserve_gib=3):
    layout = Path(layout).resolve()
    data = json.loads(Path(measurement).read_text())
    tag = data['image'].split('@')[0]
    canonical = 'docker.io/' + (tag if '/' in tag else 'library/' + tag)
    expected = data['image'].split('@')[1]
    index_file = layout / 'index.json'
    index = json.loads(index_file.read_text())
    if len(index['manifests']) != 1 or index['manifests'][0]['digest'] != expected:
        raise RuntimeError('OCI index must reference exactly the pinned image manifest')
    # Match Docker 29's own OCI export naming. Short ref.name alone imports an
    # unnormalized tag which Docker's normal lookup cannot resolve.
    index['manifests'][0]['annotations'] = {
        'io.containerd.image.name': canonical,
        'org.opencontainers.image.ref.name': tag.rsplit(':', 1)[1]
    }
    index_file.write_text(json.dumps(index))
    available = shutil.disk_usage(layout).free
    # Cache bytes are released as Docker consumes the archive. Containerd retains
    # compressed content as well as the unpacked snapshot; account for both.
    required = data['unpacked'] + reserve_gib * 1024**3
    if available < required:
        raise RuntimeError(f"Image import: {available/1024**3:.2f} GiB free; needs "
                           f"{required/1024**3:.2f} GiB including {reserve_gib} GiB reserve")
    entries = sorted(layout.rglob('*'), key=lambda p: p.stat().st_size if p.is_file() else 0, reverse=True)
    process = subprocess.Popen(['docker', 'image', 'load'], stdin=subprocess.PIPE)
    try:
        with tarfile.open(fileobj=process.stdin, mode='w|') as archive:
            for file in entries:
                if file.is_symlink() or not file.is_file(): continue
                if not file.resolve().is_relative_to(layout): raise RuntimeError('Unsafe image cache path')
                info = archive.gettarinfo(str(file), arcname=file.relative_to(layout).as_posix())
                with file.open('rb') as stream: archive.addfile(info, stream)
                # This directory contains only recreatable, digest-verified image blobs.
                # The transport copy is retained until the import has succeeded.
                if file.parent.name == 'sha256' and any(x['digest'].endswith(file.name) for x in data['layers']):
                    file.unlink()
        process.stdin.close()
        if process.wait(timeout=600): raise RuntimeError('Docker rejected the OCI image archive')
    except BaseException:
        process.stdin.close()
        if process.poll() is None: process.terminate()
        process.wait(timeout=60)
        raise
    actual = subprocess.check_output(['docker', 'image', 'inspect', '--format', '{{.Id}}', canonical], text=True).strip()
    if actual != expected:
        raise RuntimeError('Loaded Unity image ID does not match the pinned manifest (Docker 29 required)')
    return actual
