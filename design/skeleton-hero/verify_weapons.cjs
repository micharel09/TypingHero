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
  check((await page.evaluate(()=>Object.values(images).map(a=>a.length).join(',')))==='11,11,11,11,11,1','All reference/new images loaded');
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
  check(await page.evaluate(()=>images['axe-body'].slice(6).every((img,i)=>{
   const c=document.createElement('canvas');c.width=96;c.height=64;
   const ctx=c.getContext('2d');ctx.drawImage(img,0,0);
   const row=ctx.getImageData(0,63,96,1).data,blade=payload.poses.axe[i+5].blade;
   const contacts=[];
   for(let x=0;x<96;x++)if(row[x*4+3]&&!(x>=34&&x<=39)&&!(x>=51&&x<=56))contacts.push(x);
   return contacts.length>0&&contacts.every(x=>x>=Math.min(...blade.map(q=>q[0]))&&x<=Math.max(...blade.map(q=>q[0])));
  })),'Axe impact through final cel visibly touches the ground without VFX');
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
  check(errors.length===0,'No browser errors: '+errors.join('; '));
  console.log(JSON.stringify({checks,errors,playback,sizes:'1280x720,640x360,4x,8x'}));
 }finally{await browser.close();}
})().catch(e=>{console.error(e);process.exitCode=1;});
