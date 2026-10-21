using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class DungeonRenderTests
{
    GameObject host;
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (host) Object.Destroy(host);
        yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator TorchFlickerIsBoundedAndPreservesPhaseAcrossPauseAndHiding()
    {
        float previousScale = Time.timeScale;
        host = new GameObject("torch lifecycle test");
        var light = host.AddComponent<Light>();
        var torch = host.AddComponent<DungeonTorchLight>();
        var renderer=host.AddComponent<MeshRenderer>();
        var material=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.SetColor("_EmissionColor",new Color(3,1,.2f));renderer.sharedMaterial=material;
        typeof(DungeonTorchLight).GetField("flameRenderers",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)
            .SetValue(torch,new Renderer[]{renderer});
        var block=new MaterialPropertyBlock();
        try
        {
            Time.timeScale = 1;
            float minimum = light.intensity, maximum = light.intensity;
            for (int i = 0; i < 20; i++)
            {
                yield return null;
                minimum = Mathf.Min(minimum, light.intensity);
                maximum = Mathf.Max(maximum, light.intensity);
                renderer.GetPropertyBlock(block);
                Assert.That(block.GetColor("_EmissionColor").r,Is.EqualTo(3*light.intensity/22).Within(.0001f),"visible flame and local light share the same phase");
                Assert.That(light.intensity, Is.InRange(22 * .95f, 22 * 1.05f));
            }
            Assert.That(maximum - minimum, Is.GreaterThan(.001f));
            Time.timeScale = 0;
            float held = light.intensity;
            for (int i = 0; i < 3; i++) yield return null;
            Assert.That(light.intensity, Is.EqualTo(held), "pause freezes flame phase");
            host.SetActive(false);
            Assert.That(light.intensity, Is.EqualTo(22), "disabled flame restores authored intensity");
            host.SetActive(true);
            Assert.That(light.intensity, Is.EqualTo(held), "cinematic restore resumes the same phase");
            Time.timeScale = 1;
            yield return null;
            Assert.That(torch.enabled, Is.True);
            Assert.That(light.intensity, Is.InRange(22 * .95f, 22 * 1.05f));
        }
        finally { Time.timeScale = previousScale; Object.Destroy(material); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator OwnedWorldTextureProjectsToRealViewportAndReleasesOnDisable()
    {
        host = new GameObject("arena projection test"); host.SetActive(false);
        var camera = host.AddComponent<Camera>(); camera.orthographic = true;
        camera.orthographicSize = 5.625f; camera.transform.position = new Vector3(0, 4, -10);
        camera.transform.rotation = Quaternion.Euler(10, 0, 0);
        var originalRect = new Rect(.1f, .15f, .8f, .7f); camera.rect = originalRect;
        var arena = host.AddComponent<DungeonArenaCamera>(); host.SetActive(true);
        var texture = arena.WorldTexture;
        Assert.That(texture.width, Is.EqualTo(320)); Assert.That(texture.height, Is.EqualTo(180));
        Assert.That(texture.format, Is.EqualTo(RenderTextureFormat.ARGBHalf));
        Assert.That(texture.filterMode, Is.EqualTo(FilterMode.Point));
        Assert.That(texture.antiAliasing, Is.EqualTo(1)); Assert.That(texture.useMipMap, Is.False);
        var point = camera.transform.position + camera.transform.forward * 10;
        foreach (var rect in new[] { originalRect, new Rect(0, .125f, 1, .75f), new Rect(0, 0, 1, 1) })
        {
            CombatViewportCamera.SetViewport(camera, rect);
            var screen = CombatViewportCamera.WorldToScreenPoint(camera, point);
            Assert.That(screen.x, Is.EqualTo(rect.center.x * Screen.width).Within(.01f));
            Assert.That(screen.y, Is.EqualTo(rect.center.y * Screen.height).Within(.01f));
            Assert.That(camera.rect, Is.EqualTo(new Rect(0, 0, 1, 1)));
            Assert.That(arena.PresentationCamera.rect, Is.EqualTo(rect));
            Assert.That(arena.WorldTexture, Is.SameAs(texture), "resize must not reallocate the world texture");
        }
        arena.SetCinematic(true); Assert.That(arena.Cinematic, Is.True);
        arena.SetCinematic(true); arena.SetCinematic(false); Assert.That(arena.Cinematic, Is.False);
        arena.enabled = false;
        Assert.That(texture.IsCreated(), Is.False); Assert.That(camera.targetTexture, Is.Null);
        Assert.That(camera.rect, Is.EqualTo(originalRect));
        yield return null;
        Assert.That(host.GetComponentsInChildren<DungeonPresentationCamera>(), Is.Empty);
        arena.enabled = true;
        Assert.That(arena.WorldTexture, Is.Not.SameAs(texture));
        Assert.That(host.GetComponentsInChildren<DungeonPresentationCamera>().Length, Is.EqualTo(1));
    }
}
