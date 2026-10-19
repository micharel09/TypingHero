// Usage: node verify-browser.cjs <playwright-core module> <Chrome executable>
// Opens only this local gallery in a private headless browser, no server needed.
const { chromium } = require(process.argv[2]);
const path = require('node:path');
const {pathToFileURL}=require('node:url');
const assert = require('node:assert/strict');
(async()=>{
  const browser=await chromium.launch({headless:true,executablePath:process.argv[3]});
  try {
    const page=await browser.newPage({viewport:{width:1280,height:720}});
    const errors=[]; page.on('pageerror',e=>errors.push(e.message));
    page.on('requestfailed',r=>errors.push(r.url()+': '+r.failure().errorText));
    await page.goto(pathToFileURL(path.resolve(__dirname,'../index.html')).href);
    await page.evaluate(()=>document.fonts.ready);
    await page.waitForTimeout(2500);
    assert.equal(await page.locator('#intro .menu').evaluate(el=>getComputedStyle(el).opacity),'1');
    assert.equal(await page.locator('#intro button').isVisible(),false);
    await page.getByRole('button',{name:'Chạy lại'}).click();
    await page.waitForTimeout(150);
    assert.equal(await page.locator('#intro button').isVisible(),true);
    await page.getByRole('button',{name:'Bỏ qua'}).click();
    assert.equal(await page.locator('#intro .menu').evaluate(el=>getComputedStyle(el).opacity),'1');
    await page.getByRole('button',{name:'Chạy lại'}).click(); await page.keyboard.press('Escape');
    assert.equal(await page.locator('#intro button').isVisible(),false);
    await page.locator('#search').fill('button-primary');
    assert.equal(await page.locator('.asset:visible').count(),5);
    await page.locator('.asset:visible').first().click();
    assert.equal(await page.locator('dialog').evaluate(el=>el.open),true);
    await page.getByRole('button',{name:'Đóng',exact:true}).click();
    await page.locator('#search').fill(''); await page.locator('#group').selectOption('brand');
    assert.equal(await page.locator('.asset:visible').count(),4);
    await page.locator('#group').selectOption('');
    // Eagerly decode all review images: no missing assets hidden by lazy loading.
    const broken=await page.evaluate(async()=>{
      const imgs=[...document.images]; imgs.forEach(im=>im.loading='eager');
      await Promise.all(imgs.map(im=>im.decode().catch(()=>{})));
      return imgs.filter(im=>!im.naturalWidth).map(im=>im.src);
    }); assert.deepEqual(broken,[]);
    for (const width of [640,1280]) {
      await page.setViewportSize({width,height:Math.round(width*9/16)});
      assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
    }
    assert.deepEqual(errors,[]);
    console.log('PASS: gallery local-file assets/font/JS, auto finish, replay, button/Esc skip, filter, group, dialog, image decode, no overflow at 640/1280.');
  } finally {await browser.close()}
})().catch(e=>{console.error(e);process.exitCode=1});
