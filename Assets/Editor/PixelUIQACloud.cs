using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
public static class PixelUIQACloud {
 public static void Build() {
  PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
  var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
   scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),
   locationPathName="build/WebGL/WebGL", target=BuildTarget.WebGL,
   extraScriptingDefines=new[]{"TYPINGHERO_AUTOMATION"}
  });
  if(report.summary.result!=BuildResult.Succeeded) throw new System.Exception("QA WebGL build failed");
 }
}
