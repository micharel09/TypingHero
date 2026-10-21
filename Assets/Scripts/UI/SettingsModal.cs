using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Mirrors the existing leaderboard visibility/focus contract without owning gameplay.
[DisallowMultipleComponent]
public sealed class SettingsModal : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] RectTransform audio;
    [SerializeField] CanvasGroup previousScreen;
    [SerializeField] Button backButton;
    Transform audioParent;
    Vector2 audioPosition, audioSize;
    bool audioActive;
    GameObject previousFocus;
    static SettingsModal current;
    static int closedFrame = -1;
    public static bool BlocksShortcuts => current || closedFrame == Time.frameCount;

    public void Open()
    {
        if (BlocksShortcuts || LeaderboardModal.BlocksShortcuts || !previousScreen.interactable) return;
        previousFocus = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        audioParent = audio.parent;
        audioPosition = audio.anchoredPosition; audioSize = audio.sizeDelta;
        audioActive = audio.gameObject.activeSelf;
        current = this;
        previousScreen.interactable = previousScreen.blocksRaycasts = false;
        audio.SetParent(panel.transform.Find("SettingsPanel"), false);
        audio.anchoredPosition = new Vector2(24, -60); audio.sizeDelta = new Vector2(372, 88);
        audio.gameObject.SetActive(true);
        LayoutAudio();
        panel.SetActive(true);
        backButton.Select();
    }

    // Save slider geometry because the result screen retains its existing audio layout.
    Vector2[] positions, sizes;
    TMPro.TMP_Text[] labels;
    float[] fontSizes;
    string[] labelTexts;
    RectTransform[] children;
    void LayoutAudio()
    {
        children = new RectTransform[audio.childCount]; positions = new Vector2[children.Length]; sizes = new Vector2[children.Length];
        labels = audio.GetComponentsInChildren<TMPro.TMP_Text>(true); fontSizes = new float[labels.Length]; labelTexts = new string[labels.Length];
        for (int i = 0; i < labels.Length; i++) { fontSizes[i] = labels[i].fontSize; labels[i].fontSize = 27; labelTexts[i] = labels[i].text; labels[i].text = labels[i].name == "MusicLabel" ? "Music" : "SFX"; }
        for (int i = 0; i < children.Length; i++)
        {
            var rect = children[i] = (RectTransform)audio.GetChild(i);
            positions[i] = rect.anchoredPosition; sizes[i] = rect.sizeDelta;
            bool slider = rect.GetComponent<Slider>();
            var label = rect.GetComponent<TMPro.TMP_Text>();
            int index = slider ? (rect.name == "SFXVolume" ? 1 : 0) : (label && label.text.ToUpperInvariant().Contains("SFX") ? 1 : 0);
            rect.anchoredPosition = new Vector2(slider ? 128 : 0, -(index * 44 + (slider ? 14 : 0)));
            rect.sizeDelta = new Vector2(slider ? 244 : 120, slider ? 12 : 36);
        }
    }

    public void Close()
    {
        if (current != this) return;
        closedFrame = Time.frameCount;
        panel.SetActive(false);
        audio.SetParent(audioParent, false); audio.anchoredPosition = audioPosition; audio.sizeDelta = audioSize;
        for (int i = 0; i < children.Length; i++) { children[i].anchoredPosition = positions[i]; children[i].sizeDelta = sizes[i]; }
        for (int i = 0; i < labels.Length; i++) { labels[i].fontSize = fontSizes[i]; labels[i].text = labelTexts[i]; }
        audio.gameObject.SetActive(audioActive);
        previousScreen.interactable = previousScreen.blocksRaycasts = true;
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(previousFocus);
        current = null;
    }
    void Update() { if (current == this && Input.GetKeyDown(KeyCode.Escape)) Close(); }
    void OnDisable() { if (current == this) Close(); }
}
