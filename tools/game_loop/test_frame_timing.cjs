'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const { summarize } = require('./frame_timing.cjs');
const row = (frame, at, flags = 257) => [frame, frame, at, 16, 0, flags, 0, 1, 1, 1, 0];

test('Unity intervals retain combat hitches and ignore explicit transition intervals', () => {
  const result = summarize([row(0, 0), row(1, 16), row(2, 116), row(3, 132, 289), row(4, 148), row(5, 164)]);
  assert.equal(result.frames, 4); assert.equal(result.combatMs, 148);
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
  assert.deepEqual(result.invalidReasons, ['missing actor coverage during gameplay']);
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


test('combat exit retains 300ms hitch; entry and outside intervals are excluded', () => {
  const r = summarize([row(0, 0, 0), row(1, 200, 0), row(2, 400), row(3, 700, 289)]);
  assert.equal(r.frames, 1); assert.equal(r.p95, 300); assert.equal(r.hitchOver50Ms, 1);
});
test('mixed fast frames cannot conceal a two-second low FPS region', () => {
  const rows = [row(0, 0)];
  for (let i = 0; i < 6000; i++) rows.push(row(rows.length, rows.at(-1)[2] + 16));
  for (let i = 0; i < 60; i++) rows.push(row(rows.length, rows.at(-1)[2] + (i % 2 ? 50 : 16.6666667)));
  const r = summarize(rows);
  assert.ok(r.fps >= 59); assert.ok(r.p95 <= 18);
  assert.equal(r.sustainedLowFps, true); assert.ok(r.minCombatWindowFps < 40);
  assert.equal(r.thresholdsMet, false);
});
test('windows never join combat across Pause or loading', () => {
  for (const flags of [259, 289]) {
    const r = summarize([row(0, 0), row(1, 400), row(2, 800, flags), row(3, 2000), row(4, 2400), row(5, 2800, flags)]);
    assert.equal(r.minCombatWindowFps, null); assert.equal(r.sustainedLowFps, false);
  }
});
test('coverage uses measured combat intervals and consecutive combat counters', () => {
  const rows = [row(0, 0), row(1, 16, 323), row(2, 32, 321)];
  assert.deepEqual(summarize(rows).coverage, { parry: false, slayer: false, parryState: false });
  const combat = [row(0, 0), row(1, 16), row(2, 32), row(3, 48)];
  [8, 0, 0, 1].forEach((n, i) => { combat[i][6] = n; });
  assert.equal(summarize(combat).coverage.parry, true);
  combat[2][5] = 289; combat[3][6] = 8;
  assert.equal(summarize(combat).coverage.parry, false);
  assert.equal(summarize([row(0, 0, 321), row(1, 16, 289)]).coverage.slayer, true);
  assert.equal(summarize([row(0, 0, 385), row(1, 16)]).coverage.parryState, true);
  assert.equal(summarize([row(0, 0, 387), row(1, 16)]).coverage.parryState, false);
});

// In-memory scenario fixture: no browser, player build or output artifacts.
async function runFixture({ final = 'valid', inputError = false, saveError = false } = {}) {
  const timing = require('./frame_timing.cjs'), scenario = require('./dungeon_performance.cjs');
  const original = { install: timing.install, drain: timing.drain, write: fs.writeFileSync,
    output: process.env.PERF_OUTPUT, diagnostic: process.env.PERF_DIAGNOSTIC, mode: process.env.PERF_MODE };
  let calls = 0, closed = false, saved, error;
  const report = { build: 'fixture', graphics: { renderer: 'NVIDIA GTX 1650 Ti' } };
  process.env.PERF_OUTPUT = require('node:path').resolve('unused-in-memory-fixture');
  process.env.PERF_DIAGNOSTIC = '1'; process.env.PERF_MODE = 'normal';
  timing.install = async () => {};
  timing.drain = async () => {
    calls++;
    if (calls === 2 && final === 'drain error') throw Error('drain fixture');
    const rows = calls === 1 ? Array.from({ length: inputError ? 2 : 601 }, (_, i) => row(i, 1000 + i * 1000 / 60)) :
      [row(final === 'gap' ? 603 : 601, 11000 + 1000 / 60)];
    return { at: 12000, focused: !(calls === 2 && final === 'focus'), visible: 'visible', browser: { events: [] },
      unity: { rows, dropped: calls === 2 && final === 'overflow' ? 1 : 0 } };
  };
  fs.writeFileSync = (_, text) => {
    if (saveError) throw Error('save fixture');
    saved = JSON.parse(text);
  };
  try {
    await scenario({ report,
      session: async () => ({ context: { close: async () => { closed = true; } }, page: { evaluate: async () => {
        if (inputError && calls) throw Error('input fixture');
        return 1000;
      } } }),
      start: async () => {}, fight: async () => {},
      state: async () => ({ worldWidth: 320, worldHeight: 180, width: 1280, height: 720 }),
      check: (ok, message) => { if (!ok) throw Error(message); }
    }, {});
  } catch (e) { error = e; }
  finally {
    timing.install = original.install; timing.drain = original.drain; fs.writeFileSync = original.write;
    for (const [key, value] of [['PERF_OUTPUT', original.output], ['PERF_DIAGNOSTIC', original.diagnostic], ['PERF_MODE', original.mode]]) {
      if (value === undefined) delete process.env[key]; else process.env[key] = value;
    }
  }
  return { calls, closed, saved, error, report };
}
for (const final of ['focus', 'overflow', 'gap']) test(`final batch ${final} invalidates verdict and retains trace`, async () => {
  const r = await runFixture({ final });
  assert.ok(r.error); assert.equal(r.closed, true); assert.equal(r.calls, 2);
  assert.equal(r.saved.status, 'failed'); assert.equal(r.saved.batches.length, 2);
  assert.equal(r.report.timing.status, r.saved.status);
  assert.deepEqual(r.report.timing.invalidReasons, r.saved.summary.invalidReasons);
});
test('final valid batch gives matching receipt and report without another drain', async () => {
  const r = await runFixture();
  assert.equal(r.error, undefined); assert.equal(r.calls, 2); assert.equal(r.closed, true);
  assert.equal(r.saved.status, 'passed'); assert.equal(r.report.timing.frames, r.saved.summary.frames);
});
test('input error survives final drain error and context closes', async () => {
  const r = await runFixture({ inputError: true, final: 'drain error' });
  assert.match(r.error.message, /input fixture/); assert.match(r.saved.error, /input fixture/);
  assert.match(r.saved.finalDrainError, /drain fixture/); assert.equal(r.closed, true);
  assert.equal(r.saved.batches.length, 1); assert.equal(r.report.timing.status, 'failed');
});
test('save failure still closes context and reports failure', async () => {
  const r = await runFixture({ saveError: true });
  assert.match(r.error.message, /save fixture/); assert.equal(r.closed, true);
  assert.equal(r.report.timing.status, 'failed'); assert.match(r.report.timing.saveError, /save fixture/);
});
