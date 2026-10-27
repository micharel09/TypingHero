using TMPro;
using UnityEngine;
using UnityEngine.UI;

// READY borrows the combat frame while keeping its rail and bindings intact.
internal sealed class ReadyPromptLayout
{
    readonly RectTransform rail;
    readonly TextMeshProUGUI word, secondary;
    readonly Slider countdown;
    readonly Image board;
    readonly float pixel;
    readonly bool boardEnabled, countdownActive;
    RectTransform frame, instruction;
    Vector2 wordPosition;

    internal ReadyPromptLayout(RectTransform rail, TextMeshProUGUI word,
        TextMeshProUGUI secondary, Slider countdown, float pixel)
    {
        this.rail = rail; this.word = word; this.secondary = secondary;
        this.countdown = countdown; this.pixel = pixel;
        board = rail.parent.GetComponent<Image>();
        boardEnabled = board.enabled;
        countdownActive = countdown && countdown.gameObject.activeSelf;
    }

    internal void Show()
    {
        word.ForceMeshUpdate();
        // Measure the complete READY once. Typed prefix colors never change its box.
        Bounds ink = word.textBounds;
        wordPosition = rail.rect.center - (Vector2)ink.center;
        frame = Node("ReadyFrame");
        frame.sizeDelta = new Vector2(Mathf.Ceil(ink.size.x / pixel) + 16,
            Mathf.Ceil(ink.size.y / pixel) + 8) * pixel;
        frame.SetAsFirstSibling();
        var image = frame.gameObject.AddComponent<Image>();
        image.sprite = board.sprite; image.color = board.color;
        image.type = Image.Type.Sliced; image.pixelsPerUnitMultiplier = 1f / pixel;
        image.raycastTarget = false;
        board.enabled = false;
        if (countdown) countdown.gameObject.SetActive(false);

        instruction = Node("ReadyInstruction");
        var label = instruction.gameObject.AddComponent<TextMeshProUGUI>();
        label.font = secondary.font; label.fontSharedMaterial = secondary.fontSharedMaterial;
        label.fontSize = 8 * pixel;
        // The combat word's base ink is cream; rich-text progress remains on READY only.
        label.color = new Color32(242, 231, 207, 255);
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = label.enableWordWrapping = label.raycastTarget = false;
        label.overflowMode = TextOverflowModes.Overflow;
        label.text = "TYPE READY TO CONTINUE";
        instruction.sizeDelta = label.GetPreferredValues();
        instruction.anchoredPosition = new Vector2(0,
            frame.sizeDelta.y / 2 + 6 * pixel + instruction.sizeDelta.y / 2);
        PlaceWord();
    }

    RectTransform Node(string name)
    {
        var node = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        node.SetParent(rail.parent, false);
        node.anchorMin = node.anchorMax = node.pivot = new Vector2(.5f, .5f);
        return node;
    }

    internal void PlaceWord()
    {
        if (frame) word.rectTransform.anchoredPosition = wordPosition;
    }

    internal void Restore()
    {
        board.enabled = boardEnabled;
        if (countdown) countdown.gameObject.SetActive(countdownActive);
        foreach (var node in new[] { frame, instruction })
            if (node) { node.gameObject.SetActive(false); Object.Destroy(node.gameObject); }
    }
}
