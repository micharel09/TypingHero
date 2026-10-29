'use strict';
// Adapted from arena_pulse_pause normal typing/Escape and arena_combat timed Space.
// Read-only fast frame observer; screenshots only after a confirmed lit Pause.
const fs=require('node:fs'),path=require('node:path');
module.exports=async function(api,browser){
 const {session,start,fight,state,until,check,report}=api;
 const {context,page}=await session(browser,{width:1280,height:720},1000);
 const out=report.outputDirectory,events=[];
 const frame=()=>page.evaluate(()=>window.typingHeroCombatFrame);
 const read=async label=>{const prior=await state(page);const s=await until(page,s=>s.frame>prior.frame),f=await frame();events.push({label,at:Date.now(),s,f});return {s,f};};
 const wait=async(kind,arg={},timeout=2000)=>{
  const h=await page.waitForFunction(({kind,arg})=>{
   const f=window.typingHeroCombatFrame;if(!f||f.frame<=(arg.before??-1))return false;
   const owner=f.pulseRigCount===1&&f.pulseRigId!==0&&f.pulseRigScene==='SampleScene'&&f.pulseRigProfile==='DungeonCombat';
   let ok=false;
   if(kind==='fresh')ok=true;
   if(kind==='window'){
    const age=(performance.now()-window.typingHeroCombatFramePublishedAt)/1000,contact=f.enemyContactIn-age*f.timeScale;
    ok=!f.slayer&&f.canParry&&f.enemyAttacking&&age<.1&&contact>.11&&contact<=.24;
   }
   if(kind==='pulse')ok=owner&&(!arg.owner||f.pulseRigId===arg.owner)&&f[arg.mode+'PulseEnabled']&&f[arg.mode+'PulseIntensity']>0&&(arg.mode!=='parry'||f.parries>arg.parries);
   if(kind==='pause')ok=f.timeScale===0;
   if(kind==='resume')ok=f.timeScale===1;
   if(kind==='clear')ok=f.combatPulseIntensity===0&&f.parryPulseIntensity===0;
   if(!ok)return false;
   const fields=['frame','parries','timeScale','pulseRigCount','pulseRigId','pulseRigScene','pulseRigProfile','combatPulseEnabled','combatPulseIntensity','parryPulseEnabled','parryPulseIntensity'];
   return Object.fromEntries(fields.map(k=>[k,f[k]]));
  },{kind,arg},{polling:'raf',timeout});
  try{return await h.jsonValue();}finally{await h.dispose();}
 };
 const fresh=async()=>{const prior=await frame();return wait('fresh',{before:prior?.frame??-1});};
 const heldKey=async(key,kind,arg,timeout=3000)=>{try{await page.keyboard.down(key);return await wait(kind,arg,timeout);}finally{await page.keyboard.up(key);}};
 const menuCheck=async()=>{await until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);const f=await fresh();check(f.pulseRigCount===0&&f.pulseRigId===0&&!f.pulseRigScene&&!f.pulseRigProfile&&f.combatPulseIntensity===0&&f.parryPulseIntensity===0,'fresh Menu publication clears owner and pulses');};
 const owner=f=>f&&f.pulseRigCount===1&&f.pulseRigId!==0&&f.pulseRigScene==='SampleScene'&&f.pulseRigProfile==='DungeonCombat';
 try{
  await menuCheck();
  await start(page);await fight(page);
  const actor=await state(page),initialOwner=await fresh();
  check(owner(initialOwner)&&initialOwner.pulseRigScene===actor.scene,'unique gameplay-scene DungeonCombat rig');
  events.push({label:'initial-owner',f:initialOwner});
  for(const mode of ['combat','parry']){
   const enabled=mode+'PulseEnabled',intensity=mode+'PulseIntensity';let held;
   for(let attempt=0;attempt<3&&!held;attempt++){
    await read(mode+'-attempt-'+attempt);
    let pulse;
    if(mode==='combat'){
     const s=await until(page,s=>!s.paused&&!s.finished&&!s.hitLocked&&!s.pendingAttack&&!s.enemyAttacking&&s.current.length>0&&s.wordTime>1);
     const before=await fresh();await page.keyboard.type(s.current,{delay:35});
     try{pulse=await wait('pulse',{before:before.frame,owner:before.pulseRigId,mode},1500);}
     catch(error){if(error.name!=='TimeoutError')throw error;events.push({label:mode+'-miss-'+attempt});continue;}
    }else{
     const window=await wait('window',{},15000);
     try{pulse=await heldKey('Space','pulse',{before:window.frame,owner:window.pulseRigId,parries:window.parries,mode},1500);}
     catch(error){if(error.name!=='TimeoutError')throw error;events.push({label:mode+'-input-miss-'+attempt});continue;}
    }
    const paused=await heldKey('Escape','pause',{before:pulse.frame});
    events.push({label:mode+'-pause-publication-'+attempt,f:paused,pulse});
    if(!owner(paused)||paused.pulseRigId!==pulse.pulseRigId||!paused[enabled]||paused[intensity]<=0){
     await heldKey('Escape','resume',{before:paused.frame});await until(page,s=>!s.paused);continue;
    }
    const prior=await state(page),slow=await until(page,s=>s.frame>prior.frame&&s.paused&&s.timeScale===0,3000);
    held={s:slow,f:paused,at:Date.now()};events.push({label:mode+'-paused-'+attempt,...held});
   }
   check(!!held,'native Escape catches positive '+mode+' light');
   await page.screenshot({path:path.join(out,mode+'-held.png')});
   let previous=held.f.frame,slowPrevious=held.s.frame;
   for(let i=0;i<3;i++){
    await page.waitForTimeout(150);const now=await read(mode+'-hold-'+i);
    check(now.f.frame>previous&&now.s.frame>slowPrevious&&now.s.paused&&now.s.hp===held.s.hp&&now.s.score===held.s.score&&now.s.combatSeconds===held.s.combatSeconds,'new publication freezes combat '+mode+' '+i);
    check(owner(now.f)&&now.f.pulseRigId===held.f.pulseRigId&&now.f[enabled]&&now.f[intensity]===held.f[intensity],'lit pulse exact hold '+mode+' '+i);previous=now.f.frame;slowPrevious=now.s.frame;
   }
   events.push({label:mode+'-hold-elapsed',elapsedMs:Date.now()-held.at,firstFrame:held.f.frame,lastFrame:previous,firstSlowFrame:held.s.frame,lastSlowFrame:slowPrevious});
   check(Date.now()-held.at>=450,'hold at least 450ms');
   await heldKey('Escape','resume',{before:previous});await until(page,s=>!s.paused&&s.timeScale===1);
   await wait('clear');
   const s=await until(page,s=>!s.slayer&&s.slayerPresentationWeight===0&&s.cameraOrtho===5.625,10000);
   check(s.worldWidth===320&&s.worldHeight===180,'normal presentation restored');
  }
  const old=(await frame()).pulseRigId;
  await heldKey('Escape','pause',{before:(await frame()).frame});await until(page,s=>s.paused);
  await page.keyboard.press('r',{delay:60});await fight(page);
  const restarted=await fresh();
  check(owner(restarted)&&restarted.pulseRigId!==old,'Restart selects a new rig');
  events.push({label:'restart-owner',f:restarted});
  await heldKey('Escape','pause',{before:(await frame()).frame});await until(page,s=>s.paused);
  await page.keyboard.press('m',{delay:55});await until(page,s=>s.scene==='MainMenu'&&!s.dungeonTransitioning);
  await menuCheck();
  await start(page);await fight(page);const reentered=await fresh();
  check(owner(reentered)&&reentered.pulseRigId!==old&&reentered.pulseRigId!==restarted.pulseRigId&&reentered.combatPulseIntensity===0&&reentered.parryPulseIntensity===0,'reentry selects fresh owner without stale pulse');
  events.push({label:'reentry-owner',f:reentered});
 }finally{report.pulsePauseEvents=events;fs.writeFileSync(path.join(out,'pulse-pause-events.json'),JSON.stringify(events,null,2));await context.close();}
};
