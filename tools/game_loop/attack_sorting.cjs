// Real keyboard actions only; sorting observations come from post-LateUpdate frames.
module.exports=async function attackSorting(api,browser){
  const {session,start,state,until,check,report}=api;
  const baseline=process.env.TYPINGHERO_ATTACK_SORTING_BASELINE==='1';
  const {context,page}=await session(browser,{width:640,height:360},1000);
  const inspect=async()=>{
    const s=await state(page);
    if(!baseline&&s.playerSorting){
      check(s.playerSorting.mismatches===0,'every observed rendered frame has correct sorting');
      check(s.playerSorting.enemyOrder===0,'skeleton retains order0');
    }
    return s;
  };
  const capture=async label=>{
    await page.screenshot({path:report.outputDirectory+`/${baseline?'before':'after'}-${label}.png`});
    await page.keyboard.press('Escape',{delay:30});
    const s=await until(page,t=>t.paused&&t.timeScale===0);
    (report.sortingCaptures||=[]).push({label,sorting:s.playerSorting});
    await page.waitForTimeout(250);
    const held=await inspect();
    if(!baseline) check(held.playerSorting.order===s.playerSorting.order,'pause holds order');
    await page.keyboard.press('Escape',{delay:30});
    await until(page,t=>!t.paused);
  };
  const restart=async()=>{
    const s=await inspect();
    if(!s.overlay&&!s.paused){await page.keyboard.press('Escape',{delay:60});await until(page,t=>t.paused);}
    await page.keyboard.press('r',{delay:60});
    await until(page,t=>!t.paused&&!t.overlay&&t.hp===40&&t.enemyHp===850&&t.score===0);
    if(!baseline) await until(page,t=>t.playerSorting?.order===0);
  };
  try{
    let s=await start(page);
    s=await until(page,t=>!t.enemyAttacking);
    await page.keyboard.type(s.current,{delay:80});
    await until(page,t=>t.score>0,3000);
    if(!baseline) await until(page,t=>t.playerSorting?.attack&&t.playerSorting.order===1,3000);
    else await page.waitForTimeout(20);
    await capture('normal');
    await restart();
    let lastParry=-Infinity,captured=false,mask=0;
    const capturedStates=new Set();
    const deadline=performance.now()+120000;
    while(performance.now()<deadline){
      s=await inspect();mask|=s.playerSorting?.attackMask||0;
      if(s.slayer){
        if(s.current) await page.keyboard.type(s.current[0],{delay:70});
        await page.waitForTimeout(120);
        s=await inspect();mask|=s.playerSorting?.attackMask||0;
        const name=s.playerSorting?.state;
        if(baseline&&!captured){await capture('slayer');captured=true;}
        else if(name?.includes('attackS')&&!capturedStates.has(name)){
          await capture(name.split('.').pop());capturedStates.add(name);captured=true;
        }
        if(baseline||((mask&14)===14&&capturedStates.size===3))break;
      }else{
        const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
        if(s.canParry&&s.enemyAttacking&&impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12)&&impactIn>.04&&performance.now()-lastParry>350){
          await page.keyboard.down('Space');
          try{await until(page,t=>t.parryPose||t.parryWindow||t.matchParries>s.matchParries,1500);}
          finally{await page.keyboard.up('Space');}
          lastParry=performance.now();
        }
      }
      await page.waitForTimeout(25);
    }
    check(captured,'real parries and typing reach Slayer');
    if(!baseline){
      // The first normal attack preceded restart, so its bit belongs to that round.
      check((mask&14)===14&&capturedStates.size===3,'all three Slayer attack animations rendered and captured');
      await until(page,t=>!t.slayer&&t.playerSorting?.order===0&&t.environmentVisible&&t.redAlpha===0);
      s=await inspect();check(s.environmentVisible&&s.redAlpha===0,'Slayer VFX restore');
      await restart();
      await until(page,t=>t.hp<40&&!t.finished,15000);
      await until(page,t=>t.playerSorting?.order===0);
      await until(page,t=>t.overlay,60000);
      s=await inspect();check(s.hp<=0&&s.playerSorting.order===0,'hit/death leaves no elevated order');
      await restart();
      s=await inspect();check(s.playerSorting.order===0&&!s.slayer,'restart restores original order');
    }
    report.sortingMask=mask;
  }finally{await context.close();}
};
