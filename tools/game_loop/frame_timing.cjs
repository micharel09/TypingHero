'use strict';

// Rows come from the automation-only Unity LateUpdate buffer, never the 10Hz state.
const ACTIVE = 1 | 256, EXCLUDED = 2 | 4 | 8 | 16 | 32 | 512;
const active = row => (row[5] & ACTIVE) === ACTIVE && !(row[5] & EXCLUDED);
function summarize(rows, dropped = 0) {
  const intervals = [], reasons = new Set();
  let combatMs = 0, slowWindowMs = 0, longestSlowWindowMs = 0, slowStart = null;
  if (dropped) reasons.add('buffer overflow');
  for (let i = 1; i < rows.length; i++) {
    const a = rows[i - 1], b = rows[i];
    if (b[0] !== a[0] + 1 || b[1] !== a[1] + 1) reasons.add('missing Unity frames');
    if (b[10] !== a[10]) reasons.add('focus/visibility changed');
    if (!b[8] || !b[9] || !a[8] || !a[9]) reasons.add('page not focused/visible');
    if ((b[5] & 1) && !(b[5] & EXCLUDED) && !(b[5] & 256)) reasons.add('missing actor coverage during gameplay');
    if (!active(a) || !active(b)) { slowStart = null; slowWindowMs = 0; continue; }
    const ms = b[2] - a[2];
    if (!Number.isFinite(ms) || ms <= 0) { reasons.add('invalid timestamp'); continue; }
    intervals.push(ms); combatMs += ms;
    // A continuous run of >25ms combat frames must remain visible in the report.
    if (ms > 25) {
      slowStart ??= a[2]; slowWindowMs = b[2] - slowStart;
      longestSlowWindowMs = Math.max(longestSlowWindowMs, slowWindowMs);
    } else { slowStart = null; slowWindowMs = 0; }
  }
  const sorted = [...intervals].sort((a, b) => a - b);
  const p95 = sorted.length ? sorted[Math.ceil(sorted.length * .95) - 1] : null;
  const fps = combatMs ? intervals.length * 1000 / combatMs : null;
  if (!intervals.length) reasons.add('no combat frames');
  return { valid: reasons.size === 0, invalidReasons: [...reasons], frames: intervals.length,
    combatMs, fps, p95, hitchOver50Ms: intervals.filter(ms => ms > 50).length,
    longestSlowWindowMs, thresholdsMet: fps >= 59 && p95 <= 18 && longestSlowWindowMs < 1000 };
}

// Installs browser clocks separately from Unity's frame markers. GPU queries are
// diagnostic only and bounded; unavailable/disjoint results stay null.
async function install(page, { detailed = true, gpu = false, counters = false } = {}) {
  await page.evaluate(({ detailed, gpu, counters }) => {
    window.typingHeroPerfOptions = { enabled: true, detailed };
    const trace = window.typingHeroBrowserFrames = { raf: [], callbacks: [], gpu: [], events: [] };
    for (const name of ['focus', 'blur', 'visibilitychange'])
      (name === 'visibilitychange' ? document : window).addEventListener(name, () =>
        trace.events.push({ at: performance.now(), name, focused: document.hasFocus(), visible: document.visibilityState }));
    const request = window.requestAnimationFrame.bind(window);
    let previous, gl, ext;
    const pending = [];
    let draws = 0;
    if (counters) for (const name of ['drawElements', 'drawArrays', 'drawElementsInstanced', 'drawArraysInstanced', 'drawRangeElements']) {
      const original = WebGL2RenderingContext.prototype[name];
      WebGL2RenderingContext.prototype[name] = function (...args) { draws++; return original.apply(this, args); };
    }
    function tick(at) {
      trace.raf.push([at, previous === undefined ? null : at - previous, document.hasFocus(), document.visibilityState]);
      previous = at; request(tick);
    }
    request(tick);
    window.requestAnimationFrame = callback => request(function (at) {
      if (gpu && !gl) {
        gl = document.querySelector('canvas')?.getContext('webgl2');
        ext = gl?.getExtension('EXT_disjoint_timer_query_webgl2');
        trace.gpuSupported = !!ext;
      }
      if (ext) {
        const disjoint = gl.getParameter(ext.GPU_DISJOINT_EXT);
        while (pending.length && (disjoint || gl.getQueryParameter(pending[0].query, gl.QUERY_RESULT_AVAILABLE))) {
          const q = pending.shift();
          trace.gpu.push({ at: q.at, unityFrames: q.unityFrames, ms: disjoint ? null : gl.getQueryParameter(q.query, gl.QUERY_RESULT) / 1e6, disjoint });
          gl.deleteQuery(q.query);
        }
      }
      const query = ext && pending.length < 64 && !gl.getQuery(ext.TIME_ELAPSED_EXT, gl.CURRENT_QUERY) ? gl.createQuery() : null;
      if (query) gl.beginQuery(ext.TIME_ELAPSED_EXT, query);
      const begin = performance.now();
      const before = window.typingHeroPerfBuffer?.write || 0, beforeDraws = draws;
      try { return callback(at); }
      finally {
        const unityFrames = (window.typingHeroPerfBuffer?.write || 0) - before;
        trace.callbacks.push({ at: begin, ms: performance.now() - begin, unityFrames,
          drawCalls: counters ? draws - beforeDraws : null });
        if (query) { gl.endQuery(ext.TIME_ELAPSED_EXT); pending.push({ query, at: begin, unityFrames }); }
      }
    });
  }, { detailed, gpu, counters });
}
async function drain(page) {
  return page.evaluate(() => {
    const unity = window.typingHeroReadFrames?.();
    const trace = window.typingHeroBrowserFrames;
    const browser = {};
    if (trace) for (const key of ['raf', 'callbacks', 'gpu', 'events']) browser[key] = trace[key].splice(0);
    return { unity, browser, at: performance.now(), focused: document.hasFocus(), visible: document.visibilityState,
      gpuSupported: trace?.gpuSupported ?? null, memory: window.typingHeroQA?.memory ?? null,
      jsHeap: performance.memory?.usedJSHeapSize ?? null };
  });
}
module.exports = { active, summarize, install, drain };
