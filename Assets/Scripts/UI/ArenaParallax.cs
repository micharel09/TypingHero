using UnityEngine;

// Camera-relative presentation only: never move the arena's physics objects.
[DisallowMultipleComponent]
[DefaultExecutionOrder(200)]
public sealed class ArenaParallax : MonoBehaviour
{
    [SerializeField] Camera target;
    [SerializeField, Range(0, 1)] float follow = .5f;
    Vector3 origin, cameraOrigin;
    void Start() { origin = transform.position; if (target) cameraOrigin = target.transform.position; }
    void LateUpdate()
    {
        if (!target) return;
        var delta = target.transform.position - cameraOrigin; delta.z = 0;
        transform.position = origin + delta * follow;
    }
}
