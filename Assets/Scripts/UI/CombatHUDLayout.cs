using TMPro;
using UnityEngine;

// One position owner, after the rendered camera/Slayer viewport and before tutorial highlights.
[DisallowMultipleComponent]
[DefaultExecutionOrder(1050)]
public sealed class CombatHUDLayout : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] BoxCollider2D playerBody, enemyBody;
    [SerializeField] Vector2 bodyVerticalInset = new Vector2(.28f, .12f);
    [SerializeField] RectTransform playerStatus, enemyStatus, typingTray, scoreRoot, popupAnchor;
    [SerializeField] TMP_Text scoreText, multiplierText;
    [SerializeField] RectTransform comboRoot, comboDecay;

    public Vector3 Head(BoxCollider2D body) => new Vector3(body.bounds.center.x,
        body.bounds.max.y - bodyVerticalInset.x, body.bounds.center.z);
    public Vector3 Foot(BoxCollider2D body) => new Vector3(body.bounds.center.x,
        body.bounds.min.y + bodyVerticalInset.y, body.bounds.center.z);
    public Vector3 HighestHead => Head(playerBody).y >= Head(enemyBody).y ? Head(playerBody) : Head(enemyBody);
    public Vector3 LowestFoot => Foot(playerBody).y <= Foot(enemyBody).y ? Foot(playerBody) : Foot(enemyBody);

    void LateUpdate()
    {
        var root = (RectTransform)transform;
        LayoutScore(root);
        if (targetCamera && enemyBody)
        {
            var parent = (RectTransform)popupAnchor.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
                targetCamera.WorldToScreenPoint(Head(enemyBody)), null, out var head);
            popupAnchor.position = parent.TransformPoint(head + new Vector2(24, 12));
        }
        // Keep the last projected positions after lethal destroys either actor.
        if (!targetCamera || !playerBody || !enemyBody) return;
        PlaceBelowFoot(playerStatus, playerBody);
        PlaceBelowFoot(enemyStatus, enemyBody);
        float lowest = Mathf.Min(BottomIn(root, playerStatus), BottomIn(root, enemyStatus));
        PlaceTopLeft(typingTray, root.TransformPoint(new Vector3(root.rect.center.x - 336, lowest - 12, 0)));
    }

    void PlaceBelowFoot(RectTransform status, BoxCollider2D body)
    {
        var parent = (RectTransform)status.parent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            targetCamera.WorldToScreenPoint(Foot(body)), null, out var foot);
        PlaceTopLeft(status, parent.TransformPoint(new Vector3(foot.x - 56, foot.y - 12, 0)));
    }

    static float BottomIn(RectTransform root, RectTransform item) =>
        root.InverseTransformPoint(item.TransformPoint(new Vector3(0, item.rect.yMin, 0))).y;

    static void PlaceTopLeft(RectTransform item, Vector3 point)
    {
        // Builder uses a top-left pivot; keep scale ownership with the score effects.
        item.position = point;
    }

    void LayoutScore(RectTransform root)
    {
        float width = scoreText.preferredWidth;
        float pulse = scoreText.transform.localScale.x;
        float multiplierWidth = multiplierText.gameObject.activeSelf ? multiplierText.preferredWidth : 0;
        scoreRoot.localScale = Vector3.one;
        scoreText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        scoreRoot.position = root.TransformPoint(new Vector3(root.rect.xMax - 24 - width * pulse,
            root.rect.yMax - 24, 0));
        comboRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, multiplierWidth);
        multiplierText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, multiplierWidth);
        comboRoot.position = root.TransformPoint(new Vector3(root.rect.xMax - 24 - multiplierWidth,
            root.rect.yMax - 24 - 24 * pulse - 4, 0));
        comboDecay.gameObject.SetActive(multiplierText.gameObject.activeSelf);
        comboDecay.position = root.TransformPoint(new Vector3(root.rect.xMax - 24 - 112,
            root.InverseTransformPoint(comboRoot.position).y - 20, 0));
    }
}
