"""Measure pinned Docker Hub layers without writing image layers to disk."""
import gzip
import hashlib
import json
import io
import shutil
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path
import sys
import urllib.request


class LayerReader(io.RawIOBase):
    """Bounded range prefetch avoids a single slow CDN connection for multi-GB layers."""
    def __init__(self, url, size, output=None):
        self.url, self.size = url, size
        self.chunk_size = 8 * 1024 * 1024
        self.executor = ThreadPoolExecutor(max_workers=8)
        self.offset = 0
        self.futures = []
        self.buffer = memoryview(b'')
        self.output = output
        self.hasher = hashlib.sha256()
        for _ in range(8): self.schedule()

    def schedule(self):
        if self.offset < self.size:
            start = self.offset
            end = min(start + self.chunk_size, self.size) - 1
            self.futures.append(self.executor.submit(self.fetch, start, end))
            self.offset = end + 1

    def fetch(self, start, end):
        request = urllib.request.Request(self.url, headers={'Range': f'bytes={start}-{end}'})
        with urllib.request.urlopen(request, timeout=120) as response:
            if response.status != 206:
                raise RuntimeError('Registry CDN did not honor the bounded range request')
            data = response.read(end - start + 2)
        if len(data) != end - start + 1:
            raise RuntimeError('Registry layer range was truncated')
        return data

    def readable(self): return True

    def readinto(self, target):
        if not self.buffer:
            if not self.futures: return 0
            self.buffer = memoryview(self.futures.pop(0).result())
            self.schedule()
        length = min(len(target), len(self.buffer))
        target[:length] = self.buffer[:length]
        self.hasher.update(self.buffer[:length])
        if self.output: self.output.write(self.buffer[:length])
        self.buffer = self.buffer[length:]
        return length

    def close(self):
        self.executor.shutdown(wait=True, cancel_futures=True)
        super().close()


def measure(image, cache=None):
    repository, digest = image.split("@", 1)
    repository = repository.split(":", 1)[0]
    token_url = "https://auth.docker.io/token?service=registry.docker.io&scope=repository:" + repository + ":pull"
    with urllib.request.urlopen(token_url, timeout=60) as response:
        token = json.load(response)["token"]

    def request(url):
        return urllib.request.urlopen(urllib.request.Request(url, headers={
            "Authorization": "Bearer " + token,
            "Accept": "application/vnd.oci.image.manifest.v1+json,application/vnd.docker.distribution.manifest.v2+json"
        }), timeout=120)

    base = "https://registry-1.docker.io/v2/" + repository
    with request(base + "/manifests/" + digest) as response:
        raw = response.read()
    if "sha256:" + hashlib.sha256(raw).hexdigest() != digest:
        raise RuntimeError("Registry manifest digest mismatch")
    manifest = json.loads(raw)
    if cache:
        cache = Path(cache)
        blobs = cache / 'blobs/sha256'
        blobs.mkdir(parents=True, exist_ok=True)
        (cache / 'oci-layout').write_text('{"imageLayoutVersion":"1.0.0"}')
        (blobs / digest.split(':')[1]).write_bytes(raw)
        (cache / 'index.json').write_text(json.dumps({'schemaVersion': 2, 'manifests': [{
            'mediaType': manifest['mediaType'], 'digest': digest, 'size': len(raw),
            'annotations': {'org.opencontainers.image.ref.name': image.split('@')[0]}
        }]}))
        config_digest = manifest['config']['digest']
        with request(base + '/blobs/' + config_digest) as response:
            config_data = response.read()
        if 'sha256:' + hashlib.sha256(config_data).hexdigest() != config_digest:
            raise RuntimeError('Image configuration digest mismatch')
        (blobs / config_digest.split(':')[1]).write_bytes(config_data)
    sizes = []
    for index, layer in enumerate(manifest["layers"]):
        cached = blobs / layer['digest'].split(':')[1] if cache else None
        if cached and cached.is_file() and cached.stat().st_size == layer['size']:
            digest_check = hashlib.sha256()
            with cached.open('rb') as file:
                while block := file.read(8 * 1024 * 1024): digest_check.update(block)
            if 'sha256:' + digest_check.hexdigest() == layer['digest']:
                with gzip.open(cached, 'rb') as file:
                    count = 0
                    while block := file.read(8 * 1024 * 1024): count += len(block)
                sizes.append({'digest': layer['digest'], 'compressed': layer['size'], 'unpacked': count})
                print(f'Reused verified layer {index+1}/{len(manifest["layers"])}', flush=True)
                continue
        if cache and shutil.disk_usage(cache).free < layer['size'] + 1024**3:
            raise RuntimeError('Insufficient space to cache the next compressed image layer plus 1 GiB reserve')
        with request(base + "/blobs/" + layer["digest"]) as response:
            url = response.geturl()
        output = (blobs / layer['digest'].split(':')[1]).open('wb') if cache else None
        reader = LayerReader(url, layer['size'], output)
        with io.BufferedReader(reader) as chunks:
            stream = gzip.GzipFile(fileobj=chunks)
            count = 0
            while True:
                chunk = stream.read(8 * 1024 * 1024)
                if not chunk: break
                count += len(chunk)
                if count % (512 * 1024 * 1024) == 0:
                    print(f'Layer {index+1}: measured {count / 1024**3:.1f} GiB', flush=True)
        if output: output.close()
        if 'sha256:' + reader.hasher.hexdigest() != layer['digest']:
            raise RuntimeError('Image layer digest mismatch')
        sizes.append({"digest": layer["digest"], "compressed": layer["size"], "unpacked": count})
        print(f"Measured layer {index+1}/{len(manifest['layers'])}: {count / 1024**3:.2f} GiB unpacked", flush=True)
    return {"image": image, "layers": sizes,
            "compressed": sum(x["compressed"] for x in sizes),
            "unpacked": sum(x["unpacked"] for x in sizes)}


if __name__ == "__main__":
    image, output, *cache = sys.argv[1:]
    result = measure(image, cache[0] if cache else None)
    Path(output).write_text(json.dumps(result, indent=2))
    print(json.dumps({k: v for k, v in result.items() if k != "layers"}), flush=True)
