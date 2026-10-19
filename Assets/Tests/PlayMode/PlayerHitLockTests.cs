using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class PlayerHitLockTests
{
    [UnityTest, Category("Quick")]
    public IEnumerator SlayerExitRestoresAnAdvancingAnimatorBeforeTheNextIncomingHit()
    {
        var actor = new GameObject("Slayer exit actual controller fixture");
        actor.SetActive(false); actor.AddComponent<SpriteRenderer>();
        var animator = actor.AddComponent<Animator>(); animator.fireEvents = false;
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Characters/Player/AC_Controller.controller");
        var gate = actor.AddComponent<PlayerInputGate>();
        var health = actor.AddComponent<PlayerHealth>(); health.animator = animator; health.gate = gate;
        var attacks = actor.AddComponent<PlayerAttackEvents>();
        var slayer = actor.AddComponent<PlayerSlayerMode>();
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        typeof(PlayerSlayerMode).GetField("animator", flags).SetValue(slayer, animator);
        typeof(PlayerSlayerMode).GetField("attackEvents", flags).SetValue(slayer, attacks);
        typeof(PlayerSlayerMode).GetField("autoPlayScreenFX", flags).SetValue(slayer, false);
        typeof(PlayerSlayerMode).GetField("silhouetteOnInSlayer", flags).SetValue(slayer, false);
        Time.timeScale = 1; actor.SetActive(true); yield return null;
        try
        {
            foreach (string from in new[] { "Base Layer.player_slayer.attackS1",
                "Base Layer.player_slayer.attackS2", "Base Layer.player_slayer.attackS3" })
            {
                slayer.Activate(5);
                animator.Play(from, 0, .8f); animator.Update(.01f);
                typeof(PlayerSlayerMode).GetMethod("Exit", flags).Invoke(slayer, null);
                Assert.That(animator.GetFloat("AttackSpeedMul"), Is.GreaterThan(0f),
                    "The outgoing Slayer state must keep advancing while it blends to idle/hit");
                yield return new WaitForSeconds(.12f);
                health.TakeDamage(1, Vector2.zero);
                yield return new WaitForSeconds(.8f);
                Assert.That(gate.IsHitLocked, Is.False, "Hit recovery must finish after exiting " + from);
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash,
                    Is.EqualTo(Animator.StringToHash("Base Layer.player_idle")), "Slayer exit/hit must reach idle");
                Assert.That(animator.IsInTransition(0), Is.False, "No frozen outgoing Slayer transition");
            }
        }
        finally { Time.timeScale = 1; Object.DestroyImmediate(actor); }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator InterruptedIncomingHitReleasesLockWithoutShorteningARealHit()
    {
        var actor = new GameObject("Actual player hit transition fixture");
        actor.SetActive(false);
        actor.AddComponent<SpriteRenderer>();
        var animator = actor.AddComponent<Animator>();
        animator.fireEvents = false; // Observe state behaviours without unrelated attack/VFX events.
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Characters/Player/AC_Controller.controller");
        var gate = actor.AddComponent<PlayerInputGate>();
        var health = actor.AddComponent<PlayerHealth>();
        health.animator = animator; health.gate = gate; health.postHitIFrames = 0;
        Time.timeScale = 1;
        actor.SetActive(true);
        yield return null;
        try
        {
            foreach (string from in new[] { "Base Layer.player_idle", "Base Layer.attack1",
                "Base Layer.player_slayer.attackS1", "Base Layer.player_slayer.attackS2",
                "Base Layer.player_slayer.attackS3", "Base Layer.player_parry", "Base Layer.player_parry_success" })
            foreach (float incomingTime in new[] { .0001f, .001f, .015f, .03f })
            foreach (bool pauseBeforeInterruption in new[] { false, true })
            {
                animator.Play(from, 0, .15f); animator.Update(0);
                gate.SetHitLocked(false); health.FullRestore();
                health.TakeDamage(1, Vector2.zero);
                animator.Update(incomingTime);
                Assert.That(gate.IsHitLocked, Is.True, "The real hit state acquires its input lock");
                if (pauseBeforeInterruption)
                {
                    Time.timeScale = 0;
                    yield return new WaitForSecondsRealtime(.08f);
                    Assert.That(gate.IsHitLocked, Is.True, "Pause must retain a real incoming hit lock");
                    Time.timeScale = 1;
                }
                // This is the same idle crossfade used by PlayerSlayerMode.Exit.
                animator.CrossFadeInFixedTime("Base Layer.player_idle", .02f, 0, 0);
                animator.Update(.03f);
                yield return null;
                Assert.That(gate.IsHitLocked, Is.False,
                    "An interrupted incoming hit cannot leave a lock after returning to idle: " + from + " / " + incomingTime);
            }
        }
        finally { Time.timeScale = 1; Object.DestroyImmediate(actor); }
    }
}
