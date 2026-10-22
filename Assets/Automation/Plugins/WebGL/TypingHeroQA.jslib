mergeInto(LibraryManager.library, {
  TypingHeroQA_CombatFrame: function (json) {
    window["typingHeroCombatFrame"] = Object.freeze(JSON.parse(UTF8ToString(json)));
    window["typingHeroCombatFramePublishedAt"] = performance.now();
  },
  TypingHeroQA_Publish: function (json) {
    window["typingHeroQA"] = Object.freeze(JSON.parse(UTF8ToString(json)));
  }
});
