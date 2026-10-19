// Durable pending/reload and tutorial regression. All points come from real input;
// all Firebase requests remain intercepted by the existing isolated fixture.
const path=require('node:path');
module.exports=async function difficultyNetwork(api,browser){
  const {session,start,state,until,clickControl,check,report}=api;
  const {context,page}=await session(browser,{width:960,height:540},1000);
  const key=async name=>{
    const before=await state(page),frame=before.frame+Math.ceil(before.qaAgeMs/Math.max(1,before.frameMs))+2;
    await page.keyboard.down(name);
    try {await page.waitForTimeout(Math.max(40,before.frameMs*2));await until(page,s=>s.frame>=frame);}
    finally {await page.keyboard.up(name);}
    await page.waitForTimeout(40);
  };
  const capture=name=>page.screenshot({path:path.join(report.outputDirectory,name+'.png')});
  let offline=true,attempts=0,reads=0,pending=null,written=null;
  const endpoint='**/leaderboards/endless_v2/hard/*.json*';
  const headers={'access-control-allow-origin':'*','access-control-allow-methods':'GET, PUT, OPTIONS',
    'access-control-allow-headers':'*','access-control-expose-headers':'ETag',ETag:'"durable-fixture"'};
  try{
    await until(page,s=>s.scene==='MainMenu');
    await clickControl(page,'NameInput','input');await key('Tab');await key('ArrowRight');
    await until(page,s=>s.menuDifficulty==='hard');
    await context.route(endpoint,route=>{
      const req=route.request();
      if(req.method()==='OPTIONS') return route.fulfill({status:204,headers});
      if(req.method()==='GET'){reads++;return route.fulfill({status:200,headers,contentType:'application/json',body:'null'});}
      check(req.method()==='PUT','pending uses conditional best PUT');
      check(req.headers()['if-match']==='"durable-fixture"','pending retry retains the ETag contract');
      attempts++;const body=JSON.parse(req.postData());
      if(offline) pending=body;else written={path:new URL(req.url()).pathname,body};
      return route.fulfill({status:offline?503:200,headers,contentType:'application/json',body:offline?'{}':req.postData()});
    });
    await start(page);let s=await until(page,s=>s.runDifficulty==='hard');
    check(s.runDifficulty==='hard','pending run snapshots Hard');
    await key('Enter');await until(page,s=>!s.encounterReading);
    const word=(await state(page)).current;
    await page.keyboard.type(word,{delay:140});await until(page,s=>s.score>0);
    await until(page,s=>s.overlay && s.hp<=0,60000);await until(page,()=>pending!==null);
    const original={...pending};check(original.score>0,'pending score was earned by actual typing');
    await key('m');await until(page,s=>s.scene==='MainMenu');
    await clickControl(page,'NameInput','input');await key('Tab');await key('ArrowRight');await key('ArrowRight');
    await until(page,s=>s.menuDifficulty==='very_easy');
    const beforeReload=attempts;await page.waitForTimeout(500);await page.reload();
    await page.waitForFunction(()=>window.typingHeroQA?.frame>0,null,{timeout:120000});
    s=await until(page,s=>s.scene==='MainMenu');
    check(s.menuDifficulty==='very_easy','menu difficulty persists through reload');
    check(attempts===beforeReload,'menu change and reload do not resubmit a different difficulty');
    offline=false;
    await clickControl(page,'Top 1000Button','button');
    await until(page,()=>written!==null);
    s=await until(page,s=>s.leaderboardOpen && s.dataRows===1000);
    check(s.tableDifficulty==='very_easy' && written.path.startsWith('/leaderboards/endless_v2/hard/'),
      'opening a Very Easy table retries the saved Hard partition');
    check(JSON.stringify(written.body)===JSON.stringify(original),'pending name/progress/score/time survive reload unchanged');
    await capture('pending-retried-from-other-table');
    const afterRetry=reads;
    await key('Escape');await until(page,s=>!s.leaderboardOpen);
    await clickControl(page,'Top 1000Button','button');await until(page,s=>s.leaderboardOpen && s.dataRows===1000);
    check(reads===afterRetry,'successful pending retry is cleared durably');
    await key('Escape');await until(page,s=>!s.leaderboardOpen);
    // Replay the actual six-lesson tutorial with a non-Normal menu selection.
    await clickControl(page,'TutorialButton','button');
    s=await until(page,s=>s.tutorial && !s.tutorialPracticing);
    check(Math.abs(s.comboWindowSeconds-1.5)<.0001,'Very Easy does not scale the tutorial combo timer');
    await require('./tutorial.cjs')(api,page,{freshTutorial:true,viewports:[[960,540],[640,360]]});
    check(report.fixtureWrites===0,'pending and tutorial never write the Classic table');
    report.pendingReload={attempts,reads,path:written.path,entry:written.body};
  }finally{await context.close();}
};
