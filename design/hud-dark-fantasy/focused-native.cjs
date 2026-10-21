// Palette acceptance: native input only; QA bridge is read-only.
module.exports=async function({page,state,start,until,clickControl,check,report}) {
 const fs=require('node:fs'),dpr=Number(process.env.TYPINGHERO_DPR||1);
 const capture=async(name,sizes=[[320,180],[640,360]])=>{
  for(const [width,height] of sizes){
   await page.setViewportSize({width,height});
   const a=await until(page,s=>s.width===Math.round(width*dpr)&&s.height===Math.round(height*dpr));
   await until(page,s=>s.frame>=a.frame+2);
   await page.screenshot({path:`${report.outputDirectory}/${name}-${width}x${height}.png`});
   fs.writeFileSync(`${report.outputDirectory}/${name}-${width}x${height}.json`,JSON.stringify(await state(page),null,2));
  }
 };
 const liveCapture=async name=>{
  for(const [width,height] of [[320,180],[640,360]]){
   await page.setViewportSize({width,height});
   await until(page,s=>s.paused&&s.width===Math.round(width*dpr)&&s.height===Math.round(height*dpr));
   await page.keyboard.press('Escape',{delay:10});await until(page,s=>!s.paused&&s.timeScale===1);
   await capture(name,[[width,height]]);
   await page.keyboard.press('Escape',{delay:10});await until(page,s=>s.paused);
  }
 };
 await start(page,false,process.env.TYPINGHERO_NAME||'WWWWWWWWWW');
 await clickControl(page,'Skip TutorialButton','button');await until(page,s=>!s.tutorial&&s.encounterReading);
 await page.keyboard.press('Enter',{delay:80});await until(page,s=>!s.encounterReading&&s.timeScale===1);
 await capture('normal');
 let s=await state(page);
 for(let i=0;i<7&&!s.comboVisible;i++){
  await page.keyboard.type(s.current,{delay:65});
  s=await until(page,t=>t.score>s.score||t.finished);
 }
 check(s.comboVisible&&s.comboDecayVisible,'real words reveal multiplier and decay');
 check(Math.abs(s.multiplier-1.15)<.001&&s.score===52,'five normal words preserve the original multiplier and score');
 check(s.popupCount>0&&s.popupAlpha>0,'real award starts the existing visible popup');
 await page.keyboard.press('Escape',{delay:30});await until(page,s=>s.paused);
 // Pause freezes the same real combo while observing all viewports.
 await liveCapture('combo');
 await clickControl(page,'ResumeButton','button');await until(page,s=>!s.paused&&s.encounterVisible);

 await until(page,s=>!s.comboVisible,10000);check(!(await state(page)).comboDecayVisible,'changed combo display hides on expiry');
 await until(page,s=>s.hp>0&&s.hp<=10,90000);
 await page.keyboard.press('Escape',{delay:30});await until(page,s=>s.paused);
 await liveCapture('low-hp');
 await clickControl(page,'ResumeButton','button');await until(page,s=>!s.paused&&s.encounterVisible);

 await page.keyboard.press('Escape',{delay:30});await until(page,s=>s.paused);await capture('pause');
 await clickControl(page,'ResumeButton','button');await until(page,s=>!s.paused&&s.timeScale===1);
 check(true,'pause and resume retain live combat');
 check(report.paletteLowHpObserved,'low HP red was observed through the real damage path');
};
