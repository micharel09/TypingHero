// Targeted HUD victory/restart through actual keyboard input; no gameplay mutations.
module.exports = async function hudVictory(api, page) {
  const {state,until,check,report}=api;
  let lastPress=-Infinity;
  const end=performance.now()+180000;
  let sawSlayer=false, sawStaminaChange=false, lastStamina=250;
  while(performance.now()<end){
    let s=await state(page);
    if(s.finished){
      check(s.hp>0 && s.enemyHp===0,'HUD keyboard round wins against850HP');
      check(sawSlayer && sawStaminaChange,'real parry/typing drains stamina and enters Slayer before victory');
      await until(page,t=>t.overlay);
      await page.screenshot({path:report.outputDirectory+'/hud-victory.png'});
      await page.keyboard.press('r',{delay:100});
      s=await until(page,t=>!t.overlay && t.hp===40 && t.enemyHp===850 && t.score===0);
      check(s.playerHpFill===1 && s.enemyHpFill===1 && s.staminaFill===1,'victory restart restores relocated bars');
      check(!s.slayer && s.scoreZoom===1,'victory restart restores normal HUD');
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
    if(s.current && (s.slayer || !s.enemyAttacking)){
      const letters=s.inputLetters;
      await page.keyboard.type(s.current,{delay:15});
      await until(page,t=>t.inputLetters>letters || t.finished,1500);
      const frame=(await state(page)).frame;
      await until(page,t=>t.frame>=frame+2 || t.finished,1500);
    } else await page.waitForTimeout(25);
  }
  throw new Error('HUD victory did not finish within180seconds: '+JSON.stringify(await state(page)));
};
