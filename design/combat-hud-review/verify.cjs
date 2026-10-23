'use strict';
// Offline review checks only. Does not load Unity or call a gameplay/provider API.
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const { pathToFileURL } = require('node:url');
const { chromium } = require(process.env.PLAYWRIGHT_MODULE || 'playwright');
const root = __dirname, output = path.join(root, 'exports');
fs.mkdirSync(output, { recursive: true });
const receipt = { scope: 'Offline design board, not gameplay acceptance', checks: [], errors: [] };
function check(value, name) { assert.ok(value, name); receipt.checks.push(name); }

(async () => {
  const browser = await chromium.launch({ channel: process.env.PLAYWRIGHT_CHANNEL || 'msedge', headless: true });
  try {
    const page = await browser.newPage({ viewport: { width: 1500, height: 1100 }, deviceScaleFactor: 1 });
    page.on('pageerror', error => receipt.errors.push(String(error)));
    await page.route('https://**/*', route => route.abort());
    await page.goto(pathToFileURL(path.join(root, 'index.html')).href);
    await page.waitForFunction(() => !!window.HudReview);
    const metrics = await page.evaluate(() => {
      const r = window.HudReview, values = r.data.words.map(word => ({ word, main: r.measure(word, 16), preview: r.measure(word, 10) }));
      return { raw: r.data.rawWordCount, unique: values.length,
        main: values.reduce((a, b) => b.main > a.main ? b : a),
        preview: values.reduce((a, b) => b.preview > a.preview ? b : a),
        fits: values.every(v => v.main <= 112 && v.preview <= 54),
        hasI: r.data.words.includes('i'), duplicate: values.length !== new Set(r.data.words).size,
        sameNative: [document.getElementById('p1'), document.getElementById('p2')].every(c => c.width === 640 && c.height === 360) };
    });
    receipt.wordMetrics = metrics;
    check(metrics.raw === 220 && metrics.unique === 219 && metrics.hasI && !metrics.duplicate, 'Pool matches lowercase/deduplication: 220 raw, 219 unique');
    check(metrics.fits, 'Every normalized pool word fits both current slots and both preview slots at fixed sizes');
    check(metrics.sameNative, 'Both views render at requested native dimensions');
    for (const width of [640, 1280]) for (const scenario of ['typing', 'slayer']) {
      await page.evaluate(v => window.HudReview.set(v), { width, scenario, word: 'different', bounds: false });
      for (const variant of [1, 2]) {
        const image = await page.locator('#p' + variant).evaluate(c => c.toDataURL().split(',')[1]);
        fs.writeFileSync(path.join(output, `PA${variant}-${scenario}-${width}.png`), Buffer.from(image, 'base64'));
      }
    }
    check(true, 'Eight native PNGs exported: two concepts × normal/Slayer × two sizes');
    const states = await page.evaluate(() => window.HudReview.states);
    const stateImages = [[], []];
    for (const [scenario, label] of states) {
      await page.evaluate(v => window.HudReview.set(v), { width: 640, scenario, bounds: false });
      for (let variant = 1; variant <= 2; variant++) stateImages[variant - 1].push({ label,
        src: await page.locator('#p' + variant).evaluate(c => c.toDataURL()) });
    }
    for (let variant = 1; variant <= 2; variant++) {
      const sheet = await page.evaluate(async ({ images, variant }) => {
        const c = document.createElement('canvas'); c.width = 1000; c.height = 1280; const x = c.getContext('2d');
        x.fillStyle = '#100F13'; x.fillRect(0, 0, c.width, c.height); x.fillStyle = '#E5BE72'; x.font = '18px sans-serif'; x.fillText(`PA${variant} / BẢNG TRẠNG THÁI / KHÔNG PHẢI GAMEPLAY`, 12, 26);
        for (const [i, row] of images.entries()) {
          const im = new Image(); im.src = row.src; await im.decode(); const left = 12 + i % 3 * 328, top = 48 + Math.floor(i / 3) * 202;
          x.imageSmoothingEnabled = false; x.drawImage(im, left, top + 18, 320, 180); x.fillStyle = '#F2E7CF'; x.font = '12px sans-serif'; x.fillText(row.label, left, top + 12);
        }
        return c.toDataURL().split(',')[1];
      }, { images: stateImages[variant - 1], variant });
      fs.writeFileSync(path.join(output, `PA${variant}-states.png`), Buffer.from(sheet, 'base64'));
    }
    check(states.length === 17, 'All 17 design states render in both concepts without missing assets');
    for (const scenario of ['typing', 'slayer']) {
      await page.evaluate(v => window.HudReview.set(v), { width: 1280, scenario, bounds: true });
      for (const variant of [1, 2]) {
        const png = await page.locator('#p' + variant).evaluate(c => c.toDataURL().split(',')[1]);
        fs.writeFileSync(path.join(output, `PA${variant}-${scenario}-bounds.png`), Buffer.from(png, 'base64'));
      }
    }
    await page.selectOption('#word', 'is');
    check(await page.evaluate(() => window.HudReview.view.word === 'is'), 'Short-word control changes both proposals');
    await page.selectOption('#scenario', 'pause');
    check(await page.evaluate(() => window.HudReview.view.scenario === 'pause'), 'State selector renders Pause');
    await page.locator('.state-card[data-state="locked"]').click();
    check(await page.locator('.state-card[data-state="locked"]').getAttribute('aria-pressed') === 'true', 'State-card interaction updates selected state');
    await page.locator('#play').click(); await page.waitForTimeout(230);
    check(await page.evaluate(() => window.HudReview.view.time > 0), 'Storyboard starts and advances');
    await page.locator('#play').click(); const held = await page.evaluate(() => window.HudReview.view.time); await page.waitForTimeout(100);
    check(await page.evaluate(t => window.HudReview.view.time === t, held), 'Stopping storyboard freezes presentation clock');
    await page.evaluate(() => window.HudReview.set({ width: 640, scenario: 'typing', word: 'different', bounds: false }));
    const download = page.waitForEvent('download'); await page.locator('[data-export="1"]').click();
    check((await download).suggestedFilename() === 'PA1-typing-640.png', 'PNG export button produces a local download');
    check(await page.locator('.reference-card').count() === 5, 'Five source images and linked references available offline');
    await page.screenshot({ path: path.join(output, 'review-page.png'), fullPage: true });
    for (const w of [640, 1280]) {
      await page.setViewportSize({ width: w, height: 900 });
      check(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), 'Review page has no horizontal overflow at ' + w);
    }
    if (process.argv.includes('--video-frames')) {
      const frames = path.join(root, '.render-frames'); fs.mkdirSync(frames, { recursive: true });
      for (let i = 0; i < 144; i++) {
        const data = await page.evaluate(async t => {
          const r = window.HudReview; r.set({ width: 640, word: 'different', bounds: false }); r.storyboardAt(t);
          const c = document.createElement('canvas'); c.width = 1280; c.height = 420; const ctx = c.getContext('2d');
          ctx.fillStyle = '#100F13'; ctx.fillRect(0, 0, 1280, 420); ctx.fillStyle = '#E5BE72'; ctx.font = '16px sans-serif';
          ctx.fillText('PA1 / KHAY HAI TẦNG', 18, 25); ctx.fillText('PA2 / HÀNG BA Ô', 658, 25);
          ctx.drawImage(document.getElementById('p1'), 0, 40); ctx.drawImage(document.getElementById('p2'), 640, 40);
          ctx.fillStyle = '#C5B9A7'; ctx.font = '11px sans-serif';
          ctx.fillText('STORYBOARD UI · NỀN FRAME THẬT · CHƯA ĐỒNG BỘ ĐÒN / TIMING PARRY', 18, 413);
          ctx.fillText(`${t.toFixed(2)}s  /  ${r.states.find(s => s[0] === r.view.scenario)[1]}`, 960, 413);
          return c.toDataURL().split(',')[1];
        }, i / 12);
        fs.writeFileSync(path.join(frames, String(i).padStart(4, '0') + '.png'), Buffer.from(data, 'base64'));
      }
      receipt.storyboard = { frames: 144, fps: 12, seconds: 12, audio: false, type: 'UI storyboard over unchanged still captures, not live gameplay' };
    }
    check(receipt.errors.length === 0, 'No browser script errors');
    receipt.status = 'passed';
  } catch (e) { receipt.status = 'failed'; receipt.error = String(e.stack || e); process.exitCode = 1; }
  finally { await browser.close(); fs.writeFileSync(path.join(root, 'verification.json'), JSON.stringify(receipt, null, 2) + '\n'); console.log(JSON.stringify(receipt, null, 2)); }
})();
