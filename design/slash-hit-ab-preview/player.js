const $ = id => document.getElementById(id);
const ranges = [[0, 6], [6, 12], [12, 19], [0, 26]];
let playing = true, speed = 1, previousTime;
const clips = {};
let angle = null;
const gameplay = () => $('timing').value !== 'source' && Number($('color').value) < 8;
function duration(name, clip) {
  const index = clip.sequence[clip.cursor];
  if (index < 0) return 300; // Preview spacing only; not a gameplay frame.
  if (name === 'b' && gameplay()) return ($('timing').value === 'finisher' ? [60, 40, 30, 30] : [40, 30, 30, 20])[index];
  return DATA[name].frames[index].ms;
}

function newStroke() {
  const authored = {s1: 90, s2: 45, s3: 0}[$('direction').value];
  if (authored !== undefined && Number($('color').value) < 8) {
    angle = $('facing').value === 'left' ? (270 - authored) % 360 : authored;
    return;
  }
  if ($('direction').value === 'original' || Number($('color').value) === 8) {
    angle = 0;
    return;
  }
  // Uniformly choose among the seven other directions, without retries.
  const previous = angle === null ? -1 : angle / 45;
  let next = Math.floor(Math.random() * (previous < 0 ? 8 : 7));
  if (previous >= 0 && next >= previous) next++;
  angle = next * 45;
}

function advance(name, clip, delta = 1) {
  clip.cursor = (clip.cursor + delta + clip.sequence.length) % clip.sequence.length;
  if (name === 'b' && delta === 1 && clip.cursor === 0) newStroke();
}

function sequenceA() {
  const [start, end] = ranges[Number($('variant').value)];
  return Array.from({length: end - start}, (_, i) => start + i);
}

function reset() {
  if (!clips.a) return;
  clips.a.sequence = sequenceA();
  clips.b.sequence = gameplay() ? [0, 1, 2, 3, -1] : [4, 5, 0, 1, 2, 3];
  newStroke();
  for (const clip of Object.values(clips)) { clip.cursor = 0; clip.elapsed = 0; }
  previousTime = undefined;
  render();
}

function render() {
  for (const [name, clip] of Object.entries(clips)) {
    const index = clip.sequence[clip.cursor];
    const canvas = $(name), ctx = canvas.getContext('2d');
    const ms = duration(name, clip);
    ctx.imageSmoothingEnabled = false;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    if (index < 0) {
      // The stroke has fully cleared.
    } else if (name === 'b' && Number($('color').value) < 8) {
      const color = Number($('color').value);
      // Each 64x64 color tile is already 4x in the original public montage.
      ctx.save();
      ctx.translate(canvas.width / 2, canvas.height / 2);
      ctx.rotate(angle * Math.PI / 180);
      ctx.drawImage(clip.images[index], 128 + (color % 4) * 256,
        384 + Math.floor(color / 4) * 256, 256, 256, -128, -128, 256, 256);
      ctx.restore();
    } else {
      ctx.drawImage(clip.images[index], 0, 0);
    }
    $(name + '-status').textContent = `${index < 0 ? 'Tan hết · khoảng nghỉ preview' : 'Frame nguồn F' + index} · ${ms} ms / ${ms / speed} ms đang phát${name === 'b' ? (Number($('color').value) === 8 ? ' · Montage gốc · không xoay' : ` · Góc ${angle}° · ${gameplay() ? 'Nhịp trong game' : 'Timing nguồn'}`) : ''}`;
  }
}

function tick(now) {
  if (previousTime !== undefined && playing) {
    const dt = (now - previousTime) * speed;
    for (const [name, clip] of Object.entries(clips)) {
      clip.elapsed += dt;
      while (clip.elapsed >= duration(name, clip)) {
        clip.elapsed -= duration(name, clip);
        advance(name, clip);
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
    $('b').width = full ? 1260 : 384; $('b').height = full ? 1000 : 384;
    $('b').style.width = full ? '420px' : '384px';
    $('b').style.height = full ? 'auto' : '384px';
    reset();
  };
  $('direction').onchange = reset;
  $('timing').onchange = reset;
  $('facing').onchange = reset;
  $('pause').onclick = () => pause(playing);
  $('restart').onclick = () => { pause(false); reset(); };
  for (const [id, delta] of [['previous', -1], ['next', 1]]) {
    $(id).onclick = () => {
      pause(true);
      for (const [name, clip] of Object.entries(clips)) {
        advance(name, clip, delta);
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
