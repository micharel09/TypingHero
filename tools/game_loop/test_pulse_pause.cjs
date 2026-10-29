 'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),vm=require('node:vm'),path=require('node:path');
const source=fs.readFileSync(path.join(__dirname,'arena_pulse_pause.cjs'),'utf8');
const lit=(frame,extra={})=>({frame,parries:2,timeScale:1,pulseRigCount:1,pulseRigId:9,pulseRigScene:'SampleScene',pulseRigProfile:'DungeonCombat',parryPulseEnabled:true,parryPulseIntensity:2,combatPulseEnabled:true,combatPulseIntensity:2,...extra});
function helpers(publications,{timeout=false,downError=false}={}){
 const events=[],window={typingHeroCombatFrame:lit(100)};
 const page={keyboard:{down:async k=>{events.push('down:'+k);if(downError)throw Error('down');},up:async k=>events.push('up:'+k)},evaluate:async()=>{events.push('read');return window.typingHeroCombatFrame;},waitForFunction:async(fn,arg)=>{
  events.push('wait:'+arg.kind);
  if(!timeout)for(const f of publications){window.typingHeroCombatFrame=f;const v=fn(arg);if(v){const copy=JSON.parse(JSON.stringify(v));window.typingHeroCombatFrame=lit(999);return{jsonValue:async()=>copy,dispose:async()=>events.push('dispose')};}}
  const e=Error('timeout');e.name='TimeoutError';throw e;
 }};
 const code=source.slice(source.indexOf(' const wait='),source.indexOf(' const menuCheck'))+'\n globalThis.helpers={wait,heldKey};';
 const context={page,window,frame:()=>page.evaluate(),performance:{now:()=>0}};vm.createContext(context);vm.runInContext(code,context);
 return {...context.helpers,events};
}
test('Space rejects stale and counter-only frames and retains successful publication',async()=>{
 const h=helpers([lit(10),lit(11,{parries:1}),lit(11,{pulseRigId:8}),lit(11,{parryPulseIntensity:0}),lit(12)]);
 const f=await h.heldKey('Space','pulse',{before:10,parries:1,mode:'parry',owner:9});
 assert.equal(f.frame,12);assert.deepEqual(h.events,['down:Space','wait:pulse','dispose','up:Space']);
 await h.heldKey('Escape','pause',{before:12}).catch(()=>{});
 assert.deepEqual(h.events.slice(4,6),['down:Escape','wait:pause']);
});
for(const downError of [false,true])test(`timeout or down failure releases key (${downError})`,async()=>{
 const h=helpers([],{timeout:true,downError});await assert.rejects(h.heldKey('Escape','pause',{before:1}));assert.equal(h.events.at(-1),'up:Escape');
});
test('missed lit Pause resumes and three failed attempts fail the run',async()=>{
 let frame=0,attempts=0,closed=false;const events=[];
 const originalWrite=fs.writeFileSync;fs.writeFileSync=()=>{};
 const page={evaluate:async()=>{events.push('read');return lit(++frame);},keyboard:{type:async()=>events.push('type'),down:async k=>events.push('down:'+k),up:async k=>events.push('up:'+k)},waitForFunction:async(fn,{kind,arg})=>{
  events.push('wait:'+kind);let f;
  if(kind==='fresh')f=lit(++frame,{pulseRigCount:0,pulseRigId:0,pulseRigScene:'',pulseRigProfile:'',combatPulseIntensity:0,parryPulseIntensity:0});
  if(kind==='pulse')f=lit(++frame);
  if(kind==='pause'){attempts++;f=lit(++frame,{timeScale:0,combatPulseIntensity:0});}
  if(kind==='resume')f=lit(++frame);
  // Initial gameplay owner publication follows Menu.
  if(kind==='fresh'&&events.filter(x=>x==='wait:fresh').length>1)f=lit(++frame);
  const value=vm.runInNewContext('('+fn.toString()+')(arg)',{window:{typingHeroCombatFrame:f},arg:{kind,arg}});
  assert.ok(value,'fixture publication satisfies real predicate');
  return {jsonValue:async()=>value,dispose:async()=>{}};
 }};
 try{
  await assert.rejects(require('./arena_pulse_pause.cjs')({session:async()=>({page,context:{close:async()=>{closed=true;}}}),start:async()=>{},fight:async()=>{},state:async()=>{events.push('state');return {frame:++frame,scene:'SampleScene'};},until:async()=>{events.push('until');return {frame:++frame,scene:'SampleScene',current:'a',wordTime:2};},check:(ok,m)=>{if(!ok)throw Error(m);},report:{outputDirectory:__dirname}},{}),/catches positive combat light/);
  assert.equal(attempts,3);assert.equal(closed,true);assert.equal(events.filter(x=>x==='wait:resume').length,3);
  for(let i=0;i<events.length;i++)if(events[i]==='wait:pulse')assert.deepEqual(events.slice(i,i+3),['wait:pulse','down:Escape','wait:pause']);
 }finally{fs.writeFileSync=originalWrite;}
});
