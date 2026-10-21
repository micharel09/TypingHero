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
        bool show = mul > 1.0001f;
        if (decaySlider && decaySlider.gameObject.activeSelf != show)
            decaySlider.gameObject.SetActive(show);
        if (multiplierText)
        {
            // 1) Ẩn khi x1.0
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

    void LateUpdate()
    {
        if (followScore && multiplierText && multiplierText.gameObject.activeSelf)
            FollowRightOfScore(multiplierText.rectTransform, followScore, gapPx, extraOffset);
    }

    // ========= Helpers =========
    static void FollowRightOfScore(RectTransform me, RectTransform score, float gap, Vector2 extra)
    {
        var parent = me.parent as RectTransform;
        if (!parent) return;
        float right = score.rect.xMax;
        if (score.TryGetComponent(out TMP_Text label))
            right = Mathf.Min(right, score.rect.xMin + label.preferredWidth);
        var point = score.TransformPoint(new Vector3(right, score.rect.center.y, 0));
        var local = parent.InverseTransformPoint(point);
        me.anchorMin = me.anchorMax = parent.pivot;
        // Position the left edge, including scale/pulse of the score and this label's pivot.
        me.localPosition = new Vector3(local.x + gap + me.pivot.x * me.rect.width + extra.x,
            local.y + (me.pivot.y - .5f) * me.rect.height + extra.y, 0);
    }
}
