using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class ScoreUI_ComboHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text multiplierText;     // label “x2.5”
    [SerializeField] Slider decaySlider;          // thanh decay 0..1

    [Header("Follow ScoreText")]
    [SerializeField] RectTransform followScore;   // KÉO RectTransform của ScoreText vào đây
    [SerializeField] float gapPx = 12f;           // khoảng cách giữa Score và Combo
    [SerializeField] Vector2 extraOffset = Vector2.zero; // offset tinh chỉnh (tuỳ UI)

    // ========= Public API từ ScoreSystem =========
    public void SetMultiplier(float mul, float maxMul, Color tint)
    {
        if (multiplierText)
        {
            // 1) Ẩn khi x1.0
            bool show = mul > 1.0001f;
            if (multiplierText.gameObject.activeSelf != show)
                multiplierText.gameObject.SetActive(show);

            if (show)
            {
                multiplierText.text = $"x{mul:0.0}";
                multiplierText.color = tint;

                // 2) Dính theo ScoreText (bên phải, có gap)
                if (followScore)
                    FollowRightOfScore(multiplierText.rectTransform, followScore, gapPx, extraOffset);
            }
        }
    }

    public void SetDecay(float t01)
    {
        if (decaySlider)
            decaySlider.value = Mathf.Clamp01(t01);
    }

    // ========= Helpers =========
    static void FollowRightOfScore(RectTransform me, RectTransform score, float gap, Vector2 extra)
    {
        // Chúng ta làm việc trong toạ độ local của PARENT chung
        var parent = me.parent as RectTransform;
        if (parent == null) return;

        // Lấy điểm RIGHT-CENTER của ScoreText trong local của parent
        Vector3 rightCenterWorld = score.TransformPoint(new Vector3(score.rect.xMax, score.rect.center.y, 0f));
        Vector2 rightCenterLocal = parent.InverseTransformPoint(rightCenterWorld);

        // Lấy nửa chiều rộng của chính mình (dùng preferredWidth nếu rect.width chưa có)
        float myHalfW = me.rect.width > 0.1f ? me.rect.width * 0.5f :
                        (me.TryGetComponent(out TMP_Text tmp) ? tmp.preferredWidth * 0.5f : 40f);

        // Đặt anchoredPosition: ngay phải của Score + gap + nửa width của mình
        Vector2 target = rightCenterLocal + new Vector2(gap + myHalfW, 0f) + extra;
        me.anchoredPosition = target;

        // Căn dọc cùng hàng với Score
        me.anchorMin = me.anchorMax = parent.pivot; // giữ anchor ổn định
    }
}
