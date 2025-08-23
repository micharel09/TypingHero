using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/SFX Event", fileName = "ev_new_sfx")]
public class SfxEvent : ScriptableObject
{
    [Header("Clips (ít nhất 1)")]
    [SerializeField] AudioClip[] clips;

    [Header("Mix & Randomize")]
    [SerializeField, Range(0f, 1f)] float volume = 1f;
    [SerializeField, Range(0f, 1f)] float volumeJitter = 0.1f;

    public enum PitchRandomMode { JitterAroundBase, RandomBetweenRange }

    [SerializeField] PitchRandomMode pitchMode = PitchRandomMode.JitterAroundBase;

    [Tooltip("Dịch chuyển pitch cơ bản (semitone). Dùng với JitterAroundBase.")]
    [SerializeField] float pitchSemitone = 0f;

    [Tooltip("Biên độ jitter ± (semitone).")]
    [SerializeField, Range(0f, 12f)] float pitchJitterSemitone = 0.5f;

    [Tooltip("Khoảng random tuyệt đối (semitone). Dùng với RandomBetweenRange.")]
    [SerializeField] Vector2 pitchSemitoneRange = new Vector2(-2f, 2f);

    [Header("Routing")]
    [SerializeField] AudioMixerGroup mixerGroup;

    [Header("Limits")]
    [SerializeField, Min(0)] int maxVoices = 6;
    [SerializeField, Min(0f)] float cooldown = 0f; // giây

    float _lastPlayRealtime;
    int _voicesNow;

    void OnValidate()
    {
        if (pitchSemitoneRange.y < pitchSemitoneRange.x)
            (pitchSemitoneRange.x, pitchSemitoneRange.y) = (pitchSemitoneRange.y, pitchSemitoneRange.x);
    }

    public bool CanPlayNow()
    {
        if (clips == null || clips.Length == 0) return false;
        if (cooldown > 0f && Time.unscaledTime < _lastPlayRealtime + cooldown) return false;
        if (_voicesNow >= Mathf.Max(1, maxVoices)) return false;
        return true;
    }

    public AudioClip PickClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    public void ApplyToSource(AudioSource src)
    {
        // ---- luôn 2D tuyệt đối ----
        src.outputAudioMixerGroup = mixerGroup;
        src.spatialBlend = 0f;
        src.panStereo = 0f;
        src.dopplerLevel = 0f;                 // tránh méo cao độ vô tình
        // src.rolloffMode có thể để mặc định, vì 2D không dùng

        // ---- volume ----
        float v = Mathf.Clamp01(volume + Random.Range(-volumeJitter, volumeJitter));
        src.volume = v;

        // ---- pitch theo semitone ----
        float semitone;
        if (pitchMode == PitchRandomMode.RandomBetweenRange)
            semitone = Random.Range(pitchSemitoneRange.x, pitchSemitoneRange.y);
        else
            semitone = pitchSemitone + Random.Range(-pitchJitterSemitone, pitchJitterSemitone);

        src.pitch = Mathf.Pow(2f, semitone / 12f);
    }

    public void NotifyStart() { _voicesNow++; _lastPlayRealtime = Time.unscaledTime; }
    public void NotifyEnd() { _voicesNow = Mathf.Max(0, _voicesNow - 1); }
}
