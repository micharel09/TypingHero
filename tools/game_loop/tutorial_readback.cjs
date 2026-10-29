// Adapted from MDN WebGL best practices: non-blocking async data readback.
// Reuses the tutorial harness default-framebuffer source, without PNG work during capture.
module.exports = function installTutorialReadback({ seconds = 24, fps = 30 } = {}) {
  if (seconds <= 0 || seconds > 24 || fps !== 30) throw Error('Capture requires 30 fps, at most 24 seconds');
  const canvas = document.querySelector('#unity-canvas');
  const gl = canvas.getContext('webgl2');
  if (!gl || canvas.width !== 640 || canvas.height !== 360) throw Error('Expected 640x360 WebGL2 canvas');
  const width = 640, height = 360, bytes = width * height * 4;
  const capacity = Math.min(Math.ceil(seconds * fps) + 3, Math.floor(768 * 1024 * 1024 / bytes) - 3);
  // Keep pre-existing player GL errors separate from errors introduced by capture.
  const initialErrors = [];
  for (let i = 0; i < 16; i++) {
    const code = gl.getError();
    if (code === gl.NO_ERROR) break;
    initialErrors.push(code);
  }
  const frames = [], pending = [], slots = [], storage = [];
  let started, nextAt, raf, stopping = false, done = false, error = null, latest = null;
  let requested = 0, allocated = 0, stoppedAt;
  let callbacks = 0, callbackMs = 0, maxCallbackMs = 0;
  const packNames = [gl.PACK_ALIGNMENT, gl.PACK_ROW_LENGTH, gl.PACK_SKIP_PIXELS, gl.PACK_SKIP_ROWS];
  function withState(action) {
    const buffer = gl.getParameter(gl.PIXEL_PACK_BUFFER_BINDING);
    const framebuffer = gl.getParameter(gl.READ_FRAMEBUFFER_BINDING);
    const readBuffer = gl.getParameter(gl.READ_BUFFER);
    const pack = packNames.map(name => gl.getParameter(name));
    try {
      gl.bindFramebuffer(gl.READ_FRAMEBUFFER, null);
      gl.readBuffer(gl.BACK);
      packNames.forEach((name, i) => gl.pixelStorei(name, i === 0 ? 1 : 0));
      return action();
    } finally {
      gl.bindBuffer(gl.PIXEL_PACK_BUFFER, buffer);
      packNames.forEach((name, i) => gl.pixelStorei(name, pack[i]));
      gl.bindFramebuffer(gl.READ_FRAMEBUFFER, framebuffer);
      gl.readBuffer(readBuffer);
    }
  }
  function cleanup() {
    for (const slot of slots) {
      if (slot.sync) gl.deleteSync(slot.sync);
      gl.deleteBuffer(slot.buffer);
      slot.sync = null;
    }
    canvas.removeEventListener('webglcontextlost', lost);
    cancelAnimationFrame(raf);
    done = true;
  }
  function fail(reason) { error = reason; stopping = true; cleanup(); }
  function lost() { fail('WebGL context lost'); }
  try {
    withState(() => {
      for (let i = 0; i < 3; i++) {
        const buffer = gl.createBuffer();
        if (!buffer) throw Error('PBO allocation failed');
        slots.push({ buffer, sync: null });
        gl.bindBuffer(gl.PIXEL_PACK_BUFFER, buffer);
        gl.bufferData(gl.PIXEL_PACK_BUFFER, bytes, gl.STREAM_READ);
      }
    });
    // Allocate all CPU destinations before the first timed request, avoiding external-memory GC during capture.
    for (let i = 0; i < capacity; i++) storage.push(new Uint8Array(bytes));
    const allocationError = gl.getError();
    if (allocationError !== gl.NO_ERROR) throw Error('PBO allocation error: ' + allocationError);
  } catch (e) { cleanup(); throw e; }
  canvas.addEventListener('webglcontextlost', lost);
  function tick(now) {
    const entered = performance.now();
    try {
      if (gl.isContextLost()) return fail('WebGL context lost');
      withState(() => {
        // Drain in request order. A later ready fence cannot reorder frames.
        while (pending.length) {
          const slot = pending[0];
          const status = gl.clientWaitSync(slot.sync, 0, 0);
          if (status === gl.WAIT_FAILED) throw Error('Fence wait failed');
          if (status === gl.TIMEOUT_EXPIRED) break;
          gl.bindBuffer(gl.PIXEL_PACK_BUFFER, slot.buffer);
          gl.getBufferSubData(gl.PIXEL_PACK_BUFFER, 0, slot.pixels);
          gl.deleteSync(slot.sync); slot.sync = null;
          latest = { timestamp: slot.timestamp, pixels: slot.pixels };
          frames.push(latest); pending.shift(); slot.pixels = null;
        }
        if (started === undefined) { started = now; nextAt = now; }
        if (now - started >= seconds * 1000) stopping = true;
        if (!stopping && now >= nextAt - .5) {
          const slot = slots.find(item => !item.sync);
          if (!slot) throw Error('All three PBOs occupied; capture rejected');
          if (allocated >= capacity) throw Error('RGBA memory budget exhausted');
          slot.pixels = storage[allocated++];
          gl.bindBuffer(gl.PIXEL_PACK_BUFFER, slot.buffer);
          slot.timestamp = performance.now() / 1000;
          gl.readPixels(0, 0, width, height, gl.RGBA, gl.UNSIGNED_BYTE, 0);
          slot.sync = gl.fenceSync(gl.SYNC_GPU_COMMANDS_COMPLETE, 0);
          if (!slot.sync) throw Error('Fence allocation failed');
          pending.push(slot); requested++;
          // Do not fabricate catch-up frames when the renderer misses a deadline.
          nextAt += 1000 / fps;
          if (nextAt <= now) nextAt = now + 1000 / fps;
        }
      });
      if (stopping && stoppedAt === undefined) stoppedAt = now;
      if (stopping && pending.length && now - stoppedAt > 5000) throw Error('Readback drain timed out');
      if (stopping && pending.length === 0) {
        if (!requested || frames.length !== requested) throw Error('Missing readback frames');
        if (gl.getError() !== gl.NO_ERROR) throw Error('WebGL readback error');
        cleanup(); return;
      }
      raf = requestAnimationFrame(tick);
    } catch (e) { fail(e.message); }
    finally {
      const elapsed = performance.now() - entered;
      callbacks++; callbackMs += elapsed; maxCallbackMs = Math.max(maxCallbackMs, elapsed);
    }
  }
  raf = requestAnimationFrame(tick);
  let fullSurface, wordSurface;
  function image(frame, region = { x: 0, y: 0, width, height }) {
    if (!frame) throw Error('No completed frame');
    let surface = region.width === width ? fullSurface : wordSurface;
    if (!surface) {
      const output = document.createElement('canvas');
      output.width = region.width; output.height = region.height;
      const ctx = output.getContext('2d');
      surface = { output, ctx, data: ctx.createImageData(region.width, region.height) };
      if (region.width === width) fullSurface = surface;
      else wordSurface = surface;
    }
    const { output, ctx, data } = surface;
    for (let y = 0; y < region.height; y++) {
      const offset = ((height - 1 - (region.y + y)) * width + region.x) * 4;
      data.data.set(frame.pixels.subarray(offset, offset + region.width * 4), y * region.width * 4);
    }
    ctx.putImageData(data, 0, 0);
    return output.toDataURL('image/png');
  }
  // Same classifier as the original capture driver's read-png.cjs, applied to RAM RGBA.
  function pale(rect, threshold = [180, 175, 155], frame = latest) {
    if (!frame) return 0;
    let count = 0, total = 0;
    for (let y = Math.max(0, Math.floor(rect.y)); y < Math.min(height, Math.ceil(rect.y + rect.height)); y++)
      for (let x = Math.max(0, Math.floor(rect.x)); x < Math.min(width, Math.ceil(rect.x + rect.width)); x++) {
        const i = ((height - 1 - y) * width + x) * 4;
        total++;
        if (frame.pixels[i] > threshold[0] && frame.pixels[i + 1] > threshold[1] && frame.pixels[i + 2] > threshold[2]) count++;
      }
    return count / Math.max(1, total);
  }
  async function wordSignature(frame) {
    if (!frame) throw Error('No completed frame');
    // Original read-png signature: first-word white-pixel mask, SHA256.
    const mask = new Uint8Array(120 * 26);
    let at = 0;
    for (let y = 266; y < 292; y++) for (let x = 130; x < 250; x++) {
      const i = ((height - 1 - y) * width + x) * 4;
      mask[at++] = frame.pixels[i] > 100 && frame.pixels[i + 1] > 100 && frame.pixels[i + 2] > 80 ? 1 : 0;
    }
    const digest = new Uint8Array(await crypto.subtle.digest('SHA-256', mask));
    return Array.from(digest, value => value.toString(16).padStart(2, '0')).join('');
  }
  window.tutorialReadback = {
    stop() { stopping = true; },
    metrics() { return { timestamp: latest?.timestamp,
      hp: pale({ x: 38, y: 45, width: 90, height: 5 }, [100, 20, 30]),
      enemyHP: pale({ x: 534, y: 44, width: 91, height: 5 }, [100, 20, 30]),
      guard: pale({ x: 526, y: 76, width: 100, height: 4 }, [60, 120, 110]),
      cue: pale({ x: 354, y: 98, width: 52, height: 18 }, [120, 110, 100]),
      slayerRed: pale({ x: 80, y: 140, width: 40, height: 40 }, [80, -1, -1]),
      timer: pale({ x: 130, y: 304, width: 380, height: 2 }, [40, 90, 80]) }; },
    status() { return { done, error, initialErrors, requested, callbacks, callbackMs, maxCallbackMs, completed: frames.length, bytes: capacity * bytes + 3 * bytes,
      timestamps: frames.map(frame => frame.timestamp), width, height }; },
    // OCR callers may request only the word strip from the latest completed RAM frame.
    async wordSample() {
      const frame = latest;
      const signature = await wordSignature(frame);
      return { timestamp: frame.timestamp, signature,
        timer: pale({ x: 130, y: 304, width: 380, height: 2 }, [40, 90, 80], frame),
        png: image(frame, { x: 130, y: 265, width: 380, height: 32 }) };
    },
    wordSignature() { return wordSignature(latest); },
    wordStrip() { return image(latest, { x: 130, y: 265, width: 380, height: 32 }); },
    png(index) { if (!done || error || frames.length !== requested) throw Error(error || 'Capture incomplete'); return image(frames[index]); },
    release() { stopping = true; if (!done) cleanup(); frames.length = 0; storage.length = 0; pending.length = 0; latest = null;
      for (const surface of [fullSurface, wordSurface]) if (surface) surface.output.width = 0;
      fullSurface = wordSurface = null; }
  };
};
