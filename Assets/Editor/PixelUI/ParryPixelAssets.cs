using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Derivatives of the licensed, free Pimen Hit Effect 01. Never rewrite its source/importer.
internal static class ParryPixelAssets
{
    const string Root = ParryClashAssets.Root;
    const string Source = "Assets/Art/VFX/Pimen/HitSpark/Hit Effect 01/Hit Effect 01 1.png";
    internal static bool Migrate(SerializedObject burst)
    {
        if (burst.FindProperty("pixelVersion").intValue >= 1) return false;
        var original = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        original.LoadImage(File.ReadAllBytes(Source));
        try
        {
            Texture("PixelContact", 96, 16, (x, y) => {
                int cell = x / 16, px = x % 16;
                return cell == 0 ? original.GetPixel(16 + px, 16 + y) : original.GetPixel(cell * 48 + px * 3 + 1, y * 3 + 1);
            }, true);
            // A thin row and hot pixel from cel0; rotate the row to the velocity-aligned Y axis.
            Texture("PixelRay", 1, 8, (x, y) => y == 0 || y == 7 ? Color.clear : original.GetPixel(22 + y - 1, 24), false);
            Texture("PixelEmber", 1, 1, (x, y) => original.GetPixel(24, 24), false);
        }
        finally { Object.DestroyImmediate(original); }
        var sprites = AssetDatabase.LoadAllAssetsAtPath(Root + "PixelContact.png").OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
        burst.FindProperty("pixelCore").objectReferenceValue = sprites[0];
        var frames = burst.FindProperty("pixelBurst"); frames.arraySize = 5;
        for (int i = 0; i < 5; i++) frames.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i + 1];
        burst.FindProperty("pixelContactMaterial").objectReferenceValue = Material("PixelContact", 8);
        burst.FindProperty("pixelRayMaterial").objectReferenceValue = Material("PixelRay", 3);
        burst.FindProperty("pixelEmberMaterial").objectReferenceValue = Material("PixelEmber", 2);
        burst.FindProperty("pixelVersion").intValue = 1;
        burst.ApplyModifiedPropertiesWithoutUndo();
        string receipt = Root + "PixelSource.txt";
        if (!File.Exists(receipt)) File.WriteAllText(receipt,
            "Pimen Hit Effect 01: https://pimen.itch.io/battle-vfx-hit-spark\n" +
            "Source: " + Source + "\n" +
            "Free original01 only. See ../Pimen/HitSpark-License.txt and CREDITS.txt.\n" +
            "Contact: core cel0 central16x16; burst cels1-5 nearest48->16, fixed pivot.\n" +
            "Ray: cel0 row23 x22..27, rotated; ember: cel0 pixel(24,23).\n");
        return true;
    }
    static void Texture(string name, int width, int height, System.Func<int, int, Color> pixel, bool sprites)
    {
        string path = Root + name + ".png";
        if (File.Exists(path)) return;
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        try
        {
            for (int y = 0; y < height; y++) for (int x = 0; x < width; x++) texture.SetPixel(x, y, pixel(x, y));
            texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG());
        }
        finally { Object.DestroyImmediate(texture); }
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = sprites ? TextureImporterType.Sprite : TextureImporterType.Default;
        importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false; importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaIsTransparency = true;
        if (sprites)
        {
            importer.spriteImportMode = SpriteImportMode.Multiple; importer.spritePixelsPerUnit = 16;
            importer.spritesheet = Enumerable.Range(0, 6).Select(i => new SpriteMetaData {
                name = "PimenContact_" + i, rect = new Rect(i * 16, 0, 16, 16), pivot = Vector2.one * .5f,
                alignment = (int)SpriteAlignment.Center }).ToArray();
        }
        importer.SaveAndReimport();
    }
    static Material Material(string name, float hdr)
    {
        string path = Root + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + name + ".png");
        material.SetTexture("_BaseMap", texture); material.SetTexture("_MainTex", texture);
        material.SetColor("_BaseColor", Color.white * hdr); material.SetColor("_Color", Color.white * hdr);
        // Preserve alpha=1 when authoring the HDR RGB multiplier.
        var tint = new Color(hdr, hdr, hdr, 1); material.SetColor("_BaseColor", tint); material.SetColor("_Color", tint);
        material.SetFloat("_Surface", 1); material.SetFloat("_Blend", 2); material.SetFloat("_ColorMode", 0);
        material.SetFloat("_SrcBlend", (int)BlendMode.SrcAlpha); material.SetFloat("_DstBlend", (int)BlendMode.One);
        material.SetFloat("_ZWrite", 0); material.SetFloat("_Cull", (int)CullMode.Off);
        material.SetFloat("_SoftParticlesEnabled", 0); material.SetFloat("_DistortionEnabled", 0);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); material.SetOverrideTag("RenderType", "Transparent");
        material.renderQueue = (int)RenderQueue.Transparent;
        AssetDatabase.CreateAsset(material, path); return material;
    }
    internal static ParryContactLibrary Contacts()
    {
        string path = Root + "ParryContacts.asset";
        var library = AssetDatabase.LoadAssetAtPath<ParryContactLibrary>(path);
        if (library && library.version >= 4) return library;
        var clean = CleanWeaponCels();
        bool create = !library;
        if (create) library = ScriptableObject.CreateInstance<ParryContactLibrary>();
        var frames = new List<ParryContactLibrary.Frame>();
        void Add(string texture, int index, Vector2 heel, Vector2 tip, Vector2 incoming, bool defense = false, int cleanIndex = -1, int defenseIndex = -1)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(texture).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
            if (index >= sprites.Length) throw new System.InvalidOperationException("Missing parry contact cel: " + texture + ":" + index);
            float height = sprites[index].rect.height;
            frames.Add(new ParryContactLibrary.Frame { sprite = sprites[index], contactCel = cleanIndex < 0 ? sprites[index] : clean[cleanIndex],
                defenseCel = defenseIndex < 0 ? null : frames[defenseIndex].sprite, heel = new Vector2(heel.x, height - heel.y),
                tip = new Vector2(tip.x, height - tip.y), incoming = new Vector2(incoming.x, -incoming.y), defense = defense });
        }
        const string player = "Assets/Art/male_hero/individual_sheets/male_hero-combo_3_end.png";
        Vector2[] heels = { new Vector2(79,45), new Vector2(79,47), new Vector2(78,62), new Vector2(77,63), new Vector2(76,63), new Vector2(76,63) };
        Vector2[] tips = { new Vector2(75,29), new Vector2(74,33), new Vector2(80,39), new Vector2(81,40), new Vector2(81,40), new Vector2(81,40) };
        for (int i = 0; i < 6; i++) Add(player, i, heels[i], tips[i], Vector2.zero, true);
        const string tutorial = "Assets/Art/Skeletons_Free_Pack/Skeleton_Sword/Skeleton_White/Skeleton_With_VFX/Skeleton_01_White_Attack1.png";
        for (int i = 4; i <= 6; i++) Add(tutorial, i, new Vector2(71,29), new Vector2(90,25), new Vector2(60,-4), false, 0);
        // Contact and its adjacent held cels share an authored strike direction from the preceding weapon pose.
        foreach (string form in new[] { "sword", "sword-shield", "axe" })
        foreach (int attack in new[] { 1, 2 })
        {
            bool axe = form == "axe"; int contact = axe ? (attack == 1 ? 6 : 4) : attack == 1 ? 5 : 2;
            Vector2 heel = axe ? (attack == 1 ? new Vector2(78,11) : new Vector2(80,34)) : new Vector2(attack == 1 ? 71 : 69, attack == 1 ? 29 : 32);
            Vector2 tip = axe ? (attack == 1 ? new Vector2(83,6) : new Vector2(89,29)) : new Vector2(90, attack == 1 ? 25 : 32);
            Vector2 incoming = axe ? (attack == 1 ? new Vector2(12,-40) : new Vector2(65,8)) : attack == 1 ? new Vector2(60,-4) : new Vector2(18,-8);
            if (form == "sword-shield")
            {
                heel = attack == 1 ? new Vector2(66,27) : new Vector2(69,30);
                tip = attack == 1 ? new Vector2(86,23) : new Vector2(84,30);
            }
            if (axe)
            {
                heel = attack == 1 ? new Vector2(77,12) : new Vector2(77,28);
                tip = attack == 1 ? new Vector2(80,7) : new Vector2(84,24);
            }
            int cel = (form == "sword" ? 0 : form == "sword-shield" ? 2 : 4) + attack - 1;
            for (int i = contact - 1; i <= contact + 1; i++) Add("Assets/Art/EndlessSkeletons/" + form + "/Attack" + attack + ".png", i, heel, tip, incoming, false, cel, attack == 2 ? 1 : 0);
        }
        library.frames = frames.ToArray(); library.version = 4;
        if (create) AssetDatabase.CreateAsset(library, path); else EditorUtility.SetDirty(library);
        return library;
    }

    static Sprite[] CleanWeaponCels()
    {
        const string name = "PixelWeaponContacts";
        string path = Root + name + ".png";
        if (!File.Exists(path))
        {
            // These are existing finished cels, copied without drawing or retouching.
            // Separate sprite identities keep SlashVolume's attack-only sweep out of a block.
            string sword = "Assets/Art/Skeletons_Free_Pack/Skeleton_Sword/Skeleton_White/Skeleton_Without_VFX/Skeleton_01_White_Attack";
            string[] sources = { sword + "1.png", sword + "2.png", "Assets/Art/EndlessSkeletons/sword-shield/Attack1.png",
                "Assets/Art/EndlessSkeletons/sword-shield/Attack2.png", "Assets/Art/EndlessSkeletons/axe/Attack1.png", "Assets/Art/EndlessSkeletons/axe/Attack2.png" };
            int[] cels = { 5, 2, 6, 3, 7, 5 };
            var atlas = new Texture2D(576, 64, TextureFormat.RGBA32, false);
            try
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    var original = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    try { original.LoadImage(File.ReadAllBytes(sources[i])); atlas.SetPixels(i * 96, 0, 96, 64, original.GetPixels(cels[i] * 96, 0, 96, 64)); }
                    finally { Object.DestroyImmediate(original); }
                }
                atlas.Apply(); File.WriteAllBytes(path, atlas.EncodeToPNG());
            }
            finally { Object.DestroyImmediate(atlas); }
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 16; importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false; importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed; importer.npotScale = TextureImporterNPOTScale.None;
            importer.alphaIsTransparency = true;
            importer.spritesheet = Enumerable.Range(0, 6).Select(i => new SpriteMetaData {
                name = "ParryWeaponContact_" + i, rect = new Rect(i * 96, 0, 96, 64), alignment = 9, pivot = new Vector2(.47f, .41f) }).ToArray();
            importer.SaveAndReimport();
            File.AppendAllText(Root + "PixelSource.txt", "Weapon contacts: lossless96x64 copies, no new drawing.\n" +
                string.Join("\n", sources.Select((source, i) => source + " cel" + cels[i])) + "\nOriginal licenses/credits retained in project.\n");
        }
        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.rect.x).ToArray();
    }
}
