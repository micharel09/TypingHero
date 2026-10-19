const { chromium } = require(process.argv[2]);
const {pathToFileURL} = require('node:url');
const path = require('node:path');
const fs = require('node:fs');
const assert = require('node:assert/strict');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:process.argv[3]});
 try {
 const page=await browser.newPage({viewport:{width:1280,height:900}});
 const errors=[];page.on('pageerror',e=>errors.push(e.message));
 await page.route('https://i.pinimg.com/**',r=>r.abort());
 await page.goto(pathToFileURL(path.resolve(__dirname,'../index.html')).href);
 const images=await page.evaluate(async()=>{const all=[...document.images].filter(i=>i.src.startsWith('file:'));await Promise.all(all.map(i=>i.decode()));return all.map(i=>({width:i.naturalWidth,filter:getComputedStyle(i).imageRendering}));});
 assert.ok(images.every(i=>i.width>0&&i.filter==='pixelated'));
 assert.equal(await page.locator('article').count(),7);
 const state=()=>page.locator('#typing').evaluate(e=>({...e.dataset,transform:e.style.transform}));
 await page.locator('#entry').fill('RES');assert.equal((await state()).progress,'3');
 await page.locator('#wrong').click();assert.equal((await state()).progress,'0');
 // Real RAF sampling, including a repeated error during active feedback.
 const capture=await page.evaluate(async()=>{
  const el=document.querySelector('#typing'), samples=[];
  document.querySelector('#wrong').click();const start=performance.now();let repeated=false, restarted=0;
  while(performance.now()-start<350){await new Promise(requestAnimationFrame);const t=performance.now()-start;
   if(t>60&&!repeated){document.querySelector('#wrong').click();repeated=true;restarted=t;}
   samples.push({t,offset:Number(el.dataset.offset),error:el.dataset.error});}
  return {samples,restarted};
 });
 const {samples,restarted}=capture;
 assert.ok(samples.some(s=>s.offset!==0));assert.ok(samples.every(s=>Math.abs(s.offset)<=1));
 assert.ok(samples.some(s=>s.t>restarted+120&&s.t<restarted+165&&s.error==='true')); // repeated error restarted 180ms timer
 assert.ok(samples.filter(s=>s.t>restarted+150).every(s=>s.offset===0));
 assert.equal((await state()).transform,'translateX(0px)');assert.equal((await state()).error,'false');
 if(process.argv[4]){
  fs.mkdirSync(process.argv[4],{recursive:true});
  await page.locator('#review').scrollIntoViewIfNeeded();
  const cdp=await page.context().newCDPSession(page);let index=0;
  cdp.on('Page.screencastFrame',async e=>{
   fs.writeFileSync(path.join(process.argv[4],`motion-${String(index++).padStart(3,'0')}.png`),Buffer.from(e.data,'base64'));
   await cdp.send('Page.screencastFrameAck',{sessionId:e.sessionId});
  });
  await cdp.send('Page.startScreencast',{format:'png',everyNthFrame:1});
  await page.locator('#wrong').click();await page.waitForTimeout(280);
  await cdp.send('Page.stopScreencast');await cdp.detach();
 }
 await page.locator('#reduced').check();await page.locator('#wrong').click();
 const reduced=await page.evaluate(async()=>{const out=[];for(let i=0;i<14;i++){await new Promise(requestAnimationFrame);out.push({...document.querySelector('#typing').dataset});}return out;});
 assert.ok(reduced.every(s=>s.offset==='0'));assert.ok(reduced.some(s=>s.error==='true'));
 await page.locator('#reset').click();await page.locator('#entry').fill('resolve');assert.equal((await state()).progress,'7');
 await page.locator('#reset').click();assert.equal((await state()).progress,'0');
 await page.emulateMedia({reducedMotion:'reduce'});assert.equal(await page.locator('#reduced').isChecked(),true);
 for(const width of [320,640,1280]){await page.setViewportSize({width,height:900});assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),`overflow ${width}`);}
 await page.setViewportSize({width:1280,height:900});
 if(process.argv[4]){fs.mkdirSync(process.argv[4],{recursive:true});await page.locator('#review').screenshot({path:path.join(process.argv[4],'preview.png')});}
 assert.deepEqual(errors,[]);
 console.log('PASS: seven-screen offline gallery; native preview typing/reset; real RAF ±1px and repeated-error restart; final origin; reduced motion; 320/640/1280 overflow checks.');
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
