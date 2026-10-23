using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

#if UNITY_EDITOR
// Explicit rendered acceptance fixture; excluded from Quick and never shipped.
public sealed class EnemyStateVfxRenderedTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    readonly List<Object> owned = new();
    static void Set(object owner, string name, object value) => owner.GetType().GetField(name, Flags).SetValue(owner, value);
    [UnityTearDown] public IEnumerator Cleanup()
    {
        foreach (var item in owned) if (item) Object.Destroy(item);
        owned.Clear(); GamePauseController.ResetSessionState(); Time.timeScale = 1;
        yield return null;
    }
    [System.Serializable] sealed class Frame
    {
        public int index, unityFrame, direction, bursts;
        public float time, realTime, floorY;
        public string phase;
        public string sprite;
    }
    [System.Serializable] sealed class Trace { public List<Frame> frames = new(); }
    static bool InsertCapture(ref PlayerLoopSystem loop, PlayerLoopSystem capture)
    {
        if (loop.subSystemList == null) return false;
        var list = new List<PlayerLoopSystem>(loop.subSystemList);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].type == typeof(PostLateUpdate.UpdateAllRenderers))
            {
                list.Insert(i + 1, capture); loop.subSystemList = list.ToArray(); return true;
            }
            var child = list[i];
            if (InsertCapture(ref child, capture)) { list[i] = child; loop.subSystemList = list.ToArray(); return true; }
        }
        return false;
    }
    [UnityTest, Category("StateVfxVisual")]
    public IEnumerator EveryVariantAndDirectionRendersStunContactTailAndCleanup()
    {
        Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null), "This fixture needs a real graphics device");
        var outRoot = System.Environment.GetEnvironmentVariable("STATE_VFX_CAPTURE") ?? Path.Combine(Application.temporaryCachePath, "EnemyStateVfxReview");
        Directory.CreateDirectory(outRoot);
        var go = new GameObject("state cue capture camera"); owned.Add(go);
        var camera = go.AddComponent<Camera>(); camera.tag = "MainCamera";
        camera.orthographic = true; camera.orthographicSize = 5.625f;
        camera.transform.position = new Vector3(0, 0, -10); camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.065f, .065f, .08f); camera.allowHDR = false;
        var rt = new RenderTexture(320, 180, 24); owned.Add(rt); camera.targetTexture = rt;
        var png = new Texture2D(320, 180, TextureFormat.RGB24, false); owned.Add(png);
        int oldRate = Application.targetFrameRate, oldCaptureRate = Time.captureFramerate;
        Application.targetFrameRate = 60; Time.captureFramerate = 60;
        var originalLoop = PlayerLoop.GetCurrentPlayerLoop(); var captureLoop = originalLoop;
        System.Action captureFrame = null;
        Assert.That(InsertCapture(ref captureLoop, new PlayerLoopSystem {
            type = typeof(EnemyStateVfxRenderedTests), updateDelegate = () => captureFrame?.Invoke() }), Is.True);
        PlayerLoop.SetPlayerLoop(captureLoop);
        try
        {
            foreach (string form in new[] { "sword", "sword-shield", "axe", "legacy" })
            foreach (int direction in new[] { -1, 1 })
            {
                GamePauseController.ResetSessionState();
                var actor = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Enermy/Skeleton.prefab"));
                owned.Add(actor); actor.SetActive(false); actor.transform.position = Vector3.zero;
                actor.transform.localScale = new Vector3(direction, 1, 1);
                var health = actor.GetComponent<SkeletonController>(); health.HoldPracticeAttacks(true);
                var animator = actor.GetComponent<Animator>();
                if (form != "legacy") animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                    "Assets/Art/EndlessSkeletons/" + form + "/Combat.controller");
                var renderer = actor.GetComponent<SpriteRenderer>();
                renderer.sharedMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/VFX/PixelSparks/Pixel.mat");
                var sceneRoot = new GameObject("scene owned dust"); owned.Add(sceneRoot);
                var dust = sceneRoot.AddComponent<ImpactVfxEmitter>();
                var binder = actor.GetComponent<EnemyStateVfxBinder>(); Set(binder, "deathEmitter", dust);
                var body = actor.GetComponent<BoxCollider2D>();
                float floor = body.transform.TransformPoint(body.offset).y - body.size.y * .5f; Set(binder, "floorY", floor);
                actor.SetActive(true); yield return null;
                camera.Render(); yield return null; camera.Render(); yield return null;
                var sessionRoot = new GameObject("fixture combat end"); owned.Add(sessionRoot); sessionRoot.AddComponent<CombatSession>();
                var folder = Path.Combine(outRoot, form + "-" + direction); Directory.CreateDirectory(folder);
                var trace = new Trace(); var sprites = new HashSet<string>();
                float began = Time.time, realBegan = Time.realtimeSinceStartup; bool stunned = false, killed = false, sawGuard = false, sawDust = false;
                // Capture after Unity finishes native particle simulation, rather than
                // inside the test coroutine before particle geometry is ready.
                captureFrame = () =>
                {
                    int i = trace.frames.Count; float age = Time.time - began;
                    bool newPose = false;
                    if (actor)
                    {
                        newPose = sprites.Add(renderer.sprite.name);
                        sawGuard |= actor.GetComponent<ImpactVfxEmitter>().active.Count > 0;
                    }
                    sawDust |= dust.active.Count > 0;
                    // Screenshot I/O/shader compilation must not skip source cels.
                    // Only these fixture instances follow the fixed capture clock;
                    // production prefabs retain unscaled time, tested separately.
                    foreach (var burst in Object.FindObjectsOfType<ImpactVfxBurst>())
                        foreach (var particles in burst.Systems) { var main = particles.main; main.useUnscaledTime = false; }
                    camera.Render(); var previous = RenderTexture.active; RenderTexture.active = rt;
                    png.ReadPixels(new Rect(0, 0, 320, 180), 0, 0); png.Apply(); RenderTexture.active = previous;
                    File.WriteAllBytes(Path.Combine(folder, i.ToString("D4") + ".png"), png.EncodeToPNG());
                    if (newPose)
                    {
                        int mask = camera.cullingMask; camera.cullingMask = 1 << actor.layer;
                        camera.Render(); previous = RenderTexture.active; RenderTexture.active = rt;
                        png.ReadPixels(new Rect(0, 0, 320, 180), 0, 0); png.Apply(); RenderTexture.active = previous;
                        File.WriteAllBytes(Path.Combine(folder, i.ToString("D4") + "-off.png"), png.EncodeToPNG());
                        camera.cullingMask = mask;
                    }
                    trace.frames.Add(new Frame { index = i, unityFrame = Time.frameCount, direction = direction, time = age,
                        sprite = actor ? renderer.sprite.name : "destroyed", bursts = dust.active.Count, floorY = floor,
                        realTime = Time.realtimeSinceStartup - realBegan,
                        phase = !actor ? "cleanup" : health.IsDead ? "death" : actor.GetComponent<EnemyStunController>().IsStunned ? "stun" : "idle" });
                };
                while (Time.time - began < 2.15f)
                {
                    float age = Time.time - began;
                    if (!stunned && age >= .1f)
                    {
                        stunned = true; actor.GetComponent<EnemyStamina>().PreparePractice(0);
                        actor.GetComponent<EnemyStunController>().TriggerStun(10f);
                    }
                    if (!killed && age >= .6f) { killed = true; health.TakeDamage(9999, Vector2.zero); }
                    yield return null;
                }
                captureFrame = null;
                File.WriteAllText(Path.Combine(folder, "trace.json"), JsonUtility.ToJson(trace, true));
                Assert.That(sawGuard, Is.True, form + " guard"); Assert.That(sawDust, Is.True, form + " dust");
                Assert.That(actor == null, Is.True, form + " actor cleanup"); Assert.That(dust.active, Is.Empty);
                Object.Destroy(sceneRoot); Object.Destroy(sessionRoot); yield return null;
            }
        }
        finally
        {
            captureFrame = null; PlayerLoop.SetPlayerLoop(originalLoop);
            Application.targetFrameRate = oldRate; Time.captureFramerate = oldCaptureRate;
        }
    }
}
#endif
