using System.Collections;
using System.Reflection;
using Cinemachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class CombatCameraImpactTests
{
    GameObject host, cameraHost;
    CombatCameraImpact impact;
    PlayerHealth health;
    Camera view;
    CombatCameraProfile profile;
    static void Set(object owner, string field, object value) => owner.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(owner, value);
    [SetUp] public void Setup()
    {
        GamePauseController.ResetSessionState(); TutorialRun.End(); Time.timeScale = 1;
        host = new GameObject("impact session"); host.SetActive(false);
        health = host.AddComponent<PlayerHealth>();
        impact = host.AddComponent<CombatCameraImpact>(); Set(impact, "health", health);
        cameraHost = new GameObject("impact camera"); view = cameraHost.AddComponent<Camera>();
        view.transform.position = new Vector3(0, 0, -17.8403f); view.fieldOfView = 35;
        profile = Object.Instantiate(LightingTestAssets.Definition.cameraImpact);
        foreach (var preset in profile.presets) preset.units = CombatCameraProfile.ShakeUnits.WorldUnits;
        host.SetActive(true); impact.Bind(profile); Set(impact, "view", view);
    }
    [TearDown] public void Cleanup()
    {
        Object.DestroyImmediate(host); Object.DestroyImmediate(cameraHost); Object.DestroyImmediate(profile);
        GamePauseController.ResetSessionState(); TutorialRun.End(); Time.timeScale = 1;
    }
    CameraImpactTimeline Timeline(out CombatCameraProfile.Preset preset, int index = 0)
    { preset = profile.Prepare()[index]; return new CameraImpactTimeline(); }
    [TestCase(.14), TestCase(.18), TestCase(.2), TestCase(.5), Category("Quick")]
    public void LongReceivingFrameCannotExpireBeforePeakRendered(double stall)
    {
        var t = Timeline(out var p); t.Play(p, 10, Vector3.zero);
        t.Sample(10, 0, false, false); Assert.That(t.Zoom, Is.Zero, "pre-contact frame time is excluded");
        t.Sample(10 + stall, 0, false, false); Assert.That(t.Zoom, Is.EqualTo(.02f));
        t.Sample(11, 0, false, false); Assert.That(t.Zoom, Is.EqualTo(.02f));
        t.Acknowledge(t.Serial, t.RenderMask, false);
        t.Sample(11.05, 0, false, false); Assert.That(t.Zoom, Is.EqualTo(.01f).Within(.00001));
        t.Sample(11.101, 0, false, false); Assert.That(t.Active, Is.Null);
        Assert.That(t.PeakRenderCount, Is.EqualTo(1));
    }
    [TestCase(.01), TestCase(.019), TestCase(.03), TestCase(.08), Category("Quick")]
    public void StallsBeforeAndAfterPeakNeverReplayIt(double hitchAt)
    {
        var t = Timeline(out var p); t.Play(p, 0, Vector3.zero); t.Sample(hitchAt, 0, false, false);
        if (hitchAt >= .02) t.Acknowledge(t.Serial, t.RenderMask, false);
        t.Sample(hitchAt + .5, 0, false, false);
        if (hitchAt < .02)
        {
            Assert.That(t.Zoom, Is.EqualTo(.02f)); t.Acknowledge(t.Serial, t.RenderMask, false);
            t.Sample(hitchAt + .551, 0, false, false); Assert.That(t.Zoom, Is.InRange(0, .02f));
        }
        t.Sample(hitchAt + 1, 0, false, false); Assert.That(t.Active, Is.Null);
        Assert.That(t.PeakRenderCount, Is.EqualTo(1));
    }
    [Test, Category("Quick")]
    public void SlowWorldRenderStartsZoomRecoveryAtTheActualAcknowledgment()
    {
        var t = Timeline(out var p); t.Play(p, 0, Vector3.zero); t.Sample(.03, .03, false, false);
        t.Acknowledge(t.Serial, t.RenderMask, false, .5);
        t.Sample(.55, .363, false, false); Assert.That(t.Zoom, Is.EqualTo(.01f).Within(.00001f));
        t.Sample(.61, .423, false, false); Assert.That(t.Active, Is.Null);
        Assert.That(t.PeakRenderCount, Is.EqualTo(1));
    }
    [TestCase(.516, true), TestCase(1.5, false), Category("Quick")]
    public void SlowRenderShakeKeepsGameplayRecoveryButNeverReplaysAfterALaterHitch(double nextTime, bool recovering)
    {
        var t = Timeline(out var p, 2); t.Play(p, 0, Vector3.left); t.Sample(.03, .03, false, false);
        t.Acknowledge(t.Serial, t.RenderMask, false, .5);
        t.Sample(nextTime, .363, false, false);
        if (recovering) Assert.That(t.Offset.x, Is.InRange(-1f, -.5f));
        else Assert.That(t.Active, Is.Null, "time after the acknowledged render must not replay the peak");
        t.Sample(nextTime + 1, 1.363, false, false); Assert.That(t.Active, Is.Null);
        Assert.That(t.ShakePeakRenderCount, Is.EqualTo(1));
    }
    [Test, Category("Quick")]
    public void DisabledPresetsAndMinimumOwnershipRespectAuthoredConfiguration()
    {
        var t = Timeline(out var p); p.enabled = false;
        Assert.That(t.Play(p, 0, Vector3.zero), Is.False);
        p.enabled = true; p.minimumPriorityTime = .5f; t.Play(p, 0, Vector3.zero);
        t.Sample(.03, 0, false, false); t.Acknowledge(t.Serial, t.RenderMask, false);
        t.Sample(.2, 0, false, false); Assert.That(t.Zoom, Is.Zero); Assert.That(t.Active, Is.SameAs(p));
        Assert.That(t.Play(p, .2, Vector3.zero), Is.False);
        t.Sample(.51, 0, false, false); Assert.That(t.Active, Is.Null);
    }
    [Test, Category("Quick")]
    public void PriorityPreemptsBeforeRenderAndRejectsEqualOrWeakerWithoutExtension()
    {
        var t = Timeline(out var normal); var all = profile.Prepare();
        Assert.That(t.Play(normal, 0, Vector3.zero), Is.True); t.Sample(.03, 0, false, false);
        int old = t.Serial;
        Assert.That(t.Play(all[1], .03, Vector3.left), Is.True);
        t.Sample(.06, 0, false, false); t.Acknowledge(old, 3, false);
        Assert.That(t.PeakRenderCount, Is.Zero);
        Assert.That(t.Play(all[1], .06, Vector3.left), Is.False);
        Assert.That(t.Play(normal, .06, Vector3.zero), Is.False);
        t.Acknowledge(t.Serial, t.RenderMask, false); Assert.That(t.PeakRenderCount, Is.EqualTo(1));
        Assert.That(t.Play(all[2], .06, Vector3.left), Is.True); t.Sample(.06, 0, false, false);
        Assert.That(t.Zoom, Is.Zero, "received clears outgoing zoom immediately");
    }
    [Test, Category("Quick")]
    public void PausePreservesPendingHitstopAndRenderAndClearInvalidatesAcknowledgment()
    {
        var t = Timeline(out var p, 1); t.Play(p, 0, Vector3.left); t.Sample(.03, 0, true, false);
        var held = t.Zoom; int serial = t.Serial;
        t.Sample(.03, 0, true, true); t.Acknowledge(serial, 3, true);
        Assert.That(t.Zoom, Is.EqualTo(held)); Assert.That(t.Offset, Is.EqualTo(Vector3.zero));
        Assert.That(t.PeakRenderCount, Is.Zero);
        t.Sample(.04, 0, false, false); t.Sample(.05, .03, false, false);
        Assert.That(t.Offset.x, Is.LessThan(-.99));
        t.Clear(); t.Play(p, .05, Vector3.right); t.Sample(.08, .03, false, false);
        t.Acknowledge(serial, 3, false); Assert.That(t.PeakRenderCount, Is.Zero);
        t.Acknowledge(t.Serial, t.RenderMask, false); Assert.That(t.PeakRenderCount, Is.EqualTo(1));
    }
    [Test, Category("Quick")]
    public void ShakeClockWaitsForHitstopAndItsPeakMustRenderBeforeRecovery()
    {
        var t = Timeline(out var p, 2); t.Play(p, 0, Vector3.left);
        t.Sample(.5, 0, true, false); Assert.That(t.Offset, Is.EqualTo(Vector3.zero)); Assert.That(t.Active, Is.Not.Null);
        t.Sample(.51, 0, false, false); t.Sample(.7, .5, false, false);
        Assert.That(t.Offset.x, Is.LessThan(-.99));
        t.Acknowledge(t.Serial, t.RenderMask, false);
        t.Sample(.74, .54, false, false); Assert.That(t.Offset.x, Is.InRange(-.99f, 0));
        t.Sample(.9, .7, false, false); Assert.That(t.Active, Is.Null);
        Assert.That(t.ShakePeakRenderCount, Is.EqualTo(1));
        t.Sample(2, 2, false, false); Assert.That(t.Offset, Is.EqualTo(Vector3.zero));
    }
    [Test, Category("Quick")]
    public void IndependentPreparedInstancesAndRebindingUseTheirWorldsAuthoredValues()
    {
        var a = Timeline(out var p); var b = Timeline(out var q);
        a.Play(p, 0, Vector3.zero); a.Sample(.03, 0, false, false);
        Assert.That(b.Zoom, Is.Zero); Assert.That(q.attackCurve, Is.Not.SameAs(p.attackCurve));
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f); Assert.That(impact.Zoom, Is.EqualTo(.02f));
        var other = UnityEditor.AssetDatabase.LoadAssetAtPath<WorldDefinition>("Assets/Tests/Fixtures/Lighting/OtherWorld.asset");
        impact.Bind(other.cameraImpact); impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.01f));
        impact.Bind(profile); impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.02f));
    }
    [Test, Category("Quick")]
    public void ConfigurePreservesAcceptedTuningAndCurvesWhilePreparingFutureRequests()
    {
        var other = Object.Instantiate(profile);
        try
        {
            var old = profile.presets[1]; old.zoomPeak = .07f; old.attack = .01f; old.hold = .08f;
            old.release = .2f; old.strength = 2; old.shakeDuration = .24f;
            impact.Configure(profile); impact.Parry(Vector3.zero, Vector3.left, .5f);
            old.zoomPeak = .9f; old.strength = 10; old.shakeDuration = 2;
            var next = other.presets[1]; next.zoomPeak = .04f; next.attack = .01f; next.hold = .08f;
            next.release = .1f; next.strength = .5f; next.shakeDuration = .1f;
            next.shakeEnvelope = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, 1), new Keyframe(1, 0));
            impact.Configure(other); impact.Advance(.06f);
            Assert.That(impact.Zoom, Is.EqualTo(.07f).Within(.00001f));
            Assert.That(impact.Offset.magnitude, Is.EqualTo(1).Within(.0001f), "accepted curve/duration/power stay captured");
            impact.Clear(); impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.06f);
            Assert.That(impact.Zoom, Is.EqualTo(.04f).Within(.00001f));
            Assert.That(impact.Offset.magnitude, Is.EqualTo(.5f).Within(.0001f));
        }
        finally { impact.Clear(); impact.Configure(profile); Object.DestroyImmediate(other); }
    }
    [Test, Category("Quick")]
    public void TwoControllersSharingOneAssetKeepIndependentRuntimeState()
    {
        var secondHost = new GameObject("second impact controller"); secondHost.SetActive(false);
        var second = secondHost.AddComponent<CombatCameraImpact>(); second.Bind(profile); secondHost.SetActive(true);
        string authored = JsonUtility.ToJson(profile);
        try
        {
            impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
            Assert.That(second.Zoom, Is.Zero); Assert.That(second.Serial, Is.Zero);
            second.Hit(Vector3.zero, Vector3.right, false, false); second.Advance(.03f);
            impact.Clear(); Assert.That(second.Zoom, Is.EqualTo(.02f));
            Assert.That(JsonUtility.ToJson(profile), Is.EqualTo(authored));
        }
        finally { Object.DestroyImmediate(secondHost); }
    }
    [Test, Category("Quick")]
    public void AssetEditsApplyOnlyWhenReboundAndNeverMutateRunningState()
    {
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        profile.presets[0].zoomPeak = .04f;
        profile.presets[0].attackCurve = AnimationCurve.Linear(0, 0, 1, 1);
        Assert.That(impact.Zoom, Is.EqualTo(.02f));
        impact.Clear(); impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.02f));
        impact.Bind(profile); impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.04f));
    }
    [TestCase(false, .05f), TestCase(true, .02f), Category("Quick")]
    public void AcceptedLethalRetainsCapturedModeAfterMatchFinished(bool slayer, float peak)
    {
        host.AddComponent<CombatSession>().Complete(true);
        impact.Hit(Vector3.zero, Vector3.right, slayer, true); impact.Advance(.5f);
        Assert.That(impact.Zoom, Is.EqualTo(peak)); Assert.That(impact.LethalCount, Is.EqualTo(1));
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.01f);
        Assert.That(impact.NormalCount, Is.Zero);
    }
    [TestCase(1), TestCase(999), Category("Quick")]
    public void SlayerNormalAndRejectedDamageDoNotEmitButAcceptedDamageDoes(int damage)
    {
        impact.Hit(Vector3.zero, Vector3.right, true, false); impact.Advance(.01f);
        Assert.That(impact.Serial, Is.Zero);
        health.TakeDamage(damage, Vector2.right); impact.Advance(.01f);
        Assert.That(impact.ReceivedCount, Is.EqualTo(1)); Assert.That(impact.Zoom, Is.Zero);
        health.TakeDamage(1, Vector2.right); impact.Advance(.01f); Assert.That(impact.ReceivedCount, Is.EqualTo(1));
    }
    [Test, Category("Quick")]
    public void MissingBindingDoesNotSelectRuntimeDefaults()
    {
        impact.Bind(null); impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Serial, Is.Zero); Assert.That(impact.Zoom, Is.Zero);
    }
    [TestCase(CombatCameraProfile.ShakeUnits.WorldUnits), TestCase(CombatCameraProfile.ShakeUnits.WorldPixels), Category("Quick")]
    public void WarmRepeatedContactsAndUpdatesAllocateNoManagedMemory(CombatCameraProfile.ShakeUnits units)
    {
        cameraHost.SetActive(false);
        var arena = cameraHost.AddComponent<DungeonArenaCamera>(); arena.profile = LightingTestAssets.Definition.lighting;
        cameraHost.SetActive(true); Set(impact, "arena", arena);
        foreach (var preset in profile.presets) preset.units = units;
        impact.Bind(profile);
        var state = CameraState.Default; var lens = state.Lens; lens.OrthographicSize = view.orthographicSize; state.Lens = lens;
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.02f); _ = impact.Offset; _ = impact.OffsetForState(state); impact.Clear();
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 32; i++) { impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.02f); _ = impact.Offset; _ = impact.OffsetForState(state); impact.Clear(); }
        Assert.That(System.GC.GetAllocatedBytesForCurrentThread() - before, Is.Zero);
    }
    static void FullCycle(CameraImpactTimeline t, CombatCameraProfile.Preset p)
    {
        t.Play(p, 0, Vector3.left); t.Sample(.01, .01, false, false);
        t.Sample(.03, .03, false, false); t.Acknowledge(t.Serial, t.RenderMask, false);
        t.Sample(.08, .08, false, false); t.Sample(.3, .3, false, false);
    }
    [Test, Category("Quick")]
    public void FullTimelineAttackPeakAcknowledgmentAndRecoveryAllocateNothingAfterWarmup()
    {
        var t = Timeline(out var p, 1); FullCycle(t, p);
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 32; i++) FullCycle(t, p);
        Assert.That(System.GC.GetAllocatedBytesForCurrentThread() - before, Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator ReadingClearsDelayedRequestAndPauseHoldsOutput()
    {
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.03f);
        GamePauseController.SetTutorialReading(true); yield return null;
        Assert.That(impact.Zoom, Is.Zero); Assert.That(impact.Offset, Is.EqualTo(Vector3.zero));
        GamePauseController.SetTutorialReading(false);
        var pause = cameraHost.AddComponent<GamePauseController>();
        Set(pause, "targetCamera", view); Set(pause, "pauseOnLoseFocus", false); Set(pause, "zoomDuration", .05f);
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        pause.TogglePause(true); float held = impact.Zoom;
        yield return new WaitForSecondsRealtime(.08f); Assert.That(impact.Zoom, Is.EqualTo(held));
        pause.Resume(); yield return new WaitForSecondsRealtime(.1f);
        Assert.That(GamePauseController.InputBlocked, Is.False); impact.Clear();
    }
    [Test, Category("Quick")]
    public void OnlyMatchingWorldCameraAcknowledgesRenderedPeak()
    {
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.5f);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var begin = typeof(CombatCameraImpact).GetMethod("BeginRender", flags);
        var end = typeof(CombatCameraImpact).GetMethod("EndRender", flags);
        var other = host.AddComponent<Camera>();
        object[] args = { default(UnityEngine.Rendering.ScriptableRenderContext), other };
        begin.Invoke(impact, args); end.Invoke(impact, args); Assert.That(impact.PeakRenderCount, Is.Zero);
        args[1] = view; begin.Invoke(impact, args); end.Invoke(impact, args);
        Assert.That(impact.PeakRenderCount, Is.EqualTo(1));
        end.Invoke(impact, args); Assert.That(impact.PeakRenderCount, Is.EqualTo(1));
    }
    [TestCase(140), TestCase(180), TestCase(200), TestCase(500), Category("Quick")]
    public void AdapterDoesNotUseTimeBeforeContactInReceivingFrame(int milliseconds)
    {
        System.Threading.Thread.Sleep(milliseconds);
        impact.Hit(Vector3.zero, Vector3.right, false, false);
        typeof(CombatCameraImpact).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(impact, null);
        Assert.That(impact.NormalCount, Is.EqualTo(1));
        Assert.That(impact.Zoom, Is.LessThan(.02f)); Assert.That(impact.ActiveKind, Is.EqualTo(CombatCameraImpact.Kind.Normal));
    }
    [TestCase(180), TestCase(144), Category("Quick")]
    public void LogicalPixelUsesActualWorldTextureAndPerspectiveProjection(int height)
    {
        cameraHost.SetActive(false);
        var arena = cameraHost.AddComponent<DungeonArenaCamera>();
        var lighting = Object.Instantiate(LightingTestAssets.Definition.lighting);
        lighting.resolution = new Vector2Int(height == 144 ? 256 : 320, height);
        arena.profile = lighting; cameraHost.SetActive(true);
        try
        {
            view.orthographic = false; view.fieldOfView = 35;
            Assert.That(arena.WorldTexture.height, Is.EqualTo(height));
            float pixel = 2 * 17.8403f * Mathf.Tan(35 * Mathf.Deg2Rad / 2) / height;
            Assert.That(arena.PixelSizeAtDepth(17.8403f), Is.EqualTo(pixel).Within(.000001f));
            Set(arena, "impact", impact);
            impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
            Assert.That(arena.PixelSizeAtDepth(17.8403f), Is.EqualTo(pixel / 1.02f).Within(.000001f));
            Assert.That(arena.PixelSizeAtDepth(17.8403f, view.orthographicSize * .8f), Is.EqualTo(pixel * .8f / 1.02f).Within(.000001f),
                "Noise uses the current brain lens before Camera receives it");
            view.orthographicSize *= .8f;
            Assert.That(arena.PixelSizeAtDepth(17.8403f), Is.EqualTo(pixel * .8f / 1.02f).Within(.000001f));
        }
        finally { Object.DestroyImmediate(arena); Object.DestroyImmediate(lighting); }
    }
    [Test, Category("Quick")]
    public void ProfileValidationRejectsMissingDuplicatesNegativeAndInvalidCurves()
    {
        profile.Validate(); var save = profile.presets[1]; profile.presets[1] = profile.presets[0];
        Assert.Throws<System.InvalidOperationException>(() => profile.Validate()); profile.presets[1] = save;
        profile.presets[0].attack = -1; Assert.Throws<System.InvalidOperationException>(() => profile.Validate());
        profile.presets[0].attack = .02f; profile.presets[0].attackCurve = new AnimationCurve();
        Assert.Throws<System.InvalidOperationException>(() => profile.Validate());
    }
    [Test, Category("Quick")]
    public void PulseKeepsAuthoredFootLandmarkAtItsBaselineScreenHeight()
    {
        view.transform.rotation = Quaternion.Euler(5, 0, 0);
        view.orthographicSize = 5.625f;
        var brain = cameraHost.AddComponent<CinemachineBrain>();
        brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.ManualUpdate;
        var vcam = cameraHost.AddComponent<CinemachineVirtualCamera>();
        vcam.m_Lens.OrthographicSize = 5.625f;
        var player = host.AddComponent<BoxCollider2D>(); player.size = new Vector2(1, 2); player.offset = Vector2.up;
        var enemyHost = new GameObject("anchor enemy"); enemyHost.transform.SetParent(host.transform);
        enemyHost.transform.localPosition = Vector3.right * 2;
        var enemy = enemyHost.AddComponent<BoxCollider2D>(); enemy.size = new Vector2(1, 2); enemy.offset = Vector2.up;
        var hud = new GameObject("anchor landmarks", typeof(RectTransform)); hud.transform.SetParent(host.transform);
        var layout = hud.AddComponent<CombatHUDLayout>();
        Set(layout, "playerBody", player); Set(layout, "enemyBody", enemy);
        Physics2D.SyncTransforms();
        cameraHost.SetActive(false);
        var arena = cameraHost.AddComponent<DungeonArenaCamera>();
        arena.profile = LightingTestAssets.Definition.lighting;
        cameraHost.SetActive(true);
        arena.player = player.transform; arena.enemy = enemy.transform;
        Set(arena, "framingLayout", layout); Set(arena, "impact", impact); Set(impact, "arena", arena);
        arena.ResetFraming(); brain.ManualUpdate();
        var update = typeof(DungeonArenaCamera).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        update.Invoke(arena, null);
        float baseline = arena.WorldToScreenPoint(layout.LowestFoot).y;
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        update.Invoke(arena, null);
        Assert.That(arena.WorldToScreenPoint(layout.LowestFoot).y, Is.EqualTo(baseline).Within(.01f));
        arena.ResetImpacts(true);
        Assert.That(arena.WorldToScreenPoint(layout.LowestFoot).y, Is.EqualTo(baseline).Within(.01f));
    }
    [Test, Category("Quick")]
    public void PerspectiveCompositionLeavesBaseLensOwnedBySlayerAndPause()
    {
        view.orthographicSize = 5.625f;
        cameraHost.SetActive(false);
        var arena = cameraHost.AddComponent<DungeonArenaCamera>();
        arena.profile = LightingTestAssets.Definition.lighting;
        cameraHost.SetActive(true);
        Set(arena, "impact", impact); Set(impact, "arena", arena);
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        var update = typeof(DungeonArenaCamera).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (float baseline in new[] { 5.625f, 3.3f, 1.65f })
        {
            view.orthographicSize = baseline;
            update.Invoke(arena, null);
            Assert.That(view.orthographicSize, Is.EqualTo(baseline));
            Assert.That(view.fieldOfView, Is.EqualTo(2 * Mathf.Atan(baseline / 1.02f / arena.profile.FramingDistance) * Mathf.Rad2Deg).Within(.0001f));
        }
        arena.ResetFraming(); update.Invoke(arena, null);
        Assert.That(impact.Zoom, Is.Zero);
        Assert.That(view.fieldOfView, Is.EqualTo(2 * Mathf.Atan(1.65f / arena.profile.FramingDistance) * Mathf.Rad2Deg).Within(.0001f));
    }
}
