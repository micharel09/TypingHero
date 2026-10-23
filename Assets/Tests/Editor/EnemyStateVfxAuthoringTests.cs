using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class EnemyStateVfxAuthoringTests
{
    [Test]
    public void BuilderIsIdempotentAndSceneBindsIndependentDeathOwnerAndSceneFloor()
    {
        EnemyStateVfxBuild.SerializeOnly();
        Assert.That(File.ReadAllBytes(EnemyStateVfxBuild.Root + "GuardBreak-Sheet.png"),
            Is.EqualTo(File.ReadAllBytes("Assets/Art/VFX/Pimen/BattleEffects/Effect 2 - Sprite Sheet.png")), "No source pixels are generated or edited");
        var paths = Directory.GetFiles(EnemyStateVfxBuild.Root, "*", SearchOption.AllDirectories)
            .Concat(new[] { "Assets/Scenes/SampleScene.unity", "Assets/Characters/Enermy/Skeleton.prefab", "Assets/Scripts/skeleton_die.anim" })
            .Concat(new[] { "sword", "sword-shield", "axe" }.Select(f => "Assets/Art/EndlessSkeletons/" + f + "/Die.anim")).ToArray();
        var before = paths.Select(File.ReadAllBytes).ToArray();
        EnemyStateVfxBuild.SerializeOnly();
        for (int i = 0; i < paths.Length; i++) Assert.That(File.ReadAllBytes(paths[i]), Is.EqualTo(before[i]), paths[i]);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            var actors = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<EnemyStateVfxBinder>(true)).ToArray();
            Assert.That(actors.Length, Is.EqualTo(1));
            foreach (var binder in actors)
            {
                var so = new SerializedObject(binder);
                foreach (var field in new[] { "health", "stamina", "stun", "body", "deathAnchor", "guardEmitter", "deathEmitter", "guardBreak", "bodyFallDust" })
                    Assert.That(so.FindProperty(field).objectReferenceValue, Is.Not.Null, field);
                var emitter = (ImpactVfxEmitter)so.FindProperty("deathEmitter").objectReferenceValue;
                Assert.That(emitter.transform.IsChildOf(binder.transform), Is.False);
                Assert.That(new SerializedObject(emitter).FindProperty("maxActive").intValue, Is.EqualTo(2));
                var body = binder.GetComponent<BoxCollider2D>();
                var floor = body.transform.TransformPoint(body.offset).y - body.size.y * Mathf.Abs(body.transform.lossyScale.y) * .5f;
                Assert.That(so.FindProperty("floorY").floatValue, Is.EqualTo(floor).Within(.00001));
                var gate = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<DungeonSessionBindings>(true)).Single();
                Assert.That(gate.roots.Contains(emitter.gameObject), Is.True, "Session teardown owns the emitter root");
            }
        }
        finally { if (setup.Any(s => s.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }
}
