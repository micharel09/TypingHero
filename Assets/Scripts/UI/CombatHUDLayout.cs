using TMPro;
using UnityEngine;

// One position owner, after the rendered camera/Slayer viewport and before tutorial highlights.
[DisallowMultipleComponent]
[DefaultExecutionOrder(1050)]
public sealed class CombatHUDLayout : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] SlayerScreenFX slayerFX;
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
        // Fixed 320x180 HUD geometry is authored by CombatR7Build.
        scoreRoot.localScale = Vector3.one;
        if (targetCamera && enemyBody)
        {
            var parent = (RectTransform)popupAnchor.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
                targetCamera.WorldToScreenPoint(Head(enemyBody)), null, out var head);
            popupAnchor.position = parent.TransformPoint(head + new Vector2(24, 12));
        }
    }

}
