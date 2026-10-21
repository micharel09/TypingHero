using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Only Pause changes geometry. Restore the authored result layout before gameplay resumes.
public sealed class CompactPauseLayout : MonoBehaviour
{
    readonly List<System.Action> restore = new List<System.Action>();
    void Place(RectTransform rt, float x, float y, float w, float h)
    {
        var position = rt.anchoredPosition; var size = rt.sizeDelta;
        restore.Add(() => { rt.anchoredPosition = position; rt.sizeDelta = size; });
        rt.anchoredPosition = new Vector2(x, -y); rt.sizeDelta = new Vector2(w, h);
    }
    void Awake() { transform.Find("SettingsButton").gameObject.SetActive(false); }
    public void Apply(bool paused)
    {
        foreach (var reset in restore) reset();
        restore.Clear();
        if (!paused) return;
        Place((RectTransform)transform, 270, 100, 420, 340);
        var title = transform.Find("PauseLabel").GetComponent<TMP_Text>();
        Place(title.rectTransform, 24, 20, 372, 40);
        float titleSize = title.fontSize; restore.Add(() => title.fontSize = titleSize); title.fontSize = 30;
        string[] names = { "ResumeButton", "RestartButton", "SettingsButton", "Top 1000Button", "Main MenuButton" };
        for (int i = 0; i < names.Length; i++)
        {
            var button = transform.Find(names[i]);
            bool active = button.gameObject.activeSelf; restore.Add(() => button.gameObject.SetActive(active)); button.gameObject.SetActive(true);
            Place((RectTransform)button, 24, 72 + i * 44, 372, 36);
            var label = button.GetComponentInChildren<TMP_Text>(true);
            Place(label.rectTransform, 8, 0, 356, 36);
            float size = label.fontSize; restore.Add(() => label.fontSize = size); label.fontSize = 24;
        }
        foreach (string name in new[] { "AudioPreferences", "KeyHint", "RestartButton/key-r" })
        {
            var child = transform.Find(name).gameObject; bool active = child.activeSelf;
            restore.Add(() => child.SetActive(active)); child.SetActive(false);
        }
    }
}
