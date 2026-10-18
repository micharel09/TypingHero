using UnityEngine;

[DisallowMultipleComponent]
public sealed class PanelReveal : MonoBehaviour
{
    CanvasGroup group;
    Canvas canvas;
    bool visible;
    float elapsed;
    void Awake() { group = GetComponent<CanvasGroup>(); canvas = GetComponentInParent<Canvas>(); }
    void OnEnable() { elapsed = 0; visible = false; if (group) group.alpha = 0; }
    void Update()
    {
        if (canvas && !canvas.isActiveAndEnabled) { visible = false; return; }
        if (!visible) { elapsed = 0; visible = true; }
        elapsed += Time.unscaledDeltaTime;
        if (group) group.alpha = Mathf.SmoothStep(0, 1, elapsed / .12f);
    }
    void OnDisable() { visible = false; if (group) group.alpha = 1; }
}
