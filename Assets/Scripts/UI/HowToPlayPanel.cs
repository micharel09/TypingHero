using System;
using UnityEngine;

public sealed class HowToPlayPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    Action afterClose;
    public static bool Seen => PlayerPrefs.GetInt("HowToPlaySeen", 0) != 0;
    public bool IsOpen => panel && panel.activeSelf;
    public void Show() => Open(null);
    public void Open(Action onClose)
    {
        afterClose = onClose;
        panel.SetActive(true);
    }
    public void Close()
    {
        PlayerPrefs.SetInt("HowToPlaySeen", 1);
        PlayerPrefs.Save();
        panel.SetActive(false);
        var action = afterClose;
        afterClose = null;
        action?.Invoke();
    }
}
