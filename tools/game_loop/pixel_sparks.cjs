// Real keyboard input; the QA bridge is observation only. Baseline runs are measurements.
const path=require('node:path');
module.exports=async function pixelSparks(api,browser){
  const {session,start,state,until,check,report}=api;
  const baseline=process.env.TYPINGHERO_PIXEL_BASELINE==='1';
  const parryOnly=process.env.TYPINGHERO_PIXEL_PARRY_ONLY==='1';
  report.pixelSparks=[];
  for(const [width,height] of [[1280,720],[640,360]]){
    const opened=performance.now();
    const {context,page}=await session(browser,{width,height},1000);
    const row={width,height,baseline,samples:[],roles:[],captures:[]}; report.pixelSparks.push(row);
    const seen=new Set(), pixelSeen=new Set();
    const observe=async()=>{
      const s=await state(page);
      const roles=(s.sparks||[]).map(p=>p.role);
      const phase=s.finished?'lethal':roles.some(r=>r.startsWith('Slayer'))?'slayer':roles.includes('Parry')?'parry':roles.includes('Normal')?'normal':'idle';
      if(row.samples.at(-1)?.frame!==s.frame)row.samples.push({frame:s.frame,frameMs:s.frameMs,memory:s.memory,objects:s.objects,
        allocatedBytes:s.allocatedBytes,allocationCounterAvailable:s.allocationCounterAvailable,
        slayer:s.slayer,paused:s.paused,phase,particles:(s.sparks||[]).reduce((n,p)=>n+(p.particles||0),0),bursts:s.sparks?.length||0});
      for(const p of s.sparks||[]){
        seen.add(p.role.startsWith('Slayer')?'Slayer':p.role);
        if(!baseline&&p.particles>0){
          pixelSeen.add(p.role);
          const budget={Normal:24,Parry:40,Slayer:32,Lethal:56}[p.role];
          check(p.particles<=budget,'profile particle budget');
          if(p.maxAge<.3&&p.particles===budget)
            check(Number.isFinite(p.meanVelocityX)&&p.meanVelocityX*Math.sign(s.enemyFootPoint.x-s.playerFootPoint.x)>0,
              'burst force points from attacker toward target, including mirrored actors');
          check(p.order===30,'pixel sparks retain sorting order30');
          check(p.minY>=p.groundY-.005,'particles stay above fixed ground');
        }
      }
      check((s.sparks?.length||0)<=16,'both owners stay within eight bursts each');
      return s;
    };
    const capture=async role=>{
      await page.keyboard.press('Escape',{delay:20});
      let s=await until(page,t=>t.paused&&t.timeScale===0);
      s=await until(page,t=>t.paused&&t.frame>=s.frame+2);
      const sparks=s.sparks;
      if(!baseline)check(sparks.some(p=>p.role.toLowerCase().startsWith(role)&&p.particles>0),'pause retains the requested live burst');
      await page.waitForTimeout(200);
      const held=await observe();
      check(JSON.stringify(held.sparks.map(p=>[p.id,p.particles,p.maxAge,p.minY,p.maxY]))===
        JSON.stringify(sparks.map(p=>[p.id,p.particles,p.maxAge,p.minY,p.maxY])),'pause holds simulation and lifetime');
      row.captures.push({role,sparks,slayer:s.slayer,ms:performance.now()-opened});
      await page.keyboard.press('Escape',{delay:20});await until(page,t=>!t.paused&&t.timeScale>0);
      // The menu covers the arena; keep the hold assertion separate from live readback.
      await page.screenshot({path:path.join(report.outputDirectory,`pixel-${width}-${role}.png`)});
    };
    const restart=async()=>{
      let s=await observe();
      if(!s.overlay&&!s.paused){await page.keyboard.press('Escape',{delay:30});await until(page,t=>t.paused);}
      await page.keyboard.press('r',{delay:40});
      const ready=await until(page,t=>!t.paused&&!t.overlay&&t.hp===40&&t.enemyHp===850&&t.score===0);
      await until(page,t=>t.frame>=ready.frame+2&&t.sparks.length===0);
      check((await observe()).sparks.length===0,'restart reclaims prior bursts');
    };
    try{
      let s=await start(page); check(s.width===width&&s.height===height,'exact requested render size');
      if(!parryOnly){
        s=await until(page,t=>!t.enemyAttacking&&t.canParry&&t.current.length>0&&t.wordTime>1);
        await page.keyboard.type(s.current,{delay:15});
        s=await until(page,t=>t.enemyHp<850&&t.sparks.some(p=>p.role==='Normal'));
        await observe();await capture('normal');await restart();
      }
      let lastParry=-Infinity,parryCaptured=false,slayerCaptured=false,lethalCaptured=false;
      const deadline=performance.now()+180000;
      while(performance.now()<deadline){
        s=await observe();
        if(s.finished){
          if(s.sparks.some(p=>p.role==='Lethal')){
            await page.screenshot({path:path.join(report.outputDirectory,`pixel-${width}-lethal.png`)});
            row.captures.push({role:'lethal',sparks:s.sparks,ms:performance.now()-opened}); lethalCaptured=true;
          }
          // Legacy180ms bursts can finish during a whole-word key sequence. The
          // existing post-LateUpdate mask records their rendered presence at frame rate.
          if(baseline&&(s.sparkMask&4)){
            seen.add('Lethal');lethalCaptured=true;
            row.legacyLethalRenderedMask=s.sparkMask;
          }
          check(s.enemyHp===0&&s.hp>0,'keyboard combat reaches lethal victory');
          await until(page,t=>t.overlay&&t.sparks.length===0);break;
        }
        if(!parryCaptured&&s.sparks.some(p=>p.role==='Parry')){
          await capture('parry');parryCaptured=true;
          if(parryOnly){
            await page.waitForTimeout(1000);
            check(!(await observe()).sparks.some(p=>p.role==='Parry'),'parry motion finishes and naturally releases');
            break;
          }
          continue;
        }
        if(s.slayer){
          if(!slayerCaptured&&s.sparks.some(p=>p.role.startsWith('Slayer'))){
            await capture('slayer');slayerCaptured=true;continue;
          }
          if(s.current){await page.keyboard.type(s.current,{delay:35});}
        }else{
          const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
          if(s.canParry&&s.enemyAttacking&&impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12)&&impactIn>.04&&performance.now()-lastParry>350){
            await page.keyboard.down('Space');
            try{await until(page,t=>t.parryPose||t.parryWindow||t.matchParries>s.matchParries,1500);}
            finally{await page.keyboard.up('Space');}
            lastParry=performance.now();
          }
        }
        await page.waitForTimeout(15);
      }
      row.roles=[...seen];row.pixelRoles=[...pixelSeen];
      check(parryCaptured,'successful keyboard parry rendered');
      if(!baseline)check((parryOnly?['Parry']:['Normal','Parry','Slayer','Lethal']).every(r=>pixelSeen.has(r)),'requested roles use live particle prefabs');
      if(!parryOnly)check(slayerCaptured&&lethalCaptured&&['Normal','Parry','Slayer','Lethal'].every(r=>seen.has(r)),'all four roles rendered in normal and Slayer camera');
      await restart();
      console.log(JSON.stringify({stage:'pixel-sparks',width,roles:row.roles,samples:row.samples.length}));
    }finally{await context.close();if(page.video())row.video=await page.video().path();}
  }
};
