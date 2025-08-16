using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Parry Config", fileName = "PC_Parry")]
public class ParryConfig : ScriptableObject
{
    [Header("Input")]
    public KeyCode parryKey = KeyCode.Space;

    [Header("Press behaviour (parry stance)")]
    public bool showPoseOnPress = true;
    public float pressWindowSeconds = 0.18f;
    public float whiffRecoverySeconds = 0.10f;

    [Header("Window (seconds)")]
    public float preLeniency = 0.06f;
    public float postLeniency = 0.06f;
    public bool useUnscaledTime = true;

    [Header("On Success (boss)")]
    public float stunEnemySeconds = 0.35f;

    [Header("Player Pose (optional)")]
    [Tooltip("FullPath của clip POSE dùng để mở/đóng cửa sổ qua Animation Events (vd: Base Layer.player_parry).")]
    public string playerParryPoseStatePath = "Base Layer.player_parry";
    [Range(0f, .2f)] public float playerParryPoseCrossfade = 0.02f;

    [Header("Player Success (hiệu ứng khi parry thành công)")]
    [Tooltip("FullPath của clip parry-thành-công (vd: Base Layer.player_parry_success). Bỏ trống để không phát.")]
    public string playerParrySuccessStatePath = "";
    [Range(0f, .2f)] public float playerParrySuccessCrossfade = 0.02f;

    [Header("Advanced")]
    public bool requirePlayerOpenWindow = false;
}
