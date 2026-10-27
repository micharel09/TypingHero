// Only native input changes gameplay. The QA bridge observes scene bindings and geometry.
module.exports = async function scoreCluster(api, browser) {
  const {session,start,state,until,clickControl,check,report}=api;
  const sizes=[[1280,720],[400,300]];
  const geometry=(s,label)=>{
    const k=Math.min(s.width/960,s.height/540);
    const center=r=>r.x+r.width/2;
    check(s.comboUnderScore && s.comboDecayUnderScore,label+': multiplier and decay belong to score cluster');
    const frame=s.scoreFrameRect;
    const left=frame.x, right=frame.x+frame.width;
    const top=frame.y+frame.height, bottom=frame.y;
    const score=s.scoreGlyphRect, mult=s.comboGlyphRect, decay=s.decayRect;
    check(score.x>=left && score.x+score.width<=right && score.y>=bottom && score.y+score.height<=top,label+': score inside frame');
    if(s.comboVisible){
      check(mult.x>=score.x+score.width && mult.x+mult.width<=right,label+': multiplier to right without overlap');
      check(decay.y>=bottom && decay.y+decay.height<=Math.min(score.y,mult.y),label+': decay inside frame below text');
    } else check(Math.abs(center(s.scoreRect)-s.width/2)<1,label+': x1 total centered');
    check(Math.abs(s.playerStatusRect.height/k-50.4)<1,label+': player status contains HP row only');
    check(s.comboVisible===(s.multiplier>1.0001) && s.comboDecayVisible===s.comboVisible,label+': visibility follows actual multiplier');
    if(s.comboVisible) check(Math.abs(s.comboDecayValue-Math.max(0,Math.min(1,s.comboRemaining/s.comboWindowSeconds)))<.002+s.frameMs/1000/s.comboWindowSeconds,label+': fill follows live remaining time within one update');
  };
  const resizeCapture=async(page,label,live=false)=>{
    for(const [width,height] of sizes){
      await page.setViewportSize({width,height});
      const before=await until(page,s=>s.width===width && s.height===height);
      const s=await until(page,s=>s.frame>=before.frame+3);
      geometry(s,label+' '+width+'x'+height);
      if(live){
        await page.keyboard.press('Escape',{delay:10});
        await until(page,s=>!s.paused && s.timeScale===1 && !s.controls.some(c=>c.name==='ResumeButton'));
      }
      await page.screenshot({path:report.outputDirectory+'/'+label+'-'+width+'x'+height+'.png'});
      if(live){await page.keyboard.press('Escape',{delay:10});await until(page,s=>s.paused);}
    }
    await page.setViewportSize({width:640,height:360});
    await until(page,s=>s.width===640 && s.height===360);
  };
  // Slayer is exercised in real combat now that the guide has no practice mode.
  const battle=await session(browser,{width:640,height:360},1000);
  try {
    const page=battle.page;
    const menu=await until(page,s=>s.scene==='MainMenu' && !s.dungeonTransitioning);
    await until(page,s=>s.frame>=menu.frame+4);
    if(!(await state(page)).controls.some(c=>c.kind==='input')) {
      await clickControl(page,'PlayButton','button');
      await until(page,s=>s.controls.some(c=>c.kind==='input'));
    }
    await clickControl(page,'<Button','button');
    await clickControl(page,'<Button','button');
    await until(page,s=>s.menuDifficulty==='very_easy');
    await start(page,false);
    let initial=await until(page,s=>!s.dungeonTransitioning);
    if(initial.tutorial){
      await page.keyboard.press('Enter',{delay:350});
      await until(page,s=>!s.tutorial && !s.dungeonTransitioning);
    }
    await until(page,s=>!s.tutorial && !s.introRunning && !s.dungeonTransitioning && s.timeScale===1);
    await page.keyboard.press('Escape',{delay:40});await until(page,s=>s.paused);
    await resizeCapture(page,'normal-x1',true);
    await page.keyboard.press('Escape',{delay:60});
    await until(page,s=>!s.encounterReading && s.timeScale===1);
    const word=async()=>{
      const s=await state(page),before=s.words;
      await page.keyboard.type(s.current,{delay:12});
      return until(page,t=>t.words>before);
    };
    for(let i=0;i<5;i++) await word();
    let s=await until(page,s=>s.multiplier>1 && s.comboDecayVisible);
    check(Math.abs(s.multiplier-1.15)<.001,'fifth normal word still activates x1.15');
    const scorePoint=s.scorePoint;
    await page.keyboard.press('Escape',{delay:40});
    await until(page,s=>s.paused && s.timeScale===0);
    await resizeCapture(page,'normal-combo',true);
    const held=await until(page,s=>s.paused && s.timeScale===0);
    await page.waitForTimeout(1700);
    s=await state(page);
    check(s.multiplier===held.multiplier && Math.abs(s.comboRemaining-held.comboRemaining)<.001,'pause holds grouped combo timer');
    await page.keyboard.press('Escape',{delay:40});
    s=await until(page,s=>!s.paused && s.multiplier===1 && !s.comboDecayVisible);
    check(!s.comboVisible && Math.abs(s.scorePoint.y-scorePoint.y)<1,'expiry hides both rows without moving total');
    const beforeTimeout=await state(page);
    s=await until(page,s=>s.multiplier===1 && s.words===beforeTimeout.words && s.wordTime>beforeTimeout.wordTime,10000);
    check(!s.comboVisible && !s.comboDecayVisible,'word timeout leaves multiplier and decay hidden');
    await page.keyboard.press('Escape',{delay:30});await until(page,s=>s.paused);
    await page.keyboard.press('r',{delay:30});
    s=await until(page,s=>!s.dungeonTransitioning && !s.tutorial && s.score===0 && s.hp===40);
    check(!s.comboVisible && !s.comboDecayVisible,'normal restart clears grouped UI');
    await until(page,s=>!s.introRunning && s.timeScale===1);
    // Keep typing sparse enough for the enemy to finish a real swing.
    for(let i=0;i<5;i++) await word();
    let renewed=false;
    const parryDeadline=performance.now()+15000;
    while(!renewed && performance.now()<parryDeadline){
      s=await state(page);
      if(s.finished) throw new Error('Encounter ended before combo renewal');
      const contact=s.enemyContactIn-s.qaAgeMs/1000*s.timeScale;
      if(s.multiplier>1 && s.enemyAttacking && s.canParry && !s.parryPose && contact>.025 && contact<.25){
        const before=s;
        await page.keyboard.down('Space');
        try {s=await until(page,t=>t.matchParries>before.matchParries,1800);} finally {await page.keyboard.up('Space');}
        check(s.multiplier>before.multiplier && s.comboRemaining>0 && s.comboDecayVisible,'successful parry renews multiplier and grouped timer');
        renewed=true;
      } else if(s.multiplier<=1 || s.comboRemaining<.8) await word();
      await page.waitForTimeout(20);
    }
    check(renewed,'real successful parry observed with active combo');
    await page.screenshot({path:report.outputDirectory+'/normal-parry-renew.png'});
    const slayerDeadline=performance.now()+25000;
    while(!(await state(page)).slayer && performance.now()<slayerDeadline){
      const count=(await state(page)).matchParries;
      await page.waitForFunction(()=>{
        const s=window.typingHeroQA;
        if(s?.slayer) return true;
        if(!s || !s.enemyAttacking || !s.canParry || s.parryPose || s.paused || s.finished) return false;
        const age=(performance.now()-window.typingHeroQAPublishedAt)/1000*s.timeScale;
        const contact=s.enemyContactIn-age;
        return contact>.025 && contact<.20;
      },null,{polling:'raf',timeout:12000});
      if((await state(page)).slayer) break;
      await page.keyboard.down('Space');
      try {await until(page,s=>s.matchParries>count,1800);} finally {await page.keyboard.up('Space');}
    }
    await until(page,s=>s.slayer && s.redAlpha>.99);
    for(let i=0;i<24 && (await state(page)).multiplier<3;i++) {
      s=await state(page);
      await page.keyboard.type(s.current,{delay:35});
      await until(page,t=>t.inputLetters>s.inputLetters,2000);
    }
    s=await until(page,s=>s.slayer && s.multiplier===3 && s.comboVisible);
    geometry(s,'Slayer maximum combo');
    check(s.popupCount===1 && s.popupAlpha>0,'award popup remains beside character');
    await page.keyboard.press('Escape',{delay:30});await until(page,s=>s.paused);
    await resizeCapture(page,'slayer-x3',true);
    await page.keyboard.press('Escape',{delay:30});await until(page,s=>!s.paused && !s.controls.some(c=>c.name==='ResumeButton'));
    const exitDeadline=performance.now()+15000;
    while((await state(page)).slayer && performance.now()<exitDeadline){
      await page.waitForTimeout(850);
      s=await state(page);
      if(s.slayer && s.current.length) await page.keyboard.type(s.current[0],{delay:30});
    }
    s=await until(page,s=>!s.slayer && s.redAlpha===0 && s.barReveal===0,5000);
    geometry(s,'natural Slayer exit');
    check(s.redAlpha===0 && s.environmentVisible,'natural Slayer exit restores normal HUD');
    await page.screenshot({path:report.outputDirectory+'/slayer-natural-exit.png'});
    if(s.multiplier<=1) for(let i=0;i<5;i++) await word();
    s=await until(page,s=>!s.slayer && s.multiplier>1 && s.comboDecayVisible);
    geometry(s,'normal combo after Slayer');
    await page.screenshot({path:report.outputDirectory+'/normal-after-slayer.png'});
    check(report.exceptions.length===0,'no JavaScript/managed exceptions');
  } finally {await battle.context.close();}
};
