'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const { summarize } = require('./frame_timing.cjs');
const row = (frame, at, flags = 257) => [frame, frame, at, 16, 0, flags, 0, 1, 1, 1, 0];

test('Unity intervals retain combat hitches and ignore explicit transition intervals', () => {
  const result = summarize([row(0, 0), row(1, 16), row(2, 116), row(3, 132, 289), row(4, 148), row(5, 164)]);
  assert.equal(result.frames, 3); assert.equal(result.combatMs, 132);
  assert.equal(result.hitchOver50Ms, 1); assert.equal(result.p95, 100);
  assert.equal(result.thresholdsMet, false);
});
test('missing frames, overflow and focus changes invalidate rather than filter a run', () => {
  const rows = [row(0, 0), row(2, 16), row(3, 32)];
  rows[2][10] = 2; rows[2][8] = 0;
  const result = summarize(rows, 1);
  assert.equal(result.valid, false); assert.equal(result.invalidReasons.length, 4);
  assert.equal(result.frames, 2);
});
test('60Hz passes; continuous 30Hz combat is explicitly reported', () => {
  assert.equal(summarize(Array.from({ length: 121 }, (_, i) => row(i, i * 1000 / 60))).thresholdsMet, true);
  const slow = summarize(Array.from({ length: 121 }, (_, i) => row(i, i * 1000 / 30)));
  assert.equal(slow.longestSlowWindowMs, 4000); assert.equal(slow.thresholdsMet, false);
});
test('unclassified gameplay without live actor observations invalidates coverage', () => {
  const result = summarize([row(0, 0), row(1, 16, 1), row(2, 32)]);
  assert.equal(result.valid, false);
  assert.deepEqual(result.invalidReasons, ['missing actor coverage during gameplay', 'no combat frames']);
});
test('automation bridge has fixed capacity, explicit loss, monotonic drain and focus epoch', () => {
  let bridge, now = 0;
  const listeners = {};
  const window = { addEventListener: (name, fn) => { listeners[name] = fn; } };
  const document = { addEventListener: window.addEventListener, hasFocus: () => true, visibilityState: 'visible' };
  vm.runInNewContext(fs.readFileSync(require('node:path').join(__dirname, '../../Assets/Automation/Plugins/WebGL/TypingHeroQA.jslib'), 'utf8'),
    { LibraryManager: { library: {} }, mergeInto: (_, value) => { bridge = value; }, window, document, performance: { now: () => now++ }, Float64Array });
  assert.equal(bridge.TypingHeroQA_FrameBegin(), 1);
  assert.equal(window.typingHeroPerfBuffer, undefined);
  window.typingHeroPerfOptions = { enabled: true, detailed: false };
  for (let i = 0; i < 16386; i++) {
    assert.equal(bridge.TypingHeroQA_FrameBegin(), 2);
    bridge.TypingHeroQA_FrameEnd(i, 16, 257, 0, 1);
  }
  listeners.blur(); listeners.focus();
  const result = window.typingHeroReadFrames();
  assert.equal(result.rows.length, 16384); assert.equal(result.dropped, 2);
  assert.equal(result.rows[0][0], 2); assert.equal(result.rows.at(-1)[0], 16385);
  assert.equal(result.focusEpoch, 2); assert.equal(result.schema.length, 11);
  assert.equal(window.typingHeroReadFrames().rows.length, 0);
  window.typingHeroPerfOptions.detailed = true;
  assert.equal(bridge.TypingHeroQA_FrameBegin(), 3);
  bridge.TypingHeroQA_FrameEnd(16386, 16, 257, 0, 1);
  assert.equal(window.typingHeroReadFrames().rows[0][10], 2);
});

test('a failed input retains raw frames and the error before closing the context', async () => {
  const timing = require('./frame_timing.cjs');
  const scenario = require('./dungeon_performance.cjs');
  const original = { install: timing.install, drain: timing.drain, write: fs.writeFileSync, output: process.env.PERF_OUTPUT };
  let saved, closed = false, calls = 0;
  process.env.PERF_OUTPUT = require('node:path').resolve('unused-in-memory-fixture');
  timing.install = async () => {};
  timing.drain = async () => ({ at: 2000, focused: true, visible: 'visible', browser: { events: [] },
    unity: { rows: [row(calls++, 1100 + calls * 16)], dropped: 0 } });
  fs.writeFileSync = (_, text) => { assert.equal(closed, false); saved = JSON.parse(text); };
  try {
    await assert.rejects(scenario({
      report: { build: 'fixture', graphics: { renderer: 'NVIDIA GTX 1650 Ti' } },
      session: async () => ({ context: { close: async () => { closed = true; } }, page: { evaluate: async () => {
        if (!calls) return 1000;
        throw Error('input failure fixture');
      } } }),
      start: async () => {}, fight: async () => {},
      check: (ok, message) => { if (!ok) throw Error(message); },
      state: async () => {
        if (!calls) return { worldWidth: 320, worldHeight: 180, width: 1280, height: 720 };
        throw Error('input failure fixture');
      }
    }, {}), /input failure fixture/);
    assert.equal(closed, true); assert.equal(saved.status, 'failed');
    assert.match(saved.error, /input failure fixture/); assert.equal(saved.batches.length, 2);
  } finally {
    timing.install = original.install; timing.drain = original.drain; fs.writeFileSync = original.write;
    if (original.output === undefined) delete process.env.PERF_OUTPUT; else process.env.PERF_OUTPUT = original.output;
  }
});
