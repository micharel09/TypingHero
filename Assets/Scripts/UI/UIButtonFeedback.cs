using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class UIButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] SfxEvent sound;
    RectTransform label;
    Vector2 origin;
    Button button;
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Play);
        var text = GetComponentInChildren<TMPro.TMP_Text>();
        label = text ? text.rectTransform : null;
        if (label) origin = label.anchoredPosition;
    }
    void Press(bool down)
    {
        if (label) label.anchoredPosition = origin + (down ? Vector2.down : Vector2.zero);
    }
    public void OnPointerDown(PointerEventData data) { if (button.IsInteractable()) Press(true); }
    public void OnPointerUp(PointerEventData data) => Press(false);
    public void OnPointerExit(PointerEventData data) => Press(false);
    void Play()
    {
        var canvas = GetComponentInParent<Canvas>();
        if ((!canvas || canvas.isActiveAndEnabled) && SfxPlayer.I) SfxPlayer.I.Play(sound, Vector3.zero);
    }
    void OnDestroy() { if (button) button.onClick.RemoveListener(Play); }
    void OnDisable() => Press(false);
}
