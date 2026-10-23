#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class CombatHUDFeedbackTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Get<T>(object target, string name) => (T)target.GetType().GetField(name, Flags).GetValue(target);
    static void Tick(CombatHUDFeedback feedback) => typeof(CombatHUDFeedback).GetMethod("LateUpdate", Flags).Invoke(feedback, null);

    [UnityTest, Category("HUDFeedback")]
    public IEnumerator ActualDamageRestoreStunAndPauseRespectPresentationLifecycle()
    {
        var oldMusic = MusicPlayer.I; var oldSfx = SfxPlayer.I;
        TutorialRun.Begin();
        yield return DungeonSceneTestFlow.Start();
        yield return null;
        try
        {
            var hud = Object.FindObjectOfType<CombatHUDFeedback>();
            var player = Object.FindObjectOfType<PlayerHealth>();
            var enemy = Object.FindObjectOfType<SkeletonController>();
            var stamina = enemy.GetComponent<EnemyStamina>();
            var stun = enemy.GetComponent<EnemyStunController>();
            var hp = Get<TMP_Text>(hud, "playerHp");
            var frame = Get<Image>(hud, "hpFrame");
            var icon = Get<Image>(hud, "staminaIcon");
            var label = Get<TMP_Text>(hud, "guardBreak");
            var red = Get<Color>(hud, "red");
            Assert.That(Object.FindObjectsOfType<CombatHUDFeedback>().Length, Is.EqualTo(1));
            Assert.That(label.enabled, Is.False);
            Assert.That(icon.sprite.name, Is.EqualTo("icon-shield"));
            Assert.That(Get<float>(hud, "hitRemaining"), Is.Zero);
            var status = Object.FindObjectOfType<CombatR7Status>();
            Assert.That(status.transform.Find("RoundR7"), Is.Null);
            Assert.That(status.transform.Find("RoundReadabilityR7"), Is.Null);
            GamePauseController.SetTutorialReading(false);
            var parry = player.parry;
            var key = Get<Image>(hud, "parryKey");
            var cue = Get<GameObject>(enemy, "attackCue");
            Assert.That(Get<float>(parry, "successIFrame"), Is.EqualTo(.08f));
            Assert.That(parry.CanBeginParry, Is.True);
            parry.gate.SetHitLocked(true); cue.SetActive(true); Tick(hud);
            Assert.That(parry.CanBeginParry, Is.False);
            Assert.That(key.color.r, Is.EqualTo(.4f).Within(.001f), "locked cue stays dim");
            parry.gate.SetHitLocked(false); Tick(hud);
            Assert.That(key.color, Is.EqualTo(Get<Color>(hud, "gold")), "real ready cue is gold");
            cue.SetActive(false);
            Assert.That(parry.TryBeginParry(), Is.True); Tick(hud);
            Assert.That(key.color.r, Is.EqualTo(.4f).Within(.001f), "requested pose locks input immediately");
            parry.Anim_ParryOpen(); Tick(hud);
            Assert.That(key.color, Is.EqualTo(Get<Color>(hud, "gold")));
            parry.Anim_ParryClose(); parry.enabled = false;
            player.parry = null; player.postHitIFrames = 0;
            player.RestoreEncounterHealth(11); Tick(hud);
            Assert.That(hp.color, Is.Not.EqualTo(red));
            Assert.That(Get<float>(hud, "hitRemaining"), Is.Zero, "carry/restore is not damage");
            player.TakeDamage(1, Vector2.zero); Tick(hud);
            Assert.That(player.Current, Is.EqualTo(10));
            Assert.That(hp.color, Is.EqualTo(red), "exact 25 percent is low HP");
            Assert.That(frame.color, Is.Not.EqualTo(Color.white));
            player.TakeDamage(1, Vector2.zero);
            Assert.That(Get<float>(hud, "hitRemaining"), Is.EqualTo(.18f));
            Tick(hud); Assert.That(hp.color, Is.EqualTo(red));
            GamePauseController.SetTutorialReading(true);
            float remaining = Get<float>(hud, "hitRemaining");
            yield return new WaitForSecondsRealtime(.22f);
            Assert.That(Get<float>(hud, "hitRemaining"), Is.EqualTo(remaining));
            GamePauseController.SetTutorialReading(false);
            player.FullRestore(); Tick(hud);
            Assert.That(hp.color, Is.Not.EqualTo(red));
            yield return new WaitForSecondsRealtime(.22f);
            Assert.That(frame.color, Is.EqualTo(Color.white));
            player.FullRestore();
            Assert.That(Get<float>(hud, "hitRemaining"), Is.Zero, "restore never starts a damage flash");
            // Setup notifications alone never announce a break.
            stamina.PreparePractice(0); stun.TriggerStun(.05f); Tick(hud);
            Assert.That(label.enabled, Is.False);
            yield return new WaitForSecondsRealtime(.08f);
            stamina.PreparePractice(stamina.max);
            stamina.ConsumeHit(stamina.max);
            Assert.That(label.enabled, Is.False, "zero stamina needs an actual stun");
            stun.TriggerStun(.12f); Tick(hud);
            Assert.That(icon.sprite.name, Is.EqualTo("icon-warning"));
            Assert.That(label.enabled, Is.False, "Slayer replaces the guard-break announcement");
            GamePauseController.SetTutorialReading(true);
            yield return new WaitForSecondsRealtime(.22f);
            Assert.That(icon.sprite.name, Is.EqualTo("icon-warning"));
            Assert.That(label.enabled, Is.False);
            Assert.That(stun.IsStunned, Is.True);
            GamePauseController.SetTutorialReading(false);
            yield return new WaitForSecondsRealtime(.95f);
            Assert.That(icon.sprite.name, Is.EqualTo("icon-shield"));
            Assert.That(label.enabled, Is.False);
            stamina.PreparePractice(stamina.max); stamina.ConsumeParry();
            stamina.ConsumeHit(stamina.max); stun.TriggerStun(.1f); Tick(hud);
            Assert.That(icon.sprite.name, Is.EqualTo("icon-warning"));
            Assert.That(label.enabled, Is.False, "second break only changes icon");
            hud.enabled = false;
            Assert.That(icon.sprite.name, Is.EqualTo("icon-shield"));
            hud.enabled = true;
            Assert.That(label.enabled, Is.False);
        }
        finally
        {
            TutorialRun.End(); GamePauseController.SetTutorialReading(false);
            var clean = SceneManager.CreateScene("HUDFeedbackCleanup"); SceneManager.SetActiveScene(clean);
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
