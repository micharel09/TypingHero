// Real controls and browser focus only; the Unity probe remains read-only.
const path=require('node:path');
module.exports=async function difficultyUI(api,browser){
  const {session,start,state,until,clickControl,check,report}=api;
  const {context,page}=await session(browser,{width:960,height:540},1000);
  const ids=['very_easy','easy','normal','hard','very_hard'];
  const key=async name=>{
    const s=await state(page),frame=s.frame+Math.ceil(s.qaAgeMs/Math.max(1,s.frameMs))+2;
    await page.keyboard.down(name);
    try{await page.waitForTimeout(Math.max(40,s.frameMs*2));await until(page,t=>t.frame>=frame);}
    finally{await page.keyboard.up(name);}
    await page.waitForTimeout(40);
  };
  const focus=async reading=>{
    const cdp=await context.newCDPSession(page);
    await cdp.send('Emulation.setFocusEmulationEnabled',{enabled:false});
    const before=await state(page),away=await context.newPage();
    try{
      await away.goto('about:blank');await away.bringToFront();
      const actual=await page.evaluate(()=>({focused:document.hasFocus(),hidden:document.hidden}));
      check(!actual.focused||actual.hidden,'Endless loses actual browser tab focus');
      await page.waitForTimeout(700);
    }finally{await away.close();await page.bringToFront();}
    const after=await until(page,s=>s.frame>before.frame&&s.qaAgeMs<500&&s.paused&&s.timeScale===0);
    check(after.hp===before.hp&&after.inputLetters===before.inputLetters&&Math.abs(after.wordTime-before.wordTime)<.25,
      'focus loss holds Endless health, input and word timer');
    await key('Escape');await until(page,s=>!s.paused&&s.timeScale===(reading?0:1));
    await cdp.detach();
  };
  try{
    await until(page,s=>s.scene==='MainMenu');
    await clickControl(page,'NameInput','input');await page.keyboard.press('Control+a',{delay:80});
    await page.keyboard.type('LoopQA',{delay:60});
    await until(page,s=>s.controls.some(c=>c.name==='PlayButton'));
    for(let i=0;i<5;i++){
      const before=await state(page),next=ids[(ids.indexOf(before.menuDifficulty)+1)%5];
      await clickControl(page,'>Button','button');await until(page,s=>s.menuDifficulty===next);
      await key('ArrowLeft');await until(page,s=>s.menuDifficulty===before.menuDifficulty);
      await key('ArrowRight');await until(page,s=>s.menuDifficulty===next);
      await key('Enter');await until(page,s=>s.menuDifficulty===ids[(ids.indexOf(next)+1)%5]);
      await key('ArrowLeft');await until(page,s=>s.menuDifficulty===next);
      check((await state(page)).scene==='MainMenu','mouse and focused selector arrows never launch Play');
      for(const [width,height] of [[960,540],[640,360]]){
        await page.setViewportSize({width,height});await until(page,s=>s.width===width);
        await page.screenshot({path:path.join(report.outputDirectory,`menu-${next}-${width}.png`)});
      }
    }
    await page.keyboard.down('Shift');await key('Tab');await page.keyboard.up('Shift');
    await until(page,s=>s.selectedControl==='NameInput');
    await key('Tab');await until(page,s=>s.selectedControl==='NormalButton');
    await key('Tab');await until(page,s=>s.selectedControl==='PlayButton');
    await page.setViewportSize({width:960,height:540});await until(page,s=>s.width===960);
    await start(page);await until(page,s=>s.encounterReading);
    await focus(true);await key('Enter');await until(page,s=>!s.encounterReading);await focus(false);
    await key('Escape');await until(page,s=>s.paused);await key('m');await until(page,s=>s.scene==='MainMenu');
    const old=(await state(page)).menuDifficulty;
    await clickControl(page,'>Button','button');const changed=await until(page,s=>s.menuDifficulty!==old);
    await start(page);const run=await until(page,s=>s.encounterReading&&s.runDifficulty===changed.menuDifficulty);
    check(run.runDifficulty!==old,'return to menu permits a new difficulty snapshot');
    check(report.fixtureWrites===0&&(report.endlessWrites||0)===0,'selector/focus checks submit no scores');
    // Exercise the exact death -> table -> Restart path in every partition.
    for(const difficulty of ids){
      await key('Escape');await until(page,s=>s.paused);await key('m');await until(page,s=>s.scene==='MainMenu');
      for(let step=0;(await state(page)).menuDifficulty!==difficulty;step++){
        check(step<5,'mouse selector reaches the requested death/restart level');
        await clickControl(page,'>Button','button');await page.waitForTimeout(150);
      }
      await start(page);await until(page,s=>s.encounterReading&&s.runDifficulty===difficulty);
      await key('Enter');await until(page,s=>!s.encounterReading);
      await until(page,s=>s.overlay&&s.hp<=0,80000);
      await clickControl(page,'Top 1000Button','button');
      const table=await until(page,s=>s.leaderboardOpen&&s.dataRows===1000);
      check(table.tableDifficulty===difficulty&&table.collectionFirstRow.startsWith(difficulty+' '),
        'death table opens the defeated run partition');
      await key('Escape');await until(page,s=>!s.leaderboardOpen&&s.overlay);
      await key('r');
      await until(page,s=>s.encounterReading&&s.hp===40&&s.score===0&&s.defeated===0&&s.runDifficulty===difficulty);
    }
    check(report.fixtureWrites===0,'death-table-restart never writes Classic');
  }finally{await context.close();}
};
