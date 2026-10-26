mergeInto(LibraryManager.library, {
  TypingHeroQA_TutorialClock: function(now) {
    window.typingHeroTutorialClock = { unity: now, browser: performance.now() };
  },
  TypingHeroQA_TutorialTrace: function(json) {
    window.typingHeroTutorialTrace = { json: UTF8ToString(json), receivedAt: performance.now() };
  },
  TypingHeroQA_CombatFrame: function (json) {
    window["typingHeroCombatFrame"] = Object.freeze(JSON.parse(UTF8ToString(json)));
    window["typingHeroCombatFramePublishedAt"] = performance.now();
  },
  TypingHeroQA_Publish: function (json) {
    window["typingHeroQA"] = Object.freeze(JSON.parse(UTF8ToString(json)));
  }
});
