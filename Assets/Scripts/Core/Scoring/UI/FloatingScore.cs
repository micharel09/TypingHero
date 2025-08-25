using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] float riseDistance = 48f;
    [SerializeField] float life = 0.6f;
    [SerializeField] float fadeDelay = 0.15f;

    RectTransform _rt;             // của chính popup
    Canvas _canvas;
    RectTransform _anchor;         // nơi bám (trên Canvas)
    Vector2 _start;
    Vector2 _end;
    float _t0;

public void Setup(string text, Color color, Canvas canvas, RectTransform anchor, Vector2 offset)
    {
        if (!_rt) _rt = GetComponent<RectTransform>();
        _canvas = canvas;
        _anchor = anchor;

        if (!label) label = GetComponentInChildren<TMP_Text>(true);
        label.text = text;
        label.color = color;

        // lấy vị trí anchor
        _start = ResolveAnchorPosition(_canvas, _anchor) + offset;
        _end   = _start + new Vector2(0, riseDistance);
        _rt.anchoredPosition = _start;

        _t0 = Time.unscaledTime;
        enabled = true;
    }


    void Reset()
    {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!label) label = GetComponentInChildren<TMP_Text>(true);
    }

    void Update()
    {
        float t = Mathf.InverseLerp(_t0, _t0 + life, Time.unscaledTime);
        _rt.anchoredPosition = Vector2.LerpUnclamped(_start, _end, t);

        var c = label.color;
        float fadeStart = fadeDelay / Mathf.Max(0.0001f, life);
        float a = (t <= fadeStart) ? 1f : Mathf.Lerp(1f, 0f, (t - fadeStart) / (1f - fadeStart));
        c.a = a;
        label.color = c;

        if (t >= 1f) Destroy(gameObject);
    }

    // —— tính anchoredPosition đúng trong mọi RenderMode ——
    static Vector2 ResolveAnchorPosition(Canvas cv, RectTransform anchor)
    {
        var canvasRT = cv.transform as RectTransform;

        if (anchor != null)
        {
            // convert anchor world → local của canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRT,
                RectTransformUtility.WorldToScreenPoint(cv.worldCamera, anchor.position),
                cv.renderMode == RenderMode.ScreenSpaceOverlay ? null : cv.worldCamera,
                out Vector2 local);
            return local;
        }

        // fallback: giữa canvas
        return Vector2.zero;
    }
}
