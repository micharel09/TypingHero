using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public sealed class PlayerIdentityTests
{
    const string Key = "player_name_v1";
    string previous;
    bool existed;
    static void Reload()
    {
        typeof(PlayerIdentity).GetField("_loaded", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
        PlayerIdentity.EnsureLoaded();
    }

    [SetUp]
    public void SavePreference()
    {
        existed = PlayerPrefs.HasKey(Key);
        previous = PlayerPrefs.GetString(Key);
    }

    [TearDown]
    public void RestorePreference()
    {
        if (existed) PlayerPrefs.SetString(Key, previous);
        else PlayerPrefs.DeleteKey(Key);
        typeof(PlayerIdentity).GetField("_loaded", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
        PlayerPrefs.Save();
    }

    [Test, Category("Quick")]
    public void NewGuestIsValidAndStableAcrossReload()
    {
        PlayerPrefs.DeleteKey(Key);
        Reload();
        var guest = PlayerIdentity.Name;
        Assert.That(guest, Does.Match("^Guest[0-9A-F]{4}$"));
        Assert.That(PlayerIdentity.Validate(guest, 3, 10, out _, out _), Is.True);
        Assert.That(PlayerPrefs.GetString(Key), Is.EqualTo(guest));
        Reload();
        Assert.That(PlayerIdentity.Name, Is.EqualTo(guest));
    }

    [TestCase("1234567890123456")]
    [TestCase("Old Player")]
    [TestCase("GuestA12F")]
    [Category("Quick")]
    public void ExistingNameIsNeverReplaced(string name)
    {
        PlayerPrefs.SetString(Key, name);
        Reload();
        Assert.That(PlayerIdentity.Name, Is.EqualTo(name));
        Assert.That(PlayerPrefs.GetString(Key), Is.EqualTo(name));
    }

    [TestCase("", false)]
    [TestCase("a", false)]
    [TestCase("ab", false)]
    [TestCase("abc", true)]
    [TestCase("abcdefghi", true)]
    [TestCase("abcdefghij", true)]
    [Category("Quick")]
    public void NameLengthBoundaries(string raw, bool expected)
    {
        Assert.That(PlayerIdentity.MinNameLength, Is.EqualTo(3));
        Assert.That(PlayerIdentity.MaxNameLength, Is.EqualTo(10));
        Assert.That(PlayerIdentity.TrySetName(raw, out var normalized, out _), Is.EqualTo(expected));
        if (expected) Assert.That(PlayerPrefs.GetString(Key), Is.EqualTo(normalized));
    }

    [TestCase("12345678901")]
    [TestCase("1234567890123456")]
    [TestCase("   abc     ")]
    [Category("Quick")]
    public void MenuRejectsRawOverLimitBeforeNormalization(string raw)
    {
        var args = new object[] { raw, null, null };
        var validate = typeof(MainMenuUI).GetMethod("ValidateName", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(validate.Invoke(null, args), Is.False);
        Assert.That(args[2], Does.Contain("10"));
    }

    [Test, Category("Quick")]
    public void MenuShowsLegacyNameWithoutChangingPreferenceAndDisablesStart()
    {
        const string legacy = "1234567890123456";
        PlayerPrefs.SetString(Key, legacy);
        Reload();
        var root = new GameObject("IdentityMenuTest");
        root.SetActive(false);
        try
        {
            var input = root.AddComponent<TMPro.TMP_InputField>();
            var buttonObject = new GameObject("Start", typeof(RectTransform), typeof(UnityEngine.UI.Button));
            buttonObject.transform.SetParent(root.transform);
            var button = buttonObject.GetComponent<UnityEngine.UI.Button>();
            var menu = root.AddComponent<MainMenuUI>();
            typeof(MainMenuUI).GetField("nameInput", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(menu, input);
            typeof(MainMenuUI).GetField("playButton", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(menu, button);
            typeof(MainMenuUI).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
            Assert.That(input.text, Is.EqualTo(legacy));
            Assert.That(input.characterLimit, Is.EqualTo(PlayerIdentity.MaxNameLength));
            Assert.That(input.restoreOriginalTextOnEscape, Is.False);
            Assert.That(button.interactable, Is.False);
            Assert.That(PlayerPrefs.GetString(Key), Is.EqualTo(legacy));
            Assert.That(PlayerIdentity.Name, Is.EqualTo(legacy));
        }
        finally { Object.DestroyImmediate(root); }
    }

    [Test, Category("Quick")]
    public void EditingGuestKeepsValidationAndPersistsCustomName()
    {
        PlayerPrefs.DeleteKey(Key);
        Reload();
        var guest = PlayerIdentity.Name;
        Assert.That(PlayerIdentity.TrySetName("!", out _, out _), Is.False);
        Assert.That(PlayerIdentity.TrySetName("a", out _, out _), Is.False);
        Assert.That(PlayerIdentity.Name, Is.EqualTo(guest));
        Assert.That(PlayerIdentity.TrySetName("  new   hero! ", out var normalized, out _), Is.True);
        Assert.That(normalized, Is.EqualTo("New Hero"));
        Reload();
        Assert.That(PlayerIdentity.Name, Is.EqualTo(normalized));
    }
}
