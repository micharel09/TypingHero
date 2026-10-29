'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const timing = require('./frame_timing.cjs');
const { readyRemaining, continueReady } = require('./dungeon_input.cjs');

// Scenario for the existing native, fixture-intercepted keyboard/mouse harness.
// PERF_OUTPUT must point at a task-owned output directory; video stays disabled.
module.exports = async (api, browser) => {
  const { session, start, fight, state, until, key, clickControl, check, report } = api;
  const output = process.env.PERF_OUTPUT;
  if (!output || !path.isAbsolute(output)) throw Error('PERF_OUTPUT requires an absolute task output directory');
  const mode = process.env.PERF_MODE || 'normal';
  if (!['normal', 'parry'].includes(mode)) throw Error('PERF_MODE must be normal or parry');
  const diagnostic = process.env.PERF_DIAGNOSTIC === '1';
  const duration = 60000, warmup = 15000;
  const { context, page } = await session(browser, { width: 1280, height: 720 }, 0, false);
  const receipt = { mode, diagnostic, warmup, duration, build: report.build, batches: [], states: [], actions: [] };
  receipt.driverHashes = Object.fromEntries([__filename, require.resolve('./frame_timing.cjs'), require.resolve('./dungeon_input.cjs')].map(file =>
    [path.basename(file), crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')]));
  receipt.options = { detailed: process.env.PERF_DETAILED !== '0', gpu: diagnostic && process.env.PERF_GPU === '1', counters: true, verifyCombatSerialization: diagnostic && process.env.PERF_VERIFY === '1' };
  const rows = []; let dropped = 0, begun, collectAt, lastDrain = 0, lastTyped = 0, lastParry = 0;
  const filename = path.join(output, 'frames.json');
  const save = () => fs.writeFileSync(filename, JSON.stringify(receipt));
  function summarize() {
    receipt.summary = timing.summarize(rows.filter(row => row[2] >= collectAt), dropped);
    if (receipt.batches.some(b =>
      (!b.focused || b.visible !== 'visible' || b.browser.events?.length))) {
      receipt.summary.valid = false;
      receipt.summary.invalidReasons.push('browser focus/visibility event during warm-up or measurement');
    }
  }
  async function collect() {
    const batch = await timing.drain(page);
    receipt.batches.push(batch);
    if (batch.unity) { rows.push(...batch.unity.rows); dropped = Math.max(dropped, batch.unity.dropped); }
    summarize();
    save(); lastDrain = Date.now();
  }
  let failure;
  try {
    check(diagnostic || receipt.options.detailed, 'official runs require PERF_DETAILED=1');
    async function preflight(phase) {
    const preflightDeadline=Date.now()+60000;
    let readySince=0, previousFrame=-1, preflightPassed=false;
    receipt.preflight={phase,status:'WAITING',requiredMs:2000};save();
    while(Date.now()<preflightDeadline){
      const now=Date.now();
      const native=await api.foregroundVerdict({from:now-250,to:now});
      const tab=await page.evaluate(()=>({visible:document.visibilityState,focused:document.hasFocus(),frame:window.typingHeroQA?.frame}));
      if(native.pass&&tab.visible==='visible'&&tab.focused&&tab.frame>previousFrame){
        if(!readySince)readySince=now;
        if(now-readySince>=2000){preflightPassed=true;break;}
      }else readySince=0;
      previousFrame=tab.frame;await page.waitForTimeout(150);
    }
    receipt.preflight.status=preflightPassed?'PASSED':'PREFLIGHT_BLOCKED';
    (receipt.preflightHistory??=[]).push({...receipt.preflight});save();
    check(preflightPassed,'Win32 foreground, visible focused tab and advancing Unity frames for 2 seconds');
    }
    console.log('H\u00e3y focus c\u1eeda s\u1ed5 QA: '+process.env.ARENA_WINDOW_MARKER+'; preflight ch\u1edd t\u1ed1i \u0111a 60 gi\u00e2y.');
    await preflight('before-start');
    await start(page);
    const setup = await state(page);
    if (setup.paused) { try { await page.keyboard.down('Escape'); await until(page,s=>!s.paused); } finally { await page.keyboard.up('Escape'); } }
    await fight(page);
    check(/NVIDIA.*1650 Ti/.test(report.graphics.renderer), 'actual GTX 1650 Ti');
    const initial = await state(page);
    check(initial.worldWidth === 320 && initial.worldHeight === 180 && initial.width === 1280 && initial.height === 720,
      'world 320x180 and native WebGL 1280x720');
    receipt.dimensions = { world: [initial.worldWidth, initial.worldHeight], screen: [initial.width, initial.height] };
    receipt.graphics = report.graphics; receipt.browser = report.browser;
    await preflight('after-setup');
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
          'hitLocked', 'playerHitStateActive', 'pendingAttack', 'scene', 'endless', 'endlessSlot', 'timeScale',
          'comboRemaining', 'staminaFill'];
        return Object.fromEntries(fields.map(name => [name, s?.[name]]));
      });
      if (s.errors) throw Error(s.lastError);
      receipt.states.push({ at: Date.now(), frame: s.frame, hp: s.hp, enemyHp: s.enemyHp,
        intro: s.introRunning, transition: s.dungeonTransitioning, paused: s.paused,
        finished: s.finished, reading: s.encounterReading, slayer: s.slayer, parries: s.matchParries,
        comboRemaining: s.comboRemaining, staminaFill: s.staminaFill });
      if (s.introRunning || s.dungeonTransitioning) { await page.waitForTimeout(20); continue; }
      if (s.encounterReading) { await key(page, 'Enter'); await fight(page); continue; }
      if (s.finished) {
        if (s.hp > 0 && s.defeated < 6) {
          if (!readyRemaining(s)) { await page.waitForTimeout(20); continue; }
          await continueReady(api, page, action => receipt.actions.push({ at: Date.now(), kind: 'READY', ...action }));
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
      // A slow Unity clock can make a wall-clock delay shorter than stamina recovery.
      // Normal must not accumulate a guard break and enter Slayer without a parry.
      const normalReady = mode !== 'normal' || (s.staminaFill >= 1 && s.comboRemaining <= 0);
      if (normalReady && s.current && !s.hitLocked && !s.playerHitStateActive && !s.pendingAttack && Date.now() - lastTyped > (s.slayer ? 1600 : 4000)) {
        await page.keyboard.type(s.current, { delay: 25 }); lastTyped = Date.now();
        receipt.actions.push({ at: lastTyped, kind: 'typed', frame: s.frame });
      }
      await page.waitForTimeout(15);
    }
  } catch (error) {
    receipt.error = String(error.stack); failure = error;
  } finally {
    // Drain exactly once before the verdict, retaining the original input error.
    try { if (begun) await collect(); }
    catch (error) { receipt.finalDrainError = String(error.stack); failure ??= error; }
    if (begun && receipt.options.verifyCombatSerialization) {
      try { receipt.serializationChecks = await page.evaluate(() => window.typingHeroCombatSerializationChecks || 0); }
      catch (error) { receipt.serializationCheckError = String(error.stack); failure ??= error; }
    }
    summarize();
    if (begun && api.foregroundVerdict) {
      try {
        receipt.windowsForeground = await api.foregroundVerdict({ from: begun, to: Date.now() });
        if (!receipt.windowsForeground.pass) {
          receipt.summary.valid = false;
          receipt.summary.invalidReasons.push('task browser lost actual Windows foreground');
        }
      } catch (error) { receipt.foregroundError = String(error.stack); failure ??= error; }
    }
    if (!failure) {
      try {
        check(diagnostic || !!api.foregroundVerdict, 'official runs require actual Windows foreground evidence');
        if (report.exceptions) check(report.exceptions.length === 0, 'no runtime exceptions before final verdict');
        if (receipt.options.verifyCombatSerialization) check(rows.length > 0 && receipt.serializationChecks >= rows.length, 'original serializer checked for every observed frame');
        check(receipt.summary.valid, 'continuous, visible, focused Unity frame coverage');
        check(receipt.summary.combatMs >= duration, 'required combat duration');
        const coverage = receipt.summary.coverage;
        check(mode !== 'parry' || (coverage.parry && coverage.slayer), 'actual parry and Slayer coverage');
        check(mode !== 'normal' || (!coverage.parry && !coverage.slayer && !coverage.parryState), 'normal excludes parry/Slayer');
        if (!diagnostic) check(receipt.summary.thresholdsMet, 'each run meets FPS >=59 / p95 <=18ms / no rolling 1s combat window below 40FPS');
      } catch (error) { receipt.error = String(error.stack); failure = error; }
    }
    receipt.status = receipt.preflight?.status==='PREFLIGHT_BLOCKED' ? 'PREFLIGHT_BLOCKED' : failure ? 'failed' : 'passed';
    try { save(); }
    catch (error) {
      receipt.saveError = String(error.stack); failure ??= error; receipt.status = 'failed';
    } finally {
      report.timing = { ...receipt.summary, status: receipt.status, diagnostic, file: filename,
        error: receipt.error, finalDrainError: receipt.finalDrainError, saveError: receipt.saveError,
        serializationChecks: receipt.serializationChecks, serializationCheckError: receipt.serializationCheckError,
        windowsForeground: receipt.windowsForeground, foregroundError: receipt.foregroundError };
      report.status = receipt.status;
      // The native adapter persists the same verdict before browser teardown.
      try { if (api.saveReport) await api.saveReport(); }
      catch (error) {
        receipt.reportSaveError = report.timing.reportSaveError = String(error.stack);
        failure ??= error;
        receipt.status = report.status = report.timing.status = 'failed';
        try { save(); } catch (saveError) { receipt.saveError ??= String(saveError.stack); }
      } finally {
        try {
          if (api.closeContext) await api.closeContext(context);
          else await context.close();
        } catch (error) { report.closeError = String(error.stack); }
      }
    }
  }
  if (failure) throw failure;
};
