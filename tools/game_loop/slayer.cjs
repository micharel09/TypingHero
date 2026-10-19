// Cinematic acceptance through real typing/parry input and read-only observations.
module.exports = async function slayerFlow(api, browser) {
  const {session,start,state,until,check,report} = api;
  const {context,page} = await session(browser,{width:960,height:540},1000);
  try {
    await start(page);
    await until(page,s=>s.endless && s.encounterReading);
    await page.keyboard.press('Enter',{delay:60});
    await until(page,s=>!s.encounterReading && s.timeScale===1);
    let lastPress = -Infinity;
    const deadline = performance.now()+90000;
    while (performance.now()<deadline) {
      const s=await state(page);
      if(s.slayer) break;
      if(s.overlay) throw new Error('Match ended before cinematic acceptance');
      const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
      if(s.canParry && s.enemyAttacking && impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12) && impactIn>.04 && performance.now()-lastPress>350){
        await page.keyboard.down('Space');
        try { await until(page,t=>t.parryPose || t.parryWindow || t.matchParries>s.matchParries,1500); }
        finally { await page.keyboard.up('Space'); }
        lastPress=performance.now();
      }
      await page.waitForTimeout(25);
    }
    const entry=await until(page,s=>s.slayer && s.redAlpha>.99 && s.cameraOrtho<4.5 && s.scoreZoom===1);
    let sawYellow=false, sawPulse=false, sawPopup=false;
    const typingEnd=performance.now()+4000;
    while(performance.now()<typingEnd){
      let s=await state(page);
      if(!s.slayer) throw new Error('Slayer expired before reaching x3');
      sawPulse ||= s.scorePulse>1.001;
      if(s.multiplier>=2 && s.multiplier<3){
        check(s.scoreColor.r===1 && Math.abs(s.scoreColor.g-.85)<.01 && Math.abs(s.scoreColor.b-.25)<.01,'x2 score uses configured yellow');
        report.slayerX2={score:s.score,multiplier:s.multiplier,color:s.scoreColor};
        sawYellow=true;
      }
      if(s.multiplier===3) break;
      if(!s.current) { await until(page,t=>!t.slayer || t.current.length>0); continue; }
      const before=s.inputLetters;
      await page.keyboard.type(s.current,{delay:10});
      const awarded=await until(page,t=>t.inputLetters>before);
      sawPulse ||= awarded.scorePulse>1.001;
      if(awarded.barReveal>.999){
        const glyph=awarded.scoreGlyphRect;
        check(awarded.scoreFont===36 && Math.abs(glyph.x+glyph.width/2-awarded.width/2)<1.5,'rapid Slayer award keeps12px native glyphs centered during pulse');
      }
      if(awarded.popupAlpha>0){
        const k=awarded.height/540;
        check(awarded.popupFont===18,'award font18');
        check(awarded.popupCount===1,'one active award popup');
        if(awarded.popupId===s.popupId && s.popupAlpha>0){
          check(Number(awarded.popupText.slice(1))===Number(s.popupText.slice(1))+awarded.score-s.score,'same popup accumulates exact received score delta');
        }
        check(awarded.scoreZoom===1,'score remains fixed scale during Slayer');
        check(Math.abs((awarded.popupRect.x-awarded.popupPoint.x)/k)<1.5,'award follows live anchor horizontally');
        sawPopup=true;
        report.popupObserved={rect:awarded.popupRect,anchor:awarded.popupPoint,zoom:awarded.scoreZoom};
      }
      await page.waitForTimeout(80);
    }
    let s=await until(page,s=>s.slayer && s.multiplier===3);
    report.slayerX3={score:s.score,multiplier:s.multiplier,color:s.scoreColor,zoom:s.scoreZoom};
    check(sawYellow,'real Slayer typing reaches x2');
    check(s.scoreColor.r===1 && s.scoreColor.g===0 && s.scoreColor.b===0,'real Slayer typing reaches configured x3 red');
    const rewarded=s.chars-entry.chars;
    let expected=entry.score;
    let multiplier=entry.multiplier;
    for(let i=1;i<=rewarded;i++) {
      multiplier=Math.min(3,Math.fround(multiplier+Math.fround(.15)));
      const points=Math.fround(2*multiplier), lower=Math.floor(points);
      expected+=points-lower===.5 ? lower+(lower%2) : Math.round(points);
    }
    check(rewarded>=14 && s.score===expected,'real Slayer points match character awards and capped multiplier exactly');
    await page.keyboard.press('Escape');
    await until(page,t=>t.paused);
    const popupHeld=await state(page);
    check(popupHeld.popupCount===1,'pause captured an active popup');
    if(process.env.TYPINGHERO_SCORE_LAYOUT_ONLY==='1') await page.screenshot({path:report.outputDirectory+'/score-award-x3.png'});
    await page.waitForTimeout(700);
    const popupStill=await state(page);
    check(popupStill.popupId===popupHeld.popupId && popupStill.popupElapsed===popupHeld.popupElapsed && popupStill.popupAlpha===popupHeld.popupAlpha,'pause holds popup instance and fade time');
    await page.keyboard.press('Escape');await until(page,t=>!t.paused);
    await page.waitForTimeout(250);
    s=await state(page);
    check(s.scorePulse===1,'idle combo decay does not pulse score');
    check(sawPulse,'real score awards pulse text');
    check(sawPopup,'real typing displays anchored floating award');
    await page.keyboard.press('Escape',{delay:100});await until(page,s=>s.paused);
    // Measure all render sizes while pause holds the same real x3 state.
    const held=await state(page);
    for(const [width,height] of [[960,540],[1280,720],[1920,1080],[640,360]]){
      await page.setViewportSize({width,height});
      const resized=await until(page,s=>s.width===width && s.height===height);
      await until(page,s=>s.frame>=resized.frame+4);
      s=await state(page);
      (report.slayerPausedSizes ||= []).push({width,height,left:s.barLeft,right:s.barRight});
      check(!s.environmentVisible && !s.hudPanelVisible && s.redAlpha===1,`${width}: red cinematic hides environment and HUD panel`);
      check(Math.abs(s.barLeft)<1 && Math.abs(s.barRight-width)<1,`${width}: bars span render width`);
      check(s.score===held.score,'pause holds actual score');
    }
    await page.setViewportSize({width:960,height:540});
    await until(page,s=>s.width===960 && s.height===540);
    await page.keyboard.press('Escape',{delay:80});await until(page,s=>!s.paused && s.cameraOrtho<4.5 && s.scoreZoom===1 && !s.controls.some(c=>c.name==='ResumeButton'));
    await page.screenshot({path:report.outputDirectory+'/slayer-x3.png'});
    await until(page,s=>!s.paused && !s.slayer);
    await page.waitForTimeout(500);s=await state(page);
    check(s.scoreFont===36 && s.comboFont===24 && s.barReveal===0,'exit restores normal font sizes with closed bars');
    check(s.environmentVisible && !s.hudPanelVisible && s.redAlpha===0 && Math.abs(s.scoreZoom-1)<.001,'Slayer exit restores environment, panels and red overlay');
    await page.screenshot({path:report.outputDirectory+'/slayer-exit.png'});
    await page.keyboard.press('Escape',{delay:100});await until(page,s=>s.paused);
    await page.keyboard.press('r',{delay:100});
    s=await until(page,s=>!s.paused && s.score===0 && s.hp===40);
    check(!s.slayer && s.environmentVisible && !s.hudPanelVisible && s.scoreZoom===1 && s.popupCount===0,'restart restores normal score presentation and clears popup');
  } finally { await context.close(); }
};
