using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(1100)]
public sealed class TutorialOverlay : MonoBehaviour
{
    internal enum Focus { Typing, Cue, Stamina, Combine }
    [SerializeField] TMP_Text heading, instruction;
    [SerializeField] Button continueButton;
    [SerializeField] GameObject trayHighlight, staminaHighlight;
    [SerializeField] RectTransform cueHighlight;
    [SerializeField] Transform cue;
    [SerializeField] RectTransform typingTray, staminaTarget;
    Canvas canvas;
    CanvasGroup interaction;
    Focus focus;

    void Awake() { canvas = GetComponent<Canvas>(); interaction = GetComponent<CanvasGroup>(); }
    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled;
        FollowHighlight((RectTransform)trayHighlight.transform, typingTray);
        FollowHighlight((RectTransform)staminaHighlight.transform, staminaTarget);
        bool cueVisible = cue && cue.gameObject.activeInHierarchy;
        trayHighlight.SetActive(focus == Focus.Typing || (focus == Focus.Combine && !cueVisible));
        staminaHighlight.SetActive(focus == Focus.Stamina);
        cueHighlight.gameObject.SetActive(cueVisible && (focus == Focus.Cue || focus == Focus.Combine));
        if (!cueHighlight.gameObject.activeSelf) return;
        var point = CombatViewportCamera.WorldToScreenPoint(Camera.main, cue.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)cueHighlight.parent, point, null, out var local);
        cueHighlight.anchoredPosition = local;
    }

    static void FollowHighlight(RectTransform highlight, RectTransform target)
    {
        if (!target) return;
        highlight.position = target.TransformPoint(new Vector3(target.rect.xMin - 4, target.rect.yMax + 4, 0));
        var size = target.rect.size + new Vector2(8, 8);
        highlight.sizeDelta = size;
        // Existing four edges keep their identities; update geometry with their target.
        var boxes = new[] { new Rect(0, 0, size.x, 2), new Rect(0, size.y - 2, size.x, 2),
            new Rect(0, 0, 2, size.y), new Rect(size.x - 2, 0, 2, size.y) };
        for (int i = 0; i < 4; i++)
        {
            var edge = (RectTransform)highlight.GetChild(i);
            edge.anchoredPosition = new Vector2(boxes[i].x, -boxes[i].y);
            edge.sizeDelta = boxes[i].size;
        }
    }

    internal void Show(string title, string message, string action, Focus target)
    {
        heading.text = title;
        instruction.text = message;
        continueButton.gameObject.SetActive(!string.IsNullOrEmpty(action));
        continueButton.GetComponentInChildren<TMP_Text>(true).text = action;
        focus = target;
        ClearSelection();
    }

    internal void SubmitContinue()
    {
        if (continueButton.gameObject.activeInHierarchy && continueButton.IsInteractable())
            continueButton.onClick.Invoke();
    }

    internal void ClearSelection()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
    }
}
