/* Queue/input/reset adapted from r4; source revision remains immutable. */
const $=s=>document.querySelector(s), preference=matchMedia('(prefers-reduced-motion: reduce)');
const normal=['resolve','honor','courage','guard','strike'];
const fixtures={normal,pool,long:['different','question','remember'],short:['a','i','to','be'],mixed:['a','different','i','question','to','remember'],stress:['a','to','counterrevolutionary','i','resolve']};
const state={index:0,completed:0,progress:0,wrong:null,started:-Infinity};
const reduced=()=>$('#reduced').checked||preference.matches;
const words=()=>fixtures[$('#fixture').value],target=()=>words()[state.index%words().length];
let raf=0,slideStart=-Infinity,slideDistance=0,slide7=0,ready=false;
const cache=new Map();
function load(src){if(!cache.has(src)){const im=new Image();im.src=src;cache.set(src,im);}return cache.get(src);}
const raster=(w,mode)=>`../hud-r4/png/${mode}-${w}.png`;
const current=(w,mode,i)=>`../hud-r4/png/word-${w}-16-${mode}-${i}.png`;
const assets=['preview/normal.png','preview/slayer.png','preview/error-frame.png'];
for(const w of new Set(Object.values(fixtures).flat())){
 for(const mode of ['progress','wrong'])for(let i=0;i<w.length+(mode==='progress');i++)assets.push(current(w,mode,i));
 for(const color of ['muted','metal'])assets.push(raster(w,color));
}
function paint(now){
 if(!ready)return;
 const error=now-state.started<180,t=Math.min(1,Math.max(0,(now-slideStart)/100));
 const offset=reduced()?0:Math.round(slideDistance*Math.pow(1-t,3));
 const offset7=reduced()?0:Math.round(slide7*Math.pow(1-t,3));
 const queue=[0,1,2].map(i=>words()[(state.index+i)%words().length]);
 for(const rev of [6,7]){
  const canvas=$(`#r${rev} canvas`),ctx=canvas.getContext('2d');ctx.imageSmoothingEnabled=false;
  ctx.clearRect(0,0,320,180);ctx.drawImage(load(`preview/${$('#slayer').checked?'slayer':'normal'}.png`),0,0);
  if(error&&!$('#slayer').checked)ctx.drawImage(load('preview/error-frame.png'),61,146);
  let cursor=65+offset;const positions=[];
  ctx.save();ctx.beginPath();ctx.rect(65,151,190,20);ctx.clip();
  queue.forEach((w,i)=>{
   const left=rev===7?65+i*66+Math.round((58-widths[w])/2)+offset7:cursor;
   ctx.save();if(rev===7){ctx.beginPath();ctx.rect(65+i*66+offset7,151,58,20);ctx.clip();}
   ctx.drawImage(load(i===0?current(w,error?'wrong':'progress',error?state.wrong:state.progress):raster(w,i===1?'muted':'metal')),left,151);
   ctx.restore();positions.push(left);cursor+=widths[w]+8;
  });ctx.restore();
  Object.assign(canvas.dataset,{current:queue[0],next:queue[1],third:queue[2],completed:state.completed,progress:state.progress,wrong:state.wrong??'',error,started:state.started,paintedAt:now,offset:rev===7?offset7:offset,positions:JSON.stringify(positions)});
 }
 $('#status').value=`${state.progress}/${target().length} · ${state.completed} từ`;
 cancelAnimationFrame(raf);if(error||offset||offset7)raf=requestAnimationFrame(paint);
}
function input(value){let distance=0,completed=0;for(const ch of value){if(!/^\p{L}$/u.test(ch))continue;if(ch.toLowerCase()===target()[state.progress]){state.progress++;state.started=-Infinity;state.wrong=null;if(state.progress===target().length){distance=widths[target()]+8;state.index=(state.index+1)%words().length;state.completed++;state.progress=0;completed++;}}else{state.wrong=state.progress;state.progress=0;state.started=performance.now();}}if(completed){slideStart=performance.now();slideDistance=completed===1?distance:0;slide7=completed===1?66:0;}paint(performance.now());}
function reset(){slideStart=-Infinity;slideDistance=slide7=0;state.index=0;state.completed=0;state.progress=0;state.wrong=null;state.started=-Infinity;$('#entry').value='';paint(performance.now());}
$('#entry').addEventListener('input',e=>{input(e.target.value);e.target.value='';});
$('#correct').onclick=()=>input(target()[state.progress]);$('#wrong').onclick=()=>input(target()[state.progress]==='z'?'x':'z');$('#reset').onclick=reset;
$('#fixture').onchange=()=>{$('#notice').textContent=$('#fixture').value==='stress'?'Stress ngoài pool — từ rộng hơn58px bị clip trong ô; chỉ áp dụng bản review.':'';reset();};
$('#slayer').onchange=()=>paint(performance.now());
$('#gray').onchange=()=>document.querySelectorAll('canvas').forEach(c=>c.style.filter=$('#gray').checked?'grayscale(1)':'none');
$('#scale').onchange=()=>document.querySelectorAll('canvas').forEach(c=>{c.style.width=320*Number($('#scale').value)+'px';c.style.height=180*Number($('#scale').value)+'px';});
$('#order').onclick=()=>{const main=$('main');main.append(main.firstElementChild);};
function reduce(){if(reduced()){slideStart=-Infinity;slideDistance=slide7=0;}paint(performance.now());}
$('#reduced').onchange=reduce;preference.addEventListener('change',reduce);
Promise.all(assets.map(s=>load(s).decode())).then(()=>{ready=true;reset();document.body.dataset.ready='true';}).catch(e=>{$('#status').value='Không tải được raster: '+e.message;});
