'use strict';
(() => {
  const D = window.HUD_REVIEW_DATA;
  const C = { ink: '#F2E7CF', muted: '#C5B9A7', third: '#938B80', teal: '#73B6AA',
    gold: '#E5BE72', error: '#F16B67', bg: '#14111E', rim: '#8E784F', hp: '#C34B55' };
  const states = [
    ['typing', 'Đang gõ', 'Tô phần đã gõ; căn bằng toàn bộ từ.'],
    ['idle', 'Chờ nhập', 'Ba từ hiện liên tục, không chuyển chỗ.'],
    ['complete', 'Hoàn thành từ', 'Từ kế tiếp thay ngay; góc khung sáng một lần.'],
    ['error', 'Gõ sai', 'Báo đỏ trong khay; chữ không rung hoặc co lại.'],
    ['timeout', 'Hết thời gian', 'Thanh về 0; phản ánh reset từ hiện có.'],
    ['windup', 'Chuẩn bị đánh', 'Hai dấu tiến vào hai mép keycap Space.'],
    ['ready', 'Mốc Space', 'Keycap sáng rõ, giữ đến va chạm hoặc hủy đòn.'],
    ['locked', 'Parry bị khóa', 'Gạch chéo keycap; không phát tín hiệu bấm được.'],
    ['stun', 'Enemy stun', 'Giữ khay đọc; không báo nhịp cho đòn đã hủy.'],
    ['slayer-entry', 'Vào Slayer', 'Cảnh thật đang chuyển; khay và header đứng yên.'],
    ['slayer', 'Slayer', 'Giữ chữ lớn; chỉ nhấn vàng ở góc khung.'],
    ['slayer-exit', 'Ra Slayer', 'Chữ giữ nguyên vị trí qua chuyển tiếp camera.'],
    ['pause', 'Pause', 'Giữ tiến độ; bảng Pause không che khay hoặc Space.'],
    ['hitstop', 'Hitstop', 'Cue và timer đứng ở cùng một mốc.'],
    ['tutorial', 'Tutorial', 'Chỉ dẫn tĩnh trỏ khay và cue; không che hai đích.'],
    ['encounter', 'Đổi encounter', 'Dọn chữ và cue cũ; khung đọc không nhảy chỗ.'],
    ['end', 'Kết thúc', 'Ẩn khay/cue; chỉ hiển thị kết quả trận.']
  ];
  const view = { scenario: 'typing', word: 'different', width: 640, bounds: false, time: 0 };
  const images = {}, native = [1, 2].map(() => Object.assign(document.createElement('canvas'), { width: 320, height: 180 }));
  let playing = false, start = 0, raf = 0;
  const ctx0 = native[0].getContext('2d');
  const ready = Promise.all(Object.entries(D.assets).map(async ([key, src]) => {
    if (key === 'font') { const font = new FontFace('Cobblestone', `url(${src})`); await font.load(); document.fonts.add(font); return; }
    const im = new Image(); im.src = src; await im.decode(); images[key] = im;
  }));

  function text(ctx, str, x, y, size, color = C.ink, align = 'left') {
    ctx.font = `${size}px Cobblestone`; ctx.textAlign = align; ctx.textBaseline = 'alphabetic';
    ctx.fillStyle = '#08070B'; ctx.fillText(str, x, y + 1);
    ctx.fillStyle = color; ctx.fillText(str, x, y);
  }
  function width(str, size) { ctx0.font = `${size}px Cobblestone`; return ctx0.measureText(str).width; }
  function line(ctx, x, y, w, h, color) { ctx.fillStyle = color; ctx.fillRect(x, y, w, h); }
  function frame(ctx, skin, x, y, w, h) {
    const im = images[skin], b = 4, sw = im.width, sh = im.height;
    const sx = [0, b, sw - b], sy = [0, b, sh - b], dx = [x, x + b, x + w - b], dy = [y, y + b, y + h - b];
    const widths = [b, sw - b * 2, b], heights = [b, sh - b * 2, b];
    const dw = [b, w - b * 2, b], dh = [b, h - b * 2, b];
    for (let r = 0; r < 3; r++) for (let c = 0; c < 3; c++)
      ctx.drawImage(im, sx[c], sy[r], widths[c], heights[r], dx[c], dy[r], dw[c], dh[r]);
  }
  function status(ctx, x, name, health, guard, broken = false) {
    text(ctx, name, x + (guard ? 96 : 0), 12, 10, C.ink, guard ? 'right' : 'left');
    text(ctx, guard ? '150/180' : '35/40', x + (guard ? 0 : 94), 12, 10, C.muted, guard ? 'left' : 'right');
    frame(ctx, 'resource-frame', x, 17, 96, 15);
    ctx.drawImage(images['icon-hp'], x + 3, 19, 10, 10);
    line(ctx, x + 17, 22, 73, 5, '#32222C'); line(ctx, x + 17, 22, Math.round(73 * health), 5, C.hp);
    if (guard) {
      frame(ctx, 'secondary-frame', x, 34, 96, 9); ctx.drawImage(images['icon-shield'], x + 3, 34, 8, 8);
      line(ctx, x + 17, 37, 73, 2, '#2A3032');
      if (broken) text(ctx, 'BREAK', x + 88, 41, 8, C.gold, 'right');
      else line(ctx, x + 17, 37, 51, 2, C.teal);
    } else text(ctx, 'ROUND 1 / DUEL 1', x, 41, 8, C.muted);
  }
  function header(ctx, state) {
    line(ctx, 0, 0, 320, 46, '#08080A');
    status(ctx, 8, 'PLAYER', .875, false); status(ctx, 216, 'SWORDSMAN', .83, true, state.startsWith('slayer'));
    frame(ctx, 'score-frame', 112, 4, 96, 18); text(ctx, '1746', 160, 18, 12, C.ink, 'center');
    text(ctx, 'x1.6', 116, 33, 10, C.gold); text(ctx, state === 'idle' ? '' : '+47', 204, 33, 10, C.gold, 'right');
    line(ctx, 116, 38, 88, 2, '#302A30'); line(ctx, 116, 38, 62, 2, C.gold);
  }
  function cue(ctx, variant, p, locked) {
    const y = variant === 1 ? 128 : 136, key = locked ? '#706D67' : p >= 1 ? C.ink : C.gold;
    ctx.globalAlpha = locked ? .45 : p >= 1 ? 1 : .78;
    ctx.drawImage(images['keycap-space'], 148, y, 24, 12); ctx.globalAlpha = 1;
    text(ctx, 'SPACE', 160, y + 8, 8, key, 'center');
    const l = Math.round(116 + 29 * p), r = Math.round(202 - 29 * p);
    if (!locked) { line(ctx, l, y + 3, 2, 6, key); line(ctx, r, y + 3, 2, 6, key); }
    if (p >= 1 && !locked) { line(ctx, 149, y + 10, 22, 1, C.gold); line(ctx, 149, y, 22, 1, C.gold); }
    if (locked) { ctx.strokeStyle = '#A49A88'; ctx.lineWidth = 1; ctx.beginPath(); ctx.moveTo(150, y + 10); ctx.lineTo(170, y + 1); ctx.stroke(); }
  }
  function word(ctx, str, cx, baseline, size, typed, error) {
    const x = cx - width(str, size) / 2;
    text(ctx, str, x, baseline, size, error ? C.error : C.ink);
    if (typed && !error) {
      ctx.save(); ctx.beginPath(); ctx.rect(x, baseline - size, width(str.slice(0, typed), size), size + 3); ctx.clip();
      text(ctx, str, x, baseline, size, C.teal); ctx.restore();
    }
  }
  function dock(ctx, variant, options) {
    const { current, typed, timer, error, slayer, complete } = options;
    const x = variant === 1 ? 56 : 28, y = variant === 1 ? 142 : 152, w = variant === 1 ? 208 : 264, h = variant === 1 ? 36 : 26;
    frame(ctx, 'typing-frame', x, y, w, h);
    if (variant === 1) {
      word(ctx, current, 160, 158, 16, typed, error);
      text(ctx, complete ? 'between' : 'because', 133, 171, 10, C.muted, 'center'); text(ctx, complete ? 'question' : 'between', 187, 171, 10, C.third, 'center');
      line(ctx, 62, 175, 196, 1, '#333238'); line(ctx, 62, 175, Math.round(196 * timer), 1, error ? C.error : C.teal);
    } else {
      word(ctx, current, 90, 168, 16, typed, error);
      line(ctx, 150, 157, 1, 15, '#4E4545'); line(ctx, 225, 157, 1, 15, '#4E4545');
      text(ctx, complete ? 'between' : 'because', 187, 167, 10, C.muted, 'center'); text(ctx, complete ? 'question' : 'between', 259, 167, 10, C.third, 'center');
      line(ctx, 32, 174, 112, 1, '#333238'); line(ctx, 32, 174, Math.round(112 * timer), 1, error ? C.error : C.teal);
    }
    if (slayer || complete || error) {
      const col = error ? C.error : complete ? C.teal : C.gold;
      for (const xx of [x, x + w - 6]) { line(ctx, xx, y, 6, 1, col); line(ctx, xx, y + h - 1, 6, 1, col); }
    }
  }
  function modal(ctx, variant, state) {
    if (state === 'pause') {
      ctx.fillStyle = '#08070B66'; ctx.fillRect(0, 46, 320, 80);
      frame(ctx, 'typing-frame', 105, 56, 110, 68);
      text(ctx, 'PAUSE', 160, 71, 12, C.ink, 'center');
      for (const [i, label] of ['Resume', 'Restart', 'Settings'].entries()) text(ctx, label, 160, 86 + i * 12, 10, i ? C.muted : C.gold, 'center');
      text(ctx, `DESIGN / PA${variant}`, 160, 121, 8, C.muted, 'center');
    }
    if (state === 'tutorial') {
      frame(ctx, 'typing-frame', 8, 104, 80, 28); text(ctx, 'TYPE TO ATTACK', 48, 115, 8, C.gold, 'center'); text(ctx, '3 WORDS / 1 FOCUS', 48, 126, 8, C.ink, 'center');
      frame(ctx, 'typing-frame', 232, 104, 80, 28); text(ctx, 'PARRY ON SPACE', 272, 115, 8, C.gold, 'center'); text(ctx, 'WATCH THE MARKS', 272, 126, 8, C.ink, 'center');
      ctx.strokeStyle = C.gold; ctx.lineWidth = 1; ctx.beginPath(); ctx.moveTo(70, 132); ctx.lineTo(variant === 1 ? 60 : 32, variant === 1 ? 146 : 156); ctx.moveTo(246, 132); ctx.lineTo(206, variant === 1 ? 134 : 142); ctx.stroke();
    }
    if (state === 'encounter') { text(ctx, 'NEXT DUEL', 160, 85, 16, C.gold, 'center'); text(ctx, 'PRESS ENTER', 160, 99, 10, C.ink, 'center'); }
    if (state === 'end') { frame(ctx, 'typing-frame', 102, 65, 116, 45); text(ctx, 'VICTORY', 160, 83, 16, C.gold, 'center'); text(ctx, '1746 POINTS', 160, 100, 10, C.ink, 'center'); }
  }
  function timeline(time) {
    const frames = ['typing', 'windup', 'ready', 'locked', 'complete', 'error', 'slayer-entry', 'slayer', 'slayer-exit', 'pause', 'tutorial', 'encounter'];
    return frames[Math.min(11, Math.floor(time))];
  }
  function renderOne(target, variant) {
    const ctx = native[variant - 1].getContext('2d'), state = view.scenario;
    const slayer = state === 'slayer', trans = state === 'slayer-entry' || state === 'slayer-exit';
    ctx.imageSmoothingEnabled = false; ctx.clearRect(0, 0, 320, 180);
    ctx.drawImage(images[slayer ? 'slayer' : state === 'slayer-entry' ? 'entry' : trans ? 'exit' : 'normal'], 0, 0, 320, 180);
    header(ctx, state); line(ctx, 0, 140, 320, 40, '#08080A');
    const empty = state === 'encounter' || state === 'end';
    if (!empty) {
      dock(ctx, variant, { current: state === 'complete' ? 'because' : view.word, typed: ['idle', 'error', 'timeout', 'complete'].includes(state) ? 0 : 3,
        timer: state === 'timeout' ? 0 : .68, error: state === 'error' || state === 'timeout', slayer: slayer || trans, complete: state === 'complete' });
      if (['windup', 'ready', 'locked', 'pause', 'hitstop', 'tutorial'].includes(state)) {
        const progress = state === 'ready' ? 1 : state === 'windup' && (playing || view.storyboard) ? Math.min(1, (view.time % 1) * 1.2) : .6;
        cue(ctx, variant, progress, state === 'locked');
      }
    }
    ctx.globalAlpha = .6; ctx.drawImage(images['icon-pause'], 305, 166, 9, 9); ctx.globalAlpha = 1;
    modal(ctx, variant, state);
    if (view.bounds) {
      ctx.lineWidth = 1; ctx.setLineDash([3, 2]); ctx.strokeStyle = '#73B6AA';
      const box = slayer || trans ? [55, 47, 210, 89] : [68, 48, 184, 76]; ctx.strokeRect(...box);
      ctx.strokeStyle = C.gold; ctx.strokeRect(variant === 1 ? 56 : 28, variant === 1 ? 142 : 152, variant === 1 ? 208 : 264, variant === 1 ? 36 : 26); ctx.setLineDash([]);
    }
    target.width = view.width; target.height = view.width * 9 / 16;
    const out = target.getContext('2d'); out.imageSmoothingEnabled = false; out.drawImage(native[variant - 1], 0, 0, target.width, target.height);
  }
  function render() {
    for (const i of [1, 2]) renderOne(document.getElementById(`p${i}`), i);
    document.getElementById('state-note').textContent = states.find(s => s[0] === view.scenario)[2];
    document.getElementById('time-label').textContent = playing ? `STORYBOARD / ${view.time.toFixed(1)}s` : 'STILL / DỮ LIỆU MẪU';
    document.querySelectorAll('.state-card').forEach(b => b.setAttribute('aria-pressed', String(b.dataset.state === view.scenario)));
  }
  function stop() { playing = false; cancelAnimationFrame(raf); document.getElementById('play').textContent = 'Chạy storyboard'; }
  function set(next) { Object.assign(view, { storyboard: false }, next); document.getElementById('scenario').value = view.scenario; document.getElementById('word').value = view.word; document.getElementById('size').value = String(view.width); document.getElementById('bounds').checked = view.bounds; render(); return { ...view }; }
  function tick(now) { view.time = ((now - start) / 1000) % 12; view.scenario = timeline(view.time); document.getElementById('scenario').value = view.scenario; render(); raf = requestAnimationFrame(tick); }
  function init() {
    const select = document.getElementById('scenario'), grid = document.getElementById('state-grid');
    for (const [i, s] of states.entries()) {
      select.add(new Option(s[1], s[0]));
      const b = document.createElement('button'); b.className = 'state-card'; b.dataset.state = s[0]; b.innerHTML = `<span>${String(i + 1).padStart(2, '0')}</span><strong>${s[1]}</strong><small>${s[2]}</small>`;
      b.onclick = () => { stop(); set({ scenario: s[0] }); document.getElementById('compare-title').scrollIntoView({ behavior: 'smooth', block: 'start' }); }; grid.append(b);
    }
    for (const ref of D.references) {
      const a = document.createElement('article'); a.className = 'reference-card';
      a.innerHTML = `<a target="_blank" rel="noopener" href="${ref.url}"><img alt="Ảnh gameplay tham khảo ${ref.name}" src="${ref.image}"><h3>${ref.name} ↗</h3></a><p>${ref.lesson}</p>`; document.getElementById('references').append(a);
    }
    select.onchange = () => { stop(); set({ scenario: select.value }); };
    document.getElementById('word').onchange = e => { stop(); set({ word: e.target.value }); };
    document.getElementById('size').onchange = e => set({ width: Number(e.target.value) });
    document.getElementById('bounds').onchange = e => set({ bounds: e.target.checked });
    document.getElementById('play').onclick = () => { if (playing) stop(); else { playing = true; start = performance.now(); document.getElementById('play').textContent = 'Dừng storyboard'; raf = requestAnimationFrame(tick); } };
    document.querySelectorAll('[data-export]').forEach(b => { b.onclick = () => { const a = document.createElement('a'); a.download = `PA${b.dataset.export}-${view.scenario}-${view.width}.png`; a.href = document.getElementById('p' + b.dataset.export).toDataURL('image/png'); a.click(); }; });
    window.HudReview = { ready, set, stop, timeline, states, data: D, view, measure: width,
      storyboardAt(time) { stop(); set({ scenario: timeline(time), time, storyboard: true }); },
      geometry: { mainSize: 16, previewSize: 10, p1: { dock: [56, 142, 208, 36], cue: [116, 128, 88, 12] }, p2: { dock: [28, 152, 264, 26], cue: [116, 136, 88, 12] } } };
    render();
  }
  ready.then(init).catch(error => { document.getElementById('state-note').textContent = 'Không tải được tài nguyên: ' + error.message; console.error(error); });
})();
