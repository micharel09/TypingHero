using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The modal owns visibility/focus only. The pause controller retains match ownership.
[DisallowMultipleComponent]
public sealed class LeaderboardModal : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Button closeButton;
    [SerializeField] CanvasGroup previousScreen;
    [SerializeField] LeaderboardUIBinder binder;
    GameObject previousFocus;
    static LeaderboardModal current;
    static int closedFrame = -1;
    public static bool IsOpen => current && current.panel && current.panel.activeInHierarchy;
    public static bool BlocksShortcuts => IsOpen || closedFrame == Time.frameCount;

    public void Open()
    {
        if (BlocksShortcuts) return;
        var canvas = previousScreen ? previousScreen.GetComponentInParent<Canvas>() : null;
        if ((canvas && !canvas.isActiveAndEnabled) || (previousScreen && !previousScreen.interactable)) return;
        previousFocus = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        current = this;
        if (previousScreen) { previousScreen.interactable = false; previousScreen.blocksRaycasts = false; }
        panel.SetActive(true);
        if (binder) binder.enabled = true;
        if (closeButton) closeButton.Select();
    }

    public void Close()
    {
        closedFrame = Time.frameCount;
        panel.SetActive(false);
        if (binder) binder.enabled = false;
        if (previousScreen) { previousScreen.interactable = true; previousScreen.blocksRaycasts = true; }
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(previousFocus);
        if (current == this) current = null;
    }

    void Update()
    {
        if (current == this && IsOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
    }

    void OnDisable()
    {
        if (current == this) Close();
    }
}
