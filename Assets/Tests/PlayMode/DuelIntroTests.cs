using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class DuelIntroTests
{
    DungeonSession session;
    static T Find<T>() where T : Object => Object.FindObjectOfType<T>(true);
    static object Field(object owner, string name) => owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(owner);
    [UnitySetUp] public IEnumerator Setup()
    {
        TutorialRun.End(); EndlessRun.Prepare(); GamePauseController.ResetSessionState();
        session = DungeonSession.I;
        yield return session.LoadWorld(LightingTestAssets.Definition);
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (session) Object.Destroy(session.gameObject);
        yield return null;
        foreach (var name in new[] { "SampleScene", "MainMenu", LightingTestAssets.WorldScene })
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        }
        TutorialRun.End(); EndlessRun.Clear(); GamePauseController.ResetSessionState();
        yield return null;
    }
    IEnumerator Activated()
    {
        float deadline = Time.realtimeSinceStartup + 20;
        while (!Object.FindObjectOfType<SkeletonController>() && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(Object.FindObjectOfType<SkeletonController>(), Is.Not.Null);
        yield return null;
    }
    IEnumerator Finished()
    {
        float deadline = Time.realtimeSinceStartup + 20;
        while (session.Transitioning && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(session.Transitioning, Is.False);
        yield return null; yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator FullIntroBlocksTypingDamageStatisticsAndSchedulesFirstAttackFromHandoff()
    {
        Debug.Log("INTRO_TEST full opening begin");
        session.BeginGame(); yield return Activated();
        Debug.Log("INTRO_TEST opening activated");
        var intro = Find<DuelIntroPresentation>();
        var enemy = Find<SkeletonController>(); var player = Find<PlayerHealth>();
        var typing = Find<TypingManager>();
        int hp = player.Current, enemyHp = enemy.Current;
        string word = (string)Field(typing, "current"); float timer = (float)Field(typing, "timer");
        var wordText = (TMPro.TMP_Text)Field(typing, "wordText");
        string previewInk = ColorUtility.ToHtmlStringRGB((Color)Field(typing, "nextColor"));
        var preview = session.gameObject.AddComponent<DuelIntroPreviewProbe>();
        preview.Intro = intro;
        Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(((System.Collections.Generic.HashSet<CanvasGroup>)Field(intro, "wordHud")).Count, Is.GreaterThan(0), "word tray is bound independently from status HUD");
        Assert.That(Time.timeScale, Is.EqualTo(1)); Assert.That(AudioListener.pause, Is.False);
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>(true))
            Assert.That(renderer.forceRenderingOff, Is.True, renderer.name + " is hidden before reveal");
        foreach (var group in Find<GamePauseController>().IntroHud) Assert.That(group.alpha, Is.Zero);
        float start = Time.realtimeSinceStartup;
        while (intro.Running && Time.realtimeSinceStartup - start < 6)
        {
            var heading = (TMPro.TMP_Text)Field(intro, "label");
            var hudName = Find<CombatR7Status>().IntroEnemyLabel;
            Assert.That(heading.alpha > 0 && hudName.alpha > 0, Is.False, "one opponent name at a time");
            Assert.That(heading.rectTransform.anchoredPosition, Is.EqualTo(Vector2.zero));
            foreach (var actorRenderer in enemy.GetComponentsInChildren<Renderer>(true))
                Assert.That(actorRenderer.forceRenderingOff, Is.EqualTo(intro.Elapsed < 1.03f), "whole pose and shadow switch together");
            typing.ProcessInput(word + "wrong ");
            player.TakeDamage(5, Vector2.zero); enemy.TakeDamage(5, Vector2.zero);
            Assert.That(player.Current, Is.EqualTo(hp)); Assert.That(enemy.Current, Is.EqualTo(enemyHp));
            Assert.That(CombatSession.I.Seconds, Is.Zero); Assert.That(CombatSession.I.Letters, Is.Zero);
            Assert.That((float)Field(intro, "wordAlpha"), Is.LessThanOrEqualTo((float)Field(intro, "hudAlpha")), "word tray follows status HUD");
            Assert.That(ScoreSystem.I.Score, Is.Zero); Assert.That(enemy.IsAttacking, Is.False);
            Assert.That(Field(typing, "current"), Is.EqualTo(word)); Assert.That(Field(typing, "timer"), Is.EqualTo(timer));
            Assert.That((bool)Field(typing, "introPreview"), Is.True);
            Assert.That(wordText.text, Does.Contain("<color=#" + previewInk + ">" + word));
            yield return null;
        }
        Assert.That(intro.Running, Is.False, $"intro elapsed={intro.Elapsed}, paused={GamePauseController.IsPaused}, frozen={GamePauseController.PresentationFrozen}");
        Assert.That(Time.realtimeSinceStartup - start, Is.InRange(2.1f, 3.5f));
        Assert.That(preview.FirstApplied, Is.GreaterThan(0));
        Assert.That(Time.realtimeSinceStartupAsDouble - preview.FirstApplied, Is.GreaterThanOrEqualTo(.3), "300ms from applied alpha to handoff");
        Assert.That((bool)Field(typing, "introPreview"), Is.False);
        Assert.That((float)Field(typing, "introStartRemaining"), Is.GreaterThan(0));
        Assert.That((float)Field(enemy, "_nextAttackAt") - Time.time, Is.InRange(.4f, .51f));
        yield return Finished();
        Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(enemy.GetComponent<Renderer>().forceRenderingOff, Is.False);
        Assert.That(Find<CombatR7Status>().IntroEnemyLabel.alpha, Is.EqualTo(1));
        typing.ProcessInput(word.Substring(0, 1)); Assert.That(CombatSession.I.Letters, Is.EqualTo(1));
        Assert.That((float)Field(typing, "introStartRemaining"), Is.Zero, "typing feedback supersedes start cue");
        Assert.That(Field(ScoreSystem.I, "_paused"), Is.False);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator ShortPreviewPausesWithoutConsumingWordAndCancelDoesNotSignalCombat()
    {
        session.BeginGame(); yield return Finished();
        EndlessRun.Restart(); session.ReloadGameplay();
        yield return new WaitForSecondsRealtime(.3f); yield return Activated();
        var intro = Find<DuelIntroPresentation>(); var typing = Find<TypingManager>();
        float deadline = Time.realtimeSinceStartup + 6;
        while (intro.Elapsed < .71f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(intro.Running, Is.True); Assert.That(intro.Elapsed, Is.LessThan(.9f));
        Assert.That((float)Field(intro, "wordAlpha"), Is.EqualTo(1));
        string word = (string)Field(typing, "current"); float timer = (float)Field(typing, "timer");
        var pause = Find<GamePauseController>(); pause.SendMessage("OnApplicationFocus", false);
        float held = intro.Elapsed;
        typing.ProcessInput(word); yield return new WaitForSecondsRealtime(.2f);
        Assert.That(intro.Elapsed, Is.EqualTo(held));
        Assert.That(Field(typing, "current"), Is.EqualTo(word)); Assert.That(Field(typing, "timer"), Is.EqualTo(timer));
        Assert.That(CombatSession.I.Letters, Is.Zero);
        pause.Resume(); yield return Finished();
        Assert.That(intro.Elapsed, Is.EqualTo(1f));
        // The real handoff starts its color cue; Pause must hold that cue as well.
        typing.BeginIntroPreview(); typing.EndIntroPreview(true);
        pause.SendMessage("OnApplicationFocus", false);
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That((float)Field(typing, "introStartRemaining"), Is.EqualTo(.1f));
        pause.Resume();
        deadline = Time.realtimeSinceStartup + 3;
        while ((GamePauseController.IsPaused || (float)Field(typing, "introStartRemaining") > 0) && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That((float)Field(typing, "introStartRemaining"), Is.Zero);
        typing.BeginIntroPreview(); typing.EndIntroPreview(false);
        Assert.That((bool)Field(typing, "introPreview"), Is.False);
        Assert.That((float)Field(typing, "introStartRemaining"), Is.Zero);
        string activeInk = ColorUtility.ToHtmlStringRGB((Color)Field(typing, "currentColor"));
        Assert.That(((TMPro.TMP_Text)Field(typing, "wordText")).text, Does.Contain("<color=#" + activeInk + ">"));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator PauseAndFocusFreezeIntroAndDustThenResumeAtSameMilestone()
    {
        session.BeginGame(); yield return Activated();
        var intro = Find<DuelIntroPresentation>();
        float deadline = Time.realtimeSinceStartup + 6;
        while (intro.Elapsed < .96f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(intro.Elapsed, Is.GreaterThanOrEqualTo(.96f));
        var pause = Find<GamePauseController>();
        Assert.That(((Canvas)Field(intro, "canvas")).sortingOrder,
            Is.LessThan(((Canvas)Field(pause, "pauseCanvas")).sortingOrder), "Pause stays above the partially closed cover");
        pause.SendMessage("OnApplicationFocus", false);
        float held = intro.Elapsed;
        var dust = (ParticleSystem)Field(intro, "dust");
        var mesh = (Mesh)Field(intro, "smokeMesh");
        Assert.That(dust.GetComponent<ParticleSystemRenderer>().renderMode, Is.EqualTo(ParticleSystemRenderMode.Mesh));
        Assert.That(mesh.vertexCount, Is.EqualTo(20));
        var before = new ParticleSystem.Particle[18]; int count = dust.GetParticles(before);
        yield return new WaitForSecondsRealtime(.25f);
        Assert.That(intro.Elapsed, Is.EqualTo(held)); Assert.That(GamePauseController.IsPaused, Is.True);
        var after = new ParticleSystem.Particle[18]; Assert.That(dust.GetParticles(after), Is.EqualTo(count));
        for (int i = 0; i < count; i++)
        {
            Assert.That(after[i].position, Is.EqualTo(before[i].position));
            Assert.That(after[i].startColor, Is.EqualTo(before[i].startColor));
        }
        Assert.That(Field(intro, "smokeMesh"), Is.SameAs(mesh));
        Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
        pause.Resume();
        // Resume completes the existing camera tween before releasing the pause.
        // Wait for the behavior rather than assuming a fixed number of editor frames.
        deadline = Time.realtimeSinceStartup + 3;
        while (intro.Elapsed <= held && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(intro.Elapsed, Is.GreaterThan(held));
        // Repeat at whole-pose reveal and the label -> HUD name handoff.
        foreach (float milestone in new[] { 1.05f, 1.90f })
        {
            deadline = Time.realtimeSinceStartup + 4;
            while (intro.Elapsed < milestone && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(intro.Running, Is.True);
            var enemy = Find<SkeletonController>();
            Assert.That(enemy.GetComponent<Renderer>().forceRenderingOff, Is.False);
            var heading = (TMPro.TMP_Text)Field(intro, "label");
            var hudName = Find<CombatR7Status>().IntroEnemyLabel;
            pause.SendMessage("OnApplicationFocus", false);
            yield return null; // Observe after presentation LateUpdate commits this frame.
            float titleAlpha = heading.alpha, hudNameAlpha = hudName.alpha;
            count = dust.GetParticles(before); held = intro.Elapsed;
            yield return new WaitForSecondsRealtime(.15f);
            Assert.That(intro.Elapsed, Is.EqualTo(held));
            Assert.That(heading.alpha, Is.EqualTo(titleAlpha));
            Assert.That(hudName.alpha, Is.EqualTo(hudNameAlpha));
            Assert.That(dust.GetParticles(after), Is.EqualTo(count));
            for (int i = 0; i < count; i++) Assert.That(after[i].position, Is.EqualTo(before[i].position));
            Assert.That(Field(intro, "smokeMesh"), Is.SameAs(mesh), "Resume never respawns smoke");
            Assert.That(enemy.GetComponent<Renderer>().forceRenderingOff, Is.False);
            pause.Resume();
            deadline = Time.realtimeSinceStartup + 3;
            while (intro.Elapsed <= held && Time.realtimeSinceStartup < deadline) yield return null;
        }
        yield return Finished(); Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(Field(intro, "smokeMesh"), Is.Null);
        Assert.That(intro.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator RetryUsesShortIntroAndMenuCancelsWithoutLeakingWorldOrCover()
    {
        session.BeginGame(); yield return Activated();
        var world = session.World;
        EndlessRun.Restart(); session.ReloadGameplay();
        // Wait for old actors to disappear before looking for the new scene.
        yield return new WaitForSecondsRealtime(.3f); yield return Activated();
        var intro = Find<DuelIntroPresentation>();
        float deadline = Time.realtimeSinceStartup + 6;
        while (intro.Elapsed < .75f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(intro.Elapsed, Is.GreaterThanOrEqualTo(.75f));
        var pause = Find<GamePauseController>();
        pause.TogglePause(true); yield return null;
        pause.ReturnToMenu(); yield return null;
        var heading = (TMPro.TMP_Text)Field(intro, "label");
        var cover = (RectTransform)Field(intro, "curtain");
        deadline = Time.realtimeSinceStartup + 6;
        while (heading && cover.GetSiblingIndex() < heading.transform.GetSiblingIndex() && Time.realtimeSinceStartup < deadline)
        {
            Assert.That(heading.enabled, Is.False, "navigation never re-shows the paused heading before cover");
            yield return null;
        }
        Assert.That(((RectTransform)Field(intro, "curtain")).GetSiblingIndex(),
            Is.GreaterThan(((TMPro.TMP_Text)Field(intro, "label")).transform.GetSiblingIndex()),
            "navigation cover also occludes the live opponent heading");
        yield return Finished();
        Assert.That(session.World, Is.SameAs(world)); Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(Object.FindObjectOfType<SkeletonController>(), Is.Null);
        Assert.That(intro.Running, Is.False);
        Assert.That(Field(intro, "smokeMesh"), Is.Null);
        Assert.That(intro.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
        Assert.That(intro.transform.Find("Duel presentation/Scene cover").gameObject.activeSelf, Is.False);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator RevealPreservesLiveLightingPropertiesAndRestoresOnCompletionAndCancel()
    {
        session.BeginGame(); yield return Activated();
        var intro = Find<DuelIntroPresentation>(); var enemy = Find<SkeletonController>();
        var renderer = enemy.GetComponent<SpriteRenderer>();
        int reveal = Shader.PropertyToID("_IntroReveal"), marker = Shader.PropertyToID("_FlashAmount");
        Assert.That(renderer.sharedMaterial.GetVector(reveal), Is.EqualTo(Vector4.zero), "default actor material stays fully visible");
        var block = new MaterialPropertyBlock(); renderer.GetPropertyBlock(block); block.SetFloat(marker, .123f); renderer.SetPropertyBlock(block);
        var originalMaterial = renderer.sharedMaterial;
        var name = Find<CombatR7Status>().IntroEnemyLabel;
        float originalAlpha = (float)Field(intro, "enemyHudNameAlpha");
        var revealEnemy = typeof(DuelIntroPresentation).GetMethod("RevealEnemy", BindingFlags.Instance | BindingFlags.NonPublic);
        revealEnemy.Invoke(intro, null);
        renderer.GetPropertyBlock(block);
        Assert.That(block.GetVector(reveal), Is.EqualTo(Vector4.zero));
        Assert.That(block.GetFloat(marker), Is.EqualTo(.123f));
        foreach (var child in enemy.GetComponentsInChildren<Renderer>(true))
            Assert.That(child.forceRenderingOff, Is.False, "whole pose and shadow restored together");
        renderer.GetPropertyBlock(block); block.SetFloat(marker, .234f); renderer.SetPropertyBlock(block);
        session.StopAllCoroutines(); intro.Cancel();
        renderer.GetPropertyBlock(block);
        Assert.That(block.GetVector(reveal), Is.EqualTo(Vector4.zero));
        Assert.That(block.GetFloat(marker), Is.EqualTo(.234f));
        Assert.That(renderer.sharedMaterial, Is.SameAs(originalMaterial));
        Assert.That(name.alpha, Is.EqualTo(originalAlpha));
        yield return null;
        Assert.That(Field(intro, "smokeMesh"), Is.Null);
        Assert.That(intro.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator PauseOnFinalPresentationFrameDefersCombatHandoffUntilResume()
    {
        session.BeginGame(); yield return Activated();
        var intro = Find<DuelIntroPresentation>(); var pause = Find<GamePauseController>();
        var boundary = session.gameObject.AddComponent<DuelIntroHandoffPause>();
        boundary.Intro = intro; boundary.Pause = pause;
        float deadline = Time.realtimeSinceStartup + 6;
        while (!boundary.Triggered && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(boundary.Triggered, Is.True);
        yield return null; yield return null;
        Assert.That(intro.Elapsed, Is.EqualTo(2.4f));
        Assert.That(intro.Running, Is.True, "the final presentation frame must not hand off while frozen");
        Assert.That((bool)Field(Find<TypingManager>(), "introPreview"), Is.True);
        Assert.That(CombatSession.I.Seconds, Is.Zero);
        Assert.That(GamePauseController.InputBlocked, Is.True);
        pause.Resume(); yield return Finished();
        Assert.That(intro.Running, Is.False);
        Assert.That(GamePauseController.InputBlocked, Is.False);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator StallsBeforeFullAlphaPreserveThreeHundredMillisecondsForPlayAndRetry()
    {
        for (int flow = 0; flow < 4; flow++)
        {
            if (flow % 2 == 0)
            {
                if (flow > 0) { session.ReturnToMenu(); yield return Finished(); }
                session.BeginGame();
            }
            else
            {
                var oldEnemy = Find<SkeletonController>();
                EndlessRun.Restart(); session.ReloadGameplay();
                float unloadDeadline = Time.realtimeSinceStartup + 6;
                while (oldEnemy && Time.realtimeSinceStartup < unloadDeadline) yield return null;
                Assert.That(!oldEnemy, Is.True);
            }
            yield return Activated();
            var intro = Find<DuelIntroPresentation>();
            var probe = session.gameObject.AddComponent<DuelIntroPreviewProbe>(); probe.Intro = intro;
            var stall = session.gameObject.AddComponent<DuelIntroPreviewStall>(); stall.Intro = intro;
            stall.Threshold = flow % 2 == 0 ? 1.99f : .59f; stall.Milliseconds = flow < 2 ? 180 : 500;
            float deadline = Time.realtimeSinceStartup + 8;
            while (intro.Running && Time.realtimeSinceStartup < deadline)
            {
                if (stall.Triggered && probe.FirstApplied > 0)
                {
                    Assert.That(GamePauseController.InputBlocked, Is.True);
                    Assert.That(CombatSession.I.Seconds, Is.Zero);
                }
                yield return null;
            }
            yield return null; // Observe handoff after the probe LateUpdate.
            Assert.That(stall.Triggered, Is.True);
            Assert.That(probe.FirstApplied, Is.GreaterThan(0));
            Assert.That(probe.Handoff - probe.FirstApplied, Is.GreaterThanOrEqualTo(.3));
            Assert.That(intro.Running, Is.False);
            Object.Destroy(stall); Object.Destroy(probe);
            yield return Finished();
        }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator PreviewClockPreservesProgressAcrossFocusPauseAndCountsVisibleStall()
    {
        session.BeginGame(); yield return Activated();
        var intro = Find<DuelIntroPresentation>();
        float deadline = Time.realtimeSinceStartup + 6;
        while ((double)Field(intro, "previewSeconds") < .05 && Time.realtimeSinceStartup < deadline) yield return null;
        var pause = Find<GamePauseController>(); pause.SendMessage("OnApplicationFocus", false);
        double saved = (double)Field(intro, "previewSeconds");
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That((double)Field(intro, "previewSeconds"), Is.EqualTo(saved));
        Assert.That(intro.Running, Is.True);
        pause.Resume();
        deadline = Time.realtimeSinceStartup + 3;
        while (GamePauseController.PresentationFrozen && Time.realtimeSinceStartup < deadline) yield return null;
        yield return null;
        Assert.That((double)Field(intro, "previewSeconds"), Is.LessThan(saved + .15));
        // The already painted word remains visible throughout this blocked frame.
        System.Threading.Thread.Sleep(350);
        yield return null;
        Assert.That(!intro.Running || (double)Field(intro, "previewSeconds") >= saved + .35, Is.True,
            "a stall after full alpha counts toward the existing reading credit");
        yield return Finished();
        Assert.That(intro.Running, Is.False);
        Assert.That((double)Field(intro, "previewSeconds"), Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator NavigationDuringExtendedPreviewCancelsOldHandoffAndResetsClock()
    {
        for (int action = 0; action < 2; action++)
        {
            session.BeginGame(); yield return Activated();
            var intro = Find<DuelIntroPresentation>();
            var stall = session.gameObject.AddComponent<DuelIntroPreviewStall>();
            stall.Intro = intro; stall.Threshold = 2f; stall.Milliseconds = 500;
            float deadline = Time.realtimeSinceStartup + 6;
            while (intro.Elapsed < 2.4f && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(stall.Triggered, Is.True); Assert.That(intro.Running, Is.True);
            var pause = Find<GamePauseController>(); pause.TogglePause(true);
            yield return new WaitForSecondsRealtime(.35f);
            Assert.That(intro.Running, Is.True); Assert.That(CombatSession.I.Seconds, Is.Zero);
            if (action == 0) pause.ReturnToMenu(); else pause.Restart();
            Object.Destroy(stall);
            yield return Finished();
            Assert.That((double)Field(intro, "previewSeconds"), Is.Zero);
            if (action == 0) Assert.That(Find<TypingManager>(), Is.Null);
            else Assert.That(CombatSession.I.Letters, Is.Zero);
            Assert.That(intro.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
        }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator TutorialOnlyUncoversAndRetainsItsOwnReadingLock()
    {
        TutorialRun.Begin(); session.BeginGame(); yield return Finished();
        Assert.That(TutorialRun.Active, Is.True); Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(Time.timeScale, Is.Zero);
        Assert.That((bool)Field(Find<TypingManager>(), "introPreview"), Is.False);
        Assert.That(Find<DuelIntroPresentation>().Running, Is.False);
        Assert.That(Find<SkeletonController>().GetComponent<Renderer>().forceRenderingOff, Is.False);
        Assert.That(Find<DuelIntroPresentation>().transform.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
    }
}

// Pause after the coroutine reaches its final frame, before its next continuation.
[DefaultExecutionOrder(31999)]
internal sealed class DuelIntroHandoffPause : MonoBehaviour
{
    internal DuelIntroPresentation Intro;
    internal GamePauseController Pause;
    internal bool Triggered;
    void LateUpdate()
    {
        if (!Triggered && Intro && Intro.Running && Intro.Elapsed >= 2.4f)
        {
            Triggered = true;
            Pause.SendMessage("OnApplicationFocus", false);
        }
    }
}

// Runs before the coroutine updates its timeline, producing a real blocked frame.
internal sealed class DuelIntroPreviewStall : MonoBehaviour
{
    internal DuelIntroPresentation Intro;
    internal float Threshold;
    internal int Milliseconds;
    internal bool Triggered;
    void Update()
    {
        if (!Triggered && Intro && Intro.Running && Intro.Elapsed >= Threshold)
        { Triggered = true; System.Threading.Thread.Sleep(Milliseconds); }
    }
}

[DefaultExecutionOrder(32001)]
internal sealed class DuelIntroPreviewProbe : MonoBehaviour
{
    internal DuelIntroPresentation Intro;
    internal double FirstApplied, Handoff;
    void LateUpdate()
    {
        if (!Intro) return;
        if (Intro.Running && FirstApplied == 0 &&
            (double?)typeof(DuelIntroPresentation).GetField("previewPrevious", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Intro) is double applied)
            FirstApplied = applied;
        if (!Intro.Running && FirstApplied > 0 && Handoff == 0) Handoff = Time.realtimeSinceStartupAsDouble;
    }
}
