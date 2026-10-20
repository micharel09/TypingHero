using UnityEngine;
using System.Collections.Generic;

// Animation owns the visible cel. No independent timer or damage-window approximation.
[DefaultExecutionOrder(220)]
public sealed class SlashVolume : MonoBehaviour
{
    [SerializeField] SpriteRenderer source;
    [SerializeField] SlashLibrary library;
    readonly Dictionary<Sprite, SlashLibrary.Frame> lookup = new();
    MeshFilter front, back;
    MeshRenderer frontRenderer, backRenderer;
    void Awake()
    {
        foreach (var frame in library.frames) lookup[frame.sprite] = frame;
        front = Part("SlashFront", out frontRenderer); back = Part("SlashBack", out backRenderer);
    }
    MeshFilter Part(string name, out MeshRenderer renderer)
    {
        var part = new GameObject(name); part.transform.SetParent(transform, false);
        var filter = part.AddComponent<MeshFilter>(); renderer = part.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = library.material; renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false; renderer.enabled = false; return filter;
    }
    void LateUpdate()
    {
        if (!source || !source.enabled || CombatSession.Finished || !source.sprite || !lookup.TryGetValue(source.sprite, out var frame))
        { Hide(); return; }
        front.sharedMesh = frame.front; back.sharedMesh = frame.back;
        frontRenderer.enabled = frame.front; backRenderer.enabled = frame.back;
        frontRenderer.sortingLayerID = backRenderer.sortingLayerID = source.sortingLayerID;
        frontRenderer.sortingOrder = source.sortingOrder + 1; backRenderer.sortingOrder = source.sortingOrder - 1;
        front.transform.localScale = back.transform.localScale = new Vector3(source.flipX ? -1 : 1, source.flipY ? -1 : 1, 1);
    }
    void Hide() { if (frontRenderer) frontRenderer.enabled = false; if (backRenderer) backRenderer.enabled = false; }
    void OnDisable() => Hide();
}
