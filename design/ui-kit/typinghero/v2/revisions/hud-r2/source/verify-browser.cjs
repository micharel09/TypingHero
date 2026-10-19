const {chromium}=require(process.argv[2]||'E:/codex-data/packages/rea/node_modules/playwright-core');
const {pathToFileURL}=require('node:url'),path=require('node:path'),fs=require('node:fs'),assert=require('node:assert/strict');
const temp=process.argv[4];
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:process.argv[3]||'C:/Program Files/Google/Chrome/Application/chrome.exe'});
 try{
  const page=await browser.newPage({viewport:{width:1280,height:900}}),errors=[];
  page.on('pageerror',e=>errors.push(e.message));
  await page.route('http://**',r=>r.abort());await page.route('https://**',r=>r.abort());
  await page.goto(pathToFileURL(path.resolve(__dirname,'../index.html')).href);
  await page.evaluate(()=>Promise.all([...document.images].map(i=>i.decode())));
  assert.equal(await page.locator('article').count(),10);
  const state=()=>page.locator('#typing').evaluate(e=>({...e.dataset,transform:e.style.transform}));
  for(const rank of ['normal','elite','boss'])for(const slayer of [false,true])for(const hp of ['0','1','50','100']){
   await page.locator('#rank').selectOption(rank);await page.locator('#slayer').setChecked(slayer);await page.locator('#hp').selectOption(hp);
   await page.locator('#hud').evaluate(i=>i.decode());
   assert.ok((await page.locator('#hud').getAttribute('src')).endsWith(`hud-${rank}-${+slayer}-0-${hp}.png`));
  }
  for(const prefix of ['', 'res', 'resolv']){
   await page.locator('#reset').click();if(prefix)await page.locator('#entry').fill(prefix);
   await page.locator('#wrong').click();assert.equal((await state()).progress,'0');assert.equal((await state()).wrong,String(prefix.length));
  }
  await page.locator('#reset').click();await page.locator('#entry').fill('RESOLVE');assert.equal((await state()).progress,'7');
  await page.locator('#correct').click();assert.equal((await state()).progress,'1');
  await page.locator('#stress').check();await page.locator('#entry').fill('counterrevolutionary');assert.equal((await state()).progress,'20');
  await page.locator('#stress').uncheck();
  // RAF timestamps observe actual painted DOM state, rather than endpoint-only timers.
  const timing=await page.evaluate(async()=>{
   const el=document.querySelector('#typing'),samples=[];document.querySelector('#wrong').click();let restarted=false,start=performance.now();
   while(performance.now()-start<400){await new Promise(requestAnimationFrame);if(!restarted&&performance.now()-start>55){document.querySelector('#wrong').click();restarted=true;}
    samples.push({t:Number(el.dataset.paintedAt),sampledAt:performance.now(),started:Number(el.dataset.started),error:el.dataset.error,offset:Number(el.dataset.offset)});}
   return samples;
  });
  assert.ok(timing.some(s=>s.offset!==0));assert.ok(timing.every(s=>Math.abs(s.offset)<=1));
  const latest=timing.at(-1).started,after=timing.filter(s=>s.started===latest);
  const end=after.findIndex(s=>s.error==='false');assert.ok(end>0);
  const firstClear=after[end],lastRed=after[end-1],frame=firstClear.t-lastRed.t;
  assert.ok(firstClear.t-latest>=180&&firstClear.t-latest<=180+frame+3,JSON.stringify({firstClear,latest,frame}));
  assert.ok(after.filter(s=>s.t-latest>=120+frame).every(s=>s.offset===0));
  assert.equal((await state()).transform,'translateX(0px)');
  await page.locator('#camera').click();
  const motion=await page.evaluate(async()=>{
   const out=[],start=performance.now();document.querySelector('#wrong').click();
   while(performance.now()-start<4200){await new Promise(requestAnimationFrame);const ids=['far','mid','floor','foreground','hero','enemy'];out.push({t:performance.now()-start,offsets:ids.map(id=>Number(document.getElementById(id).dataset.offset)),hud:document.getElementById('hud').getBoundingClientRect().x,typing:document.getElementById('typing').style.transform});}
   return out;
  });
  for(const [i,amplitude] of [1,2,4,4,2,2].entries()){
   assert.ok(motion.every(s=>Number.isInteger(s.offsets[i])&&Math.abs(s.offsets[i])<=amplitude));
   assert.ok(motion.some(s=>s.offsets[i]===amplitude)&&motion.some(s=>s.offsets[i]===-amplitude));
  }
  assert.equal(new Set(motion.map(s=>s.hud)).size,1);
  assert.ok(motion.some(s=>s.typing!=='translateX(0px)'));
  await page.locator('#camera').click();
  const frozen=await page.locator('#far').getAttribute('style');await page.locator('#correct').click();assert.equal(await page.locator('#far').getAttribute('style'),frozen);
  await page.locator('#reduced').check();await page.locator('#wrong').click();
  assert.equal((await state()).offset,'0');assert.equal((await state()).error,'true');assert.ok(await page.locator('#camera').isDisabled());
  await page.locator('#reduced').uncheck();await page.emulateMedia({reducedMotion:'reduce'});
  assert.ok(await page.locator('#camera').isDisabled());await page.locator('#wrong').click();assert.equal((await state()).offset,'0');
  await page.emulateMedia({reducedMotion:'no-preference'});
  await page.locator('#gray').check();assert.equal(await page.locator('#stage').evaluate(e=>getComputedStyle(e).filter),'grayscale(1)');
  for(const width of [320,640,1280]){
   await page.setViewportSize({width,height:900});
   for(const scale of ['1','2','4']){await page.locator('#scale').selectOption(scale);assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),`overflow ${width}/${scale}`);assert.ok(await page.evaluate(()=>document.querySelector('#stage').getBoundingClientRect().right<=document.querySelector('#viewport').getBoundingClientRect().right+1),'clipped HUD');}
  }
  if(temp){
   fs.mkdirSync(temp,{recursive:true});await page.setViewportSize({width:1280,height:900});await page.locator('#scale').selectOption('1');
   await page.locator('#gray').uncheck();await page.locator('#reset').click();await page.locator('#review').screenshot({path:path.join(temp,'review.png')});
   const cdp=await page.context().newCDPSession(page);let index=0;
   cdp.on('Page.screencastFrame',async e=>{fs.writeFileSync(path.join(temp,`motion-${String(index++).padStart(3,'0')}.png`),Buffer.from(e.data,'base64'));await cdp.send('Page.screencastFrameAck',{sessionId:e.sessionId});});
   await cdp.send('Page.startScreencast',{format:'png',everyNthFrame:1});await page.locator('#camera').click();
   await page.locator('#wrong').click();await page.waitForTimeout(2200);await page.locator('#wrong').click();await page.waitForTimeout(2200);
   await cdp.send('Page.stopScreencast');await cdp.detach();
   fs.writeFileSync(path.join(temp,'timing.json'),JSON.stringify({timing,motion},null,2));
  }
  assert.deepEqual(errors,[]);
  console.log(`PASS: offline 10-screen gallery; 24 rank/Slayer/HP combinations; typing start/middle/end/repeat/complete/reset; red 180ms (observed ${Math.round(firstClear.t-latest)}ms, frame ${Math.round(frame)}ms); shake ±1/120ms; 4s integer camera amplitudes; HUD fixed; reduced motion checkbox/system; grayscale; 320/640/1280 layouts.`);
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
