'use strict';

// Read-only QA observations; all progress is driven through real keyboard input.
const READY = 'READY';
function installObservationJSON() {
  const parse = JSON.parse;
  // JsonUtility can publish non-finite screen coordinates while a label exits.
  // Preserve those numbers; quoted text and every ordinary JSON payload stay intact.
  JSON.parse = function (text, reviver) {
    try { return parse(text, reviver); }
    catch (error) {
      if (!(error instanceof SyntaxError) || typeof text !== 'string' || !text.includes('Infinity')) throw error;
      const converted = text.replace(/"(?:\\.|[^"\\])*"|(-?Infinity)(?=\s*[,}\]])/g,
        (token, number) => number ? (number[0] === '-' ? '-1e999' : '1e999') : token);
      if (converted === text) throw error;
      return parse(converted, reviver);
    }
  };
}

function readyRemaining(s) {
  const word = typeof s.current === 'string' ? s.current.toUpperCase() : '';
  return s.scene === 'SampleScene' && s.endless && s.finished && s.hp > 0 &&
    s.defeated > 0 && s.defeated < 6 && !s.paused && !s.encounterReading &&
    !s.introRunning && !s.dungeonTransitioning && word && READY.endsWith(word) ? word : null;
}

async function observedKey(page, key, until, accepted) {
  try {
    await page.keyboard.down(key);
    return await until(page, accepted);
  } finally {
    await page.keyboard.up(key);
  }
}

async function observedClick(page, x, y, until, beforeFrame) {
  await page.mouse.move(x, y);
  let held;
  try {
    await page.mouse.down();
    held = await until(page, s => s.frame > beforeFrame + 1);
  } finally {
    await page.mouse.up();
  }
  return until(page, s => s.frame > held.frame);
}

async function selectAllObserved(api, page) {
  const before = await api.state(page);
  let held;
  try {
    await page.keyboard.down('Control');
    held = await observedKey(page, 'A', api.until, s => s.frame > before.frame + 1);
  } finally {
    await page.keyboard.up('Control');
  }
  return api.until(page, s => s.frame > held.frame);
}

async function continueReady(api, page, onInput = () => {}) {
  const { state, until } = api;
  let s = await state(page);
  if (!readyRemaining(s)) throw Error('Typed READY requires a live intermediate-duel prompt');
  const defeated = s.defeated, slot = s.endlessSlot;
  const sameDuel = t => t.scene === 'SampleScene' && t.defeated === defeated && t.endlessSlot === slot;
  for (;;) {
    const remaining = readyRemaining(s), before = s;
    if (!remaining || !sameDuel(s)) throw Error('READY prompt changed before input');
    const expected = remaining.slice(1), letter = remaining[0];
    s = await observedKey(page, letter, until, t => {
      if (!(t.frame > before.frame)) return false; // A repeated 100ms publication cannot acknowledge input.
      if (expected) return sameDuel(t) && readyRemaining(t) === expected;
      return !sameDuel(t) || !t.finished || t.current === '' || t.dungeonTransitioning;
    });
    onInput({ letter, remaining, beforeFrame: before.frame, afterFrame: s.frame, defeated, slot });
    if (!expected) break;
  }
  // No more letters after Y, including during loading, intro or held-input release.
  return until(page, t => t.frame > s.frame && t.scene === 'SampleScene' && t.endless &&
    t.defeated === defeated && t.endlessSlot === slot &&
    !t.finished && !t.paused && !t.introRunning && !t.dungeonTransitioning &&
    !t.encounterReading && t.timeScale > 0 && t.hp > 0 && t.enemyHp > 0 && !!t.current);
}

module.exports = { readyRemaining, observedKey, observedClick, selectAllObserved, continueReady, installObservationJSON };
