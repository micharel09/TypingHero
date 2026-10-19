using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class PlayerAttackSortingTests
{
    GameObject player;
    Animator animator;
    SpriteRenderer sprite;
    PlayerAttackSorting sorting;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        Time.timeScale = 1;
        player = new GameObject("sorting fixture", typeof(SpriteRenderer), typeof(Animator));
        animator = player.GetComponent<Animator>();
        sprite = player.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
        animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Characters/Player/AC_Controller.controller");
#endif
        animator.fireEvents = false; // Isolate presentation from damage/audio/death events.
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        sorting = player.AddComponent<PlayerAttackSorting>();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        Time.timeScale = 1;
        Object.Destroy(player);
        yield return null;
    }

    IEnumerator Show(string state)
    {
        animator.Play(state, 0, 0);
        animator.Update(0);
        yield return null;
    }

#if UNITY_EDITOR
    [Test, Category("Quick")]
    public void PrefabKeepsAnimatorSpriteAndSortingLayerBindings()
    {
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Player/Player.prefab");
        var enemy = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Characters/Enermy/Skeleton.prefab");
        Assert.That(prefab.GetComponents<PlayerAttackSorting>().Length, Is.EqualTo(1));
        Assert.That(prefab.GetComponent<SpriteRenderer>().sortingOrder, Is.Zero);
        Assert.That(enemy.GetComponent<SpriteRenderer>().sortingOrder, Is.Zero);
        Assert.That(prefab.GetComponent<SpriteRenderer>().sortingLayerID,
            Is.EqualTo(enemy.GetComponent<SpriteRenderer>().sortingLayerID));
        Assert.That(prefab.GetComponent<PlayerController>().animator, Is.EqualTo(prefab.GetComponent<Animator>()));
        Assert.That(prefab.GetComponent<Animator>().runtimeAnimatorController, Is.EqualTo(animator.runtimeAnimatorController));
    }
#endif

    [UnityTest, Category("Quick")]
    public IEnumerator DisableRestoresTheCapturedOrderRatherThanAssumingZero()
    {
        Object.Destroy(sorting);
        yield return null;
        sprite.sortingOrder = 7;
        sorting = player.AddComponent<PlayerAttackSorting>();
        foreach (var state in new[] { "attack1", "player_parry", "player_parry_success" })
        {
            yield return Show("Base Layer." + state);
            Assert.That(sprite.sortingOrder, Is.EqualTo(1), state);
            foreach (var exit in new[] { "player_idle", "player_hit", "player_die" })
            {
                yield return Show("Base Layer." + exit);
                Assert.That(sprite.sortingOrder, Is.EqualTo(7), exit);
                yield return Show("Base Layer." + state);
            }
            sorting.enabled = false;
            Assert.That(sprite.sortingOrder, Is.EqualTo(7));
            sorting.enabled = true;
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator EveryAttackParryAndInterruptedStateUsesItsOwnOrder()
    {
        foreach (var attack in new[] { "Base Layer.attack1", "Base Layer.player_slayer.attackS1",
            "Base Layer.player_slayer.attackS2", "Base Layer.player_slayer.attackS3",
            "Base Layer.player_parry", "Base Layer.player_parry_success" })
        {
            yield return Show(attack);
            var tag = attack.Contains("parry") ? "Parry" : "Attack";
            Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsTag(tag), Is.True, attack);
            Assert.That(sprite.sortingOrder, Is.EqualTo(1), attack);
            foreach (var exit in new[] { "player_hit", "player_die", "player_idle" })
            {
                yield return Show("Base Layer." + exit);
                Assert.That(sprite.sortingOrder, Is.Zero, exit);
                yield return Show(attack);
            }
        }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator CrossfadesUseDestinationAndPauseDisableRestorePresentation()
    {
        yield return Show("Base Layer.player_idle");
        foreach (var destination in new[] { "attack1", "player_parry", "player_parry_success",
            "player_parry", "attack1", "player_slayer.attackS2", "player_parry", "player_parry_success" })
        {
            animator.CrossFadeInFixedTime("Base Layer." + destination, .5f, 0, 0);
            animator.Update(.01f);
            yield return null;
            Assert.That(animator.IsInTransition(0), Is.True, destination);
            Assert.That(sprite.sortingOrder, Is.EqualTo(1), destination);
            yield return Show("Base Layer." + destination);
        }

        foreach (var foreground in new[] { "attack1", "player_parry", "player_parry_success" })
        {
            yield return Show("Base Layer." + foreground);
            Time.timeScale = 0;
            yield return Show("Base Layer.player_hit");
            Assert.That(sprite.sortingOrder, Is.EqualTo(1), "pause holds displayed order: " + foreground);
            Time.timeScale = 1;
            yield return null;
            Assert.That(sprite.sortingOrder, Is.Zero);

            yield return Show("Base Layer." + foreground);
            sorting.enabled = false;
            Assert.That(sprite.sortingOrder, Is.Zero, "disable restores immediately");
            sorting.enabled = true;
            yield return null;
            Assert.That(sprite.sortingOrder, Is.EqualTo(1), foreground);
            foreach (var exit in new[] { "player_idle", "player_hit", "player_die" })
            {
                animator.CrossFadeInFixedTime("Base Layer." + exit, .5f, 0, 0);
                animator.Update(.01f);
                yield return null;
                Assert.That(sprite.sortingOrder, Is.Zero, "interruption destination: " + exit);
                yield return Show("Base Layer." + foreground);
            }
        }
    }
}
