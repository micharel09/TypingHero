using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class DungeonRenderTests
{
    GameObject host;
    [UnityTest, Category("Quick")]
    public IEnumerator EnvironmentClockPinsClothAndSurvivesPauseAndSlayerHiding()
    {
        host=new GameObject("environment clock lifecycle");host.SetActive(false);
        var banner=new GameObject("cloth");banner.transform.SetParent(host.transform);
        var filter=banner.AddComponent<MeshFilter>();
        var mesh=new Mesh();mesh.vertices=new[]{new Vector3(-1,1,0),new Vector3(1,1,0),new Vector3(-1,-1,0),new Vector3(1,-1,0)};
        mesh.triangles=new[]{0,2,1,1,2,3};mesh.RecalculateBounds();filter.sharedMesh=mesh;
        var renderer=banner.AddComponent<MeshRenderer>();
        var weather=new GameObject("rain");weather.transform.SetParent(host.transform);
        var rain=weather.AddComponent<ParticleSystem>();
        var motion=host.AddComponent<DungeonEnvironmentMotion>();
        var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
        typeof(DungeonEnvironmentMotion).GetField("banner",flags).SetValue(motion,filter);
        typeof(DungeonEnvironmentMotion).GetField("atmosphere",flags).SetValue(motion,new Renderer[]{renderer});
        typeof(DungeonEnvironmentMotion).GetField("weather",flags).SetValue(motion,new[]{rain});
        float previous=Time.timeScale;
        try
        {
            Time.timeScale=1;host.SetActive(true);
            for(int i=0;i<3;i++)yield return null;
            Assert.That(filter.sharedMesh,Is.Not.SameAs(mesh));
            Assert.That(filter.sharedMesh.vertices[0],Is.EqualTo(mesh.vertices[0]));
            Assert.That(filter.sharedMesh.vertices[1],Is.EqualTo(mesh.vertices[1]),"top seam stays attached");
            var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block);
            float held=block.GetFloat("_DungeonAge");Assert.That(held,Is.GreaterThan(0));
            rain.Emit(1);
            Time.timeScale=0;yield return null;yield return null;
            float particleTime=rain.time;
            var particles=new ParticleSystem.Particle[32];int particleCount=rain.GetParticles(particles);
            Assert.That(particleCount,Is.GreaterThan(0));var particle=particles[0];
            renderer.GetPropertyBlock(block);Assert.That(block.GetFloat("_DungeonAge"),Is.EqualTo(held));
            Assert.That(rain.isPaused,Is.True);
            host.SetActive(false);Assert.That(filter.sharedMesh,Is.SameAs(mesh));
            host.SetActive(true);yield return null;
            renderer.GetPropertyBlock(block);Assert.That(block.GetFloat("_DungeonAge"),Is.EqualTo(held));
            Assert.That(rain.isPaused,Is.True);
            Assert.That(rain.time,Is.EqualTo(particleTime));
            Assert.That(rain.GetParticles(particles),Is.EqualTo(particleCount));
            Assert.That(particles[0].position,Is.EqualTo(particle.position));
            Assert.That(particles[0].remainingLifetime,Is.EqualTo(particle.remainingLifetime));
            Time.timeScale=1;yield return null;yield return null;
            renderer.GetPropertyBlock(block);Assert.That(block.GetFloat("_DungeonAge"),Is.GreaterThan(held));
            Assert.That(rain.isPlaying,Is.True);
        }
        finally {Time.timeScale=previous;Object.Destroy(mesh);}
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (host) Object.Destroy(host);
        yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator MainLampStaysConstantAcrossPauseAndCinematicRestore()
    {
        float previousScale=Time.timeScale;
        host=new GameObject("fixed key lifecycle");host.SetActive(false);
        var light=host.AddComponent<Light>();var torch=host.AddComponent<DungeonTorchLight>();
        var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
        typeof(DungeonTorchLight).GetField("baseIntensity",flags).SetValue(torch,40f);
        typeof(DungeonTorchLight).GetField("flickerAmplitude",flags).SetValue(torch,0f);
        try
        {
            Time.timeScale=1;host.SetActive(true);
            for(int i=0;i<20;i++){yield return null;Assert.That(light.intensity,Is.EqualTo(40));}
            Time.timeScale=0;yield return null;Assert.That(light.intensity,Is.EqualTo(40));
            host.SetActive(false);host.SetActive(true);yield return null;
            Assert.That(light.intensity,Is.EqualTo(40));
        }
        finally{Time.timeScale=previousScale;}
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
                Assert.That(light.intensity, Is.InRange(22 * .98f, 22 * 1.02f));
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
            Assert.That(light.intensity, Is.InRange(22 * .98f, 22 * 1.02f));
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
