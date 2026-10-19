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
            if (burst && (!burst.Systems[0] || !burst.Systems[1])) return null;
            float minY = float.PositiveInfinity, maxY = float.NegativeInfinity, maxAge = 0;
            float velocityX = 0, depthSpeed = 0; int observedCount = 0;
            var systems = burst ? burst.Systems : clash ? clash.Systems : null;
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
                particles = burst ? burst.ParticleCount : clash ? clash.ParticleCount : 0, groundY = burst ? burst.GroundY : 0,
                minY = observedCount > 0 ? minY : 0, maxY = observedCount > 0 ? maxY : 0, maxAge = maxAge,
                meanVelocityX = observedCount > 0 ? velocityX / observedCount : 0,
                meanDepthSpeed = observedCount > 0 ? depthSpeed / observedCount : 0,
                screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y) };
        }).Where(frame => frame != null).ToArray();
    }
    [Serializable] sealed class State
    {
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
        public float enemyAnimationTime, frameMs, scroll, timeScale, wordTime;
        public long memory;
        public long allocatedBytes;
        public bool allocationCounterAvailable;
        public int rows, dataRows, pool, firstVisible, width, height;
        public bool virtualized;
        public bool leaderboardOpen;
        public bool tutorial, tutorialPracticing, tutorialPassed, tutorialRetry, stunned;
        public int tutorialLesson, tutorialStatus;
        public float cameraOrtho, multiplier, scorePulse, scoreZoom, redAlpha;
        public Color scoreColor;
        public bool environmentVisible, hudPanelVisible;
        public Vector2 barTopEdge, barBottomEdge, scorePoint, trayPoint, bodyHead, bodyFoot;
        public float barLeft, barRight;
        public Rect playerStatusRect, enemyStatusRect, trayRect, scoreRect, multiplierRect, staminaRect;
        public Vector2 playerFootPoint, enemyFootPoint;
        public float playerHpFill, enemyHpFill, staminaFill;
        public Rect[] wordRects;
        public Rect popupRect, decayRect;
        public Vector2 popupPoint;
        public float popupAlpha, popupFont, popupElapsed;
        public int popupCount, popupId;
        public string popupText;
        public Vector2 enemyHeadPoint;
        public bool playerNameVisible, enemyNameVisible;
        public Color staminaColor;

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
        while (true)
        {
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
            var playerHpSlider = playerStatus ? playerStatus.GetComponentInChildren<Slider>() : null;
            var enemyHpSlider = enemyStatus ? enemyStatus.Find("EnemyHP").GetComponent<Slider>() : null;
            var staminaSlider = enemyStatus ? enemyStatus.Find("EnemyStamina").GetComponent<Slider>() : null;
            var env = Read<Transform>(fx, "environment");
            var red = Read<SpriteRenderer>(fx, "redOverlay");
            var hudImage = Read<Image>(fx, "hudPanel");
            var tray = Read<RectTransform>(fx, "typingTray");
            var award = FindObjectOfType<FloatingScore>();
            var awardLabel = award ? award.GetComponentInChildren<TMP_Text>() : null;
            var state = new State
            {
                playerSorting = sortingFrame,
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
                staminaColor = staminaSlider ? staminaSlider.fillRect.GetComponent<Image>().color : default,
                popupPoint = ScreenRect(Read<RectTransform>(layout, "popupAnchor")).position,
                decayRect = ScreenRect(Read<RectTransform>(layout, "comboDecay")),
                playerStatusRect = ScreenRect(playerStatus), enemyStatusRect = ScreenRect(enemyStatus),
                trayRect = ScreenRect(tray), scoreRect = TextRect(scoreLabel), multiplierRect = TextRect(multiplierLabel),
                staminaRect = staminaSlider ? ScreenRect((RectTransform)staminaSlider.transform) : default,
                playerFootPoint = layout && bodyA ? (Vector2)Camera.main.WorldToScreenPoint(layout.Foot(bodyA)) : default,
                enemyFootPoint = layout && bodyB ? (Vector2)Camera.main.WorldToScreenPoint(layout.Foot(bodyB)) : default,
                playerHpFill = playerHpSlider ? playerHpSlider.normalizedValue : -1,
                enemyHpFill = enemyHpSlider ? enemyHpSlider.normalizedValue : -1,
                staminaFill = staminaSlider ? staminaSlider.normalizedValue : -1,
                wordRects = new[] { TextRect(Read<TMP_Text>(typing, "wordText")), TextRect(Read<TMP_Text>(typing, "nextWordText")), TextRect(Read<TMP_Text>(typing, "thirdWordText")) },
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
                canParry = gate && gate.CanParry, enemyAttacking = attacking,
                enemyAnimationTime = info.normalizedTime, overlay = Read(pause, "_overlayLocked", false),
                paused = Read(pause, "_paused", false), frameMs = Time.unscaledDeltaTime * 1000,
                timeScale = Time.timeScale, wordTime = Read(typing, "timer", 0f),
                memory = Profiler.GetTotalAllocatedMemoryLong(), allocatedBytes = frameAllocations.Valid ? frameAllocations.LastValue : -1,
                allocationCounterAvailable = frameAllocations.Valid,
                objects = Resources.FindObjectsOfTypeAll<GameObject>().Length,
                rows = pool == null ? 0 : pool.Count(r => r && r.gameObject.activeInHierarchy),
                dataRows = data == null ? 0 : data.Count,
                virtualized = Read(board, "virtualizeRows", false), firstVisible = Read(board, "_firstVisible", -1),
                leaderboardOpen = LeaderboardModal.IsOpen, cameraOrtho = Camera.main ? Camera.main.orthographicSize : 0,
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
