using UnityEngine;
using UnityEngine.UI;

public sealed class AudioPreferences : MonoBehaviour
{
    // Keep headroom when combat, typing and music overlap; preserve their relative mix.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ApplyOutputHeadroom() => AudioListener.volume = .5f;

    [SerializeField] Slider music, sfx;
    public static float Music => PlayerPrefs.GetFloat("MusicVolume", 1f);
    public static float Sfx => PlayerPrefs.GetFloat("SfxVolume", 1f);
    void OnEnable()
    {
        music.SetValueWithoutNotify(Music);
        sfx.SetValueWithoutNotify(Sfx);
        music.onValueChanged.AddListener(SetMusic);
        sfx.onValueChanged.AddListener(SetSfx);
    }
    void OnDisable()
    {
        music.onValueChanged.RemoveListener(SetMusic);
        sfx.onValueChanged.RemoveListener(SetSfx);
    }
    void SetMusic(float value) { PlayerPrefs.SetFloat("MusicVolume", value); PlayerPrefs.Save(); }
    void SetSfx(float value) { PlayerPrefs.SetFloat("SfxVolume", value); PlayerPrefs.Save(); }
}
