const { chromium } = require(process.env.TYPINGHERO_PLAYWRIGHT || '/tools/node_modules/playwright');
const http = require('node:http');
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');

const profile = process.argv[2];
const seconds = Number(process.argv[3] || 1800);
const report = { status: 'failed', profile, checks: 0, samples: [], rounds: 0, externalBlocked: 0, fixtureReads: 0, fixtureTopReads: 0, fixtureWrites: 0, exceptions: [] };
report.outputDirectory = process.env.TYPINGHERO_RESULTS_ROOT || '/results';
report.origin = process.env.TYPINGHERO_REMOTE_QA_HOST || `http://127.0.0.1:${process.env.TYPINGHERO_QA_PORT || 8123}`;
report.toolchain = JSON.parse(fs.readFileSync(path.join(process.env.TYPINGHERO_RUNNER_ROOT || '/runner', 'toolchain.json'), 'utf8'));
report.snapshot = JSON.parse(fs.readFileSync(path.join(report.outputDirectory, 'snapshot.json'), 'utf8'));
report.viewport = profile === 'stress' ? '960x600 and 400x600' : '960x600';
report.driverSha256 = require('node:crypto').createHash('sha256').update(fs.readFileSync(__filename)).digest('hex');
report.tutorialDriverSha256 = Object.fromEntries(['tutorial.cjs','tutorial_focus.cjs','tutorial_embedding.cjs','tutorial_sizes.cjs'].map(name =>
  [name, require('node:crypto').createHash('sha256').update(fs.readFileSync(path.join(__dirname,name))).digest('hex')]));
const started = performance.now();
const buildRoot=path.resolve(process.env.TYPINGHERO_BUILD_ROOT || '/build');
const check = (value, message) => { assert(value, message); report.checks++; };
const server = http.createServer((req, res) => {
  const pathname = new URL(req.url, 'http://localhost').pathname;
  if (pathname === '/__tutorial-embed.html') {
    res.setHeader('Content-Type', 'text/html');
    res.end('<style>body{margin:0;background:#152725}iframe{display:block;width:960px;height:540px;border:0}iframe:fullscreen{width:100vw;height:100vh}</style>' +
      '<button id="fullscreen" onclick="document.querySelector(\'iframe\').requestFullscreen()">Fullscreen</button><iframe name="game" src="/" allowfullscreen></iframe>');
    return;
  }
  const file = path.resolve(buildRoot, '.' + (pathname.endsWith('/') ? pathname + 'index.html' : pathname));
  if (!file.startsWith(buildRoot+path.sep)) { res.writeHead(403).end(); return; }
  const target = file;
  const types = { '.html': 'text/html', '.js': 'application/javascript', '.wasm': 'application/wasm', '.json': 'application/json', '.data': 'application/octet-stream' };
  res.setHeader('Content-Type', types[path.extname(target)] || 'application/octet-stream');
  res.setHeader('Cross-Origin-Opener-Policy', 'same-origin');
  res.setHeader('Cross-Origin-Embedder-Policy', 'require-corp');
  if (target === path.join(buildRoot,'index.html')) {
    // The QA host embeds the canvas at the requested viewport, as an iframe host
    // would. Game UI remains untouched; assert the actual Unity render size below.
    const hostStyle = '<style>html,body{margin:0;width:100%;height:100%;overflow:hidden}' +
      '#unity-container{position:fixed!important;inset:0!important;transform:none!important;width:100vw!important;height:100vh!important}' +
      '#unity-canvas{width:100vw!important;height:100vh!important}#unity-footer{display:none!important}</style>';
    fs.readFile(target, 'utf8', (error, html) => {
      if (error) res.writeHead(404).end();
      else res.end(html.replace('</head>', hostStyle + '</head>'));
    });
    return;
  }
  const stream = fs.createReadStream(target);
  stream.on('error', () => res.writeHead(404).end());
  stream.pipe(res);
});

async function state(page) {
  const s = await page.evaluate(() => ({ ...window.typingHeroQA,
    qaAgeMs: performance.now() - (window.typingHeroQAPublishedAt || performance.now()) }));
  assert(s && Number.isFinite(s.frame), 'Missing read-only QA state');
  report.lastState = s;
  assert.equal(s.errors, 0, 'Unity reported an error/exception/assert');
  return s;
}
async function until(page, predicate, timeout = 30000) {
  const deadline = performance.now() + timeout;
  while (performance.now() < deadline) {
    const s = await state(page);
    if (predicate(s)) return s;
    await page.waitForTimeout(100);
  }
  throw new Error('State deadline exceeded: ' + JSON.stringify(await state(page)));
}
async function clickControl(page, name, kind, input = page) {
  const s = await state(page);
  const p = s.controls.find(c => (!name || c.name === name) && (!kind || c.kind === kind));
  assert(p, 'Missing interactable control: ' + name);
  const box = await page.locator('#unity-canvas').boundingBox();
  assert(box, 'Unity canvas missing');
  await input.mouse.move(box.x + p.x * box.width, box.y + p.y * box.height);
  await input.mouse.down();
  try { await until(page, t => t.frame >= s.frame + 2); }
  finally { await input.mouse.up(); }
  await page.waitForTimeout(150);
}
async function start(page, skipTutorial = true) {
  await until(page, s => s.scene === 'MainMenu');
  if (!(await state(page)).controls.some(c => c.kind === 'input')) await clickControl(page, 'PlayButton', 'button');
  await until(page, s => s.controls.some(c => c.kind === 'input'));
  await clickControl(page, 'NameInput', 'input');
  await page.waitForTimeout(200);
  await page.keyboard.press('Control+A');
  await page.waitForTimeout(200);
  await page.keyboard.press('Backspace');
  await page.waitForTimeout(200);
  await page.keyboard.type('LoopQA', { delay: 120 });
  await page.waitForTimeout(250);
  const named = await state(page);
  check(named.controls.some(c => c.name === 'PlayButton'),
    'valid name enables Start');
  await page.keyboard.press('Enter');
  const ready = await until(page, s => s.scene === 'SampleScene' || s.controls.some(c => c.name === 'ContinueButton'));
  if (ready.scene !== 'SampleScene') await clickControl(page, 'ContinueButton', 'button');
  let s = await until(page, s => s.scene === 'SampleScene' && s.hp > 0 && s.current.length > 0);
  if (s.tutorial && skipTutorial) {
    await clickControl(page, 'PracticeButton', 'button');
    await until(page, s => s.tutorialParry);
    await clickControl(page, 'Start Battle \u00b7 EnterButton', 'button');
    s = await until(page, s => s.scene === 'SampleScene' && !s.tutorial && s.hp === 40 && s.score === 0);
  }
  check(s.playerName === 'LoopQA', 'name persisted into gameplay');
  return s;
}

async function session(browser, viewport, rows, embedded = false) {
  const context = await browser.newContext({ viewport, serviceWorkers: 'block',
    ...((process.env.TYPINGHERO_PIXEL_SPARKS_ONLY === '1' || process.env.TYPINGHERO_ENDLESS_ONLY === '1' || process.env.TYPINGHERO_ENDLESS_NETWORK_ONLY === '1' || process.env.TYPINGHERO_HUD_R7_ONLY === '1' || process.env.TYPINGHERO_PARRY_CLASH_ONLY === '1') ? {recordVideo:{dir:report.outputDirectory,size:viewport}} : {}) });
  const endlessEntries = new Map();
  await context.route('**/*', async route => {
    const req = route.request();
    const url = new URL(req.url());
    if (url.origin === report.origin) return route.continue();
    const headers = { 'access-control-allow-origin': '*', 'access-control-allow-methods': 'GET, PUT, PATCH, POST, OPTIONS',
      'access-control-allow-headers': '*' };
    if (url.hostname === 'typinghero-41b29-default-rtdb.asia-southeast1.firebasedatabase.app' && url.pathname.startsWith('/leaderboards/global')) {
      if (req.method() === 'OPTIONS') return route.fulfill({ status: 204, headers });
      let body = 'null';
      if (req.method() === 'GET') {
        report.fixtureReads++;
        if (url.pathname === '/leaderboards/global.json') {
          report.fixtureTopReads++;
          body = JSON.stringify(Object.fromEntries(Array.from({ length: rows }, (_, i) =>
            ['fixture' + i, { name: 'Player' + i, bestScore: (rows - i) * 10, updatedAt: 1700000000000 + i }])));
        }
      } else { report.fixtureWrites++; body = req.postData() || '{}'; }
      return route.fulfill({ status: 200, contentType: 'application/json', headers, body });
    }
    if (url.hostname === 'typinghero-41b29-default-rtdb.asia-southeast1.firebasedatabase.app' && /^\/leaderboards\/endless_v[12](?:[/.])/.test(url.pathname)) {
      headers['access-control-expose-headers']='ETag'; headers.ETag='"fixture-v1"';
      if(req.method()==='OPTIONS') return route.fulfill({status:204,headers});
      let body='null';
      report.endlessPaths ||= [];
      report.endlessPaths.push({method:req.method(),path:url.pathname});
      if(req.method()==='GET') {
        report.endlessReads=(report.endlessReads||0)+1;
        if(/^\/leaderboards\/(?:endless_v1|endless_v2\/(?:very_easy|easy|hard|very_hard))\.json$/.test(url.pathname)) {
          const level=url.pathname.includes('endless_v1')?'normal':url.pathname.split('/').at(-1).replace('.json','');
          const entries=Array.from({length:2100},(_,i)=>['endless'+i,{name:level+' Duelist'+i,defeated:i<900?12:6,score:i,updatedAt:1700000000000+i}]);
          const boundary=url.searchParams.has('equalTo');
          body=JSON.stringify(Object.fromEntries(boundary?entries.slice(900):[...entries.slice(0,900),...entries.slice(-100)]));
        } else body=JSON.stringify(endlessEntries.get(url.pathname)||null);
      } else if(req.method()==='PUT') {
        report.endlessWrites=(report.endlessWrites||0)+1;
        body=req.postData(); endlessEntries.set(url.pathname,JSON.parse(body));
        (report.endlessPayloads ||= []).push({path:url.pathname,...JSON.parse(body)});
      } else throw new Error('Unexpected Endless fixture method '+req.method());
      return route.fulfill({status:200,headers,contentType:'application/json',body});
    }
    report.externalBlocked++;
    await route.abort('blockedbyclient');
  });
  const page = await context.newPage();
  if(Number(process.env.TYPINGHERO_CPU_THROTTLE || 1)>1){
    const timingSession=await context.newCDPSession(page);
    const rate=Number(process.env.TYPINGHERO_CPU_THROTTLE);
    await timingSession.send('Emulation.setCPUThrottlingRate',{rate});
    report.cpuThrottlingRate=rate;
  }
  // Observe publication time without changing any value exposed by the game.
  await page.addInitScript(() => {
    let latest;
    Object.defineProperty(window, 'typingHeroQA', {
      configurable: true, get: () => latest,
      set: value => { latest = value; window.typingHeroQAPublishedAt = performance.now(); }
    });
  });
  if(process.env.TYPINGHERO_ENDLESS_AUDIO_ONLY==='1') await page.addInitScript(()=>{
    window.difficultyAudioStarts=[];
    const original=AudioBufferSourceNode.prototype.start;
    AudioBufferSourceNode.prototype.start=function(...args){
      if(this.buffer) window.difficultyAudioStarts.push({at:performance.now(),duration:this.buffer.duration});
      return original.apply(this,args);
    };
  });
  page.on('pageerror', error => { report.pageError = String(error); });
  page.on('console', message => {
    if (message.type() === 'error' && /Exception|Assertion|RuntimeError|abort\(/i.test(message.text()))
      report.exceptions.push(message.text());
  });
  await page.goto(report.origin + (embedded ? '/__tutorial-embed.html' : '/'));
  const observed = embedded ? page.frame({name:'game'}) : page;
  const expected = embedded ? {width:960,height:540} : viewport;
  await observed.waitForFunction(() => window.typingHeroQA?.frame > 0, null, { timeout: 120000 });
  const dimensions = await until(observed, s => s.width === expected.width && s.height === expected.height);
  report.graphics = await observed.evaluate(() => {
    const gl = document.getElementById('unity-canvas').getContext('webgl2');
    const debug = gl.getExtension('WEBGL_debug_renderer_info');
    return { renderer: gl.getParameter(debug ? debug.UNMASKED_RENDERER_WEBGL : gl.RENDERER) };
  });
  check(dimensions.width === expected.width && dimensions.height === expected.height, 'actual Unity render size matches viewport');
  return { context, page };
}
async function fullFlow(page, soak) {
  await modalFlow(page, false);
  let initial = await start(page);
  await page.keyboard.press('Escape');
  const paused = await until(page, s => s.paused && s.timeScale === 0);
  await page.waitForTimeout(400); // Observe a real interval with gameplay time stopped.
  const held = await state(page);
  check(held.paused && held.timeScale === 0 && Math.abs(held.wordTime - paused.wordTime) < .001,
    'pause stops the word timer');
  check(held.hp === paused.hp && held.score === paused.score, 'HP and score stay stable while paused');
  await modalFlow(page, true);
  await page.keyboard.press('Escape');
  await until(page, s => !s.paused && s.timeScale === 1);
  let lastProgress = performance.now(), lastWords = initial.words, lastParryAt = 0;
  let sawSlayer = false, sawParry = false, sawDamage = false, sawScore = false;
  let roundScored = false, roundSlayer = false;
  const end = performance.now() + (soak ? seconds * 1000 : 180000);
  let nextSample = 0;
  while (performance.now() < end) {
    const s = await state(page);
    assert(s.scene === initial.scene, 'Combat keyboard input navigated out of the match');
    assert(!report.pageError, report.pageError);
    sawSlayer ||= s.slayer;
    roundSlayer ||= s.slayer;
    sawParry ||= s.parries > initial.parries;
    sawDamage ||= (s.hp > 0 && s.hp < initial.hp) || s.enemyHp < initial.enemyHp;
    sawScore ||= s.score > 0;
    roundScored ||= s.score > 0;
    if (s.words > lastWords) { lastWords = s.words; lastProgress = performance.now(); }
    if (s.overlay) {
      check(roundScored, 'completed round made scoring progress');
      report.rounds++;
      await page.keyboard.press('r');
      const restarted = await until(page, t => !t.overlay && t.hp > 0 && t.hp === t.maxHp && t.enemyHp > 0 && t.score === 0 && t.scene === initial.scene);
      check(restarted.score === 0 && restarted.hp === restarted.maxHp, 'restart restores round');
      roundScored = false;
      roundSlayer = false;
      // Restarting by itself is not gameplay progress: only completed words
      // reset the watchdog, so a loop of dead rounds cannot pass a soak.
      if (!soak && sawScore && sawDamage && sawParry && sawSlayer) break;
    } else if (!(roundSlayer && roundScored && sawParry)) {
      // skeleton_attack_heavy: strike .9833 s / clip 1.3333 s; player parry
      // opens at 0 and closes at .25 s. Press near .8 s of the enemy clip.
      const attackPhase = s.enemyAnimationTime % 1;
      const impactIn = 1 - (attackPhase * 1.3333334 + s.qaAgeMs / 1000 * s.timeScale);
      if (s.canParry && s.enemyAttacking && impactIn <= 2 * s.frameMs / 1000 + .12 && impactIn > .04 && performance.now() - lastParryAt > 250) {
        await page.keyboard.press('Space', { delay: 100 });
        lastParryAt = performance.now();
      }
      // The serialized enemy has 250 stamina and each parry drains 25. Keep
      // observing every strike until Slayer, rather than blocking on a word.
      const watchParry = s.enemyAttacking && attackPhase >= .25 && attackPhase <= .9;
      if (s.current && !watchParry) await page.keyboard.type(s.current, { delay: 35 });
    }
    // After observing Slayer and score, allow real enemy attacks to end the
    // round. Continuing to parry forever would never exercise round/restart.
    assert(performance.now() - lastProgress < (roundSlayer && roundScored && sawParry ? 45000 : 30000), 'Gameplay exceeded its word-progress or eight-hit death deadline');
    if (performance.now() >= nextSample) {
      report.samples.push({ elapsed: (performance.now() - started) / 1000, frame: s.frame,
        words: s.words, score: s.score, hp: s.hp, enemyHp: s.enemyHp, parries: s.parries,
        slayer: s.slayer, paused: s.paused, timeScale: s.timeScale, current: s.current,
        canParry: s.canParry, enemyAnimationTime: s.enemyAnimationTime,
        frameMs: s.frameMs, memory: s.memory, objects: s.objects });
      if (report.samples.length % 10 === 0)
        console.log(JSON.stringify({ stage: 'gameplay', ...report.samples.at(-1), rounds: report.rounds }));
      nextSample = performance.now() + 1000;
    }
    await page.waitForTimeout(100);
  }
  check(sawScore, 'typing increased score');
  check(sawDamage, 'combat changed HP');
  check(sawParry, 'successful parry observed');
  check(sawSlayer, 'Slayer entered through gameplay');
  check(report.rounds > 0, 'round ended and restarted');
  if (soak) check(performance.now() >= end, '1800 wall-clock seconds completed');
}
async function focusFlow(page) {
  const focusSession = await page.context().newCDPSession(page);
  await focusSession.send('Emulation.setFocusEmulationEnabled', { enabled: false });
  const away = await page.context().newPage();
  await away.goto('about:blank'); await away.bringToFront();
  report.focusEnvironment = await page.evaluate(() => ({ focused: document.hasFocus(), hidden: document.hidden }));
  console.log(JSON.stringify({ stage: 'focus-environment', ...report.focusEnvironment }));
  check(!report.focusEnvironment.focused || report.focusEnvironment.hidden, 'browser focus precondition is real rather than emulated');
  await page.waitForTimeout(300);
  const unfocused = await state(page);
  await page.waitForTimeout(400);
  let s = await state(page);
  check(s.combatSeconds === unfocused.combatSeconds && s.hp === unfocused.hp && s.stamina === unfocused.stamina,
    'a real tab focus change pauses combat, time and stamina');
  await away.close(); await page.bringToFront();
  await focusSession.send('Emulation.setFocusEmulationEnabled', { enabled: true });
  await focusSession.detach();
  await until(page, t => t.paused, 5000);
  check((await state(page)).paused, 'focus return waits for explicit resume');
  await page.keyboard.press('Escape');
  s = await until(page, t => !t.paused && t.timeScale === 1);
  report.focusEnvironment.verified = true;
}

async function coreFlow(page) {
  await page.keyboard.press('Escape');
  let s = await until(page, t => t.paused);
  const paused = s;
  await page.keyboard.type('type');
  await page.waitForTimeout(650);
  s = await state(page);
  check(s.inputLetters === paused.inputLetters && s.combatSeconds === paused.combatSeconds && s.stamina === paused.stamina,
    'pause freezes letter statistics, combat time and stamina');
  await clickControl(page, 'MusicVolume');
  await clickControl(page, 'SFXVolume');
  s = await until(page, t => t.musicVolume > .35 && t.musicVolume < .65 && t.sfxVolume > .35 && t.sfxVolume < .65);
  const music = s.musicVolume, sfx = s.sfxVolume;
  await page.keyboard.press('m');
  await until(page, t => t.scene === 'MainMenu');
  check(Math.abs((await state(page)).musicVolume - music) < .001 && Math.abs((await state(page)).sfxVolume - sfx) < .001,
    'audio preferences survive scene return');
  await page.reload();
  await page.waitForFunction(() => window.typingHeroQA?.frame > 0, null, { timeout: 120000 });
  s = await until(page, t => t.scene === 'MainMenu');
  check(s.helpSeen && Math.abs(s.musicVolume - music) < .001 && Math.abs(s.sfxVolume - sfx) < .001,
    'browser reload preserves audio preferences and viewed tutorial');
  await clickControl(page, 'How to PlayButton');
  await until(page, t => t.helpOpen);
  await clickControl(page, 'ContinueButton');
  s = await until(page, t => !t.helpOpen);
  check(s.scene === 'MainMenu' && s.helpSeen, 'menu reopens and closes saved How to Play without starting combat');
  s = await start(page);
  check(s.hp === 40 && s.enemyHp > 0 && s.inputLetters === 0 && s.matchParries === 0, 'new match resets health and statistics');
  if (process.env.TYPINGHERO_HEADED === '1') await focusFlow(page);
  else report.focusCoverage = 'requires separate headed browser focus check';
  while (s.current.length < 3) {
    await page.keyboard.type(s.current);
    s = await until(page, t => t.current.length >= 3 && t.current !== s.current);
  }
  const word = s.current;
  await page.keyboard.type(word[0]);
  s = await until(page, t => t.current === word.slice(1));
  const beforeError = s.wordTime;
  await page.keyboard.type(s.current[0] === 'z' ? 'x' : 'z');
  s = await until(page, t => t.current === word);
  check(s.wordTime <= beforeError, 'wrong key restores whole word without refilling timer');
  await page.keyboard.press('Escape');
  await until(page, t => t.paused);
  await page.keyboard.press('r');
  s = await until(page, t => !t.paused && t.score === 0 && t.inputLetters === 0 && t.hp === 40 && t.current.length > 0);
  const enemyBeforeQueue = s.enemyHp;
  const queueWord = s.current;
  if (queueWord.length > 1) {
    await page.keyboard.type(queueWord.slice(0, -1), { delay: 30 });
    await until(page, t => t.current === queueWord.slice(-1));
  }
  await page.keyboard.press('Space', { delay: 40 });
  s = await until(page, t => t.parryPose);
  await page.keyboard.type(queueWord.slice(-1));
  await until(page, t => t.pendingAttack);
  await page.keyboard.press('Escape');
  s = await until(page, t => t.paused);
  await page.waitForTimeout(450);
  s = await state(page);
  check(s.pendingAttack && s.enemyHp === enemyBeforeQueue, 'completed word waits through parry and pause without attacking');
  await page.keyboard.press('Escape');
  await until(page, t => !t.paused && t.timeScale === 1);
  s = await until(page, t => !t.pendingAttack && !t.parryPose && t.enemyHp < enemyBeforeQueue);
  await page.waitForTimeout(350);
  check((await state(page)).enemyHp === enemyBeforeQueue - 10, 'parry releases exactly one queued normal slash');
  const damagedEnemy = await until(page,t=>Math.abs(t.enemyHpFill-t.enemyHp/850)<.002);
  check(damagedEnemy.enemyHpFill<1,'relocated enemy HP bar reflects the real slash');
  await page.keyboard.press('Escape');
  await until(page, t => t.paused);
  await page.keyboard.press('r');
  await until(page, t => !t.paused && t.score === 0 && t.inputLetters === 0 && t.hp === 40);
  // Observe actual cue/impact timestamps across eight keyboard-idle swings.
  let previousHp = 40, cueAt = null, cueWasVisible = false, impacts = 0;
  const telegraphs = [], deathDeadline = performance.now() + 45000;
  do {
    s = await state(page);
    if (s.cue && !cueWasVisible) cueAt = s.combatSeconds;
    cueWasVisible = s.cue;
    if (s.hp >= 0 && s.hp < previousHp) {
      check(previousHp - s.hp === 5, 'one normal impact removes exactly five HP');
      const damagedPlayer = await until(page,t=>Math.abs(t.playerHpFill-t.hp/t.maxHp)<.002);
      check(damagedPlayer.playerHpFill<1,'relocated player HP bar reflects real incoming damage');
      if (cueAt !== null) telegraphs.push(s.combatSeconds - cueAt);
      impacts++; previousHp = s.hp; cueAt = null;
    }
    if (!s.finished) await page.waitForTimeout(25);
  } while (!s.finished && performance.now() < deathDeadline);
  report.telegraphSeconds = telegraphs;
  check(impacts === 8 && telegraphs.length === 8, 'eight swings each show a cue and exactly one impact');
  const medianLead = [...telegraphs].sort((a,b) => a-b)[Math.floor(telegraphs.length / 2)];
  check(telegraphs.every(t => t > 0 && t < .6) && medianLead >= .15 && medianLead <= .45, 'cue precedes every impact and median lead is approximately 300ms under 100ms frame sampling');
  check(s.hp === 0 && s.inputLetters === 0 && s.finalScore === 0, 'eight unblocked swings finish an untouched 40 HP round');
  check(s.result.includes('ACCURACY \u2014'), 'no-letter accuracy displays an em dash');
  const frozen = s;
  await page.keyboard.type('typewords');
  await until(page, t => t.overlay, 5000);
  s = await state(page);
  check(s.inputLetters === frozen.inputLetters && s.score === frozen.score && s.combatSeconds === frozen.combatSeconds,
    'lethal hit freezes results through death animation and overlay');
  check(Math.abs(s.musicVolume - music) < .001 && Math.abs(s.sfxVolume - sfx) < .001, 'restart retains audio preferences');
}

async function referenceBalance(page) {
  report.balance = [];
  for (let round = 0; round < 3; round++) {
    let s = await state(page);
    if (s.scene === 'SampleScene') {
      if (!s.overlay && !s.paused) await page.keyboard.press('Escape');
      await until(page, t => t.overlay || t.paused);
      await page.keyboard.press('r');
      s = await until(page, t => !t.overlay && !t.paused && t.hp === 40 && t.score === 0 && t.combatSeconds < 2);
    } else s = await start(page);
    const startHp = s.enemyHp;
    let nextLetter = performance.now(), attempts = 0, swings = 0, attemptedParries = 0;
    let wasAttacking = false, parriedThisSwing = false, previousPhase = 0;
    const parryAttempts = [];
    const trace = [], deadline = performance.now() + 240000;
    while (!s.finished && performance.now() < deadline) {
      const phase = s.enemyAnimationTime % 1;
      if (s.enemyAttacking && (!wasAttacking || phase < previousPhase - .4)) { swings++; parriedThisSwing = false; }
      wasAttacking = s.enemyAttacking; previousPhase = phase;
      const resolved = s.matchParries + (40 - s.hp) / 5;
      const impactIn = 1 - (phase * 1.3333334 + s.qaAgeMs / 1000 * s.timeScale);
      const wantParry = s.matchParries < Math.round(.8 * (resolved + 1));
      const inputLead = 2 * s.frameMs / 1000 + .12;
      if (s.enemyAttacking && wantParry && !parriedThisSwing && s.canParry && impactIn <= inputLead && impactIn > .04) {
        parryAttempts.push({ time: s.combatSeconds, phase, ageMs: s.qaAgeMs, impactIn, hp: s.hp, parries: s.matchParries });
        await page.keyboard.press('Space', { delay: 80 });
        attemptedParries++; parriedThisSwing = true;
      }
      if (performance.now() >= nextLetter) {
        const remaining = s.current || s.next.slice(s.nextProgress);
        if (remaining) {
          attempts++;
          const expected = remaining[0].toLowerCase();
          const letter = attempts % 20 === 0 ? (expected === 'z' ? 'x' : 'z') : expected;
          await page.keyboard.type(letter);
        }
        nextLetter += 60000 / (45 * 5);
        if (nextLetter < performance.now() - 267) nextLetter = performance.now();
      }
      if (trace.at(-1)?.frame !== s.frame) trace.push({ frame: s.frame, time: s.combatSeconds,
        damage: startHp - s.enemyHp, hp: s.hp, phase, parryWindow: s.parryWindow, parryPose: s.parryPose,
        parries: s.matchParries, enemyAttacking: s.enemyAttacking, ageMs: s.qaAgeMs, frameMs: s.frameMs });
      await page.waitForTimeout(25);
      s = await state(page);
    }
    check(s.finished, 'reference round reaches a lethal hit within 240s');
    const outcome = { round: round + 1, enemyStartHp: startHp, won: s.hp > 0, seconds: s.combatSeconds,
      accuracy: s.inputCorrect / s.inputLetters, inputWpm: s.inputLetters / 5 / (s.combatSeconds / 60),
      parries: s.matchParries, swings, attemptedParries, parryRate: s.matchParries / (s.matchParries + (40 - s.hp) / 5), parryAttempts,
      finalScore: s.finalScore, trace };
    report.balance.push(outcome);
    console.log(JSON.stringify({ stage: 'reference-balance', ...outcome, trace: undefined }));
    check(outcome.won, '45 WPM reference player wins');
    check(s.result.includes('SCORE ' + s.finalScore) && s.result.includes('PARRIES ' + s.matchParries), 'victory result contains the frozen score and successful parry total');
    check(outcome.accuracy >= .94 && outcome.accuracy <= .96, 'reference accuracy is approximately 95%');
    check(outcome.inputWpm >= 42 && outcome.inputWpm <= 48, 'reference input is approximately 45 WPM');
    check(outcome.parryRate >= .70 && outcome.parryRate <= .90, 'reference successful parry rate is approximately 80%');
    await until(page, t => t.overlay);
  }
  const candidates = [];
  for (let hp = 50; hp <= report.balance[0].enemyStartHp; hp += 50) {
    const times = report.balance.map(r => r.trace.find(t => t.damage >= hp)?.time || (hp === r.enemyStartHp ? r.seconds : null));
    if (times.every(t => t !== null)) candidates.push({ hp, medianSeconds: [...times].sort((a,b) => a-b)[1] });
  }
  report.balanceCandidates = candidates.sort((a,b) => Math.abs(a.medianSeconds - 90) - Math.abs(b.medianSeconds - 90));
  report.balanceMedian = report.balance.map(r => r.seconds).sort((a,b) => a-b)[1];
}

async function leaderboard(page, rows) {
  await start(page);
  await page.keyboard.press('Escape');
  await until(page, s => s.paused && s.timeScale === 0);
  await clickControl(page, 'Top 1000Button', 'button');
  const s = await until(page, s => s.dataRows === rows);
  const pool = s.pool;
  const rowIds = [...s.rowIds].sort((a, b) => a - b);
  check(s.dataRows === rows && s.rows > 0 && s.rows <= pool, 'full fixture dataset bound with visible rows');
  if (s.virtualized) {
    check(pool <= Math.ceil(s.viewportHeight / s.rowHeight) + 4 && pool < rows,
      'virtualized leaderboard pool stays bounded by viewport plus overscan');
  } else check(pool === rows, 'nonvirtualized leaderboard pool covers dataset');
  // Real wheel input inside the serialized ScrollRect viewport.
  const control = s.controls.find(c => c.kind === 'scroll');
  assert(control, 'Leaderboard ScrollRect viewport missing');
  const box = await page.locator('#unity-canvas').boundingBox();
  await page.mouse.move(box.x + control.x * box.width, box.y + control.y * box.height);
  // Unity's legacy input maps each browser wheel event to a scroll notch;
  // a large DOM delta alone does not traverse a 1,000-row content area.
  for (let notch = 0; notch < 100 && (await state(page)).scroll >= .95; notch++) {
    await page.mouse.wheel(0, 650);
    await page.waitForTimeout(120);
  }
  const scrolled = await until(page, t => t.scroll < .95);
  if (s.virtualized) check(scrolled.firstVisible > s.firstVisible, 'wheel input changes the rendered dataset window');
  const beforeReads = report.fixtureTopReads;
  await until(page, () => report.fixtureTopReads > beforeReads, 20000);
  await page.waitForTimeout(500); // allow the observed refresh's UI layout to finish
  const after = await state(page);
  check(after.pool === pool && after.dataRows === rows && after.rows > 0, 'refresh reuses row objects for the full dataset');
  check(JSON.stringify([...after.rowIds].sort((a, b) => a - b)) === JSON.stringify(rowIds), 'same leaderboard instances survive refresh');
  check(Math.abs(after.scroll - scrolled.scroll) < .03, 'refresh retains scroll position');
}

async function modalFlow(page, paused) {
  const before = await state(page);
  await clickControl(page, 'Top 1000Button', 'button');
  const open = await until(page, s => s.leaderboardOpen && s.dataRows === 1000 && s.rows > 0);
  check(open.pool <= Math.ceil(open.viewportHeight / open.rowHeight) + 4, 'modal virtualizes all 1000 rows');
  await page.keyboard.press('r', { delay: 120 }); await page.keyboard.press('m', { delay: 120 });
  await page.waitForTimeout(250);
  const held = await state(page);
  check(held.leaderboardOpen && held.scene === before.scene && held.paused === before.paused,
    'R/M shortcuts cannot escape leaderboard modal');
  if (paused) check(held.hp === before.hp && held.combatSeconds === before.combatSeconds && held.timeScale === 0,
    'opening leaderboard from pause keeps combat frozen');
  const thumb = open.controls.find(c => c.name === 'Scrollbar');
  assert(thumb, 'modal scrollbar control missing');
  const box = await page.locator('#unity-canvas').boundingBox();
  const half = open.viewportHeight * Math.min(box.width / 960, box.height / 540) / 2;
  const x = box.x + thumb.x * box.width, y = box.y + thumb.y * box.height;
  await page.mouse.move(x, y - half + 8); await page.mouse.down();
  await page.mouse.move(x, y + half - 1, { steps: 15 }); await page.mouse.up();
  const bottom = await until(page, s => s.scroll < .002 && s.firstVisible > 970);
  check(bottom.firstVisible > 970, 'native scrollbar reaches rank 1000');
  const reads = report.fixtureTopReads;
  await page.waitForTimeout(1200); // Existing provider's one-second fetch throttle.
  await clickControl(page, 'RefreshButton', 'button');
  await until(page, () => report.fixtureTopReads > reads);
  const refreshed = await state(page);
  check(refreshed.dataRows === 1000 && refreshed.pool === open.pool && refreshed.scroll < .002,
    'explicit refresh preserves full dataset, pool and bottom scroll');
  await page.keyboard.press('Escape', { delay: 120 });
  const closed = await until(page, s => !s.leaderboardOpen);
  await page.waitForTimeout(150); // Observe key release before the next screen's shortcut.
  check(closed.paused === before.paused && closed.scene === before.scene, 'Escape returns to prior screen');
  if (paused) check(closed.timeScale === 0, 'modal Escape does not resume paused match');
}

(async () => {
  if (!process.env.TYPINGHERO_REMOTE_QA_HOST)
    await new Promise(resolve => server.listen(Number(new URL(report.origin).port), '127.0.0.1', resolve));
  let browser;
  try {
    const native = process.env.TYPINGHERO_NATIVE === '1';
    const options = { headless: process.env.TYPINGHERO_HEADED !== '1',
      ...(native ? {channel:'msedge'} : {}),
      args: ['--no-sandbox', '--enable-webgl', '--use-gl=angle',
        ...(native ? ['--use-angle=d3d11'] : ['--use-angle=swiftshader','--enable-unsafe-swiftshader'])] };
    if (native) {
      const context = await chromium.launchPersistentContext(path.join(report.outputDirectory,'edge-profile'), {
        ...options, downloadsPath:path.join(report.outputDirectory,'downloads'), tracesDir:path.join(report.outputDirectory,'traces')});
      browser = context.browser();
    } else browser = await chromium.launch(options);
    report.chromium = browser.version();
    if (process.env.TYPINGHERO_CAMERA_IMPACT_ONLY === '1') {
      report.scenario='camera-impact'; report.viewport='1280x720 and 640x360';
      await require('./camera_impact.cjs')({session,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_TUTORIAL_ONLY === '1') {
      report.scenario='tutorial';
      await require('./tutorial_sizes.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_HUD_FEEDBACK_ONLY === '1') {
      report.scenario='score-cluster';
      await require('./combat_hud_feedback.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_SCORE_CLUSTER_ONLY === '1') {
      report.scenario='score-cluster';
      await require('./score_cluster.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_PARRY_CLASH_ONLY === '1') {
      report.scenario='parry-clash';
      await require('./parry_clash.cjs')({session,start,state,until,check,report},browser);
    } else if (process.env.TYPINGHERO_HUD_R7_ONLY === '1') {
      report.scenario='combat-r7';
      const {context,page}=await session(browser,{width:960,height:540},1000);
      try {
        await page.emulateMedia({reducedMotion:process.env.TYPINGHERO_REDUCED_MOTION==='1'?'reduce':'no-preference'});
        await start(page,false);
        await require('./tutorial.cjs')({state,until,clickControl,check,report,
          focus:(p,reading)=>require('./tutorial_focus.cjs')({state,until,check,report},p,reading)},page,{
          freshTutorial:true,viewports:[[320,180],[640,360],[1280,720],[400,300]],captureGrayscale:true,r7Input:true
        });
      } finally {await context.close();}
    } else if (process.env.TYPINGHERO_ENDLESS_AUDIO_ONLY === '1') {
      report.scenario='endless-typing-audio';
      await require('./difficulty_audio.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_ENDLESS_UI_ONLY === '1') {
      report.scenario='endless-ui';
      await require('./difficulty_ui.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_ENDLESS_NETWORK_ONLY === '1') {
      report.scenario='endless-network';
      report.difficultyNetworkDriverSha256=require('node:crypto').createHash('sha256').update(fs.readFileSync(path.join(__dirname,'difficulty_network.cjs'))).digest('hex');
      await require('./difficulty_network.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_ENDLESS_ONLY === '1' ||
        (profile === 'webgl' && !Object.keys(process.env).some(k => k.startsWith('TYPINGHERO_') && k.endsWith('_ONLY') && process.env[k] === '1'))) {
      report.scenario='endless';
      report.viewport='960x540; intro resize640x360';
      report.endlessDriverSha256=require('node:crypto').createHash('sha256').update(fs.readFileSync(path.join(__dirname,'endless.cjs'))).digest('hex');
      await require('./endless.cjs')({session,start,state,until,clickControl,check,report},browser);
    } else if (process.env.TYPINGHERO_PIXEL_SPARKS_ONLY === '1') {
      report.scenario='pixel-sparks'; report.viewport='1280x720 and 640x360';
      report.pixelSparksDriverSha256=require('node:crypto').createHash('sha256').update(fs.readFileSync(path.join(__dirname,'pixel_sparks.cjs'))).digest('hex');
      await require('./pixel_sparks.cjs')({session,start,state,until,check,report},browser);
    } else if (process.env.TYPINGHERO_SCORE_LAYOUT_ONLY === '1') {
      report.scenario='score-layout';
      const {context,page}=await session(browser,{width:960,height:600},1000);
      try {
        await start(page, false);
        await require('./tutorial.cjs')({state,until,clickControl,check,report},page,{stopAfterSlayerLayout:true,freshTutorial:true});
      } finally {await context.close();}
      await require('./slayer.cjs')({session,start,state,until,check,report},browser);
      const victory=await session(browser,{width:960,height:540},1000);
      try { await start(victory.page); await require('./hud_victory.cjs')({state,until,check,report},victory.page); }
      finally { await victory.context.close(); }
      const board=await session(browser,{width:960,height:540},1000);
      try { await leaderboard(board.page,1000); } finally { await board.context.close(); }

    } else if (process.env.TYPINGHERO_ATTACK_SORTING_ONLY === '1') {
      report.scenario='attack-sorting';
      await require('./attack_sorting.cjs')({session,start,state,until,check,report},browser);
    } else if (process.env.TYPINGHERO_SKELETON_ART_ONLY === '1') {
      report.viewport='1280x720 and 640x360';
      report.scenario='skeleton-art';
      await require('./skeleton_art.cjs')({session,start,state,until,check,report},browser);
    } else if (process.env.TYPINGHERO_HUD_ONLY === '1') {
      const {context,page}=await session(browser,{width:960,height:540},1000);
      try {
        await start(page);
        await require('./hud_victory.cjs')({state,until,check,report},page);
      } finally {await context.close();}
    } else if (process.env.TYPINGHERO_SIZES_ONLY === '1') {
      await require('./tutorial_sizes.cjs')({session, start, state, until, check, report}, browser);
    } else if (process.env.TYPINGHERO_SLAYER_ONLY === '1') {
      await require('./slayer.cjs')({session,start,state,until,check,report}, browser);
    } else if (process.env.TYPINGHERO_EMBED_ONLY === '1') {
      await require('./tutorial_embedding.cjs')({session, state, until, clickControl, check, report}, browser);
    } else if (profile === 'stress') {
      for (const viewport of [{ width: 960, height: 600 }, { width: 400, height: 600 }]) {
        for (const rows of [500, 1000]) {
          const { context, page } = await session(browser, viewport, rows);
          try { await leaderboard(page, rows); } finally { await context.close(); }
        }
      }
    } else {
      const { context, page } = await session(browser, { width: 960, height: 600 }, 1000);
      try {
        if (profile === 'webgl') {
          const api = {state, until, clickControl, check, report,
            focus: (p, reading) => require('./tutorial_focus.cjs')({state, until, check, report}, p, reading)};
          await require('./tutorial.cjs')(api, page);
        }
        if (process.env.TYPINGHERO_FOCUS_ONLY === '1') { await start(page); await focusFlow(page); }
        else if (process.env.TYPINGHERO_TUTORIAL_ONLY !== '1') {
        if (process.env.TYPINGHERO_CORE_ONLY === '1') await start(page);
        else await fullFlow(page, profile === 'soak');
        if (profile === 'webgl' && typeof (await state(page)).combatSeconds === 'number') { if (process.env.TYPINGHERO_BALANCE_ONLY !== '1') await coreFlow(page); await referenceBalance(page); }
        }
      } finally { await context.close(); }
      if (profile === 'webgl') await require('./tutorial_embedding.cjs')({session, state, until, clickControl, check, report}, browser);
    }
    if (profile === 'webgl' && process.env.TYPINGHERO_PARRY_CLASH_ONLY !== '1' && !['endless','endless-network','endless-ui','endless-typing-audio','score-cluster','tutorial','camera-impact'].includes(report.scenario) && process.env.TYPINGHERO_PIXEL_SPARKS_ONLY !== '1' && process.env.TYPINGHERO_SLAYER_ONLY !== '1' && process.env.TYPINGHERO_SKELETON_ART_ONLY !== '1' && process.env.TYPINGHERO_ATTACK_SORTING_ONLY !== '1' && process.env.TYPINGHERO_SCORE_LAYOUT_ONLY !== '1') await require('./slayer.cjs')({session,start,state,until,check,report}, browser);
    check(!report.pageError, 'no JavaScript exceptions');
    check(report.exceptions.length === 0, 'no startup or runtime exceptions in console');
    report.status = 'passed';
  } catch (error) {
    report.error = String(error.stack || error);
    process.exitCode = 1;
  } finally {
    report.elapsedSeconds = (performance.now() - started) / 1000;
    if (report.samples.length) {
      const range = key => {
        const values = report.samples.map(s => s[key]).sort((a, b) => a - b);
        return { min: values[0], max: values.at(-1), p50: values[Math.floor((values.length - 1) * .5)],
          p95: values[Math.floor((values.length - 1) * .95)] };
      };
      report.summary = { sampleCount: report.samples.length, frameMs: range('frameMs'),
        memory: range('memory'), objects: range('objects'), words: range('words') };
    }
    fs.writeFileSync(path.join(report.outputDirectory, 'browser.json'), JSON.stringify(report, null, 2));
    if (browser) await browser.close();
    if (server.listening) server.close();
    console.log(JSON.stringify({ status: report.status, checks: report.checks, rounds: report.rounds, error: report.error }));
  }
})();
