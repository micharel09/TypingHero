using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

internal static class PlayerParryAssets
{
    const string Source = "Assets/Art/male_hero/individual_sheets/";

    [MenuItem("CLACKBLADE/Apply Original Player Parry")]
    internal static void Apply()
    {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(Source + "male_hero-combo_3_end.png")
            .OfType<Sprite>().ToDictionary(sprite => sprite.name);
        var frames = Enumerable.Range(2, 4).Select(i => sprites["male_hero-combo_3_end_" + i]).ToArray();
        Clip("Assets/Characters/Player/player_parry.anim", frames,
            new[] { 0f, .25f, 19f / 60f, 23f / 60f }, .4f, true);
        const string liftPath = Source + "male_hero-parry_lift.png";
        SurfaceNormalAssets.Prepare(liftPath);
        var combo = AssetDatabase.LoadAllAssetsAtPath(Source + "male_hero-combo_3.png")
            .OfType<Sprite>().ToDictionary(sprite => sprite.name);
        var end = AssetDatabase.LoadAllAssetsAtPath(Source + "male_hero-combo_2_end.png")
            .OfType<Sprite>().OrderBy(sprite => sprite.name).ToArray();
        var success = new[] { combo["male_hero-combo_3_1"], AssetDatabase.LoadAssetAtPath<Sprite>(liftPath),
            combo["male_hero-combo_3_0"] }.Concat(end).ToArray();
        // Preserve the 800ms follow-up window and 950ms lock; only the art changes.
        var successTimes = new[] { 0f, .8f, 50f / 60f, 52f / 60f, 54f / 60f, 55f / 60f, 56f / 60f };
        Clip(Source + "player_parry_success.anim", success, successTimes, .95f, false);
        Clip("Assets/Characters/Player/player_parry_success.anim", success, successTimes, .95f, false);

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        FollowUp(controller);
        foreach (var child in controller.layers[0].stateMachine.states)
            if (child.state.name == "player_parry" || child.state.name == "player_parry_success")
                foreach (var transition in child.state.transitions)
                {
                    if (!transition.destinationState || transition.destinationState.name != "player_idle") continue;
                    transition.exitTime = 1f;
                    transition.duration = 0f;
                    EditorUtility.SetDirty(transition);
                }
        // The main-branch contact snapshot must hold the same original entry cel.
        // Preserve enemy contact cels and the physical spark geometry pipeline.
        var contacts = ParryPixelAssets.Contacts();
        var defense = contacts.frames.Single(frame => frame.defense && frame.sprite == frames[0]);
        contacts.frames = new[] { defense }.Concat(contacts.frames.Where(frame => !frame.defense)
            .Select(frame => { frame.defenseCel = frames[0]; return frame; })).ToArray();
        EditorUtility.SetDirty(contacts);
        AssetDatabase.SaveAssets();
    }

    static void FollowUp(AnimatorController controller)
    {
        const string clipPath = "Assets/Characters/Player/parry_followup.anim";
        const string configPath = "Assets/Data/Attack/AC_ParryFollowUp.asset";
        var frames = new[] { "combo_2", "combo_2_end" }.SelectMany(sheet =>
            AssetDatabase.LoadAllAssetsAtPath(Source + "male_hero-" + sheet + ".png")
                .OfType<Sprite>().OrderBy(sprite => sprite.name)).ToArray();
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (!clip) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
        clip.frameRate = 60;
        const float stretch = 34f / 31f; // Match the normal attack duration with the retained nine cels.
        AnimationUtility.SetObjectReferenceCurve(clip,
            EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"),
            frames.Skip(1).Select((sprite, i) => new ObjectReferenceKeyframe
            { time = (i < 5 ? i * .05f : .25f + (i - 5) / 15f) * stretch, value = sprite })
            // Unity derives length from the last key plus one sample. Keep the
            // final cel held to frame34 without introducing another pose.
            .Concat(new[] { new ObjectReferenceKeyframe { time = 33f / 60f, value = frames[9] } }).ToArray());
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0; settings.stopTime = 34f / 60f; settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        AnimationUtility.SetAnimationEvents(clip, new[] {
            new AnimationEvent { time = .1f * stretch, functionName = "OnAttackStartCel", objectReferenceParameter = frames[3] },
            new AnimationEvent { time = .15f * stretch, functionName = "OnAttackContact", objectReferenceParameter = frames[4] },
            new AnimationEvent { time = .2f * stretch, functionName = "OnParryFollowUpEnd", objectReferenceParameter = frames[3] }
        });
        EditorUtility.SetDirty(clip);
        var machine = controller.layers[0].stateMachine;
        var state = machine.states.Select(child => child.state).SingleOrDefault(s => s.name == "parry_followup")
            ?? machine.AddState("parry_followup");
        state.motion = clip; state.tag = "Attack"; state.speed = 1;
        var exit = state.behaviours.OfType<ParryFollowUpExit>().SingleOrDefault() ?? state.AddStateMachineBehaviour<ParryFollowUpExit>();
        exit.openingCel = frames[3];
        EditorUtility.SetDirty(exit);
        var idle = machine.states.Single(child => child.state.name == "player_idle").state;
        const string queuedParameter = "ParryFollowUpQueued";
        if (!controller.parameters.Any(p => p.name == queuedParameter))
            controller.AddParameter(queuedParameter, AnimatorControllerParameterType.Bool);
        var success = machine.states.Single(child => child.state.name == "player_parry_success").state;
        var direct = success.transitions.SingleOrDefault(t => t.destinationState == state) ?? success.AddTransition(state);
        direct.hasExitTime = true; direct.exitTime = 1; direct.duration = .02f; direct.hasFixedDuration = true;
        if (!direct.conditions.Any(c => c.parameter == queuedParameter))
            direct.AddCondition(AnimatorConditionMode.If, 0, queuedParameter);
        var noFollowUp = success.transitions.Single(t => t.destinationState == idle);
        if (!noFollowUp.conditions.Any(c => c.parameter == queuedParameter))
            noFollowUp.AddCondition(AnimatorConditionMode.IfNot, 0, queuedParameter);
        var transition = state.transitions.SingleOrDefault(t => t.destinationState == idle) ?? state.AddTransition(idle);
        transition.hasExitTime = true; transition.exitTime = 1; transition.duration = 0;
        EditorUtility.SetDirty(controller);
        var config = AssetDatabase.LoadAssetAtPath<AttackConfig>(configPath);
        if (!config) { config = ScriptableObject.CreateInstance<AttackConfig>(); AssetDatabase.CreateAsset(config, configPath); }
        config.statePath = "Base Layer.parry_followup"; config.crossfade = .02f; config.startTime = 0;
        EditorUtility.SetDirty(config);
        const string prefabPath = "Assets/Characters/Player/Player.prefab";
        var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            foreach (var player in prefab.GetComponentsInChildren<PlayerController>(true)) player.parryFollowUp = config;
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        }
        finally { PrefabUtility.UnloadPrefabContents(prefab); }
    }

    static void Clip(string path, Sprite[] sprites, float[] times, float end, bool opensWindow)
    {
        // Update the existing objects: their GUIDs, controller states and original sprite imports survive.
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, sprites.Select((sprite, i) =>
            new ObjectReferenceKeyframe { time = times[i], value = sprite }).ToArray());
        clip.frameRate = 60;
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0; settings.stopTime = end; settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        var events = new System.Collections.Generic.List<AnimationEvent>();
        if (opensWindow)
        {
            events.Add(new AnimationEvent { time = 0, functionName = "Anim_ParryOpen" });
            events.Add(new AnimationEvent { time = 0, functionName = "Anim_ParryPoseSfx" });
            events.Add(new AnimationEvent { time = .25f, functionName = "Anim_ParryClose" });
        }
        events.Add(new AnimationEvent { time = end, functionName = "Anim_ParryEnd", intParameter = opensWindow ? 0 : 1 });
        AnimationUtility.SetAnimationEvents(clip, events.ToArray());
        EditorUtility.SetDirty(clip);
    }
}
