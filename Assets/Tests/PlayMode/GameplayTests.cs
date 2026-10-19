using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class GameplayTests
{
    readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
    TypingManager typing;
    GameObject root;
    int words, chars, timeouts;
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    static void Set(object target, string name, object value)
        => target.GetType().GetField(name, Private).SetValue(target, value);
    static T Get<T>(object target, string name)
        => (T)target.GetType().GetField(name, Private).GetValue(target);
    static void Call(object target, string name)
        => target.GetType().GetMethod(name, Private).Invoke(target, null);
    T Own<T>(T item) where T : UnityEngine.Object { owned.Add(item); return item; }
    void Word() => words++;
    void Char(char _) => chars++;
    void Timeout() => timeouts++;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        Time.timeScale = 1;
        TypingManager.MuteWordCorrect = false;
        words = chars = timeouts = 0;
        root = Own(new GameObject("offline fixture", typeof(RectTransform), typeof(Canvas)));
        root.SetActive(false);
        var rail = new GameObject("rail", typeof(RectTransform));
        rail.transform.SetParent(root.transform, false);
        typing = root.AddComponent<TypingManager>();
        Set(typing, "rail", rail.GetComponent<RectTransform>());
        foreach (var field in new[] { "wordText", "nextWordText", "thirdWordText" })
        {
            var label = new GameObject(field, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            label.transform.SetParent(rail.transform, false);
            Set(typing, field, label.GetComponent<TextMeshProUGUI>());
        }
        Set(typing, "enableErrorFlash", false);
        TypingManager.OnWordCorrect += Word;
        TypingManager.OnCorrectChar += Char;
        TypingManager.OnWordTimeout += Timeout;
        root.SetActive(true);
        yield return null;
        typing.enabled = false; // Feed exactly the production input path; timer is controlled per test.
        Set(typing, "current", "type");
        Set(typing, "originalCurrent", "type");
        Set(typing, "next", "next");
        Set(typing, "third", "third");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        TypingManager.OnWordCorrect -= Word;
        TypingManager.OnCorrectChar -= Char;
        TypingManager.OnWordTimeout -= Timeout;
        TypingManager.MuteWordCorrect = false;
        foreach (var item in owned) if (item) UnityEngine.Object.Destroy(item);
        owned.Clear();
        Time.timeScale = 1;
        AudioListener.pause = false;
        yield return null;
        Assert.That(UnityEngine.Object.FindObjectsOfType<TypingManager>(), Is.Empty);
        Assert.That(UnityEngine.Object.FindObjectsOfType<ScoreSystem>(), Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator FixedRowAccepts15010060MsInputAndCompletionDuringSlide()
    {
        foreach (float cadence in new[] { .15f, .1f, .06f })
        {
            typing.SetPractice(new[] { "at", "in", "to" }, false, true);
            int startWords = words, startChars = chars;
            typing.enabled = true;
            foreach (char c in "atinto")
            {
                typing.ProcessInput(c.ToString());
                yield return new WaitForSecondsRealtime(cadence);
            }
            typing.enabled = false;
            Assert.That(words - startWords, Is.EqualTo(3));
            Assert.That(chars - startChars, Is.EqualTo(6));
            Assert.That(Get<string>(typing, "current"), Is.EqualTo("at"));
        }
        typing.SetPractice(new[] { "a", "i", "to" }, false, true);
        int before = words;
        typing.ProcessInput("a");
        Assert.That(Get<float>(typing, "slideRemaining"), Is.GreaterThan(0));
        typing.ProcessInput("i");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("to"));
        typing.ProcessInput("toaito");
        Assert.That(words - before, Is.EqualTo(6));
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("a"));
        Assert.That(Get<float>(typing, "slideRemaining"), Is.Zero, "burst paints final queue without stacked animation");
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ErrorRestarts180MsAndCorrectInputClearsItWithoutMovingWord()
    {
        Set(typing, "enableErrorFlash", true);
        typing.SetPractice(new[] { "resolve", "honor", "courage" }, false, true);
        typing.enabled = true;
        yield return new WaitForSecondsRealtime(.12f);
        var label = Get<TextMeshProUGUI>(typing, "wordText");
        var origin = label.rectTransform.anchoredPosition;
        typing.ProcessInput("res");
        Assert.That(label.rectTransform.anchoredPosition, Is.EqualTo(origin));
        typing.ProcessInput("z");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("resolve"));
        yield return new WaitForSecondsRealtime(.12f);
        typing.ProcessInput("z");
        yield return new WaitForSecondsRealtime(.1f);
        Assert.That(Get<float>(typing, "errorRemaining"), Is.GreaterThan(0));
        typing.ProcessInput("r");
        Assert.That(Get<float>(typing, "errorRemaining"), Is.Zero);
        Assert.That(label.rectTransform.anchoredPosition, Is.EqualTo(origin));
        typing.ProcessInput("z");
        yield return new WaitForSecondsRealtime(.21f);
        Assert.That(Get<float>(typing, "errorRemaining"), Is.Zero);
        typing.enabled = false;
    }

    [Test, Category("Quick")]
    public void TypingSfxFollowsActualAttemptsAndUnsubscribesAcrossEnableCycles()
    {
        var player = Own(new GameObject("typing audio fixture")).AddComponent<SfxPlayer>();
        var correct = Own(ScriptableObject.CreateInstance<SfxEvent>());
        var error = Own(ScriptableObject.CreateInstance<SfxEvent>());
        var clip = Own(AudioClip.Create("long test voice", 44100, 1, 44100, false));
        foreach (var ev in new[] { correct, error })
        {
            Set(ev, "clips", new[] { clip }); Set(ev, "maxVoices", 32);
            Set(ev, "volumeJitter", 0f); Set(ev, "pitchJitterSemitone", 0f);
        }
        var binder = root.AddComponent<TypingInputSfx>();
        Set(binder, "correct", correct); Set(binder, "error", error);
        TypingManager.MuteWordCorrect = true;
        typing.ProcessInput("TYztypeNE");
        Assert.That(Get<int>(correct, "_voicesNow"), Is.EqualTo(8));
        Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(1));
        Assert.That(chars, Is.EqualTo(6)); // Retyped positions sound without extra rewards.
        typing.ProcessInput("1! ");
        GamePauseController.SetTutorialReading(true);
        try { typing.ProcessInput("zzz"); }
        finally { GamePauseController.SetTutorialReading(false); }
        Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(1));
        binder.enabled = false;
        typing.ProcessInput("z");
        Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(1));
        for (int i = 0; i < 3; i++) { binder.enabled = true; binder.enabled = false; }
        binder.enabled = true;
        typing.ProcessInput("z");
        Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(2));
        bool hadSfx = PlayerPrefs.HasKey("SfxVolume");
        float previousSfx = AudioPreferences.Sfx;
        try
        {
            PlayerPrefs.SetFloat("SfxVolume", 0f);
            typing.ProcessInput("z");
            Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(2), "Muted input must not start a WebGL voice");
        }
        finally
        {
            if (hadSfx) PlayerPrefs.SetFloat("SfxVolume", previousSfx);
            else PlayerPrefs.DeleteKey("SfxVolume");
        }
        var session = root.AddComponent<CombatSession>(); session.Complete(false);
        typing.ProcessInput("z");
        Assert.That(Get<int>(error, "_voicesNow"), Is.EqualTo(2));
        foreach (var src in player.GetComponentsInChildren<AudioSource>())
        {
            Assert.That(src.spatialBlend, Is.Zero);
            Assert.That(src.ignoreListenerPause, Is.False);
        }
    }

    [Test, Category("Quick")]
    public void TypingVoiceLimitsAndMixRespectExistingPreferences()
    {
        var player = Own(new GameObject("typing mix fixture")).AddComponent<SfxPlayer>();
        var ev = Own(ScriptableObject.CreateInstance<SfxEvent>());
        Set(ev, "clips", new[] { Own(AudioClip.Create("tick", 44100, 1, 44100, false)) });
        Set(ev, "volume", .16f); Set(ev, "volumeJitter", 0f);
        Set(ev, "pitchJitterSemitone", 0f); Set(ev, "maxVoices", 4);
        var first = player.Play(ev, Vector3.zero);
        Assert.That(first, Is.Not.Null);
        Assert.That(first.volume, Is.EqualTo(.16f * AudioPreferences.Sfx).Within(.0001f));
        Assert.That(first.pitch, Is.EqualTo(1f));
        Assert.That(first.ignoreListenerPause, Is.False);
        for (int i = 0; i < 3; i++) Assert.That(player.Play(ev, Vector3.zero), Is.Not.Null);
        Assert.That(player.Play(ev, Vector3.zero), Is.Null);
        Assert.That(Get<int>(ev, "_voicesNow"), Is.EqualTo(4));
        var error = Own(ScriptableObject.CreateInstance<SfxEvent>());
        Set(error, "clips", new[] { first.clip }); Set(error, "maxVoices", 1);
        Set(error, "cooldown", .12f); Set(error, "_lastPlayRealtime", -1f);
        Assert.That(player.Play(error, Vector3.zero), Is.Not.Null);
        Assert.That(player.Play(error, Vector3.zero), Is.Null);
        error.NotifyEnd();
        Assert.That(error.CanPlayNow(), Is.False, "Cooldown survives voice completion");
        Set(error, "_lastPlayRealtime", Time.unscaledTime - .121f);
        Assert.That(error.CanPlayNow(), Is.True);
    }

    [Test, Category("Quick")]
    public void LettersCompareIgnoringCaseAndWrongCharactersDoNotAdvance()
    {
        typing.ProcessInput("1!zT");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("ype"));
        Assert.That(chars, Is.EqualTo(1));
        Assert.That(words, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CompletionAndTypeAheadCarryAcrossPromotion()
    {
        typing.ProcessInput("TYPEnE");
        Assert.That(words, Is.EqualTo(1));
        Assert.That(chars, Is.EqualTo(6));
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("xt"));
        Assert.That(Get<int>(typing, "_typeAheadCount"), Is.Zero);
    }

    [Test, Category("Quick")]
    public void CompletionPromotesQueueSynchronouslyWithoutATypeAheadGate()
    {
        typing.ProcessInput("typen");
        Assert.That(chars, Is.EqualTo(5));
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("ext"));
        Assert.That(Get<int>(typing, "_typeAheadCount"), Is.Zero);
    }

    [Test, Category("Quick")]
    public void PracticeWordsKeepProductionMistakeAndInputLockBehavior()
    {
        typing.SetPractice(new[] { "BLADE", "STEEL", "FIRE" }, false, true);
        GamePauseController.SetTutorialReading(true);
        try
        {
            typing.ProcessInput("blade");
            Assert.That(chars, Is.Zero); Assert.That(words, Is.Zero);
            Assert.That(Get<string>(typing, "current"), Is.EqualTo("BLADE"));
        }
        finally { GamePauseController.SetTutorialReading(false); }
        typing.ProcessInput("blz");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("BLADE"));
        typing.ProcessInput("blade");
        Assert.That(words, Is.EqualTo(1));
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("STEEL"));
        Assert.That(Get<string>(typing, "next"), Is.EqualTo("FIRE"));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReplacingPracticeWordsCancelsPreviousPromotionAndParryLessonIgnoresLetters()
    {
        typing.ProcessInput("type");
        typing.SetPractice(new[] { "FIRE" }, false, false);
        int accepted = chars, completed = words;
        typing.ProcessInput("FIRE");
        yield return new WaitForSecondsRealtime(.5f);
        Assert.That(chars, Is.EqualTo(accepted)); Assert.That(words, Is.EqualTo(completed));
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("FIRE"));
        Assert.That(Get<bool>(typing, "isTransitioning"), Is.False);
        typing.SetPractice(new[] { "a" }, false, true);
        typing.ProcessInput("a");
        Assert.That(Get<float>(typing, "slideRemaining"), Is.GreaterThan(0), "A new practice occurrence does not inherit an old burst counter");
    }

    [Test, Category("Quick")]
    public void TimeoutAdvancesImmediatelyAndResetsTimer()
    {
        Set(typing, "timer", -1f);
        Call(typing, "HandleTimer");
        Call(typing, "HandleTimer");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("next"));
        Assert.That(timeouts, Is.EqualTo(1));
        Assert.That(words, Is.Zero);
    }

    [Test, Category("Quick")]
    public void MistakesResetWholeWordWithoutRefillingTimer()
    {
        Set(typing, "timer", 1.1f);
        typing.ProcessInput("tyzz");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("type"));
        Assert.That(Get<float>(typing, "timer"), Is.EqualTo(1.1f));
        Assert.That(words, Is.Zero);
        typing.ProcessInput("type");
        Assert.That(words, Is.EqualTo(1));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator TypeAheadMistakeOnlyResetsNextAndKeepsItsRewardHistory()
    {
        TypingManager.MuteWordCorrect = true;
        typing.ProcessInput("typenexzn");
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("ext"));
        Assert.That(chars, Is.EqualTo(7));
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("ext"));
        typing.ProcessInput("ext");
        Assert.That(chars, Is.EqualTo(8));
    }

    [Test, Category("Quick")]
    public void SlayerRetypingCountsAccuracyButDoesNotRewardPositionsAgain()
    {
        TypingManager.MuteWordCorrect = true;
        int total = 0, correctCount = 0;
        System.Action<bool> result = ok => { total++; if (ok) correctCount++; };
        TypingManager.OnInputResult += result;
        try { typing.ProcessInput("tyztype"); }
        finally { TypingManager.OnInputResult -= result; }
        Assert.That(total, Is.EqualTo(7));
        Assert.That(correctCount, Is.EqualTo(6));
        Assert.That(chars, Is.EqualTo(4));
    }

    [Test, Category("Quick")]
    public void EightArmedSwingsKillFortyHpPlayerOnlyOncePerSwing()
    {
        var player = Own(new GameObject("player")); player.SetActive(false);
        var hp = player.AddComponent<PlayerHealth>(); hp.maxHealth = 40; hp.postHitIFrames = 0;
        player.SetActive(true);
        var enemy = Own(new GameObject("enemy")); enemy.SetActive(false);
        var strike = enemy.AddComponent<AttackStrike>();
        Set(strike, "damageReceiver", hp);
        enemy.SetActive(true);
        for (int i = 1; i <= 8; i++)
        {
            strike.BeginSwing(); strike.Anim_DealDamage(); strike.Anim_DealDamage();
            Assert.That(hp.Current, Is.EqualTo(40 - 5 * i));
        }
        Assert.That(hp.IsDead, Is.True);
    }

    [Test, Category("Quick")]
    public void ResultFreezesAtLethalHitAndRejectsLaterInput()
    {
        var session = root.AddComponent<CombatSession>();
        typing.ProcessInput("tz");
        session.Complete(true);
        Assert.That(session.Letters, Is.EqualTo(2));
        Assert.That(session.Correct, Is.EqualTo(1));
        typing.ProcessInput("type");
        session.Complete(false);
        Assert.That(session.Letters, Is.EqualTo(2));
        Assert.That(words, Is.Zero);
    }

    [Test, Category("Quick")]
    public void ParryQueuesOneAttackAndClearsForSlayerOrDeath()
    {
        var controller = root.AddComponent<PlayerController>();
        controller.animator = root.AddComponent<Animator>();
        controller.attack = Own(ScriptableObject.CreateInstance<AttackConfig>());
        controller.gate = root.AddComponent<PlayerInputGate>();
        controller.gate.PushParryPose(this);
        Call(controller, "DoAttack"); Call(controller, "DoAttack");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        TypingManager.MuteWordCorrect = true;
        Call(controller, "Update");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
        TypingManager.MuteWordCorrect = false;
        Call(controller, "DoAttack");
        controller.gate.SetDeadLocked(true); Call(controller, "Update");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseStopsInputStatisticsPromotionAndQueuedAttack()
    {
        var session = root.AddComponent<CombatSession>();
        var controller = root.AddComponent<PlayerController>();
        controller.animator = root.AddComponent<Animator>();
        controller.attack = Own(ScriptableObject.CreateInstance<AttackConfig>());
        controller.gate = root.AddComponent<PlayerInputGate>();
        controller.gate.PushParryPose(this);
        Call(controller, "DoAttack");
        var pause = root.AddComponent<GamePauseController>();
        typing.ProcessInput("type");
        pause.TogglePause(true);
        controller.gate.PopParryPose(this);
        Call(controller, "Update");
        typing.ProcessInput("next");
        float before = session.Seconds;
        yield return new WaitForSecondsRealtime(.45f);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("next"));
        Assert.That(session.Letters, Is.EqualTo(4));
        Assert.That(session.Seconds, Is.EqualTo(before));
        controller.animator = null;
        pause.Resume(); Call(controller, "Update");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator IdenticalWordsInDifferentOccurrencesRewardNormally()
    {
        TypingManager.MuteWordCorrect = true;
        Set(typing, "next", "type");
        typing.ProcessInput("tyztype");
        yield return new WaitForSecondsRealtime(.4f);
        typing.ProcessInput("type");
        Assert.That(chars, Is.EqualTo(8));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator RealSkeletonAnimationTelegraphsThenResolvesOneImpact()
    {
        var player = Own(new GameObject("impact target"));
        var hp = player.AddComponent<PlayerHealth>(); hp.postHitIFrames = 0;
        var enemy = Own(new GameObject("animated skeleton")); enemy.SetActive(false);
        var animator = enemy.AddComponent<Animator>();
#if UNITY_EDITOR
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
        var skeleton = enemy.AddComponent<SkeletonController>();
        Set(skeleton, "animator", animator);
        var strike = enemy.AddComponent<AttackStrike>(); Set(strike, "damageReceiver", hp);
        var cue = Own(new GameObject("cue")); cue.SetActive(false); Set(skeleton, "attackCue", cue);
        enemy.SetActive(true);
        animator.Update(0);
        Set(skeleton, "_nextAttackAt", -1f); Call(skeleton, "Update");
        Assert.That(skeleton.IsAttacking, Is.True, "do not end against the previous animator frame");
        animator.Update(0); animator.Update(.72f);
        Assert.That(cue.activeSelf, Is.True, "real clip opens the cue at .7s");
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth));
        animator.Update(.31f);
        Assert.That(cue.activeSelf, Is.False, "impact hides the cue at 1s");
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth - 5));
        strike.Anim_DealDamage();
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth - 5));
        skeleton.enabled = false; skeleton.Anim_Telegraph();
        Assert.That(cue.activeSelf, Is.False, "disabled/stunned controller cannot replay a late cue event");
        yield return null;
    }

    [UnityTest, Category("Quick")]
    public IEnumerator DeathDuringDisabledStunFinishesRealDeathClipAndShowsVictory()
    {
        var pause = root.AddComponent<GamePauseController>();
        var enemy = Own(new GameObject("terminal stun regression")); enemy.SetActive(false);
        var animator = enemy.AddComponent<Animator>();
#if UNITY_EDITOR
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
        var skeleton = enemy.AddComponent<SkeletonController>();
        Set(skeleton, "animator", animator); Set(skeleton, "gamePause", pause);
        var stun = enemy.AddComponent<EnemyStunController>(); Set(stun, "animator", animator);
        bool finished = false;
        skeleton.OnDeathFinished += () => finished = true;
        enemy.SetActive(true); animator.Update(0);
        stun.TriggerStun(.15f);
        stun.enabled = false; // The Slayer hit-pose driver disables it this way.
        skeleton.TakeDamage(skeleton.maxHealth, Vector2.zero);
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(stun.IsStunned, Is.False);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.skeleton_die"), Is.True);
        float deadline = Time.realtimeSinceStartup + 2f;
        while (!finished && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(finished, Is.True, "the real death clip must deliver its cleanup event");
        Assert.That(Get<bool>(pause, "_overlayLocked"), Is.True);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator UserMusicGainSurvivesFadeDuckAndRestoreIncludingZero()
    {
        bool hadPreference = PlayerPrefs.HasKey("MusicVolume");
        float saved = AudioPreferences.Music;
        var host = Own(new GameObject("music gain test"));
        var music = host.AddComponent<MusicPlayer>();
        var cue = Own(ScriptableObject.CreateInstance<MusicCue>());
        Set(cue, "singleClip", Own(AudioClip.Create("offline music", 44100, 1, 44100, false)));
        Set(cue, "volume", .5f);
        try
        {
            PlayerPrefs.SetFloat("MusicVolume", 0);
            music.Play(cue, 0, .05f, .01);
            yield return new WaitForSecondsRealtime(.1f);
            var voices = host.GetComponentsInChildren<AudioSource>();
            foreach (var voice in voices) Assert.That(voice.volume, Is.Zero);
            PlayerPrefs.SetFloat("MusicVolume", .6f); Call(music, "LateUpdate");
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.3f).Within(.001f));
            music.DuckTo(.1f, 0);
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.03f).Within(.001f));
            PlayerPrefs.SetFloat("MusicVolume", .8f); Call(music, "LateUpdate");
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.04f).Within(.001f));
            music.RestoreFromDuck(0);
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.4f).Within(.001f));
            music.TransitionGain = .5f; Call(music, "LateUpdate");
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.2f).Within(.001f));
            music.TransitionGain = 1;
            var binder = host.AddComponent<MusicOnSlayerMode>();
            Set(binder, "_lastActive", true);
            music.DuckTo(.1f, 0); binder.enabled = false;
            music.Play(cue, 0, .5f, .01);
            yield return new WaitForSecondsRealtime(.6f);
            Assert.That(music.CurrentMasterVolume, Is.EqualTo(1).Within(.001f));
            foreach (var voice in voices) Assert.That(voice.volume, Is.EqualTo(.4f).Within(.001f));
        }
        finally
        {
            if (hadPreference) PlayerPrefs.SetFloat("MusicVolume", saved);
            else PlayerPrefs.DeleteKey("MusicVolume");
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LosingFocusPausesCombatUntilExplicitResume()
    {
        var session = root.AddComponent<CombatSession>();
        var pause = root.AddComponent<GamePauseController>();
        typeof(GamePauseController).GetMethod("OnApplicationFocus", Private).Invoke(pause, new object[] { false });
        Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(Time.timeScale, Is.Zero);
        float before = session.Seconds;
        typing.ProcessInput("type");
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That(session.Seconds, Is.EqualTo(before));
        Assert.That(session.Letters, Is.Zero);
        typeof(GamePauseController).GetMethod("OnApplicationFocus", Private).Invoke(pause, new object[] { true });
        Assert.That(GamePauseController.InputBlocked, Is.True);
        pause.Resume();
        Assert.That(GamePauseController.InputBlocked, Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ExtraLettersAfterCompletedTypeAheadCountWithoutReopeningTheWord()
    {
        int total = 0;
        System.Action<bool> result = _ => total++;
        TypingManager.OnInputResult += result;
        try { typing.ProcessInput("typenextz"); }
        finally { TypingManager.OnInputResult -= result; }
        Assert.That(total, Is.EqualTo(9));
        Assert.That(Get<string>(typing, "current"), Is.EqualTo("third"));
        yield return new WaitForSecondsRealtime(.7f);
        Assert.That(words, Is.EqualTo(2));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator RealParryRequestLocksImmediatelyAndReleasesOneQueuedSlash()
    {
        var player = Own(new GameObject("real parry player")); player.SetActive(false);
        var animator = player.AddComponent<Animator>();
        var gate = player.AddComponent<PlayerInputGate>();
        var parry = player.AddComponent<ParrySystem>(); parry.logs = false;
        var controller = player.AddComponent<PlayerController>();
        player.AddComponent<PlayerAttackEvents>(); player.AddComponent<SfxOnParryPose>();
#if UNITY_EDITOR
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        parry.config = UnityEditor.AssetDatabase.LoadAssetAtPath<ParryConfig>("Assets/Data/Attack/PC_Parry.asset");
        controller.attack = UnityEditor.AssetDatabase.LoadAssetAtPath<AttackConfig>("Assets/Data/Attack/AC_Attack1.asset");
#endif
        parry.playerAnimator = animator; parry.gate = gate;
        controller.animator = animator; controller.gate = gate;
        player.SetActive(true); animator.Update(0);
        Assert.That(parry.TryBeginParry(), Is.True);
        Assert.That(gate.IsParryPoseLocked, Is.True, "lock before deferred animator evaluation");
        typing.ProcessInput("type");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        animator.Update(0); animator.Update(.1f);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, Is.EqualTo(Animator.StringToHash(parry.config.playerParryPoseStatePath)));
        yield return new WaitForSeconds(.8f);
        Assert.That(gate.IsParryPoseLocked, Is.False);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
        Assert.That(words, Is.EqualTo(1));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseHoldsActiveStunAndStaminaRegeneration()
    {
        var enemy = Own(new GameObject("paused stamina")); enemy.SetActive(false);
        var stamina = enemy.AddComponent<EnemyStamina>(); stamina.max = 100; stamina.hitCost = 10;
        Set(stamina, "regenDelaySeconds", .05f); Set(stamina, "regenTickInterval", .05f);
        var stun = enemy.AddComponent<EnemyStunController>();
        enemy.SetActive(true);
        stamina.ConsumeHit(); stun.TriggerStun(.2f);
        var pause = root.AddComponent<GamePauseController>(); pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(stamina.Current, Is.EqualTo(90));
        Assert.That(stun.IsStunned, Is.True);
        pause.Resume();
        yield return new WaitForSecondsRealtime(.3f);
        Assert.That(stamina.Current, Is.GreaterThan(90));
        Assert.That(stun.IsStunned, Is.False);
    }

    [Test, Category("Quick")]
    public void InterruptedParryClosesItsWindowAndRejectsLateOpenEvents()
    {
        var gate = root.AddComponent<PlayerInputGate>();
        var parry = root.AddComponent<ParrySystem>(); parry.gate = gate; parry.logs = false;
        parry.Anim_ParryOpen();
        Assert.That(parry.IsWindowActive, Is.True);
        gate.SetHitLocked(true); Call(parry, "Update");
        Assert.That(parry.IsWindowActive, Is.False);
        parry.Anim_ParryOpen();
        Assert.That(parry.IsWindowActive, Is.False);
        gate.SetHitLocked(false); parry.Anim_ParryOpen();
        gate.SetDeadLocked(true); Call(parry, "Update");
        Assert.That(parry.IsWindowActive, Is.False);
    }

    [Test, Category("Quick")]
    public void PoolNormalizesDeduplicatesAndClearsEmptyCache()
    {
        var pool = Own(ScriptableObject.CreateInstance<WordPoolConfig>());
        Set(pool, "rawWords", " TYPE \r\ntype\n\nNEXT ");
        pool.BuildCache();
        CollectionAssert.AreEquivalent(new[] { "type", "next" }, Get<string[]>(pool, "_cached"));
        Set(pool, "rawWords", " ");
        pool.BuildCache();
        Assert.That(pool.GetRandomWord(), Is.Null);
    }

    [Test, Category("Quick")]
    public void GateKeepsIndependentPoseSourcesAndDeathLock()
    {
        var gate = root.AddComponent<PlayerInputGate>();
        var a = new object(); var b = new object();
        gate.PushParryPose(a); gate.PushParryPose(b); gate.PopParryPose(a);
        Assert.That(gate.CanAttack, Is.False);
        gate.PopParryPose(b);
        Assert.That(gate.CanParry, Is.True);
        gate.SetHitLocked(true);
        Assert.That(gate.CanAttack, Is.False);
        gate.SetHitLocked(false); gate.SetDeadLocked(true);
        Assert.That(gate.CanParry, Is.False);
    }

    [Test, Category("Quick")]
    public void DamageParryAndDeathRespectHealthAndGate()
    {
        var gate = root.AddComponent<PlayerInputGate>();
        var parry = root.AddComponent<ParrySystem>();
        parry.logs = false; parry.gate = gate;
        var hp = root.AddComponent<PlayerHealth>();
        hp.gate = gate; hp.parry = parry; hp.postHitIFrames = 0;
        hp.TakeDamage(3, Vector2.zero);
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth - 3));
        int successes = 0;
        parry.OnParrySuccess += _ => successes++;
        parry.Anim_ParryOpen();
        hp.TakeDamage(5, Vector2.zero);
        Assert.That(successes, Is.EqualTo(1));
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth - 3));
        Assert.That(parry.IsSuccessActive, Is.True);
        hp.parry = null;
        hp.TakeDamage(1000, Vector2.zero);
        Assert.That(hp.IsDead, Is.True);
        Assert.That(gate.IsDeadLocked, Is.True);
        hp.FullRestore();
        Assert.That(hp.Current, Is.EqualTo(hp.maxHealth));
        Assert.That(gate.CanAttack, Is.True);
    }

    [Test, Category("Quick")]
    public void EnemyStrikeResolvesOpenParryWindow()
    {
        root.AddComponent<PlayerHealth>();
        var target = root.AddComponent<ParryTarget>();
        var parry = root.AddComponent<ParrySystem>();
        parry.enabled = false;
        parry.logs = false; parry.enemy = target;
        parry.enabled = true;
        int successes = 0;
        parry.OnParrySuccess += _ => successes++;
        parry.Anim_ParryOpen(); target.Anim_StrikeFrame();
        Assert.That(successes, Is.EqualTo(1));
        Assert.That(parry.IsWindowActive, Is.False);
        Assert.That(parry.IsSuccessActive, Is.True);
    }

    [Test, Category("Quick")]
    public void ComboActivatesAtThresholdAndTimeoutResetsMultiplier()
    {
        var go = Own(new GameObject("score")); go.SetActive(false);
        var score = go.AddComponent<ScoreSystem>();
        var rules = Own(ScriptableObject.CreateInstance<ScoreRules>());
        Set(score, "rules", rules);
        go.SetActive(true);
        for (int i = 0; i < 4; i++) Call(score, "OnWordCorrect");
        Assert.That(score.Score, Is.EqualTo(40));
        Assert.That(score.Multiplier, Is.EqualTo(1));
        Call(score, "OnWordCorrect");
        Assert.That(score.Multiplier, Is.EqualTo(1.15f).Within(.001f));
        int before = score.Score;
        Set(typing, "timer", -1f); Call(typing, "HandleTimer");
        Assert.That(score.Score, Is.EqualTo(before));
        Assert.That(score.Multiplier, Is.EqualTo(1));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator DifficultyComboUsesAssetBaseAcrossRenewalPauseExpiryResetAndTutorial()
    {
        var host = Own(new GameObject("difficulty combo")); host.SetActive(false);
        var score = host.AddComponent<ScoreSystem>();
        var rules = Own(ScriptableObject.CreateInstance<ScoreRules>());
        rules.comboWindowSeconds = 1.5f; rules.minWordsForCombo = 1;
        Set(score, "rules", rules); host.SetActive(true); score.enabled = false;
        try
        {
            foreach (var difficulty in EndlessDifficulties.All)
            {
                EndlessRun.Begin(difficulty); score.ResetScore();
                Call(score, "OnWordCorrect");
                float expected = 1.5f / difficulty.Pace();
                Assert.That(Get<float>(score, "_windowRemain"), Is.EqualTo(expected));
                Assert.That(rules.comboWindowSeconds, Is.EqualTo(1.5f), "Shared asset must stay immutable");
                Set(score, "_windowRemain", .001f);
                score.SetPaused(true); Call(score, "Update");
                Assert.That(Get<float>(score, "_windowRemain"), Is.EqualTo(.001f));
                score.SetPaused(false);
                Call(score, "OnWordCorrect");
                Assert.That(Get<float>(score, "_windowRemain"), Is.EqualTo(expected));
                Set(score, "_windowRemain", Time.unscaledDeltaTime + .001f);
                Call(score, "Update");
                Assert.That(score.Multiplier, Is.GreaterThan(1));
                Assert.That(score.ComboRemain01, Is.EqualTo(.001f / expected).Within(.0001f));
                Set(score, "_windowRemain", - .001f); Call(score, "Update");
                Assert.That(score.Multiplier, Is.EqualTo(1)); Assert.That(score.ComboRemain01, Is.Zero);
                Call(score, "OnWordCorrect"); score.ResetScore();
                Assert.That(Get<float>(score, "_windowRemain"), Is.Zero);
            }
            EndlessRun.Begin(EndlessDifficulty.VeryHard); TutorialRun.Begin();
            Assert.That(score.ComboWindowSeconds, Is.EqualTo(1.5f));
            yield return null;
        }
        finally { TutorialRun.End(); EndlessRun.Clear(); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator HitstopRestartBeforeExpiredCoroutineResumesRestoresOriginalScale()
    {
        var hitstop = Own(new GameObject("hitstop")).AddComponent<HitStopper>();
        foreach (bool stacking in new[] { false, true })
        {
            hitstop.allowStacking = stacking;
            Time.timeScale = .75f;
            hitstop.Stop(.05f);
            Assert.That(Time.timeScale, Is.EqualTo(hitstop.frozenTimeScale));
            // A new strike can execute in Update after the deadline but before
            // the old coroutine continuation runs on the same slow frame.
            Set(hitstop, "restoreAt", Time.unscaledTime - 1f);
            hitstop.Stop(.01f);
            yield return new WaitForSecondsRealtime(.05f);
            Assert.That(Time.timeScale, Is.EqualTo(.75f), "restore the pre-hitstop scale, including slow motion");
        }
    }

    [Test, Category("Stress")]
    public void LargeWordPoolBuildsAndReusesCache()
    {
        var pool = Own(ScriptableObject.CreateInstance<WordPoolConfig>());
        var lines = new string[50000];
        for (int i = 0; i < lines.Length; i++) lines[i] = "word" + i;
        Set(pool, "rawWords", string.Join("\n", lines)); pool.BuildCache();
        var cache = Get<string[]>(pool, "_cached");
        Assert.That(cache.Length, Is.EqualTo(50000));
        for (int i = 0; i < 10000; i++) Assert.That(pool.GetRandomWord(), Is.Not.Null);
        Assert.That(Get<string[]>(pool, "_cached"), Is.SameAs(cache));
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ScorePulsesOnlyOnAwardAndUsesConfiguredComboTiers()
    {
        var host = Own(new GameObject("score presentation")); host.SetActive(false);
        var labelHost = new GameObject("score", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelHost.transform.SetParent(root.transform, false);
        var label = labelHost.GetComponent<TextMeshProUGUI>();
        var score = host.AddComponent<ScoreSystem>();
        var rules = Own(ScriptableObject.CreateInstance<ScoreRules>());
        Set(score, "rules", rules); Set(score, "scoreText", label);
        host.SetActive(true);
        Assert.That(label.transform.localScale, Is.EqualTo(Vector3.one), "initial rendering must not award a pulse");
        yield return new WaitForSecondsRealtime(.1f);
        Call(score, "OnWordCorrect");
        Assert.That(score.Score, Is.EqualTo(10));
        Assert.That(label.transform.localScale.x, Is.GreaterThan(1));
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(label.transform.localScale, Is.EqualTo(Vector3.one), "decay updates must not restart a pulse");
        for (int i = 0; i < 10; i++) Call(score, "OnWordCorrect");
        Assert.That(score.Multiplier, Is.GreaterThanOrEqualTo(2));
        Assert.That(label.color, Is.EqualTo(rules.colorTier2));
        for (int i = 0; i < 10; i++) Call(score, "OnWordCorrect");
        Assert.That(score.Multiplier, Is.EqualTo(3));
        Assert.That(label.color, Is.EqualTo(rules.colorTier3));
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(label.transform.localScale, Is.EqualTo(Vector3.one));
        score.BreakCombo();
        Assert.That(label.color, Is.EqualTo(rules.colorTier1));
        Assert.That(label.transform.localScale, Is.EqualTo(Vector3.one), "reset must not pulse");
    }

    [UnityTest, Category("Stress")]
    public IEnumerator ParryVfxBurstsReuseInstancesAndResetScale()
    {
        GameObject prefab = null;
#if UNITY_EDITOR
        prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/VFX/VFX_HitSpark_Parry.prefab");
#endif
        Assert.That(prefab, Is.Not.Null, "Stress uses the real offline project prefab in the Editor");
        var host = Own(new GameObject("VFX host")); host.SetActive(false);
        var parry = host.AddComponent<ParrySystem>(); parry.logs = false;
        var spawner = host.AddComponent<ParrySparkVFX_Pooled>();
        Set(spawner, "parrySystem", parry); Set(spawner, "sparkPrefab", prefab);
        Set(spawner, "playerSparkAnchor", host.transform);
        host.SetActive(true);
        var seen = new HashSet<int>();
        for (int burst = 0; burst < 100; burst++)
        {
            for (int i = 0; i < 16; i++) parry.ParrySuccess();
            var active = UnityEngine.Object.FindObjectsOfType<PooledVFX>();
            foreach (var spark in active)
                if (seen.Add(spark.GetInstanceID())) Own(spark.gameObject);
            Assert.That(active.Length, Is.EqualTo(16));
            foreach (var spark in active)
            {
                spark.transform.localScale = Vector3.one * 7;
            }
            // Exercise the actual serialized Animator/clip event rather than
            // returning a synthetic spark to the pool by calling release ourselves.
            float deadline = Time.realtimeSinceStartup + 2f;
            while (Array.Exists(active, spark => spark && spark.gameObject.activeSelf) && Time.realtimeSinceStartup < deadline)
                yield return null;
            foreach (var spark in active)
            {
                Assert.That(spark, Is.Not.Null);
                Assert.That(spark.gameObject.activeSelf, Is.False);
                Assert.That(spark.transform.localScale, Is.EqualTo(Vector3.one));
            }
        }
        Assert.That(seen.Count, Is.EqualTo(16));
    }

    [UnityTest, Category("Stress")]
    public IEnumerator ThousandWordsCompleteThroughProductionInput()
    {
        Set(typing, "fallbackWord", "type");
        Set(typing, "next", "type"); Set(typing, "third", "type");
        for (int i = 0; i < 1000; i++)
        {
            typing.ProcessInput(Get<string>(typing, "current"));
            yield return new WaitUntil(() => !Get<bool>(typing, "pendingAdvance"));
        }
        Assert.That(words, Is.EqualTo(1000));
        Assert.That(chars, Is.EqualTo(4000));
    }
}
