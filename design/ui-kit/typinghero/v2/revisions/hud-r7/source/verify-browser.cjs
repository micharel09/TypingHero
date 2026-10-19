const {chromium}=require('E:/codex-data/packages/rea/node_modules/playwright-core');
const assert=require('node:assert/strict'),path=require('node:path'),{pathToFileURL}=require('node:url');
(async()=>{const b=await chromium.launch({headless:true,args:['--allow-file-access-from-files'],executablePath:'C:/Program Files/Google/Chrome/Application/chrome.exe'});try{
 const p=await b.newPage({viewport:{width:1500,height:1000}}),errors=[];p.on('pageerror',e=>errors.push(e.message));p.on('requestfailed',r=>errors.push(r.url()));
 await p.goto(pathToFileURL(path.resolve(__dirname,'../index.html')).href);await p.waitForFunction(()=>document.body.dataset.ready==='true');
 const state=()=>p.locator('#r7 canvas').evaluate(e=>({...e.dataset})),input=s=>p.locator('#entry').fill(s),reset=()=>p.locator('#reset').click();
 for(const prefix of ['','res','resolv']){await reset();const before=(await state()).positions;await input(prefix);await p.locator('#wrong').click();assert.equal((await state()).positions,before);assert.equal((await state()).wrong,String(prefix.length));assert.equal((await state()).next,'honor');await input('r');assert.equal((await state()).error,'false');}
 for(const delay of [150,100,60]){await reset();await p.locator('#entry').pressSequentially('resolvehonorcourageguardstrike',{delay});assert.equal((await state()).completed,'5');}
 await reset();await input('RESOLVE honor!courage12guard strike');assert.equal((await state()).completed,'5');assert.equal((await state()).offset,'0');
 const samples=await p.evaluate(async()=>{document.querySelector('#reset').click();const send=s=>{const e=document.querySelector('#entry');e.value=s;e.dispatchEvent(new Event('input'));};send('resolve');let out=[{...document.querySelector('#r7 canvas').dataset}];await new Promise(requestAnimationFrame);send('honor');for(let i=0;i<12;i++){await new Promise(requestAnimationFrame);out.push({...document.querySelector('#r7 canvas').dataset});}return out;});
 assert.equal(samples[0].offset,'66');assert.equal(samples.at(-1).completed,'2');assert.equal(samples.at(-1).current,'courage');assert.equal(samples.at(-1).offset,'0');assert(samples.some(x=>+x.offset>0&&+x.offset<66));
 const timing=await p.evaluate(async()=>{document.querySelector('#wrong').click();await new Promise(r=>setTimeout(r,60));document.querySelector('#wrong').click();const out=[];for(let i=0;i<20;i++){await new Promise(requestAnimationFrame);out.push({...document.querySelector('#r7 canvas').dataset});}return out;});assert(timing.some(x=>x.error==='true'));const clear=timing.find(x=>x.error==='false');assert(clear);assert(+clear.paintedAt-Number(clear.started)>=180);
 for(const fixture of ['pool','long','short','mixed','stress']){await p.locator('#fixture').selectOption(fixture);const words=await p.evaluate(()=>words());await input(words.join(''));assert.equal((await state()).completed,String(words.length));assert.equal((await state()).current,words[0]);}
 await p.locator('#fixture').selectOption('normal');await p.locator('#reduced').check();await input('resolve');assert.equal((await state()).offset,'0');await p.emulateMedia({reducedMotion:'reduce'});await p.locator('#reduced').uncheck();await reset();await input('resolve');assert.equal((await state()).offset,'0');await p.emulateMedia({reducedMotion:'no-preference'});
 // Compare actual decoded canvas pixels, outside typing ink, in both modes.
 for(const slayer of [false,true])for(const scale of ['1','2','4'])for(const gray of [false,true]){
  await p.locator('#slayer').setChecked(slayer);await p.locator('#scale').selectOption(scale);await p.locator('#gray').setChecked(gray);await reset();
  assert(await p.evaluate(()=>{const a=document.querySelector('#r6 canvas').getContext('2d').getImageData(0,0,320,180).data,b=document.querySelector('#r7 canvas').getContext('2d').getImageData(0,0,320,180).data;for(let y=0;y<180;y++)for(let x=0;x<320;x++)if(!(x>=65&&x<255&&y>=151&&y<171))for(let c=0;c<4;c++)if(a[(y*320+x)*4+c]!==b[(y*320+x)*4+c])return false;return true;}));
  assert.equal(await p.locator('#r7 canvas').evaluate(e=>getComputedStyle(e).width),`${320*+scale}px`);
 }
 await p.locator('#scale').selectOption('2');await p.locator('#gray').uncheck();await p.locator('#slayer').uncheck();await p.screenshot({path:'E:/CodexTemp/TypingHero/hud-r7/review.png',fullPage:true});
 await p.locator('#order').click();assert.equal(await p.locator('main figure').first().getAttribute('id'),'r7');assert.deepEqual(errors,[]);
 console.log('PASS Chrome: pool219, all fixtures, stable positions, speeds150/100/60, burst/overlap, slide66/100ms, errors180ms, reduced motion, 12 mode/scale/grayscale cases, pixel isolation, order/replay; no page/request errors');
}finally{await b.close();}})().catch(e=>{console.error(e);process.exitCode=1});
