using UnityEngine;
using Cinemachine;

// Keep the world camera aligned with the UGUI combat region in embeds and fullscreen.
[DisallowMultipleComponent]
[DefaultExecutionOrder(990)]
public sealed class CombatViewportCamera : MonoBehaviour
{
    [SerializeField] Camera target;
    [SerializeField] bool anchorFloorDuringZoom;
    [SerializeField] float normalOrtho = 5.625f;
    CinemachineBrain brain;
    RectTransform viewport;
    readonly Vector3[] corners = new Vector3[4];

    void Awake()
    {
        viewport = (RectTransform)transform;
        if (target) brain = target.GetComponent<CinemachineBrain>();
    }

    void LateUpdate()
    {
        if (!target || !viewport || Screen.width == 0 || Screen.height == 0) return;
        viewport.GetWorldCorners(corners);
        var bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        var topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
        target.rect = Rect.MinMaxRect(bottomLeft.x / Screen.width, bottomLeft.y / Screen.height,
            topRight.x / Screen.width, topRight.y / Screen.height);
        if (anchorFloorDuringZoom && brain && brain.isActiveAndEnabled)
        {
            // Keep the real floor at native y138 while the existing lens zooms.
            // Use the brain's final position to retain impulses without accumulating
            // corrections; the pause controller owns the camera while brain is disabled.
            var position = brain.CurrentCameraState.FinalPosition;
            position.y += (target.orthographicSize - normalOrtho) * (48f / 90f);
            target.transform.position = position;
        }
    }
}
