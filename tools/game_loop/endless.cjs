// All actions are real keyboard/mouse input; probe state is read-only.
const path = require('node:path');
module.exports = async function endless(api, browser) {
  if (process.env.TYPINGHERO_DIFFICULTY_MATRIX === '1' && !api.difficulty) {
    api.report.difficultyRuns=[];
    const levels=(process.env.TYPINGHERO_DIFFICULTY_LEVELS || 'very_easy,easy,normal,hard,very_hard').split(',');
    for (const difficulty of levels) {
      api.check(['very_easy','easy','normal','hard','very_hard'].includes(difficulty),'matrix level is a supported difficulty');
      const begin=performance.now();
      await module.exports({...api,difficulty,rounds:1},browser);
      api.report.difficultyRuns.push({difficulty,seconds:(performance.now()-begin)/1000,
        duels:api.report.endlessDuels,fastDuelSeconds:api.report.fastDuelSeconds,
        timing:api.report.runTiming,trace:api.report.pilotTrace});
      console.log('PASS difficulty '+difficulty);
    }
    return;
  }
  const {session,start:baseStart,state,until,check,report} = api;
  const difficulty=api.difficulty || process.env.TYPINGHERO_ENDLESS_DIFFICULTY || 'normal';
  const baseWrites=report.endlessWrites||0;
  const {context,page} = await session(browser,{width:960,height:540},1000);
  const acknowledged=async predicate=>{
    const deadline=performance.now()+5000;
    while(performance.now()<deadline){
      const observed=await state(page);
      if(predicate(observed)) return observed;
      await page.waitForTimeout(10);
    }
    throw new Error('Real input was not acknowledged: '+JSON.stringify(await state(page)));
  };
  const key = async k => {
    const before=await state(page);
    // The published probe can precede keydown by several rendered frames.
    const frame=before.frame+Math.ceil(before.qaAgeMs/Math.max(1,before.frameMs))+2;
    await page.keyboard.down(k);
    try { await page.waitForTimeout(Math.max(40,before.frameMs*2)); await acknowledged(s=>s.frame>=frame); }
    finally { await page.keyboard.up(k); }
    await page.waitForTimeout(80); // Observe released input and the updated selector before the next action.
  };
  const capture = name => page.screenshot({path:path.join(report.outputDirectory,difficulty+'-'+name+'.png')});
  const typeWordKeys=async(input,delay)=>{
    // Keep native commands short so a Space event can arrive between letters.
    for(const letter of input){
      await page.keyboard.press(letter,{delay:10});
      if(delay>10) await new Promise(resolve=>setTimeout(resolve,delay-10));
    }
  };
  let defendEnabled=false,stopDefense=false,defenseFailure=null;
  // Observe the published read-only state inside the browser at RAF frequency.
  // Only native keyboard events perform defense; no Unity method/state is changed.
  const defense=(async()=>{
    while(!stopDefense){
      if(!defendEnabled){await page.waitForTimeout(20);continue;}
      try{
        const ready=await page.waitForFunction(lateHeavy=>{
          const s=window.typingHeroQA;
          if(!s||!s.cue||!s.canParry||s.parryPose||s.paused||s.finished||s.slayer||s.timeScale<=0) return false;
          const age=(performance.now()-window.typingHeroQAPublishedAt)/1000*s.timeScale;
          const axe=/HEAVY|BERSERKER/.test(s.opponent),second=s.enemyTexture==='Attack2';
          const contact=second&&!axe&&s.enemyAnimationTime===0?.3-age:
            (second?(axe?.4:.2):(axe?.6:.5))-s.enemyAnimationTime*(second?1:1.2)-age;
          return contact>0&&contact<=(lateHeavy && axe?.035:.25);
        },process.env.TYPINGHERO_LATE_HEAVY_PARRY==='1',{polling:'raf',timeout:500});
        if(stopDefense||!defendEnabled){await ready.dispose();continue;}
        await page.keyboard.down('Space');
        try{await page.waitForTimeout(Number(process.env.TYPINGHERO_DEFENSE_HOLD_MS || 60));}finally{await page.keyboard.up('Space');}
        await ready.dispose();
        const observed=await state(page);
        (report.parryAttempts ||= []).push({observer:true,frame:observed.frame,hp:observed.hp,parries:observed.matchParries});
        await page.waitForFunction(()=>!window.typingHeroQA?.cue||window.typingHeroQA?.parryPose||!window.typingHeroQA?.canParry,
          null,{polling:'raf',timeout:500}).then(h=>h.dispose()).catch(e=>{if(e.name!=='TimeoutError')throw e;});
      }catch(e){if(e.name!=='TimeoutError'){defenseFailure=e;return;}}
    }
  })();
  // Actual unchanged clips: Attack1=1.2s, Attack2=1s. Contact events are authored
  // in EndlessAssets (sword/shield .5/.2s, axe .6/.4s); thrust holds its first cel100ms.
  const impactIn=(s,firstCueAt)=>{
    const axe=/HEAVY|BERSERKER/.test(s.opponent),second=s.enemyTexture==='Attack2';
    if(second && !axe && s.enemyAnimationTime===0)
      return .3-(performance.now()-firstCueAt)/1000;
    return (second?(axe?.4:.2):(axe?.6:.5))-s.enemyAnimationTime*(second?1:1.2)-s.qaAgeMs/1000*s.timeScale;
  };
  const start=async page=>{
    const ids=['very_easy','easy','normal','hard','very_hard'];
    await until(page,t=>t.scene==='MainMenu');
    await api.clickControl(page,'NameInput','input');
    await key('Tab');
    check((await state(page)).selectedControl==='NormalButton','Tab focuses the difficulty selector');
    await key('ArrowRight'); await key('ArrowLeft');
    let menu=await state(page);
    let steps=0;
    while(menu.menuDifficulty!==difficulty){
      check(steps++<5,'difficulty selector cycles through exactly five levels');
      const before=menu.menuDifficulty;
      await key('ArrowRight'); menu=await until(page,t=>t.menuDifficulty!==before);
    }
    check(ids.includes(menu.menuDifficulty) && menu.scene==='MainMenu','selector arrows keep the menu open');
    await page.setViewportSize({width:640,height:360}); await until(page,t=>t.width===640);
    await capture('difficulty-menu-'+difficulty+'-640');
    await page.setViewportSize({width:960,height:540}); await until(page,t=>t.width===960);
    const result=await baseStart(page);
    await until(page,t=>t.runDifficulty===difficulty);
    return result;
  };
  try {
    let s = await start(page);
    s = await until(page,t=>t.endless && t.encounterReading && t.enemyHp===180);
    check(s.runDifficulty===difficulty && Math.abs(s.comboWindowSeconds-1.5/s.difficultyPace)<.0001,
      'run snapshots difficulty and uses the asset combo window divided by pace');
    check(s.endless && s.encounterReading && s.enemyHp === 180 && s.hp === 40,
      'tutorial skip enters a fresh Endless intro');
    await capture('endless-intro');
    await page.setViewportSize({width:640,height:360});
    await until(page,t=>t.width===640 && t.height===360);
    await capture('endless-intro-640');
    await page.setViewportSize({width:960,height:540});
    s = await until(page,t=>t.width===960 && t.height===540);
    const frozen = s;
    await page.keyboard.type('type'); await key('Space'); await page.waitForTimeout(400);
    s = await state(page);
    check(s.inputLetters === frozen.inputLetters && s.hp === frozen.hp && s.wordTime === frozen.wordTime,
      'intro holds combat, timer and typing');
    await key('Escape'); await until(page,t=>t.paused);
    await api.clickControl(page,'Top 1000Button','button');
    await until(page,t=>t.leaderboardOpen && t.dataRows===1000);
    check((await state(page)).tableDifficulty===difficulty,'combat table defaults to the run difficulty');
    await api.clickControl(page,'>Button','button');
    await until(page,t=>t.tableDifficulty!==difficulty && t.dataRows===1000);
    check((await state(page)).runDifficulty===difficulty,'table filter does not alter the active run');
    await key('Tab');
    await key('Escape');
    s=await until(page,t=>!t.leaderboardOpen);
    check(s.paused && s.timeScale===0 && s.selectedControl==='Top 1000Button','modal preserves pause and restores focus');
    await key('Escape'); await until(page,t=>!t.paused);
    await until(page,t=>!t.leaderboardOpen && t.selectedControl!=='Top 1000Button'); // Wait for actual pause-canvas/focus cleanup.
    await page.waitForTimeout(300); // Resume's existing150ms camera tween still owns the input lock.
    check((await state(page)).timeScale === 0,'resume preserves encounter reading');
    await key('Enter'); await until(page,t=>!t.encounterReading && t.timeScale === 1);
    // Establish real incoming damage before a win to verify HP carry, not just full HP.
    s = await until(page,t=>t.hp===35);
    const firstHp = s.hp;
    defendEnabled=true;
    report.endlessDuels=[];
    report.parryAttempts=[];
    const letterMs=Number(process.env.TYPINGHERO_ENDLESS_LETTER_MS || 200);
    const wordInput=process.env.TYPINGHERO_ENDLESS_WORD_INPUT==='1';
    report.inputCadence=wordInput?`Native current-word keys at ${letterMs}ms/character; acknowledge every accepted character count before rereading the promoted word. Automated input, not human playtest.`:
      `One letter per >=${letterMs}ms including acknowledgement; cue polling continues between letters. Automated input, not human playtest.`;
    let sawSlayer=false, pausedSlayer=false, nextType=0, lastCue=false;
    let firstCueAt=null;
    let nextTrace=0;
    report.pilotTrace=[];
    report.runTiming={combo:[],recovery:[],frameMs:[]};
    let previousTiming=null,blockedIdleSince=null;
    let previousSlayer=false,awaitingDiagnosticHit=null;
    const inducedHitEncounters=new Set();
    const assetSamples=new Set();
    let lastForm="sword";
    const rounds = 1;
    const target = rounds * 6;
    const deadline = performance.now()+rounds*480000;
    while (performance.now()<deadline) {
      if(defenseFailure) throw defenseFailure;
      s=await state(page);
      if(s.hp>0&&!s.finished&&!s.paused&&!s.slayer&&s.timeScale>0&&s.playerAnimatorEnabled)
        check(s.playerAnimatorSpeed>0&&s.playerStateSpeed>0,'live outgoing Slayer/idle/hit animation keeps advancing');
      if(process.env.TYPINGHERO_IDLE_HIT_DIAGNOSTIC==='1'){
        if(awaitingDiagnosticHit && (s.finished||s.defeated!==awaitingDiagnosticHit.encounter)){
          awaitingDiagnosticHit=null;defendEnabled=true;
        }
        if(previousSlayer&&!s.slayer&&!s.finished&&/HEAVY/.test(s.opponent)&&!inducedHitEncounters.has(s.defeated)){
          inducedHitEncounters.add(s.defeated);awaitingDiagnosticHit={hp:s.hp,encounter:s.defeated};defendEnabled=false;
        }
        if(awaitingDiagnosticHit && s.hp<awaitingDiagnosticHit.hp){
          (report.deliberatePostSlayerHits ||= []).push({difficulty,defeated:s.defeated,before:awaitingDiagnosticHit.hp,after:s.hp,frame:s.frame});
          awaitingDiagnosticHit=null;defendEnabled=true;
        }
        previousSlayer=s.slayer;
      }
      if(Number.isFinite(s.unscaledClock)){
        report.runTiming.frameMs.push(s.frameMs);
        const old=previousTiming;
        if(old && !s.finished && !old.finished && !s.paused && !old.paused &&
            s.score===old.score && s.multiplier>1 && old.multiplier>1 && s.comboRemaining>0 &&
            s.unscaledClock>old.unscaledClock){
          const elapsed=s.unscaledClock-old.unscaledClock,decay=old.comboRemaining-s.comboRemaining;
          if(elapsed<.3 && decay>0){
            check(Math.abs(decay-elapsed)<.05,'combo decay follows actual unscaled time between awards');
            report.runTiming.combo.push({elapsed,decay,window:s.comboWindowSeconds});
          }
        }
        if(old && s.defeated===old.defeated && s.opponent===old.opponent && !s.finished && !old.finished &&
            !s.slayer && !old.slayer && !s.stunned && !old.stunned &&
            s.enemyNextAttackAt!==old.enemyNextAttackAt && !s.enemyAttacking && old.enemyAttacking && s.enemyChainRemaining===0){
          const remaining=s.enemyNextAttackAt-s.gameplayClock;
          const sampleInterval=s.gameplayClock-old.gameplayClock;
          check(remaining<=s.recoverySeconds+.04 && remaining+sampleInterval+.04>=s.recoverySeconds,
            'enemy recovery deadline agrees with the difficulty formula within the sampled frame interval');
          report.runTiming.recovery.push({remaining,configured:s.recoverySeconds,clock:s.gameplayClock});
        }
        const staleLock=s.hp>0 && !s.finished && !s.slayer && !s.parryPose &&
          (s.hitLocked || s.typingMuted) && /idle/i.test(s.playerSorting.sprite);
        if(staleLock){
          blockedIdleSince ??= {gameplay:s.gameplayClock,unscaled:s.unscaledClock};
          // The existing hit clip is450ms plus its crossfade/render sampling.
          // Measure gameplay time so legitimate hitstop cannot look like a stuck lock.
          check(s.gameplayClock-blockedIdleSince.gameplay<1,'living idle player must release hit/typing locks after the authored hit recovery');
        }else if(blockedIdleSince){
          (report.idleLockRecoveries ||= []).push({gameplay:s.gameplayClock-blockedIdleSince.gameplay,unscaled:s.unscaledClock-blockedIdleSince.unscaled});
          blockedIdleSince=null;
        }
        previousTiming=s;
      }
      if(process.env.TYPINGHERO_STATE_TRACE==='1' && performance.now()>=nextTrace){
        const keys=['frame','qaAgeMs','combatSeconds','current','next','hp','enemyHp','score','inputLetters','inputCorrect','matchParries',
          'slayer','stunned','canParry','parryPose','parryWindow','pendingAttack','cue','enemyAttacking','enemyAnimationTime','timeScale',
          'gameplayClock','unscaledClock','comboWindowSeconds','comboRemaining','enemyNextAttackAt','enemyChainRemaining','hitLocked','typingMuted','frameMs',
          'playerCurrentState','playerNextState','playerAnimatorTime','playerAnimatorSpeed','playerStateSpeed','playerHitStateActive','playerAnimatorTransition','playerAnimatorEnabled'];
        report.pilotTrace.push({...Object.fromEntries(keys.map(k=>[k,s[k]])),player:s.playerSorting});
        nextTrace=performance.now()+100;
      }
      if(s.slayer) sawSlayer=true;
      if(s.enemyHp>0) {
        check(['Idle','Walk','Attack1','Attack2','Hurt','Die'].includes(s.enemyTexture),'only approved form textures appear');
        const form=/HEAVY|BERSERKER/.test(s.opponent)?'axe':/GUARD|CAPTAIN/.test(s.opponent)?'shield':'sword';
        lastForm=form;
        const sample=form+'-'+s.enemyTexture;
        if(process.env.TYPINGHERO_SKIP_ASSET_CAPTURES!=='1' && !assetSamples.has(sample)) {assetSamples.add(sample); await capture('asset-'+sample);}
      }
      if(s.hp<=0) {
        await until(page,t=>t.overlay,5000); await capture('endless-unexpected-defeat');
        throw new Error(`Keyboard pilot died at ${s.opponent}: defeated=${s.defeated}, enemyHP=${s.enemyHp}, parries=${s.matchParries}`);
      }
      if(s.slayer && !pausedSlayer){
        await key('Escape'); const held=await until(page,t=>t.paused);
        await page.waitForTimeout(300);
        check((await state(page)).enemyHp===held.enemyHp,'Slayer pause holds damage');
        await key('Escape'); await until(page,t=>!t.paused); pausedSlayer=true;
        await page.waitForTimeout(300); await capture('endless-slayer');
        await page.setViewportSize({width:640,height:360});
        await until(page,t=>t.width===640 && t.height===360);
        await capture('endless-slayer-640');
        await page.setViewportSize({width:960,height:540});
        await until(page,t=>t.width===960 && t.height===540);
        continue; // Layout/pause waits invalidate the earlier word snapshot.
      }
      if(s.finished){
        if(s.defeated===6 && s.enemyHp!==-1) {
          check(!s.overlay,'boss result waits for death animation cleanup');
          for(let i=0;i<3;i++) await key('Enter');
          check((await state(page)).defeated===6,'Enter during boss death cannot start another duel');
        }
        const deathSample=lastForm+'-Die';
        if(!assetSamples.has(deathSample)) {
          const dying=await until(page,t=>t.enemyTexture==='Die' || t.enemyHp===-1);
          if(dying.enemyHp!==-1) {assetSamples.add(deathSample);await capture('asset-'+deathSample);}
        }
        await until(page,t=>t.enemyHp===-1);
        const end=await state(page);
        check(end.defeated===report.endlessDuels.length+1,'enemy death advances progress once');
        if(end.defeated<6) check(!end.overlay,'intermediate victory retains Continue');
        check(!end.slayer,'enemy death exits Slayer');
        report.endlessDuels.push({defeated:end.defeated,hp:end.hp,score:end.score,seconds:end.combatSeconds,parries:end.matchParries});
        console.log(JSON.stringify({difficulty,...report.endlessDuels.at(-1)}));
        await capture('endless-cleared-'+end.defeated);
        if(end.defeated===6) {
          s=await until(page,t=>t.overlay);
          for(let i=0;i<5;i++) await key('Enter');
          const held=await state(page);
          check(held.finished && held.overlay && held.defeated===6 && held.hp===end.hp && held.score===end.score,'boss result rejects repeated Enter without healing or duel seven');
          break;
        }
        if(end.defeated===1) await api.clickControl(page,'PracticeButton','button');
        else await key('Enter');
        s=await until(page,t=>!t.finished && !t.encounterReading && t.defeated===end.defeated);
        const expectedHp=end.defeated%6===0?Math.min(40,end.hp+10):end.hp;
        check(s.hp===expectedHp && s.score===end.score && s.inputLetters===end.inputLetters && s.matchParries===end.matchParries,
          'next opponent retains run data');
        check(!s.slayer && !s.parryWindow,'next encounter clears Slayer and parry window');
        check(s.runDifficulty===difficulty,'Continue preserves the difficulty snapshot');
        const baseRecovery=Math.max(.7,(/HEAVY|BERSERKER/.test(s.opponent)?2.5:1.7)-(s.endlessSlot===4?.2:0)-Math.min(s.endlessRound-1,8)*.12);
        check(Math.abs(s.recoverySeconds-baseRecovery/s.difficultyPace)<.001,'actual configured opponent recovery matches role, elite, round and difficulty');
        await capture('endless-opponent-'+s.endlessSlot);
        if(end.defeated===1) check(s.hp<=firstHp,'damage persists after first win');
        if(end.defeated===6 || end.defeated===12) {
          const width=end.defeated===6?640:960,height=end.defeated===6?360:540;
          await page.setViewportSize({width,height});await until(page,t=>t.width===width && t.height===height);
        }
        if(end.defeated===target){check(s.endlessRound===rounds+1 && s.endlessSlot===0,'boss loops into the next round');break;}
        lastCue=false; nextType=0; continue;
      }
      if(!s.cue) {lastCue=false;firstCueAt=null;}
      else if(firstCueAt===null) firstCueAt=performance.now()-s.qaAgeMs;
      const contactIn=s.cue?impactIn(s,firstCueAt):Infinity;
      const fresh=s; // No awaited action since this iteration's acknowledged observation.
      if(performance.now()>=nextType && fresh.current && (fresh.defeated>0 || fresh.matchParries>0) &&
          (fresh.slayer || !fresh.enemyAttacking && !fresh.cue)){
        const input=wordInput?fresh.current:fresh.current[0];
        if(wordInput) await typeWordKeys(input,letterMs); else await page.keyboard.type(input,{delay:20});
        const accepted=await acknowledged(t=>t.inputLetters>=fresh.inputLetters+input.length || t.finished);
        if(wordInput && !accepted.finished) check(accepted.inputCorrect>=fresh.inputCorrect+input.length,'every native word character is accepted before promotion is reread');
        nextType=performance.now()+Math.max(0,letterMs-20);
      }
      await page.waitForTimeout(25);
    }
    report.assetSamples=[...assetSamples];
    check(report.endlessDuels.length===target && sawSlayer && pausedSlayer,'keyboard typing/parry/Slayer clears the required rounds');
    if(Number.isFinite(s.unscaledClock)) check(report.runTiming.combo.length>0 && report.runTiming.recovery.length>0,
      'each difficulty has actual clock measurements for combo decay and enemy recovery');
    defendEnabled=false;
    s=await until(page,t=>t.overlay,5000);
    check(s.result.includes('BOSS DEFEATED') && !s.result.includes('ROUND') && s.result.includes('DEFEATED 6') && s.result.includes('ACCURACY') && s.result.includes('PARRIES') && s.result.includes('NEW BEST 6'),
      'boss victory summarizes six defeated and final statistics');
    await page.waitForTimeout(300);
    await capture('victory-960');
    await until(page,()=>report.endlessWrites>baseWrites);
    check(report.endlessPayloads.at(-1).defeated===6 && report.endlessPayloads.at(-1).score===s.score,'boss sends six defeated and the final score through the existing payload');
    await api.clickControl(page,'Top 1000Button','button');
    await until(page,t=>t.leaderboardOpen && t.dataRows===1000);
    check((await state(page)).tableDifficulty===difficulty,'victory table uses run difficulty');
    await capture('victory-table');
    await page.setViewportSize({width:640,height:360});
    await until(page,t=>t.width===640 && t.height===360);
    await capture('victory-table-640');
    await key('Escape'); await until(page,t=>!t.leaderboardOpen);
    await capture('victory-640');
    await page.setViewportSize({width:640,height:360});
    await until(page,t=>t.width===640 && t.height===360);
    await capture('endless-defeat-640');
    await page.setViewportSize({width:960,height:540});
    await until(page,t=>t.width===960 && t.height===540);
    const final=s; await page.keyboard.type('type'); await page.waitForTimeout(250); s=await state(page);
    check(s.inputLetters===final.inputLetters && s.score===final.score && s.combatSeconds===final.combatSeconds,
      'death freezes statistics');
    await key('r');
    s=await until(page,t=>t.encounterReading && !t.paused && t.defeated===0 && t.hp===40 && t.score===0);
    check(s.inputLetters===0 && s.matchParries===0,'restart clears all run statistics');
    check(s.runDifficulty===difficulty,'Restart preserves the difficulty snapshot');
    await key('Enter'); await until(page,t=>!t.encounterReading);
    await until(page,t=>t.finished && t.hp<=0,60000);
    s=await until(page,t=>t.overlay,5000);
    check(s.result.includes('ACCURACY —') && s.result.includes('BEST '+target+' DEFEATED'),
      'fresh no-letter defeat preserves the earlier Endless best across restart');
    await capture('endless-best-retained');
    await key('r'); await until(page,t=>t.encounterReading && !t.paused && t.hp===40);
    await key('Escape'); await until(page,t=>t.paused); await key('m');
    await until(page,t=>t.scene==='MainMenu');
    check(report.fixtureWrites===0,'Endless never writes the single-duel Firebase table');
    await until(page,()=>report.endlessWrites>baseWrites);
    await api.clickControl(page,'Top 1000Button','button');
    s=await until(page,t=>t.leaderboardOpen && t.dataRows===1000 && t.collectionMode===0);
    check(s.tableDifficulty===difficulty && s.collectionFirstRow.startsWith(difficulty+' '),'table defaults to menu difficulty and binds its own partition');
    check(s.collectionTitle.includes('ENDLESS'),'Endless is the default community table');
    await capture('endless-table');
    await page.setViewportSize({width:640,height:360});
    await until(page,t=>t.width===640 && t.height===360);
    await capture('endless-table-640');
    const delayedEndpoint=difficulty==='normal'?'**/leaderboards/endless_v1.json*':'**/leaderboards/endless_v2/'+difficulty+'.json*';
    let oldRequest=false;
    await context.route(delayedEndpoint,async route=>{
      oldRequest=true;
      await page.waitForTimeout(700);
      await route.fulfill({status:503,headers:{'access-control-allow-origin':'*'},body:'{}'});
    });
    await api.clickControl(page,'RefreshButton','button');
    await until(page,()=>oldRequest);
    await api.clickControl(page,'>Button','button');
    const changed=await until(page,t=>t.tableDifficulty!==difficulty && t.dataRows===1000);
    await page.waitForTimeout(900);
    s=await state(page);
    check(s.tableDifficulty===changed.tableDifficulty && s.collectionFirstRow.startsWith(changed.tableDifficulty+' ') && !s.collectionStatus.includes('Could not load'),
      'late error from a previous filter cannot replace current rows or status');
    check(s.menuDifficulty===difficulty,'table filtering does not change the menu selection');
    await context.unroute(delayedEndpoint);
    await api.clickControl(page,'<Button','button');
    await until(page,t=>t.tableDifficulty===difficulty && t.dataRows===1000);
    await api.clickControl(page,'ClassicButton','button');
    s=await until(page,t=>t.collectionMode===1 && t.dataRows===1000);
    check(s.collectionTitle==='CLASSIC','Classic remains independently accessible');
    await capture('endless-classic-640');
    await api.clickControl(page,'OpponentsButton','button');
    s=await until(page,t=>t.collectionMode===2);
    check(s.collectionDetails.includes('SWORDSMAN') && s.collectionDetails.includes('Elite:'),'encounter profiles show unlocked roles and elite records');
    await capture('endless-profiles-640');
    await api.clickControl(page,'AchievementsButton','button');
    s=await until(page,t=>t.collectionMode===3);
    check(s.collectionDetails.includes('FIRST CAPTAIN') && s.collectionDetails.includes('UNLOCKED'),'first boss achievement persists through restart and menu');
    await capture('endless-achievements-640');
    const endpoint=difficulty==='normal'?'**/leaderboards/endless_v1.json*':'**/leaderboards/endless_v2/'+difficulty+'.json*';
    await context.route(endpoint,route=>route.fulfill({status:503,headers:{'access-control-allow-origin':'*'},body:'{}'}));
    await api.clickControl(page,'EndlessButton','button');
    await until(page,t=>t.collectionMode===0 && t.collectionStatus.includes('Could not load'));
    await capture('endless-network-error-640');
    await context.unroute(endpoint);
    await context.route(endpoint,route=>route.fulfill({status:200,headers:{'access-control-allow-origin':'*'},contentType:'application/json',body:'null'}));
    await api.clickControl(page,'RefreshButton','button');
    await until(page,t=>t.dataRows===0 && t.collectionStatus.includes('No Endless'));
    await capture('endless-empty-640');
    await context.unroute(endpoint);
    await api.clickControl(page,'RefreshButton','button');
    await until(page,t=>t.dataRows===1000);
    check(true,'network error and empty state can retry to the populated table');
    await key('Escape');
    s=await until(page,t=>!t.leaderboardOpen);
    check(s.scene==='MainMenu' && s.selectedControl==='Top 1000Button','modal Escape restores the menu and focus');
    await page.setViewportSize({width:960,height:540});await until(page,t=>t.width===960 && t.height===540);
    const beforeAbandon={reads:report.endlessReads||0,writes:report.endlessWrites||0};
    await start(page); await until(page,t=>t.encounterReading);
    await key('Enter');
    await until(page,t=>t.hp===35);
    defendEnabled=true;
    const fastDeadline=performance.now()+45000;
    let fastNext=0, fastCue=false,fastCueAt=null;
    while(performance.now()<fastDeadline) {
      s=await state(page);
      if(s.finished) break;
      if(!s.cue) {fastCue=false;fastCueAt=null;}
      else if(fastCueAt===null) fastCueAt=performance.now()-s.qaAgeMs;
      const fastContact=s.cue?impactIn(s,fastCueAt):Infinity;
      // Parry waits and promotion can invalidate the earlier word snapshot.
      // Acknowledge real input before choosing another character at this cadence.
      const fresh=s;
      if(fresh.current && performance.now()>=fastNext && (fresh.slayer || !fresh.enemyAttacking && !fresh.cue)) {
        const input=wordInput?fresh.current:fresh.current[0];
        if(wordInput) await typeWordKeys(input,20); else await page.keyboard.type(input,{delay:20});
        await acknowledged(t=>t.inputLetters>=fresh.inputLetters+input.length || t.finished);
        fastNext=performance.now()+30;
      }
      await page.waitForTimeout(15);
    }
    check(s.finished && s.hp>0,'fast typing completes the same first opponent');
    check(s.inputLetters===s.inputCorrect,'fast typing comparison uses correct acknowledged input');
    report.fastDuelSeconds=s.combatSeconds;
    if(!wordInput) check(s.combatSeconds<report.endlessDuels[0].seconds,'faster real typing ends the same opponent sooner');
    else report.fastTimingNote='Word-key integration sample uses a new random word sequence; elapsed time is observational, not a controlled speed/balance comparison.';
    await until(page,t=>t.enemyHp===-1);
    await key('Escape');await until(page,t=>t.paused);await key('m');
    await until(page,t=>t.scene==='MainMenu');
    check((report.endlessReads||0)===beforeAbandon.reads && (report.endlessWrites||0)===beforeAbandon.writes,
      'winning then abandoning a live run never submits a result');

  } finally { stopDefense=true; await defense; await context.close(); }
};
