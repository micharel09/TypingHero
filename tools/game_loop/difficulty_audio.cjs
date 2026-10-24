// Native tutorial keys; observe WebAudio buffer starts without changing Unity/audio state.
module.exports=async function difficultyAudio(api,browser){
  const {session,start,state,until,clickControl,check,report}=api;
  const {context,page}=await session(browser,{width:960,height:540},1000);
  const key=async name=>{
    const s=await state(page),frame=s.frame+Math.ceil(s.qaAgeMs/Math.max(1,s.frameMs))+2;
    await page.keyboard.down(name);
    try{await page.waitForTimeout(Math.max(40,s.frameMs*2));await until(page,t=>t.frame>=frame);}
    finally{await page.keyboard.up(name);}
  };
  const slider=async(name,value)=>{
    await clickControl(page,name,'button');
    const field=name==='MusicVolume'?'musicVolume':'sfxVolume';
    await page.keyboard.down(value===0?'ArrowLeft':'ArrowRight');
    try{await until(page,s=>Math.abs(s[field]-value)<.015,6000);}
    finally{await page.keyboard.up(value===0?'ArrowLeft':'ArrowRight');}
  };
  const wrong=async()=>((await state(page)).current[0]||'').toLowerCase()==='z'?'q':'z';
  const sounds=()=>page.evaluate(()=>window.difficultyAudioStarts||[]);
  const typingSounds=list=>list.filter(x=>Math.abs(x.duration-.02)<.006||Math.abs(x.duration-.102)<.008);
  try{
    await slider('MusicVolume',0);await slider('SFXVolume',1);
    await start(page,false);
    await until(page,s=>s.tutorial&&s.tutorialStatus===0);
    let before=await sounds();await page.keyboard.type('bz',{delay:100});await page.waitForTimeout(250);
    check(typingSounds((await sounds()).slice(before.length)).length===0,'tutorial reading rejects typing without tick/error buffers');
    await clickControl(page,'PracticeButton','button');await until(page,s=>s.tutorialParry); await clickControl(page,'Start Battle \u00b7 EnterButton','button');await until(page,s=>!s.tutorial&&!s.dungeonTransitioning&&s.timeScale===1);
    await page.waitForTimeout(300);before=await sounds();
    const accepted=await state(page);
    await key(accepted.current[0].toLowerCase());await until(page,s=>s.inputCorrect>accepted.inputCorrect);await page.waitForTimeout(150);
    check((await sounds()).slice(before.length).some(x=>Math.abs(x.duration-.02)<.006),'accepted battle character plays the original20ms typing tick');
    before=await sounds();await key(await wrong());await page.waitForTimeout(180);
    check((await sounds()).slice(before.length).some(x=>Math.abs(x.duration-.102)<.008),'rejected battle character plays the original102ms error');
    await key('Escape');await until(page,s=>s.paused);await page.waitForTimeout(300);
    before=await sounds();await page.keyboard.type('lz',{delay:100});await page.waitForTimeout(250);
    check(typingSounds((await sounds()).slice(before.length)).length===0,'pause rejects typing without tick/error buffers');
    await slider('SFXVolume',0);await key('Escape');await until(page,s=>!s.paused);await page.waitForTimeout(350);
    before=await sounds();
    const muted=await state(page);
    await key(muted.current[0].toLowerCase());await until(page,s=>s.inputCorrect>muted.inputCorrect);
    await key(await wrong());await page.waitForTimeout(250);
    check(typingSounds((await sounds()).slice(before.length)).length===0,'muted accepted/rejected typing starts no tick/error buffers');
    report.typingAudioStarts=typingSounds(await sounds());
    check((report.endlessWrites||0)===0&&report.fixtureWrites===0,'typing audio regression produces no score submissions');
  }finally{await context.close();}
};
