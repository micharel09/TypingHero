using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

[DisallowMultipleComponent]
[DefaultExecutionOrder(1060)]
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
                multiplierText.text = "\u00d7" + mul.ToString("0.##", CultureInfo.InvariantCulture);
                multiplierText.color = tint;
            }
        }
    }

    public void SetDecay(float t01)
    {
        if (decaySlider)
            decaySlider.value = Mathf.Clamp01(t01);
    }

    float scoreFontSize;
    float multiplierFontSize;

    void LateUpdate()
    {
        if (!followScore || !multiplierText || !followScore.TryGetComponent(out TMP_Text score)) return;
        var frame = followScore.parent as RectTransform;
        if (!frame) return;
        if (scoreFontSize == 0) scoreFontSize = score.fontSize;
        if (multiplierFontSize == 0) multiplierFontSize = multiplierText.fontSize;

        bool show = multiplierText.gameObject.activeSelf;
        // Reserve the largest existing award pulse (x3), so text never jumps out
        // of the frame and long totals retain every digit.
        const float pulse = 1.2f;
        float scoreWidth = score.GetPreferredValues(score.text, Mathf.Infinity, Mathf.Infinity).x
            * scoreFontSize / score.fontSize;
        float multWidth = show ? multiplierText.GetPreferredValues(multiplierText.text,
            Mathf.Infinity, Mathf.Infinity).x * multiplierFontSize / multiplierText.fontSize : 0;
        float gap = show ? gapPx : 0;
        float fit = Mathf.Min(1, (frame.rect.width - 30 - gap) /
            Mathf.Max(1, scoreWidth * pulse + multWidth));
        score.fontSize = scoreFontSize * fit;
        multiplierText.fontSize = multiplierFontSize * fit;
        float visibleScore = scoreWidth * fit * followScore.localScale.x;
        float visibleMult = multWidth * fit;
        float left = frame.rect.center.x - (visibleScore + gap + visibleMult) * .5f;
        Place(followScore, frame, left + visibleScore * .5f, -27);
        if (show)
            Place(multiplierText.rectTransform, frame,
                left + visibleScore + gap + visibleMult * .5f, -27);
    }

    static void Place(RectTransform text, RectTransform frame, float x, float y)
    {
        text.pivot = new Vector2(.5f, .5f);
        text.sizeDelta = new Vector2(text.GetComponent<TMP_Text>().preferredWidth, 36);
        text.position = frame.TransformPoint(new Vector3(x, frame.rect.yMax + y, 0));
    }
}
