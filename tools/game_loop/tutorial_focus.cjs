const sessions = new WeakMap();

module.exports = async function focus(api, page, reading) {
  const {state, until, check, report} = api;
  let cdp = sessions.get(page);
  if (!cdp) { cdp = await page.context().newCDPSession(page); sessions.set(page, cdp); }
  await cdp.send('Emulation.setFocusEmulationEnabled', {enabled:false});
  const before = await state(page);
  const away = await page.context().newPage();
  try {
    await away.goto('about:blank'); await away.bringToFront();
    const actual = await page.evaluate(() => ({focused:document.hasFocus(), hidden:document.hidden}));
    check(!actual.focused || actual.hidden, 'tutorial focus loss is a real browser focus change');
    await page.waitForTimeout(700);
    const hidden = await state(page);
    report.tutorialHiddenPublicationAgeMs = hidden.qaAgeMs;
  } finally {
    await away.close(); await page.bringToFront();
    // Keep real focus enabled for all checks in this task-owned page. Its context
    // closes this observer; adding detached emulation overrides between checks
    // can otherwise make a later background page appear focused.
  }
  // Browsers may suspend a hidden tab's render loop, including bridge publication.
  // Verify a fresh state after return, before sending the explicit Resume input.
  const after = await until(page, s => s.frame > before.frame && s.qaAgeMs < 500 && s.paused && s.timeScale === 0);
  check(after.hp === before.hp && after.stamina === before.stamina && after.inputLetters === before.inputLetters &&
    after.slayer === before.slayer && Math.abs(after.wordTime - before.wordTime) < .25 &&
    after.combatSeconds - before.combatSeconds < .25, 'focus loss holds tutorial state and combat time');
  report.tutorialFocus = (report.tutorialFocus || 0) + 1;
  check(after.paused, 'focus return requires explicit tutorial resume');
  await page.keyboard.press('Escape', {delay:120});
  await until(page, s => !s.paused && !s.controls.some(c => c.name === 'ResumeButton') && s.timeScale === (reading ? 0 : 1));
};
