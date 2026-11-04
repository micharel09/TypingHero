const path = require('node:path');
const {installObservationJSON}=require('./dungeon_input.cjs');

module.exports = async function semiSlayer({session,start,state,until,clickControl,check,report},browser) {
  const {context,page}=await session(browser,{width:960,height:540},1000);
  await page.evaluate(installObservationJSON);
  const video=page.video();
  let stopDefense=false, defenseEnabled=false, defenseFailure=null, defense=Promise.resolve();
  try {
    const scenarioDifficulty=process.env.TYPINGHERO_SEMI_SLAYER_DIFFICULTY;
    if(scenarioDifficulty) {
      const levels=['very_easy','easy','normal','hard','very_hard'];
      check(levels.includes(scenarioDifficulty),'scenario difficulty is supported');
      await until(page,t=>t.scene==='MainMenu');
      let initialMenu=await state(page);
      if(!initialMenu.controls.some(c=>c.kind==='input')) {
        await clickControl(page,'PlayButton','button');
        await until(page,t=>t.controls.some(c=>c.kind==='input'));
      }
      let menu=await state(page);
      let steps=0;
      while(menu.menuDifficulty!==scenarioDifficulty) {
        check(steps++<levels.length,'difficulty selector reaches the requested scenario level');
        const before=menu.menuDifficulty;
        await clickControl(page,'>Button','button');
        menu=await until(page,t=>t.menuDifficulty!==before);
      }
      check(menu.menuDifficulty===scenarioDifficulty&&menu.scene==='MainMenu',
        'Semi Slayer scenario uses the selected Endless difficulty');
      report.scenarioDifficulty=scenarioDifficulty;
    }
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
    const terminal=t=>t.scene==='SampleScene'&&!t.dungeonTransitioning&&!t.introInputBlocked&&
      (t.hp<=0||t.runCompleted);
    const failOnTerminal=(phase,t)=>{
      if(!terminal(t)) return false;
      report.terminalStates ||= [];
      report.terminalStates.push({phase,hp:t.hp,runCompleted:t.runCompleted,
        defeated:t.defeated,opponent:t.opponent,current:t.current});
      check(false,'terminal death or boss win ends the scenario before the lethal skill during '+phase+': '+JSON.stringify(t));
      return true;
    };
    s=await until(page,inCombat,30000);
    let lastParryAt=0, wordsTyped=0, continues=0;
    report.parryAttempts=[];
    report.readyTransitions=[];
    report.terminalStates=[];
    defense=(async()=>{
      while(!stopDefense) {
        if(!defenseEnabled) { await page.waitForTimeout(50); continue; }
        try {
          const cue=await page.waitForFunction(()=>{
            const f=window.typingHeroCombatFrame;
            if(!f||!f.canParry||f.parryWindow||!f.enemyAttacking||f.slayer||f.timeScale<=0) return false;
            const age=Math.max(0,(performance.now()-window.typingHeroCombatFramePublishedAt)/1000);
            const contact=f.enemyContactIn-age*f.timeScale;
            return contact>0&&contact<=.25?{frame:f.frame,contact,age,timeScale:f.timeScale}:false;
          },null,{polling:'raf',timeout:500});
          const cueSample=await cue.jsonValue();
          await cue.dispose();
          if(stopDefense) continue;
          const before=await state(page);
          lastParryAt=performance.now();
          await page.keyboard.down('Space');
          try {
            await page.waitForFunction(frame=>window.typingHeroCombatFrame?.frame>frame,
              cueSample.frame,{polling:'raf',timeout:1200});
          }
          catch(error) { if(error.name!=='TimeoutError') throw error; }
          finally { await page.keyboard.up('Space'); }
          const after=await state(page);
          report.parryAttempts.push({observer:true,frame:after.frame,contact:cueSample.contact,
            contactAge:cueSample.age,hp:after.hp,
            chargeBefore:before.semiSlayerCharge,chargeAfter:after.semiSlayerCharge,
            parriesBefore:before.matchParries,parriesAfter:after.matchParries,
            success:after.matchParries>before.matchParries});
          await cue.dispose();
          const settled=await page.waitForFunction(()=>{
            const s=window.typingHeroQA;
            return !s?.cue||s?.parryPose||!s?.canParry||s?.paused||s?.finished;
          },null,{polling:'raf',timeout:500}).catch(error=>{
            if(error.name!=='TimeoutError') throw error;
            return null;
          });
          if(settled) await settled.dispose();
        } catch(error) {
          if(error.name!=='TimeoutError') { defenseFailure=error; return; }
        }
      }
    })();
    const deadline=performance.now()+240000;
    async function continueWithReady(phase) {
      let ready=await until(page,t=>t.current==='READY'||terminal(t),30000);
      if(terminal(ready)) {
        report.terminalStates.push({phase,hp:ready.hp,runCompleted:ready.runCompleted,
          defeated:ready.defeated,opponent:ready.opponent,current:ready.current});
        check(false,'scenario reached a terminal player death or boss win before READY: '+JSON.stringify(ready));
        return ready;
      }
      const before={charge:ready.semiSlayerCharge,hp:ready.hp,round:ready.endlessRound,slot:ready.endlessSlot,
        defeated:ready.defeated,opponent:ready.opponent,enemyHp:ready.enemyHp,current:ready.current,
        scene:ready.scene};
      const attempt={phase,before};
      report.readyAttempts ||= [];
      report.readyAttempts.push(attempt);
      await page.keyboard.type('READY',{delay:45});
      const activeAfterReady=t=>t.scene==='SampleScene'&&!t.tutorial&&!t.finished&&t.hp>0&&
        t.current&&t.current!=='READY'&&!t.paused&&!t.dungeonTransitioning&&
        !t.introInputBlocked&&t.enemyHp>0&&
        (t.defeated>before.defeated||t.enemyHp>before.enemyHp||t.current!==before.current);
      const after=await until(page,t=>terminal(t)||activeAfterReady(t),30000);
      if(terminal(after)) {
        attempt.terminal={charge:after.semiSlayerCharge,hp:after.hp,runCompleted:after.runCompleted,
          round:after.endlessRound,slot:after.endlessSlot,defeated:after.defeated,
          opponent:after.opponent,current:after.current,enemyHp:after.enemyHp,scene:after.scene};
        const nextEncounterLoaded=after.scene==='SampleScene'&&after.current&&after.current!=='READY'&&
          after.current!==before.current&&after.enemyHp>0;
        if(nextEncounterLoaded) {
          check(after.semiSlayerCharge===before.charge,
            'typed READY retains charge in the loaded encounter before terminal player death');
          check(after.endlessRound!==before.round||after.endlessSlot!==before.slot||
            after.opponent!==before.opponent||after.enemyHp>before.enemyHp,
            'typed READY loads a fresh encounter before terminal player death');
          report.readyTransitions.push({phase,before,after:{charge:after.semiSlayerCharge,
            round:after.endlessRound,slot:after.endlessSlot,defeated:after.defeated,
            opponent:after.opponent,enemyHp:after.enemyHp,current:after.current,terminal:true}});
        }
        report.terminalStates.push({phase,hp:after.hp,runCompleted:after.runCompleted,
          defeated:after.defeated,opponent:after.opponent,current:after.current});
        check(false,nextEncounterLoaded
          ?'player died after READY loaded the next encounter and before the lethal-skill sequence: '+JSON.stringify(after)
          :'terminal state arrived during READY transition: '+JSON.stringify(after));
        return after;
      }
      check(after.semiSlayerCharge===before.charge,
        'keyboard READY preserves the exact Semi Slayer charge');
      check(after.endlessRound!==before.round||after.endlessSlot!==before.slot||
        after.opponent!==before.opponent||after.enemyHp>before.enemyHp,
        'keyboard READY loads a fresh encounter');
      attempt.after={charge:after.semiSlayerCharge,hp:after.hp,round:after.endlessRound,
        slot:after.endlessSlot,defeated:after.defeated,opponent:after.opponent,
        enemyHp:after.enemyHp,current:after.current};
      report.readyTransitions.push({phase,before,after:{charge:after.semiSlayerCharge,
        round:after.endlessRound,slot:after.endlessSlot,defeated:after.defeated,opponent:after.opponent,
        enemyHp:after.enemyHp,current:after.current}});
      continues++;
      return after;
    }
    while (performance.now()<deadline) {
      s=await state(page);
      if(terminal(s)) {
        report.terminalStates.push({phase:'charge collection',hp:s.hp,runCompleted:s.runCompleted,
          defeated:s.defeated,opponent:s.opponent,current:s.current});
        check(false,'player death or boss win is terminal; no READY input is sent: '+JSON.stringify(s));
        return;
      }
      if (s.current==='READY') {
        s=await continueWithReady('charge collection');
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
        const beforeWords=s.words,word=s.current,chargeBefore=s.semiSlayerCharge,
          parriesBefore=s.matchParries,wasSlayer=s.slayer;
        await page.keyboard.type(word,{delay:8});
        s=await until(page,t=>t.scene==='SampleScene'&&(t.words>beforeWords||t.finished),8000);
        if (wasSlayer&&s.slayer)
          check(s.semiSlayerCharge===chargeBefore,'a word completed during full Slayer adds no Semi Slayer charge');
        else if (!wasSlayer&&!s.slayer) {
          const parryDelta=Math.max(0,s.matchParries-parriesBefore);
          const expectedCharge=Math.min(100,chargeBefore+10+20*parryDelta);
          const attempt={word,beforeWords,afterWords:s.words,chargeBefore,chargeAfter:s.semiSlayerCharge,
            parriesBefore,parriesAfter:s.matchParries,parryDelta,expectedCharge,scene:s.scene,
            slayer:s.slayer,tutorial:s.tutorial,paused:s.paused,
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
      !t.hitLocked&&!t.deadLocked&&!t.parryPose&&!t.playerHitStateActive&&!t.enemyAttacking&&
      !t.slayer&&t.redAlpha<0.001&&t.hp>0;
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
    s=await until(page,t=>t.semiSlayerActive&&t.semiSlayerSwings>=1,5000);
    const heldBaseline={current:s.current,wordTime:s.wordTime,combo:s.comboDecayValue,
      score:s.score,combatSeconds:s.combatSeconds,charge:s.semiSlayerCharge};
    const middle=await until(page,t=>t.semiSlayerActive&&t.semiSlayerSwings>=3,5000);
    report.chainTiming={beforeShift:{current:baseline.current,wordTime:baseline.wordTime,combo:baseline.combo},
      firstActive:{current:heldBaseline.current,wordTime:heldBaseline.wordTime,combo:heldBaseline.combo,
        combatSeconds:heldBaseline.combatSeconds,charge:heldBaseline.charge},
      inputToActiveWordTimeDelta:heldBaseline.wordTime-baseline.wordTime,
      middle:{current:middle.current,wordTime:middle.wordTime,frame:middle.frame,
        typingHeld:middle.typingHeld,semiSlayerActive:middle.semiSlayerActive}};
    check(middle.semiSlayerEnemyHeld&&middle.typingHeld,'enemy and typing are held during the chain');
    check(heldBaseline.current===baseline.current,'the current word stays fixed when Semi Slayer first becomes active');
    check(middle.current===heldBaseline.current&&Math.abs(middle.wordTime-heldBaseline.wordTime)<0.005,
      'current word and timer stay fixed during the chain: '+JSON.stringify(report.chainTiming));
    check(Math.abs(middle.comboDecayValue-heldBaseline.combo)<0.005,'combo window stays fixed during the chain');
    check(middle.score===heldBaseline.score&&middle.semiSlayerCharge===0,'Semi Slayer adds no score and consumes charge at activation');
    check(middle.combatSeconds>heldBaseline.combatSeconds+0.05,'combat timer continues during Semi Slayer');
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
    check(lastSwing.current===heldBaseline.current&&Math.abs(lastSwing.wordTime-heldBaseline.wordTime)<0.005&&
      Math.abs(lastSwing.comboDecayValue-heldBaseline.combo)<0.02&&lastSwing.score===heldBaseline.score,
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
    report.preActivationWordTimeDelta=heldBaseline.wordTime-baseline.wordTime;
    report.wordTimeDelta=lastSwing.wordTime-heldBaseline.wordTime;
    report.comboDelta=lastSwing.comboDecayValue-heldBaseline.combo;
    await page.screenshot({path:path.join(report.outputDirectory,'semi-slayer-finish-960x540.png')});

    // A nonlethal skill must hand the trail back to the ordinary next attack.
    s=await until(page,t=>!t.semiSlayerActive&&t.playerIdle&&t.hp>0&&
      !t.hitLocked&&!t.deadLocked&&!t.semiSlayerEnemyHeld&&!t.typingHeld&&t.activeSlashParts===0,5000);
    const ordinaryWord=s.current;
    const ordinaryBefore={enemyHp:s.enemyHp,charge:s.semiSlayerCharge,frame:s.frame,
      words:s.words,slayer:s.slayer};
    await page.keyboard.type(ordinaryWord,{delay:8});
    s=await until(page,t=>t.words>ordinaryBefore.words||t.current==='READY'||t.finished||terminal(t),5000);
    check(s.words>ordinaryBefore.words,'the ordinary follow-up word completes before trail verification');
    const ordinaryTrail=await until(page,t=>t.activeSlashParts>0&&!t.semiSlayerActive,5000);
    check(ordinaryTrail.activeSlashParts>0,'the ordinary attack after a nonlethal Semi skill renders its slash trail');
    check(ordinaryTrail.semiSlayerCharge===Math.min(100,ordinaryBefore.charge+10),
      'the normal follow-up uses the existing ordinary +10 charge rule');
    report.ordinaryTrail={word:ordinaryWord,before:ordinaryBefore,observed:{frame:ordinaryTrail.frame,
      activeSlashParts:ordinaryTrail.activeSlashParts,slayer:ordinaryTrail.slayer,
      semiSlayerActive:ordinaryTrail.semiSlayerActive,enemyHp:ordinaryTrail.enemyHp}};
    await page.screenshot({path:path.join(report.outputDirectory,'semi-slayer-normal-trail.png')});
    defenseEnabled=true;

    // Fight through the current encounter using the existing keyboard and parry paths.
    async function fightUntilReady(phase) {
      const fightDeadline=performance.now()+90000;
      while(performance.now()<fightDeadline) {
        let current=await state(page);
        if(terminal(current)) {
          report.terminalStates.push({phase,hp:current.hp,runCompleted:current.runCompleted,
            defeated:current.defeated,opponent:current.opponent,current:current.current});
          check(false,'terminal death or boss win ends the scenario without waiting for READY');
          return current;
        }
        if(current.current==='READY') return current;
        if(current.finished) { await page.waitForTimeout(50); continue; }

        const attackPhase=current.enemyAnimationTime%1;
        const impactIn=1-(attackPhase*1.3333334+current.qaAgeMs/1000*current.timeScale);
        if(current.canParry&&current.enemyAttacking&&impactIn>0.04&&
          impactIn<=2*Math.min(current.frameMs,50)/1000+0.12&&performance.now()-lastParryAt>450) {
          const before=current.matchParries;
          await page.keyboard.press('Space',{delay:100});
          lastParryAt=performance.now();
          const after=await state(page);
          report.parryAttempts.push({phase,frame:current.frame,impactIn,parriesBefore:before,
            parriesAfter:after.matchParries,success:after.matchParries>before});
          continue;
        }
        if(current.current) {
          const beforeWords=current.words;
          await page.keyboard.type(current.current,{delay:8});
          await until(page,t=>t.words>beforeWords||t.current==='READY'||t.finished||terminal(t),5000);
        }
        else await page.waitForTimeout(80);
      }
      throw new Error('Timed out before READY in '+phase+': '+JSON.stringify(await state(page)));
    }

    let readyState=await fightUntilReady('first post-skill encounter');
    if(terminal(readyState)) return;
    check(readyState.current==='READY','first nonlethal encounter reaches READY after normal combat');
    s=await continueWithReady('after nonlethal skill and ordinary slash');
    if(terminal(s)) return;
    check(report.readyTransitions.length>=1,'scenario advances at least one encounter using typed READY');

    // On the next live encounter, fill charge and leave 46-50 HP.
    // The ordinary 5-HP hit can step from 51 to 46; either value dies on Semi hit ten.
    let prepDeadline=performance.now()+180000;
    while(s.semiSlayerCharge<100&&performance.now()<prepDeadline) {
      if(terminal(s)) break;
      if(s.current==='READY') {
        s=await continueWithReady('charge for lethal skill');
        if(terminal(s)) return;
        continue;
      }
      if(s.finished) { await page.waitForTimeout(50); s=await state(page); continue; }
      const word=s.current;
      if(word) {
        const beforeWords=s.words;
        await page.keyboard.type(word,{delay:8});
        s=await until(page,t=>t.words>beforeWords||t.current==='READY'||t.finished||terminal(t),5000);
      } else { await page.waitForTimeout(80); s=await state(page); }
    }
    if(failOnTerminal('charge for lethal skill',s)) return;
    check(s.semiSlayerCharge===100,'charge reaches full before lethal-skill setup');
    const lethalReady=t=>!t.paused&&t.timeScale===1&&!t.dungeonTransitioning&&!t.introInputBlocked&&
      t.playerIdle&&!t.hitLocked&&!t.deadLocked&&!t.parryPose&&!t.playerHitStateActive&&
      !t.slayer&&t.redAlpha<0.001&&t.hp>0;
    let lethalBaseline=null;
    report.lethalTargetAttempts=[];
    while(!lethalBaseline&&performance.now()<prepDeadline) {
      if(failOnTerminal('lethal target setup',s)) return;
      if(s.current==='READY') {
        s=await continueWithReady('lethal-skill target setup');
        if(terminal(s)) return;
        continue;
      }
      if(s.finished) { await page.waitForTimeout(50); s=await state(page); continue; }
      if(s.enemyHp>50) {
        if(!s.playerIdle) {
          s=await until(page,t=>t.playerIdle||t.current==='READY'||t.finished||terminal(t),5000);
          continue;
        }
        const word=s.current;
        if(word) {
          const beforeWords=s.words;
          await page.keyboard.type(word,{delay:8});
          s=await until(page,t=>t.words>beforeWords||t.current==='READY'||t.finished||terminal(t),5000);
        } else { await page.waitForTimeout(80); s=await state(page); }
        continue;
      }

      // Let the last ordinary attack land before deciding how many Semi hits are lethal.
      s=await until(page,t=>terminal(t)||t.current==='READY'||t.finished||lethalReady(t),10000);
      if(terminal(s)) return;
      if(s.current==='READY'||s.finished) continue;
      const attempt={round:s.endlessRound,slot:s.endlessSlot,opponent:s.opponent,
        enemyHp:s.enemyHp,playerHp:s.hp,frame:s.frame};
      report.lethalTargetAttempts.push(attempt);
      if(Math.ceil(s.enemyHp/5)===10) {
        lethalBaseline={enemyHp:s.enemyHp,charge:s.semiSlayerCharge,hp:s.hp,
          round:s.endlessRound,slot:s.endlessSlot,opponent:s.opponent,frame:s.frame};
        break;
      }

      // If a normal hit settled below the ten-hit window, finish this duel and try the next one.
      s=await fightUntilReady('lethal target settled below 46 HP');
      if(terminal(s)) return;
      s=await continueWithReady('next lethal-skill target');
      if(terminal(s)) return;
      check(s.semiSlayerCharge===100,
        'typed READY retains full charge while moving to another lethal-skill target');
    }
    if(failOnTerminal('lethal target setup',s)) return;
    check(performance.now()<prepDeadline&&lethalBaseline!==null,
      'a stable living target requiring ten 5-HP hits is ready before the deadline: '+JSON.stringify(s));
    check(lethalBaseline.charge===100&&lethalBaseline.enemyHp>45&&lethalBaseline.enemyHp<=50&&
      lethalBaseline.hp>0,
      'lethal skill starts with exactly 100 charge and a settled target at 46-50 HP: '+JSON.stringify(lethalBaseline));
    report.lethalTargetReady=lethalBaseline;
    await page.keyboard.down('Shift');
    const lethalResult=await until(page,t=>t.semiSlayerSwings===10&&
      (!t.semiSlayerActive||t.semiSlayerHits===10),6000);
    await page.keyboard.up('Shift');
    const lethalSettled=await until(page,t=>!t.semiSlayerActive&&t.playerIdle&&t.hp>0&&
      !t.hitLocked&&!t.deadLocked&&!t.semiSlayerEnemyHeld&&!t.typingHeld,5000);
    check(lethalResult.semiSlayerHits===10&&lethalResult.semiSlayerHitAttackIds.length===10,
      'lethal skill registers ten distinct hits before idle cleanup');
    check(lethalBaseline.enemyHp>45&&lethalBaseline.enemyHp<=50&&lethalResult.enemyHp===0,
      'the tenth 5-HP hit is lethal from '+lethalBaseline.enemyHp+' HP');
    check(lethalResult.semiSlayerHitFrames.length===10&&new Set(lethalResult.semiSlayerHitFrames).size===10,
      'lethal hit sequence keeps ten distinct hit frames');
    check(lethalSettled.playerIdle&&!lethalSettled.hitLocked&&!lethalSettled.deadLocked&&
      !lethalSettled.semiSlayerEnemyHeld&&!lethalSettled.typingHeld,
      'live player returns idle and releases locks after the lethal hit');
    report.lethalSkill={baseline:lethalBaseline,hits:lethalResult.semiSlayerHitFrames.length,
      attackIds:lethalResult.semiSlayerHitAttackIds,hitFrames:lethalResult.semiSlayerHitFrames,
      end:{enemyHp:lethalResult.enemyHp,swings:lethalResult.semiSlayerSwings,
        charge:lethalSettled.semiSlayerCharge,playerIdle:lethalSettled.playerIdle,
        hitLocked:lethalSettled.hitLocked,deadLocked:lethalSettled.deadLocked,
        enemyHeld:lethalSettled.semiSlayerEnemyHeld,typingHeld:lethalSettled.typingHeld}};
    await page.screenshot({path:path.join(report.outputDirectory,'semi-slayer-lethal-idle.png')});
    check(report.readyTransitions.length>=1,'typed READY advances at least one encounter');
  } finally {
    stopDefense=true;
    await defense;
    if(defenseFailure) throw defenseFailure;
    await context.close();
    if(video) report.video=await video.path();
  }
};
