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
        if (!targetCamera || !playerBody || !enemyBody) return;
        PlaceBelowFoot(playerStatus, playerBody);
        PlaceBelowFoot(enemyStatus, enemyBody);
        var root = (RectTransform)transform;
        float lowest = Mathf.Min(BottomIn(root, playerStatus), BottomIn(root, enemyStatus));
        PlaceTopLeft(typingTray, root.TransformPoint(new Vector3(root.rect.center.x - 336, lowest - 8 - 48 - 8, 0)));
        LayoutScore(root, lowest - 8);
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

    void LayoutScore(RectTransform root, float rowTop)
    {
        float scoreWidth = scoreText.preferredWidth;
        float pulse = scoreText.transform.localScale.x;
        bool showMultiplier = multiplierText.gameObject.activeSelf;
        float multiplierWidth = showMultiplier ? multiplierText.preferredWidth : 0;
        float total = scoreWidth * pulse + (showMultiplier ? 8 + multiplierWidth : 0);
        scoreText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scoreWidth);
        comboRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, multiplierWidth);
        multiplierText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, multiplierWidth);
        comboRoot.anchoredPosition = new Vector2(scoreWidth * pulse + 8, 0);
        scoreRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, total);
        float scale = scoreRoot.localScale.x;
        // Include both the cluster zoom and the award pulse when centering visible text.
        scoreRoot.position = root.TransformPoint(new Vector3(root.rect.center.x - total * scale * .5f,
            rowTop, 0));
        comboDecay.position = root.TransformPoint(new Vector3(root.rect.center.x - 56,
            rowTop - Mathf.Max(24 * pulse, 24) * scale - 4, 0));
        popupAnchor.position = root.TransformPoint(new Vector3(root.rect.center.x + total * scale * .5f + 12,
            rowTop - 18, 0));
    }
}
