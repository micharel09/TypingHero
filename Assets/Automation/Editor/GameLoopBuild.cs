using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class GameLoopBuild
{
    public static void Windows64()
    {
        var output = Environment.GetEnvironmentVariable("TYPINGHERO_BUILD_PATH");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("Missing build output");
        var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone);
        var width = PlayerSettings.defaultScreenWidth;
        var height = PlayerSettings.defaultScreenHeight;
        var native = PlayerSettings.defaultIsNativeResolution;
        var mode = PlayerSettings.fullScreenMode;
        var resize = PlayerSettings.resizableWindow;
        try
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = output + "/TypingHero.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
                extraScriptingDefines = Array.Empty<string>()
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            CopyCredits(output);
            System.IO.File.WriteAllText(output + "/game-loop-build.json", "{\"qa\":false,\"target\":\"Windows64\"}");
            System.IO.File.WriteAllText(output + "/README.txt",
                "TypingHero - Windows 64-bit\r\n\r\nExtract the entire ZIP, then open TypingHero.exe.\r\n" +
                "Keep the Data folder, MonoBleedingEdge and DLL files beside the executable.\r\n" +
                "No Python, browser or Unity installation required.\r\nAlt+F4 closes the game.\r\n");
            Debug.Log("TypingHero Windows64 build complete; QA=false");
        }
        finally
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, backend);
            PlayerSettings.defaultScreenWidth = width;
            PlayerSettings.defaultScreenHeight = height;
            PlayerSettings.defaultIsNativeResolution = native;
            PlayerSettings.fullScreenMode = mode;
            PlayerSettings.resizableWindow = resize;
            AssetDatabase.SaveAssets();
        }
    }

    public static void WebGL()
    {
        bool qa = Environment.GetEnvironmentVariable("TYPINGHERO_QA") == "1";
        var output = Environment.GetEnvironmentVariable("TYPINGHERO_BUILD_PATH");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("Missing build output");
        var compression = PlayerSettings.WebGL.compressionFormat;
        try
        {
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None,
                extraScriptingDefines = qa ? new[] { "TYPINGHERO_AUTOMATION" } : Array.Empty<string>()
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("WebGL build failed: " + report.summary.result);
            // Production output is additionally scanned by the runner for the JS bridge.
            // The unreferenced native function is stripped when the C# QA probe is excluded.
            System.IO.File.WriteAllText(output + "/game-loop-build.json",
                "{\"qa\":" + (qa ? "true" : "false") + "}");
            CopyCredits(output);
            Debug.Log("TypingHero WebGL build complete; QA=" + qa);
        }
        finally { PlayerSettings.WebGL.compressionFormat = compression; }
    }

    static void CopyCredits(string output)
    {
        System.IO.File.Copy("Assets/Art/Environment/TinyPixelJapan/CREDITS.txt", output + "/CREDITS.txt", true);
        System.IO.File.Copy("Assets/Art/Environment/TinyPixelJapan/LICENSE.txt", output + "/TinyPixelJapan-License.txt", true);
        System.IO.File.Copy("Assets/Art/UI/DarkAges/LICENSE.txt", output + "/DarkAges-License.txt", true);
        foreach (var license in new[] { "Crusenho-License.txt", "Kenney-License.txt", "VT323-OFL.txt" })
            System.IO.File.Copy("Assets/Art/UI/Pixel/" + license, output + "/" + license, true);
        System.IO.File.AppendAllText(output + "/CREDITS.txt", "\n" +
            System.IO.File.ReadAllText("Assets/Audio/SFX/Clips/KenneyImpact/CREDITS.txt"));
        System.IO.File.Copy("Assets/Audio/SFX/Clips/KenneyImpact/License.txt", output + "/Kenney-Impact-License.txt", true);
        System.IO.File.AppendAllText(output + "/CREDITS.txt", "\n" + System.IO.File.ReadAllText("Assets/Audio/SFX/Clips/KenneyUI/CREDITS.txt"));
        System.IO.File.Copy("Assets/Audio/SFX/Clips/KenneyUI/License.txt", output + "/Kenney-UI-License.txt", true);
        System.IO.File.AppendAllText(output + "/CREDITS.txt", "\n" + System.IO.File.ReadAllText("Assets/Art/VFX/Pimen/CREDITS.txt"));
        System.IO.File.AppendAllText(output + "/CREDITS.txt", "\n" + System.IO.File.ReadAllText("Assets/Art/VFX/CodeManu/CREDITS.txt"));
        System.IO.File.Copy("Assets/Art/VFX/CodeManu/LICENSE.txt", output + "/CodeManu-License.txt", true);
        foreach (var license in new[] { "HitSpark-License.txt", "BattleEffects-License.txt" })
            System.IO.File.Copy("Assets/Art/VFX/Pimen/" + license, output + "/Pimen-" + license, true);
    }
}
