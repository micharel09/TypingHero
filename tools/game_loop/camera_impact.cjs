// Camera acceptance through real input. The bridge and frame recorder only observe.
const fs=require('node:fs'),path=require('node:path');
module.exports=async function({session,state,until,clickControl,check,report},browser){
  report.cameraImpact=[];
  report.cameraImpactDriverSha256=require('node:crypto').createHash('sha256').update(fs.readFileSync(__filename)).digest('hex');
  const sizes=process.env.TYPINGHERO_CAMERA_SINGLE==='1'?[[1280,720]]:[[1280,720],[640,360]];
  for(const [width,height] of sizes){
    const {context,page}=await session(browser,{width,height},1000);
    const row={width,height};report.cameraImpact.push(row);
    const key=k=>page.keyboard.press(k,{delay:40});
    const frame=()=>page.evaluate(()=>window.typingHeroCombatFrame);
    const live=async()=>{
      let s=await until(page,t=>t.scene==='SampleScene'&&!t.dungeonTransitioning&&!t.tutorial);
      if(s.encounterReading){await key('Enter');s=await until(page,t=>!t.encounterReading&&!t.paused&&t.hp>0);}
      return s;
    };
    const pause=async()=>{await key('Escape');return until(page,t=>t.paused&&t.timeScale===0);};
    const resume=async()=>{await clickControl(page,'ResumeButton','button');return until(page,t=>!t.paused&&t.timeScale>0);};
    const restart=async()=>{
      let s=await state(page);if(!s.paused&&!s.overlay)await pause();
      await key('r');await until(page,t=>!t.paused&&!t.overlay&&t.score===0&&!t.dungeonTransitioning);
      s=await live();check(s.cameraImpactZoom===0&&s.cameraImpactSerial===0,'restart clears camera state');return s;
    };
    try{
      await page.evaluate(()=>{
        let latest=window.typingHeroCombatFrame;
        window.cameraFrames=[];
        Object.defineProperty(window,'typingHeroCombatFrame',{configurable:true,get:()=>latest,set:value=>{
          latest=value;
          const {parryParticles,...small}=value;
          window.cameraFrames.push({...small,at:performance.now()/1000});
        }});
      });
      await clickControl(page,'PlayButton','button');
      await until(page,t=>t.controls.some(c=>c.name==='NameInput'));
      await clickControl(page,'NameInput','input');await page.keyboard.press('Control+A');
      await page.keyboard.type('CameraQA',{delay:60});await key('Enter');
      let s=await until(page,t=>t.scene==='SampleScene'&&!t.dungeonTransitioning);
      if(s.tutorial){await clickControl(page,'PracticeButton','button');await until(page,t=>!t.tutorial&&!t.dungeonTransitioning);}
      s=await live();check(s.width===width&&s.height===height,'camera viewport matches render');
      // Warm the real render paths, then reset through the normal UI before recording.
      s=await until(page,t=>!t.enemyAttacking&&t.canParry&&t.current.length>0&&t.wordTime>1);
      await page.keyboard.type(s.current,{delay:35});
      await page.waitForFunction(()=>window.typingHeroCombatFrame?.normalImpacts===1&&window.typingHeroCombatFrame?.peakRenderCount===1&&window.typingHeroCombatFrame?.impactZoom===0,null,{timeout:5000});
      s=await restart();await page.evaluate(()=>window.cameraFrames=[]);
      const hud=JSON.stringify([s.playerStatusRect,s.enemyStatusRect,s.trayRect]);
      if(process.env.TYPINGHERO_CAMERA_CAPTURE==='1')await page.evaluate(()=>{
        const stream=document.querySelector('#unity-canvas').captureStream(60);
        const recorder=new MediaRecorder(stream,{mimeType:'video/webm;codecs=vp8',videoBitsPerSecond:4000000});
        const chunks=[];recorder.ondataavailable=e=>{if(e.data.size)chunks.push(e.data);};
        window.cameraRecording={recorder,chunks,stream,start:performance.now()/1000};recorder.start(250);
      });
      await page.waitForTimeout(300);
      s=await until(page,t=>!t.enemyAttacking&&t.canParry&&t.current.length>0&&t.wordTime>1);
      const hpBefore=s.enemyHp;
      await page.keyboard.type(s.current.slice(0,-1),{delay:35});
      await page.evaluate(last=>{
        const block=e=>{
          if(e.key.toLowerCase()!==last.toLowerCase())return;
          document.removeEventListener('keydown',block,true);
          const start=performance.now();while(performance.now()-start<180){}
          window.cameraHitchMs=performance.now()-start;
        };
        document.addEventListener('keydown',block,true);
      },s.current.slice(-1));
      await page.keyboard.type(s.current.slice(-1),{delay:35});
      await page.waitForFunction(()=>window.typingHeroCombatFrame?.normalImpacts>0,null,{timeout:10000});
      await page.waitForFunction(()=>window.typingHeroCombatFrame?.peakRenderCount===1&&window.typingHeroCombatFrame?.impactZoom===0,null,{timeout:2500});
      let f=await frame();check(f.impactZoom===0&&f.normalImpacts===1,'confirmed normal hit pulses once and returns');
      s=await state(page);check(JSON.stringify([s.playerStatusRect,s.enemyStatusRect,s.trayRect])===hud,'HUD and typing geometry remain fixed');
      row.hitchMs=await page.evaluate(()=>window.cameraHitchMs);
      check(row.hitchMs>=180&&f.enemyHP<hpBefore,'180ms final-key hitch still confirms actual damage');
      row.hitchContact=await page.evaluate(()=>{
        const frames=window.cameraFrames,i=frames.findIndex(f=>f.normalImpacts===1),contact=frames[i];
        const previous=frames.slice(0,i).reverse().find(f=>f.frame<contact.frame);
        return {frame:contact.frame,serial:contact.effectSerial,delta:previous?contact.unscaledNow-previous.unscaledNow:0};
      });
      check(row.hitchContact.delta>=.14,'hitch occurs on the actual confirmed-contact frame');
      const hitchFrames=await page.evaluate(()=>window.cameraFrames.filter(f=>f.normalImpacts===1));
      check(hitchFrames.some(f=>Math.abs(f.impactZoom-.02)<.00001)&&hitchFrames.some(f=>f.peakRenderCount===1),'hitch pulse renders and acknowledges its exact peak');
      check(f.peakRenderCount===1,'hitch normal peak acknowledged exactly once');
      check(f.worldWidth===320&&f.worldHeight===180,'probe reports actual dungeon render texture');
      const normal=f.normalImpacts;
      await page.keyboard.press(s.current[0]?.toLowerCase()==='z'?'q':'z',{delay:30});
      await page.waitForTimeout(120);check((await frame()).normalImpacts===normal,'wrong character emits no attack camera pulse');
      await page.waitForFunction(()=>window.typingHeroCombatFrame?.receivedImpacts>0,null,{timeout:15000});
      f=await frame();check(f.playerHP<40&&f.receivedImpacts===1,'accepted damage produces received kick');
      await page.waitForTimeout(250);check((await frame()).impactZoom===0,'received damage does not zoom');
      let lastParry=0,pausedPulse=false,slayerTyped=false;
      const deadline=performance.now()+120000;
      while(performance.now()<deadline){
        s=await state(page);f=await frame();
        if(s.finished)break;
        if(!pausedPulse&&f.parryImpacts>0&&f.impactKind===2&&f.impactZoom>0){
          await pause();const held=await frame();await page.waitForTimeout(250);const again=await frame();
          check(held.impactZoom>0&&again.impactZoom===held.impactZoom&&again.impulseCount===held.impulseCount,'Pause freezes impact envelope and pending kick');
          await page.screenshot({path:path.join(report.outputDirectory,`camera-${width}-pause.png`)});
          await resume();pausedPulse=true;continue;
        }
        if(f.slayer){
          const serial=f.impactSerial;
          await page.keyboard.type(s.current,{delay:35});await page.waitForTimeout(100);
          const after=await frame();
          check(after.lethalImpacts>f.lethalImpacts||after.impactZoom===0,'Slayer hits do not stack transient zoom');
          if(after.lethalImpacts===f.lethalImpacts)check(after.impactSerial===serial,'Slayer typing adds no camera event');
          slayerTyped=true;continue;
        }
        if(f.canParry&&f.enemyAttacking&&f.enemyContactIn<=.19&&f.enemyContactIn>.025&&performance.now()-lastParry>350){
          await key('Space');lastParry=performance.now();
        }
        await page.waitForTimeout(12);
      }
      s=await state(page);f=await frame();
      check(pausedPulse,'real parry pulse was paused and resumed');check(slayerTyped,'real parries break guard and reach Slayer');
      check(s.finished&&s.hp>0,'keyboard combat reaches lethal victory');
      await until(page,t=>t.overlay||t.encounterReading||(t.finished&&t.enemyHp===-1&&t.controls.some(c=>c.name==='PracticeButton')),10000);
      await page.waitForTimeout(400);
      check((await frame()).impactZoom===0,'results clear impact');
      const recorded=await page.evaluate(()=>window.cameraFrames);
      row.samples=recorded;
      const normalFrames=recorded.filter(t=>t.impactKind===1),parryFrames=recorded.filter(t=>t.impactKind===2);
      const hurtFrames=recorded.filter(t=>t.impactKind===3),entryFrames=recorded.filter(t=>t.impactKind===4),lethalFrames=recorded.filter(t=>t.impactKind===5);
      check(normalFrames.some(t=>t.impactZoom>.015)&&normalFrames.every(t=>t.impactZoom<=.02001),'normal zoom reaches bounded 2 percent peak');
      check(parryFrames.some(t=>t.impactZoom>.025)&&parryFrames.every(t=>t.impactZoom<=.03001),'parry zoom reaches bounded 3 percent peak');
      check(hurtFrames.length>0&&hurtFrames.every(t=>t.impactZoom===0),'received profile remains zoom-free');
      check(entryFrames.length>0&&entryFrames.every(t=>t.impactZoom===0),'guard break uses existing Slayer base zoom');
      check(lethalFrames.length>0&&lethalFrames.every(t=>t.impactZoom<=.02001)&&lethalFrames.some(t=>t.impactZoom>.015),'Slayer lethal retains captured 2 percent profile');
      check(recorded.filter(t=>t.cameraBase>0).every(t=>Math.abs(t.cameraHalfHeight-t.cameraBase/(1+t.impactZoom))<.0001),'render projection composes pulse with current base lens');
      for(const [kind,frames,limit] of [['parry',parryFrames,1.1],['received',hurtFrames,1.1],['lethal',lethalFrames,2.2]]){
        const pixels=frames.filter(f=>f.timeScale>.01&&f.cameraHalfHeight>0).map(f=>Math.hypot(f.cameraCorrection.x,f.cameraCorrection.y,f.cameraCorrection.z)/(2*f.cameraHalfHeight/f.worldHeight));
        const peak=Math.max(0,...pixels);
        check(peak>.3&&peak<=limit,kind+' reaches the actual camera at bounded native-pixel gain');
        (row.peakPixels||={})[kind]=peak;
      }
      row.windows=Object.fromEntries([['normal',normalFrames],['parry',parryFrames],['received',hurtFrames],['guard-break',entryFrames],['lethal',lethalFrames]].map(([name,frames])=>{
        const first=name==='parry'?(frames.find(f=>f.impactSerial!==frames[0].impactSerial)||frames[0]):frames[0];
        return [name,{start:first.at-.3,end:first.at+(name==='guard-break'?1:.65)}];
      }));
      if(process.env.TYPINGHERO_CAMERA_CAPTURE==='1'){
        const video=await page.evaluate(async()=>{
          const r=window.cameraRecording;
          await new Promise(resolve=>{r.recorder.onstop=resolve;r.recorder.stop();});r.stream.getTracks().forEach(t=>t.stop());
          const blob=new Blob(r.chunks,{type:'video/webm'});
          const data=await new Promise(resolve=>{const reader=new FileReader();reader.onload=()=>resolve(reader.result.split(',')[1]);reader.readAsDataURL(blob);});
          return {start:r.start,data};
        });
        row.videoStart=video.start;row.video=`camera-${width}.webm`;
        fs.writeFileSync(path.join(report.outputDirectory,row.video),Buffer.from(video.data,'base64'));
      }
      await restart();
      await pause();await key('m');s=await until(page,t=>t.scene==='MainMenu'&&!t.dungeonTransitioning);
      check(s.cameraImpactZoom===0&&s.gameplayActors===0&&Math.abs(s.cameraOrtho-5.625)<.001&&Math.abs(s.cameraFieldOfView-35)<.01&&!s.arenaCinematic,'Menu restores camera baseline and clears gameplay ownership');
    }finally{
      if(!row.samples)row.samples=await page.evaluate(()=>window.cameraFrames||[]).catch(()=>[]);
      await context.close();
    }
  }
};
