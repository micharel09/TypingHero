#if TYPINGHERO_AUTOMATION
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Observation only: no command dispatcher, setters, or input injection.
[DefaultExecutionOrder(1000)]
public sealed class GameLoopProbe : MonoBehaviour
{
    [Serializable] sealed class SortingFrame
    {
        public int order, enemyOrder, attackMask, parryMask, mismatches, frame;
        public string state, sprite;
        public bool attack, parry;
    }
    [Serializable] sealed class SparkFrame
    {
        public string role, preset;
        public int id, frame, order;
        public Vector3 position, scale;
        public Rect screenRect;
        public int particles;
        public float groundY, minY, maxY, maxAge, meanVelocityX, meanDepthSpeed;
        public float contactGap;
        public string defenseCel, attackCel;
    }
    readonly ParticleSystem.Particle[] observedParticles = new ParticleSystem.Particle[56];
    ProfilerRecorder frameAllocations;
    Animator sortingAnimator;
    SpriteRenderer sortingSprite, sortingEnemy;
    SortingFrame sortingFrame;
    int attackMask, parryMask, sortingMismatches, sparkMask;
    SparkFrame[] sparks = Array.Empty<SparkFrame>();
    static readonly string[] SortingStates = { "Base Layer.attack1", "Base Layer.player_slayer.attackS1",
        "Base Layer.player_slayer.attackS2", "Base Layer.player_slayer.attackS3",
        "Base Layer.player_parry", "Base Layer.player_parry_success" };
    static readonly string[] SparkRoles = { "Normal", "Parry", "Lethal", "Slayer1", "Slayer2", "Slayer3", "Slayer4", "Slayer" };

    void LateUpdate()
    {
        if (!sortingAnimator || !sortingSprite) return;
        var info = sortingAnimator.IsInTransition(0) ? sortingAnimator.GetNextAnimatorStateInfo(0)
            : sortingAnimator.GetCurrentAnimatorStateInfo(0);
        bool attack = info.IsTag("Attack");
        bool parry = info.IsTag("Parry");
        string name = "other";
        for (int i = 0; i < SortingStates.Length; i++)
            if (info.fullPathHash == Animator.StringToHash(SortingStates[i]))
            {
                name = SortingStates[i];
                if (i < 4) attackMask |= 1 << i;
                else parryMask |= 1 << (i - 4);
            }
        if (Time.timeScale != 0 && sortingSprite.sortingOrder != (attack || parry ? 1 : 0)) sortingMismatches++;
        sortingFrame = new SortingFrame { order = sortingSprite.sortingOrder,
            enemyOrder = sortingEnemy ? sortingEnemy.sortingOrder : 0, attack = attack, parry = parry,
            state = name, sprite = sortingSprite.sprite ? sortingSprite.sprite.name : "",
            attackMask = attackMask, parryMask = parryMask, mismatches = sortingMismatches, frame = Time.frameCount };
        var camera = Camera.main;
        sparks = FindObjectsOfType<PooledVFX>().Select(effect =>
        {
            string role = effect.name.Replace("(Clone)", "");
            int index = Array.IndexOf(SparkRoles, role);
            if (index >= 0) sparkMask |= 1 << index;
            var sprite = effect.GetComponent<SpriteRenderer>();
            var particle = effect.GetComponentInChildren<ParticleSystemRenderer>();
            if (!sprite && !particle) return null; // A renderer may already be gone during scene teardown.
            var bounds = sprite ? sprite.bounds : particle.bounds;
            var burst = effect.GetComponent<PixelSparkBurst>();
            var clash = effect.GetComponent<ParryClashBurst>();
            var voxel = effect.GetComponent<VoxelImpactBurst>();
            var accent = effect.GetComponent<StrikeAccentBurst>();
            if (burst && (!burst.Systems[0] || !burst.Systems[1])) return null;
            float minY = float.PositiveInfinity, maxY = float.NegativeInfinity, maxAge = 0;
            float velocityX = 0, depthSpeed = 0; int observedCount = 0;
            var systems = burst ? burst.Systems : clash ? clash.Systems : voxel ? voxel.Systems : null;
            if (systems != null)
                foreach (var ps in systems)
                {
                    var renderer = ps.GetComponent<ParticleSystemRenderer>();
                    if (!renderer) continue;
                    bounds.Encapsulate(renderer.bounds);
                    int count = ps.GetParticles(observedParticles);
                    observedCount += count;
                    for (int p = 0; p < count; p++)
                    {
                        minY = Mathf.Min(minY, observedParticles[p].position.y);
                        maxY = Mathf.Max(maxY, observedParticles[p].position.y);
                        maxAge = Mathf.Max(maxAge, observedParticles[p].startLifetime - observedParticles[p].remainingLifetime);
                        velocityX += observedParticles[p].velocity.x;
                        depthSpeed += Mathf.Abs(observedParticles[p].velocity.z);
                    }
                }
            var min = camera ? camera.WorldToScreenPoint(bounds.min) : Vector3.zero;
            var max = camera ? camera.WorldToScreenPoint(bounds.max) : Vector3.zero;
            return new SparkFrame { role = role, preset = clash ? clash.EmittedPreset.ToString() : "", id = effect.GetInstanceID(), frame = Time.frameCount,
                order = sprite ? sprite.sortingOrder : particle.sortingOrder,
                position = effect.transform.position, scale = effect.transform.lossyScale,
                contactGap = clash ? clash.Visual.separation : 0,
                defenseCel = clash && clash.Visual.defenseCel ? clash.Visual.defenseCel.name : "",
                attackCel = clash && clash.Visual.attackCel ? clash.Visual.attackCel.name : "",
                particles = burst ? burst.ParticleCount : clash ? clash.ParticleCount : voxel ? voxel.ParticleCount : accent ? accent.ParticleCount : 0, groundY = burst ? burst.GroundY : 0,
                minY = observedCount > 0 ? minY : 0, maxY = observedCount > 0 ? maxY : 0, maxAge = accent ? accent.Age : maxAge,
                meanVelocityX = observedCount > 0 ? velocityX / observedCount : 0,
                meanDepthSpeed = observedCount > 0 ? depthSpeed / observedCount : 0,
                screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y) };
        }).Where(frame => frame != null).ToArray();
    }
    [Serializable] sealed class State
    {
        public bool endless, encounterReading;
        public int endlessRound, endlessSlot, defeated;
        public string opponent;
        public string runDifficulty, menuDifficulty, tableDifficulty;
        public float difficultyPace, recoverySeconds, comboWindowSeconds, comboRemaining, gameplayClock;
        public float unscaledClock, enemyNextAttackAt;
        public int enemyChainRemaining;
        public bool hitLocked, deadLocked, typingMuted;
        public int playerCurrentState, playerNextState;
        public float playerAnimatorTime, playerAnimatorSpeed, playerStateSpeed;
        public bool playerHitStateActive, playerAnimatorTransition, playerAnimatorEnabled;
        public SortingFrame playerSorting;
        public SparkFrame[] sparks;
        public int sparkMask;
        public string lastError;
        public string scene, current, next, playerName;
        public int frame, score, hp, maxHp, enemyHp, words, chars, parries, errors, objects;
        public bool slayer, parryWindow, canParry, enemyAttacking, overlay, paused;
        public bool finished, cue, pendingAttack, helpOpen, helpSeen, parryPose;
        public float combatSeconds, musicVolume, sfxVolume;
        public int inputLetters, inputCorrect, matchParries, finalScore, stamina, nextProgress;
        public string result;
        public string enemySprite, enemyTexture;
        public float enemyAnimationTime, frameMs, scroll, timeScale, wordTime, rowError, rowSlide;
        public float enemyContactIn, enemyAnimationSpeed;
        public int activeSlashParts;
        public long memory;
        public long allocatedBytes;
        public bool allocationCounterAvailable;
        public int rows, dataRows, pool, firstVisible, width, height;
        public bool virtualized;
        public bool leaderboardOpen;
        public int collectionMode;
        public string collectionTitle, collectionDetails, collectionStatus, selectedControl, collectionFirstRow;
        public bool tutorial, tutorialPracticing, tutorialPassed, tutorialRetry, stunned;
        public int tutorialLesson, tutorialStatus;
        public float cameraOrtho, multiplier, scorePulse, scoreZoom, redAlpha;
        public Color scoreColor;
        public bool environmentVisible, hudPanelVisible;
        public Vector2 barTopEdge, barBottomEdge, scorePoint, trayPoint, bodyHead, bodyFoot;
        public float barLeft, barRight;
        public Rect scoreGlyphRect, comboGlyphRect, comboDecayRect;
        public bool comboVisible, comboDecayVisible, comboUnderScore, comboDecayUnderScore;
        public float comboDecayValue;
        public float scoreFont, comboFont, barReveal;
        public Rect playerStatusRect, enemyStatusRect, trayRect, scoreRect, multiplierRect, staminaRect;
        public Vector2 playerFootPoint, enemyFootPoint;
        public float playerHpFill, enemyHpFill, staminaFill;
        public Rect[] wordRects, wordGlyphRects;
        public Rect popupRect, decayRect;
        public Vector2 popupPoint;
        public float popupAlpha, popupFont, popupElapsed;
        public int popupCount, popupId;
        public string popupText;
        public Vector2 enemyHeadPoint;
        public bool playerNameVisible, enemyNameVisible;
        public Color staminaColor;
        public string encounterText, staminaIconName;
        public bool encounterVisible, encounterBackgroundVisible, guardBreakVisible, combatHeadingVisible;
        public Color parryKeyColor, hpNumberColor, hpFrameColor, guardBreakColor;
        public float hudHitRemaining, hudSuccessRemaining;
        public Rect guardBreakRect, encounterRect;

        public float rowHeight, viewportHeight;
        public int[] rowIds;
        public Point[] controls;
    }
    static Rect ScreenRect(RectTransform rt)
    {
        if (!rt) return default;
        var min = rt.TransformPoint(new Vector3(rt.rect.xMin, rt.rect.yMin, 0));
        var max = rt.TransformPoint(new Vector3(rt.rect.xMax, rt.rect.yMax, 0));
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
    static Rect TextRect(TMP_Text text)
    {
        if (!text || !text.gameObject.activeInHierarchy) return default;
        var rt = text.rectTransform;
        float offset = text.alignment == TextAlignmentOptions.Center ? (rt.rect.width - text.preferredWidth) * .5f : 0;
        var min = rt.TransformPoint(new Vector3(rt.rect.xMin + offset, rt.rect.yMin, 0));
        var max = rt.TransformPoint(new Vector3(rt.rect.xMin + offset + text.preferredWidth, rt.rect.yMax, 0));
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
    static Rect GlyphRect(TMP_Text text)
    {
        if (!text || !text.gameObject.activeInHierarchy) return default;
        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        for (int i = 0; i < text.textInfo.characterCount; i++)
        {
            var c = text.textInfo.characterInfo[i];
            if (!c.isVisible) continue;
            var m = c.textElement.glyph.metrics;
            var topLeft = text.transform.TransformPoint(new Vector3(
                c.origin + m.horizontalBearingX * c.scale, c.baseLine + m.horizontalBearingY * c.scale, 0));
            var bottomRight = topLeft + text.transform.TransformVector(new Vector3(m.width, -m.height, 0) * c.scale);
            min = Vector2.Min(min, bottomRight); min = Vector2.Min(min, topLeft);
            max = Vector2.Max(max, bottomRight); max = Vector2.Max(max, topLeft);
        }
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
    [Serializable] sealed class Point { public string name, kind; public float x, y; }
    int words, chars, parries, errors;
    string lastError;
    ParrySystem observedParry;
    static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
    static T Read<T>(object target, string field, T fallback = default)
        => target == null ? fallback : (T)target.GetType().GetField(field, Fields).GetValue(target);
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void TypingHeroQA_Publish(string json);
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
        var go = new GameObject("TypingHero QA observation");
        DontDestroyOnLoad(go);
        go.AddComponent<GameLoopProbe>();
    }
    void OnEnable()
    {
        frameAllocations = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        TypingManager.OnWordAdvanced += Word;
        TypingManager.OnCorrectChar += Char;
        Application.logMessageReceived += Log;
        StartCoroutine(Publish());
    }
    void OnDisable()
    {
        frameAllocations.Dispose();
        TypingManager.OnWordAdvanced -= Word;
        TypingManager.OnCorrectChar -= Char;
        Application.logMessageReceived -= Log;
        if (observedParry) observedParry.OnParrySuccess -= Parry;
    }
    void Word() => words++;
    void Char(char _) => chars++;
    void Parry(ParrySystem.ParryContext _) => parries++;
    void Log(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        { errors++; lastError = message + "\n" + trace; }
    }
    IEnumerator Publish()
    {
        var wait = new WaitForSecondsRealtime(.1f);
        var rendered = new WaitForEndOfFrame();
        while (true)
        {
            yield return rendered;
            var typing = FindObjectOfType<TypingManager>();
            var health = FindObjectOfType<PlayerHealth>();
            var enemy = FindObjectOfType<SkeletonController>();
            var parry = FindObjectOfType<ParrySystem>();
            if (observedParry != parry)
            {
                if (observedParry) observedParry.OnParrySuccess -= Parry;
                observedParry = parry;
                if (observedParry) observedParry.OnParrySuccess += Parry;
            }
            var slayer = FindObjectOfType<PlayerSlayerMode>();
            var gate = FindObjectOfType<PlayerInputGate>();
            var pause = FindObjectOfType<GamePauseController>();
            var board = FindObjectOfType<LeaderboardUIBinder>();
            var pool = Read<System.Collections.Generic.List<LeaderboardRowView>>(board, "pool");
            var data = Read<System.Collections.Generic.List<FirebaseLeaderboard.Row>>(board, "_rows");
            var scroll = Read<ScrollRect>(board, "scrollRect");
            var animator = Read<Animator>(enemy, "animator");
            var info = animator ? animator.GetCurrentAnimatorStateInfo(0) : default;
            bool attacking = animator && info.fullPathHash == Animator.StringToHash(Read(enemy, "attackStatePath", ""));
            float contactIn = -1, animationSpeed = animator ? animator.speed * info.speed * info.speedMultiplier : 0;
            if (attacking)
                foreach (var clip in animator.GetCurrentAnimatorClipInfo(0))
                    foreach (var marker in clip.clip.events)
                        if (marker.functionName == "Anim_DealDamage")
                            contactIn = (marker.time - (info.normalizedTime % 1) * clip.clip.length) / Mathf.Max(.001f, animationSpeed);
            var controls = FindObjectsOfType<Selectable>().Where(s => s.IsInteractable()).Select(s =>
            {
                var canvas = s.GetComponentInParent<Canvas>();
                var rt = (RectTransform)s.transform;
                var p = RectTransformUtility.WorldToScreenPoint(
                    canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null,
                    rt.TransformPoint(rt.rect.center));
                return new Point { name = s.name, kind = s is TMP_InputField ? "input" : "button",
                    x = p.x / Screen.width, y = 1f - p.y / Screen.height };
            }).ToList();
            if (scroll && scroll.viewport)
            {
                var canvas = scroll.GetComponentInParent<Canvas>();
                var p = RectTransformUtility.WorldToScreenPoint(
                    canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null,
                    scroll.viewport.TransformPoint(scroll.viewport.rect.center));
                controls.Add(new Point { name = scroll.name, kind = "scroll", x = p.x / Screen.width,
                    y = 1f - p.y / Screen.height });
            }
            var match = CombatSession.I;
            var endless = EndlessDuel.I ? EndlessDuel.I.Run : null;
            var help = FindObjectOfType<HowToPlayPanel>();
            var cue = Read<GameObject>(enemy, "attackCue");
            var controller = FindObjectOfType<PlayerController>();
            if (controller && sortingAnimator != controller.animator)
            {
                sortingAnimator = controller.animator;
                sortingSprite = controller.GetComponent<SpriteRenderer>();
                attackMask = parryMask = sortingMismatches = sparkMask = 0;
                sortingFrame = null;
            }
            sortingEnemy = enemy ? enemy.GetComponent<SpriteRenderer>() : null;
            var stamina = FindObjectOfType<EnemyStamina>();
            var resultLabel = Read<TMP_Text>(match, "resultLabel");
            var tutorial = FindObjectOfType<TutorialDirector>();
            var fx = FindObjectOfType<SlayerScreenFX>();
            var scoreLabel = Read<TMP_Text>(ScoreSystem.I, "scoreText");
            var scoreRoot = Read<RectTransform>(fx, "scoreRoot");
            var topBar = Read<RectTransform>(fx, "topBar");
            var bottomBar = Read<RectTransform>(fx, "bottomBar");
            var bodyA = Read<BoxCollider2D>(fx, "playerBody");
            var bodyB = Read<BoxCollider2D>(fx, "enemyBody");
            var enemyRenderer = animator ? animator.GetComponent<SpriteRenderer>() : null;
            var layout = FindObjectOfType<CombatHUDLayout>();
            var playerStatus = Read<RectTransform>(layout, "playerStatus");
            var enemyStatus = Read<RectTransform>(layout, "enemyStatus");
            var multiplierLabel = Read<TMP_Text>(layout, "multiplierText");
            var comboHUD = Read<ScoreUI_ComboHUD>(ScoreSystem.I, "comboHUD");
            var comboDecay = Read<Slider>(comboHUD, "decaySlider");
            var playerHpSlider = playerStatus ? playerStatus.GetComponentInChildren<Slider>() : null;
            var enemyHpSlider = enemyStatus ? enemyStatus.Find("EnemyHP").GetComponent<Slider>() : null;
            var staminaSlider = enemyStatus ? enemyStatus.Find("EnemyStamina").GetComponent<Slider>() : null;
            var hudFeedback = FindObjectOfType<CombatHUDFeedback>();
            var hudRound = Read<TMP_Text>(FindObjectOfType<CombatR7Status>(), "round");
            var hudBackground = Read<Image>(FindObjectOfType<CombatR7Status>(), "roundBackground");
            var guardLabel = Read<TMP_Text>(hudFeedback, "guardBreak");
            var guardIcon = Read<Image>(hudFeedback, "staminaIcon");
            var combatHeading = Read<TMP_Text>(FindObjectOfType<EndlessOverlay>(), "heading");
            var env = Read<Transform>(fx, "environment");
            var red = Read<SpriteRenderer>(fx, "redOverlay");
            var hudImage = Read<Image>(fx, "hudPanel");
            var tray = Read<RectTransform>(fx, "typingTray");
            var award = FindObjectOfType<FloatingScore>();
            var awardLabel = award ? award.GetComponentInChildren<TMP_Text>() : null;
            var playerInfo = sortingAnimator ? sortingAnimator.GetCurrentAnimatorStateInfo(0) : default;
            bool playerTransition = sortingAnimator && sortingAnimator.IsInTransition(0);
            int playerNext = playerTransition ? sortingAnimator.GetNextAnimatorStateInfo(0).fullPathHash : 0;
            int hitState = health ? Animator.StringToHash(health.hitStatePath) : 0;
            var state = new State
            {
                playerSorting = sortingFrame,
                playerCurrentState = playerInfo.fullPathHash, playerNextState = playerNext,
                playerAnimatorTime = playerInfo.normalizedTime,
                playerAnimatorSpeed = sortingAnimator ? sortingAnimator.speed : 0,
                playerStateSpeed = playerInfo.speed * playerInfo.speedMultiplier,
                playerAnimatorTransition = playerTransition,
                playerAnimatorEnabled = sortingAnimator && sortingAnimator.isActiveAndEnabled,
                playerHitStateActive = hitState != 0 && (playerInfo.fullPathHash == hitState || playerNext == hitState),
                sparks = sparks, sparkMask = sparkMask, lastError = lastError,
                popupRect = TextRect(awardLabel), popupAlpha = awardLabel ? awardLabel.color.a : 0,
                popupFont = awardLabel ? awardLabel.fontSize : 0,
                popupElapsed = award ? Read<float>(award, "_elapsed") : 0,
                popupCount = FindObjectsOfType<FloatingScore>().Length,
                popupId = award ? award.GetInstanceID() : 0,
                popupText = awardLabel ? awardLabel.text : "",
                enemyHeadPoint = layout && bodyB ? (Vector2)Camera.main.WorldToScreenPoint(layout.Head(bodyB)) : default,
                playerNameVisible = playerStatus && playerStatus.Find("PlayerName").gameObject.activeInHierarchy,
                enemyNameVisible = enemyStatus && enemyStatus.Find("EnemyHPLabel").gameObject.activeInHierarchy,
                encounterText = hudRound ? hudRound.text : "",
                encounterVisible = hudRound && hudRound.enabled && hudRound.gameObject.activeInHierarchy,
                encounterBackgroundVisible = hudBackground && hudBackground.enabled && hudBackground.gameObject.activeInHierarchy,
                encounterRect = TextRect(hudRound), guardBreakRect = TextRect(guardLabel),
                combatHeadingVisible = combatHeading && combatHeading.gameObject.activeInHierarchy,
                staminaIconName = guardIcon && guardIcon.sprite ? guardIcon.sprite.name : "",
                guardBreakVisible = guardLabel && guardLabel.enabled,
                guardBreakColor = guardLabel ? guardLabel.color : default,
                parryKeyColor = Read<Image>(hudFeedback, "parryKey") ? Read<Image>(hudFeedback, "parryKey").color : default,
                hpNumberColor = Read<TMP_Text>(hudFeedback, "playerHp") ? Read<TMP_Text>(hudFeedback, "playerHp").color : default,
                hpFrameColor = Read<Image>(hudFeedback, "hpFrame") ? Read<Image>(hudFeedback, "hpFrame").color : default,
                hudHitRemaining = Read<float>(hudFeedback, "hitRemaining"),
                hudSuccessRemaining = Read<float>(hudFeedback, "successRemaining"),
                staminaColor = staminaSlider ? staminaSlider.fillRect.GetComponent<Image>().color : default,
                popupPoint = ScreenRect(Read<RectTransform>(layout, "popupAnchor")).position,
                decayRect = ScreenRect(Read<RectTransform>(layout, "comboDecay")),
                playerStatusRect = ScreenRect(playerStatus), enemyStatusRect = ScreenRect(enemyStatus),
                scoreGlyphRect = GlyphRect(scoreLabel), comboGlyphRect = GlyphRect(multiplierLabel),
                comboDecayRect = ScreenRect(Read<RectTransform>(layout, "comboDecay")),
                comboVisible = multiplierLabel && multiplierLabel.gameObject.activeInHierarchy,
                comboDecayVisible = comboDecay && comboDecay.gameObject.activeInHierarchy,
                comboUnderScore = comboHUD && scoreRoot && comboHUD.transform.IsChildOf(scoreRoot),
                comboDecayUnderScore = comboDecay && scoreRoot && comboDecay.transform.IsChildOf(scoreRoot),
                comboDecayValue = comboDecay ? comboDecay.value : 0,
                scoreFont = scoreLabel ? scoreLabel.fontSize : 0,
                comboFont = multiplierLabel ? multiplierLabel.fontSize : 0,
                barReveal = fx ? fx.BarReveal : 0,
                trayRect = ScreenRect(tray), scoreRect = TextRect(scoreLabel), multiplierRect = TextRect(multiplierLabel),
                staminaRect = staminaSlider ? ScreenRect((RectTransform)staminaSlider.transform) : default,
                playerFootPoint = layout && bodyA ? (Vector2)Camera.main.WorldToScreenPoint(layout.Foot(bodyA)) : default,
                enemyFootPoint = layout && bodyB ? (Vector2)Camera.main.WorldToScreenPoint(layout.Foot(bodyB)) : default,
                playerHpFill = playerHpSlider ? playerHpSlider.normalizedValue : -1,
                enemyHpFill = enemyHpSlider ? enemyHpSlider.normalizedValue : -1,
                staminaFill = staminaSlider ? staminaSlider.normalizedValue : -1,
                wordRects = new[] { TextRect(Read<TMP_Text>(typing, "wordText")), TextRect(Read<TMP_Text>(typing, "nextWordText")), TextRect(Read<TMP_Text>(typing, "thirdWordText")) },
                wordGlyphRects = new[] { GlyphRect(Read<TMP_Text>(typing, "wordText")), GlyphRect(Read<TMP_Text>(typing, "nextWordText")), GlyphRect(Read<TMP_Text>(typing, "thirdWordText")) },
                multiplier = ScoreSystem.I ? ScoreSystem.I.Multiplier : 1,
                scorePulse = scoreLabel ? scoreLabel.transform.localScale.x : 1,
                scoreZoom = scoreRoot ? scoreRoot.localScale.x : 1,
                scoreColor = scoreLabel ? scoreLabel.color : Color.white,
                redAlpha = red && red.enabled ? red.color.a : 0,
                environmentVisible = env && env.gameObject.activeInHierarchy,
                hudPanelVisible = hudImage && hudImage.enabled,
                barTopEdge = topBar ? (Vector2)topBar.TransformPoint(new Vector3(0, topBar.rect.yMin, 0)) : Vector2.zero,
                barBottomEdge = bottomBar ? (Vector2)bottomBar.TransformPoint(new Vector3(0, bottomBar.rect.yMax, 0)) : Vector2.zero,
                barLeft = topBar ? topBar.TransformPoint(new Vector3(topBar.rect.xMin, 0, 0)).x : 0,
                barRight = topBar ? topBar.TransformPoint(new Vector3(topBar.rect.xMax, 0, 0)).x : 0,
                scorePoint = scoreRoot ? (Vector2)scoreRoot.position : Vector2.zero,
                trayPoint = tray ? (Vector2)tray.position : Vector2.zero,
                bodyHead = layout && bodyA && bodyB ? (Vector2)Camera.main.WorldToScreenPoint(layout.HighestHead) : Vector2.zero,
                bodyFoot = layout && bodyA && bodyB ? (Vector2)Camera.main.WorldToScreenPoint(layout.LowestFoot) : Vector2.zero,
                tutorial = TutorialRun.Active, tutorialStatus = (int)TutorialRun.Status,
                tutorialLesson = tutorial ? tutorial.LessonIndex : 0, tutorialRetry = TutorialRun.Retrying,
                tutorialPracticing = tutorial && tutorial.Practicing,
                tutorialPassed = tutorial && tutorial.Passed,
                stunned = enemy && enemy.GetComponent<EnemyStunController>() && enemy.GetComponent<EnemyStunController>().IsStunned,
                scene = SceneManager.GetActiveScene().name, frame = Time.frameCount,
                endless = endless != null,
                encounterReading = GamePauseController.EncounterReading,
                endlessRound = endless != null ? endless.Round : 0,
                endlessSlot = endless != null ? endless.Slot : 0,
                defeated = endless != null ? endless.Defeated : 0,
                opponent = endless != null ? endless.Enemy.Name : "",
                runDifficulty = endless != null ? endless.Difficulty.Id() : "",
                menuDifficulty = EndlessDifficulties.Selected.Id(),
                tableDifficulty = board ? board.Difficulty.Id() : "",
                difficultyPace = endless != null ? endless.Difficulty.Pace() : 1f,
                recoverySeconds = endless != null ? endless.Recovery : 0,
                comboWindowSeconds = ScoreSystem.I ? ScoreSystem.I.ComboWindowSeconds : 0,
                comboRemaining = Read(ScoreSystem.I, "_windowRemain", 0f),
                gameplayClock = Time.time,
                unscaledClock = Time.unscaledTime,
                enemyNextAttackAt = Read(enemy, "_nextAttackAt", 0f),
                enemyChainRemaining = Read(enemy, "remainingSwings", 0),
                hitLocked = gate && gate.IsHitLocked, deadLocked = gate && gate.IsDeadLocked,
                typingMuted = TypingManager.MuteWordCorrect,
                finished = CombatSession.Finished, cue = cue && cue.activeInHierarchy,
                pendingAttack = Read(controller, "pendingAttack", false), parryPose = gate && gate.IsParryPoseLocked,
                helpOpen = help && help.IsOpen, helpSeen = HowToPlayPanel.Seen,
                combatSeconds = match ? match.Seconds : 0,
                inputLetters = match ? match.Letters : 0, inputCorrect = match ? match.Correct : 0,
                matchParries = match ? match.Parries : 0, finalScore = match ? match.FinalScore : 0,
                stamina = stamina ? stamina.Current : -1, nextProgress = Read(typing, "_typeAheadCount", 0),
                musicVolume = AudioPreferences.Music, sfxVolume = AudioPreferences.Sfx,
                result = resultLabel ? resultLabel.text : "",
                current = Read(typing, "current", ""), next = Read(typing, "next", ""),
                playerName = PlayerIdentity.Name, score = ScoreSystem.I ? ScoreSystem.I.Score : 0,
                hp = health ? health.Current : -1, maxHp = health ? health.maxHealth : -1,
                enemyHp = enemy ? enemy.Current : -1,
                enemySprite = enemyRenderer && enemyRenderer.sprite ? enemyRenderer.sprite.name : "",
                enemyTexture = enemyRenderer && enemyRenderer.sprite ? enemyRenderer.sprite.texture.name : "",
                words = words, chars = chars, parries = parries, errors = errors,
                slayer = slayer && slayer.IsActive, parryWindow = parry && parry.IsWindowActive,
                canParry = gate && gate.CanParry && (!parry || !parry.PoseLocked), enemyAttacking = attacking,
                enemyAnimationTime = info.normalizedTime, overlay = Read(pause, "_overlayLocked", false),
                enemyContactIn = contactIn, enemyAnimationSpeed = animationSpeed,
                activeSlashParts = FindObjectsOfType<MeshRenderer>().Count(r => r.enabled && r.name.StartsWith("Slash")),
                paused = Read(pause, "_paused", false), frameMs = Time.unscaledDeltaTime * 1000,
                timeScale = Time.timeScale, wordTime = Read(typing, "timer", 0f),
                rowError = Read(typing, "errorRemaining", 0f), rowSlide = Read(typing, "slideRemaining", 0f),
                memory = Profiler.GetTotalAllocatedMemoryLong(), allocatedBytes = frameAllocations.Valid ? frameAllocations.LastValue : -1,
                allocationCounterAvailable = frameAllocations.Valid,
                objects = Resources.FindObjectsOfTypeAll<GameObject>().Length,
                rows = pool == null ? 0 : pool.Count(r => r && r.gameObject.activeInHierarchy),
                dataRows = data == null ? 0 : data.Count,
                virtualized = Read(board, "virtualizeRows", false), firstVisible = Read(board, "_firstVisible", -1),
                leaderboardOpen = LeaderboardModal.IsOpen, cameraOrtho = Camera.main ? Camera.main.orthographicSize : 0,
                collectionMode = Read(board, "mode", -1),
                collectionTitle = Read<TMP_Text>(board, "title")?.text ?? "",
                collectionFirstRow = data != null && data.Count > 0 ? data[0].name : "",
                collectionDetails = Read<TMP_Text>(board, "detailText")?.text ?? "",
                collectionStatus = Read<TMP_Text>(board, "emptyLabel")?.text ?? "",
                selectedControl = UnityEngine.EventSystems.EventSystem.current && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject ?
                    UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name : "",
                rowHeight = Read(board, "_rowHeight", 0f), viewportHeight = scroll && scroll.viewport ? scroll.viewport.rect.height : 0f,
                pool = pool == null ? 0 : pool.Count, scroll = scroll ? scroll.verticalNormalizedPosition : -1,
                rowIds = pool == null ? Array.Empty<int>() : pool.Where(r => r).Select(r => r.GetInstanceID()).ToArray(),
                width = Screen.width, height = Screen.height,
                controls = controls.ToArray()
            };
#if UNITY_WEBGL && !UNITY_EDITOR
            TypingHeroQA_Publish(JsonUtility.ToJson(state));
#endif
            yield return wait;
        }
    }
}
#endif
