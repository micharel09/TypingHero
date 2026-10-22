// Read-only observations; every transition uses real keyboard/mouse input.
module.exports = async function tutorialFlow(api, page, options = {}) {
  const {state,until,clickControl,check,report}=api;
  const key=key=>page.keyboard.press(key,{delay:120});
  const reading=()=>until(page,s=>s.tutorial && s.timeScale===0 && !s.paused && s.tutorialArrows.length===5);
  const menu=async()=>{
    await key('Escape'); await until(page,s=>s.paused);
    await key('m'); return until(page,s=>s.scene==='MainMenu' && !s.dungeonTransitioning);
  };
  const launch=async replay=>{
    await clickControl(page,replay?'TutorialButton':'PlayButton','button');
    await until(page,s=>s.controls.some(c=>c.name==='NameInput'));
    await clickControl(page,'NameInput','input');
    await page.keyboard.press('Control+A'); await page.keyboard.type('LoopQA',{delay:60});
    await key('Enter');
    return until(page,s=>s.scene==='SampleScene' && !s.dungeonTransitioning && s.current.length>0);
  };
  const freshBattle=s=>check(!s.tutorial && s.endless && s.encounterReading && s.hp===40 &&
    s.enemyHp===180 && s.stamina===100 && s.score===0 && s.inputLetters===0 && s.matchParries===0 &&
    s.combatSeconds===0 && !s.slayer && !s.finished && s.runDifficulty===s.menuDifficulty,
    'Start Battle enters a fresh encounter at the selected difficulty');
  const writes=report.fixtureWrites;
  if (!options.freshTutorial) await launch(true);
  let s=await reading();
  const frozen=s;
  await page.keyboard.type('BLADEwrong',{delay:70}); await key('Space'); await page.waitForTimeout(500);
  s=await state(page);
  check(s.hp===frozen.hp && s.enemyHp===frozen.enemyHp && s.stamina===frozen.stamina &&
    s.current===frozen.current && s.wordTime===frozen.wordTime && s.inputLetters===0 &&
    s.combatSeconds===0 && s.score===0 && s.audioPaused,'reading holds input, combat, score, timer and audio');
  for (const [width,height] of (options.viewports||[[1280,720],[640,360]])) {
    await page.setViewportSize({width,height});
    const resized=await until(page,s=>s.width===width && s.height===height);
    s=await until(page,s=>s.frame>=resized.frame+3);
    await page.screenshot({path:report.outputDirectory+'/tutorial-guide-'+width+'x'+height+'.png'});
    const texts=s.typography.filter(t=>/\/(HealthGuide|EnemyGuide|TypingGuide|ScoreGuide|Shortcuts)$/.test(t.path));
    check(texts.length===5 && texts.every(t=>!t.truncated && t.glyphBounds.x>=t.bounds.x-1 &&
      t.glyphBounds.y>=t.bounds.y-1 && t.glyphBounds.x+t.glyphBounds.width<=t.bounds.x+t.bounds.width+1 &&
      t.glyphBounds.y+t.glyphBounds.height<=t.bounds.y+t.bounds.height+1),'all guide copy fits '+width+'x'+height);
    const overlaps=(a,b)=>a.x<b.x+b.width && a.x+a.width>b.x && a.y<b.y+b.height && a.y+a.height>b.y;
    check(![s.playerStatusRect,s.enemyStatusRect,s.trayRect].some(r=>overlaps(r,s.tutorialPanel)),
      'panel clears real HP/guard/typing HUD '+width+'x'+height);
    check(s.tutorialArrows.map(a=>a.target).join(',')==='PlayerHP,EnemyHP,EnemyStamina,ScoreCluster,TypingPanel' &&
      s.tutorialArrows.every(a=>a.end.x>=a.targetRect.x-.5 && a.end.x<=a.targetRect.x+a.targetRect.width+.5 &&
        a.end.y>=a.targetRect.y-.5 && a.end.y<=a.targetRect.y+a.targetRect.height+.5),
      'five arrow tips touch their real HUD targets '+width+'x'+height);
  }
  await key('Escape'); await until(page,s=>s.paused);
  await page.screenshot({path:report.outputDirectory+'/tutorial-pause.png'});
  check(!(await state(page)).controls.some(c=>c.name==='PracticeButton'),'Pause hides guide interaction');
  await clickControl(page,'ResumeButton','button'); await reading();
  check((await state(page)).tutorialStatus===frozen.tutorialStatus,'Resume does not mark the guide viewed');
  await key('Escape'); await until(page,s=>s.paused); await key('r'); s=await reading();
  check(s.hp===40 && s.enemyHp===850 && s.inputLetters===0 && s.tutorialStatus===frozen.tutorialStatus,
    'Restart reopens the same unread guide with fresh combat');
  await menu();
  check((await state(page)).tutorialStatus===frozen.tutorialStatus,'Menu does not mark unread guide viewed');
  await launch(frozen.tutorialStatus!==0); await reading();
  // Hold Enter across scene teardown; it must not confirm the encounter intro.
  await page.keyboard.down('Enter');
  try {await until(page,s=>s.dungeonTransitioning); await page.waitForTimeout(500);}
  finally {await page.keyboard.up('Enter');}
  s=await until(page,s=>!s.tutorial && s.encounterReading && !s.dungeonTransitioning);
  freshBattle(s);
  check(!s.typography.some(t=>/\/(HealthGuide|EnemyGuide|TypingGuide|ScoreGuide|Shortcuts)$/.test(t.path)),
    'encounter intro contains no cloned guide copy');
  check(s.tutorialStatus===2,'only Start Battle persists completion');
  await page.screenshot({path:report.outputDirectory+'/tutorial-fresh-battle.png'});
  await menu(); await page.reload();
  await page.waitForFunction(()=>window.typingHeroQA?.frame>0,null,{timeout:120000});
  check((await state(page)).tutorialStatus===2,'viewed preference survives reload');
  s=await launch(false); freshBattle(s);
  await menu(); await launch(true); await reading();
  // Mouse confirmation uses the same handler as Enter.
  await clickControl(page,'PracticeButton','button');
  s=await until(page,s=>!s.tutorial && s.encounterReading && !s.dungeonTransitioning);
  freshBattle(s);
  await key('Enter');
  s=await until(page,s=>!s.encounterReading && s.timeScale===1 && !s.dungeonTransitioning);
  check(!s.typography.some(t=>/EndlessHUD.*(Guide|Shortcuts)$/.test(t.path)),
    'live combat contains no cloned guide copy');
  const word=s.current, hp=s.enemyHp;
  await page.keyboard.type(word,{delay:40});
  s=await until(page,s=>s.inputCorrect>=word.length && s.enemyHp<hp && s.score>0);
  check(!s.tutorial && s.runDifficulty===s.menuDifficulty,'fresh battle accepts a real typed attack at the selected difficulty');
  await page.screenshot({path:report.outputDirectory+'/tutorial-live-battle.png'});
  check(report.fixtureWrites===writes,'guide, restart, replay and confirmations produce zero score writes');
  await menu();
};
