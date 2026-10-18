// Browser acceptance: observation through the QA bridge; every action is native input.
module.exports = async function tutorialFlow(api, page, options = {}) {
  const { state, until, clickControl, check, report } = api;
  const reading = lesson => until(page, s => s.tutorial && s.tutorialLesson === lesson &&
    !s.tutorialPracticing && !s.tutorialPassed && s.timeScale === 0 && !s.paused);
  const practice = async lesson => {
    await page.keyboard.press('Enter', { delay: 120 });
    return until(page, s => s.tutorialLesson === lesson && s.tutorialPracticing);
  };
  const next = async lesson => {
    await page.keyboard.press('Enter', { delay: 120 });
    return reading(lesson);
  };
  const finishWord = async () => {
    const s = await until(page, s => s.current.length > 0);
    await page.keyboard.type(s.current, { delay: 110 });
  };
  const parry = async (lesson, count, prepareOnRetry) => {
    let lastPress = -Infinity;
    let attempts = 0;
    const deadline = performance.now() + 45000;
    report.tutorialParryTrace ||= [];
    while (performance.now() < deadline) {
      const s = await state(page);
      if (report.tutorialParryTrace.at(-1)?.frame !== s.frame)
        report.tutorialParryTrace.push({frame:s.frame, lesson:s.tutorialLesson, time:s.combatSeconds,
          parries:s.matchParries, pose:s.parryPose, window:s.parryWindow, cue:s.cue,
          animation:s.enemyAnimationTime, frameMs:s.frameMs, ageMs:s.qaAgeMs});
      if (s.matchParries >= count) return s;
      if (s.tutorial && s.tutorialLesson === lesson && s.tutorialRetry && !s.tutorialPracticing && !s.tutorialPassed) {
        check(s.hp === 40 && s.matchParries === 0, 'a driver miss retries the same lesson with full HP and zero parries');
        if (++attempts > 6) throw new Error('Keyboard driver exceeded six retry attempts: ' + JSON.stringify(s));
        report.tutorialDriverRetries = (report.tutorialDriverRetries || 0) + 1;
        await practice(lesson);
        if (prepareOnRetry) await prepareOnRetry();
        continue;
      }
      assertPractice(s, lesson);
      const impactIn = 1 - ((s.enemyAnimationTime % 1) * 1.3333334 + s.qaAgeMs / 1000 * s.timeScale);
      if (s.cue && s.canParry && s.enemyAttacking && impactIn < 1.5 * s.frameMs / 1000 + .12 && impactIn > .04 &&
          performance.now() - lastPress > 350) {
        report.tutorialParryTrace.push({pressAt:s.combatSeconds, impactIn, frameMs:s.frameMs, ageMs:s.qaAgeMs});
        await page.keyboard.down('Space');
        try { await until(page, t => t.parryPose || t.parryWindow || t.matchParries >= count || !t.tutorialPracticing, 1500); }
        finally { await page.keyboard.up('Space'); }
        lastPress = performance.now();
      }
      await page.waitForTimeout(25);
    }
    throw new Error('Real parry did not complete: ' + JSON.stringify(await state(page)));
  };
  const assertPractice = (s, lesson) => {
    if (!s.tutorial || s.tutorialLesson !== lesson || (!s.tutorialPracticing && !s.tutorialPassed))
      throw new Error('Unexpected lesson retry during parry: ' + JSON.stringify(s));
  };
  const pauseHold = async (readingPhase, label) => {
    await page.keyboard.press('Escape', { delay: 120 });
    const before = await until(page, s => s.paused);
    await page.keyboard.press('Enter', { delay: 120 });
    await page.keyboard.type('blade', { delay: 40 });
    await page.waitForTimeout(1200);
    const held = await state(page);
    check(held.paused && held.hp === before.hp && held.stamina === before.stamina &&
      held.inputLetters === before.inputLetters && held.matchParries === before.matchParries &&
      held.slayer === before.slayer && held.wordTime === before.wordTime,
    label + ' holds input, HP, timer, parries, stamina and Slayer');
    await page.keyboard.press('Escape', { delay: 120 });
    await until(page, s => !s.paused && !s.controls.some(c => c.name === 'ResumeButton') && s.timeScale === (readingPhase ? 0 : 1));
  };

  const writes = report.fixtureWrites;
  let s = await state(page);
  check(s.scene === 'MainMenu' && s.tutorialStatus === 1, 'skipped preference returns to menu');
  await clickControl(page, 'PlayButton', 'button');
  s = await until(page, s => s.scene === 'SampleScene' && !s.tutorial && s.hp === 40);
  check(s.enemyHp === 850 && s.stamina === 250 && s.score === 0, 'Play after Skip goes directly to clean battle');
  await page.keyboard.press('Escape', { delay: 120 }); await until(page, s => s.paused);
  await page.keyboard.press('m', { delay: 120 }); await until(page, s => s.scene === 'MainMenu');
  await clickControl(page, 'TutorialButton', 'button');
  await reading(0); await practice(0); await finishWord();
  s = await until(page, s => s.tutorialPassed);
  check(s.enemyHp < 850, 'replay also requires a real attack hit');

  await next(1);
  for (const size of [{width:960,height:540},{width:1280,height:720},{width:1920,height:1080},{width:640,height:360}]) {
    await page.setViewportSize(size);
    s = await until(page, s => s.width === size.width && s.height === size.height);
    await until(page,t=>t.frame>=s.frame+4); s=await state(page);
    require('./combat_hud.cjs')(s,check,'normal '+size.width);
    (report.scoreGeometry ||= []).push({mode:'normal',width:s.width,height:s.height,score:s.scoreRect,tray:s.trayRect,words:s.wordRects});
    check(s.tutorialLesson === 1 && s.timeScale === 0 && s.hp === 40, 'tutorial reading at ' + size.width + 'x' + size.height);
    await page.screenshot({ path: report.outputDirectory + '/tutorial-' + size.width + 'x' + size.height + '.png' });
  }
  await page.setViewportSize({width:960,height:600}); await until(page, s => s.width === 960 && s.height === 600);
  await practice(1); await page.keyboard.type('Bx', {delay:100});
  s = await until(page, s => s.inputLetters >= 2 || s.tutorialRetry);
  check(s.current === 'BLADE' && s.wordTime < 3 && s.wordTime > 0, 'accuracy mistake resets word without refilling three-second timer');
  s = await reading(1);
  check(s.tutorialRetry && s.hp === 40 && s.inputLetters === 0, 'timeout retries accuracy from its checkpoint at full HP');
  await practice(1); await finishWord();
  await until(page, s => s.current === 'STEEL'); await finishWord();
  await until(page, s => s.tutorialPassed);
  check(true, 'two timed words complete accuracy lesson');

  await next(2); await practice(2);
  await until(page, s => s.cue);
  await page.screenshot({path:report.outputDirectory + '/tutorial-cue.png'});
  s = await reading(2);
  check(s.tutorialRetry && s.hp === 40 && s.matchParries === 0, 'missed parry retries only parry lesson at full HP');
  await practice(2); await page.keyboard.type('blade', {delay:40});
  check((await state(page)).inputLetters === 0, 'parry lesson ignores attack typing');
  await parry(2, 1);
  if (process.env.TYPINGHERO_HEADED === '1') await api.focus(page, false);
  await pauseHold(false, 'parry practice pause');
  await parry(2, 2); await until(page, s => s.tutorialPassed);
  check(true, 'two real combat parries complete lesson');

  s = await next(3);
  check(s.stamina === 25, 'stamina break prepares 25 of 250 stamina');
  await page.screenshot({path:report.outputDirectory + '/tutorial-stamina.png'});
  await practice(3); await parry(3, 1); s = await reading(4);
  check(s.stamina === 0 && s.stunned && s.slayer, 'last real parry stuns and activates Slayer');
  await page.waitForTimeout(5500);
  check((await state(page)).slayer, 'reading preserves the real Slayer duration');
  for(const [width,height] of [[960,540],[1280,720],[1920,1080],[640,360]]) {
    await page.setViewportSize({width,height});
    const resized=await until(page,s=>s.width===width && s.height===height);
    await until(page,s=>s.frame>=resized.frame+4);
    const frame=await state(page), scale=height/540;
    require('./combat_hud.cjs')(frame,check,'Slayer '+width);
    (report.scoreGeometry ||= []).push({mode:'slayer',width,height,score:frame.scoreRect,tray:frame.trayRect,words:frame.wordRects});
    (report.slayerReadingSizes ||= []).push({width,height,topGap:(frame.barTopEdge.y-frame.bodyHead.y)/scale,bottomGap:(frame.bodyFoot.y-frame.barBottomEdge.y)/scale});
    check(frame.scorePoint.y<frame.barBottomEdge.y && frame.trayPoint.y<frame.barBottomEdge.y,`${width}: score and typing sit on lower black bar`);
    check(frame.slayer && !frame.environmentVisible && frame.redAlpha===1,`${width}: tutorial preserves cinematic red`);
    check(Math.abs(frame.barLeft)<1 && Math.abs(frame.barRight-width)<1,`${width}: tutorial bars span screen`);
    check(Math.abs((frame.barTopEdge.y-frame.bodyHead.y)/scale-8)<1.5 && Math.abs((frame.bodyFoot.y-frame.barBottomEdge.y)/scale-8)<1.5,
      `${width}: tutorial body framing keeps eight pixel padding`);
    await page.screenshot({path:report.outputDirectory+`/slayer-reading-${width}x${height}.png`});
  }
  if(options.stopAfterSlayerLayout) return;
  await page.setViewportSize({width:960,height:600});
  await until(page,s=>s.width===960 && s.height===600);

  if (process.env.TYPINGHERO_HEADED === '1') await api.focus(page, true);
  await pauseHold(true, 'Slayer reading pause');
  await page.keyboard.press('Escape', {delay:120}); await until(page, s => s.paused);
  await page.keyboard.press('r', {delay:120}); s = await reading(3);
  check(s.hp === 40 && s.stamina === 25 && !s.slayer, 'Restart during Slayer reconstructs stamina break checkpoint');
  await practice(3); await parry(3, 1); await reading(4);
  await practice(4); s = await reading(3);
  check(s.tutorialRetry && s.hp === 40 && s.stamina === 25, 'Slayer expiry repeats stamina break');
  await practice(3); await parry(3, 1); await reading(4);
  await practice(4);
  if (process.env.TYPINGHERO_HEADED === '1') await api.focus(page, false);
  await pauseHold(false, 'Slayer practice pause');
  await page.keyboard.type('FxFIRE', {delay:120});
  s = await until(page, s => s.tutorialLesson === 4 && s.tutorialPassed);
  check(s.enemyHp < 850 && s.slayer, 'FIRE completion and actual Slayer hit complete lesson despite retyping');
  await page.screenshot({path:report.outputDirectory + '/tutorial-slayer.png'});

  await next(5); await practice(5); s = await reading(5);
  check(s.tutorialRetry && s.hp === 40 && s.matchParries === 0, 'getting hit retries mixed challenge from start');
  await practice(5); await finishWord();
  await parry(5, 1, finishWord);
  await until(page, s => s.current.length > 0); await finishWord();
  s = await until(page, s => s.tutorialLesson === 5 && s.tutorialPassed);
  check(s.matchParries >= 1 && s.tutorialStatus === 2, 'two words and a parry complete tutorial and save completion');
  await page.screenshot({path:report.outputDirectory + '/tutorial-complete.png'});
  check(report.fixtureWrites === writes, 'all tutorial lessons and retries produce zero score writes');
  await clickControl(page, 'PracticeButton', 'button');
  s = await until(page, s => !s.tutorial && s.scene === 'SampleScene' && s.hp === 40 && s.enemyHp === 850 && s.score === 0);
  check(s.stamina === 250 && s.inputLetters === 0 && s.inputCorrect === 0 && s.matchParries === 0 &&
    s.combatSeconds < 1 && s.finalScore === 0 && !s.slayer && !s.finished && !s.parryPose && !s.pendingAttack,
    'Start Battle loads fresh HP, stamina, score, statistics and combat lifecycle');

  await page.keyboard.press('Escape', {delay:120}); await until(page, s => s.paused);
  await page.keyboard.press('m', {delay:120}); await until(page, s => s.scene === 'MainMenu');
  await page.reload(); await page.waitForFunction(() => window.typingHeroQA?.frame > 0, null, {timeout:120000});
  check((await state(page)).tutorialStatus === 2, 'tutorial completion survives browser reload');
  await page.screenshot({path:report.outputDirectory + '/tutorial-menu.png'});
  await clickControl(page, 'TutorialButton', 'button'); await reading(0);
  await page.keyboard.press('Escape', {delay:120}); await until(page, s => s.paused);
  await page.keyboard.press('r', {delay:120}); s = await reading(0);
  check(s.hp === 40 && s.enemyHp === 850 && s.score === 0, 'Restart preserves tutorial mode and resets attempt');
  await clickControl(page, 'Skip TutorialButton', 'button');
  await until(page, s => !s.tutorial && s.hp === 40);
  check((await state(page)).tutorialStatus === 2, 'skipping replay cannot erase completion');
  await page.keyboard.press('Escape', {delay:120}); await until(page, s => s.paused);
  await page.keyboard.press('m', {delay:120}); await until(page, s => s.scene === 'MainMenu');
  console.log('PASS tutorial acceptance: six lessons, retries, preferences, pause, restart, clean battle, zero writes');
};
