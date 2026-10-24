using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Exercise the real serialization entry point, not the retired lighting builder.
public static class SpriteDepthAuthoringChecks
{
    public static void Verify()
    {
        SpriteDepthAuthoring.SynchronizeSelectedWorlds();
        var library = AssetDatabase.LoadAssetAtPath<SpriteDepthLibrary>("Assets/Data/Lighting/ActorDepth.asset");
        SpriteDepthAuthoring.Validate(library);
        var contactSprites = SpriteDepthAuthoring.ReferencedSprites().Where(x =>
            AssetDatabase.GetAssetPath(x).Contains("PixelWeaponContacts")).ToArray();
        if (!library || contactSprites.Length == 0 || contactSprites.Any(x => !library.CreateLookup().ContainsKey(x)))
            throw new Exception("Depth inventory must cover gameplay parry contact cels");
        var definitionPath = "Assets/Data/Lighting/DungeonWorld.asset";
        var materialPath = "Assets/Art/Environment/KayKitDungeon/DungeonSprite.mat";
        var definition = AssetDatabase.LoadAssetAtPath<WorldDefinition>(definitionPath);
        if (definition.spriteDepth != library) throw new Exception("Production world must select authored depth");
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material.GetFloat("_SelfShadowStrength") != .65f || material.GetFloat("_DepthScale") != .15f ||
            material.GetFloat("_SelfShadowBias") != .01f) throw new Exception("Production depth material configuration changed");
        var results = Environment.GetEnvironmentVariable("TYPINGHERO_RESULTS_PATH");
        if (!string.IsNullOrEmpty(results))
            SpriteDepthAuthoring.ExportInventory(Path.Combine(results, "depth-inventory.json"));
        Debug.Log("Validated available depth maps; missing gameplay maps are reported by synchronization");
        var painted = Directory.GetFiles("Assets/Art/Lighting", "*.png", SearchOption.AllDirectories)
            .ToDictionary(path => path, Hash);
        var references = new[] { definitionPath, materialPath, AssetDatabase.GetAssetPath(library) }
            .ToDictionary(path => path, File.ReadAllText);
        PixelUIBuild.SerializeOnly(); // QA mode also checks the second full application.
        foreach (var image in painted)
            if (Hash(image.Key) != image.Value) throw new Exception("Builder changed painted map: " + image.Key);
        foreach (var reference in references)
            if (File.ReadAllText(reference.Key) != reference.Value)
                throw new Exception("Builder changed depth configuration/references: " + reference.Key);
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            var gate = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<DungeonSessionBindings>(true)).Single();
            var actors = gate.roots.SelectMany(x => x.GetComponentsInChildren<PlayerHealth>(true))
                .Cast<Component>().Concat(gate.roots.SelectMany(x => x.GetComponentsInChildren<SkeletonController>(true)))
                .ToArray();
            if (actors.Length != 2) throw new Exception("Expected the serialized hero and opponent");
            foreach (var actor in actors)
                if (actor.GetComponents<SpriteDepthBinding>().Length != 1)
                    throw new Exception(actor.name + ": gameplay builder must serialize exactly one depth binding");
            var worldScene = EditorSceneManager.OpenScene(gate.worldDefinition.scenePath, OpenSceneMode.Additive);
            var world = worldScene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<DungeonWorld>(true)).Single();
            // Reacquire editor assets after opening scenes.
            library = AssetDatabase.LoadAssetAtPath<SpriteDepthLibrary>("Assets/Data/Lighting/ActorDepth.asset");
            var block = new MaterialPropertyBlock();
            int ready = Shader.PropertyToID("_DepthReady");
            gate.Bind(world);
            foreach (var actor in actors)
            {
                actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                if (block.GetFloat(ready) != (world.definition.spriteDepth ? 1 : 0))
                    throw new Exception(actor.name + ": serialized scene did not bind its selected depth library");
            }
            gate.Bind(world, true);
            foreach (var actor in actors)
            {
                actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                if (block.GetFloat(ready) != 0) throw new Exception(actor.name + ": depth survived world clear");
            }
            // A -> B (without a map) -> A, preserving the authored production opt-in.
            var previous = world.definition.spriteDepth;
            try
            {
                world.definition.spriteDepth = library;
                gate.Bind(world);
                foreach (var actor in actors)
                {
                    actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                    if (block.GetFloat(ready) != 1)
                    {
                        var sprite = actor.GetComponent<SpriteRenderer>().sprite;
                        actor.GetComponent<SpriteDepthBinding>().Refresh(true);
                        actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                        float afterRefresh = block.GetFloat(ready);
                        actor.GetComponent<SpriteDepthBinding>().Bind(library.CreateLookup());
                        actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                        throw new Exception("Complete library failed to bind: " + actor.name + "/" +
                            (sprite ? sprite.name : "null sprite") + " lookup=" + (sprite && library.CreateLookup().ContainsKey(sprite)) +
                            " libraryAlive=" + (bool)library + " refresh=" + afterRefresh + " direct=" + block.GetFloat(ready));
                    }
                }
                gate.Bind(world, true);
                world.definition.spriteDepth = null;
                gate.Bind(world);
                foreach (var actor in actors)
                {
                    actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                    if (block.GetFloat(ready) != 0) throw new Exception("Unmapped world inherited depth");
                }
                gate.Bind(world, true);
                world.definition.spriteDepth = library;
                gate.Bind(world);
                foreach (var actor in actors)
                {
                    actor.GetComponent<SpriteRenderer>().GetPropertyBlock(block);
                    if (block.GetFloat(ready) != 1) throw new Exception("Depth did not restore on world return");
                }
            }
            finally { gate.Bind(world, true); world.definition.spriteDepth = previous; }
            Debug.Log("PASS: repeated gameplay builder, two serialized actor bindings, selected world bind and clear");
        }
        finally
        {
            if (setup.Any(x => x.isLoaded)) EditorSceneManager.RestoreSceneManagerSetup(setup);
            else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }
    static string Hash(string path)
    {
        using (var hash = SHA256.Create())
            return Convert.ToBase64String(hash.ComputeHash(File.ReadAllBytes(path)));
    }
}
