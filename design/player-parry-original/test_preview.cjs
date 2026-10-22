'use strict';
const assert = require('node:assert/strict');
const fs = require('node:fs');
const { createController } = require('./preview.js');
const data = JSON.parse(fs.readFileSync(require.resolve('./preview-data.js'), 'utf8').replace(/^window.PARRY_PREVIEW = /, '').trim().replace(/;$/, ''));
function fixture() {
  let now = 0, serial = 0;
  const timers = new Map(), events = [];
  const clock = {
    setTimeout(fn, delay) { const id = ++serial; timers.set(id, { at: now + delay, fn }); return id; },
    clearTimeout(id) { timers.delete(id); }
  };
  function tick(ms) {
    const end = now + ms;
    while (true) {
      const next = [...timers.entries()].filter(([, t]) => t.at <= end + 1e-8).sort((a, b) => a[1].at - b[1].at)[0];
      if (!next) break;
      now = next[1].at; timers.delete(next[0]); next[1].fn();
    }
    now = end;
  }
  const controller = createController(data.clips, clock, state => events.push({ at: now, ...state }), data.loopPauseMs);
  return { controller, tick, timers, events };
}
assert.deepEqual(data.frames.map(f => f.index), [2, 3, 4, 5]);
for (const [index, clip] of data.clips.entries()) for (const speed of [1, .5]) {
  assert.ok(Math.abs(clip.holds.reduce((a, b) => a + b, 0) - clip.endMs) < 1e-8);
  const { controller: c, tick, timers, events } = fixture();
  c.select(index); c.setSpeed(speed); c.play(false);
  assert.equal(c.snapshot().frameIndex, 0);
  for (let i = 0; i < 4; i++) {
    tick(clip.holds[i] / speed - .001); assert.equal(c.snapshot().frameIndex, i);
    tick(.001); assert.equal(c.snapshot().frameIndex, i + 1);
  }
  assert.equal(c.snapshot().mode, 'finished'); assert.equal(timers.size, 0);
  assert.equal(c.snapshot().timeMs, clip.endMs);
  tick(5000); assert.equal(c.snapshot().frameIndex, 4);
  c.play(true); tick(clip.endMs / speed);
  assert.equal(c.snapshot().mode, 'pause'); assert.equal(c.snapshot().frameIndex, 4);
  tick(449.999); assert.equal(c.snapshot().frameIndex, 4);
  tick(.001); assert.equal(c.snapshot().frameIndex, 0);
  assert.equal(c.snapshot().mode, 'loop');
  for (const operation of ['stop', 'seek', 'step', 'select', 'setSpeed']) {
    c.play(true); tick(1);
    if (operation === 'seek') c.seek(clip.endMs);
    else if (operation === 'step') c.step(1);
    else if (operation === 'select') c.select(index);
    else if (operation === 'setSpeed') c.setSpeed(speed);
    else c.stop();
    const before = c.snapshot(); tick(10000);
    assert.deepEqual(c.snapshot(), before, operation); assert.equal(timers.size, 0);
  }
  c.select(index); c.seek(clip.holds[0] - .001); assert.equal(c.snapshot().frameIndex, 0);
  c.seek(clip.holds[0]); assert.equal(c.snapshot().frameIndex, 1);
  c.seek(clip.endMs); assert.equal(c.snapshot().frameIndex, 4);
  c.step(-1); assert.equal(c.snapshot().frameIndex, 3);
  assert.ok(events.length > 10);
}
console.log('PASS: original 2/3/4/5/idle schedules, exact boundaries, both speeds, 450 ms rest, scrub and canceled callbacks.');
