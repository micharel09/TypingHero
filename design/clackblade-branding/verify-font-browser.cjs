// Explicit browser shaping check: no network, no page/game/account writes.
const fs = require('node:fs');
const path = require('node:path');
const {chromium} = require(process.env.TYPINGHERO_PLAYWRIGHT || 'playwright');
const root = path.resolve(__dirname, '../..');
const output = process.argv[2];
if (!output) throw Error('Supply a task-temporary output directory.');
const samples = [
  'ff fi fl ft AV VA To Ty Wa rn il 1I',
  'Difficulty Guest Clackblade PLAYER NAME Normal',
  '0123456789 40 40 00 00 x1 5 +100',
  'i j f l t y g p q',
  'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
  'abcdefghijklmnopqrstuvwxyz',
  '~!@#$%^&*()_+<>?',
];
(async () => {
  fs.mkdirSync(output, {recursive:true});
  const context = await chromium.launchPersistentContext(path.join(output,'edge-profile'), {
    channel:'msedge', headless:true, viewport:{width:2600,height:140}, deviceScaleFactor:1,
    downloadsPath:path.join(output,'downloads'), tracesDir:path.join(output,'traces')
  });
  const browser = context.browser();
  try {
    const page = await context.newPage();
    await page.route('**/*', route=>route.abort());
    const font = fs.readFileSync(path.join(root, 'Assets/Art/UI/Branding/ClackbladeSpecimenPixel.ttf')).toString('base64');
    await page.setContent(`<canvas width="2600" height="140"></canvas><style>@font-face{font-family:Specimen;src:url(data:font/ttf;base64,${font})}</style>`);
    await page.evaluate(()=>document.fonts.load('100px Specimen'));
    const measurements=[];
    for (let i=0;i<samples.length;i++) {
      const result=await page.evaluate(text=>{
        const canvas=document.querySelector('canvas'),ctx=canvas.getContext('2d');
        ctx.fillStyle='#000';ctx.fillRect(0,0,canvas.width,canvas.height);
        ctx.font='100px Specimen';ctx.fontKerning='normal';ctx.textBaseline='alphabetic';
        ctx.fillStyle='#fff';ctx.fillText(text,12,105);
        return {png:canvas.toDataURL().split(',')[1], advance:ctx.measureText(text).width,
          prefix_advances:Array.from({length:text.length+1},(_,i)=>ctx.measureText(text.slice(0,i)).width)};
      },samples[i]);
      fs.writeFileSync(path.join(output,`browser-string-${i}.png`),Buffer.from(result.png,'base64'));
      delete result.png;measurements.push({text:samples[i],...result});
    }
    fs.writeFileSync(path.join(output,'browser-metrics.json'),JSON.stringify(measurements,null,2));
    console.log(`Rendered ${samples.length} positioned strings with Edge ${browser.version()}.`);
  } finally { await context.close(); }
})().catch(error=>{console.error(error);process.exitCode=1;});
