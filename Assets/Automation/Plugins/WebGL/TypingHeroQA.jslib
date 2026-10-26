mergeInto(LibraryManager.library, {
  // Fixed storage; only the batch reader allocates/serializes sample rows.
  // Unity LateUpdate markers count engine frames, independently of browser rAF.
  TypingHeroQA_FrameBegin: function () {
    var options = window["typingHeroPerfOptions"];
    if (!options || !options.enabled) return 1;
    var buffer = window["typingHeroPerfBuffer"];
    if (!buffer) {
      var capacity = 16384, stride = 11;
      buffer = { data: new Float64Array(capacity * stride), read: 0, write: 0,
        dropped: 0, epoch: 0, begin: 0 };
      var changed = function () { buffer.epoch++; };
      window.addEventListener("focus", changed);
      window.addEventListener("blur", changed);
      document.addEventListener("visibilitychange", changed);
      window["typingHeroReadFrames"] = function () {
        var rows = [];
        while (buffer.read < buffer.write) {
          var offset = buffer.read++ % capacity * stride;
          rows.push(Array.from(buffer.data.subarray(offset, offset + stride)));
        }
        return { schema: ["sequence", "frame", "at", "deltaMs", "observerMs", "flags",
          "parries", "timeScale", "focused", "visible", "focusEpoch"],
          rows: rows, dropped: buffer.dropped, focusEpoch: buffer.epoch };
      };
      window["typingHeroPerfBuffer"] = buffer;
    }
    buffer.begin = performance.now();
    return options.detailed === false ? 2 : 3;
  },
  TypingHeroQA_FrameEnd: function (frame, deltaMs, flags, parries, timeScale) {
    var buffer = window["typingHeroPerfBuffer"];
    var at = performance.now(), capacity = 16384, stride = 11;
    if (buffer.write - buffer.read === capacity) { buffer.read++; buffer.dropped++; }
    var offset = buffer.write % capacity * stride, data = buffer.data;
    data[offset] = buffer.write++;
    data[offset + 1] = frame;
    data[offset + 2] = at;
    data[offset + 3] = deltaMs;
    data[offset + 4] = at - buffer.begin;
    data[offset + 5] = flags;
    data[offset + 6] = parries;
    data[offset + 7] = timeScale;
    data[offset + 8] = document.hasFocus() ? 1 : 0;
    data[offset + 9] = document.visibilityState === "visible" ? 1 : 0;
    data[offset + 10] = buffer.epoch;
  },
  TypingHeroQA_CombatFrame: function (json) {
    window["typingHeroCombatFrame"] = Object.freeze(JSON.parse(UTF8ToString(json)));
    window["typingHeroCombatFramePublishedAt"] = performance.now();
  },
  TypingHeroQA_Publish: function (json) {
    window["typingHeroQA"] = Object.freeze(JSON.parse(UTF8ToString(json)));
  }
});
