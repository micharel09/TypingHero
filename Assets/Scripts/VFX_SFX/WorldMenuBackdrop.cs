using UnityEngine;

// Match the existing menu background rectangles while rendering their art before UI.
[DefaultExecutionOrder(300)]
public sealed class WorldMenuBackdrop : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] RectTransform[] sources;
    [SerializeField] SpriteRenderer[] layers;
    readonly Vector3[] corners = new Vector3[4];
    void LateUpdate()
    {
        if (!targetCamera) return;
        for (int i = 0; i < sources.Length; i++)
        {
            if (!sources[i] || !layers[i] || !layers[i].sprite) continue;
            sources[i].GetWorldCorners(corners);
            var min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            var max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            float distance = -targetCamera.transform.position.z;
            Vector3 bottom = targetCamera.ScreenToWorldPoint(new Vector3(min.x,min.y,distance));
            Vector3 top = targetCamera.ScreenToWorldPoint(new Vector3(max.x,max.y,distance));
            layers[i].transform.position = (bottom+top)*.5f;
            var size = layers[i].sprite.bounds.size;
            layers[i].transform.localScale = new Vector3((top.x-bottom.x)/size.x,(top.y-bottom.y)/size.y,1);
        }
    }
}
