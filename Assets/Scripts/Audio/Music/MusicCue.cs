using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Music Cue", fileName = "mu_new_cue")]
public class MusicCue : ScriptableObject
{
    public enum Mode { SingleLoop, IntroThenLoop }

    [Header("Playback Mode")]
    [SerializeField] Mode mode = Mode.SingleLoop;

    [Header("Clips")]
    [SerializeField] AudioClip singleClip;     // dùng cho SingleLoop
    [SerializeField] AudioClip introClip;      // dùng cho IntroThenLoop
    [SerializeField] AudioClip loopClip;       // dùng cho IntroThenLoop

    [Header("Mixer & Level")]
    [SerializeField] AudioMixerGroup mixerGroup;
    [SerializeField, Range(0f, 1f)] float volume = 1f;     // level c?a cue
    [SerializeField, Range(-12f, 12f)] float pitchSemitone = 0f;

    [Header("Options (SingleLoop)")]
    [SerializeField] bool loopSingle = true;


    public Mode PlaybackMode => mode;
    public AudioClip Single => singleClip;
    public AudioClip Intro => introClip;
    public AudioClip Loop => loopClip;
    public AudioMixerGroup Group => mixerGroup;
    public float Volume => volume;
    public float PitchScale => Mathf.Pow(2f, pitchSemitone / 12f);
    public bool LoopSingle => loopSingle;

    void OnValidate()
    {
        if (mode == Mode.SingleLoop && !singleClip)
            Debug.LogWarning($"[MusicCue] {name}: SingleLoop c?n Single Clip.");
        if (mode == Mode.IntroThenLoop && (!introClip || !loopClip))
            Debug.LogWarning($"[MusicCue] {name}: IntroThenLoop c?n Intro + Loop Clip.");
    }
}

