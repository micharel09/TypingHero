using UnityEngine;
using System.Collections.Generic;

// Animation owns geometry; Slayer additionally expires a held swing's luminous trail.
[DefaultExecutionOrder(220)]
public sealed class SlashVolume : MonoBehaviour
{
    [SerializeField] SpriteRenderer source;
    [SerializeField] SlashLibrary library;
    [SerializeField] Material normalPlayerMaterial;
    readonly Dictionary<Sprite, SlashLibrary.Frame> lookup = new();
    MeshFilter front, back;
    MeshRenderer frontRenderer, backRenderer;
    bool slayerSwing;
    float slayerAge;
    int bornFrame;
    Sprite lastSlayerCel;
    PlayerSlayerMode slayerOwner;
    internal bool TrailVisible => (frontRenderer && frontRenderer.enabled) || (backRenderer && backRenderer.enabled);
    internal void BeginSlayerSwing()
    {
        slayerSwing = true; slayerAge = 0; bornFrame = Time.frameCount;
        lastSlayerCel = source ? source.sprite : null;
    }
    void OnEnable() => SlayerModeSignals.OnSetActive += OnSlayerChanged;
    void OnSlayerChanged(bool active)
    {
        if (!slayerOwner) return;
        // Entry must not retain a preceding parry; exit must not revive the held cel.
        slayerSwing = true; slayerAge = .12f;
        lastSlayerCel = source ? source.sprite : null;
    }
    void Awake()
    {
        slayerOwner = GetComponentInParent<PlayerSlayerMode>();
        if (normalPlayerMaterial)
        {
            // Prepare the declared lighting variants before the first visible swing.
            var variants = new ShaderVariantCollection();
            var keywords = new List<string>(3);
            foreach (var main in new[] { "", "_MAIN_LIGHT_SHADOWS", "_MAIN_LIGHT_SHADOWS_CASCADE" })
            for (int additional = 0; additional < 2; additional++)
            for (int shadows = 0; shadows < 2; shadows++)
            {
                keywords.Clear();
                if (main.Length > 0) keywords.Add(main);
                if (additional != 0) keywords.Add("_ADDITIONAL_LIGHTS");
                if (shadows != 0) keywords.Add("_ADDITIONAL_LIGHT_SHADOWS");
                variants.Add(new ShaderVariantCollection.ShaderVariant(normalPlayerMaterial.shader,
                    UnityEngine.Rendering.PassType.Normal, keywords.ToArray()));
            }
            variants.WarmUp();
            Destroy(variants);
        }
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
        if (slayerSwing && !SlayerModeSignals.Active && source && source.sprite != lastSlayerCel)
            slayerSwing = false;
        if (slayerSwing)
        {
            if (!GamePauseController.InputBlocked && Time.timeScale != 0 && bornFrame != Time.frameCount)
                slayerAge += Time.unscaledDeltaTime;
            if (SlayerModeSignals.Active && source) lastSlayerCel = source.sprite;
        }
        if ((slayerOwner && SlayerModeSignals.Active && !slayerSwing) || (slayerSwing && slayerAge >= .12f))
        { Hide(); return; }
        if (!source || !source.enabled || CombatSession.Finished || !source.sprite || !lookup.TryGetValue(source.sprite, out var frame))
        { Hide(); return; }
        var material = normalPlayerMaterial && !SlayerModeSignals.Active ? normalPlayerMaterial : library.material;
        frontRenderer.sharedMaterial = backRenderer.sharedMaterial = material;
        front.sharedMesh = frame.front; back.sharedMesh = frame.back;
        frontRenderer.enabled = frame.front; backRenderer.enabled = frame.back;
        frontRenderer.sortingLayerID = backRenderer.sortingLayerID = source.sortingLayerID;
        frontRenderer.sortingOrder = source.sortingOrder + 1; backRenderer.sortingOrder = source.sortingOrder - 1;
        front.transform.localScale = back.transform.localScale = new Vector3(source.flipX ? -1 : 1, source.flipY ? -1 : 1, 1);
    }
    void Hide() { if (frontRenderer) frontRenderer.enabled = false; if (backRenderer) backRenderer.enabled = false; }
    void OnDisable() { SlayerModeSignals.OnSetActive -= OnSlayerChanged; slayerSwing = false; Hide(); }
}
