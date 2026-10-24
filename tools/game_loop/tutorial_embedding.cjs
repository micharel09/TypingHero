module.exports = async function embedding(api, browser) {
  const {session, state, until, clickControl, check, report} = api;
  const {context, page} = await session(browser, {width:1280,height:720}, 1000, true);
  try {
    const writes = report.fixtureWrites;
    const game = page.frame({name:'game'});
    await game.waitForFunction(() => window.typingHeroQA?.scene === 'MainMenu', null, {timeout:120000});
    await until(game, s => s.width === 960 && s.height === 540);
    await game.evaluate(() => { window.__qaKeys = []; addEventListener('keydown', e => window.__qaKeys.push(e.key)); });
    await page.evaluate(() => { window.__qaKeys = []; addEventListener('keydown', e => window.__qaKeys.push(e.key)); });
    await game.locator('#unity-canvas').click({position:{x:30,y:220}});
    await clickControl(game, 'PlayButton', 'button', page);
    await until(game,s=>s.controls.some(c=>c.name==='NameInput'));
    await clickControl(game, 'NameInput', 'input', page);
    report.iframeFocus = await game.evaluate(() => ({focused:document.hasFocus(), active:document.activeElement?.tagName}));
    await page.keyboard.press('Control+A', {delay:120});
    await page.keyboard.press('Backspace', {delay:120});
    await page.keyboard.type('FrameQA', {delay:100});
    await page.keyboard.press('Enter', {delay:120});
    await page.waitForTimeout(700);
    report.iframeKeys = await game.evaluate(() => window.__qaKeys);
    report.outerKeys = await page.evaluate(() => window.__qaKeys);
    await page.screenshot({path:report.outputDirectory + '/tutorial-iframe-entry.png'});
    let s = await until(game, s => s.tutorial && s.current.length > 0 && s.timeScale === 0);
    check(s.width === 960 && s.height === 540, 'tutorial starts in actual 960x540 iframe');
    await page.screenshot({path:report.outputDirectory + '/tutorial-iframe.png'});
    await page.locator('#fullscreen').click();
    const canvas = game.locator('#unity-canvas');
    await canvas.click({position:{x:30,y:220}});
    s = await until(game, s => s.width === 1280 && s.height === 720);
    // Render dimensions publish before the reference canvas finishes its resize.
    const resizedFrame = s.frame;
    s = await until(game, s => s.frame >= resizedFrame + 3 && s.width === 1280 && s.height === 720);
    check(await page.evaluate(() => document.fullscreenElement?.tagName === 'IFRAME'), 'native fullscreen belongs to iframe');
    if (s.paused) {
      report.iframeResume = {canvas:await canvas.boundingBox(), frame:s.frame,
        focus:await game.evaluate(() => ({focused:document.hasFocus(), active:document.activeElement?.tagName}))};
      await clickControl(game, 'ResumeButton', 'button', page);
      s = await until(game, s => !s.paused && !s.controls.some(c => c.name === 'ResumeButton'));
    }
    check(s.tutorial && s.timeScale === 0, 'fullscreen/focus return preserves reading lock');
    await page.screenshot({path:report.outputDirectory + '/tutorial-fullscreen.png'});
    for (let n = 1; n <= 6; n++) {
      await page.keyboard.press('Enter', {delay:120});
      s = await until(game, s => n === 6 ? !s.tutorial : s.tutorialStep === n);
      await page.waitForTimeout(80);
    }
    s = await until(game, s => !s.tutorial && !s.dungeonTransitioning && s.hp > 0);
    check(s.tutorialStatus === 2 && s.inputLetters === 0, 'fullscreen Start Battle opens a fresh encounter');
    check(report.fixtureWrites === writes, 'iframe/fullscreen tutorial produces zero score writes');
  } catch(error) {
    await page.screenshot({path:report.outputDirectory + '/tutorial-embedding-failure.png'});
    throw error;
  } finally { await context.close(); }
};
