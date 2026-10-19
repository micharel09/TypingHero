mergeInto(LibraryManager.library, {
  TypingHero_ReducedMotion: function() {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches ? 1 : 0;
  }
});
