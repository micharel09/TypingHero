using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer I { get; private set; }

    [SerializeField, Min(1)] int initialVoices = 8;
    [SerializeField, Min(1)] int maxVoices = 32;

    Transform _root;

    // Pool
    readonly Queue<AudioSource> _idle = new();

    // Active voices + metadata để crossfade theo "channel"
    sealed class Active
    {
        public AudioSource src;
        public SfxEvent ev;
        public Transform owner;     // nhóm theo chủ sở hữu (Player, Enemy…)
        public string channel;      // cùng channel => bị crossfade/override
        public bool fading;         // đang fade-out
        public Coroutine endCo;     // coroutine trả voice khi tự kết thúc
        public Coroutine fadeCo;    // coroutine fade-out thủ công
    }

    readonly Dictionary<AudioSource, Active> _bySrc = new();
    readonly List<Active> _actives = new();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;

        _root = new GameObject("SFX_AudioPool").transform;
        _root.SetParent(transform, false);

        for (int i = 0; i < initialVoices; i++) _idle.Enqueue(CreateVoice());

        DontDestroyOnLoad(gameObject);
    }

    AudioSource CreateVoice()
    {
        var go = new GameObject("sfx_voice");
        go.transform.SetParent(_root, false);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        src.dopplerLevel = 0f;
        return src;
    }

    AudioSource Rent()
    {
        if (_idle.Count > 0) return _idle.Dequeue();
        if (_bySrc.Count < Mathf.Max(initialVoices, maxVoices)) return CreateVoice();
        return null;
    }

    void Return(AudioSource s)
    {
        if (!s) return;

        if (_bySrc.TryGetValue(s, out var a))
        {
            if (a.endCo != null) StopCoroutine(a.endCo);
            if (a.fadeCo != null) StopCoroutine(a.fadeCo);
            a.ev?.NotifyEnd();
            _actives.Remove(a);
            _bySrc.Remove(s);
        }

        s.Stop();
        s.transform.SetParent(_root, false);
        s.clip = null;
        s.outputAudioMixerGroup = null;
        s.volume = 1f;
        s.pitch = 1f;
        _idle.Enqueue(s);
    }

    // ---------------------- PUBLIC API ----------------------

    /// Phát SFX bình thường (không quản lý channel).
    public AudioSource Play(SfxEvent ev, Vector3 pos, Transform follow = null)
        => StartVoice(ev, pos, follow, null, null);

    /// Phát SFX trên "channel" (cùng chủ sở hữu). Bất kỳ voice đang chạy trên channel đó sẽ được fade-out rồi giải phóng.
    public AudioSource PlayOnChannel(
        SfxEvent ev, Vector3 pos, Transform follow, string channelKey, Transform owner, float crossfadeOutSeconds = 0.06f)
    {
        if (!string.IsNullOrEmpty(channelKey) && owner)
        {
            // Fade-out tất cả voice cùng channel & owner
            for (int i = _actives.Count - 1; i >= 0; i--)
            {
                var a = _actives[i];
                if (a.owner == owner && a.channel == channelKey && !a.fading)
                    a.fadeCo = StartCoroutine(FadeOutAndReturn(a, Mathf.Max(0f, crossfadeOutSeconds)));
            }
        }

        return StartVoice(ev, pos, follow, channelKey, owner);
    }

    // ---------------------- INTERNAL ----------------------

    AudioSource StartVoice(SfxEvent ev, Vector3 pos, Transform follow, string channel, Transform owner)
    {
        if (!ev || !ev.CanPlayNow()) return null;
        var clip = ev.PickClip();
        if (!clip) return null;

        var src = Rent();
        if (!src) return null;

        ev.ApplyToSource(src);
        src.clip = clip;

        if (follow)
        {
            src.transform.SetParent(follow, false);
            src.transform.localPosition = Vector3.zero;
        }
        else
        {
            src.transform.SetParent(_root, false);
            src.transform.position = pos;
        }

        var active = new Active { src = src, ev = ev, owner = owner, channel = channel, fading = false };
        _bySrc[src] = active;
        _actives.Add(active);

        ev.NotifyStart();
        src.Play();
        active.endCo = StartCoroutine(ReturnWhenDone(active));
        return src;
    }

    IEnumerator ReturnWhenDone(Active a)
    {
        var s = a.src;
        float est = (s.clip ? s.clip.length / Mathf.Max(0.01f, s.pitch) : 0f) + 0.05f;
        float tEnd = Time.unscaledTime + est;

        // Đợi nguồn tự kết thúc (trừ khi đã bị fade-out thủ công)
        while (s && s.isPlaying && !a.fading && Time.unscaledTime < tEnd) yield return null;

        if (s) Return(s);
    }

    IEnumerator FadeOutAndReturn(Active a, float seconds)
    {
        a.fading = true;
        var s = a.src;
        if (!s) yield break;

        if (seconds <= 0f)
        {
            Return(s);
            yield break;
        }

        float v0 = s.volume;
        float t = 0f;
        while (t < seconds && s)
        {
            t += Time.unscaledDeltaTime;
            s.volume = Mathf.Lerp(v0, 0f, t / seconds);
            yield return null;
        }
        if (s) Return(s);
    }
}
