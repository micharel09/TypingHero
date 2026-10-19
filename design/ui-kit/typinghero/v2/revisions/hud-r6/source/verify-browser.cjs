// node verify-browser.cjs <playwright-core> <chrome>
const {chromium}=require(process.argv[2]);
const {pathToFileURL}=require('node:url');
const path=require('node:path');
const assert=require('node:assert/strict');
(async()=>{
 const browser=await chromium.launch({headless:true,executablePath:process.argv[3]});
 try{
  const page=await browser.newPage();const errors=[];
  page.on('pageerror',e=>errors.push(e.message));page.on('requestfailed',r=>errors.push(r.url()));
  await page.goto(pathToFileURL(path.resolve(__dirname,'../index.html')).href);
  const states=await page.locator('#state option').allTextContents();
  for(const state of states)for(const scale of [1,2,4])for(const gray of [false,true]){
   await page.setViewportSize({width:320*scale,height:180*scale});
   await page.locator('#state').selectOption(state);await page.locator('#scale').selectOption(String(scale));
   await page.locator('#gray').setChecked(gray);
   const images=await page.locator('img').evaluateAll(async imgs=>{
    await Promise.all(imgs.map(i=>i.decode()));
    return imgs.map(i=>({w:i.width,h:i.height,nw:i.naturalWidth,nh:i.naturalHeight,filter:i.style.filter,render:getComputedStyle(i).imageRendering,src:i.src}));
   });
   for(const im of images){assert.equal(im.w,320*scale);assert.equal(im.h,180*scale);assert.equal(im.nw,im.w);assert.equal(im.nh,im.h);assert.equal(im.filter,gray?'grayscale(1)':'');assert.equal(im.render,'pixelated');assert(im.src.includes(state));}
  }
  assert.deepEqual(errors,[]);
  console.log(`PASS Chrome: ${states.length*6} state/scale/grayscale combinations; controls, dimensions, PNG decode, nearest-neighbor CSS, no page/request errors`);
 }finally{await browser.close()}
})().catch(e=>{console.error(e);process.exitCode=1});
