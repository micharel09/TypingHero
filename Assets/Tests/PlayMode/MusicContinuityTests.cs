using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class MusicContinuityTests
{
    GameObject host, starter;
    MusicCue cue;
    AudioClip clip;
    GameObject menuSfx, duplicate;
    static void Set(object owner, string name, object value) => owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(owner, value);
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (starter) Object.Destroy(starter);
        if (duplicate) Object.Destroy(duplicate);
        if (menuSfx) Object.Destroy(menuSfx);
        if (host) Object.Destroy(host);
        if (cue) Object.Destroy(cue);
        if (clip) Object.Destroy(clip);
        AudioListener.pause = false;
        yield return null;
    }
    [UnityTest, Category("Quick")]
    public IEnumerator MenuSfxDuplicatePreservesMusicSiblingAndReloadKeepsOriginalServices()
    {
        var oldMusic = MusicPlayer.I ? MusicPlayer.I.gameObject : null;
        var oldSfx = SfxPlayer.I ? SfxPlayer.I.gameObject : null;
        if (oldMusic) Object.Destroy(oldMusic);
        if (oldSfx && oldSfx != oldMusic) Object.Destroy(oldSfx);
        yield return null;
        menuSfx = new GameObject("persistent menu SFX");
        var originalSfx = menuSfx.AddComponent<SfxPlayer>();
        host = new GameObject("shared scene Audio_Services"); host.SetActive(false);
        host.AddComponent<SfxPlayer>();
        var originalMusic = host.AddComponent<MusicPlayer>();
        var binder = host.AddComponent<MusicOnSlayerMode>();
        host.SetActive(true); yield return null; yield return null;
        Assert.That(host, Is.Not.Null);
        Assert.That(MusicPlayer.I, Is.SameAs(originalMusic));
        Assert.That(SfxPlayer.I, Is.SameAs(originalSfx));
        Assert.That(host.GetComponent<SfxPlayer>(), Is.Null);
        Assert.That(binder, Is.Not.Null, "shared scene components survive duplicate SFX");
        duplicate = new GameObject("next scene Audio_Services"); duplicate.SetActive(false);
        duplicate.AddComponent<SfxPlayer>(); duplicate.AddComponent<MusicPlayer>();
        var nextBinder = duplicate.AddComponent<MusicOnSlayerMode>();
        duplicate.SetActive(true); yield return null; yield return null;
        Assert.That(SfxPlayer.I, Is.SameAs(originalSfx));
        Assert.That(MusicPlayer.I, Is.SameAs(originalMusic));
        Assert.That(duplicate.GetComponent<SfxPlayer>(), Is.Null);
        Assert.That(duplicate.GetComponent<MusicPlayer>(), Is.Null);
        Assert.That(nextBinder, Is.Not.Null, "per-scene Slayer binding remains alive");
    }
    [UnityTest, Category("Quick")]
    public IEnumerator SceneStartKeepsLiveMusicPositionAndRestartsAfterMenuStoppedIt()
    {
        Debug.Log("INTRO_TEST music continuity started");
        host = MusicPlayer.I ? MusicPlayer.I.gameObject : new GameObject("music continuity");
        var player = host.GetComponent<MusicPlayer>();
        if (!player) player = host.AddComponent<MusicPlayer>();
        if (!Object.FindObjectOfType<AudioListener>()) host.AddComponent<AudioListener>();
        foreach (var voice in player.GetComponentsInChildren<AudioSource>()) voice.Stop();
        cue = ScriptableObject.CreateInstance<MusicCue>();
        clip = AudioClip.Create("continuity fixture", 44100 * 5, 1, 44100, false);
        Set(cue, "singleClip", clip);
        AudioListener.pause = false;
        player.Play(cue, 0, 0);
        yield return new WaitForSecondsRealtime(.2f);
        Assert.That(player.IsPlaying(cue), Is.True, "actual scheduled source is playing");
        var source = player.GetComponentsInChildren<AudioSource>().Single(s => s.isPlaying);
        int before = source.timeSamples;
        starter = new GameObject("reloaded scene music"); starter.SetActive(false);
        var start = starter.AddComponent<MusicOnSceneStart>(); Set(start, "cue", cue);
        starter.SetActive(true);
        yield return new WaitForSecondsRealtime(.65f);
        Assert.That(player.GetComponentsInChildren<AudioSource>().Single(s => s.isPlaying), Is.SameAs(source));
        Assert.That(source.timeSamples, Is.GreaterThan(before), "scene entry does not rewind or replace the live voice");
        source.Stop(); Assert.That(player.IsPlaying(cue), Is.False);
        start.SendMessage("Start");
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That(player.IsPlaying(cue), Is.True, "menu-stopped music can start on the next Play");
        Debug.Log("INTRO_TEST music continuity passed");
    }
}
