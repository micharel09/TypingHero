// Targeted art/state acceptance. The bridge is read-only; all actions are keys.
const path=require('node:path');

module.exports=async function skeletonArt(api,browser){
  const {session,start,state,until,check,report}=api;
  report.skeletonArt=[];
  for(const [width,height] of [[1280,720],[640,360]]){
    const {context,page}=await session(browser,{width,height},1000);
    const record={width,height,captures:[],sprites:[],states:[]};
    report.skeletonArt.push(record);
    const sprites=new Set();
    const observe=async()=>{const s=await state(page);if(s.enemySprite)sprites.add(s.enemySprite);return s;};
    const capture=async label=>{
      const before=await observe();
      await page.screenshot({path:path.join(report.outputDirectory,`skeleton-${width}-${label}.png`)});
      const after=await observe();
      record.captures.push({label,before:before.enemySprite,after:after.enemySprite,frame:after.frame});
    };
    const restart=async()=>{
      let s=await observe();
      if(!s.overlay&&!s.paused){await page.keyboard.press('Escape',{delay:80});await until(page,t=>t.paused);}
      await page.keyboard.press('r',{delay:80});
      s=await until(page,t=>!t.paused&&!t.overlay&&t.hp===40&&t.enemyHp===850&&t.score===0);
      check(!s.slayer&&!s.stunned,`${width}: restart clears Slayer/stun`);
      return s;
    };
    try{
      let s=await start(page);
      s=await until(page,t=>t.enemyTexture?.endsWith('_Idle'));
      check(s.width===width&&s.height===height,`${width}: exact Unity render size`);
      await capture('idle');record.states.push('Idle');
      await page.keyboard.press('Escape',{delay:80});
      const announced=await until(page,t=>t.paused&&t.timeScale===0);
      // Publication may precede this frame's Animator evaluation. Measure the
      // frozen pose after two rendered frames, retaining exact hold assertions.
      const paused=await until(page,t=>t.paused&&t.timeScale===0&&t.frame>=announced.frame+2);
      await page.waitForTimeout(350);
      s=await observe();
      record.pause={announced,paused,held:s};
      check(s.hp===paused.hp&&s.enemyHp===paused.enemyHp&&s.enemySprite===paused.enemySprite&&s.wordTime===paused.wordTime,`${width}: pause holds sprite and combat`);
      await page.keyboard.press('Escape',{delay:80});await until(page,t=>!t.paused&&t.timeScale===1);
      const cue=await until(page,t=>t.cue&&t.enemyAttacking);
      check(cue.enemyTexture.endsWith('_Attack1'),`${width}: telegraph uses Attack1`);
      record.states.push('Attack');
      s=await until(page,t=>t.hp<cue.hp);
      const lead=s.combatSeconds-cue.combatSeconds;
      record.impact={cueTime:cue.combatSeconds,impactTime:s.combatSeconds,damage:cue.hp-s.hp,lead};
      check(cue.hp-s.hp===5&&lead>0&&lead<.6,`${width}: cue precedes one5HP impact`);
      record.cueLead=lead;await capture('after-impact');
      // Screenshots can stall software WebGL. Capture a different swing so
      // readback cannot contaminate the cue-to-damage timing measurement.
      await until(page,t=>!t.cue);
      await until(page,t=>t.cue&&t.enemyAttacking);
      await capture('telegraph');
      record.hurtOpeningTrace=[];
      s=await until(page,t=>{
        record.hurtOpeningTrace.push({frame:t.frame,time:t.combatSeconds,hp:t.hp,canParry:t.canParry,wordTime:t.wordTime,enemy:t.enemySprite,attacking:t.enemyAttacking,player:t.playerSorting?.sprite});
        return !t.enemyAttacking&&t.canParry&&t.current.length>0&&
          t.wordTime>t.current.length*.015+.2&&t.enemyTexture.endsWith('_Idle')&&t.enemyAnimationTime<1;
      });
      const hp=s.enemyHp,word=s.current;
      record.hurtInput={before:s,word};
      await page.keyboard.type(word,{delay:15});
      record.hurtInput.after=await until(page,t=>t.inputLetters>=s.inputLetters+word.length);
      s=await until(page,t=>t.enemyHp<hp&&t.enemyTexture.endsWith('_Hurt'));
      check(s.inputLetters>=word.length,`${width}: real typing causes Hurt`);
      await capture('hurt');record.states.push('Hurt');
      await until(page,t=>t.enemyTexture.endsWith('_Idle'));record.states.push('Hurt->Idle');
      await restart();
      let lastParry=-Infinity,sawStun=false,sawSlayerHit=false,deathCaptured=false;
      const deadline=performance.now()+180000;
      while(performance.now()<deadline){
        s=await observe();
        if(s.finished){
          check(s.hp>0&&s.enemyHp===0,`${width}: real-input enemy death`);
          s=await until(page,t=>t.enemyTexture.endsWith('_Die')||t.enemyHp<0);
          check(s.enemyTexture.endsWith('_Die'),`${width}: death enters Die sheet before cleanup`);
          await capture('die');deathCaptured=true;record.states.push('Die');
          await until(page,t=>t.overlay&&t.enemyHp<0&&!t.enemySprite);
          check(true,`${width}: death event removes enemy and opens result`);
          record.states.push('cleanup');break;
        }
        if(s.stunned&&!sawStun){
          check(s.slayer&&s.matchParries>0,`${width}: real parries activate Stun/Slayer`);
          s=await until(page,t=>t.stunned&&t.enemyTexture.endsWith('_Hurt'));
          await capture('stun');sawStun=true;record.states.push('Stun');
          s=await observe();
        }
        const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
        if(s.canParry&&s.enemyAttacking&&impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12)&&impactIn>.04&&performance.now()-lastParry>350){
          await page.keyboard.down('Space');
          try{await until(page,t=>t.parryPose||t.parryWindow||t.matchParries>s.matchParries,1500);}
          finally{await page.keyboard.up('Space');}
          lastParry=performance.now();continue;
        }
        if(s.current&&(s.slayer||(!s.enemyAttacking&&s.canParry))){
          const letters=s.inputLetters,beforeHp=s.enemyHp,wasSlayer=s.slayer;
          await page.keyboard.type(s.current,{delay:15});
          s=await until(page,t=>t.inputLetters>letters||t.finished,1500);
          if(wasSlayer&&!sawSlayerHit&&!s.finished){
            s=await until(page,t=>t.enemyHp<beforeHp&&t.enemyTexture.endsWith('_Hurt'));
            await capture('slayer-hit');sawSlayerHit=true;record.states.push('hitBySlayer');
          }
          const frame=s.frame;await until(page,t=>t.frame>=frame+2||t.finished,1500);
        }else await page.waitForTimeout(25);
      }
      check(deathCaptured&&sawStun&&sawSlayerHit,`${width}: all required art consumers observed`);
      await restart();await capture('restart');
      record.sprites=[...sprites].sort();
      console.log(JSON.stringify({stage:'skeleton-art',width,states:record.states,cueLead:record.cueLead}));
    }finally{await context.close();}
  }
};
