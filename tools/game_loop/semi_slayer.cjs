const path = require('node:path');

module.exports = async function semiSlayer({session,start,state,until,clickControl,check,report},browser) {
  const {context,page}=await session(browser,{width:960,height:540},1000);
  try {
    let s=await start(page,false);
    if (s.tutorial) {
      await page.screenshot({path:path.join(report.outputDirectory,'semi-slayer-tutorial-entry.png')});
      report.tutorialSteps=[];
      for (let step=0;step<8&&s.tutorial;step++) {
        const previousStep=s.tutorialStep;
        await clickControl(page,'Start Battle · EnterButton','button');
        s=await until(page,t=>!t.tutorial||t.tutorialStep!==previousStep,5000);
        report.tutorialSteps.push({before:previousStep,after:s.tutorialStep,tutorial:s.tutorial});
      }
    }
    const inCombat=t=>t.scene==='SampleScene'&&!t.tutorial&&!t.finished&&t.hp>0&&
      t.current&&t.current!=='READY'&&t.canParry&&!t.paused&&!t.dungeonTransitioning&&
      !t.introInputBlocked&&t.timeScale===1;
    s=await until(page,inCombat,30000);
    let lastParryAt=0, wordsTyped=0, continues=0;
    report.parryAttempts=[];
    const deadline=performance.now()+240000;
    while (performance.now()<deadline) {
      s=await state(page);
      if (s.hp<=0) throw new Error('Player died before Semi Slayer charge was ready: '+JSON.stringify(s));
      if (s.finished || s.current==='READY') {
        const ready=await until(page,t=>t.controls.some(c=>c.name==='ContinueButton')||t.hp<=0||t.overlay,30000);
        if (!ready.controls.some(c=>c.name==='ContinueButton'))
          throw new Error('Run ended before charge and a safe target were available: '+JSON.stringify(ready));
        const chargeBefore=ready.semiSlayerCharge;
        await clickControl(page,'ContinueButton','button');
        s=await until(page,inCombat,30000);
        continues++;
        check(s.semiSlayerCharge>=chargeBefore,'Semi Slayer charge survives Continue');
        continue;
      }

      if (s.semiSlayerCharge>=100 && s.enemyHp>=70 && s.comboDecayValue>0.05) break;

      const attackPhase=s.enemyAnimationTime%1;
      const impactIn=1-(attackPhase*1.3333334+s.qaAgeMs/1000*s.timeScale);
      if (s.canParry && s.enemyAttacking && impactIn>0.04 &&
          impactIn<=2*Math.min(s.frameMs,50)/1000+0.12 && performance.now()-lastParryAt>450) {
        const before=s.matchParries,chargeBefore=s.semiSlayerCharge,wasSlayer=s.slayer;
        await page.keyboard.press('Space',{delay:100});
        lastParryAt=performance.now();
        let after=await state(page);
        for(let sample=0;sample<8&&after.matchParries===before;sample++) {
          await page.waitForTimeout(35);
          after=await state(page);
        }
        const attempt={frame:s.frame,impactIn,chargeBefore,chargeAfter:after.semiSlayerCharge,
          parriesBefore:before,parriesAfter:after.matchParries,success:after.matchParries>before};
        report.parryAttempts.push(attempt);
        if (after.matchParries>before) {
          report.parryChargeEvents=(report.parryChargeEvents||0)+1;
          if (wasSlayer)
            check(after.semiSlayerCharge===chargeBefore,'a parry during full Slayer adds no Semi Slayer charge');
          else if (!after.slayer)
            check(after.semiSlayerCharge===Math.min(100,chargeBefore+20),'successful parry adds 20 charge');
        }
        continue;
      }

      if (s.current && s.current!=='READY') {
        const beforeWords=s.words,word=s.current,chargeBefore=s.semiSlayerCharge,wasSlayer=s.slayer;
        await page.keyboard.type(word,{delay:8});
        s=await until(page,t=>t.scene==='SampleScene'&&(t.words>beforeWords||t.finished),8000);
        if (wasSlayer&&s.slayer)
          check(s.semiSlayerCharge===chargeBefore,'a word completed during full Slayer adds no Semi Slayer charge');
        else if (!wasSlayer&&!s.slayer) {
          const expectedCharge=Math.min(100,chargeBefore+10);
          const attempt={word,beforeWords,afterWords:s.words,chargeBefore,chargeAfter:s.semiSlayerCharge,
            expectedCharge,scene:s.scene,slayer:s.slayer,tutorial:s.tutorial,paused:s.paused,
            finished:s.finished,hp:s.hp,enemyHp:s.enemyHp,enemyAttacking:s.enemyAttacking,
            dungeonTransitioning:s.dungeonTransitioning,introInputBlocked:s.introInputBlocked,
            typingMuted:s.typingMuted,nextProgress:s.nextProgress,frame:s.frame};
          report.chargeAttempts ||= [];
          report.chargeAttempts.push(attempt);
          check(s.semiSlayerCharge===expectedCharge,
            'completed normal word adds 10 charge: '+JSON.stringify(attempt));
        }
        check(s.semiSlayerCharge<=100,'charge is capped at 100');
        wordsTyped++;
        report.chargeTrace ||= [];
        report.chargeTrace.push({charge:s.semiSlayerCharge,enemyHp:s.enemyHp,words:s.words,parries:s.matchParries,slot:s.encounterText});
      } else await page.waitForTimeout(80);
    }
    check(performance.now()<deadline,'charge collection reaches a viable target before the run deadline');
    s=await state(page);
    if (s.semiSlayerCharge<100 || s.enemyHp<70 || s.current==='READY' || s.finished)
      throw new Error('Could not arrange a charged combat target with enough health: '+JSON.stringify(s));

    report.wordsTypedForCharge=wordsTyped;
    report.continues=continues;
    report.skillEnemyStartHp=s.enemyHp;
    report.skillWord=s.current;
    if (s.comboDecayValue<=0.05) {
      const beforeWords=s.words, word=s.current;
      await page.keyboard.type(word,{delay:8});
      s=await until(page,t=>t.scene==='SampleScene'&&(t.words>beforeWords||t.finished),8000);
    }
    if (s.enemyHp<60 || s.finished)
      throw new Error('Preparing a live combo left too little target health for ten hits: '+JSON.stringify(s));
    check(s.comboDecayValue>0.05,'a live combo window is present before activation');

    for (const viewport of [{width:320,height:180},{width:640,height:360}]) {
      await page.setViewportSize(viewport);
      const view=await until(page,t=>t.width===viewport.width&&t.height===viewport.height&&
        t.semiSlayerFill>0.99&&t.semiSlayerRect.width>0,15000);
      check(view.semiSlayerFill>0.99,'charge bar is visibly full at '+viewport.width+'x'+viewport.height);
      report.hudViews ||= [];
      report.hudViews.push({viewport:`${viewport.width}x${viewport.height}`,rect:view.semiSlayerRect,
        playerStatus:view.playerStatusRect,typingTray:view.trayRect});
      await page.screenshot({path:path.join(report.outputDirectory,`semi-slayer-hud-${viewport.width}x${viewport.height}.png`)});
    }
    await page.setViewportSize({width:960,height:540});
    s=await until(page,t=>t.width===960&&t.height===540,15000);

    // A press during pause is rejected and is not replayed after resume.
    await page.keyboard.press('Escape');
    const paused=await until(page,t=>t.paused&&t.timeScale===0,5000);
    await page.keyboard.press('Shift');
    await page.waitForTimeout(250);
    const blocked=await state(page);
    check(blocked.semiSlayerCharge===100&&!blocked.semiSlayerActive,'Shift during Pause is not buffered');
    await page.keyboard.press('Escape');
    s=await until(page,t=>!t.paused&&t.timeScale===1,5000);
    await page.waitForTimeout(180);
    s=await state(page);
    check(s.semiSlayerCharge===100&&!s.semiSlayerActive,'Pause resume does not replay a blocked Shift');

    const activationSafe=t=>!t.paused&&t.timeScale===1&&!t.dungeonTransitioning&&!t.introInputBlocked&&
      !t.hitLocked&&!t.deadLocked&&!t.parryPose&&!t.playerHitStateActive&&!t.enemyAttacking&&t.hp>0;
    s=await until(page,activationSafe,10000);
    if (s.comboDecayValue<=0.05) {
      const beforeWords=s.words, word=s.current;
      await page.keyboard.type(word,{delay:8});
      s=await until(page,t=>t.scene==='SampleScene'&&(t.words>beforeWords||t.finished),8000);
      s=await until(page,activationSafe,10000);
    }
    const baseline={current:s.current,wordTime:s.wordTime,combo:s.comboDecayValue,score:s.score,
      combatSeconds:s.combatSeconds,enemyHp:s.enemyHp,charge:s.semiSlayerCharge,stamina:s.stamina};
    await page.keyboard.down('Shift');
    s=await until(page,t=>t.semiSlayerActive&&t.semiSlayerSwings>=2,5000);
    const middle=await until(page,t=>t.semiSlayerActive&&t.semiSlayerSwings>=3,5000);
    report.chainTiming={baseline:{current:baseline.current,wordTime:baseline.wordTime},
      middle:{current:middle.current,wordTime:middle.wordTime,frame:middle.frame,
        typingHeld:middle.typingHeld,semiSlayerActive:middle.semiSlayerActive}};
    check(middle.semiSlayerEnemyHeld&&middle.typingHeld,'enemy and typing are held during the chain');
    check(middle.current===baseline.current&&Math.abs(middle.wordTime-baseline.wordTime)<0.005,
      'current word and timer stay fixed during the chain: '+JSON.stringify(report.chainTiming));
    check(Math.abs(middle.comboDecayValue-baseline.combo)<0.005,'combo window stays fixed during the chain');
    check(middle.score===baseline.score&&middle.semiSlayerCharge===0,'Semi Slayer adds no score and consumes charge at activation');
    check(middle.combatSeconds>baseline.combatSeconds+0.05,'combat timer continues during Semi Slayer');
    check(!middle.slayer&&middle.redAlpha<0.001&&middle.slayerPresentationWeight<0.001,
      'Semi Slayer leaves global Slayer presentation off');
    check(middle.timeScale===1,'Semi Slayer hits do not trigger hitstop');

    const beforeStall=await state(page);
    await page.evaluate(()=>{const until=performance.now()+250;while(performance.now()<until){}});
    const afterStall=await until(page,t=>t.frame>beforeStall.frame,5000);
    check(afterStall.semiSlayerSwings<=beforeStall.semiSlayerSwings+1,
      'a slow frame does not catch up with multiple Semi Slayer swings');

    await page.keyboard.press('Escape');
    const pausedDuring=await until(page,t=>t.paused&&t.timeScale===0&&t.semiSlayerActive,5000);
    const pausedSwings=pausedDuring.semiSlayerSwings, pausedHits=pausedDuring.semiSlayerHits;
    await page.waitForTimeout(350);
    const held=await state(page);
    check(held.semiSlayerActive&&held.semiSlayerSwings===pausedSwings&&held.semiSlayerHits===pausedHits,
      'Pause holds the remaining strikes without catch-up');
    check(Math.abs(held.wordTime-pausedDuring.wordTime)<0.005&&
      Math.abs(held.comboDecayValue-pausedDuring.comboDecayValue)<0.005,
      'word and combo clocks stay held during Pause');
    check(Math.abs(held.combatSeconds-pausedDuring.combatSeconds)<0.005,'combat timer stops only during Pause');
    await page.keyboard.press('Escape');
    await until(page,t=>!t.paused&&t.timeScale===1,5000);
    const swingDeadline=performance.now()+5000;
    let lastSwing;
    while (performance.now()<swingDeadline) {
      lastSwing=await state(page);
      if (lastSwing.semiSlayerActive&&lastSwing.semiSlayerSwings===10) break;
      await page.waitForTimeout(10);
    }
    check(lastSwing&&lastSwing.semiSlayerActive&&lastSwing.semiSlayerSwings===10,
      'the tenth strike remains observable before skill cleanup');
    const completed=await until(page,t=>!t.semiSlayerActive&&t.semiSlayerSwings===10,5000);
    await page.keyboard.up('Shift');
    await page.waitForTimeout(250);
    const settled=await state(page);

    check(lastSwing.semiSlayerHits===10&&lastSwing.semiSlayerHitAttackIds.length===10,
      'ten separate Semi Slayer hits reach the enemy');
    check(new Set(lastSwing.semiSlayerHitAttackIds).size===10,'each swing produces one distinct attack hit');
    check(lastSwing.semiSlayerHitFrames.length===10&&new Set(lastSwing.semiSlayerHitFrames).size===10,
      'each attack hit is visible in one separate frame');
    const hitGaps=lastSwing.semiSlayerHitTimes.slice(1).map((time,index)=>time-lastSwing.semiSlayerHitTimes[index]);
    check(hitGaps.length===9&&hitGaps.every(gap=>gap>=0.075),'the ten hits keep at least the planned 100 ms cadence without catch-up');
    check(baseline.enemyHp-lastSwing.enemyHp===50,'ten hits deal exactly 50 HP total');
    check(lastSwing.stamina>=baseline.stamina,
      'Semi Slayer hits do not drain enemy stamina: '+baseline.stamina+' -> '+lastSwing.stamina);
    check(settled.semiSlayerCharge===0&&!settled.semiSlayerActive&&settled.semiSlayerSwings===10,
      'a held Shift does not automatically reuse the skill');
    check(lastSwing.current===baseline.current&&Math.abs(lastSwing.wordTime-baseline.wordTime)<0.005&&
      Math.abs(lastSwing.comboDecayValue-baseline.combo)<0.02&&lastSwing.score===baseline.score,
      'typing progress, word timer, combo timer and score stay fixed through the last strike');
    check(!settled.semiSlayerEnemyHeld&&!settled.typingHeld,'normal input and enemy attack schedule are released after the chain');
    report.hitGaps=hitGaps;
    report.hitFrames=lastSwing.semiSlayerHitFrames;
    report.hitAttackIds=lastSwing.semiSlayerHitAttackIds;
    report.skillStartStamina=baseline.stamina;
    report.skillEndStamina=lastSwing.stamina;
    report.skillStartHp=baseline.enemyHp;
    report.skillEndHp=lastSwing.enemyHp;
    report.pausedSwingCount=pausedSwings;
    report.wordTimeDelta=lastSwing.wordTime-baseline.wordTime;
    report.comboDelta=lastSwing.comboDecayValue-baseline.combo;
    await page.screenshot({path:path.join(report.outputDirectory,'semi-slayer-finish-960x540.png')});
  } finally { await context.close(); }
};
