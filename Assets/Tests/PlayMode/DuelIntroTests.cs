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
        Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(((System.Collections.Generic.HashSet<CanvasGroup>)Field(intro, "wordHud")).Count, Is.GreaterThan(0), "word tray is bound independently from status HUD");
        Assert.That(Time.timeScale, Is.EqualTo(1)); Assert.That(AudioListener.pause, Is.False);
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>(true))
            Assert.That(renderer.forceRenderingOff, Is.True, renderer.name + " is hidden before reveal");
        foreach (var group in Find<GamePauseController>().IntroHud) Assert.That(group.alpha, Is.Zero);
        float start = Time.realtimeSinceStartup;
        while (intro.Running && Time.realtimeSinceStartup - start < 6)
        {
            typing.ProcessInput(word + "wrong ");
            player.TakeDamage(5, Vector2.zero); enemy.TakeDamage(5, Vector2.zero);
            Assert.That(player.Current, Is.EqualTo(hp)); Assert.That(enemy.Current, Is.EqualTo(enemyHp));
            Assert.That(CombatSession.I.Seconds, Is.Zero); Assert.That(CombatSession.I.Letters, Is.Zero);
            Assert.That((float)Field(intro, "wordAlpha"), Is.LessThanOrEqualTo((float)Field(intro, "hudAlpha")), "word tray follows status HUD");
            Assert.That(ScoreSystem.I.Score, Is.Zero); Assert.That(enemy.IsAttacking, Is.False);
            Assert.That(Field(typing, "current"), Is.EqualTo(word)); Assert.That(Field(typing, "timer"), Is.EqualTo(timer));
            yield return null;
        }
        Assert.That(intro.Running, Is.False, $"intro elapsed={intro.Elapsed}, paused={GamePauseController.IsPaused}, frozen={GamePauseController.PresentationFrozen}");
        Assert.That(Time.realtimeSinceStartup - start, Is.InRange(2.1f, 3.5f));
        Assert.That((float)Field(enemy, "_nextAttackAt") - Time.time, Is.InRange(.4f, .51f));
        yield return Finished();
        Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(enemy.GetComponent<Renderer>().forceRenderingOff, Is.False);
        typing.ProcessInput(word.Substring(0, 1)); Assert.That(CombatSession.I.Letters, Is.EqualTo(1));
        Assert.That(Field(ScoreSystem.I, "_paused"), Is.False);
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
        var dust = Find<ParticleSystem>();
        yield return new WaitForSecondsRealtime(.25f);
        Assert.That(intro.Elapsed, Is.EqualTo(held)); Assert.That(GamePauseController.IsPaused, Is.True);
        Assert.That(Time.timeScale, Is.Zero); Assert.That(AudioListener.pause, Is.True);
        pause.Resume();
        // Resume completes the existing camera tween before releasing the pause.
        // Wait for the behavior rather than assuming a fixed number of editor frames.
        deadline = Time.realtimeSinceStartup + 3;
        while (intro.Elapsed <= held && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(GamePauseController.IsPaused, Is.False);
        Assert.That(intro.Elapsed, Is.GreaterThan(held));
        yield return Finished(); Assert.That(GamePauseController.InputBlocked, Is.False);
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
        while (intro.Elapsed < .4f && Time.realtimeSinceStartup < deadline) yield return null;
        Assert.That(intro.Elapsed, Is.GreaterThanOrEqualTo(.4f));
        session.ReturnToMenu(); yield return Finished();
        Assert.That(session.World, Is.SameAs(world)); Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(Object.FindObjectOfType<SkeletonController>(), Is.Null);
        Assert.That(intro.Running, Is.False);
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
        var paint = typeof(DuelIntroPresentation).GetMethod("PaintEnemyReveal", BindingFlags.Instance | BindingFlags.NonPublic);
        paint.Invoke(intro, new object[] { .5f });
        renderer.GetPropertyBlock(block);
        Assert.That(block.GetVector(reveal).x, Is.EqualTo(1));
        Assert.That(block.GetVector(reveal).y, Is.InRange(renderer.bounds.min.y, renderer.bounds.max.y));
        Assert.That(block.GetFloat(marker), Is.EqualTo(.123f));
        Assert.That(renderer.forceRenderingOff, Is.False);
        foreach (var child in enemy.GetComponentsInChildren<Renderer>())
            if (!child.sharedMaterial.HasProperty(reveal)) Assert.That(child.forceRenderingOff, Is.True, "unclipped shadow waits until body is complete");
        paint.Invoke(intro, new object[] { 1f });
        renderer.GetPropertyBlock(block);
        Assert.That(block.GetVector(reveal), Is.EqualTo(Vector4.zero));
        Assert.That(block.GetFloat(marker), Is.EqualTo(.123f));
        // Simulate a live property change by another owner; cancellation must not replace its block.
        renderer.GetPropertyBlock(block); block.SetFloat(marker, .234f); renderer.SetPropertyBlock(block);
        paint.Invoke(intro, new object[] { .5f }); session.StopAllCoroutines(); intro.Cancel();
        renderer.GetPropertyBlock(block);
        Assert.That(block.GetVector(reveal), Is.EqualTo(Vector4.zero));
        Assert.That(block.GetFloat(marker), Is.EqualTo(.234f));
        Assert.That(renderer.forceRenderingOff, Is.False);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator TutorialOnlyUncoversAndRetainsItsOwnReadingLock()
    {
        TutorialRun.Begin(); session.BeginGame(); yield return Finished();
        Assert.That(TutorialRun.Active, Is.True); Assert.That(GamePauseController.InputBlocked, Is.True);
        Assert.That(Time.timeScale, Is.Zero);
        Assert.That(Find<DuelIntroPresentation>().Running, Is.False);
        Assert.That(Find<SkeletonController>().GetComponent<Renderer>().forceRenderingOff, Is.False);
        Assert.That(Find<DuelIntroPresentation>().transform.GetComponentsInChildren<ParticleSystem>().Length, Is.Zero);
    }
}
