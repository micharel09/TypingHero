using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PixelUIFactory;

internal static class CoreMatchBuild
{
    internal static void Apply()
    {
        // Drop retired serialized typing guards/attack clock fields through Unity.
        foreach (var path in new[] { "Assets/Characters/Player/Player.prefab", "Assets/Characters/Enermy/Skeleton.prefab" })
        {
            var contents = PrefabUtility.LoadPrefabContents(path);
            try { PrefabUtility.SaveAsPrefabAsset(contents, path); }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        var menuRoot = GameObject.Find("PixelUI/Reference960x540").transform;
        var legacyButton = menuRoot.Find("How to PlayButton");
        if (legacyButton) Object.DestroyImmediate(legacyButton.gameObject);
        foreach (var canvas in Object.FindObjectsOfType<Canvas>(true))
            if (canvas.name == "HowToPlay") Object.DestroyImmediate(canvas.gameObject);
        foreach (var help in Object.FindObjectsOfType<HowToPlayPanel>(true)) Object.DestroyImmediate(help);
        if (!menuRoot.Find("AudioPreferences")) Volumes(menuRoot, 700, 220, 230);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        if (!GameObject.Find("PixelBackgroundCamera"))
        {
            var background = new GameObject("PixelBackgroundCamera").AddComponent<Camera>();
            background.clearFlags = CameraClearFlags.SolidColor;
            background.backgroundColor = Ink;
            background.cullingMask = 0;
            background.depth = Camera.main.depth - 1;
            background.orthographic = true;
        }
        var enemy = Object.FindObjectOfType<SkeletonController>(true);
        var player = Object.FindObjectOfType<PlayerHealth>(true);
        Value(enemy, "maxHealth", 850);
        foreach (var root in new[] { player.gameObject, enemy.gameObject })
        {
            var changes = PrefabUtility.GetPropertyModifications(root);
            if (changes == null) continue;
            var kept = changes.Where(change => !RetiredProperty(change)).ToArray();
            if (kept.Length != changes.Length) PrefabUtility.SetPropertyModifications(root, kept);
        }
        if (!Object.FindObjectOfType<CombatSession>(true))
        {
            Value(player, "maxHealth", 40);
            Value(enemy.GetComponent<AttackStrike>(), "damage", 5);
            var overlay = GameObject.Find("PixelOverlay/Reference960x540/CombatOverlay/OverlayPanel").GetComponent<RectTransform>();
            Box(overlay, overlay.parent, 120, 6, 448, 408);
            foreach (var title in new[] { "PauseLabel", "ThanksLabel" })
                Box(overlay.Find(title).GetComponent<RectTransform>(), overlay, 16, 12, 416, 56);
            var pause = Object.FindObjectOfType<GamePauseController>(true);
            Value(pause, "thanksText", "VICTORY");
            Box(overlay.Find("ResumeButton").GetComponent<RectTransform>(), overlay, 24, 140, 400, 40);
            Box(overlay.Find("RestartButton").GetComponent<RectTransform>(), overlay, 24, 190, 400, 40);
            Box(overlay.Find("Main MenuButton").GetComponent<RectTransform>(), overlay, 24, 240, 400, 40);
            Box(overlay.Find("KeyHint").GetComponent<RectTransform>(), overlay, 24, 374, 400, 24);
            var result = Label("MatchResult", "", overlay, 24, 70, 400, 60, 22);
            result.alignment = TextAlignmentOptions.Center;
            var session = new GameObject("CombatSession").AddComponent<CombatSession>();
            Set(session, "resultLabel", result);
            Volumes(overlay, 24, 292, 400);
            var cue = new GameObject("AttackCue", typeof(TextMeshPro));
            cue.transform.SetParent(enemy.transform, false);
            cue.transform.localPosition = new Vector3(0, 1.8f, 0);
            var text = cue.GetComponent<TextMeshPro>();
            text.font = Typeface; text.text = "!"; text.fontSize = 10;
            text.color = Gold; text.alignment = TextAlignmentOptions.Center;
            text.renderer.sortingOrder = 100;
            Set(enemy, "attackCue", cue);
            cue.SetActive(false);
        }
        var cueText = enemy.transform.Find("AttackCue").GetComponent<TextMeshPro>();
        cueText.rectTransform.sizeDelta = Vector2.one;
        cueText.rectTransform.anchoredPosition3D = new Vector3(0, 1.8f, 0);
        cueText.ForceMeshUpdate(true);
        EditorUtility.SetDirty(cueText.rectTransform);
        // Edit the actual controller clip with Unity, retaining its GUID and sprite curves.
        var animator = enemy.GetComponent<Animator>();
        var clip = animator.runtimeAnimatorController.animationClips.First(c => c.name == "skeleton_attack_heavy");
        var events = AnimationUtility.GetAnimationEvents(clip)
            .Where(e => e.functionName != "Anim_StrikeFrame" && e.functionName != "Anim_DealDamage" && e.functionName != "Anim_EnemyAttackSfx" && e.functionName != "Anim_Telegraph").ToList();
        events.Add(new AnimationEvent { time = .7f, functionName = "Anim_Telegraph" });
        events.Add(new AnimationEvent { time = 1f, functionName = "Anim_DealDamage" });
        AnimationUtility.SetAnimationEvents(clip, events.OrderBy(e => e.time).ToArray());
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static bool RetiredProperty(PropertyModification change)
    {
        return (change.target is SkeletonController &&
            (change.propertyPath == "useUnscaledClock" || change.propertyPath == "attackStartTime" || change.propertyPath == "attackCrossfade")) ||
            (change.target is PlayerSlayerMode &&
            (change.propertyPath == "inputDebounce" || change.propertyPath == "postWordCooldown"));
    }

    static void Volumes(Transform parent, float x, float y, float width)
    {
        var host = Rect("AudioPreferences", parent);
        Box(host, parent, x, y, width, 76);
        var preferences = host.gameObject.AddComponent<AudioPreferences>();
        foreach (var name in new[] { "Music", "SFX" })
        {
            float row = name == "Music" ? 0 : 38;
            Label(name + "Label", name + "  0–100%", host, 0, row, 160, 24, 20);
            var slider = Bar(name + "Volume", host, 162, row + 6, width - 162, Teal);
            slider.interactable = true;
            slider.GetComponent<Image>().raycastTarget = true;
            Set(preferences, name == "Music" ? "music" : "sfx", slider);
        }
    }
}
