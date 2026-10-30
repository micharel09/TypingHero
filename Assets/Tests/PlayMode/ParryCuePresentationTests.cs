using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public sealed class ParryCuePresentationTests
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Set(object target, string field, object value) => target.GetType().GetField(field, Private).SetValue(target, value);
    static void Draw(ParryCuePresenter presenter) => typeof(ParryCuePresenter).GetMethod("LateUpdate", Private).Invoke(presenter, null);

    [Test, Category("Quick")]
    public void RealAttackCountdownDrivesTwoTapCyclesPauseWindowLockAndContact()
    {
        var ui = new GameObject("Cue fixture", typeof(RectTransform)); ui.SetActive(false);
        var presenter = ui.AddComponent<ParryCuePresenter>();
        var key = new GameObject("Key", typeof(RectTransform)).GetComponent<RectTransform>(); key.SetParent(ui.transform);
        Image Part(string name) { var p = new GameObject(name, typeof(RectTransform), typeof(Image)); p.transform.SetParent(key); return p.GetComponent<Image>(); }
        var face = Part("Face"); var right = Part("Unused"); var locked = Part("Lock");
        var label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>(); label.transform.SetParent(key);
        Set(presenter, "key", key); Set(presenter, "left", face); Set(presenter, "right", right);
        Set(presenter, "locked", locked);
        var texture = new Texture2D(4, 1);
        var frames = new Sprite[4];
        foreach (int i in new[] { 0, 1, 2, 3 }) frames[i] = Sprite.Create(texture, new Rect(i, 0, 1, 1), Vector2.zero);
        Set(presenter, "released", frames[0]); Set(presenter, "pressed", frames[1]);
        Set(presenter, "readyReleased", frames[2]); Set(presenter, "readyPressed", frames[3]);
        var actor = new GameObject("Real attack", typeof(Animator)); actor.SetActive(false);
        var animator = actor.GetComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
        actor.AddComponent<AttackStrike>();
        var brain = actor.AddComponent<SkeletonController>(); Set(brain, "animator", animator);
        var target = actor.AddComponent<ParryTarget>(); target.skeleton = brain;
        var player = new GameObject("Manual parry", typeof(Animator)); player.SetActive(false);
        var playerAnimator = player.GetComponent<Animator>();
        playerAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        var parry = player.AddComponent<ParrySystem>(); parry.logs = false;
        parry.playerAnimator = playerAnimator; parry.enemy = target;
        parry.config = AssetDatabase.LoadAssetAtPath<ParryConfig>("Assets/Data/Attack/PC_Parry.asset");
        Set(presenter, "parry", parry);
        // The fixture isolates presentation from authoring; runtime bindings are checked in the player.
        Set(presenter, "headController", animator.runtimeAnimatorController);
        void At(float remaining)
        {
            animator.Play("Base Layer.skeleton_attack_heavy", 0, 0); animator.Update(0);
            Assert.That(brain.TryGetAttackTelegraph(out float contact), Is.True);
            var state = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play("Base Layer.skeleton_attack_heavy", 0, (contact - remaining) / state.length); animator.Update(0);
            Assert.That(brain.TryGetAttackTelegraph(out float predicted), Is.True);
            Assert.That(predicted, Is.EqualTo(remaining).Within(.002f));
            Draw(presenter);
        }
        try
        {
            player.SetActive(true); actor.SetActive(true);
            Set(brain, "_attacking", true); Set(brain, "attackPending", true);
            At(.8f); Assert.That(key.gameObject.activeSelf, Is.False);
            At(.3f); Assert.That(key.gameObject.activeSelf, Is.False, "no advance warning");
            At(.2f); Assert.That(key.gameObject.activeSelf, Is.True, "cue starts at the press beat");
            Assert.That(face.sprite, Is.SameAs(frames[2]));
            At(.18f); Assert.That(face.sprite, Is.SameAs(frames[2]));
            At(.13f); Assert.That(face.sprite, Is.SameAs(frames[3]));
            Time.timeScale = 0; At(.08f); Assert.That(face.sprite, Is.SameAs(frames[3]), "hitstop freezes the drawn frame");
            Time.timeScale = 1; Draw(presenter); Assert.That(face.sprite, Is.SameAs(frames[2]));
            At(.03f); Assert.That(face.sprite, Is.SameAs(frames[3]), "second tap cycle");
            parry.Anim_ParryOpen(); At(.3f);
            Assert.That(key.gameObject.activeSelf, Is.True, "open window remains visible outside the lead");
            At(.03f);
            Assert.That(face.sprite, Is.SameAs(frames[3])); Assert.That(locked.gameObject.activeSelf, Is.False);
            parry.Anim_ParryClose(); Set(parry, "poseRequestedFrame", Time.frameCount); Draw(presenter);
            Assert.That(locked.gameObject.activeSelf, Is.True);
            Set(parry, "poseRequestedFrame", -1); brain.HideCue(); Draw(presenter);
            Assert.That(key.gameObject.activeSelf, Is.False, "contact or cancellation clears the key");
            Set(brain, "attackPending", true); At(.9f);
            Assert.That(key.gameObject.activeSelf, Is.False, "next strike stays hidden until its beat");
            Time.timeScale = 0; At(.13f);
            Assert.That(key.gameObject.activeSelf, Is.False, "hitstop cannot reveal a hidden cue");
            Time.timeScale = 1; Draw(presenter);
            Assert.That(key.gameObject.activeSelf, Is.True);
            brain.HideCue(); Draw(presenter);
            Assert.That(key.gameObject.activeSelf, Is.False, "cancellation clears a visible cue");
        }
        finally
        {
            Time.timeScale = 1;
            Object.DestroyImmediate(ui); Object.DestroyImmediate(actor); Object.DestroyImmediate(player);
            foreach (var frame in frames) Object.DestroyImmediate(frame);
            Object.DestroyImmediate(texture);
        }
    }
}
