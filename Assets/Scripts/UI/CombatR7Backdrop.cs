using UnityEngine;

[DefaultExecutionOrder(1040)]
public sealed class CombatR7Backdrop : MonoBehaviour
{
    [SerializeField] Camera target;
    void LateUpdate()
    {
        if (!target) return;
        float height = target.orthographicSize * 2;
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, 0);
        transform.localScale = Vector3.one * (height / 1.8f);
    }
}
