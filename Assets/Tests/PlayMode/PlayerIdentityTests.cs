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
