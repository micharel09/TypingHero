using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Keep the pictured plus sign's three-source-pixel stems on a physical pixel.
// Move only its quad, within its existing advance; never dilate the font.
[DisallowMultipleComponent]
public sealed class BlackletterPixelSnap : MonoBehaviour
{
    [SerializeField] Vector2 plusAnchor = new Vector2(11.5f, 26.5f);
    struct Glyph
    {
        public int mesh, vertex;
        public Vector3 anchor, a, b, c, d;
    }
    readonly List<Glyph> glyphs = new List<Glyph>();
    TMP_Text text;
    Canvas canvas;

    void OnEnable()
    {
        text = GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();
        if (!text) return;
        text.OnPreRenderText += Capture;
        Canvas.willRenderCanvases += Snap;
        text.havePropertiesChanged = true;
    }

    void OnDisable()
    {
        if (text) { text.OnPreRenderText -= Capture; text.havePropertiesChanged = true; }
        Canvas.willRenderCanvases -= Snap;
        glyphs.Clear();
    }

    void Capture(TMP_TextInfo info)
    {
        glyphs.Clear();
        if (!text.fontSharedMaterial || text.fontSharedMaterial.shader.name != "CLACKBLADE/Text/Pixel SDF") return;
        for (int i = 0; i < info.characterCount; i++)
        {
            var ch = info.characterInfo[i];
            if (!ch.isVisible || ch.character != '+' ||
                ch.fontAsset.faceInfo.familyName != "Clackblade Specimen Pixel") continue;
            var vertices = info.meshInfo[ch.materialReferenceIndex].vertices;
            int v = ch.vertexIndex;
            glyphs.Add(new Glyph { mesh = ch.materialReferenceIndex, vertex = v,
                anchor = new Vector3(ch.origin + plusAnchor.x * ch.scale, ch.baseLine + plusAnchor.y * ch.scale),
                a = vertices[v], b = vertices[v + 1], c = vertices[v + 2], d = vertices[v + 3] });
        }
        Apply(info);
    }

    void Snap()
    {
        if (!text || !text.isActiveAndEnabled || glyphs.Count == 0) return;
        // Popup motion and canvas/DPR resize can change phase without regenerating text.
        Apply(text.textInfo);
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    void Apply(TMP_TextInfo info)
    {
        var rect = text.rectTransform;
        var camera = canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null :
            canvas && canvas.worldCamera ? canvas.worldCamera : Camera.main;
        for (int i = 0; i < glyphs.Count; i++)
        {
            var glyph = glyphs[i];
            if (glyph.mesh >= info.meshInfo.Length) continue;
            var vertices = info.meshInfo[glyph.mesh].vertices;
            if (vertices == null || glyph.vertex + 3 >= vertices.Length) continue;
            Vector3 world = rect.TransformPoint(glyph.anchor);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(camera, world);
            screen = new Vector2(Mathf.Floor(screen.x) + .5f, Mathf.Floor(screen.y) + .5f);
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screen, camera, out var snapped)) continue;
            Vector3 delta = rect.InverseTransformVector(snapped - world);
            vertices[glyph.vertex] = glyph.a + delta;
            vertices[glyph.vertex + 1] = glyph.b + delta;
            vertices[glyph.vertex + 2] = glyph.c + delta;
            vertices[glyph.vertex + 3] = glyph.d + delta;
        }
    }
}
