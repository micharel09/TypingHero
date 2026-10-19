#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class TypingInputSfxTests
{
    [TestCase("TypingCorrect", "tick_002", .16f, 0f, 4, .02f)]
    [TestCase("TypingError", "error_004", .22f, .12f, 1, .102f)]
    [Category("Quick")]
    public void AuthoredEventsKeepOriginalClipsAndCombatRouting(string name, string clip,
        float volume, float cooldown, int voices, float duration)
    {
        var ev = AssetDatabase.LoadAssetAtPath<SfxEvent>("Assets/Audio/SFX/Events/UI/" + name + ".asset");
        Assert.That(ev, Is.Not.Null);
        var data = new SerializedObject(ev);
        var clips = data.FindProperty("clips");
        Assert.That(clips.arraySize, Is.EqualTo(1));
        var original = AssetDatabase.LoadAssetAtPath<AudioClip>(
            "Assets/Audio/SFX/Clips/KenneyInterface/" + clip + ".ogg");
        Assert.That(original, Is.Not.Null);
        Assert.That(clips.GetArrayElementAtIndex(0).objectReferenceValue, Is.EqualTo(original));
        Assert.That(original.length, Is.EqualTo(duration).Within(.008f));
        Assert.That(data.FindProperty("volume").floatValue, Is.EqualTo(volume));
        Assert.That(data.FindProperty("volumeJitter").floatValue, Is.Zero);
        Assert.That(data.FindProperty("pitchJitterSemitone").floatValue, Is.Zero);
        Assert.That(data.FindProperty("pitchSemitone").floatValue, Is.Zero);
        Assert.That(data.FindProperty("pitchMode").enumValueIndex, Is.Zero);
        Assert.That(data.FindProperty("maxVoices").intValue, Is.EqualTo(voices));
        Assert.That(data.FindProperty("cooldown").floatValue, Is.EqualTo(cooldown));
        Assert.That(data.FindProperty("ignoreListenerPause").boolValue, Is.False);
        var combat = new SerializedObject(AssetDatabase.LoadAssetAtPath<SfxEvent>(
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_normal.asset"));
        Assert.That(data.FindProperty("mixerGroup").objectReferenceValue,
            Is.EqualTo(combat.FindProperty("mixerGroup").objectReferenceValue));
        var go = new GameObject("typing source contract");
        try
        {
            var source = go.AddComponent<AudioSource>();
            ev.ApplyToSource(source);
            Assert.That(source.spatialBlend, Is.Zero);
            Assert.That(source.pitch, Is.EqualTo(1f));
            Assert.That(source.ignoreListenerPause, Is.False);
        }
        finally { Object.DestroyImmediate(go); }
    }
}
#endif
