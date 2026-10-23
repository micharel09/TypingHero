using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class ParryCueTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Set(object target, string name, object value) => target.GetType().GetField(name, Flags).SetValue(target, value);
    static void Call(object target, string name) => target.GetType().GetMethod(name, Flags).Invoke(target, null);

    [UnityTest, Category("Quick")]
    public IEnumerator AuthoredImpactPredictionTracksClassicAndSixAttacksSpeedHoldPauseAndCancel()
    {
        int[] encounters = { -1, 0, 12, 2, 6, 3, 3 };
        for (int test = 0; test < encounters.Length; test++)
        {
            var receiver = new GameObject("Prediction target").AddComponent<PlayerHealth>();
            receiver.maxHealth = 40; receiver.FullRestore();
            var host = new GameObject("Prediction actor"); host.SetActive(false);
            host.AddComponent<SpriteRenderer>();
            var animator = host.AddComponent<Animator>();
#if UNITY_EDITOR
            animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
            var strike = host.AddComponent<AttackStrike>(); Set(strike, "damageReceiver", receiver);
            var brain = host.AddComponent<SkeletonController>();
            var cue = new GameObject("Cue"); cue.transform.SetParent(host.transform); cue.SetActive(false);
            Set(brain, "animator", animator); Set(brain, "attackCue", cue);
            host.SetActive(true);
            if (test > 0)
            {
                brain.ConfigureEndless(new EndlessRun { Defeated = encounters[test] });
                if (test == 2 || test == 6) Set(brain, "pattern", 1);
            }
            try
            {
                Set(brain, "_nextAttackAt", -1f); Call(brain, "Update");
                animator.Update(0);
                float deadline = Time.realtimeSinceStartup + 5;
                while (!brain.TryGetAttackTelegraph(out _) && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(brain.TryGetAttackTelegraph(out float before), Is.True, "telegraph case " + test);
                Assert.That(before, Is.InRange(.20f, .4f));
                GamePauseController.SetEncounterReading(true);
                yield return new WaitForSecondsRealtime(.1f);
                Assert.That(brain.TryGetAttackTelegraph(out float paused), Is.True);
                Assert.That(paused, Is.EqualTo(before).Within(.002f));
                GamePauseController.SetEncounterReading(false);
                // Change playback speed after the authored cue: countdown follows the Animator.
                if (animator.speed > 0)
                {
                    animator.speed = 2;
                    Assert.That(brain.TryGetAttackTelegraph(out float fast), Is.True);
                    Assert.That(fast, Is.EqualTo(before / 2).Within(.002f));
                    animator.speed = 1;
                }
                while (brain.TryGetAttackTelegraph(out float remaining) && remaining > .2f && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(brain.TryGetAttackTelegraph(out float lead), Is.True);
                float lit = Time.time;
                while (receiver.Current == 40 && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(receiver.Current, Is.LessThan(40), "real damage event fired");
                Assert.That(Time.time - lit, Is.EqualTo(lead).Within(.045f), "prediction meets impact case " + test);
                Assert.That(brain.TryGetAttackTelegraph(out _), Is.False, "impact clears cue");
                strike.Anim_DealDamage();
                brain.enabled = false;
                Assert.That(brain.TryGetAttackTelegraph(out _), Is.False, "stun/disable clears cue");
            }
            finally
            {
                GamePauseController.SetEncounterReading(false);
                Object.DestroyImmediate(host); Object.DestroyImmediate(receiver.gameObject);
            }
        }
    }
    [UnityTest, Category("Quick")]
    public IEnumerator CrossfadeOutOfRealAttackClearsPredictionBeforeOutgoingStateFinishes()
    {
        var host = new GameObject("Canceled Classic strike"); host.SetActive(false);
        var animator = host.AddComponent<Animator>();
#if UNITY_EDITOR
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
        host.AddComponent<AttackStrike>();
        var brain = host.AddComponent<SkeletonController>(); Set(brain, "animator", animator);
        var cue = new GameObject("Cue"); cue.transform.SetParent(host.transform); cue.SetActive(false); Set(brain, "attackCue", cue);
        try
        {
            host.SetActive(true); animator.Update(0);
            Set(brain, "_nextAttackAt", -1f); Call(brain, "Update");
            animator.Update(0); animator.Update(.72f);
            Assert.That(brain.TryGetAttackTelegraph(out _), Is.True);
            animator.CrossFade("Base Layer.skeleton_hit", .1f, 0, 0);
            animator.Update(.01f);
            Assert.That(animator.IsInTransition(0), Is.True);
            Assert.That(brain.TryGetAttackTelegraph(out _), Is.False, "outgoing attack no longer requests a parry");
        }
        finally { Object.DestroyImmediate(host); }
        yield return null;
    }

}
