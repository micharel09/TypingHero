// Targeted HUD victory/restart through actual keyboard input; no gameplay mutations.
module.exports = async function hudVictory(api, page) {
  const {state,until,check,report}=api;
  const initial=await until(page,s=>s.endless && s.encounterReading), enemyHealth=initial.enemyHp;
  if(initial.encounterReading){
    await page.keyboard.press('Enter',{delay:60});
    await until(page,s=>!s.encounterReading && s.timeScale===1);
  }
  let lastPress=-Infinity;
  const end=performance.now()+180000;
  let sawSlayer=false, sawStaminaChange=false, lastStamina=250;
  while(performance.now()<end){
    let s=await state(page);
    if(s.finished){
      check(s.hp>0 && s.enemyHp<=0,`HUD keyboard round wins against${enemyHealth}HP`);
      check(sawSlayer && sawStaminaChange,'real parry/typing drains stamina and enters Slayer before victory');
      await until(page,t=>t.popupCount===0);
      check((await state(page)).popupCount===0,'lethal popup completes fade despite score freeze');
      const ended=await until(page,t=>t.barReveal===0 && t.scoreFont===36 && t.comboFont===24);
      check(ended.scoreZoom===1 && ended.redAlpha===0,'lethal restores normal score fonts/scale and closes frame after actor destruction');
      report.hudVictory={enemyHealth,score:ended.score,seconds:ended.combatSeconds,hp:ended.hp};
      await page.screenshot({path:report.outputDirectory+'/hud-victory.png'});
      if(!(await state(page)).overlay){
        await page.keyboard.press('Escape',{delay:100});
        await until(page,t=>t.paused);
      }
      await page.keyboard.press('r',{delay:100});
      s=await until(page,t=>!t.overlay && !t.paused && !t.finished && t.endless && t.encounterReading && t.hp===40 && t.enemyHp===enemyHealth && t.score===0);
      check(s.playerHpFill===1 && s.enemyHpFill===1 && s.staminaFill===1,'victory restart restores relocated bars');
      check(!s.slayer && s.scoreZoom===1 && s.popupCount===0,'victory restart restores normal HUD and clears awards');
      return;
    }
    sawSlayer ||= s.slayer;
    sawStaminaChange ||= s.stamina!==lastStamina;
    lastStamina=s.stamina;
    const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
    if(s.canParry && s.enemyAttacking && impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12) && impactIn>.04 && performance.now()-lastPress>350){
      await page.keyboard.down('Space');
      try{await until(page,t=>t.parryPose || t.parryWindow || t.matchParries>s.matchParries,1500);}
      finally{await page.keyboard.up('Space');}
      lastPress=performance.now();
      continue;
    }
    // Keep watching the cue during normal attacks; in Slayer every new letter strikes.
    if(s.current && (s.slayer || (sawSlayer && !s.enemyAttacking))){
      const letters=s.inputLetters;
      await page.keyboard.type(s.current,{delay:15});
      await until(page,t=>t.inputLetters>letters || t.finished,1500);
      const frame=(await state(page)).frame;
      await until(page,t=>t.frame>=frame+2 || t.finished,1500);
    } else await page.waitForTimeout(25);
  }
  throw new Error('HUD victory did not finish within180seconds: '+JSON.stringify(await state(page)));
};
