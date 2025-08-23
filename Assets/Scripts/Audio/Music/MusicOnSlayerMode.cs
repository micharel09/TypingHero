using UnityEngine;

[DisallowMultipleComponent]
public sealed class MusicOnSlayerMode : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerSlayerMode slayer;      // Kéo PlayerSlayerMode vào đây

    [Header("Behavior")]
    [SerializeField] bool watchState = true;       // Theo dõi slayer.IsActive mỗi frame
    [SerializeField, Range(0f, 1f)] float duckVolume01 = 0.05f;
    [SerializeField] float fadeDownSeconds = 0.30f;
    [SerializeField] float fadeUpSeconds = 0.40f;

    bool _lastActive;

    void Awake()
    {
        if (watchState && slayer != null) _lastActive = slayer.IsActive;
    }

    void Update()
    {
        if (!watchState || slayer == null) return;
        bool now = slayer.IsActive;
        if (now == _lastActive) return;
        _lastActive = now;
        if (now) OnSlayerEnter();
        else OnSlayerExit();
    }

    // Có thể nối UnityEvent của PlayerSlayerMode vào 2 hàm này
    public void OnSlayerEnter()
    {
        if (MusicPlayer.I) MusicPlayer.I.DuckTo(duckVolume01, fadeDownSeconds);
    }

    public void OnSlayerExit()
    {
        if (MusicPlayer.I) MusicPlayer.I.RestoreFromDuck(fadeUpSeconds);
    }
}
