module.exports = async function tutorialSizes(api, browser) {
  const {session,start,until,clickControl,check}=api;
  const {context,page}=await session(browser,{width:1280,height:720},1000);
  try {
    await clickControl(page,'How to PlayButton','button');
    await until(page,s=>s.helpOpen);
    await clickControl(page,'ContinueButton','button');
    await until(page,s=>!s.helpOpen);
    await clickControl(page,'PlayButton','button');
    await until(page,s=>s.controls.some(c=>c.name==='NameInput'));
    await clickControl(page,'>Button','button');
    await until(page,s=>s.menuDifficulty==='hard');
    await start(page,false);
    check((await api.state(page)).tutorialStatus===0,'How to Play leaves first-Play guide unseen');
    await until(page,s=>s.tutorial && !s.dungeonTransitioning);
    await require('./tutorial.cjs')(api,page,{freshTutorial:true,viewports:[[1280,720],[640,360]]});
  } finally {await context.close();}
};
