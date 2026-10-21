using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Reuses the existing tutorial prompt, font, colors, reveal and button feedback.
public sealed class EndlessOverlay : MonoBehaviour
{
    TMP_Text heading, instruction;
    Button action;
    GameObject background;
    RectTransform prompt;
    Canvas canvas;
    CanvasGroup interaction;

    internal static EndlessOverlay Create()
    {
        var source = Object.FindObjectOfType<TutorialOverlay>(true);
        if (!source) return null;
        var root = Instantiate(source.gameObject, source.transform.parent);
        root.name = "EndlessHUD";
        root.SetActive(false);
        root.GetComponent<TutorialOverlay>().enabled = false;
        var overlay = root.AddComponent<EndlessOverlay>();
        overlay.canvas = root.GetComponent<Canvas>();
        overlay.interaction = root.GetComponent<CanvasGroup>();
        var design = root.transform.Find("Reference960x540");
        foreach (Transform child in design) child.gameObject.SetActive(child.name == "TutorialPrompt");
        overlay.prompt = (RectTransform)design.Find("TutorialPrompt");
        overlay.heading = overlay.prompt.Find("Heading").GetComponent<TMP_Text>();
        overlay.instruction = overlay.prompt.Find("Instruction").GetComponent<TMP_Text>();
        overlay.background = overlay.prompt.Find("InstructionBackground").gameObject;
        var buttons = overlay.prompt.GetComponentsInChildren<Button>(true);
        overlay.action = buttons[0];
        foreach (var button in buttons) button.gameObject.SetActive(false);
        overlay.action.onClick = new Button.ButtonClickedEvent();
        root.SetActive(true);
        return overlay;
    }

    internal void Show(string title, string message, string caption, UnityEngine.Events.UnityAction next)
    {
        prompt.anchoredPosition = new Vector2(48, -107);
        prompt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 262);
        // The clone owns its reading bounds; NEXT adds a third description line.
        Place(heading.rectTransform, 16, 0, 832, 56);
        Place(instruction.rectTransform, 16, 58, 832, 144);
        instruction.alignment = TextAlignmentOptions.MidlineLeft;
        Place((RectTransform)background.transform, 0, 0, 864, 208);
        Place((RectTransform)action.transform, 16, 210, 360, 52);
        var captionLabel = action.GetComponentInChildren<TMP_Text>(true);
        Place(captionLabel.rectTransform, 8, 0, 344, 52);
        captionLabel.enableWordWrapping = false;

        heading.gameObject.SetActive(true);
        heading.text = title; instruction.text = message;
        instruction.gameObject.SetActive(true); background.SetActive(true);
        action.gameObject.SetActive(true);
        action.GetComponentInChildren<TMP_Text>(true).text = caption;
        action.onClick.RemoveAllListeners(); action.onClick.AddListener(next);
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (events) events.SetSelectedGameObject(null);
    }

    internal void ShowCombat(string title)
    {
        // The fixed HUD owns difficulty/duel during combat.
        prompt.anchoredPosition = new Vector2(144, -156);
        heading.gameObject.SetActive(false);
        instruction.gameObject.SetActive(false); background.SetActive(false); action.gameObject.SetActive(false);
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (events) events.SetSelectedGameObject(null);
    }

    static void Place(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
    }

    void LateUpdate()
    {
        canvas.enabled = !GamePauseController.IsPaused;
        interaction.interactable = interaction.blocksRaycasts = canvas.enabled && action.gameObject.activeSelf;
    }
}
