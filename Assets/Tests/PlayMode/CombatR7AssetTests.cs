#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[Category("HUDFeedback")]
public sealed class CombatR7AssetTests
{
    const string Art = "Assets/Art/UI/CombatR7/";

    [Test, Category("Quick")]
    public void EveryCombatComponentImportsAsAnActualPointClampSprite()
    {
        var paths = Directory.GetFiles(Art, "*.png");
        Assert.That(paths.Length, Is.EqualTo(29));
        foreach (var path in paths)
        {
            var asset = path.Replace('\\', '/');
            var importer = (TextureImporter)AssetImporter.GetAtPath(asset);
            Assert.That(importer.textureShape, Is.EqualTo(TextureImporterShape.Texture2D), asset);
            Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite), asset);
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), asset);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Clamp), asset);
            Assert.That(importer.mipmapEnabled, Is.False, asset);
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed), asset);
            Assert.That(AssetDatabase.LoadAssetAtPath<Sprite>(asset), Is.Not.Null, asset);
        }
    }

    [Test, Category("Quick")]
    public void ActualPoolFitsFixedCellsUsingTheImported16PixelFont()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Art + "VT323Pixel16.asset");
        Assert.That(font, Is.Not.Null);
        Assert.That(font.faceInfo.pointSize, Is.EqualTo(16));
        Assert.That(font.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
        Assert.That(font.atlasPadding, Is.GreaterThanOrEqualTo(1), "TMP bitmap quad requires a blank gutter");
        foreach (var atlas in font.atlasTextures) Assert.That(atlas.filterMode, Is.EqualTo(FilterMode.Point));
        var pool = AssetDatabase.LoadAssetAtPath<WordPoolConfig>("Assets/Data/Words/WP_Common200.asset");
        pool.BuildCache();
        var words = (string[])typeof(WordPoolConfig).GetField("_cached", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(pool);
        Assert.That(words.Length, Is.EqualTo(219));
        var host = new GameObject("native font measurement", typeof(RectTransform), typeof(Canvas));
        try
        {
            var text = host.AddComponent<TextMeshProUGUI>();
            text.font = font; text.fontSize = 16; text.enableWordWrapping = false;
            int maximum = 0;
            foreach (var word in words)
            {
                int width = Mathf.RoundToInt(text.GetPreferredValues(word).x);
                maximum = Mathf.Max(maximum, width);
                Assert.That(width, Is.InRange(1, 58), word);
            }
            Assert.That(maximum, Is.EqualTo(54));
            Assert.That(66 - maximum, Is.EqualTo(12), "Worst-case adjacent words retain twelve native pixels of air");
        }
        finally { Object.DestroyImmediate(host); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator SerializedCombatHpTracksFillZeroOneFiftyAndOneHundredPercent()
    {
        var oldMusic = MusicPlayer.I; var oldSfx = SfxPlayer.I;
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        yield return null;
        try
        {
            var root = GameObject.Find("PixelUI/Reference960x540").transform;
            foreach (string path in new[] { "HUD/PlayerStatus/PlayerHP", "HUD/EnemyStatus/EnemyHP" })
            {
                var original = root.Find(path).GetComponent<Slider>();
                Assert.That(original.fillRect.parent, Is.EqualTo(original.transform));
                // The approved compact status rails retain a 171-unit HP track.
                Assert.That(((RectTransform)original.transform).rect.size, Is.EqualTo(new Vector2(171, 18)));
                var clone = Object.Instantiate(original.gameObject, root);
                try
                {
                    foreach (var behaviour in clone.GetComponents<MonoBehaviour>())
                        if (!(behaviour is Slider)) behaviour.enabled = false;
                    var slider = clone.GetComponent<Slider>();
                    slider.minValue = 0; slider.maxValue = 1; slider.wholeNumbers = false;
                    Assert.That(slider.fillRect.GetComponent<Image>().sprite, Is.Not.Null);
                    Assert.That(clone.transform.Find("TrackR7").GetComponent<Image>().enabled, Is.True);
                    foreach (float value in new[] { 0f, .01f, .5f, 1f })
                    {
                        slider.value = value;
                        Canvas.ForceUpdateCanvases();
                        Assert.That(slider.fillRect.rect.width, Is.EqualTo(171 * value).Within(.01f), path + " / " + value);
                        Assert.That(slider.fillRect.rect.height, Is.EqualTo(18).Within(.01f));
                    }
                }
                finally { Object.DestroyImmediate(clone); }
            }
        }
        finally
        {
            TutorialRun.End(); GamePauseController.SetTutorialReading(false);
            var clean = SceneManager.CreateScene("CombatR7TestCleanup"); SceneManager.SetActiveScene(clean);
            SceneManager.UnloadSceneAsync("SampleScene");
            DungeonSceneTestFlow.Cleanup();
            if (!oldMusic && MusicPlayer.I) Object.Destroy(MusicPlayer.I.gameObject);
            if (!oldSfx && SfxPlayer.I) Object.Destroy(SfxPlayer.I.gameObject);
            AudioListener.pause = false; Time.timeScale = 1;
        }
        yield return null; yield return null;
    }
}
#endif
