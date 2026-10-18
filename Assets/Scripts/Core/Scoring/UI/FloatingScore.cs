using UnityEngine;
using TMPro;

[DefaultExecutionOrder(1060)]
public class FloatingScore : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] float riseDistance = 18f;
    [SerializeField] float life = 0.6f;
    [SerializeField] float fadeDelay = 0.15f;

    RectTransform _rt;             // của chính popup
    Canvas _canvas;
    RectTransform _anchor;         // nơi bám (trên Canvas)
    Vector2 _offset;
    float _elapsed, _riseElapsed;
    int _total;
    bool _paused;
    Vector2 _lastAnchor;

    public void SetPaused(bool paused) => _paused = paused;

    public void AddAward(int delta, string format, Color color)
    {
        _total += delta;
        label.text = "+" + _total.ToString(format);
        color.a = 1;
        label.color = color;
        _elapsed = 0;
    }

    public void Setup(string text, Color color, Canvas canvas, RectTransform anchor, Vector2 offset)
    {
        if (!_rt) _rt = GetComponent<RectTransform>();
        _rt.anchorMin = _rt.anchorMax = ((RectTransform)canvas.transform).pivot;
        _rt.pivot = new Vector2(0, .5f);
        _canvas = canvas;
        _anchor = anchor;

        if (!label) label = GetComponentInChildren<TMP_Text>(true);
        label.text = text;
        label.color = color;

        // lấy vị trí anchor
        _offset = offset;
        _rt.anchoredPosition = ResolveAnchorPosition(_canvas, _anchor) + _offset;

        _elapsed = _riseElapsed = 0;
        _total = 0;
        _lastAnchor = ResolveAnchorPosition(_canvas, _anchor);
        enabled = true;
    }


    void Reset()
    {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!label) label = GetComponentInChildren<TMP_Text>(true);
    }

    void LateUpdate()
    {
        if (!_paused) { _elapsed += Time.unscaledDeltaTime; _riseElapsed += Time.unscaledDeltaTime; }
        float t = Mathf.Clamp01(_elapsed / life);
        if (_anchor) _lastAnchor = ResolveAnchorPosition(_canvas, _anchor);
        _rt.anchoredPosition = _lastAnchor + _offset + Vector2.up * (riseDistance * Mathf.Clamp01(_riseElapsed / life));

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
