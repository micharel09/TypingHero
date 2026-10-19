using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class TypingPanelFeedback : MonoBehaviour
{
    [SerializeField] Image[] borders;
    [SerializeField] Color idle = new Color(.22f, .42f, .4f);
    [SerializeField] Color error = new Color(.8f, .25f, .2f);
    float remaining;
    void OnEnable() { TypingManager.OnInputResult += OnInput; TypingManager.OnWordTimeout += Flash; }
    void OnDisable()
    {
        TypingManager.OnInputResult -= OnInput; TypingManager.OnWordTimeout -= Flash;
        remaining = 0; Paint(idle);
    }
    void OnInput(bool correct) { if (!correct) Flash(); }
    void Flash() => remaining = .18f;
    void Update()
    {
        if (GamePauseController.InputBlocked) return;
        remaining = Mathf.Max(0, remaining - Time.unscaledDeltaTime);
        Paint(Color.Lerp(idle, error, remaining / .18f));
    }
    void Paint(Color color) { if (borders != null) foreach (var border in borders) if (border) border.color = color; }
}
