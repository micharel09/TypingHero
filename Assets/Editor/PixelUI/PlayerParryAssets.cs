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
        Clip("Assets/Characters/Player/player_parry.anim", frames, .25f, .4f, true);
        Clip(Source + "player_parry_success.anim", frames, .8f, .95f, false);
        Clip("Assets/Characters/Player/player_parry_success.anim", frames, .8f, .95f, false);

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        foreach (var child in controller.layers[0].stateMachine.states)
            if (child.state.name == "player_parry" || child.state.name == "player_parry_success")
                foreach (var transition in child.state.transitions)
                {
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

    static void Clip(string path, Sprite[] sprites, float hold, float end, bool opensWindow)
    {
        // Update the existing objects: their GUIDs, controller states and original sprite imports survive.
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var times = new[] { 0f, hold, hold + 4f / 60f, end - 1f / 60f };
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
