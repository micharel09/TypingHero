using UnityEngine;

// Keep the world camera aligned with the UGUI combat region in embeds and fullscreen.
[DisallowMultipleComponent]
public sealed class CombatViewportCamera : MonoBehaviour
{
    [SerializeField] Camera target;
    RectTransform viewport;
    readonly Vector3[] corners = new Vector3[4];

    void Awake() => viewport = (RectTransform)transform;

    void LateUpdate()
    {
        if (!target || !viewport || Screen.width == 0 || Screen.height == 0) return;
        viewport.GetWorldCorners(corners);
        var bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        var topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
        target.rect = Rect.MinMaxRect(bottomLeft.x / Screen.width, bottomLeft.y / Screen.height,
            topRight.x / Screen.width, topRight.y / Screen.height);
    }
}
