(function (scope) {
  'use strict';

  // Adapted from the existing parry preview timer: generation tokens cancel stale callbacks.
  function createController(clips, clock, onChange, loopPauseMs = 450) {
    let clipIndex = 0, frameIndex = 0, timeMs = 0, speed = 1, mode = 'stopped';
    let timer = null, generation = 0;
    const starts = clip => clip.holds.map((_, i) => clip.holds.slice(0, i).reduce((a, b) => a + b, 0));
    function cancel() {
      generation++;
      if (timer !== null) clock.clearTimeout(timer);
      timer = null;
    }
    function snapshot() { return { clipIndex, frameIndex, timeMs, speed, mode }; }
    function emit() { onChange(snapshot()); }
    function schedule(delay, callback) {
      const active = generation;
      timer = clock.setTimeout(() => {
        if (generation !== active) return;
        timer = null; callback();
      }, delay);
    }
    function advance() {
      const clip = clips[clipIndex];
      frameIndex++;
      timeMs = frameIndex < clip.holds.length ? starts(clip)[frameIndex] : clip.endMs;
      if (frameIndex < clip.holds.length) {
        emit(); schedule(clip.holds[frameIndex] / speed, advance);
      } else if (mode === 'loop') {
        mode = 'pause'; emit();
        schedule(loopPauseMs, () => {
          frameIndex = 0; timeMs = 0; mode = 'loop'; emit();
          schedule(clip.holds[0] / speed, advance);
        });
      } else { mode = 'finished'; emit(); }
    }
    function seek(value) {
      cancel();
      const clip = clips[clipIndex];
      timeMs = Math.max(0, Math.min(clip.endMs, value));
      frameIndex = starts(clip).filter(t => t <= timeMs + 1e-8).length - 1;
      if (timeMs >= clip.endMs) frameIndex = clip.holds.length;
      mode = 'stopped'; emit();
    }
    return {
      snapshot,
      play(loop) {
        cancel(); frameIndex = 0; timeMs = 0; mode = loop ? 'loop' : 'once'; emit();
        schedule(clips[clipIndex].holds[0] / speed, advance);
      },
      stop() { cancel(); mode = 'stopped'; emit(); },
      seek,
      step(delta) {
        const clip = clips[clipIndex];
        const index = Math.max(0, Math.min(clip.holds.length, frameIndex + delta));
        seek(index === clip.holds.length ? clip.endMs : starts(clip)[index]);
      },
      select(index) {
        cancel(); clipIndex = index; frameIndex = 0; timeMs = 0; mode = 'stopped'; emit();
      },
      setSpeed(value) { cancel(); speed = value; mode = 'stopped'; emit(); }
    };
  }

  if (typeof module !== 'undefined') module.exports = { createController };
  if (!scope.document) return;
  const data = scope.PARRY_PREVIEW, byId = id => document.getElementById(id);
  async function load(src) {
    const image = new Image(); image.src = src;
    await image.decode(); return image;
  }
  async function init() {
    const [source, idle] = await Promise.all([load(data.source), load(data.idle)]);
    const clipSelect = byId('clip'), scrub = byId('scrub');
    data.clips.forEach((clip, i) => clipSelect.add(new Option(clip.label, i)));
    let controller;
    function draw(canvas, image, index) {
      const context = canvas.getContext('2d');
      context.clearRect(0, 0, 128, 128); context.save(); context.imageSmoothingEnabled = false;
      if (byId('direction').value === 'left') { context.translate(128, 0); context.scale(-1, 1); }
      context.drawImage(image, index * 128, 0, 128, 128, 0, 0, 128, 128);
      context.restore();
      const zoom = Number(byId('zoom').value);
      canvas.style.width = canvas.style.height = `${128 * zoom}px`;
    }
    function render(state) {
      const clip = data.clips[state.clipIndex], frame = data.frames[state.frameIndex];
      draw(byId('reference'), idle, 0);
      draw(byId('candidate'), frame ? source : idle, frame ? frame.index : 0);
      byId('candidate-label').textContent = clip.label;
      scrub.max = clip.endMs; scrub.value = state.timeMs;
      byId('frame-state').textContent = `${frame ? frame.sprite : 'male_hero-idle_0'} · ${Math.round(state.timeMs * 1000) / 1000} / ${clip.endMs} ms`;
      byId('frame-state').dataset.frame = frame ? frame.index : 'idle';
      byId('frame-state').dataset.mode = state.mode;
      byId('status').textContent = state.mode === 'pause' ? 'Idle — nghỉ 450 ms.' :
        state.mode === 'finished' ? 'Đã về idle.' : state.mode === 'stopped' ? 'Đã dừng.' : 'Đang phát…';
    }
    controller = createController(data.clips, scope, render, data.loopPauseMs);
    byId('once').onclick = () => controller.play(false);
    byId('loop').onclick = () => controller.play(true);
    byId('stop').onclick = () => controller.stop();
    byId('previous').onclick = () => controller.step(-1);
    byId('next').onclick = () => controller.step(1);
    clipSelect.onchange = () => controller.select(Number(clipSelect.value));
    byId('speed').onchange = () => controller.setSpeed(Number(byId('speed').value));
    scrub.oninput = () => controller.seek(Number(scrub.value));
    for (const name of ['direction', 'zoom']) byId(name).onchange = () => render(controller.snapshot());
    render(controller.snapshot()); byId('controls').disabled = false;
  }
  init().catch(error => {
    byId('status').dataset.error = 'true'; byId('status').textContent = `Không nạp được sprite: ${error.message}`;
  });
})(typeof window === 'undefined' ? globalThis : window);
