'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const timing = require('./frame_timing.cjs');

// Scenario for the existing native, fixture-intercepted keyboard/mouse harness.
// PERF_OUTPUT must point at a task-owned output directory; video stays disabled.
module.exports = async (api, browser) => {
  const { session, start, fight, state, until, key, clickControl, check, report } = api;
  const output = process.env.PERF_OUTPUT;
  if (!output || !path.isAbsolute(output)) throw Error('PERF_OUTPUT requires an absolute task output directory');
  const mode = process.env.PERF_MODE || 'normal';
  if (!['normal', 'parry'].includes(mode)) throw Error('PERF_MODE must be normal or parry');
  const diagnostic = process.env.PERF_DIAGNOSTIC === '1';
  const duration = diagnostic ? (mode === 'parry' ? 20000 : 10000) : 60000, warmup = diagnostic ? 5000 : 15000;
  const { context, page } = await session(browser, { width: 1280, height: 720 }, 0, false);
  const receipt = { mode, diagnostic, warmup, duration, build: report.build, batches: [], states: [], actions: [] };
  receipt.driverHashes = Object.fromEntries([__filename, require.resolve('./frame_timing.cjs')].map(file =>
    [path.basename(file), crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')]));
  receipt.options = { detailed: process.env.PERF_DETAILED !== '0', gpu: diagnostic && process.env.PERF_GPU === '1', counters: true };
  const rows = []; let dropped = 0, begun, collectAt, lastDrain = 0, lastTyped = 0, lastParry = 0;
  const filename = path.join(output, 'frames.json');
  const save = () => fs.writeFileSync(filename, JSON.stringify(receipt));
  async function collect() {
    const batch = await timing.drain(page);
    receipt.batches.push(batch);
    if (batch.unity) { rows.push(...batch.unity.rows); dropped = batch.unity.dropped; }
    receipt.summary = timing.summarize(rows.filter(row => row[2] >= collectAt), dropped);
    if (receipt.batches.some(b => b.at >= collectAt &&
      (!b.focused || b.visible !== 'visible' || b.browser.events?.length))) {
      receipt.summary.valid = false;
      receipt.summary.invalidReasons.push('browser focus/visibility event during measurement');
    }
    save(); lastDrain = Date.now();
  }
  try {
    await start(page); await fight(page);
    check(/NVIDIA.*1650 Ti/.test(report.graphics.renderer), 'actual GTX 1650 Ti');
    const initial = await state(page);
    check(initial.worldWidth === 320 && initial.worldHeight === 180 && initial.width === 1280 && initial.height === 720,
      'world 320x180 and native WebGL 1280x720');
    receipt.dimensions = { world: [initial.worldWidth, initial.worldHeight], screen: [initial.width, initial.height] };
    receipt.graphics = report.graphics; receipt.browser = report.browser;
    await timing.install(page, receipt.options);
    begun = Date.now(); collectAt = await page.evaluate(warmup => performance.now() + warmup, warmup);
    receipt.startedAt = begun; receipt.collectAt = collectAt;
    while (Date.now() - begun < warmup + duration * 3) {
      if (Date.now() - lastDrain >= 250) await collect();
      if (receipt.summary.combatMs >= duration) break;
      const s = await page.evaluate(() => {
        const s = window.typingHeroQA;
        const fields = ['errors', 'lastError', 'frame', 'hp', 'enemyHp', 'introRunning', 'dungeonTransitioning',
          'paused', 'finished', 'encounterReading', 'slayer', 'matchParries', 'defeated', 'current',
          'hitLocked', 'playerHitStateActive', 'pendingAttack'];
        return Object.fromEntries(fields.map(name => [name, s?.[name]]));
      });
      if (s.errors) throw Error(s.lastError);
      receipt.states.push({ at: Date.now(), frame: s.frame, hp: s.hp, enemyHp: s.enemyHp,
        intro: s.introRunning, transition: s.dungeonTransitioning, paused: s.paused,
        finished: s.finished, reading: s.encounterReading, slayer: s.slayer, parries: s.matchParries });
      if (s.introRunning || s.dungeonTransitioning) { await page.waitForTimeout(20); continue; }
      if (s.encounterReading) { await key(page, 'Enter'); await fight(page); continue; }
      if (s.finished) {
        if (s.hp > 0 && s.defeated < 6) {
          if (s.enemyHp !== -1) { await page.waitForTimeout(20); continue; }
          await key(page, 'Enter');
          await until(page, t => t.scene === 'SampleScene' && !t.finished && t.defeated === s.defeated && t.hp > 0 && t.enemyHp > 0 && !!t.current);
          await fight(page);
        } else {
          const restart = (await state(page)).controls.find(c => c.kind === 'button' && /Restart/.test(c.name));
          if (!restart) { await page.waitForTimeout(20); continue; }
          await clickControl(page, restart.name, 'button');
          await until(page, t => !t.finished && t.hp === 40 && !t.paused); await fight(page);
        }
        continue;
      }
      if (s.paused) throw Error('Unexpected Pause; measurement invalid');
      if (mode === 'parry') {
        // Use the actual per-render observation when available, not the 10Hz snapshot.
        const cue = await page.evaluate(() => {
          const f = window.typingHeroCombatFrame;
          return f ? { frame: f.frame, parries: f.parries, slayer: f.slayer,
            canParry: f.canParry, enemyAttacking: f.enemyAttacking, enemyContactIn: f.enemyContactIn,
            timeScale: f.timeScale, age: (performance.now() - window.typingHeroCombatFramePublishedAt) / 1000 } : null;
        });
        const contact = cue ? cue.enemyContactIn - cue.age * cue.timeScale : -1;
        if (cue && !cue.slayer && cue.age < .1 && cue.canParry && cue.enemyAttacking && contact > .045 && contact <= .23 && Date.now() - lastParry > 300) {
          await page.keyboard.press('Space', { delay: 55 }); lastParry = Date.now();
          receipt.actions.push({ at: lastParry, kind: 'Space', frame: cue.frame, before: cue.parries });
        }
      }
      if (s.current && !s.hitLocked && !s.playerHitStateActive && !s.pendingAttack && Date.now() - lastTyped > (s.slayer ? 1600 : 4000)) {
        await page.keyboard.type(s.current, { delay: 25 }); lastTyped = Date.now();
        receipt.actions.push({ at: lastTyped, kind: 'typed', frame: s.frame });
      }
      await page.waitForTimeout(15);
    }
    await collect();
    check(receipt.summary.valid, 'continuous, visible, focused Unity frame coverage');
    check(receipt.summary.combatMs >= duration, 'required combat duration');
    const measured = rows.filter(row => row[2] >= collectAt);
    check(mode !== 'parry' || (measured.some(row => row[5] & 64) && measured.some(row => row[6] > measured[0][6])), 'actual parry and Slayer coverage');
    check(mode !== 'normal' || measured.every(row => !(row[5] & (64 | 128))), 'normal excludes parry/Slayer');
    if (!diagnostic) check(receipt.summary.thresholdsMet, 'each run meets FPS >=59 / p95 <=18ms / no sustained 30FPS');
    receipt.status = 'passed';
  } catch (error) {
    receipt.status = 'failed'; receipt.error = String(error.stack); throw error;
  } finally {
    // Input/assertion failures must never discard the timing trace.
    let finalError;
    try { if (begun) await collect(); }
    catch (error) { receipt.finalDrainError = String(error.stack); receipt.status = 'failed'; finalError = error; }
    finally { save(); report.timing = { ...receipt.summary, status: receipt.status, diagnostic, file: filename }; await context.close(); }
    if (finalError && !receipt.error) throw finalError;
  }
};
