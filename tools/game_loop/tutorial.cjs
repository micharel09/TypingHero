// Real input only. QA fields observe the authored frame sequence and tutorial lifecycle.
module.exports = async function tutorialFlow(api, page, options = {}) {
  const {state,until,clickControl,check,report}=api;
  const startName='Start Battle \u00b7 EnterButton';
  const key=async k=>{await page.keyboard.press(k,{delay:80});await page.waitForTimeout(80);};
  const step1=s=>s.tutorial&&!s.tutorialParry&&!s.paused&&!s.dungeonTransitioning;
  const step2=s=>s.tutorial&&s.tutorialParry&&!s.paused&&!s.dungeonTransitioning;
  const screenshot=name=>page.screenshot({path:report.outputDirectory+'/'+name+'.png'});
  const menu=async()=>{
    await key('Escape');await until(page,s=>s.paused);await key('m');
    return until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);
  };
  const launch=async replay=>{
    await until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);
    await page.locator('#unity-canvas').click({position:{x:10,y:10}});
    await page.waitForTimeout(350);
    await clickControl(page,replay?'TutorialButton':'PlayButton','button');
    let s=await until(page,s=>s.scene==='SampleScene'||s.controls.some(c=>c.name==='NameInput'));
    if(s.scene!=='SampleScene'){
      await clickControl(page,'NameInput','input');await page.keyboard.press('Control+A');
      await page.keyboard.type('LoopQA',{delay:30});await key('Enter');
    }
    return until(page,s=>s.scene==='SampleScene'&&!s.dungeonTransitioning&&s.current.length>0);
  };
  const writes=report.fixtureWrites;
  if(!options.freshTutorial)await launch(false);
  let s=await until(page,step1);const initialStatus=s.tutorialStatus;
  const frozen=s;
  await page.keyboard.type('BLADEwrong',{delay:30});await key('Space');s=await state(page);
  check(s.inputLetters===0&&s.combatSeconds===0&&s.score===0&&s.hp===frozen.hp&&s.enemyHp===frozen.enemyHp&&s.audioPaused,
    'tutorial blocks input, time, score and combat');
  for(const [width,height] of options.viewports||[[1280,720],[640,360]]){
    await page.setViewportSize({width,height});await until(page,s=>s.width===width&&s.height===height);
    await screenshot('tutorial-hud-'+width);
    s=await state(page);
    check(s.tutorialArrows.length===5,'five HUD arrows at '+width);
    check(!s.typography.some(t=>t.path.includes('SpaceExample'))&&s.typography.some(t=>t.text==='1/2'),'HUD step has no old illustration at '+width);
    await clickControl(page,'PracticeButton','button');s=await until(page,step2);
    check(s.tutorialStatus===initialStatus&&s.tutorialArrows.length===0,'Next hides cards/arrows without marking viewed');
    const bounds=s.tutorialModal,clip=s.tutorialClipRect;
    check(Math.abs(bounds.width-width*816/960)<2&&Math.abs(bounds.height-height*480/540)<2&&
      clip.width>=width*.7&&clip.y>=bounds.y&&clip.y+clip.height<=bounds.y+bounds.height,'large centered modal and clip at '+width);
    const text=s.typography.filter(t=>t.path.includes('/ParryModal/'));
    check(text.length===5&&text.every(t=>!t.truncated&&t.glyphBounds.x>=t.bounds.x-1&&t.glyphBounds.y>=t.bounds.y-1&&
      t.glyphBounds.x+t.glyphBounds.width<=t.bounds.x+t.bounds.width+1&&t.glyphBounds.y+t.glyphBounds.height<=t.bounds.y+t.bounds.height+1),
      'modal title, captions and both actions fit at '+width);
    await screenshot('tutorial-parry-'+width);
    await clickControl(page,'BackButton','button');await until(page,step1);
  }
  await page.setViewportSize({width:1280,height:720});await until(page,s=>s.width===1280&&s.height===720);
  // Holding Enter must neither skip step 2 nor persist completion.
  await page.keyboard.down('Enter');
  try{await until(page,step2);await page.waitForTimeout(700);s=await state(page);
    check(step2(s)&&s.tutorialStatus===initialStatus,'held Enter stays on unread step 2');}
  finally{await page.keyboard.up('Enter');}
  await page.keyboard.type('WRONG',{delay:30});await key('Space');s=await state(page);
  check(s.inputLetters===0&&s.combatSeconds===0&&s.score===0&&s.timeScale===0,'parry modal retains input and score lock');
  await page.evaluate(()=>{
    const descriptor=Object.getOwnPropertyDescriptor(window,'typingHeroQA');window.modalTrace=[];
    Object.defineProperty(window,'typingHeroQA',{configurable:true,get:descriptor.get,set(value){
      descriptor.set.call(window,value);
      if(value.tutorialParry&&!value.paused)window.modalTrace.push({clock:value.tutorialClipTime,frame:value.tutorialClipFrame,
        caption:value.typography?.find(t=>t.path.endsWith('/ParryModal/Panel/Caption'))?.text});
    }});
  });
  await page.waitForTimeout(7800);
  const trace=await page.evaluate(()=>window.modalTrace);report.modalTrace=trace;
  check(['Wait for the attack.','Tap Space now.','Parry!','Release between hits.'].every(c=>trace.some(f=>f.caption===c)),
    'all four captions play automatically over repeated complete clip');
  await key('Escape');s=await until(page,s=>s.paused);const held=s.tutorialClipTime,frame=s.tutorialClipFrame;
  await page.waitForTimeout(400);s=await state(page);
  check(s.tutorialClipTime===held&&s.tutorialClipFrame===frame,'Pause holds exact frame and clock');
  await screenshot('tutorial-modal-pause');
  await clickControl(page,'ResumeButton','button');await until(page,s=>step2(s)&&s.tutorialClipTime>held);
  check((await state(page)).timeScale===0,'Resume retains reading lock');
  await clickControl(page,'BackButton','button');await until(page,step1);await key('Enter');
  s=await until(page,step2);check(s.tutorialClipTime<.5,'reenter resets clip');
  await key('Escape');await until(page,s=>s.paused);await key('r');s=await until(page,step1);
  check(s.tutorialStatus===initialStatus,'Restart returns to unread step 1');
  await menu();check((await state(page)).tutorialStatus===initialStatus,'Menu abandons without completion');
  await launch(true);await until(page,step1);await key('Enter');await until(page,step2);
  await key('Enter');s=await until(page,s=>!s.tutorial&&!s.dungeonTransitioning);
  check(s.tutorialStatus===2&&s.score===0&&s.inputLetters===0&&s.hp===40&&s.enemyHp===180&&s.stamina===100&&s.runDifficulty===s.menuDifficulty,'second Enter starts a fresh selected-difficulty battle and marks viewed');
  if(s.encounterReading){await key('Enter');await until(page,s=>!s.encounterReading);}
  s=await state(page);
  check(!s.typography.some(t=>/SpaceExample|ParryModal|HealthGuide|EnemyGuide/.test(t.path)),'battle has no cloned tutorial content');
  let successful=false;
  for(let attempt=0;attempt<4&&!successful;attempt++){
    await page.waitForFunction(()=>{const f=window.typingHeroCombatFrame;return f?.spaceCueVisible&&f.spaceCueFilled&&f.canParry&&f.cueImpactIn>.12;},null,{polling:'raf',timeout:12000});
    const before=await page.evaluate(()=>window.typingHeroCombatFrame.parries);
    await page.keyboard.down('Space');try{await page.waitForFunction(()=>window.typingHeroCombatFrame?.parryWindow,null,{polling:'raf',timeout:600});}
    finally{await page.keyboard.up('Space');}
    try{await page.waitForFunction(n=>window.typingHeroCombatFrame.parries>n,before,{polling:'raf',timeout:1800});successful=true;}catch{}
  }
  check(successful,'real Space parry after Start Battle');await screenshot('tutorial-live-parry');
  await menu();await page.reload();await page.waitForFunction(()=>window.typingHeroQA?.frame>0,null,{timeout:120000});
  check((await state(page)).tutorialStatus===2,'viewed persists across reload');
  s=await launch(false);check(!s.tutorial,'viewed Play bypasses tutorial');
  await menu();await launch(true);await until(page,step1);
  await clickControl(page,'PracticeButton','button');await until(page,step2);
  await clickControl(page,startName,'button');s=await until(page,s=>!s.tutorial&&!s.dungeonTransitioning);
  check(s.tutorialStatus===2,'mouse Start uses the same completion flow');
  check(report.fixtureWrites===writes,'tutorial lifecycle writes no scores');
  await menu();
};
