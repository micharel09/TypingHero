module.exports = async function tutorialSizes(api, browser) {
  const {session, start, state, until, check, report} = api;
  const writes = report.fixtureWrites;
  report.tutorialSizes = [];
  for (const viewport of [{width:960,height:540},{width:1280,height:720},
    {width:1920,height:1080},{width:640,height:360}]) {
    const {context, page} = await session(browser, viewport, 1000);
    try {
      await until(page, s => s.frame >= 3);
      let s = await start(page, false);
      check(s.tutorial && s.current === 'BLADE' && s.timeScale === 0,
        'first lesson reads at ' + viewport.width + 'x' + viewport.height);
      await page.keyboard.press('Space', {delay:120});
      await page.keyboard.type('BLADE', {delay:120});
      const frame = s.frame;
      s = await until(page, s => s.frame > frame + 2);
      check(!s.tutorialPracticing && s.enemyHp === 850 && s.inputLetters === 0,
        'reading blocks Space and letters at ' + viewport.width + 'x' + viewport.height);
      await page.keyboard.down('Enter');
      try { await until(page, s => s.tutorialPracticing); }
      finally { await page.keyboard.up('Enter'); }
      await page.keyboard.type('BLADE', {delay:120});
      s = await until(page, s => s.tutorialPassed);
      check(s.enemyHp === 840 && s.hp === 40 && s.inputLetters === 5 &&
        s.width === viewport.width && s.height === viewport.height,
        'real keyboard slash hits at ' + viewport.width + 'x' + viewport.height);
      await page.screenshot({path:report.outputDirectory + '/tutorial-keyboard-' + viewport.width + 'x' + viewport.height + '.png'});
      report.tutorialSizes.push({...viewport,enemyHp:s.enemyHp,hp:s.hp,letters:s.inputLetters});
    } finally { await context.close(); }
  }
  check(report.fixtureWrites === writes, 'four-size tutorial keyboard checks produce zero score writes');
};
