using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Use the existing cloud Unity builder. Migration also runs from the editor menu.
public sealed class PixelUIBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        try { Apply(); }
        catch (System.Exception error) { throw new BuildFailedException(error); }
        if (System.Environment.GetEnvironmentVariable("TYPINGHERO_QA") != "1") return;
        var paths = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity",
            "Assets/Data/Score/ScoreRules.asset",
            "Assets/Characters/Player/player_parry.anim", "Assets/Characters/Player/player_parry_success.anim",
            "Assets/Art/male_hero/individual_sheets/player_parry_success.anim", "Assets/Characters/Player/AC_Controller.controller",
            "Assets/Characters/Player/parry_followup.anim", "Assets/Data/Attack/AC_ParryFollowUp.asset",
            PixelUIFactory.Generated + "Panel.prefab", PixelUIFactory.Generated + "Button.prefab",
            PixelUIFactory.Generated + "StatBar.prefab", PixelUIFactory.Generated + "LeaderboardRow.prefab",
            "Assets/Characters/Player/Player.prefab", "Assets/Characters/Enermy/Skeleton.prefab",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_normal.asset",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_slayer.asset",
            "Assets/Audio/SFX/Events/Combat/Player/ev_impact_lethal.asset",
            "Assets/Audio/SFX/Events/Combat/Player/Parry/ev_parry_clang.asset" };
        paths = paths.Concat(Directory.GetFiles(CombatFeedbackAssets.Generated, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Art/VFX/CodeManu", "*"))
            .Concat(Directory.GetFiles(PixelSparkAssets.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(ParryClashAssets.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(CombatVoxelAssets.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Art/VFX/SlashVolume", "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(WorldLightingBuild.Root, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(DungeonArenaBuild.Art, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(DungeonPixelRoomBuild.Art, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(DungeonHangingLanternBuild.Art, "*", SearchOption.AllDirectories))
            .Concat(new[] { "Assets/Settings/UniversalRP.asset", "Assets/Settings/DungeonForward.asset", "Assets/Settings/DungeonPresentation.asset" }).ToArray();
        paths = paths.Concat(Directory.GetFiles(ShogunEnvironment.Art, "*"))
            .Concat(Directory.GetFiles(CombatR7Build.Art, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles(DungeonUISkin.Art, "*"))
            .Concat(Directory.GetFiles(BlackletterTitleBuild.Art, "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Art/EndlessSkeletons", "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Resources/Endless", "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Resources/Tutorial", "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Art/UI/TutorialParry", "*", SearchOption.AllDirectories))
            .Concat(Directory.GetFiles("Assets/Audio/SFX/Clips/KenneyUI", "*"))
            .Concat(Directory.GetFiles("Assets/Audio/SFX/Clips/KenneyInterface", "*"))
            .Concat(Directory.GetFiles(ShogunTheme.Events, "*")).ToArray();
        paths = paths.Concat(AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)).Distinct().ToArray();
        paths = paths.Concat(new[] { DungeonSessionBuild.WorldPath, DungeonSessionBuild.WorldPath + ".meta", "ProjectSettings/EditorBuildSettings.asset" }).ToArray();
        paths = paths.Concat(Directory.GetFiles("Assets/Data/Lighting", "*", SearchOption.AllDirectories)).ToArray();
        paths = paths.Concat(LightingBuildValidation.Dependencies())
            .Concat(LightingBuildValidation.Dependencies().Select(x => x + ".meta").Where(File.Exists)).Distinct().ToArray();
        var before = System.Array.ConvertAll(paths, File.ReadAllBytes);
        try { Apply(); }
        catch (System.Exception error) { throw new BuildFailedException(error); }
        for (int i = 0; i < paths.Length; i++)
            if (!before[i].SequenceEqual(File.ReadAllBytes(paths[i])))
            {
                if (paths[i].EndsWith(".unity") || paths[i].EndsWith(".prefab") || paths[i].EndsWith(".mat"))
                {
                    File.WriteAllBytes("/results/" + Path.GetFileNameWithoutExtension(paths[i]) + "-first" + Path.GetExtension(paths[i]), before[i]);
                    File.Copy(paths[i], "/results/" + Path.GetFileNameWithoutExtension(paths[i]) + "-second" + Path.GetExtension(paths[i]), true);
                }
                throw new BuildFailedException("UI builder is not idempotent: " + paths[i]);
            }
        Debug.Log("Shogun repeat builder: serialized objects, references and listeners unchanged");
    }

    public static void SerializeOnly()
    {
        new PixelUIBuild().OnPreprocessBuild(null);
        Debug.Log("Build succeeded!");
    }

    [MenuItem("CLACKBLADE/Apply Pixel UI")]
    public static void Apply()
    {
        var previousSetup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            var cameraPose = DungeonSessionBuild.RestoreForBuilders();
            var originalTyping = BlackletterTitleBuild.SnapshotTypingComponents();
            PixelUIAssets.Prepare();
            PixelUIAssets.SavePrimitives();
            PixelUIScenes.Menu();
            PixelUIScenes.Game();
            CoreMatchBuild.Apply();
            EndlessAssets.Prepare();
            CombatFeedbackBuild.Apply();
            PlayerParryAssets.Apply();
            TutorialBuild.Apply();
            ShogunTheme.Apply();
            CombatR7Build.Apply();
            DungeonUISkin.Apply();
            BlackletterTitleBuild.Apply();
            DungeonArenaBuild.Apply(cameraPose);
            CombatR7Build.ApplyResourceInk();
            BlackletterTitleBuild.AssertTypingComponents(originalTyping);
            TutorialBuild.Guide();
            ParryCueBuild.Apply();
            CombatR7Build.BindResultHud();
            CombatCameraImpactBuild.Apply();
            DungeonSessionBuild.Apply();
            LightingBuildValidation.ApplySelected();
            AssetDatabase.SaveAssets();
            // Return the actual Unity-serialized assets alongside the WebGL artifact.
            Directory.CreateDirectory("build/PixelUI");
            AssetDatabase.ExportPackage(new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SampleScene.unity", DungeonSessionBuild.WorldPath,
                PixelUIFactory.Generated.TrimEnd('/'), PixelUIFactory.Art.TrimEnd('/'),
                CombatR7Build.Art.TrimEnd('/'), DungeonUISkin.Art.TrimEnd('/'), BlackletterTitleBuild.Art.TrimEnd('/'),
                DungeonArenaBuild.Art.TrimEnd('/'), DungeonPixelRoomBuild.Art.TrimEnd('/'), DungeonHangingLanternBuild.Art.TrimEnd('/'), "Assets/Scripts/Rendering", "Assets/Settings", "Assets/Data/Lighting",
                ShogunEnvironment.Art.TrimEnd('/'), "Assets/Audio/SFX/Clips/KenneyUI", ShogunTheme.Events.TrimEnd('/') }.Concat(LightingBuildValidation.Dependencies()).Distinct().ToArray(), "build/PixelUI/PixelUI.unitypackage",
                ExportPackageOptions.Recurse);
            Debug.Log("Pixel UI migration and serialization complete");
        }
        finally
        {
            if (previousSetup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
}
