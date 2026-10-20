// Read-only snapshots; all gameplay actions use the existing native keyboard driver.
module.exports = async function combatHUD(api,browser) {
  const {session,start,state,until,clickControl,check,report}=api;
  const sizes=[[320,180],[640,360],[1280,720],[400,300]];
  const near=(a,b)=>Math.abs(a-b)<.015;
  const red=c=>near(c.r,241/255)&&near(c.g,107/255)&&near(c.b,103/255);
  const gold=c=>near(c.r,220/255)&&near(c.g,193/255)&&near(c.b,138/255);
  const teal=c=>near(c.r,88/255)&&near(c.g,139/255)&&near(c.b,140/255);
  const dim=c=>near(c.r,.4)&&near(c.g,.4)&&near(c.b,.4);
  const labels={very_easy:'VERY EASY',easy:'EASY',normal:'NORMAL',hard:'HARD',very_hard:'VERY HARD'};
  report.hudSeen={};
  const seen=report.hudSeen;
  let lastFrame=-1, wasFinished=false;
  const observe=async page=>{
    const s=await state(page);
    if(s.scene!=='SampleScene'||s.frame===lastFrame) return s;
    lastFrame=s.frame;
    check(s.encounterBackgroundVisible===s.encounterVisible,'difficulty backplate visibility matches its label');
    if(wasFinished&&!s.finished){
      check(s.hudHitRemaining===0&&s.hudSuccessRemaining===0&&!s.guardBreakVisible&&s.staminaIconName==='icon-shield','Continue resets feedback without treating carried HP as a hit');
      seen.continue=true;
    }
    wasFinished=s.finished;
    if(s.hp>0&&s.hp<=10){check(red(s.hpNumberColor),'low HP number is red');seen.low=true;}
    if(s.hp>10){check(!red(s.hpNumberColor),'healthy HP number has its original color');}
    if(s.paused||s.encounterReading||s.finished||s.timeScale===0){
      check(!s.encounterVisible,'modal/results hides difficulty row');
      check(dim(s.parryKeyColor),'blocked combat keycap is dim');
    } else if(s.endless){
      check(s.encounterVisible&&s.encounterText===labels[s.runDifficulty]+' \u00b7 DUEL '+(s.endlessSlot+1)+'/6','HUD difficulty and duel match the run');
      check(!s.combatHeadingVisible,'combat has no duplicate duel heading');
      if(s.hudSuccessRemaining>0){check(teal(s.parryKeyColor),'success event paints teal');seen.success=true;}
      else if(s.parryWindow){check(gold(s.parryKeyColor),'actual parry window paints gold');seen.window=true;}
      else if(s.cue&&s.canParry){check(gold(s.parryKeyColor),'ready enemy cue paints gold');seen.cue=true;}
      else if(!s.canParry){check(dim(s.parryKeyColor),'hit/pose lock dims keycap');seen.lock=true;}
      else {check(near(s.parryKeyColor.r,1),'ready keycap uses original brightness');seen.ready=true;}
      if(s.slayer) seen.slayer=true;
    }
    check(!s.guardBreakVisible,'Slayer replaces the guard-break announcement');
    if(s.staminaIconName==='icon-warning'){check(s.stunned,'warning icon follows actual stun');seen.warning=true;}
    if(s.staminaIconName==='icon-shield'&&!s.stunned) seen.shield=true;
    if(s.finished){check(!s.guardBreakVisible&&s.hudHitRemaining===0&&s.hudSuccessRemaining===0,'results clears transient feedback');seen.results=true;}
    return s;
  };
  const captureSizes=async(page,label)=>{
    let s=await state(page),paused=s.paused;
    if(!paused){await page.keyboard.press('Escape',{delay:10});await until(page,t=>t.paused);}
    for(const [width,height] of sizes){
      await page.setViewportSize({width,height});await until(page,t=>t.width===width&&t.height===height);
      await page.keyboard.press('Escape',{delay:10});
      s=await until(page,t=>!t.paused&&t.timeScale===1&&!t.controls.some(c=>c.name==='ResumeButton'));
      check(s.encounterVisible&&s.encounterBackgroundVisible,'live four-size capture shows difficulty and its backplate');
      await page.screenshot({path:report.outputDirectory+'/'+label+'-'+width+'x'+height+'.png'});
      await page.keyboard.press('Escape',{delay:10});await until(page,t=>t.paused);
    }
    await page.setViewportSize({width:960,height:540});await until(page,t=>t.width===960);
    if(!paused){await page.keyboard.press('Escape',{delay:10});await until(page,t=>!t.paused&&t.timeScale===1);}
  };
  // Five menu selections prove content without replaying the balance matrix.
  for(const level of Object.keys(labels)){
    const {context,page}=await session(browser,{width:640,height:360},1000);
    try{
      await until(page,s=>s.scene==='MainMenu');
      let s=await state(page),steps=0;
      while(s.menuDifficulty!==level){check(steps++<6,'difficulty selection progresses');await clickControl(page,'>Button','button');s=await state(page);}
      await start(page,true);s=await until(page,t=>t.encounterReading);
      check(!s.encounterVisible&&!s.encounterBackgroundVisible&&s.combatHeadingVisible,'intro owns the complete prompt and hides the HUD backplate');
      check(s.hudHitRemaining===0&&!s.guardBreakVisible,'new encounter has no false feedback');
      await page.keyboard.press('Enter',{delay:20});
      s=await until(page,t=>!t.encounterReading&&t.timeScale===1);
      check(s.encounterText===labels[level]+' \u00b7 DUEL 1/6','selected difficulty text '+level);
      await observe(page);
      console.log('HUD difficulty '+level+' PASS');
    }finally{await context.close();}
  }
  let capturedSlayer=false,pausedSuccess=false,pausedBreak=false;
  await require('./endless.cjs')({...api,state:observe,difficulty:'normal',stopAtVictory:true,
    beforeDefense:async page=>{
      // First incoming hit was produced by the existing Endless driver.
      await page.waitForFunction(()=>window.typingHeroQA.hudHitRemaining-(performance.now()-window.typingHeroQAPublishedAt)/1000>.07,null,{polling:'raf'});
      await page.keyboard.press('Escape',{delay:10});
      let s=await until(page,t=>t.paused);
      check(s.hudHitRemaining>0,'pause catches the real damage flash');
      const remaining=s.hudHitRemaining;
      await page.waitForTimeout(320);s=await state(page);
      check(s.hudHitRemaining===remaining,'pause holds damage feedback time');
      await page.keyboard.press('Escape',{delay:10});await until(page,t=>!t.paused&&t.timeScale===1&&!t.controls.some(c=>c.name==='ResumeButton'));
      await until(page,t=>t.hp===15,30000);await observe(page);
      await until(page,t=>t.hp===10,12000);await observe(page);
      await captureSizes(page,'hud-low-normal');
    },
    observeHUD:async(page,s)=>{
      if(!s.paused&&!s.finished&&((s.hudSuccessRemaining>.04&&!pausedSuccess)||(s.staminaIconName==='icon-warning'&&!pausedBreak))){
        await page.keyboard.press('Escape',{delay:10});
        const held=await until(page,t=>t.paused);
        await page.waitForTimeout(320);const after=await state(page);
        check(after.hudSuccessRemaining===held.hudSuccessRemaining&&after.staminaIconName===held.staminaIconName,'pause holds success and stun icon feedback');
        if(held.hudSuccessRemaining>0) pausedSuccess=true;
        if(held.staminaIconName==='icon-warning') pausedBreak=true;
        await page.keyboard.press('Escape',{delay:10});await until(page,t=>!t.paused&&t.timeScale===1&&!t.controls.some(c=>c.name==='ResumeButton'));
      }
      if(s.slayer&&!capturedSlayer){capturedSlayer=true;await captureSizes(page,'hud-slayer');}
    }
  },browser);
  check(pausedSuccess&&pausedBreak,'real success and stamina warning were paused mid-feedback');
  for(const key of ['low','success','window','cue','lock','ready','slayer','warning','shield','results','continue'])
    check(seen[key],'observed HUD state '+key);
  // Restart and death use the standard HUD presentation.
  const {context,page}=await session(browser,{width:640,height:360},1000);
  try{
    await start(page,true);await until(page,t=>t.encounterReading);
    await page.keyboard.press('Escape',{delay:25});await until(page,t=>t.paused);
    await page.keyboard.press('r',{delay:25});s=await until(page,t=>t.encounterReading&&t.hp===40&&!t.paused&&!t.controls.some(c=>c.name==='ResumeButton'));
    check(s.hudHitRemaining===0&&s.hudSuccessRemaining===0&&!s.guardBreakVisible&&!s.encounterVisible&&!s.encounterBackgroundVisible,'restart clears feedback and hides difficulty during intro');
    await page.keyboard.press('Enter',{delay:70});await until(page,t=>!t.encounterReading);
    s=await until(page,t=>t.finished&&t.hp===0,60000);
    check(!s.guardBreakVisible&&s.hudHitRemaining===0&&s.hudSuccessRemaining===0&&dim(s.parryKeyColor)&&!s.encounterVisible&&!s.encounterBackgroundVisible,'death clears feedback, locks keycap and hides difficulty backplate');
    check(!red(s.hpNumberColor),'zero HP does not show the living low-HP state');

  }finally{await context.close();}
  check(report.exceptions.length===0,'no managed or browser exceptions');
};
