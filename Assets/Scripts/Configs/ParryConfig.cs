using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Parry Config", fileName = "PC_Parry")]
public class ParryConfig : ScriptableObject
{
    [Header("Input")]
    public KeyCode parryKey = KeyCode.Space;

    [Header("Player Pose (optional)")]
    [Tooltip("FullPath của clip POSE dùng để mở/đóng cửa sổ qua Animation Events (vd: Base Layer.player_parry).")]
    public string playerParryPoseStatePath = "Base Layer.player_parry";
    [Range(0f, .2f)] public float playerParryPoseCrossfade = 0.02f;

    [Header("Player Success (hiệu ứng khi parry thành công)")]
    [Tooltip("FullPath của clip parry-thành-công (vd: Base Layer.player_parry_success). Bỏ trống để không phát.")]
    public string playerParrySuccessStatePath = "";
    [Range(0f, .2f)] public float playerParrySuccessCrossfade = 0.02f;
}
