using UnityEngine;

// Clock and presentation only. Each owner prepares curves once when binding its asset.
internal sealed class CameraImpactTimeline
{
    internal CombatCameraProfile.Preset Active { get; private set; }
    internal int Serial { get; private set; }
    internal int RenderMask { get; private set; }
    internal bool ShakeStarted => Active != null && shakeStarted;
    internal int PeakRenderCount { get; private set; }
    internal int ShakePeakRenderCount { get; private set; }
    internal float Zoom { get; private set; }
    internal Vector3 Offset { get; private set; }
    double start, shakeStart, requestGameplayStart, releaseStart, shakeRecoveryStart;
    double lastSampleTime, lastGameplayTime, shakeRenderTime, shakeSampleTime, shakeSampleGameplay;
    bool waitedForHitstop, shakeRecoveryPending;
    bool zoomRendered, shakeRendered, shakeStarted, zoomHeld, shakeHeld;
    float initialZoom, shakePeakTime, shakePeakValue;
    Vector3 velocity;

    internal bool Play(CombatCameraProfile.Preset preset, double now, Vector3 displacement, double gameplayNow = 0)
    {
        if (!preset.enabled || Active != null && preset.priority <= Active.priority) return false;
        initialZoom = preset.zoomPeak == 0 ? 0 : Zoom;
        Active = preset; start = now; requestGameplayStart = gameplayNow; velocity = displacement; Serial++;
        waitedForHitstop = shakeRecoveryPending = false;
        Zoom = initialZoom; Offset = Vector3.zero; RenderMask = 0;
        zoomRendered = shakeRendered = shakeStarted = zoomHeld = shakeHeld = false;
        shakePeakValue = preset.shakePeakValue; shakePeakTime = preset.shakePeakTime;
        return true;
    }
    internal void Sample(double now, double gameplayNow, bool hitstop, bool paused)
    {
        if (Active == null || paused) return;
        lastSampleTime = now; lastGameplayTime = gameplayNow;
        var p = Active; double age = now - start; RenderMask = 0;
        if (shakeRecoveryPending)
        {
            // Unity publishes gameplay time at frame boundaries. Partition the next
            // scaled delta around the actual render acknowledgment, including clamped frames.
            double span = now - shakeSampleTime;
            double afterRender = span > 0 ? System.Math.Max(0, System.Math.Min(1, (now - shakeRenderTime) / span)) : 0;
            shakeRecoveryStart = gameplayNow - System.Math.Max(0, gameplayNow - shakeSampleGameplay) * afterRender;
            shakeRecoveryPending = false;
        }
        bool zoomDone = p.zoomPeak == 0;
        if (zoomDone) Zoom = 0;
        else if (!zoomRendered)
        {
            if (age < p.attack) Zoom = Mathf.LerpUnclamped(initialZoom, p.zoomPeak, p.attackCurve.Evaluate((float)(age / p.attack)));
            else
            {
                Zoom = p.zoomPeak; zoomHeld = true; RenderMask |= 1;
                // Retain the original hold at normal speed; skip missed attack/hold after a hitch.
                releaseStart = System.Math.Max(now, start + p.attack + p.hold);
            }
        }
        else
        {
            double releaseAge = now - releaseStart;
            Zoom = releaseAge <= 0 ? p.zoomPeak : p.zoomPeak * p.releaseCurve.Evaluate(Mathf.Clamp01((float)(releaseAge / p.release)));
            zoomDone = releaseAge >= p.release;
        }
        bool shakeDone = p.strength == 0;
        if (!shakeStarted && p.afterHitstop && hitstop) waitedForHitstop = true;
        if (!shakeDone && !shakeStarted && age >= p.shakeDelay && (!p.afterHitstop || !hitstop))
        {
            shakeStarted = true;
            shakeStart = waitedForHitstop ? gameplayNow : System.Math.Min(gameplayNow, requestGameplayStart + p.shakeDelay);
        }
        if (!shakeDone && shakeStarted)
        {
            double shakeAge = gameplayNow - shakeStart;
            if (!shakeRendered && shakeAge >= shakePeakTime * p.shakeDuration)
            {
                Offset = velocity * shakePeakValue; shakeHeld = true; RenderMask |= 2;
                shakeRecoveryStart = gameplayNow;
            }
            else
            {
                float t = shakeRendered ? shakePeakTime + (float)((gameplayNow - shakeRecoveryStart) / p.shakeDuration) : (float)(shakeAge / p.shakeDuration);
                Offset = velocity * p.shakeEnvelope.Evaluate(Mathf.Clamp01(t));
                shakeDone = t >= 1;
            }
        }
        if (zoomDone && shakeDone && age >= p.minimumPriorityTime) Finish();
    }
    internal void Acknowledge(int serial, int mask, bool paused) => Acknowledge(serial, mask, paused, lastSampleTime);
    internal void Acknowledge(int serial, int mask, bool paused, double renderedAt)
    {
        if (paused || Active == null || serial != Serial) return;
        renderedAt = System.Math.Max(lastSampleTime, renderedAt);
        if ((mask & 1) != 0 && zoomHeld && !zoomRendered)
        {
            zoomRendered = true; PeakRenderCount++;
            releaseStart = System.Math.Max(releaseStart, renderedAt);
        }
        if ((mask & 2) != 0 && shakeHeld && !shakeRendered)
        {
            shakeRendered = true; ShakePeakRenderCount++;
            shakeRenderTime = renderedAt; shakeSampleTime = lastSampleTime; shakeSampleGameplay = lastGameplayTime;
            shakeRecoveryPending = true;
        }
    }
    void Finish() { Active = null; Zoom = 0; Offset = Vector3.zero; RenderMask = 0; shakeRecoveryPending = false; }
    internal void Clear() { Serial++; Finish(); }
}
