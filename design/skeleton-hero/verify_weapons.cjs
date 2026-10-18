// Run with the existing Playwright module available via NODE_PATH.
// Optional screenshot folder argument; no server or network is required.
const {chromium}=require('playwright');
const assert=require('node:assert/strict');
const {pathToFileURL}=require('node:url');
const path=require('node:path');
const fs=require('node:fs');

(async()=>{
 const browser=await chromium.launch({channel:'msedge',headless:true});
 try{
  const page=await browser.newPage({viewport:{width:1400,height:1000},deviceScaleFactor:1});
  const errors=[];
  page.on('pageerror',e=>errors.push(e.message));
  page.on('console',m=>{if(m.type()==='error')errors.push(m.text());});
  await page.goto(pathToFileURL(path.join(__dirname,'weapon-milestone/review.html')).href);
  await page.waitForFunction(()=>ready);
  const state=()=>page.evaluate(()=>({t,cel:index(),frame:frameAt(t),playing,
   size:[$('view').width,$('view').height],smoothing:$('view').getContext('2d').imageSmoothingEnabled,
   image:$('view').toDataURL()}));
  let checks=0;
  const check=(condition,message)=>{assert.ok(condition,message);checks++;};
  let current=await state();check(current.cel===0,'Initial Idle0');
  check((await page.evaluate(()=>Object.values(images).map(a=>a.length).join(',')))==='17,17,11,11,11,1','All reference/new images loaded');
  await page.locator('#telegraph').click();current=await state();check(current.frame===3&&current.t===.7,'Telegraph at original time');
  await page.locator('#impact').click();current=await state();check(current.frame===5&&current.t===1,'Impact at original time');
  const vfx=current.image;
  await page.locator('#vfx').uncheck();current=await state();check(current.image!==vfx,'VFX changes impact render');
  await page.locator('#flip').check();check((await state()).image!==current.image,'Mirror changes render');
  await page.locator('#flip').uncheck();
  await page.locator('#onion').check();check((await state()).image!==current.image,'Onion shows preceding cel');
  await page.locator('#onion').uncheck();
  await page.locator('#previous').click();check((await state()).frame===4,'Previous key');
  await page.locator('#next').click();check((await state()).frame===5,'Next key');
  await page.locator('#time').evaluate(el=>{el.value='.4167';el.dispatchEvent(new Event('input',{bubbles:true}));});
  check((await state()).frame===3,'Scrub updates frame');
  const withoutAnchors=(await state()).image;
  await page.locator('#anchors').check();check((await state()).image!==withoutAnchors,'Anchor rendering');
  await page.locator('#anchors').uncheck();
  const folder=process.argv[2];if(folder)fs.mkdirSync(folder,{recursive:true});
  for(const [zoom,size] of [['3.5',[1280,720]],['1.75',[640,360]],['4',[1764,346]],['8',[3428,602]]]){
   await page.locator('#zoom').selectOption(zoom);current=await state();
   assert.deepEqual(current.size,size);checks++;check(!current.smoothing,'Nearest sampling '+zoom);
   if(zoom==='3.5'||zoom==='1.75'){
    check(await page.evaluate(()=>{
     const w=$('view').width,s=Number($('zoom').value);
     return ['sword','axe','axe-body','spear','spear-body'].every(key=>images[key].every(img=>{
      const c=document.createElement('canvas');c.width=96;c.height=64;
      const ctx=c.getContext('2d');ctx.drawImage(img,0,0);const rgba=ctx.getImageData(0,0,96,64).data;
      let left=96,right=0;
      for(let y=0;y<64;y++)for(let x=0;x<96;x++)if(rgba[(y*96+x)*4+3]){left=Math.min(left,x);right=Math.max(right,x+1);}
      const column=key==='sword'?1:key.startsWith('axe')?2:3,origin=w*[.12,.36,.60,.84][column]-45*s;
      return origin+left*s>=0&&origin+right*s<=w&&origin+(96-right)*s>=0&&origin+(96-left)*s<=w;
     }));
    }),'All cels fit, including mirror and VFX, '+zoom);
   }
   await page.locator('#impact').click();
   if(folder)fs.writeFileSync(path.join(folder,'impact-'+zoom+'.png'),Buffer.from((await state()).image.split(',')[1],'base64'));
  }
  await page.locator('#zoom').selectOption('3.5');
  await page.locator('#ground').click();current=await state();
  check(current.frame===9&&current.cel===10,'Final-pose button selects last Attack cel');
  check(await page.evaluate(()=>images['axe-body'].slice(6,12).every((img,i)=>{
   const c=document.createElement('canvas');c.width=96;c.height=64;
   const ctx=c.getContext('2d');ctx.drawImage(img,0,0);
   const row=ctx.getImageData(0,63,96,1).data,blade=payload.poses.axe[i+5].blade;
   const contacts=[];
   for(let x=0;x<96;x++)if(row[x*4+3]&&!(x>=34&&x<=39)&&!(x>=51&&x<=56))contacts.push(x);
   return contacts.length>0&&contacts.every(x=>x>=Math.min(...blade.map(q=>q[0]))&&x<=Math.max(...blade.map(q=>q[0])));
  })),'Axe impact through final cel visibly touches the ground without VFX');
  check(await page.evaluate(()=>images.axe.every((img,i)=>
   (img.src!==images['axe-body'][i].src)===(i===5))),
  'Axe smear appears only on swing cel4, never on contact or hold');
  await page.locator('#recovery').click();current=await state();
  check(current.frame===10&&Math.abs(current.t-1.3333334)<1e-8,'Recovery starts after original clip');
  check(await page.evaluate(()=>index('sword')===10&&index('spear')===10),'Other weapons hold their last cel during recovery');
  await page.locator('#next').click();check((await state()).frame===11,'Step into blade release');
  check(await page.evaluate(()=>payload.poses.axe.slice(11).every(p=>Math.max(...p.edge.map(q=>q[1]))<63)),'Blade leaves ground at cel11');
  await page.locator('#returned').click();check((await state()).frame===15,'Returned button selects cel15');
  check(await page.evaluate(()=>images['axe-body'][0].src===images['axe-body'][16].src),'Cel15 is exact Idle RGBA');
  await page.locator('#next').click();check((await state()).frame===0,'Step wraps actual16 cels');
  await page.locator('#previous').click();check((await state()).frame===15,'Previous wraps actual16 cels');
  const playback=[];
  for(const speed of ['1','0.25']){
   await page.locator('#speed').selectOption(speed);
   await page.locator('#time').evaluate(el=>{el.value='.1';el.dispatchEvent(new Event('input',{bubbles:true}));});
   await page.locator('#play').click();
   const before=await page.evaluate(()=>({t,wall:performance.now()}));
   // Sample at least half a second; a single16ms timer tick dominates an80ms window.
   await page.waitForFunction(({start,speed})=>t>start+speed*.5,{start:before.t,speed:Number(speed)});
   const after=await page.evaluate(()=>({t,wall:performance.now()}));
   const ratio=(after.t-before.t)/((after.wall-before.wall)/1000);
   check(Math.abs(ratio/Number(speed)-1)<.08,'Measured playback speed '+speed+': '+ratio);
   playback.push({speed:Number(speed),measured:ratio,wallMs:after.wall-before.wall});
   await page.locator('#play').click();
   const held=(await state()).t;await page.waitForTimeout(150);check((await state()).t===held,'Pause holds time');
  }
  await page.locator('#idle').click();check((await state()).cel===0,'Return to Idle');
  for(const zoom of ['3.5','1.75']){
   await page.locator('#zoom').selectOption(zoom);
   await page.locator('#speed').selectOption('1');
   await page.locator('#time').evaluate(el=>{el.value='1.25';el.dispatchEvent(new Event('input',{bubbles:true}));});
   await page.locator('#play').click();
   await page.waitForFunction(()=>!playing&&t===payload.axeTimeline.duration);
   check((await state()).frame===15,'Playback ends on final cel '+zoom);
   const final=(await state()).image;
   await page.waitForTimeout(180);
   check((await state()).image===final,'Final pose stays held '+zoom);
   await page.locator('#play').click();
   current=await state();check(current.playing&&current.t<.2,'Play at end restarts '+zoom);
   await page.locator('#play').click();
   const paused=(await state()).t;
   await page.waitForTimeout(100);
   check((await state()).t===paused,'Pause after restart holds '+zoom);
   await page.locator('#play').click();
   await page.waitForFunction(start=>t>start+.1,paused);
   await page.locator('#play').click();
   check((await state()).t>paused,'Resume advances from paused time '+zoom);
  }
  // Resolve every key deterministically; wall-clock playback may drop a cel when
  // the browser is stalled. Record that separately instead of changing timing.
  for(let frame=0;frame<16;frame++){
   await page.evaluate(frame=>setTime(payload.axeTimeline.keys[frame].time),frame);
   check((await state()).frame===frame,'Resolve cel '+frame);
  }
  const sequences=[];
  for(const speed of ['1','0.25'])for(const mirror of [false,true])for(const vfx of [false,true]){
   await page.locator('#zoom').selectOption(mirror?'1.75':'3.5');
   await page.locator('#flip').setChecked(mirror);
   await page.locator('#vfx').setChecked(vfx);
   await page.locator('#speed').selectOption(speed);
   await page.locator('#time').evaluate(el=>{el.value='0';el.dispatchEvent(new Event('input',{bubbles:true}));});
   await page.evaluate(()=>{
    window.observedFrames=new Set([0]);
    window.originalRender=render;
    render=()=>{originalRender();observedFrames.add(frameAt(t));};
   });
   await page.locator('#play').click();
   await page.waitForFunction(()=>!playing&&t===payload.axeTimeline.duration);
   const frames=await page.evaluate(()=>{render=originalRender;return [...observedFrames];});
   check(frames[0]===0&&frames.at(-1)===15&&frames.every((f,i)=>i===0||f>frames[i-1]),'Playback advances in order to Idle');
   sequences.push({speed,mirror,vfx,frames,skipped:Array.from({length:16},(_,i)=>i).filter(i=>!frames.includes(i))});
  }
  check(errors.length===0,'No browser errors: '+errors.join('; '));
  console.log(JSON.stringify({checks,errors,playback,sequences,sizes:'1280x720,640x360,4x,8x'}));
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
