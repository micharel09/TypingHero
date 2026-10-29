// Install before player startup. Read-only observer; export after measurement.
(() => {
  const live = new Map(), ids = new WeakMap(), events = [];
  const cadence = new Float64Array(16384);
  let serial = 0, count = 0, overflow = false, active = true;
  let peakCount = 0, peakBytes = 0;
  const snapshot = () => {
    let bytes = 0;
    for (const texture of live.values()) bytes += texture.bytes;
    return { count: live.size, bytes };
  };
  const raf = time => {
    if (!active) return;
    if (count < cadence.length) cadence[count++] = time;
    else overflow = true;
    requestAnimationFrame(raf);
  };
  requestAnimationFrame(raf);
  for (const proto of [WebGLRenderingContext.prototype, WebGL2RenderingContext.prototype]) {
    for (const name of ['texImage2D', 'texStorage2D', 'deleteTexture']) {
      if (!Object.prototype.hasOwnProperty.call(proto, name)) continue;
      const original = proto[name];
      proto[name] = function(...args) {
        const start = performance.now();
        let id, width, height, tutorial = false;
        if (name === 'deleteTexture') {
          id = ids.get(args[0]); tutorial = live.has(id);
        } else {
          const texture = this.getParameter(this.TEXTURE_BINDING_2D);
          if (texture) {
            id = ids.get(texture);
            if (!id) { id = ++serial; ids.set(texture, id); }
          }
          if (name === 'texStorage2D') { width = args[3]; height = args[4]; }
          else if (args.length >= 9) { width = args[3]; height = args[4]; }
          else { width = args[5]?.width; height = args[5]?.height; }
          tutorial = id !== undefined && args[0] === this.TEXTURE_2D && width === 1944 && height % 184 === 0;
        }
        const result = original.apply(this, args);
        const end = performance.now();
        if (tutorial) {
          if (name === 'deleteTexture') live.delete(id);
          else live.set(id, { bytes: width * height * 4 });
          const current = snapshot();
          peakCount = Math.max(peakCount, current.count);
          peakBytes = Math.max(peakBytes, current.bytes);
          events.push({ name, id, width, height, start, end, ...current });
        }
        return result;
      };
    }
  }
  window.tutorialStreamingObserver = {
    snapshot,
    read() { return { events: events.slice(), cadence: Array.from(cadence.subarray(0, count)),
      overflow, peakCount, peakBytes, ...snapshot(), now: performance.now() }; },
    resetCadence() { count = 0; overflow = false; },
    stop() { active = false; }
  };
})();
