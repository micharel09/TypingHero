// Use the existing Playwright installation via NODE_PATH; Edge headless, file://.
const {chromium}=require('playwright');
const assert=require('node:assert/strict');
const {pathToFileURL}=require('node:url');
const path=require('node:path');
const fs=require('node:fs');

(async()=>{
 const browser=await chromium.launch({channel:'msedge',headless:true});
 try{
  const page=await browser.newPage({viewport:{width:1400,height:1100},deviceScaleFactor:1});
  const errors=[];page.on('pageerror',e=>errors.push(e.message));
  page.on('console',m=>{if(m.type()==='error')errors.push(m.text());});
  await page.goto(pathToFileURL(path.join(__dirname,'full-pack/review.html')).href);
  await page.waitForFunction(()=>ready);
  const matrix=await page.evaluate(()=>{
   let checks=0,combinations=0;const check=(ok,msg)=>{if(!ok)throw new Error(msg);checks++;};
   const change=(id,value)=>{$(id).value=value;$(id).dispatchEvent(new Event('change'));};
   const set=(id,value)=>{$(id).checked=value;$(id).dispatchEvent(new Event('change'));};
   check(Object.keys(images).length===36,'36 animation/color/weapon selections');
   check(Object.values(images).every(v=>Object.keys(v).length===2),'72 images loaded');
   const raf=window.requestAnimationFrame;window.requestAnimationFrame=()=>0;
   try{
    change('zoom','4');
    for(const kind of ['sword','axe','spear'])for(const color of ['White','Yellow'])for(const clip of ['Idle','Walk','Attack1','Attack2','Hurt','Die']){
     change('weapon',kind);change('color',color);change('clip',clip);
     const n=kind==='axe'&&clip==='Attack1'?16:{Idle:8,Walk:10,Attack1:10,Attack2:9,Hurt:5,Die:13}[clip];
     check(info().count===n,key()+' cel count');
     for(const mirror of [false,true])for(const vfx of [false,true])for(const speed of ['1','0.25']){
      combinations++;set('flip',mirror);set('vfx',vfx);change('speed',speed);
      stop();setTime(0);check(index()===0,key()+' reset');
      $('previous').click();check(index()===0,'previous clamped');
      for(let i=1;i<n;i++){$('next').click();check(index()===i,key()+' forward '+i);}
      $('next').click();check(index()===n-1,'next clamped');
      for(let i=n-2;i>=0;i--){$('previous').click();check(index()===i,key()+' backward '+i);}
      const mid=Math.floor(n/2);$('time').value=(starts()[mid]+info().durationsMs[mid]/2)/1000;
      $('time').dispatchEvent(new Event('input'));check(index()===mid&&!playing,'scrub pauses at correct cel');
      const before=t;$('play').click();check(playing&&t===before,'resume preserves time');
      tick(last+20/Number(speed));check(Math.abs(t-before-20)<.001,'speed applies to elapsed time');
      $('play').click();const paused=t;tick(last+100);check(!playing&&t===paused,'pause holds');
      $('replay').click();check(playing&&t===0,'replay starts at zero');
      tick(last+(total()+25)/Number(speed));
      check(info().loop?playing&&Math.abs(t-25)<.001:!playing&&t===total()&&index()===n-1,'loop or terminal hold');
      stop();setTime(total());$('play').click();check(playing&&t===0,'play from end replays');stop();
      for(const zoom of ['3.5','1.75','4','8']){
       change('zoom',zoom);const s=Number(zoom);
       const expected=s===3.5?[1280,720]:s===1.75?[640,360]:[96*s+120,64*s+90];
       check($('view').width===expected[0]&&$('view').height===expected[1],'canvas dimensions '+zoom);
       check(!$('view').getContext('2d').imageSmoothingEnabled,'nearest sampling');
      }
      change('zoom','4');set('anchors',true);set('onion',true);setTime(starts()[mid]);
      check($('status').textContent.includes('Cel '+mid+' / '+(n-1)),'status uses actual cel count');
      set('anchors',false);set('onion',false);
     }
     if(['Attack1','Attack2'].includes(clip)){
      const effect=clip==='Attack2'?4:kind==='axe'?4:5;setTime(starts()[effect]);
      set('vfx',true);const a=$('view').toDataURL();set('vfx',false);
      check(a!==$('view').toDataURL(),key()+' VFX toggle changes swing');
     }
    }
   }finally{window.requestAnimationFrame=raf;stop();}
   return {checks,combinations};
  });
  // Measure actual requestAnimationFrame playback, independently of matrix ticks.
  const playback=[];
  for(const speed of ['1','0.25']){
   await page.selectOption('#weapon','axe');await page.selectOption('#clip','Attack1');
   await page.selectOption('#speed',speed);await page.click('#replay');
   const start=await page.evaluate(()=>({t,now:performance.now()}));
   await page.waitForTimeout(520);
   const end=await page.evaluate(()=>({t,now:performance.now()}));
   const rate=(end.t-start.t)/(end.now-start.now);
   assert.ok(Math.abs(rate-Number(speed))<.06,'actual playback rate '+rate);
   await page.click('#play');const paused=await page.evaluate(()=>t);
   await page.waitForTimeout(100);assert.equal(await page.evaluate(()=>t),paused);
   await page.click('#play');await page.waitForTimeout(100);
   assert.ok(await page.evaluate(p=>t>p,paused),'actual resume advances');
   await page.evaluate(()=>stop());playback.push({speed:Number(speed),measured:rate});
  }
  await page.selectOption('#speed','1');await page.selectOption('#color','White');
  await page.evaluate(()=>{setTime(total()-20);play();});
  await page.waitForFunction(()=>!playing&&t===total());
  assert.equal(await page.evaluate(()=>index()),15,'actual axe hold on Idle return');
  const captures=process.argv[2];
  if(captures){
   fs.mkdirSync(captures,{recursive:true});
   for(const zoom of ['3.5','1.75','4','8'])for(const mirror of [false,true]){
    await page.selectOption('#zoom',zoom);await page.locator('#flip').setChecked(mirror);
    await page.locator('#vfx').uncheck();
    await page.evaluate(()=>setTime(starts()[5]));
    await page.locator('#view').screenshot({path:path.join(captures,`axe-contact-${zoom}-${mirror}.png`)});
   }
  }
  assert.deepEqual(errors,[],'no browser errors');
  console.log(JSON.stringify({...matrix,playback,errors,dimensions:'1280x720,640x360,nearest 4x/8x'}));
 }finally{await browser.close();}
})().catch(error=>{console.error(error);process.exitCode=1;});
