using System.Reflection;
using Cinemachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public sealed class SlayerTransitionTests
{
    static void Set(object target, string name, object value) => target.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    static void Tick(SlayerScreenFX fx, float dt) => typeof(SlayerScreenFX)
        .GetMethod("AdvancePresentation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(fx, new object[] { dt });

    [TestCase(.5f), TestCase(.01684115f), Category("Quick")]
    public void NormalFlashDuringExitSurvivesEndpointAndRestoresUntintedBaseline(float weight)
    {
        var root = new GameObject("flash exit"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>();
        var original = new Color(.7f, .4f, .2f, .8f); body.color = original;
        var flash = root.AddComponent<EnemyHitFlash>();
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        var update = typeof(EnemyHitFlash).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
        var ivory = new Color(1, .96f, .85f, 1); Set(flash, "flashColor", ivory);
        try
        {
            root.SetActive(true); group.SetPresentationWeight(weight); late.Invoke(group, null);
            flash.Tick(.06f); late.Invoke(group, null);
            var expected = Color.Lerp(ivory, Color.black, weight); expected.a = original.a;
            Assert.That(body.color, Is.EqualTo(expected), "normal-hit flash must compose with exit silhouette");
            group.SetPresentationWeight(0);
            expected = ivory; expected.a = original.a;
            Assert.That(body.color, Is.EqualTo(expected), "ending silhouette must not erase an active flash");
            Set(flash, "_flashUntil", -1f); Set(flash, "_flashFrameUntil", -1);
            update.Invoke(flash, null); late.Invoke(group, null);
            Assert.That(body.color, Is.EqualTo(original), "flash must never restore a tinted baseline");
            update.Invoke(flash, null); late.Invoke(group, null);
            Assert.That(body.color, Is.EqualTo(original));
        }
        finally { Object.DestroyImmediate(root); }
    }

    [UnityEngine.TestTools.UnityTest, Category("Quick")]
    public System.Collections.IEnumerator FlashOverEndpointHoldsThroughPauseAndSlayerCancelsIt()
    {
        var root = new GameObject("paused flash exit"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>(); var original = new Color(.4f, .7f, .6f, .65f); body.color = original;
        var flash = root.AddComponent<EnemyHitFlash>();
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var gate = root.AddComponent<DisableHitFlashOnSlayer>();
        Set(gate, "targets", new System.Collections.Generic.List<MonoBehaviour> { flash });
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        float scale = Time.timeScale;
        try
        {
            root.SetActive(true); group.SetPresentationWeight(.2f); late.Invoke(group, null); flash.Tick(.06f);
            Time.timeScale = 0; yield return new WaitForSecondsRealtime(.1f);
            group.SetPresentationWeight(0); var held = body.color;
            yield return new WaitForSecondsRealtime(.1f);
            Assert.That(body.color, Is.EqualTo(held), "pause/hitstop holds flash across endpoint");
            Time.timeScale = 1; yield return new WaitForSecondsRealtime(.09f);
            Assert.That(body.color, Is.EqualTo(original));
            flash.Tick(); SlayerModeSignals.SetActive(true); yield return null;
            Assert.That(flash.enabled, Is.False, "gameplay Slayer still cancels normal flash");
            SlayerModeSignals.SetActive(false); yield return null;
            Assert.That(body.color, Is.EqualTo(original));
        }
        finally { Time.timeScale = scale; SlayerModeSignals.SetActive(false); Object.DestroyImmediate(root); }
    }

    [Test, Category("Quick")]
    public void FlashRetriggerAndEntryReversalNeverSnapshotFlashOrSilhouette()
    {
        var root = new GameObject("flash reversal"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>();
        var original = new Color(.2f, .6f, .7f, .55f); body.color = original;
        var flash = root.AddComponent<EnemyHitFlash>();
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        var update = typeof(EnemyHitFlash).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
        var ivory = new Color(1, .96f, .85f, 1); Set(flash, "flashColor", ivory);
        try
        {
            root.SetActive(true); flash.Tick();
            group.SetPresentationWeight(.4f); late.Invoke(group, null);
            flash.Tick(); group.SetPresentationWeight(.8f); late.Invoke(group, null);
            var expected = Color.Lerp(ivory, Color.black, .8f); expected.a = original.a;
            Assert.That(body.color, Is.EqualTo(expected));
            group.SetPresentationWeight(.2f); flash.Tick(); late.Invoke(group, null);
            Set(flash, "_flashUntil", -1f); Set(flash, "_flashFrameUntil", -1);
            update.Invoke(flash, null); late.Invoke(group, null);
            expected = Color.Lerp(original, Color.black, .2f); expected.a = original.a;
            Assert.That(body.color, Is.EqualTo(expected));
            group.SetPresentationWeight(0); Assert.That(body.color, Is.EqualTo(original));
        }
        finally { Object.DestroyImmediate(root); }
    }

    [TestCase(true), TestCase(false), Category("Quick")]
    public void FlashAndSilhouetteDisableInEitherOrderRestoreBaseline(bool flashFirst)
    {
        var root = new GameObject("flash teardown"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>();
        var original = new Color(.3f, .7f, .4f, .6f); body.color = original;
        var flash = root.AddComponent<EnemyHitFlash>();
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            root.SetActive(true); group.SetPresentationWeight(.7f); late.Invoke(group, null); flash.Tick();
            if (flashFirst) { flash.enabled = false; group.enabled = false; }
            else { group.enabled = false; flash.enabled = false; }
            Assert.That(body.color, Is.EqualTo(original));
            var block = new MaterialPropertyBlock(); body.GetPropertyBlock(block);
            Assert.That(block.GetFloat("_SlayerTintWeight"), Is.Zero);
            group.enabled = true; flash.enabled = true; late.Invoke(group, null);
            Assert.That(body.color, Is.EqualTo(original), "enable must not resurrect old flash");
        }
        finally { Object.DestroyImmediate(root); }
    }

    [Test, Category("Quick")]
    public void DynamicRendererCapturedDuringFlashAndExcludedRendererKeepTheirOwnBaseline()
    {
        var root = new GameObject("dynamic flash"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>(); body.color = Color.green;
        var flash = root.AddComponent<EnemyHitFlash>();
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            var child = new GameObject("body child"); child.transform.SetParent(root.transform);
            var dynamic = child.AddComponent<SpriteRenderer>(); dynamic.color = Color.cyan;
            var spark = new GameObject("HitSpark"); spark.transform.SetParent(root.transform);
            var excluded = spark.AddComponent<SpriteRenderer>(); excluded.color = Color.yellow;
            Set(flash, "renderers", new System.Collections.Generic.List<SpriteRenderer> { body, dynamic });
            child.transform.SetParent(null); // flash knows it; group will discover it later
            root.SetActive(true); flash.Tick(); group.SetPresentationWeight(.5f);
            child.transform.SetParent(root.transform); late.Invoke(group, null);
            flash.enabled = false; late.Invoke(group, null); group.SetPresentationWeight(0);
            Assert.That(dynamic.color, Is.EqualTo(Color.cyan));
            Assert.That(excluded.color, Is.EqualTo(Color.yellow));
            Assert.That(body.color, Is.EqualTo(Color.green));
        }
        finally { Object.DestroyImmediate(root); }
    }

    [Test, Category("Quick")]
    public void CoordinatedEntryExitDuplicateReversalAndHitchesRestoreBaseline()
    {
        var host = new GameObject("transition fixture"); host.SetActive(false);
        var lens = host.AddComponent<CinemachineVirtualCamera>();
        var fx = host.AddComponent<SlayerScreenFX>();
        var panel = new GameObject("panel", typeof(RectTransform), typeof(Image));
        var original = new Color(.2f, .3f, .4f, .6f);
        panel.GetComponent<Image>().color = original;
        Set(fx, "vcam", lens); Set(fx, "hudPanel", panel.GetComponent<Image>());
        try
        {
            host.SetActive(true);
            fx.EnterSlayerFX(); Tick(fx, .18f);
            Assert.That(fx.BarReveal, Is.EqualTo(.352f).Within(.00001f));
            Assert.That(lens.m_Lens.OrthographicSize, Is.EqualTo(Mathf.Lerp(5, 3.9f, .784f)).Within(.00001f));
            Assert.That(panel.GetComponent<Image>().color.a, Is.EqualTo(.6f * .648f).Within(.00001f));
            fx.EnterSlayerFX(); Tick(fx, .27f); // duplicate must not restart
            Assert.That(fx.BarReveal, Is.EqualTo(1));
            fx.ExitSlayerFX(); Tick(fx, .18f);
            float reveal = fx.BarReveal, zoom = lens.m_Lens.OrthographicSize;
            fx.EnterSlayerFX();
            Assert.That(fx.BarReveal, Is.EqualTo(reveal));
            Assert.That(lens.m_Lens.OrthographicSize, Is.EqualTo(zoom));
            Tick(fx, .5f); Assert.That(fx.BarReveal, Is.EqualTo(1));
            fx.ExitSlayerFX(); Tick(fx, .5f);
            Assert.That(fx.BarReveal, Is.InRange(.001f, .999f));
            fx.ExitSlayerFX(); Tick(fx, .15f);
            Assert.That(fx.BarReveal, Is.Zero);
            Assert.That(lens.m_Lens.OrthographicSize, Is.EqualTo(5));
            Assert.That(panel.GetComponent<Image>().color, Is.EqualTo(original));
            fx.EnterSlayerFX(); Tick(fx, .18f); fx.enabled = false;
            Assert.That(SlayerScreenFX.PresentationWeight, Is.Zero);
            Assert.That(fx.BarReveal, Is.Zero);
            Assert.That(lens.m_Lens.OrthographicSize, Is.EqualTo(5));
            Assert.That(panel.GetComponent<Image>().color, Is.EqualTo(original));
            fx.enabled = true; Tick(fx, .5f);
            Assert.That(fx.BarReveal, Is.Zero, "disable cancels old progress");
        }
        finally { Object.DestroyImmediate(host); Object.DestroyImmediate(panel); }
    }

    [Test, Category("Quick")]
    public void SilhouetteBlendsOriginalAndDynamicColorsAndExcludesSparks()
    {
        var root = new GameObject("actor"); root.SetActive(false);
        var body = root.AddComponent<SpriteRenderer>(); body.color = new Color(.8f, .4f, .2f, .7f);
        var group = root.AddComponent<SlayerSilhouetteGroup>();
        var late = typeof(SlayerSilhouetteGroup).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            root.SetActive(true); group.SetPresentationWeight(.5f); late.Invoke(group, null);
            Assert.That(body.color.r, Is.EqualTo(.4f).Within(.00001f));
            Assert.That(body.color.a, Is.EqualTo(.7f));
            var block = new MaterialPropertyBlock(); body.GetPropertyBlock(block);
            Assert.That(block.GetFloat("_SlayerTintWeight"), Is.EqualTo(.5f));
            var child = new GameObject("dynamic"); child.transform.SetParent(root.transform);
            var dynamic = child.AddComponent<SpriteRenderer>(); dynamic.color = Color.green;
            var spark = new GameObject("HitSpark"); spark.transform.SetParent(root.transform);
            var excluded = spark.AddComponent<SpriteRenderer>(); excluded.color = Color.yellow;
            late.Invoke(group, null);
            Assert.That(dynamic.color.g, Is.EqualTo(.5f).Within(.00001f));
            Assert.That(excluded.color, Is.EqualTo(Color.yellow));
            excluded.GetPropertyBlock(block);
            Assert.That(block.GetFloat("_SlayerTintWeight"), Is.Zero, "excluded VFX never receive the material tint");
            group.SetPresentationWeight(1); late.Invoke(group, null);
            group.SetPresentationWeight(.25f); late.Invoke(group, null);
            Assert.That(body.color.r, Is.EqualTo(.6f).Within(.00001f), "reversal never snapshots a tinted original");
            group.SetPresentationWeight(0);
            Assert.That(body.color, Is.EqualTo(new Color(.8f, .4f, .2f, .7f)));
            Assert.That(dynamic.color, Is.EqualTo(Color.green));
            body.GetPropertyBlock(block);
            Assert.That(block.GetFloat("_SlayerTintWeight"), Is.Zero);
        }
        finally { Object.DestroyImmediate(root); }
    }

#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void WeatherAndVolumeFadeAndRestoreTheirAuthoredBaseline()
    {
        var root = new GameObject("weather transition"); root.SetActive(false);
        var room = new GameObject("room");
        try
        {
            var volume = room.AddComponent(System.Type.GetType("UnityEngine.Rendering.Volume, Unity.RenderPipelines.Core.Runtime"));
            var weight = volume.GetType().GetField("weight"); weight.SetValue(volume, .7f);
            var rain = new GameObject("rain"); rain.transform.SetParent(room.transform);
            rain.AddComponent<ParticleSystem>();
            var renderer = rain.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/Art/Environment/KayKitDungeon/DungeonAtmosphere.mat");
            Assert.That(renderer.sharedMaterial, Is.Not.Null);
            var original = new Color(.3f, .5f, .7f, .8f);
            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", original); block.SetFloat("_Untouched", 42);
            renderer.SetPropertyBlock(block);
            renderer.GetPropertyBlock(block); original = block.GetColor("_BaseColor");
            var fx = root.AddComponent<SlayerScreenFX>(); Set(fx, "environment", room.transform);
            root.SetActive(true); fx.EnterSlayerFX(); Tick(fx, .225f);
            renderer.GetPropertyBlock(block);
            Assert.That(block.GetColor("_BaseColor").a, Is.EqualTo(.4f).Within(.00001f));
            Assert.That((float)weight.GetValue(volume), Is.EqualTo(.35f).Within(.00001f));
            fx.ExitSlayerFX(); Tick(fx, .65f);
            renderer.GetPropertyBlock(block);
            Assert.That(block.GetColor("_BaseColor"), Is.EqualTo(original));
            Assert.That(block.GetFloat("_Untouched"), Is.EqualTo(42));
            Assert.That((float)weight.GetValue(volume), Is.EqualTo(.7f));
            fx.EnterSlayerFX(); Tick(fx, .5f); fx.enabled = false;
            renderer.GetPropertyBlock(block);
            Assert.That(block.GetColor("_BaseColor"), Is.EqualTo(original));
            Assert.That((float)weight.GetValue(volume), Is.EqualTo(.7f));
        }
        finally { Object.DestroyImmediate(root); Object.DestroyImmediate(room); }
    }
#endif

    [Test, Category("Quick")]
    public void PauseHoldsPresentationButResultsAllowExitToFinish()
    {
        var root = new GameObject("pause presentation"); root.SetActive(false);
        var clock = typeof(GamePauseController).GetField("pauseBlocked", BindingFlags.Static | BindingFlags.NonPublic);
        var result = typeof(GamePauseController).GetProperty("ResultOverlayVisible", BindingFlags.Static | BindingFlags.NonPublic);
        try
        {
            var fx = root.AddComponent<SlayerScreenFX>(); root.SetActive(true);
            fx.EnterSlayerFX(); Tick(fx, .18f);
            float held = fx.BarReveal;
            clock.SetValue(null, true);
            var update = typeof(SlayerScreenFX).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            update.Invoke(fx, null);
            Assert.That(fx.BarReveal, Is.EqualTo(held));
            fx.ExitSlayerFX(); update.Invoke(fx, null);
            Assert.That(fx.BarReveal, Is.EqualTo(held));
            result.SetValue(null, true);
            Set(fx, "_elapsed", .65f); update.Invoke(fx, null);
            Assert.That(fx.BarReveal, Is.Zero, "results do not strand the exit behind Pause");
        }
        finally
        {
            clock.SetValue(null, false); result.SetValue(null, false);
            Object.DestroyImmediate(root);
        }
    }

    [Test, Category("Quick")]
    public void MigrationPreservesAuthoredTimingOnRepeatedBuilders()
    {
        var root = new GameObject("migration"); root.SetActive(false);
        try
        {
            var fx = root.AddComponent<SlayerScreenFX>();
            Set(fx, "zoomDuration", .8f); fx.MigratePresentationDefaults();
            var duration = typeof(SlayerScreenFX).GetField("enterDuration", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(duration.GetValue(fx), Is.EqualTo(.8f));
            Set(fx, "enterDuration", .52f); fx.MigratePresentationDefaults();
            Assert.That(duration.GetValue(fx), Is.EqualTo(.52f));
        }
        finally { Object.DestroyImmediate(root); }
    }
}
