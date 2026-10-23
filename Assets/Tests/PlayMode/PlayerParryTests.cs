using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public sealed class PlayerParryTests
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    const string Original = "Assets/Art/male_hero/individual_sheets/";
    GameObject player, ui, target;
    Animator animator;
    SpriteRenderer renderer;
    ParrySystem parry;
    PlayerInputGate gate;
    PlayerController controller;
    PlayerAttackEvents attacks;
    TypingManager typing;
    Sprite[] sprites;

    static void Set(object o, string field, object value) => o.GetType().GetField(field, Private).SetValue(o, value);
    static T Get<T>(object o, string field) => (T)o.GetType().GetField(field, Private).GetValue(o);
    static void Tick(object o) => o.GetType().GetMethod("Update", Private).Invoke(o, null);

    [UnitySetUp]
    public IEnumerator Setup()
    {
        Time.timeScale = 1; TypingManager.MuteWordCorrect = false;
        ui = new GameObject("parry typing", typeof(RectTransform), typeof(Canvas)); ui.SetActive(false);
        typing = ui.AddComponent<TypingManager>();
        Set(typing, "rail", ui.GetComponent<RectTransform>());
        foreach (var field in new[] { "wordText", "nextWordText", "thirdWordText" })
        {
            var label = new GameObject(field, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            label.transform.SetParent(ui.transform, false);
            Set(typing, field, label.GetComponent<TextMeshProUGUI>());
        }
        ui.SetActive(true);
        player = new GameObject("original parry", typeof(SpriteRenderer), typeof(Animator)); player.SetActive(false);
        animator = player.GetComponent<Animator>(); renderer = player.GetComponent<SpriteRenderer>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        gate = player.AddComponent<PlayerInputGate>();
        parry = player.AddComponent<ParrySystem>(); parry.logs = false; parry.gate = gate; parry.playerAnimator = animator;
        parry.config = AssetDatabase.LoadAssetAtPath<ParryConfig>("Assets/Data/Attack/PC_Parry.asset");
        attacks = player.AddComponent<PlayerAttackEvents>();
        player.AddComponent<SfxOnParryPose>();
        controller = player.AddComponent<PlayerController>(); controller.animator = animator; controller.gate = gate;
        controller.attackEvents = attacks;
        controller.parryFollowUp = AssetDatabase.LoadAssetAtPath<AttackConfig>("Assets/Data/Attack/AC_ParryFollowUp.asset");
        controller.attack = AssetDatabase.LoadAssetAtPath<AttackConfig>("Assets/Data/Attack/AC_Attack1.asset");
        player.SetActive(true);
        sprites = AssetDatabase.LoadAllAssetsAtPath(Original + "male_hero-combo_3_end.png").OfType<Sprite>()
            .OrderBy(s => s.name).Skip(2).ToArray();
        yield return null;
        typing.enabled = false;
        typing.SetPractice(new[] { "type", "next", "third" }, false, true);
        animator.Play("Base Layer.player_idle", 0, 0); animator.Update(0);
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        Time.timeScale = 1; TypingManager.MuteWordCorrect = false; AudioListener.pause = false;
        Object.Destroy(player); Object.Destroy(ui); if (target) Object.Destroy(target);
        yield return null;
    }

    void Begin(bool success)
    {
        Assert.That(parry.TryBeginParry(), Is.True);
        animator.Update(0); animator.Update(.000001f);
        if (success) { parry.ParrySuccess(); animator.Update(0); animator.Update(.000001f); }
        Assert.That(renderer.sprite, Is.SameAs(sprites[0]), "entry must be original frame 2");
    }

    void Advance(float seconds)
    {
        animator.Update(seconds);
        // Manual Animator.Update calls do not advance Time.frameCount. The request
        // has now been evaluated, just as it would be before the next game frame.
        Set(parry, "poseRequestedFrame", -1); Set(parry, "successRequestedFrame", -1);
        Tick(parry); Tick(controller);
    }

    [TestCase(false, false, 1f)]
    [TestCase(false, true, 1f)]
    [TestCase(false, false, .5f)]
    [TestCase(false, true, .5f)]
    [TestCase(true, false, 1f)]
    [TestCase(true, true, 1f)]
    [TestCase(true, false, .5f)]
    [TestCase(true, true, .5f)]
    [Category("Quick")]
    public void OriginalKeysAndRealControllerExit(bool success, bool mirror, float speed)
    {
        renderer.flipX = mirror; animator.speed = speed;
        Begin(success);
        float hold = success ? .8f : .25f, end = success ? .95f : .4f;
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(success ? Original + "player_parry_success.anim" : "Assets/Characters/Player/player_parry.anim");
        var binding = AnimationUtility.GetObjectReferenceCurveBindings(clip).Single();
        var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
        Assert.That(keys.Select(k => k.value), Is.EqualTo(sprites));
        Assert.That(clip.length, Is.EqualTo(end).Within(.00001f));
        float current = .000002f;
        for (int i = 1; i < keys.Length; i++)
        {
            float boundary = keys[i].time;
            Advance((boundary - current - .0001f) / speed);
            Assert.That(renderer.sprite, Is.SameAs(sprites[i - 1]), "before key " + i);
            Advance(.0002f / speed);
            Assert.That(renderer.sprite, Is.SameAs(sprites[i]), "at key " + i);
            current = boundary + .0001f;
        }
        Advance((end - current - .0001f) / speed);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsTag("Parry"), Is.True);
        Advance(.0003f / speed); animator.Update(0);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, Is.EqualTo(Animator.StringToHash("Base Layer.player_idle")));
        Assert.That(renderer.sprite.name, Does.StartWith("male_hero-idle_"));
        Assert.That(gate.IsParryPoseLocked, Is.False);
        if (success) Assert.That(parry.SuccessPoseCompleted, Is.True);
        Assert.That(renderer.flipX, Is.EqualTo(mirror));
        Assert.That(keys[1].time, Is.EqualTo(hold).Within(.00001f));
    }

    [Test, Category("Quick")]
    public void WindowClosesAt250MsWhileThePoseContinuesTo400Ms()
    {
        Begin(false);
        Assert.That(parry.IsWindowActive, Is.True);
        Advance(.248f); Assert.That(parry.IsWindowActive, Is.True);
        Advance(.002f); Assert.That(parry.IsWindowActive, Is.False);
        Assert.That(gate.IsParryPoseLocked, Is.True);
        Assert.That(parry.TryBeginParry(), Is.False);
        Assert.That(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Characters/Player/player_parry.anim").events
            .Single(e => e.functionName == "Anim_ParryClose").time, Is.EqualTo(.25f));
    }

    [TestCase(.001, true)]
    [TestCase(.75, true)]
    [TestCase(.799, true)]
    [TestCase(.8, false)]
    [TestCase(.801, false)]
    [Category("Quick")]
    public void OnlyOneCompletedWordBefore800MsQueuesFollowUp(double elapsed, bool accepted)
    {
        Begin(true);
        Set(parry, "followUpUntil", Time.unscaledTimeAsDouble + (.8 - elapsed));
        typing.ProcessInput("typenext");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.EqualTo(accepted));
        Assert.That(Get<bool>(controller, "followUpQueued"), Is.EqualTo(accepted));
        Advance(.949f);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsTag("Parry"), Is.True, "no early follow-up");
        Advance(.002f); animator.Update(.02f); Tick(parry); Tick(controller); animator.Update(.02f);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"), Is.EqualTo(accepted));
        if (accepted) Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, Is.EqualTo(Animator.StringToHash("Base Layer.parry_followup")));
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
    }

    [TestCase("hit")]
    [TestCase("death")]
    [TestCase("slayer")]
    [TestCase("idle")]
    [TestCase("disable")]
    [Category("Quick")]
    public void InterruptionDiscardsTheQueuedFollowUp(string reason)
    {
        Begin(true); typing.ProcessInput("type");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        if (reason == "hit") gate.SetHitLocked(true);
        else if (reason == "death") gate.SetDeadLocked(true);
        else if (reason == "slayer") TypingManager.MuteWordCorrect = true;
        else if (reason == "disable") controller.enabled = false;
        else { animator.Play("Base Layer.player_idle", 0, 0); }
        Advance(0);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
    }

    [UnityTest, Category("Quick")]
    public IEnumerator PauseHoldsThePoseDeadlineAndPendingAttack()
    {
        Begin(true);
        var pause = ui.AddComponent<GamePauseController>();
        Set(parry, "followUpUntil", Time.unscaledTimeAsDouble + .1);
        pause.TogglePause(true);
        typing.ProcessInput("type");
        yield return null; // Finish the frame whose delta was computed before pausing.
        float before = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        yield return new WaitForSecondsRealtime(.25f);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False, "paused typing is ignored");
        Assert.That(parry.CanQueueFollowUp, Is.True, "pause does not consume the 800 ms budget");
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, Is.EqualTo(before).Within(.00001f));
        pause.Resume(); typing.ProcessInput("type");
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        pause.TogglePause(true);
        yield return new WaitForSecondsRealtime(.15f);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.True);
        pause.Resume();
    }

    [Test, Category("Quick")]
    public void SuccessIFrameDoesNotRestartPoseOrEmitAnotherParry()
    {
        var health = player.AddComponent<PlayerHealth>(); health.parry = parry; health.gate = gate;
        int successes = 0; parry.OnParrySuccess += _ => successes++;
        Begin(false); health.TakeDamage(5, Vector2.zero);
        animator.Update(0); Advance(.04f);
        float before = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        health.TakeDamage(5, Vector2.zero); animator.Update(0);
        Assert.That(successes, Is.EqualTo(1));
        Assert.That(health.Current, Is.EqualTo(health.maxHealth));
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, Is.EqualTo(before));
    }

    [Test, Category("Quick")]
    public void FollowUpUsesTheExistingAttackWindowAndDamagesATargetOnce()
    {
        var weapon = new GameObject("follow-up hitbox", typeof(BoxCollider2D)); weapon.transform.SetParent(player.transform);
        var box = weapon.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1; attacks.hitbox = box;
        target = new GameObject("follow-up target"); var damage = target.AddComponent<FeedbackDamageTarget>();
        var first = target.AddComponent<BoxCollider2D>(); var second = target.AddComponent<CircleCollider2D>();
        Begin(true); typing.ProcessInput("typenext");
        Advance(.951f); animator.Update(.03f); Tick(parry); Tick(controller); animator.Update(.03f);
        var hit = typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Private);
        Assert.That(Get<bool>(box, "active"), Is.False, "no damage during anticipation");
        animator.Update(.151f);
        hit.Invoke(box, new object[] { first }); hit.Invoke(box, new object[] { second });
        Assert.That(damage.hits, Is.EqualTo(1));
        int attackId = Get<int>(attacks, "currentAttackId");
        Advance(.7f); animator.Update(0); Tick(parry); Tick(controller); animator.Update(.1f);
        Assert.That(Get<int>(attacks, "currentAttackId"), Is.EqualTo(attackId), "one queued word cannot release twice");
        Assert.That(damage.hits, Is.EqualTo(1));
    }

    [TestCase(false)]
    [TestCase(true)]
    [Category("Quick")]
    public void AcceptedFollowUpKeepsTargetInterruptBeyondLegacyTTLForOneWindow(bool replaceBeforeClose)
    {
        var weapon = new GameObject("follow-up bypass hitbox", typeof(BoxCollider2D)); weapon.transform.SetParent(player.transform);
        attacks.hitbox = weapon.AddComponent<PlayerWeaponHitbox>();
        target = new GameObject("parried target"); target.AddComponent<FeedbackDamageTarget>();
        var enemy = target.AddComponent<ParryTarget>();
        parry.enabled = false; parry.enemy = enemy; parry.enabled = true;
        var binder = player.AddComponent<ParryBypassBinder>(); binder.enabled = false;
        Set(binder, "parry", parry); Set(binder, "attackEvents", attacks); binder.enabled = true;
        Begin(false); enemy.Anim_StrikeFrame(); animator.Update(0);
        typing.ProcessInput("type");
        // 950ms recovery + existing hitstop + frame delivery can exceed the old 1s TTL.
        Set(binder, "_lastParryTimeUnscaled", Time.unscaledTime - 2f);
        Advance(.951f); animator.Update(.03f); Tick(parry); Tick(controller); animator.Update(.03f);
        animator.Update(.151f);
        Assert.That(UninterruptibleBypass.IsActiveFor(target.transform), Is.True);
        if (replaceBeforeClose) attacks.OnAttackStart();
        else attacks.OnAttackEnd();
        Assert.That(UninterruptibleBypass.IsActiveFor(target.transform), Is.False);
        attacks.OnAttackStart();
        Assert.That(UninterruptibleBypass.IsActiveFor(target.transform), Is.False, "accepted follow-up bypass is consumed once");
        attacks.OnAttackEnd();
    }
    [TestCase(false)]
    [TestCase(true)]
    [Category("Quick")]
    public void FollowUpExitOrDisableClosesHitboxAndNotifiesOnce(bool disable)
    {
        var weapon = new GameObject("cancelled follow-up", typeof(BoxCollider2D));
        weapon.transform.SetParent(player.transform);
        attacks.hitbox = weapon.AddComponent<PlayerWeaponHitbox>();
        int closed = 0; attacks.OnWindowClose += () => closed++;
        animator.Play("Base Layer.parry_followup", 0, 0); animator.Update(0); animator.Update(.151f);
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.True);
        if (disable) controller.enabled = false;
        else { animator.Play("Base Layer.player_idle", 0, 0); animator.Update(0); }
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.False);
        attacks.CancelAttack();
        Assert.That(closed, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void FollowUpCrossfadeLasts20Milliseconds()
    {
        typeof(PlayerController).GetMethod("PlayAttack", Private).Invoke(controller, new object[] { true });
        animator.Update(.015f);
        Assert.That(animator.IsInTransition(0), Is.True, "20ms is a fixed duration, not normalized clip time");
        animator.Update(.006f);
        Assert.That(animator.IsInTransition(0), Is.False);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, Is.EqualTo(Animator.StringToHash("Base Layer.parry_followup")));
    }

    [TestCase(false)]
    [TestCase(true)]
    [Category("Quick")]
    public void FollowUpExitDoesNotCloseTheReplacementAttackWindow(bool slayer)
    {
        var weapon = new GameObject("replacement window", typeof(BoxCollider2D));
        weapon.transform.SetParent(player.transform);
        attacks.hitbox = weapon.AddComponent<PlayerWeaponHitbox>();
        animator.Play("Base Layer.parry_followup", 0, 0); animator.Update(0); animator.Update(.151f);
        var leaving = animator.GetCurrentAnimatorStateInfo(0);
        var exit = animator.GetBehaviours<ParryFollowUpExit>().Single();
        int previous = Get<int>(attacks, "currentAttackId");
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.True);
        if (slayer)
        {
            // PlayerSlayerMode.OnCorrectChar opens/closes through code immediately
            // after requesting CrossFade, before the old state's exit callback.
            animator.CrossFadeInFixedTime("Base Layer.player_slayer.attackS1", .02f, 0, 0);
            attacks.OnAttackStart(); attacks.OnAttackEnd();
        }
        else
        {
            typing.ProcessInput("type");
            var normal = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Characters/Player/attack1.anim");
            attacks.OnAttackStartCel((Sprite)normal.events.Single(e => e.functionName == "OnAttackStartCel").objectReferenceParameter);
        }
        Assert.That(Get<int>(attacks, "currentAttackId"), Is.GreaterThan(previous), "replacement opens its own window");
        var pendingClose = Get<Coroutine>(attacks, "closeCo");
        var followUp = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Characters/Player/parry_followup.anim");
        attacks.OnParryFollowUpEnd((Sprite)followUp.events[0].objectReferenceParameter);
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.True, "late old clip-end event cannot cancel a newer swing");
        exit.OnStateExit(animator, leaving, 0);
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.True, "old follow-up exit cannot cancel a newer swing");
        Assert.That(Get<Coroutine>(attacks, "closeCo"), Is.SameAs(pendingClose), "replacement keeps its remaining window time");
    }

    [Test, Category("Quick")]
    public void FollowUpClosesAt250MillisecondsAndExitDoesNotNotifyAgain()
    {
        var weapon = new GameObject("timed follow-up", typeof(BoxCollider2D));
        weapon.transform.SetParent(player.transform);
        attacks.hitbox = weapon.AddComponent<PlayerWeaponHitbox>();
        int closed = 0; attacks.OnWindowClose += () => closed++;
        animator.Play("Base Layer.parry_followup", 0, 0); animator.Update(0); animator.Update(.151f);
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.True);
        animator.Update(.1f);
        Assert.That(Get<bool>(attacks.hitbox, "active"), Is.False);
        Assert.That(closed, Is.EqualTo(1));
        animator.Update(.4f);
        Assert.That(closed, Is.EqualTo(1));
    }

    [Test, Category("Quick")]
    public void FollowUpAuthoringHasFullComboAndNormalAttackIsUnchanged()
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Characters/Player/parry_followup.anim");
        var keys = AnimationUtility.GetObjectReferenceCurve(clip, AnimationUtility.GetObjectReferenceCurveBindings(clip).Single());
        Assert.That(keys.Length, Is.EqualTo(11), "ten cels plus a repeated terminal hold key");
        for (int i = 0; i < 10; i++)
        {
            Assert.That(keys[i].time, Is.EqualTo(i < 6 ? i * .05f : .3f + (i - 6) / 15f).Within(.00001f));
            Assert.That(keys[i].value.name, Is.EqualTo(i < 6 ? "male_hero-combo_2_" + i : "male_hero-combo_2_end_" + (i - 6)));
        }
        Assert.That(keys[10].value, Is.SameAs(keys[9].value));
        Assert.That(keys[10].time, Is.EqualTo(33f / 60f).Within(.00001f));
        Assert.That(clip.length, Is.EqualTo(34f / 60f).Within(.00001f));
        Assert.That(clip.events.Select(e => e.time), Is.EqualTo(new[] { .15f, .2f, .25f }));
        Assert.That(clip.events[0].objectReferenceParameter, Is.SameAs(keys[3].value));
        Assert.That(clip.events[1].objectReferenceParameter, Is.SameAs(keys[4].value));
        Assert.That(clip.events[2].objectReferenceParameter, Is.SameAs(keys[3].value));
        var authored = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Characters/Player/AC_Controller.controller");
        var state = authored.layers[0].stateMachine.states.Single(s => s.state.name == "parry_followup").state;
        Assert.That(state.behaviours.OfType<ParryFollowUpExit>().Single().openingCel, Is.SameAs(keys[3].value));
        Assert.That(controller.parryFollowUp.startTime, Is.Zero);
        Assert.That(controller.parryFollowUp.crossfade, Is.EqualTo(.02f));
        typing.ProcessInput("type"); animator.Update(.03f);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, Is.EqualTo(Animator.StringToHash("Base Layer.attack1")));
    }

    [Test, Category("Quick")]
    public void MainContactSnapshotCannotReplaceTheOriginalEntryPose()
    {
        var library = AssetDatabase.LoadAssetAtPath<ParryContactLibrary>("Assets/Art/VFX/ParryClash/ParryContacts.asset");
        Assert.That(library, Is.Not.Null);
        Assert.That(library.frames.Where(frame => frame.defense).Select(frame => frame.sprite), Is.EqualTo(new[] { sprites[0] }));
        Assert.That(library.frames.Where(frame => !frame.defense).All(frame => frame.defenseCel == sprites[0]), Is.True);
        Set(parry, "contactLibrary", library);
        Begin(true);
        typeof(ParrySystem).GetMethod("LateUpdate", Private).Invoke(parry, null);
        Assert.That(renderer.sprite, Is.SameAs(sprites[0]), "main contact hold must not restore frame0 or frame1");
    }

    [Test, Category("Quick")]
    public void SuccessRecoveryWithoutAWordNeverOpensAnAttackWindow()
    {
        var weapon = new GameObject("unopened follow-up hitbox", typeof(BoxCollider2D));
        weapon.transform.SetParent(player.transform);
        var box = weapon.AddComponent<PlayerWeaponHitbox>(); box.targetLayers = 1; attacks.hitbox = box;
        target = new GameObject("unattacked target"); var damage = target.AddComponent<FeedbackDamageTarget>();
        var collider = target.AddComponent<BoxCollider2D>();
        var hit = typeof(PlayerWeaponHitbox).GetMethod("OnTriggerEnter2D", Private);
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Original + "player_parry_success.anim");
        Assert.That(clip.events.Any(e => e.functionName.StartsWith("OnAttack")), Is.False);
        Begin(true);
        Advance(.2f); hit.Invoke(box, new object[] { collider });
        Advance(.6f); hit.Invoke(box, new object[] { collider });
        Advance(.151f); animator.Update(0); hit.Invoke(box, new object[] { collider });
        Assert.That(damage.hits, Is.Zero);
        Assert.That(Get<bool>(controller, "pendingAttack"), Is.False);
        Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"), Is.False);
    }

}
