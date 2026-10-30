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

    [Test, Category("Quick")]
    public void HeadAnchorTracksAuthoredSkullAndCameraViewportWithoutFollowingWeaponOrColliderBounds()
    {
        var canvas = new GameObject("Head cue canvas", typeof(RectTransform), typeof(Canvas));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var key = new GameObject("Head cue key", typeof(RectTransform)).GetComponent<RectTransform>();
        key.SetParent(canvas.transform, false); key.pivot = new Vector2(0, 1); key.sizeDelta = new Vector2(96, 48);
        var host = new GameObject("Head cue presenter"); host.SetActive(false);
        var presenter = host.AddComponent<ParryCuePresenter>(); Set(presenter, "key", key);
        var camera = new GameObject("Head cue camera").AddComponent<Camera>();
        camera.orthographic = true; camera.orthographicSize = 5; camera.transform.position = new Vector3(0, 0, -10);
        var actor = new GameObject("Head cue enemy");
        var body = actor.AddComponent<BoxCollider2D>(); body.size = new Vector2(2, 3);
        actor.transform.position = new Vector3(2, -.5f, 0); actor.transform.localScale = new Vector3(-1, 1, 1);
        var renderer = actor.AddComponent<SpriteRenderer>();
        var texture = new Texture2D(20, 4);
        var small = Sprite.Create(texture, new Rect(0, 0, 4, 4), Vector2.one * .5f, 1);
        var wide = Sprite.Create(texture, new Rect(0, 0, 20, 4), Vector2.one * .5f, 1);
        var landmark = new Vector2(-.1f, 1.235f);
        Set(presenter, "enemyTransform", actor.transform); Set(presenter, "headLocal", landmark); Set(presenter, "viewCamera", camera);
        void Verify()
        {
            Physics2D.SyncTransforms();
            Call(presenter, "PositionKey");
            var expected = CombatViewportCamera.WorldToScreenPoint(camera,
                actor.transform.TransformPoint(landmark));
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, expected, null, out var head);
            var corners = new Vector3[4]; key.GetWorldCorners(corners);
            var bottom = canvas.transform.InverseTransformPoint((corners[0] + corners[3]) * .5f);
            Assert.That(bottom.x, Is.EqualTo(head.x).Within(1.51f), "key centered above enemy within one logical-pixel snap");
            Assert.That(bottom.y - head.y, Is.EqualTo(24).Within(1.51f), "eight logical pixels clear the raised axe blade");
        }
        try
        {
            Canvas.ForceUpdateCanvases(); renderer.sprite = small; Verify(); var before = key.position;
            renderer.sprite = wide; Verify(); Assert.That(key.position, Is.EqualTo(before), "weapon width cannot shift cue");
            body.offset = new Vector2(3, 2); Verify(); Assert.That(key.position, Is.EqualTo(before), "collider offset cannot shift skull cue");
            actor.transform.localScale = Vector3.one; Verify();
            actor.transform.position += new Vector3(1, .5f, 0); Verify();
            Assert.That(key.position, Is.Not.EqualTo(before), "follows current enemy");
            camera.rect = new Rect(.1f, .2f, .8f, .6f); Verify();
        }
        finally
        {
            Object.DestroyImmediate(host); Object.DestroyImmediate(canvas); Object.DestroyImmediate(camera.gameObject);
            Object.DestroyImmediate(actor); Object.DestroyImmediate(small); Object.DestroyImmediate(wide); Object.DestroyImmediate(texture);
        }
    }

    [System.Serializable]
    sealed class ReviewFrame
    {
        public float time, remaining, normalized, speed;
        public bool pending, attacking;
        public int health;
        public string sprite, asset;
        public Rect rect;
        public Vector2 pivot;
    }
    [System.Serializable]
    sealed class ReviewClip { public int variant; public ReviewFrame[] frames; }

    [UnityTest, Category("Quick")]
    public IEnumerator CaptureEveryFrameOfSevenPreparedAttacks()
    {
        int[] encounters = { -1, 0, 12, 2, 6, 3, 3 };
        int previousRate = Time.captureFramerate;
        Time.captureFramerate = 25;
        try
        {
            for (int test = 0; test < encounters.Length; test++)
            {
                var receiver = new GameObject("Review receiver").AddComponent<PlayerHealth>();
                receiver.maxHealth = 40; receiver.FullRestore();
                var host = new GameObject("Review attack", typeof(SpriteRenderer), typeof(Animator)); host.SetActive(false);
                var animator = host.GetComponent<Animator>();
#if UNITY_EDITOR
                animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
                var strike = host.AddComponent<AttackStrike>(); Set(strike, "damageReceiver", receiver);
                var anchor = host.AddComponent<EnemyHitAnchor>();
                Set(anchor, "body", host.GetComponent<SpriteRenderer>());
#if UNITY_EDITOR
                Set(anchor, "profile", UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyHitAnchorProfile>("Assets/Art/EnemyHitAnchors/ske.asset"));
#endif
                var brain = host.AddComponent<SkeletonController>(); Set(brain, "animator", animator);
                var frames = new System.Collections.Generic.List<ReviewFrame>();
                try
                {
                    host.SetActive(true);
                    if (test > 0)
                    {
                        brain.ConfigureEndless(new EndlessRun { Defeated = encounters[test] });
                        if (test == 2 || test == 6) Set(brain, "pattern", 1);
                    }
                    animator.Update(0);
                    float start = Time.time;
                    int idleFrames = 0;
                    for (int frame = 0; frame < 130; frame++)
                    {
                        var sprite = host.GetComponent<SpriteRenderer>().sprite;
                        var row = new ReviewFrame { time = Time.time - start, speed = animator.speed,
                            normalized = animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
                            pending = brain.TryGetAttackTelegraph(out float remaining), remaining = remaining,
                            attacking = brain.IsAttacking, health = receiver.Current,
                            sprite = sprite ? sprite.name : "", rect = sprite ? sprite.rect : default,
                            pivot = sprite ? sprite.pivot : default };
#if UNITY_EDITOR
                        row.asset = sprite ? UnityEditor.AssetDatabase.GetAssetPath(sprite) : "";
#endif
                        frames.Add(row);
                        if (frame > 2 && !brain.IsAttacking && ++idleFrames >= 3) break;
                        if (frame == 0) { Set(brain, "_nextAttackAt", -1f); Call(brain, "Update"); }
                        yield return null;
                    }
                    Assert.That(receiver.Current, Is.LessThan(40));
                    Assert.That(brain.IsAttacking, Is.False, "recovery must end");
                    string root = System.IO.Directory.Exists("/results") ? "/results" : Application.temporaryCachePath;
                    System.IO.File.WriteAllText(System.IO.Path.Combine(root, "attack-review-" + test + ".json"),
                        JsonUtility.ToJson(new ReviewClip { variant = test, frames = frames.ToArray() }, true));
                }
                finally { Object.DestroyImmediate(host); Object.DestroyImmediate(receiver.gameObject); }
            }
        }
        finally { Time.captureFramerate = previousRate; }
    }

    [UnityTest, Category("Quick")]
    public IEnumerator FilledKeyMatchesManualParryAndEarlyWarningDoesNotBufferSpace()
    {
        int[] encounters = { -1, 0, 12, 2, 6, 3, 3 };
        foreach (float pressLead in new[] { .8f, .18f })
        for (int test = 0; test < encounters.Length; test++)
        {
            var player = new GameObject("Manual defense", typeof(SpriteRenderer), typeof(Animator)); player.SetActive(false);
            var playerAnimator = player.GetComponent<Animator>();
            var health = player.AddComponent<PlayerHealth>(); health.maxHealth = 40;
            var gate = player.AddComponent<PlayerInputGate>();
            var parry = player.AddComponent<ParrySystem>(); parry.logs = false;
            parry.gate = gate; parry.playerAnimator = playerAnimator;
#if UNITY_EDITOR
            playerAnimator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Player/AC_Controller.controller");
            parry.config = UnityEditor.AssetDatabase.LoadAssetAtPath<ParryConfig>("Assets/Data/Attack/PC_Parry.asset");
#endif
            player.AddComponent<PlayerAttackEvents>(); player.AddComponent<SfxOnParryPose>();
            var actor = new GameObject("Warned attacker", typeof(SpriteRenderer), typeof(Animator)); actor.SetActive(false);
            var animator = actor.GetComponent<Animator>();
#if UNITY_EDITOR
            animator.runtimeAnimatorController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Characters/Enermy/AC_Skeleton.controller");
#endif
            var strike = actor.AddComponent<AttackStrike>(); Set(strike, "damageReceiver", health);
            var anchor = actor.AddComponent<EnemyHitAnchor>();
            Set(anchor, "body", actor.GetComponent<SpriteRenderer>());
#if UNITY_EDITOR
            Set(anchor, "profile", UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyHitAnchorProfile>("Assets/Art/EnemyHitAnchors/ske.asset"));
#endif
            var brain = actor.AddComponent<SkeletonController>(); Set(brain, "animator", animator);
            var target = actor.AddComponent<ParryTarget>(); target.skeleton = brain; target.damageable = brain;
            parry.enemy = target;
            int successes = 0; parry.OnParrySuccess += _ => successes++;
            try
            {
                player.SetActive(true); health.FullRestore(); actor.SetActive(true);
                if (test > 0)
                {
                    brain.ConfigureEndless(new EndlessRun { Defeated = encounters[test] });
                    if (test == 2 || test == 6) Set(brain, "pattern", 1);
                }
                Set(brain, "_nextAttackAt", -1f); Call(brain, "Update");
                float deadline = Time.realtimeSinceStartup + 5;
                while (brain.TryGetAttackTelegraph(out float remaining) && remaining > pressLead && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(parry.TryBeginParry(), Is.True, $"case {test}, lead {pressLead}");
                Assert.That(successes, Is.Zero, "opening the pose is not a premature success");
                while (brain.TryGetAttackTelegraph(out _) && Time.realtimeSinceStartup < deadline) yield return null;
                bool filled = pressLead < .2f;
                Assert.That(successes, Is.EqualTo(filled ? 1 : 0), "only the real contact inside the manual window succeeds");
                Assert.That(health.Current == 40, Is.EqualTo(filled), "outline-phase Space expires, filled-phase Space protects HP");
                strike.Anim_DealDamage(); Assert.That(successes, Is.EqualTo(filled ? 1 : 0), "spent strike cannot resolve twice");
                if (filled && test == 1)
                {
                    deadline = Time.realtimeSinceStartup + 7;
                    while (health.Current == 40 && Time.realtimeSinceStartup < deadline) yield return null;
                    Assert.That(health.Current, Is.LessThan(40), "the next attack requires another press");
                    Assert.That(successes, Is.EqualTo(1));
                }
            }
            finally { Object.DestroyImmediate(actor); Object.DestroyImmediate(player); }
        }
    }

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
            var anchor = host.AddComponent<EnemyHitAnchor>();
            Set(anchor, "body", host.GetComponent<SpriteRenderer>());
#if UNITY_EDITOR
            Set(anchor, "profile", UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyHitAnchorProfile>("Assets/Art/EnemyHitAnchors/ske.asset"));
#endif
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
                Assert.That(before, Is.InRange(.98f, 1.02f));
                GamePauseController.SetEncounterReading(true);
                yield return new WaitForSecondsRealtime(.1f);
                Assert.That(brain.TryGetAttackTelegraph(out float paused), Is.True);
                Assert.That(paused, Is.EqualTo(before).Within(.002f));
                GamePauseController.SetEncounterReading(false);
                // Once preparation releases, countdown follows the Animator speed.
                while (animator.speed == 0 && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(brain.TryGetAttackTelegraph(out float moving), Is.True);
                if (animator.speed > 0)
                {
                    animator.speed = 2;
                    Assert.That(brain.TryGetAttackTelegraph(out float fast), Is.True);
                    Assert.That(fast, Is.EqualTo(moving / 2).Within(.002f));
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

    [Test, Category("Quick")]
    public void RebindingEncountersRestoresOldCueAndRetainsOnlyCurrentGraphics()
    {
        var host = new GameObject("Presenter lifecycle"); host.SetActive(false);
        var parry = host.AddComponent<ParrySystem>();
        var presenter = host.AddComponent<ParryCuePresenter>();
        var key = new GameObject("Key", typeof(RectTransform));
        Set(presenter, "parry", parry); Set(presenter, "key", key.GetComponent<RectTransform>());
        var mode = typeof(ParryCuePresenter).GetField("useKey", BindingFlags.Static | BindingFlags.NonPublic);
        bool previousMode = (bool)mode.GetValue(null);
        GameObject actor = null;
        UnityEngine.UI.Image previousGraphic = null;
        try
        {
            mode.SetValue(null, true);
            for (int encounter = 0; encounter < 20; encounter++)
            {
                var oldActor = actor;
                actor = new GameObject("Encounter " + encounter);
                var brain = actor.AddComponent<SkeletonController>();
                var cue = new GameObject("Cue", typeof(RectTransform), typeof(UnityEngine.UI.Image));
                cue.transform.SetParent(actor.transform);
                var graphic = cue.GetComponent<UnityEngine.UI.Image>();
                Set(brain, "attackCue", cue);
                parry.enemy = actor.AddComponent<ParryTarget>();
                Call(presenter, "LateUpdate");
                Assert.That(graphic.enabled, Is.False, "new mode suppresses current legacy cue");
                if (previousGraphic) Assert.That(previousGraphic.enabled, Is.True, "old actor restored on rebind");
                var retained = (System.Collections.IDictionary)presenter.GetType().GetField("legacyGraphics", Flags).GetValue(presenter);
                Assert.That(retained.Count, Is.EqualTo(1), "do not retain prior encounter graphics");
                if (oldActor) Object.DestroyImmediate(oldActor);
                previousGraphic = graphic;
            }
            Call(presenter, "OnDisable");
            Assert.That(previousGraphic.enabled, Is.True, "disable restores current actor");
        }
        finally
        {
            mode.SetValue(null, previousMode);
            Object.DestroyImmediate(host); Object.DestroyImmediate(key);
            if (actor) Object.DestroyImmediate(actor);
        }
    }

}
