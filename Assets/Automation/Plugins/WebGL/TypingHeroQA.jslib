mergeInto(LibraryManager.library, {
  TypingHeroQA_Publish: function (json) {
    window.typingHeroQA = Object.freeze(JSON.parse(UTF8ToString(json)));
  }
});
