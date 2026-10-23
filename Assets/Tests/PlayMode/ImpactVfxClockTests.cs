using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public sealed partial class ImpactVfxTests
{
    static void ResetPulseClock(ClashLightingRig rig, double now)
    {
        var clock = new ClashLightingRig.PulseClock(); clock.Reset(now, false);
        typeof(ClashLightingRig).GetField("clock", Flags).SetValue(rig, clock);
    }
    static ClashLightingRig.PulseClock ReadPulseClock(ClashLightingRig rig) =>
        (ClashLightingRig.PulseClock)typeof(ClashLightingRig).GetField("clock", Flags).GetValue(rig);

    ImpactVfxBurst ClockOwner()
    {
        var profile = PulseProfile(true); profile.lightEnabled = false;
        return Emitter().Play(profile, Context());
    }

    [UnityTest, Category("Quick")]
    public IEnumerator ReceivingFrameHitchDoesNotAgeNewPulseBeforeItsEmission()
    {
        var profile = PulseProfile(true); profile.lightIntensity = 5;
        profile.lightHold = .07f; profile.lightDuration = .28f;
        PulseRig(out var light); var emitter = Emitter();
        emitter.Play(profile, Context()); emitter.Clear();
        var driver = emitter.gameObject.AddComponent<EarlyImpactPulseClockDriver>();
        driver.once = () => emitter.Play(profile, Context());
        // Only this integration regression needs a real long receiving frame.
        System.Threading.Thread.Sleep(320);
        yield return null;
        Assert.That(Time.unscaledDeltaTime, Is.GreaterThan(.28f));
        Assert.That(light.intensity, Is.EqualTo(5).Within(.01f));
        Assert.That(emitter.active.Count, Is.EqualTo(1));
    }

    [TestCase(false), TestCase(true), Category("Quick")]
    public void TimestampedEmissionExcludesReceivingFrameForEitherUpdateOrder(bool sampleBeforeEmission)
    {
        var owner = ClockOwner(); var rig = PulseRig(out var light);
        ResetPulseClock(rig, 10);
        var settings = Metal.Capture();
        if (sampleBeforeEmission) rig.SampleAt(10.3, false);
        rig.PulseAt(Vector3.one, settings, owner, 10.3, false);
        rig.SampleAt(10.3, false); rig.SampleAt(10.3, false);
        Assert.That(light.intensity, Is.EqualTo(5));
        Assert.That(ClashLightingRig.HasPulse(owner), Is.True);
        rig.SampleAt(10.6, false);
        Assert.That(light.intensity, Is.Zero, "Post-emission stalls still count toward duration");
        Assert.That(ClashLightingRig.HasPulse(owner), Is.False);
    }

    [Test, Category("Quick")]
    public void TimestampedMetalPulseRetainsHoldMidpointAndExpiry()
    {
        var owner = ClockOwner(); var rig = PulseRig(out var light); ResetPulseClock(rig, 10);
        rig.PulseAt(Vector3.one, Metal.Capture(), owner, 10, false);
        foreach (var sample in new[] { new Vector2(0, 5), new Vector2(.07f, 5),
            new Vector2(.175f, 2.5f), new Vector2(.28001f, 0) })
        {
            rig.SampleAt(10 + sample.x, false);
            Assert.That(light.intensity, Is.EqualTo(sample.y).Within(.001f));
        }
        Assert.That(ClashLightingRig.HasPulse(owner), Is.False);
    }

    [Test, Category("Quick")]
    public void ClockFreezeEventsExcludePausedIntervalsAndRetainActiveFractions()
    {
        var clock = new ClashLightingRig.PulseClock(); clock.Reset(100, false);
        clock.Sample(100.02, false); clock.FreezeAt(100.07, true);
        clock.Sample(103, true); clock.FreezeAt(104, false);
        Assert.That(clock.Time, Is.EqualTo(.07).Within(.000001));
        clock.Sample(104.105, false);
        Assert.That(clock.Time, Is.EqualTo(.175).Within(.000001));
        clock.FreezeAt(104.13, true); clock.Sample(110, true); clock.FreezeAt(112, false);
        clock.Sample(112.08, false);
        Assert.That(clock.Time, Is.EqualTo(.28).Within(.000001));
    }

    [Test, Category("Quick")]
    public void PollingFreezeTransitionsAndRepeatedSamplesDoNotFastForward()
    {
        var clock = new ClashLightingRig.PulseClock(); clock.Reset(10, false);
        clock.Sample(10.1, false); clock.Sample(10.8, true); clock.Sample(20, true);
        clock.Sample(21, false); clock.Sample(21, false);
        Assert.That(clock.Time, Is.EqualTo(.1).Within(.000001));
        clock.Sample(21.05, false); clock.Sample(20, false); clock.Sample(21.05, false);
        Assert.That(clock.Time, Is.EqualTo(.15).Within(.000001));
    }

    [Test, Category("Quick")]
    public void ExpiredSlotAcceptsWeakerPulseBeforeTheNextRigUpdate()
    {
        var first = ClockOwner(); var second = ClockOwner();
        var rig = PulseRig(out var light, 1); ResetPulseClock(rig, 10);
        rig.PulseAt(Vector3.zero, Metal.Capture(), first, 10, false);
        var weaker = Metal.Capture(); weaker.lightIntensity = 1;
        rig.PulseAt(Vector3.one, weaker, second, 10.3, false);
        Assert.That(light.intensity, Is.EqualTo(1));
        Assert.That(ClashLightingRig.HasPulse(first), Is.False);
        Assert.That(ClashLightingRig.HasPulse(second), Is.True);
    }

    [Test, Category("Quick")]
    public void OverlapComparesCurrentIntensityBeforeTheOldPulseExpires()
    {
        var first = ClockOwner(); var second = ClockOwner();
        var rig = PulseRig(out var light, 1); ResetPulseClock(rig, 10);
        var settings = Metal.Capture(); settings.lightHold = 0; settings.lightDuration = 1;
        settings.lightDecay = AnimationCurve.Linear(0, 1, 1, 0);
        rig.PulseAt(Vector3.zero, settings, first, 10, false);
        settings.lightIntensity = 2.5f;
        rig.PulseAt(Vector3.one, settings, second, 10.6, false);
        Assert.That(light.intensity, Is.EqualTo(2.5f));
        Assert.That(ClashLightingRig.HasPulse(first), Is.False);
        Assert.That(ClashLightingRig.HasPulse(second), Is.True);
    }

    [Test, Category("Quick")]
    public void TwoRigsKeepIndependentClocksAndOwnerUntilTheLastRigDisables()
    {
        var owner = ClockOwner(); var first = PulseRig(out var firstLight);
        var second = PulseRig(out var secondLight);
        ResetPulseClock(first, 10); ResetPulseClock(second, 100);
        var settings = Metal.Capture(); settings.lightHold = 0; settings.lightDuration = 1;
        settings.lightDecay = AnimationCurve.Linear(0, 1, 1, 0);
        first.PulseAt(Vector3.zero, settings, owner, 10, false);
        second.PulseAt(Vector3.zero, settings, owner, 100, false);
        first.SampleAt(10.5, false); second.SampleAt(100.1, false);
        Assert.That(firstLight.intensity, Is.EqualTo(2.5f).Within(.001f));
        Assert.That(secondLight.intensity, Is.EqualTo(4.5f).Within(.001f));
        first.enabled = false;
        Assert.That(firstLight.intensity, Is.Zero); Assert.That(ClashLightingRig.HasPulse(owner), Is.True);
        second.enabled = false;
        Assert.That(secondLight.intensity, Is.Zero); Assert.That(ClashLightingRig.HasPulse(owner), Is.False);
    }

    [Test, Category("Quick")]
    public void TimestampSamplingAllocatesNothingAfterWarmup()
    {
        var owner = ClockOwner(); var rig = PulseRig(out _); ResetPulseClock(rig, 10);
        var settings = Metal.Capture(); settings.lightDuration = 1;
        rig.PulseAt(Vector3.zero, settings, owner, 10, false); rig.SampleAt(10.001, false);
        long before = System.GC.GetAllocatedBytesForCurrentThread();
        for (int i = 1; i <= 100; i++) rig.SampleAt(10.001 + i * .001, false);
        long allocated = System.GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.That(allocated, Is.Zero);
    }

    [Test, Category("Quick")]
    public void DisabledRigUnsubscribesFreezeEventsAndReenableResetsItsClock()
    {
        var rig = PulseRig(out _); ResetPulseClock(rig, 10); rig.enabled = false;
        GamePauseController.SetTutorialReading(true);
        Assert.That(ReadPulseClock(rig).Time, Is.Zero);
        GamePauseController.SetTutorialReading(false); rig.enabled = true;
        Assert.That(ReadPulseClock(rig).Time, Is.Zero);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator LateEmissionAndHitstopStillUseUnscaledTime()
    {
        var profile = PulseProfile(true); profile.lightDuration = .3f;
        PulseRig(out var light); var emitter = Emitter();
        var driver = emitter.gameObject.AddComponent<LateImpactPulseClockDriver>();
        driver.once = () => emitter.Play(profile, Context());
        Time.timeScale = .0001f;
        yield return null;
        Assert.That(light.intensity, Is.EqualTo(2));
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That(light.intensity, Is.Zero); Assert.That(emitter.active, Is.Empty);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator DirectZeroTimeScaleDoesNotFastForwardOnResume()
    {
        var profile = PulseProfile(true); profile.lightDuration = .3f;
        PulseRig(out var light); var emitter = Emitter(); emitter.Play(profile, Context());
        yield return null; Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That(light.intensity, Is.EqualTo(2));
        Time.timeScale = 1; yield return null;
        Assert.That(light.intensity, Is.EqualTo(2));
        yield return new WaitForSecondsRealtime(.4f);
        Assert.That(light.intensity, Is.Zero); Assert.That(emitter.active, Is.Empty);
    }
}

[DefaultExecutionOrder(205)]
public sealed class EarlyImpactPulseClockDriver : MonoBehaviour
{
    public System.Action once;
    void Update() { var action = once; once = null; action?.Invoke(); }
}

[DefaultExecutionOrder(215)]
public sealed class LateImpactPulseClockDriver : MonoBehaviour
{
    public System.Action once;
    void Update() { var action = once; once = null; action?.Invoke(); }
}
#endif
