using System;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Editor-only primitives. The resulting scenes/prefabs contain ordinary UGUI objects.
internal static class PixelUIFactory
{
    internal const string Art = "Assets/Art/UI/Pixel/";
    internal const string Generated = "Assets/Prefabs/PixelUI/";
    internal static readonly Color Ink = Hex("101820"), Panel = Hex("243746"),
        Edge = Hex("4C6877"), Text = Hex("F4EEDA"), Muted = Hex("9AAFB9"),
        Teal = Hex("70E0CE"), Red = Hex("EB6570"), Gold = Hex("F2C66D");
    internal static TMP_FontAsset Font;
    internal static Sprite Frame;
    internal static bool BuildingTemplates;

    static RectTransform Primitive(string name, string prefab, Transform parent)
    {
        var source = BuildingTemplates ? null : AssetDatabase.LoadAssetAtPath<GameObject>(Generated + prefab + ".prefab");
        if (!source) return Rect(name, parent);
        var go = (GameObject)PrefabUtility.InstantiatePrefab(source, parent);
        go.name = name;
        return go.GetComponent<RectTransform>();
    }

    internal static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out var c);
        return c;
    }

    internal static RectTransform Rect(string name, Transform parent)
    {
        var rt = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    internal static void Box(RectTransform rt, Transform parent, float x, float y, float w, float h)
    {
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(w, h);
    }

    internal static void Stretch(RectTransform rt, Transform parent, float inset = 0)
    {
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(.5f, .5f);
        rt.offsetMin = new Vector2(inset, inset); rt.offsetMax = new Vector2(-inset, -inset);
    }

    internal static Canvas Canvas(string name, int order)
    {
        var rt = Rect(name, null);
        var c = rt.gameObject.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = order;
        c.pixelPerfect = true;
        var scaler = rt.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        rt.gameObject.AddComponent<GraphicRaycaster>();
        return c;
    }

    internal static Image Image(RectTransform rt, Color color, bool frame = false)
    {
        var img = rt.GetComponent<Image>() ?? rt.gameObject.AddComponent<Image>();
        img.sprite = frame ? Frame : null;
        img.type = frame ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
        img.color = color; img.raycastTarget = false;
        return img;
    }

    internal static RectTransform PanelAt(string name, Transform parent, float x, float y, float w, float h)
    {
        var rt = Primitive(name, "Panel", parent); Box(rt, parent, x, y, w, h);
        Image(rt, Panel, true);
        return rt;
    }

    internal static TextMeshProUGUI Label(string name, string text, Transform parent,
        float x, float y, float w, float h, float size = 24)
    {
        var rt = Rect(name, parent); Box(rt, parent, x, y, w, h);
        var label = rt.gameObject.AddComponent<TextMeshProUGUI>();
        Style(label, size); label.text = text;
        return label;
    }

    internal static void Style(TMP_Text label, float size)
    {
        label.font = Font; label.fontSize = size; label.enableAutoSizing = false;
        label.fontStyle = FontStyles.Normal; label.color = Text;
        label.enableWordWrapping = false; label.overflowMode = TextOverflowModes.Ellipsis;
        label.alignment = TextAlignmentOptions.MidlineLeft; label.raycastTarget = false;
        label.margin = Vector4.zero;
    }

    internal static Button Button(string title, Transform parent, float x, float y, float w,
        UnityAction action)
    {
        var rt = Primitive(title + "Button", "Button", parent); Box(rt, parent, x, y, w, 40);
        var img = Image(rt, Edge, true); img.raycastTarget = true;
        var button = rt.GetComponent<Button>() ?? rt.gameObject.AddComponent<Button>(); button.targetGraphic = img;
        var colors = button.colors;
        colors.normalColor = Color.white; colors.highlightedColor = Teal;
        colors.selectedColor = Teal; colors.pressedColor = Muted;
        colors.disabledColor = new Color(.4f, .4f, .4f, .7f); colors.fadeDuration = 0;
        button.colors = colors;
        var label = rt.GetComponentInChildren<TextMeshProUGUI>();
        if (!label) label = Label("Label", title, rt, 8, 0, w - 16, 40, 28);
        else { label.text = title; Box(label.rectTransform, rt, 8, 0, w - 16, 40); }
        label.alignment = TextAlignmentOptions.Center;
        if (action != null) UnityEventTools.AddPersistentListener(button.onClick, action);
        return button;
    }

    internal static Slider Bar(string name, Transform parent, float x, float y, float w, Color tint)
    {
        var rt = Primitive(name, "StatBar", parent); Box(rt, parent, x, y, w, 12);
        Image(rt, Ink, true);
        var fill = rt.Find("Fill") as RectTransform;
        if (!fill) fill = Rect("Fill", rt);
        Stretch(fill, rt, 2); Image(fill, tint);
        var s = rt.GetComponent<Slider>() ?? rt.gameObject.AddComponent<Slider>(); s.fillRect = fill;
        s.direction = Slider.Direction.LeftToRight; s.minValue = 0; s.maxValue = 1;
        s.value = 1; s.interactable = false;
        s.transition = Selectable.Transition.None;
        return s;
    }

    internal static void KeyIcon(string name, Transform parent, float x, float y)
    {
        var rt = Rect(name, parent); Box(rt, parent, x, y, 24, 24);
        var img = Image(rt, Color.white);
        img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + name + ".png");
        img.preserveAspect = true;
    }

    internal static T Get<T>(UnityEngine.Object target, string name) where T : UnityEngine.Object
        => new SerializedObject(target).FindProperty(name).objectReferenceValue as T;

    internal static void Set(UnityEngine.Object target, string name, UnityEngine.Object value)
    {
        var so = new SerializedObject(target); so.FindProperty(name).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    internal static void Value(UnityEngine.Object target, string name, object value)
    {
        var so = new SerializedObject(target); var p = so.FindProperty(name);
        if (value is bool b) p.boolValue = b;
        else if (value is float f) p.floatValue = f;
        else if (value is int i) p.intValue = i;
        else if (value is Color c) p.colorValue = c;
        else if (value is string s) p.stringValue = s;
        else throw new ArgumentException(name);
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
