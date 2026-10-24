const $ = id => document.getElementById(id);
const ranges = [[0, 6], [6, 12], [12, 19], [0, 26]];
let playing = true, speed = 1, previousTime;
const clips = {};

function sequenceA() {
  const [start, end] = ranges[Number($('variant').value)];
  return Array.from({length: end - start}, (_, i) => start + i);
}

function reset() {
  if (!clips.a) return;
  clips.a.sequence = sequenceA();
  for (const clip of Object.values(clips)) { clip.cursor = 0; clip.elapsed = 0; }
  previousTime = undefined;
  render();
}

function render() {
  for (const [name, clip] of Object.entries(clips)) {
    const index = clip.sequence[clip.cursor];
    const canvas = $(name), ctx = canvas.getContext('2d');
    const frame = DATA[name].frames[index];
    ctx.imageSmoothingEnabled = false;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    if (name === 'b' && Number($('color').value) < 8) {
      const color = Number($('color').value);
      // Each 64x64 color tile is already 4x in the original public montage.
      ctx.drawImage(clip.images[index], 128 + (color % 4) * 256,
        384 + Math.floor(color / 4) * 256, 256, 256, 0, 0, 256, 256);
    } else {
      ctx.drawImage(clip.images[index], 0, 0);
    }
    $(name + '-status').textContent = `Frame nguồn F${index} · ${frame.ms} ms gốc / ${frame.ms / speed} ms đang phát · ${clip.cursor + 1}/${clip.sequence.length}`;
  }
}

function tick(now) {
  if (previousTime !== undefined && playing) {
    const dt = (now - previousTime) * speed;
    for (const [name, clip] of Object.entries(clips)) {
      clip.elapsed += dt;
      const total = clip.sequence.reduce((sum, i) => sum + DATA[name].frames[i].ms, 0);
      clip.elapsed %= total;
      while (clip.elapsed >= DATA[name].frames[clip.sequence[clip.cursor]].ms) {
        clip.elapsed -= DATA[name].frames[clip.sequence[clip.cursor]].ms;
        clip.cursor = (clip.cursor + 1) % clip.sequence.length;
      }
    }
    render();
  }
  previousTime = now;
  requestAnimationFrame(tick);
}

function pause(value) {
  playing = !value;
  $('pause').textContent = playing ? 'Tạm dừng' : 'Tiếp tục';
}

async function init() {
  for (const name of ['a', 'b']) {
    const images = await Promise.all(DATA[name].frames.map(async frame => {
      const image = new Image(); image.src = frame.src; await image.decode(); return image;
    }));
    clips[name] = {images, sequence: images.map((_, i) => i), cursor: 0, elapsed: 0};
  }
  $('sheet').src = DATA.sheet.frames[0].src;
  $('provenance').textContent = Object.entries(DATA).map(([key, value]) =>
    `${key}: ${value.url}\nSHA-256: ${value.sha256}`).join('\n\n');
  $('speed').onchange = () => { speed = Number($('speed').value); reset(); };
  $('variant').onchange = reset;
  $('color').onchange = () => {
    const full = Number($('color').value) === 8;
    $('b').width = full ? 1260 : 256; $('b').height = full ? 1000 : 256;
    $('b').style.width = full ? '420px' : '256px';
    $('b').style.height = full ? 'auto' : '256px';
    reset();
  };
  $('pause').onclick = () => pause(playing);
  $('restart').onclick = () => { pause(false); reset(); };
  for (const [id, delta] of [['previous', -1], ['next', 1]]) {
    $(id).onclick = () => {
      pause(true);
      for (const clip of Object.values(clips)) {
        clip.cursor = (clip.cursor + delta + clip.sequence.length) % clip.sequence.length;
        clip.elapsed = 0;
      }
      render();
    };
  }
  document.addEventListener('visibilitychange', () => { previousTime = undefined; });
  $('controls').disabled = false;
  $('error').textContent = 'Sẵn sàng · dữ liệu offline từ preview tác giả.';
  reset(); requestAnimationFrame(tick);
}
init().catch(error => { $('error').textContent = `Không nạp được preview: ${error.message}`; });
