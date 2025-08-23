using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I { get; private set; }

    [SerializeField, Range(0f, 1f)] float masterVolume = 1f;   // Volume tổng của BGM (0..1)

    AudioSource _a, _b;                    // 2 voice: crossfade & intro→loop
    MusicCue _currentCue;                  // cue hiện tại (kể cả khi đang duck)
    Coroutine _xfadeCo;                    // crossfade/Play coroutine
    Coroutine _volCo;                      // fade volume coroutine
    float _resumeVolume01 = 1f;            // mức volume sẽ khôi phục sau khi duck

    public MusicCue CurrentCue => _currentCue;
    public float CurrentMasterVolume => masterVolume;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _a = CreateVoice("bgm_A");
        _b = CreateVoice("bgm_B");

        _resumeVolume01 = masterVolume;
    }

    AudioSource CreateVoice(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f; // BGM luôn 2D
        return src;
    }

    void SetupSourceForCue(AudioSource s, MusicCue cue)
    {
        s.outputAudioMixerGroup = cue.Group;
        s.volume = 0f;                       // sẽ fade lên
        s.pitch = cue.PitchScale;
        s.spatialBlend = 0f;
    }

    /// <summary>
    /// Phát một cue nhạc. Hỗ trợ crossfade với nhạc đang chạy và Intro→Loop.
    /// </summary>
    public void Play(MusicCue cue, float fadeOut = 0.5f, float fadeIn = 0.5f, double startDelay = 0.05)
    {
        if (!cue) return;
        if (_xfadeCo != null) StopCoroutine(_xfadeCo);
        _xfadeCo = StartCoroutine(CoPlay(cue, Mathf.Max(0f, fadeOut), Mathf.Max(0f, fadeIn), startDelay));
    }

    IEnumerator CoPlay(MusicCue cue, float fadeOut, float fadeIn, double startDelay)
    {
        var oldA = _a; var oldB = _b;

        // chọn voice "new start" & "old playing" để crossfade
        AudioSource newS = !oldA.isPlaying ? oldA : (!oldB.isPlaying ? oldB : oldA);
        AudioSource oldS = (newS == oldA) ? oldB : oldA;

        // fade out nhạc cũ nếu có
        if (oldS.isPlaying && fadeOut > 0f)
        {
            float v0 = oldS.volume;
            float t = 0f;
            while (t < fadeOut)
            {
                t += Time.unscaledDeltaTime;
                oldS.volume = Mathf.Lerp(v0, 0f, t / fadeOut);
                yield return null;
            }
            oldS.volume = 0f;
        }
        oldS.Stop();

        // chuẩn bị nhạc mới
        SetupSourceForCue(newS, cue);

        double dspNow = AudioSettings.dspTime;
        double startDsp = dspNow + System.Math.Max(0.01d, startDelay); // DOUBLE chuẩn DSP

        if (cue.PlaybackMode == MusicCue.Mode.SingleLoop)
        {
            newS.clip = cue.Single;
            newS.loop = cue.LoopSingle;
            newS.PlayScheduled(startDsp);
        }
        else // IntroThenLoop
        {
            var intro = newS;
            intro.clip = cue.Intro;
            intro.loop = false;
            intro.PlayScheduled(startDsp);

            // voice còn lại sẽ giữ Loop
            var loopS = (newS == _a) ? _b : _a;
            SetupSourceForCue(loopS, cue);
            loopS.clip = cue.Loop;
            loopS.loop = true;

            // tính thời lượng intro theo DSP
            double introDur = (double)cue.Intro.samples / cue.Intro.frequency / intro.pitch;
            double loopStart = startDsp + introDur;
            loopS.PlayScheduled(loopStart);

            // cắt Intro đúng lúc Loop bắt đầu để seamless
            intro.SetScheduledEndTime(loopStart);
        }

        _currentCue = cue;

        // fade in nhạc mới
        float target = cue.Volume * masterVolume;
        if (fadeIn > 0f)
        {
            float t = 0f;
            while (t < fadeIn)
            {
                t += Time.unscaledDeltaTime;
                float v = Mathf.Lerp(0f, target, t / fadeIn);
                if (newS.isPlaying) newS.volume = v;
                if (_a != newS && _a.isPlaying) _a.volume = v;
                if (_b != newS && _b.isPlaying) _b.volume = v;
                yield return null;
            }
        }

        if (_a.isPlaying) _a.volume = target;
        if (_b.isPlaying) _b.volume = target;
    }

    // ----------------- DUCK / RESTORE (không pause) -----------------

    IEnumerator CoFadeMaster(float from01, float to01, float seconds, System.Action onDone = null)
    {
        seconds = Mathf.Max(0f, seconds);
        if (seconds <= 0f)
        {
            SetMasterVolume01(to01);
            onDone?.Invoke();
            yield break;
        }

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Lerp(from01, to01, t / seconds);
            SetMasterVolume01(k);
            yield return null;
        }
        SetMasterVolume01(to01);
        onDone?.Invoke();
    }

    /// <summary>Giảm volume về target (0..1), KHÔNG pause. Lưu lại mức hiện tại để khôi phục.</summary>
    public void DuckTo(float target01, float seconds)
    {
        target01 = Mathf.Clamp01(target01);
        if (masterVolume > target01 + 0.0001f) _resumeVolume01 = masterVolume; // nhớ mức trước khi duck
        if (_volCo != null) StopCoroutine(_volCo);
        _volCo = StartCoroutine(CoFadeMaster(masterVolume, target01, seconds));
    }

    /// <summary>Khôi phục volume về mức trước khi duck.</summary>
    public void RestoreFromDuck(float seconds)
    {
        if (_volCo != null) StopCoroutine(_volCo);
        _volCo = StartCoroutine(CoFadeMaster(masterVolume, _resumeVolume01, seconds));
    }

    // ----------------- BASIC VOLUME -----------------

    /// <summary>Đặt master volume (0..1). Tự áp vào nguồn đang phát.</summary>
    public void SetMasterVolume01(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        float target = (_currentCue ? _currentCue.Volume : 1f) * masterVolume;
        if (_a.isPlaying) _a.volume = target;
        if (_b.isPlaying) _b.volume = target;
    }
}
