const path=require('node:path');
const parryRole=p=>p.role==='ParryClash'||p.role==='ParryImpact';
module.exports=async function({session,start,state,until,check,report},browser){
 report.clashes=[];
 for(const [width,height] of [[1280,720],[640,360]]){
  const {context,page}=await session(browser,{width,height},1000);
  const row={width,height,samples:[]}; report.clashes.push(row);
  try{
   await page.screenshot({path:path.join(report.outputDirectory,`lighting-${width}-menu.png`)});
   let s=await start(page); check(s.width===width&&s.height===height,'requested render size');
   await page.waitForTimeout(800); await page.screenshot({path:path.join(report.outputDirectory,`lighting-${width}-idle.png`)});
   await until(page,t=>!t.enemyAttacking&&t.canParry&&t.current.length>0&&t.wordTime>1);
   s=await state(page); await page.keyboard.type(s.current,{delay:15});
   s=await until(page,t=>t.enemyHp<850); check(s.sparks.length===0,'normal hit has no burst');
   await page.keyboard.press('Escape',{delay:20}); await until(page,t=>t.paused);
   await page.keyboard.press('r',{delay:40});
   const ready=await until(page,t=>!t.paused&&!t.overlay&&t.hp===40&&t.enemyHp===850&&t.score===0);
   await until(page,t=>t.frame>=ready.frame+2&&t.sparks.length===0);

   let lastParry=-Infinity, captured=false;
   const end=performance.now()+65000;
   while(performance.now()<end){
    s=await state(page);
    check(s.sparks.every(parryRole),'only successful parry owns sparks');
    if(row.samples.at(-1)?.frame!==s.frame) row.samples.push({frame:s.frame,frameMs:s.frameMs,allocatedBytes:s.allocatedBytes,
     objects:s.objects,slayer:s.slayer,parries:s.parries,sparks:s.sparks});
    if(s.sparks.some(p=>p.particles>0)){
     const p=s.sparks.find(p=>p.particles>0); check(p.particles<=66,'bounded parry particle count');
     check(p.meanDepthSpeed>0,'clash carries depth velocity');
     if(!captured){
      captured=true; row.first=p;
      await page.screenshot({path:path.join(report.outputDirectory,`lighting-${width}-parry.png`)});
     }
    }
    if(s.slayer&&captured){
     await page.keyboard.type(s.current,{delay:4}); await page.waitForTimeout(160);
     s=await state(page);check(s.sparks.every(parryRole),'Slayer has no hit burst');
     await page.screenshot({path:path.join(report.outputDirectory,`lighting-${width}-slayer.png`)}); break;
    }
    if(s.hp<40) throw Error('Player took damage before art sequence completed');
    const impactIn=1-((s.enemyAnimationTime%1)*1.3333334+s.qaAgeMs/1000*s.timeScale);
    if(s.canParry&&s.enemyAttacking&&impactIn<=Math.min(.24,1.5*s.frameMs/1000+.12)&&impactIn>.04&&performance.now()-lastParry>350){
     await page.keyboard.down('Space');
     try{await until(page,t=>t.parryPose||t.parryWindow||t.matchParries>s.matchParries,1500);}
     finally{await page.keyboard.up('Space');}
     lastParry=performance.now();
    }
    await page.waitForTimeout(12);
   }
   check(captured,'real keyboard successful parry emits clash');
   check(s.slayer,'sequence reaches Slayer');
  }finally{await context.close();if(page.video())row.video=await page.video().path();}
 }
};
