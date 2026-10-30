mergeInto(LibraryManager.library, {
  TypingHeroQA_TutorialClock: function(now) {
    window.typingHeroTutorialClock = { unity: now, browser: performance.now() };
  },
  TypingHeroQA_TutorialTrace: function(json) {
    window.typingHeroTutorialTrace = { json: UTF8ToString(json), receivedAt: performance.now() };
  },
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
    return (options.detailed === false ? 2 : 3) | (options.verifyCombatSerialization ? 4 : 0);
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
  $TypingHeroCombatParticles: { json: "" },
  TypingHeroQA_CombatFrame__deps: ["$TypingHeroCombatParticles"],
  TypingHeroQA_CombatFrame: function (json, particles, reference) {
    if (particles) TypingHeroCombatParticles.json = UTF8ToString(particles);
    var frame = JSON.parse(UTF8ToString(json));
    // Parse the cached string afresh: consumers retain independent nested values,
    // exactly as with the former full-frame JSON.parse, including stale slots.
    frame.parryParticles = JSON.parse(TypingHeroCombatParticles.json).parryParticles;
    if (reference) {
      if (JSON.stringify(frame) !== JSON.stringify(JSON.parse(UTF8ToString(reference))))
        throw new Error("Combat observer JSON differs from original serializer");
      window["typingHeroCombatSerializationChecks"] = (window["typingHeroCombatSerializationChecks"] || 0) + 1;
    }
    window["typingHeroCombatFrame"] = Object.freeze(frame);
    window["typingHeroCombatFramePublishedAt"] = performance.now();
  },
  TypingHeroQA_Publish: function (json) {
    window["typingHeroQA"] = Object.freeze(JSON.parse(UTF8ToString(json)));
  }
});
