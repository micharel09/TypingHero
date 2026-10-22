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
    CinemachineImpulseSource source;
    static void Set(object owner, string field, object value) => owner.GetType()
        .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(owner, value);
    [SetUp] public void Setup()
    {
        GamePauseController.ResetSessionState(); TutorialRun.End(); Time.timeScale = 1;
        host = new GameObject("impact session"); host.SetActive(false);
        health = host.AddComponent<PlayerHealth>();
        source = host.AddComponent<CinemachineImpulseSource>();
        source.m_ImpulseDefinition.m_ImpulseChannel = 2;
        source.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
        source.m_ImpulseDefinition.m_ImpulseDuration = .1f;
        source.m_ImpulseDefinition.m_ImpulseShape = 0;
        source.m_ImpulseDefinition.m_CustomImpulseShape = new AnimationCurve(
            new Keyframe(0, 0), new Keyframe(.2f, 1), new Keyframe(1, 0));
        impact = host.AddComponent<CombatCameraImpact>();
        Set(impact, "health", health); Set(impact, "recoil", source);
        cameraHost = new GameObject("impact camera"); view = cameraHost.AddComponent<Camera>();
        view.transform.position = new Vector3(0, 0, -17.8403f);
        view.fieldOfView = 35; view.orthographic = false;
        host.SetActive(true); Set(impact, "view", view);
    }
    [TearDown] public void Cleanup()
    {
        Object.DestroyImmediate(host); Object.DestroyImmediate(cameraHost);
        GamePauseController.ResetSessionState(); TutorialRun.End(); Time.timeScale = 1;
    }
    [Test, Category("Quick")]
    public void NormalHasBoundedPeakAndReturnsWithoutExtendingOnEqualRequests()
    {
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.02f).Within(.00001f));
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.04f);
        Assert.That(impact.NormalCount, Is.EqualTo(1));
        Assert.That(impact.Zoom, Is.InRange(0, .02f));
        impact.Advance(.08f);
        Assert.That(impact.Zoom, Is.Zero); Assert.That(impact.ActiveKind, Is.EqualTo(CombatCameraImpact.Kind.None));
        Assert.That(impact.ImpulseCount, Is.Zero);
    }
    [Test, Category("Quick")]
    public void SameFramePrioritySelectsOneAndSlayerEntrySuppressesGuardBreakParry()
    {
        impact.Hit(Vector3.zero, Vector3.right, false, false);
        impact.Parry(Vector3.zero, Vector3.left, 1);
        health.TakeDamage(1, Vector2.right);
        impact.EnterSlayer(null, .03f); impact.Advance(.03f);
        Assert.That(impact.LastKind, Is.EqualTo(CombatCameraImpact.Kind.SlayerEnter));
        Assert.That(impact.Serial, Is.EqualTo(1)); Assert.That(impact.Zoom, Is.Zero);
        Assert.That(impact.ParryCount + impact.NormalCount + impact.ReceivedCount, Is.Zero);
    }
    [TestCase(false, .05f), TestCase(true, .02f), Category("Quick")]
    public void AcceptedLethalRetainsCapturedModeEvenAfterSessionFinished(bool wasSlayer, float expected)
    {
        var session = host.AddComponent<CombatSession>(); session.Complete(true);
        impact.Hit(Vector3.zero, Vector3.right, wasSlayer, true); impact.Advance(.04f);
        Assert.That(impact.LethalCount, Is.EqualTo(1));
        Assert.That(impact.Zoom, Is.EqualTo(expected).Within(.00001f));
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.3f);
        Assert.That(impact.NormalCount, Is.Zero); Assert.That(impact.Zoom, Is.Zero);
        impact.Clear(); impact.EnterSlayer(source, .03f); impact.Advance(.04f);
        Assert.That(impact.Serial, Is.EqualTo(1), "finished match rejects new Slayer impact");
    }
    [Test, Category("Quick")]
    public void SlayerHitsAndRejectedDamageDoNotEmitNewImpact()
    {
        impact.Hit(Vector3.zero, Vector3.right, true, false); impact.Advance(.01f);
        Assert.That(impact.Serial, Is.Zero);
        health.TakeDamage(0, Vector2.right); impact.Advance(.01f);
        Assert.That(impact.Serial, Is.Zero);
        health.TakeDamage(1, Vector2.right); impact.Advance(.01f);
        // The zero-damage call established the existing i-frame; camera must honor it.
        Assert.That(impact.Serial, Is.Zero);
    }
    [Test, Category("Quick")]
    public void StrongerContactPreemptsWithoutAddingZoomAndClearDropsPendingKick()
    {
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.03f);
        Assert.That(impact.Zoom, Is.EqualTo(.03f).Within(.00001f));
        Assert.That(impact.Serial, Is.EqualTo(2));
        impact.Clear();
        impact.Advance(1);
        Assert.That(impact.Zoom, Is.Zero); Assert.That(impact.ActiveKind, Is.EqualTo(CombatCameraImpact.Kind.None));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator HitstopDelaysKickAndPauseFreezesEnvelopeAndPendingContact()
    {
        Time.timeScale = .0001f;
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.03f);
        Assert.That(impact.ImpulseCount, Is.Zero);
        Time.timeScale = 1;
        impact.Advance(.001f);
        Assert.That(impact.ImpulseCount, Is.EqualTo(1));
        var pause = cameraHost.AddComponent<GamePauseController>();
        Set(pause, "targetCamera", view); Set(pause, "pauseOnLoseFocus", false);
        Set(pause, "zoomDuration", .05f);
        pause.TogglePause(true);
        float held = impact.Zoom;
        yield return new WaitForSecondsRealtime(.08f);
        Assert.That(impact.Zoom, Is.EqualTo(held));
        pause.Resume();
        yield return new WaitForSecondsRealtime(.1f);
        Assert.That(GamePauseController.InputBlocked, Is.False);
        Assert.That(impact.Zoom, Is.LessThanOrEqualTo(held));
        Time.timeScale = 1; impact.Clear();
        Assert.That(impact.Zoom, Is.Zero);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator ReadingClearsBothRunningAndDelayedImpact()
    {
        Time.timeScale = .0001f;
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.03f);
        GamePauseController.SetTutorialReading(true);
        yield return null;
        Assert.That(impact.Zoom, Is.Zero);
        GamePauseController.SetTutorialReading(false); impact.Advance(.1f);
        Assert.That(impact.ImpulseCount, Is.Zero);
    }
    [Test, Category("Quick")]
    public void SlayerEntryRetainsAuthoredSourcePositionAndDelay()
    {
        source.transform.position = new Vector3(3, 2, 0);
        impact.EnterSlayer(source, .03f); impact.Advance(.01f);
        Assert.That(impact.ImpulseCount, Is.Zero);
        impact.Advance(.025f);
        Assert.That(impact.ImpulseCount, Is.EqualTo(1));
        var owned = (CinemachineImpulseManager.ImpulseEvent)typeof(CombatCameraImpact)
            .GetField("ownedImpulse", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(impact);
        Assert.That(owned.m_Position, Is.EqualTo(source.transform.position));
        Assert.That(impact.Zoom, Is.Zero);
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
        var arena = cameraHost.AddComponent<DungeonArenaCamera>();
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
    public void WarmRepeatedContactsDoNotAllocateManagedMemory()
    {
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.02f); impact.Clear();
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 32; i++)
        {
            impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.02f); impact.Clear();
        }
        long allocated = System.GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.That(allocated, Is.Zero, "camera scheduler and reused channel2 signal are allocation-free after initialization");
    }
    [Test, Category("Quick")]
    public void PerspectiveCompositionLeavesBaseLensOwnedBySlayerAndPause()
    {
        view.orthographicSize = 5.625f;
        var arena = cameraHost.AddComponent<DungeonArenaCamera>();
        Set(arena, "impact", impact); Set(impact, "arena", arena);
        impact.Hit(Vector3.zero, Vector3.right, false, false); impact.Advance(.03f);
        var update = typeof(DungeonArenaCamera).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (float baseline in new[] { 5.625f, 3.3f, 1.65f })
        {
            view.orthographicSize = baseline;
            update.Invoke(arena, null);
            Assert.That(view.orthographicSize, Is.EqualTo(baseline));
            Assert.That(view.fieldOfView, Is.EqualTo(2 * Mathf.Atan(baseline / 1.02f / 17.8403f) * Mathf.Rad2Deg).Within(.0001f));
        }
        arena.ResetFraming(); update.Invoke(arena, null);
        Assert.That(impact.Zoom, Is.Zero);
        Assert.That(view.fieldOfView, Is.EqualTo(2 * Mathf.Atan(1.65f / 17.8403f) * Mathf.Rad2Deg).Within(.0001f));
    }
    [UnityTest, Category("Quick")]
    public IEnumerator ClearingExpiredOwnedEventDoesNotCancelAnotherPooledImpulse()
    {
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.02f);
        yield return new WaitForSeconds(.16f);
        CinemachineImpulseManager.Instance.GetImpulseAt(Vector3.zero, true, 2, out _, out _);
        source.m_ImpulseDefinition.CreateAndReturnEvent(Vector3.zero, Vector3.right * .1f);
        impact.Clear();
        yield return new WaitForSeconds(.02f);
        Assert.That(CinemachineImpulseManager.Instance.GetImpulseAt(Vector3.zero, true, 2, out var value, out _), Is.True);
        Assert.That(value.x, Is.GreaterThan(0));
        yield return new WaitForSeconds(.12f);
    }
    [UnityTest, Category("Quick")]
    public IEnumerator DedicatedListenerUsesOnePixelWhileLegacyGainStaysIsolated()
    {
        var vcam = cameraHost.AddComponent<CinemachineVirtualCamera>();
        var legacy = cameraHost.AddComponent<CinemachineImpulseListener>(); legacy.m_ChannelMask = 1; legacy.m_Gain = 10;
        var listener = cameraHost.AddComponent<CinemachineImpulseListener>(); listener.m_ChannelMask = 2; listener.m_Gain = 1;
        listener.m_UseCameraSpace = false;
        legacy.m_ApplyAfter = listener.m_ApplyAfter = CinemachineCore.Stage.Noise;
        impact.Parry(Vector3.zero, Vector3.left, 1); impact.Advance(.01f);
        yield return new WaitForSeconds(.02f);
        Assert.That(CinemachineImpulseManager.Instance.GetImpulseAt(Vector3.zero, true, 2, out var signal, out _), Is.True);
        float pixel = 2 * 17.8403f * Mathf.Tan(35 * Mathf.Deg2Rad / 2) / 180;
        Assert.That(signal.x, Is.LessThan(0));
        Assert.That(signal.magnitude / pixel, Is.InRange(.1f, 1.01f));
        var callback = typeof(CinemachineImpulseListener).GetMethod("PostPipelineStageCallback", BindingFlags.Instance | BindingFlags.NonPublic);
        var state = CameraState.Default;
        object[] args = { vcam, CinemachineCore.Stage.Noise, state, .016f };
        callback.Invoke(legacy, args); callback.Invoke(listener, args);
        var result = (CameraState)args[2];
        Assert.That((result.PositionCorrection - signal).magnitude, Is.LessThan(.0001f));
    }
}
