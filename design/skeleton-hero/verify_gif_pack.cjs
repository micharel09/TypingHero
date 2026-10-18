// NODE_PATH points to the existing Playwright installation.
const {chromium}=require('playwright');
const assert=require('node:assert/strict');
const {pathToFileURL}=require('node:url');
const path=require('node:path');
(async()=>{
 const browser=await chromium.launch({channel:'msedge',headless:true});
 try{
  const page=await browser.newPage();const errors=[];
  page.on('pageerror',e=>errors.push(e.message));
  await page.goto(pathToFileURL(path.join(__dirname,'gif-pack/review.html')).href);
  await page.waitForFunction(()=>ready);
  let combinations=0;
  for(const size of [{width:1280,height:720},{width:640,height:360}]){
   await page.setViewportSize(size);
   for(const kind of ['sword','axe','sword-shield'])for(const clip of ['Timeline','Idle','Hurt','Walk','Die','Attack1','Attack2'])for(const zoom of ['4','8']){
    await page.selectOption('#kind',kind);await page.selectOption('#clip',clip);await page.selectOption('#zoom',zoom);
    for(const mirror of [true,false]){
     await page.locator('#mirror').setChecked(mirror);
     await page.locator('#scrub').fill(await page.locator('#scrub').getAttribute('max'));
     const result=await page.evaluate(()=>({frame,range:range(),playing,w:$('export').width,h:$('export').height,smoothing:$('export').getContext('2d').imageSmoothingEnabled}));
     assert.equal(result.frame,result.range[1]);assert.equal(result.playing,false);
     assert.equal(result.w,96*Number(zoom));assert.equal(result.h,64*Number(zoom));assert.equal(result.smoothing,false);
     await page.click('#prev');assert.equal(await page.evaluate(()=>frame),result.range[1]-1);
     await page.click('#next');assert.equal(await page.evaluate(()=>frame),result.range[1]);
     combinations++;
    }
   }
  }
  await page.selectOption('#clip','Timeline');await page.selectOption('#kind','axe');
  for(const speed of ['1','0.25']){
   await page.selectOption('#speed',speed);await page.click('#replay');
   await page.waitForFunction(()=>frame>=2);
   await page.click('#play');const paused=await page.evaluate(()=>frame);
   await page.evaluate(()=>new Promise(resolve=>{let n=0;function tick(){if(++n===10)resolve();else requestAnimationFrame(tick);}requestAnimationFrame(tick);}));
   assert.equal(await page.evaluate(()=>frame),paused);
   await page.click('#play');await page.waitForFunction(p=>frame!==p,paused);
  }
  await page.setViewportSize({width:1280,height:720});await page.selectOption('#zoom','4');
  await page.locator('#scrub').fill('52');
  await page.screenshot({path:'E:/CodexTemp/TypingHero/skeleton-hero-art/gif-pack-qa/preview.png',fullPage:true});
  assert.deepEqual(errors,[]);
  console.log(`PASS: ${combinations} viewport/character/clip/zoom/mirror combinations; scrub, step, replay, pause/resume, both speeds; no page errors`);
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
