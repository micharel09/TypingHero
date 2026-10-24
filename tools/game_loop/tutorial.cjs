// Keyboard/mouse only. QA state observes, never changes gameplay.
module.exports = async function tutorialFlow(api,page,options={}) {
 const {state,until,clickControl,check,report}=api;
 const action='Start Battle \u00b7 EnterButton';
 const key=async k=>{await page.keyboard.press(k,{delay:80});await page.waitForTimeout(80);};
 const at=n=>s=>s.tutorial&&s.tutorialStep===n&&!s.paused&&!s.dungeonTransitioning;
 const shot=name=>page.screenshot({path:report.outputDirectory+'/'+name+'.png'});
 const menu=async()=>{await key('Escape');await until(page,s=>s.paused);await key('m');await until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);};
 const launch=async replay=>{
  await until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);
  await page.locator('#unity-canvas').click({position:{x:10,y:10}});await page.waitForTimeout(350);
  await clickControl(page,replay?'TutorialButton':'PlayButton','button');
  let s=await until(page,s=>s.scene==='SampleScene'||s.controls.some(c=>c.name==='NameInput'));
  if(s.scene!=='SampleScene'){await clickControl(page,'NameInput','input');await page.keyboard.press('Control+A');await page.keyboard.type('LoopQA',{delay:30});await key('Enter');}
  return until(page,s=>s.scene==='SampleScene'&&!s.dungeonTransitioning&&s.current.length>0);
 };
 const writes=report.fixtureWrites;
 if(!options.freshTutorial)await launch(false);
 let s=await until(page,at(0));const status=s.tutorialStatus;
 const titles=['Type to Attack','Mistakes & Timer','Parry with Space','Break Guard \u00b7 Slayer','Score & Combo'];
 for(const [width,height] of options.viewports||[[1280,720],[640,360]]){
  await page.setViewportSize({width,height});await until(page,s=>s.width===width&&s.height===height);
  s=await state(page);check(s.tutorialArrows.length===5&&s.typography.some(t=>t.text==='1/6'),'HUD overview at '+width);await shot('tutorial-hud-'+width);
  for(let n=1;n<=5;n++){
   await clickControl(page,n===1?'PracticeButton':action,'button');s=await until(page,at(n));
   check(s.tutorialStatus===status&&s.tutorialArrows.length===0,'page '+(n+1)+' remains unread');
   const text=s.typography.filter(t=>t.path.includes('/ParryModal/'));
   check(text.some(t=>t.text===titles[n-1])&&text.some(t=>t.text===`${n+1}/6`)&&text.some(t=>t.text===(n===5?'Start Battle \u00b7 Enter':'Next \u00b7 Enter')),'title/count/action page '+(n+1));
   const bounds=s.tutorialModal,clip=s.tutorialClipRect;
   check(Math.abs(bounds.width-width*816/960)<2&&Math.abs(bounds.height-height*480/540)<2&&clip.y>=bounds.y&&clip.y+clip.height<=bounds.y+bounds.height&&text.every(t=>!t.truncated&&t.glyphBounds.x>=t.bounds.x-1&&t.glyphBounds.y>=t.bounds.y-1&&t.glyphBounds.x+t.glyphBounds.width<=t.bounds.x+t.bounds.width+1&&t.glyphBounds.y+t.glyphBounds.height<=t.bounds.y+t.bounds.height+1),'modal layout page '+(n+1)+' at '+width);
   await page.keyboard.type('WRONG',{delay:20});await key('Space');s=await state(page);
   check(s.inputLetters===0&&s.combatSeconds===0&&s.score===0&&s.timeScale===0,'input/score lock page '+(n+1));await shot(`tutorial-${n+1}-${width}`);
   await key('Escape');s=await until(page,s=>s.paused);const held=s.tutorialClipTime,frame=s.tutorialClipFrame;
   await page.waitForTimeout(250);s=await state(page);check(s.tutorialClipTime===held&&s.tutorialClipFrame===frame,'Pause holds page '+(n+1));
   await clickControl(page,'ResumeButton','button');await until(page,s=>at(n)(s)&&s.tutorialClipTime>held);
  }
  for(let n=4;n>=0;n--){await clickControl(page,'BackButton','button');s=await until(page,at(n));check(n===0||s.tutorialClipTime<.6,'Back resets page '+(n+1));}
 }
 await page.keyboard.down('Enter');
 try{await until(page,at(1));await page.waitForTimeout(650);check(at(1)(await state(page)),'held Enter cannot skip a page');}
 finally{await page.keyboard.up('Enter');await page.waitForTimeout(100);}
 for(let n=2;n<=5;n++){await key('Enter');await until(page,at(n));}
 await key('Escape');await until(page,s=>s.paused);await key('r');s=await until(page,at(0));check(s.tutorialStatus===status,'Restart resets to unread HUD');
 await menu();check((await state(page)).tutorialStatus===status,'abandon leaves viewed unchanged');await launch(true);await until(page,at(0));
 for(let n=1;n<=5;n++){await key('Enter');await until(page,at(n));}
 await key('Enter');s=await until(page,s=>!s.tutorial&&!s.dungeonTransitioning);
 check(s.tutorialStatus===2&&s.score===0&&s.inputLetters===0&&s.hp===40&&s.enemyHp===180&&s.stamina===100,'only final Start marks viewed and loads fresh battle');
 check(!s.typography.some(t=>/SpaceExample|ParryModal|HealthGuide|EnemyGuide/.test(t.path)),'encounter clone has no tutorial');
 await page.waitForFunction(()=>{const f=window.typingHeroCombatFrame;return f?.spaceCueVisible&&f.spaceCueFilled&&f.canParry&&f.cueImpactIn>.12;},null,{polling:'raf',timeout:12000});
 const before=await page.evaluate(()=>window.typingHeroCombatFrame.parries);
 await page.keyboard.down('Space');try{await page.waitForFunction(()=>window.typingHeroCombatFrame?.parryWindow,null,{polling:'raf',timeout:700});}finally{await page.keyboard.up('Space');}
 await page.waitForFunction(n=>window.typingHeroCombatFrame.parries>n,before,{polling:'raf',timeout:2500});check(true,'real parry after final Start');await shot('tutorial-live-parry');
 await menu();await page.reload();await page.waitForFunction(()=>window.typingHeroQA?.frame>0,null,{timeout:120000});check((await state(page)).tutorialStatus===2,'viewed persists across reload');
 s=await launch(false);check(!s.tutorial,'viewed Play bypasses guide');await menu();await launch(true);await until(page,at(0));
 for(let n=1;n<=5;n++){await clickControl(page,n===1?'PracticeButton':action,'button');await until(page,at(n));}
 await clickControl(page,action,'button');await until(page,s=>!s.tutorial&&!s.dungeonTransitioning);check(report.fixtureWrites===writes,'tutorial writes no external scores');await menu();
};
