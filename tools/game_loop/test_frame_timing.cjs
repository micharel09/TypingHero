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
async function runFixture({ final = 'valid', inputError = false, saveError = false, closeError = false, verification, windowsForeground } = {}) {
  const timing = require('./frame_timing.cjs'), scenario = require('./dungeon_performance.cjs');
  const original = { install: timing.install, drain: timing.drain, write: fs.writeFileSync,
    output: process.env.PERF_OUTPUT, diagnostic: process.env.PERF_DIAGNOSTIC, mode: process.env.PERF_MODE, verify: process.env.PERF_VERIFY };
  let calls = 0, closed = false, saved, error, persisted;
  const report = { build: 'fixture', graphics: { renderer: 'NVIDIA GTX 1650 Ti' } };
  process.env.PERF_OUTPUT = require('node:path').resolve('unused-in-memory-fixture');
  process.env.PERF_DIAGNOSTIC = '1'; process.env.PERF_MODE = 'normal';
  process.env.PERF_VERIFY = verification === undefined ? '0' : '1';
  timing.install = async () => {};
  timing.drain = async () => {
    calls++;
    if (calls === 2 && final === 'drain error') throw Error('drain fixture');
    const rows = calls === 1 ? Array.from({ length: inputError ? 2 : 3601 }, (_, i) => row(i, 1000 + i * 1000 / 60)) :
      [row(final === 'gap' ? 3603 : 3601, 61000 + 1000 / 60)];
    return { at: 62000, focused: !(calls === 2 && final === 'focus'), visible: 'visible', browser: { events: [] },
      unity: { rows, dropped: calls === 2 && final === 'overflow' ? 1 : 0 } };
  };
  fs.writeFileSync = (_, text) => {
    if (saveError) throw Error('save fixture');
    saved = JSON.parse(text);
  };
  try {
    await scenario({ report,
      ...(windowsForeground === undefined ? {} : { foregroundVerdict: async () => {
        assert.equal(calls, 2); assert.equal(closed, false);
        return { pass: windowsForeground, browserPid: 123, sampleCount: 600 };
      } }),
      saveReport: async () => { assert.equal(closed, false); persisted = JSON.parse(JSON.stringify(report)); },
      session: async () => ({ context: { close: async () => { closed = true; if (closeError) throw Error('close fixture'); } }, page: { evaluate: async () => {
        if (calls >= 2 && verification !== undefined) {
          if (verification === 'error') throw Error('verification fixture');
          return verification;
        }
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
    for (const [key, value] of [['PERF_OUTPUT', original.output], ['PERF_DIAGNOSTIC', original.diagnostic], ['PERF_MODE', original.mode], ['PERF_VERIFY', original.verify]]) {
      if (value === undefined) delete process.env[key]; else process.env[key] = value;
    }
  }
  return { calls, closed, saved, error, report, persisted };
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
  assert.deepEqual(r.persisted.timing, JSON.parse(JSON.stringify(r.report.timing)));
  assert.equal(r.persisted.status, r.saved.status);
});

test('a browser close error remains separate from the persisted measurement verdict', async () => {
  const r = await runFixture({ closeError: true });
  assert.equal(r.error, undefined); assert.equal(r.closed, true);
  assert.equal(r.saved.status, 'passed'); assert.equal(r.persisted.status, 'passed');
  assert.equal(r.report.timing.status, 'passed'); assert.match(r.report.closeError, /close fixture/);
});
test('actual Windows foreground denial invalidates the final verdict without deleting frames', async () => {
  const r = await runFixture({ windowsForeground: false });
  assert.ok(r.error); assert.equal(r.saved.summary.valid, false);
  assert.ok(r.saved.summary.invalidReasons.includes('task browser lost actual Windows foreground'));
  assert.equal(r.saved.batches.length, 2); assert.equal(r.saved.status, 'failed');
  assert.equal(r.persisted.timing.windowsForeground.pass, false); assert.equal(r.closed, true);
});
test('actual Windows foreground evidence is persisted identically before closing', async () => {
  const r = await runFixture({ windowsForeground: true });
  assert.equal(r.error, undefined); assert.equal(r.saved.status, 'passed');
  assert.deepEqual(r.saved.windowsForeground, r.persisted.timing.windowsForeground);
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


test('serialization verification preserves full changed and stale particle snapshots', () => {
  let bridge;
  const cache = { json: '' };
  const window = { addEventListener() {}, typingHeroPerfOptions: { enabled: true, detailed: true, verifyCombatSerialization: true } };
  const document = { addEventListener() {}, hasFocus: () => true, visibilityState: 'visible' };
  vm.runInNewContext(fs.readFileSync(require('node:path').join(__dirname, '../../Assets/Automation/Plugins/WebGL/TypingHeroQA.jslib'), 'utf8'),
    { LibraryManager: { library: {} }, mergeInto: (_, value) => { bridge = value; }, window, document,
      performance: { now: () => 123 }, Float64Array, UTF8ToString: value => value, TypingHeroCombatParticles: cache });
  assert.deepEqual(Array.from(bridge.TypingHeroQA_CombatFrame__deps), ['$TypingHeroCombatParticles']);
  assert.equal(bridge.TypingHeroQA_FrameBegin(), 7);
  const expected = { frame: 7, parrySampleCount: 0,
    parryParticles: Array.from({ length: 192 }, (_, i) => ({ identity: i, age: i / 10, position: { x: i, y: 0, z: 1 }, color: { r: i, g: 0, b: 255, a: 128 } })) };
  const core = () => JSON.stringify({ ...expected, parryParticles: [] });
  const payload = () => JSON.stringify({ parryParticles: expected.parryParticles });
  bridge.TypingHeroQA_CombatFrame(core(), payload(), JSON.stringify(expected));
  const first = window.typingHeroCombatFrame;
  assert.deepEqual(JSON.parse(JSON.stringify(first)), expected);
  assert.equal(window.typingHeroCombatSerializationChecks, 1);
  first.parryParticles[191].position.x = -1;
  window.typingHeroPerfOptions.verifyCombatSerialization = false;
  assert.equal(bridge.TypingHeroQA_FrameBegin(), 3);
  expected.frame++;
  bridge.TypingHeroQA_CombatFrame(core(), null, null);
  const deferred = window.typingHeroCombatFrame;
  const deferredExpected = JSON.parse(JSON.stringify(expected));
  assert.equal(Object.isFrozen(deferred), true);
  assert.equal(window.typingHeroCombatSerializationChecks, 1);
  expected.frame++; expected.parrySampleCount = 1;
  expected.parryParticles[0].age = 0.25; expected.parryParticles[0].position.y = 9;
  bridge.TypingHeroQA_CombatFrame(core(), payload(), JSON.stringify(expected));
  assert.deepEqual(JSON.parse(JSON.stringify(window.typingHeroCombatFrame)), expected);
  // A retained frame can first read its particles AFTER a later payload arrives.
  assert.deepEqual(JSON.parse(JSON.stringify(deferred)), deferredExpected);
  deferred.parryParticles[0].position.y = -99;
  assert.equal(window.typingHeroCombatFrame.parryParticles[0].position.y, 9);
  expected.frame++; expected.parrySampleCount = 0;
  bridge.TypingHeroQA_CombatFrame(core(), null, JSON.stringify(expected));
  assert.deepEqual(JSON.parse(JSON.stringify(window.typingHeroCombatFrame)), expected);
  assert.equal(window.typingHeroCombatSerializationChecks, 3);
  const last = window.typingHeroCombatFrame;
  assert.throws(() => bridge.TypingHeroQA_CombatFrame(core(), null, '{}'), /differs from original/);
  assert.equal(window.typingHeroCombatFrame, last);
  assert.equal(window.typingHeroCombatSerializationChecks, 3);
  assert.equal(window.typingHeroCombatFramePublishedAt, 123);
});

test('verification requires every observed frame and receipt/report retain the count', async () => {
  const missing = await runFixture({ verification: 3601 });
  assert.match(missing.error.message, /every observed frame/);
  assert.equal(missing.closed, true); assert.equal(missing.saved.status, 'failed');
  const complete = await runFixture({ verification: 3602 });
  assert.equal(complete.error, undefined); assert.equal(complete.closed, true);
  assert.equal(complete.saved.serializationChecks, 3602);
  assert.equal(complete.report.timing.serializationChecks, 3602);
});

test('verification observation error keeps the trace and closes context', async () => {
  const r = await runFixture({ verification: 'error' });
  assert.match(r.error.message, /verification fixture/);
  assert.equal(r.saved.status, 'failed'); assert.equal(r.closed, true);
  assert.equal(r.report.timing.serializationCheckError, r.saved.serializationCheckError);
});

test('input error takes precedence over verification observation error', async () => {
  const r = await runFixture({ inputError: true, verification: 'error' });
  assert.match(r.error.message, /input fixture/); assert.equal(r.closed, true);
  assert.match(r.saved.serializationCheckError, /verification fixture/);
});
