using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

internal static class EndlessAssets
{
    const string Art = "Assets/Art/EndlessSkeletons/", Data = "Assets/Resources/Endless/";
    internal static void Prepare()
    {
        Directory.CreateDirectory(Data);
        AssetDatabase.Refresh();
        foreach (string form in new[] { "sword", "axe", "sword-shield" })
        {
            string folder = Art + form + "/";
            foreach (string action in new[] { "Idle", "Walk", "Attack1", "Attack2", "Hurt", "Die" })
                MakeClip(folder, action, form);
            string path = folder + "Combat.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (!controller)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path);
                controller.AddParameter("isDead", AnimatorControllerParameterType.Bool);
            }
            var machine = controller.layers[0].stateMachine;
            foreach (string stateName in new[] { "idle", "walk", "attack_heavy", "attack2", "hit", "stun", "hitBySlayer", "die" })
            {
                string action = stateName == "attack_heavy" ? "Attack1" : stateName == "attack2" ? "Attack2" :
                    stateName == "hit" || stateName == "stun" || stateName == "hitBySlayer" ? "Hurt" :
                    char.ToUpperInvariant(stateName[0]) + stateName.Substring(1);
                string name = "skeleton_" + stateName;
                var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == name) ?? machine.AddState(name);
                state.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(folder + action + ".anim");
                state.writeDefaultValues = true;
                if (stateName == "idle") machine.defaultState = state;
                if (stateName == "hit" && state.transitions.Length == 0)
                {
                    var transition = state.AddTransition(machine.defaultState);
                    transition.hasExitTime = true; transition.exitTime = 1; transition.duration = 0;
                }
            }
            EditorUtility.SetDirty(controller);
        }
        Role(EndlessRole.Swordsman, "SWORDSMAN", "Read each cue. From round 2, watch for thrusts.", "sword", 180, 5, 1.7f, 12, new[] { 1 }, new[] { 2 });
        Role(EndlessRole.DoubleStrike, "DOUBLE STRIKE", "Two attacks, two cues. Stay ready to parry again.", "sword", 200, 5, 1.7f, 12, new[] { 1, 2 }, new[] { 1, 2 });
        Role(EndlessRole.Heavy, "HEAVY", "Heavy axe, 8 damage. Parry, then type during the long recovery.", "axe", 240, 8, 2.5f, 12, new[] { 1 }, new[] { 1 });
        Role(EndlessRole.Berserker, "BERSERKER", "Three axe attacks. Parry every cue, punish the recovery.", "axe", 200, 5, 2.5f, 12, new[] { 2, 1, 2 }, new[] { 2, 1, 2 });
        Role(EndlessRole.Guard, "GUARD", "Typing deals full damage. Parry breaks shield stamina much faster.", "sword-shield", 200, 5, 1.7f, 4, new[] { 1 }, new[] { 2 });
        Role(EndlessRole.Captain, "CAPTAIN · BOSS", "Singles, doubles, then triples as HP falls. Win to heal 10 HP.", "sword-shield", 550, 5, 1.7f, 12, new[] { 1 }, new[] { 1, 2 });
        AssetDatabase.SaveAssets();
    }

    static void Role(EndlessRole role, string name, string hint, string form, int hp, int damage, float recovery, int hitCost, int[] opening, int[] alternate)
    {
        string path = Data + role + ".asset";
        var asset = AssetDatabase.LoadAssetAtPath<EndlessOpponent>(path);
        if (!asset) { asset = ScriptableObject.CreateInstance<EndlessOpponent>(); AssetDatabase.CreateAsset(asset, path); }
        asset.role = role; asset.displayName = name; asset.hint = hint; asset.health = hp;
        asset.damage = damage; asset.recovery = recovery; asset.hitCost = hitCost;
        asset.opening = opening; asset.alternate = alternate;
        asset.controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Art + form + "/Combat.controller");
        asset.hurt = AssetDatabase.LoadAssetAtPath<AnimationClip>(Art + form + "/Hurt.anim");
        asset.idle = AssetDatabase.LoadAllAssetsAtPath(Art + form + "/Idle.png").OfType<Sprite>().OrderBy(s => s.rect.x).First();
        EditorUtility.SetDirty(asset);
    }

    static void MakeClip(string folder, string action, string form)
    {
        string path = folder + action + ".png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16; importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false; importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.alphaIsTransparency = true;
        foreach (string platform in new[] { "WebGL", "Standalone" })
        {
            var setting = importer.GetPlatformTextureSettings(platform);
            setting.overridden = true; setting.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(setting);
        }
        importer.GetSourceTextureWidthAndHeight(out int width, out _);
        int count = width / 96;
        importer.spritesheet = Enumerable.Range(0, count).Select(i => new SpriteMetaData {
            name = action + "_" + i, rect = new Rect(i * 96, 0, 96, 64), alignment = 9, pivot = new Vector2(.47f, .41f)
        }).ToArray();
        importer.SaveAndReimport();
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
        string clipPath = folder + action + ".anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (!clip) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
        clip.frameRate = 10;
        var frames = sprites.Select((s, i) => new ObjectReferenceKeyframe { time = i * .1f, value = s }).ToList();
        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), frames.ToArray());
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0; settings.stopTime = count * .1f;
        settings.loopTime = action == "Idle" || action == "Walk";
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        var events = new System.Collections.Generic.List<AnimationEvent>();
        if (action.StartsWith("Attack"))
        {
            int contact = form == "axe" ? (action == "Attack1" ? 6 : 4) : action == "Attack1" ? 5 : 2;
            // Early thrust cue/hold is scheduled by SkeletonController before playback.
            if (contact >= 3) events.Add(new AnimationEvent { time = contact * .1f - .3f, functionName = "Anim_Telegraph" });
            events.Add(new AnimationEvent { time = contact * .1f, functionName = "Anim_DealDamage" });
        }
        if (action == "Die") events.Add(new AnimationEvent { time = count * .1f, functionName = "Anim_DieCleanup" });
        AnimationUtility.SetAnimationEvents(clip, events.ToArray());
        EditorUtility.SetDirty(clip);
    }
}
