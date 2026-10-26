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
using UnityEngine.Rendering.Universal;

// Observation only: no command dispatcher, setters, or input injection.
[DefaultExecutionOrder(2000)]
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
        public string role, preset, cel;
        public Color tint;
        public bool flipX, voxel;
        public int id, frame, order;
        public Vector3 position, scale;
        public Rect screenRect;
        public int particles;
        public float groundY, minY, maxY, maxAge, meanVelocityX, meanDepthSpeed, clockwiseAngle;
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

    [Serializable] sealed class CombatRenderFrame
    {
        public int frame, attackId, hitFrame, enemyHP, playerHP, parries;
        public bool canParry, parryWindow, enemyAttacking, slayer;
        public float enemyContactIn, timeScale, cueImpactIn;
        public bool spaceCueVisible, spaceCueFilled, spaceCueLocked, legacyCueVisible;
        public int cueAttackId;
        public Rect spaceCueRect;
        public string cel, sparkCel;
        public Vector2 contact;
        public Vector3 sparkPoint, cameraPosition, cameraCorrection;
        public int impactKind, impactSerial, normalImpacts, parryImpacts, receivedImpacts, lethalImpacts, impulseCount;
        public int effectSerial, peakRenderCount, shakePeakRenderCount, peakRenderFrame, worldWidth, worldHeight;
        public float impactZoom, cameraBase, cameraHalfHeight, cameraFov, unscaledNow;
        public float presentationWeight, backgroundAlpha;
        public bool enemyFlash;
        public Color enemyColor;
        public string enemyCel;
        public float slayerBodyFlash;
        public Vector2 slayerDirection;
        public Vector3 enemyBodyCenter;
        public bool slayerTrailVisible;
        public SlayerRenderFrame[] slayerImpacts;
        public int parryParticleCount, parryBurstCount, parrySampleCount;
        public ParryParticleFrame[] parryParticles;
    }
    [Serializable] sealed class SlayerRenderFrame
    {
        public int id;
        public string cel;
        public float age, clockwiseAngle;
        public Color tint;
        public Vector3 position;
        public bool fading;
    }
    [Serializable] sealed class ParryParticleFrame
    {
        public int burst, channel;
        public uint identity;
        public float age, width, length;
        public Vector3 position;
        public Color32 color;
    }
    readonly ParryParticleFrame[] renderParryParticles = CreateParryFrames();
    static ParryParticleFrame[] CreateParryFrames()
    {
        var frames = new ParryParticleFrame[192];
        for (int i = 0; i < frames.Length; i++) frames[i] = new ParryParticleFrame();
        return frames;
    }
    readonly System.Collections.Generic.List<GameObject> renderRoots = new();
    readonly System.Collections.Generic.List<ImpactVfxBurst> renderBursts = new();
    void ObserveRenderParry()
    {
        combatFrame.parryParticleCount = combatFrame.parryBurstCount = combatFrame.parrySampleCount = 0;
        combatFrame.parryParticles = renderParryParticles;
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded) return;
        scene.GetRootGameObjects(renderRoots);
        foreach (var root in renderRoots)
        {
            root.GetComponentsInChildren(false, renderBursts);
            foreach (var burst in renderBursts)
            {
                if (!burst.Analytical || burst.ParticleCount == 0) continue;
                combatFrame.parryBurstCount++;
                combatFrame.parryParticleCount += burst.ParticleCount;
                for (int channel = 0; channel < burst.Systems.Length; channel++)
                {
                    if (!burst.IsPhysicalChannel(channel)) continue;
                    int count = burst.Systems[channel].GetParticles(observedParticles);
                    for (int i = 0; i < count && combatFrame.parrySampleCount < renderParryParticles.Length; i++)
                    {
                        int index = combatFrame.parrySampleCount++;
                        var sample = renderParryParticles[index];
                        var p = observedParticles[i];
                        sample.burst = burst.GetInstanceID(); sample.channel = burst.ChannelId(channel); sample.identity = p.randomSeed;
                        sample.age = burst.DebrisAge; sample.width = p.startSize3D.x * burst.PixelsPerUnit;
                        sample.length = p.startSize3D.y * burst.PixelsPerUnit; sample.position = p.position; sample.color = p.startColor;
                    }
                }
            }
        }
    }
    PlayerWeaponHitbox combatHitbox;
    SkeletonController combatEnemy;
    PlayerHealth combatPlayer;
    ParrySystem combatParry;
    PlayerSlayerMode combatSlayer;
    CombatCameraImpact combatImpact;
    DungeonArenaCamera combatArena;
    readonly System.Collections.Generic.List<AnimatorClipInfo> combatClips = new();
    readonly System.Collections.Generic.Dictionary<AnimationClip, float> contactTimes = new();
    float ContactInRender()
    {
        if (!combatEnemy || !combatEnemy.IsAttacking) return -1;
        var animator = Read<Animator>(combatEnemy, "animator");
        if (!animator) return -1;
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.GetCurrentAnimatorClipInfo(0, combatClips);
        if (combatClips.Count == 0) return -1;
        var clip = combatClips[0].clip;
        if (!contactTimes.TryGetValue(clip, out float marker))
        {
            marker = -1;
            foreach (var ev in clip.events) if (ev.functionName == "Anim_DealDamage") { marker = ev.time; break; }
            contactTimes[clip] = marker;
        }
        return marker < 0 ? -1 : (marker - info.normalizedTime % 1 * clip.length) /
            Mathf.Max(.001f, animator.speed * info.speed * info.speedMultiplier);
    }
    readonly CombatRenderFrame combatFrame = new CombatRenderFrame();
    void PublishCombatFrame()
    {
        if (!combatHitbox) combatHitbox = FindObjectOfType<PlayerWeaponHitbox>();
        if (!combatEnemy) combatEnemy = FindObjectOfType<SkeletonController>();
        if (!combatPlayer) combatPlayer = FindObjectOfType<PlayerHealth>();
        if (!combatParry) combatParry = FindObjectOfType<ParrySystem>();
        if (!combatSlayer) combatSlayer = FindObjectOfType<PlayerSlayerMode>();
        if (!combatImpact) combatImpact = FindObjectOfType<CombatCameraImpact>();
        if (!combatArena && Camera.main) combatArena = Camera.main.GetComponent<DungeonArenaCamera>();
        combatFrame.impactKind = combatImpact ? (int)combatImpact.ActiveKind : 0;
        combatFrame.impactSerial = combatImpact ? combatImpact.Serial : 0;
        combatFrame.normalImpacts = combatImpact ? combatImpact.NormalCount : 0;
        combatFrame.parryImpacts = combatImpact ? combatImpact.ParryCount : 0;
        combatFrame.receivedImpacts = combatImpact ? combatImpact.ReceivedCount : 0;
        combatFrame.lethalImpacts = combatImpact ? combatImpact.LethalCount : 0;
        combatFrame.impulseCount = combatImpact ? combatImpact.ImpulseCount : 0;
        combatFrame.effectSerial = combatImpact ? combatImpact.EffectSerial : 0;
        combatFrame.peakRenderCount = combatImpact ? combatImpact.PeakRenderCount : 0;
        combatFrame.shakePeakRenderCount = combatImpact ? combatImpact.ShakePeakRenderCount : 0;
        combatFrame.peakRenderFrame = combatImpact ? combatImpact.PeakRenderFrame : 0;
        combatFrame.worldWidth = combatArena && combatArena.WorldTexture ? combatArena.WorldTexture.width : 0;
        combatFrame.worldHeight = combatArena && combatArena.WorldTexture ? combatArena.WorldTexture.height : 0;
        combatFrame.impactZoom = combatImpact ? combatImpact.Zoom : 0;
        combatFrame.cameraBase = Camera.main ? Camera.main.orthographicSize : 0;
        combatFrame.cameraHalfHeight = combatArena ? combatArena.EffectiveHalfHeight : 0;
        combatFrame.cameraFov = Camera.main ? Camera.main.fieldOfView : 0;
        combatFrame.cameraPosition = Camera.main ? Camera.main.transform.position : default;
        var brain = Camera.main ? Camera.main.GetComponent<Cinemachine.CinemachineBrain>() : null;
        combatFrame.cameraCorrection = brain ? brain.CurrentCameraState.PositionCorrection : default;
        combatFrame.unscaledNow = Time.unscaledTime;
        combatFrame.presentationWeight = SlayerScreenFX.PresentationWeight;
        combatFrame.backgroundAlpha = combatArena ? combatArena.SlayerBackground.a : 0;
        var enemySprite = combatEnemy ? combatEnemy.GetComponent<SpriteRenderer>() : null;
        var enemyFlash = combatEnemy ? combatEnemy.GetComponent<EnemyHitFlash>() : null;
        combatFrame.enemyColor = enemySprite ? enemySprite.color : Color.clear;
        combatFrame.enemyCel = enemySprite && enemySprite.sprite ? enemySprite.sprite.name : "";
        var silhouette = combatEnemy ? combatEnemy.GetComponent<SlayerSilhouetteGroup>() : null;
        combatFrame.slayerBodyFlash = silhouette ? silhouette.HitFlashWeight : 0;
        var body = combatEnemy ? combatEnemy.GetComponent<BoxCollider2D>() : null;
        combatFrame.enemyBodyCenter = body ? body.bounds.center : default;
        combatFrame.slayerDirection = combatSlayer ? combatSlayer.StrikeDirection : Vector2.zero;
        var trail = combatSlayer ? combatSlayer.GetComponentInChildren<SlashVolume>() : null;
        combatFrame.slayerTrailVisible = trail && trail.TrailVisible;
        combatFrame.slayerImpacts = FindObjectsOfType<SlayerHitVfx>().Select(effect => new SlayerRenderFrame {
            id = effect.GetInstanceID(), cel = effect.GetComponent<SpriteRenderer>().sprite ? effect.GetComponent<SpriteRenderer>().sprite.name : "",
            age = effect.Age, clockwiseAngle = Mathf.Repeat(-effect.transform.eulerAngles.z, 360),
            tint = effect.GetComponent<SpriteRenderer>().color, position = effect.transform.position, fading = effect.IsFading
        }).ToArray();
        combatFrame.enemyFlash = enemyFlash && enemyFlash.isActiveAndEnabled && Read<bool>(enemyFlash, "_active");
        ObserveRenderParry();
        combatFrame.playerHP = combatPlayer ? combatPlayer.Current : -1;
        combatFrame.parries = CombatSession.I ? CombatSession.I.Parries : 0;
        combatFrame.canParry = combatParry && combatParry.CanBeginParry;
        combatFrame.parryWindow = combatParry && combatParry.IsWindowActive;
        combatFrame.enemyAttacking = combatEnemy && combatEnemy.IsAttacking;
        combatFrame.cueAttackId = combatEnemy ? combatEnemy.AttackSequenceId : 0;
        combatFrame.cueImpactIn = combatEnemy && combatEnemy.TryGetAttackTelegraph(out float cueImpact) ? cueImpact : -1;
        var presenter = FindObjectOfType<ParryCuePresenter>();
        var cueKey = presenter ? Read<RectTransform>(presenter, "key") : null;
        combatFrame.spaceCueRect = cueKey ? ScreenRect(cueKey) : default;
        var cueLeft = presenter ? Read<Image>(presenter, "left") : null;
        var cueLock = presenter ? Read<Image>(presenter, "locked") : null;
        combatFrame.spaceCueVisible = cueKey && cueKey.gameObject.activeInHierarchy;
        combatFrame.spaceCueFilled = cueLeft && cueLeft.sprite && cueLeft.sprite.name == "tile_0507";
        combatFrame.spaceCueLocked = cueLock && cueLock.gameObject.activeInHierarchy;
        var legacy = combatEnemy && combatEnemy.AttackCueObject ? combatEnemy.AttackCueObject.GetComponent<Graphic>() : null;
        combatFrame.legacyCueVisible = legacy && legacy.enabled && legacy.gameObject.activeInHierarchy;

        combatFrame.slayer = combatSlayer && combatSlayer.IsActive;
        combatFrame.enemyContactIn = ContactInRender();
        combatFrame.timeScale = Time.timeScale;
        combatFrame.frame = Time.frameCount;
        combatFrame.attackId = combatHitbox ? combatHitbox.AttackId : 0;
        combatFrame.hitFrame = combatHitbox ? combatHitbox.LastHitFrame : -1;
        combatFrame.contact = combatHitbox ? combatHitbox.LastContact : default;
        combatFrame.enemyHP = combatEnemy ? combatEnemy.Current : -1;
        combatFrame.cel = sortingSprite && sortingSprite.sprite ? sortingSprite.sprite.name : "";
        var spark = FindObjectOfType<NormalHitSpark>();
        combatFrame.sparkCel = spark && spark.GetComponent<SpriteRenderer>().sprite ? spark.GetComponent<SpriteRenderer>().sprite.name : "";
        combatFrame.sparkPoint = spark ? spark.transform.position : default;
#if UNITY_WEBGL && !UNITY_EDITOR
        TypingHeroQA_CombatFrame(JsonUtility.ToJson(combatFrame));
#endif
    }
    void LateUpdate()
    {
        PublishCombatFrame();
        if (!sortingAnimator || !sortingSprite) return;
        var info = sortingAnimator.IsInTransition(0) ? sortingAnimator.GetNextAnimatorStateInfo(0)
            : sortingAnimator.GetCurrentAnimatorStateInfo(0);
        bool attack = info.IsTag("Attack");
        bool parry = info.IsTag("Parry");
        string name = "other";
        if (info.fullPathHash == Animator.StringToHash("Base Layer.parry_followup")) name = "Base Layer.parry_followup";
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
            var slash = effect.GetComponent<SlayerHitVfx>();
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
            var min = camera ? CombatViewportCamera.WorldToScreenPoint(camera, bounds.min) : Vector3.zero;
            var max = camera ? CombatViewportCamera.WorldToScreenPoint(camera, bounds.max) : Vector3.zero;
            return new SparkFrame { role = role, cel = sprite && sprite.sprite ? sprite.sprite.name : "",
                tint = sprite ? sprite.color : Color.clear, flipX = sprite && sprite.flipX, voxel = voxel, preset = clash ? clash.EmittedPreset.ToString() : "", id = effect.GetInstanceID(), frame = Time.frameCount,
                order = sprite ? sprite.sortingOrder : particle.sortingOrder,
                position = effect.transform.position, scale = effect.transform.lossyScale,
                clockwiseAngle = Mathf.Repeat(-effect.transform.eulerAngles.z, 360),
                contactGap = clash ? clash.Visual.separation : 0,
                defenseCel = clash && clash.Visual.defenseCel ? clash.Visual.defenseCel.name : "",
                attackCel = clash && clash.Visual.attackCel ? clash.Visual.attackCel.name : "",
                particles = burst ? burst.ParticleCount : clash ? clash.ParticleCount : voxel ? voxel.ParticleCount : accent ? accent.ParticleCount : 0, groundY = burst ? burst.GroundY : 0,
                minY = observedCount > 0 ? minY : 0, maxY = observedCount > 0 ? maxY : 0, maxAge = slash ? slash.Age : accent ? accent.Age : maxAge,
                meanVelocityX = observedCount > 0 ? velocityX / observedCount : 0,
                meanDepthSpeed = observedCount > 0 ? depthSpeed / observedCount : 0,
                screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y) };
        }).Where(frame => frame != null).ToArray();
    }
    [Serializable] sealed class State
    {
        public bool introRunning, introEnemyHidden, introInputBlocked;
        public float introElapsed, introHudAlpha, introLabelAlpha, introCover;
        public float introRevealProgress, introWordAlpha;
        public int introWordHudGroups;
        public bool introWordPreview;
        public float introWordStartRemaining;
        public int introDustParticles, introMusicPlayerId, introMusicVoiceId, introMusicSample;
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
        public bool tutorial, stunned;
        public bool audioPaused;
        public int tutorialStatus;
        public Rect tutorialPanel;
        public GuideArrow[] tutorialArrows;
        public float cameraOrtho, multiplier, scorePulse, scoreZoom, redAlpha;
        public float cameraFieldOfView, cameraImpactZoom, cameraEffectiveHalfHeight;
        public Vector3 cameraPosition;
        public string cameraImpactKind, cameraImpactLastKind;
        public int cameraImpactSerial, cameraNormalImpacts, cameraParryImpacts, cameraReceivedImpacts, cameraLethalImpacts, cameraImpulseCount;
        public bool cameraPerspective;
        public float worldRenderScale;
        public int worldWidth, worldHeight, worldTextureId, presentationCameras;
        public int dungeonId, worldCameraId, worldCameras, listeners, gameplayActors, gameplayHud;
        public bool dungeonTransitioning;
        public int arenaTextures, ambientParticles, ambientParticleLimit, lightmaps, lightProbes;
        public float combatPulseIntensity;
        public bool combatPulseEnabled;
        public bool arenaCinematic, arenaPostProcessing;
        public Rect arenaViewport;
        public Color scoreColor;
        public bool environmentVisible, hudPanelVisible;
        public float slayerPresentationWeight, hudPanelAlpha, typingPanelAlpha;
        public Vector2 barTopEdge, barBottomEdge, scorePoint, trayPoint, bodyHead, bodyFoot, bodyCenter;
        public float barLeft, barRight;
        public FontObservation[] typography;
        public Rect scoreGlyphRect, comboGlyphRect, comboDecayRect;
        public bool comboVisible, comboDecayVisible, comboUnderScore, comboDecayUnderScore;
        public float comboDecayValue;
        public float scoreFont, comboFont, barReveal;
        public Rect playerStatusRect, enemyStatusRect, trayRect, scoreRect, multiplierRect, staminaRect;
        public Vector2 playerFootPoint, enemyFootPoint;
        public Vector3 enemyBodyCenter;
        public Vector2 enemyBodyCenterScreen;
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
    [Serializable] sealed class GuideArrow
    {
        public string target;
        public Vector2 start, end;
        public Rect targetRect;
    }
    static GuideArrow[] ObserveGuide(TutorialOverlay overlay)
    {
        if (!overlay || !overlay.gameObject.activeInHierarchy) return Array.Empty<GuideArrow>();
        return Read<TutorialOverlay.Arrow[]>(overlay, "arrows").Select(a => new GuideArrow {
            target = a.target.name,
            start = a.shaft.TransformPoint(new Vector3(a.shaft.rect.xMin, 0, 0)),
            end = a.shaft.TransformPoint(new Vector3(a.shaft.rect.xMax, 0, 0)),
            targetRect = ScreenRect(a.target)
        }).ToArray();
    }
    [Serializable] sealed class FontObservation
    {
        public string path, text, font, shader;
        public float size, minimum, maximum, scale;
        public bool autosize, truncated;
        public Rect bounds, glyphBounds;
        public string[] renderedFonts;
    }
    static FontObservation[] ObserveTypography()
    {
        return FindObjectsOfType<TMP_Text>().Where(t => t.isActiveAndEnabled).Select(t =>
        {
            var names = new System.Collections.Generic.List<string>();
            for (var node = t.transform; node; node = node.parent) names.Add(node.name);
            names.Reverse();
            var info = t.textInfo;
            return new FontObservation {
                path = string.Join("/", names), text = t.text, font = t.font ? t.font.name : "",
                shader = t.fontSharedMaterial ? t.fontSharedMaterial.shader.name : "",
                size = t.fontSize, minimum = t.fontSizeMin, maximum = t.fontSizeMax,
                scale = t.transform.lossyScale.y, autosize = t.enableAutoSizing,
                truncated = t.isTextTruncated, bounds = FontScreenRect(t.rectTransform, !(t is TextMeshProUGUI)),
                glyphBounds = info.characterInfo.Take(info.characterCount).Any(c => c.isVisible) ? FontGlyphRect(t) : default,
                renderedFonts = info.characterInfo.Take(info.characterCount).Where(c => c.isVisible && c.fontAsset)
                    .Select(c => c.fontAsset.name).Distinct().ToArray()
            };
        }).ToArray();
    }
    static Rect FontScreenRect(RectTransform rt, bool worldSpace = false)
    {
        if (!rt) return default;
        var min = rt.TransformPoint(new Vector3(rt.rect.xMin, rt.rect.yMin, 0));
        var max = rt.TransformPoint(new Vector3(rt.rect.xMax, rt.rect.yMax, 0));
        if (worldSpace && Camera.main)
        {
            min = CombatViewportCamera.WorldToScreenPoint(Camera.main, min);
            max = CombatViewportCamera.WorldToScreenPoint(Camera.main, max);
        }
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
    static Rect FontTextRect(TMP_Text text)
    {
        if (!text || !text.gameObject.activeInHierarchy) return default;
        var rt = text.rectTransform;
        float offset = text.alignment == TextAlignmentOptions.Center ? (rt.rect.width - text.preferredWidth) * .5f : 0;
        var min = rt.TransformPoint(new Vector3(rt.rect.xMin + offset, rt.rect.yMin, 0));
        var max = rt.TransformPoint(new Vector3(rt.rect.xMin + offset + text.preferredWidth, rt.rect.yMax, 0));
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
    static Rect FontGlyphRect(TMP_Text text)
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
            if (!(text is TextMeshProUGUI) && Camera.main)
            {
                topLeft = CombatViewportCamera.WorldToScreenPoint(Camera.main, topLeft);
                bottomRight = CombatViewportCamera.WorldToScreenPoint(Camera.main, bottomRight);
            }
            min = Vector2.Min(min, bottomRight); min = Vector2.Min(min, topLeft);
            max = Vector2.Max(max, bottomRight); max = Vector2.Max(max, topLeft);
        }
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
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
    [DllImport("__Internal")] static extern void TypingHeroQA_CombatFrame(string json);
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
            var guide = FindObjectOfType<TutorialOverlay>();
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
            var hudRound = FindObjectOfType<CombatR7Status>()?.transform.Find("RoundR7")?.GetComponent<TMP_Text>();
            var hudBackground = FindObjectOfType<CombatR7Status>()?.transform.Find("RoundReadabilityR7")?.GetComponent<Image>();
            var guardLabel = Read<TMP_Text>(hudFeedback, "guardBreak");
            var guardIcon = Read<Image>(hudFeedback, "staminaIcon");
            var combatHeading = Read<TMP_Text>(FindObjectOfType<EndlessOverlay>(), "heading");
            var env = DungeonSession.Current && DungeonSession.Current.World
                ? DungeonSession.Current.World.environment : Read<Transform>(fx, "environment");
            var arena = Camera.main ? Camera.main.GetComponent<DungeonArenaCamera>() : null;
            var impact = FindObjectOfType<CombatCameraImpact>();
            var pulseRig = FindObjectsOfType<ClashLightingRig>(true).FirstOrDefault(rig => Read<Light>(rig, "combatLight"));
            var arenaPulse = Read<Light>(pulseRig, "combatLight");
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
                introMusicPlayerId = MusicPlayer.I ? MusicPlayer.I.GetInstanceID() : 0,
                introMusicVoiceId = MusicPlayer.I && MusicPlayer.I.GetComponentsInChildren<AudioSource>().FirstOrDefault(s => s.isPlaying) ? MusicPlayer.I.GetComponentsInChildren<AudioSource>().First(s => s.isPlaying).GetInstanceID() : 0,
                introMusicSample = MusicPlayer.I && MusicPlayer.I.GetComponentsInChildren<AudioSource>().FirstOrDefault(s => s.isPlaying) ? MusicPlayer.I.GetComponentsInChildren<AudioSource>().First(s => s.isPlaying).timeSamples : 0,
                introRunning = FindObjectOfType<DuelIntroPresentation>() && FindObjectOfType<DuelIntroPresentation>().Running,
                introRevealProgress = Read<float>(FindObjectOfType<DuelIntroPresentation>(), "revealProgress"),
                introWordAlpha = Read<float>(FindObjectOfType<DuelIntroPresentation>(), "wordAlpha"),
                introWordPreview = Read<bool>(typing, "introPreview"),
                introWordStartRemaining = Read<float>(typing, "introStartRemaining"),
                introWordHudGroups = Read<System.Collections.Generic.HashSet<CanvasGroup>>(FindObjectOfType<DuelIntroPresentation>(), "wordHud")?.Count ?? 0,
                introElapsed = FindObjectOfType<DuelIntroPresentation>() ? FindObjectOfType<DuelIntroPresentation>().Elapsed : 0,
                introHudAlpha = Read<float>(FindObjectOfType<DuelIntroPresentation>(), "hudAlpha"),
                introLabelAlpha = Read<TMP_Text>(FindObjectOfType<DuelIntroPresentation>(), "label") ? Read<TMP_Text>(FindObjectOfType<DuelIntroPresentation>(), "label").alpha : 0,
                introCover = Read<RectTransform>(FindObjectOfType<DuelIntroPresentation>(), "curtain") && Read<RectTransform>(FindObjectOfType<DuelIntroPresentation>(), "curtain").gameObject.activeSelf ? 1 - Read<RectTransform>(FindObjectOfType<DuelIntroPresentation>(), "curtain").anchorMin.y : 0,
                introDustParticles = Read<ParticleSystem>(FindObjectOfType<DuelIntroPresentation>(), "dust") ? Read<ParticleSystem>(FindObjectOfType<DuelIntroPresentation>(), "dust").particleCount : 0,
                introEnemyHidden = sortingEnemy && sortingEnemy.forceRenderingOff,
                introInputBlocked = GamePauseController.InputBlocked,
                playerSorting = sortingFrame,
                playerCurrentState = playerInfo.fullPathHash, playerNextState = playerNext,
                playerAnimatorTime = playerInfo.normalizedTime,
                playerAnimatorSpeed = sortingAnimator ? sortingAnimator.speed : 0,
                playerStateSpeed = playerInfo.speed * playerInfo.speedMultiplier,
                playerAnimatorTransition = playerTransition,
                playerAnimatorEnabled = sortingAnimator && sortingAnimator.isActiveAndEnabled,
                playerHitStateActive = hitState != 0 && (playerInfo.fullPathHash == hitState || playerNext == hitState),
                sparks = sparks, sparkMask = sparkMask, lastError = lastError,
                dungeonId = FindObjectOfType<DungeonWorld>() ? FindObjectOfType<DungeonWorld>().environment.GetInstanceID() : 0,
                worldCameraId = Camera.main ? Camera.main.GetInstanceID() : 0,
                worldCameras = FindObjectsOfType<DungeonArenaCamera>().Length,
                listeners = FindObjectsOfType<AudioListener>().Count(listener => listener.isActiveAndEnabled),
                gameplayActors = FindObjectsOfType<PlayerHealth>().Length + FindObjectsOfType<SkeletonController>().Length,
                gameplayHud = FindObjectsOfType<CombatHUDLayout>().Length,
                dungeonTransitioning = DungeonSession.Current && DungeonSession.Current.Transitioning,
                popupRect = TextRect(awardLabel), popupAlpha = awardLabel ? awardLabel.color.a : 0,
                popupFont = awardLabel ? awardLabel.fontSize : 0,
                popupElapsed = award ? Read<float>(award, "_elapsed") : 0,
                popupCount = FindObjectsOfType<FloatingScore>().Length,
                popupId = award ? award.GetInstanceID() : 0,
                popupText = awardLabel ? awardLabel.text : "",
                enemyHeadPoint = layout && bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.Head(bodyB)) : default,
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
                typography = ObserveTypography(),
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
                playerFootPoint = layout && bodyA ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.Foot(bodyA)) : default,
                enemyFootPoint = layout && bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.Foot(bodyB)) : default,
                // Offset remains observable after a lethal hit disables the collider.
                enemyBodyCenter = bodyB ? bodyB.transform.TransformPoint(bodyB.offset) : default,
                enemyBodyCenterScreen = bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, bodyB.transform.TransformPoint(bodyB.offset)) : default,
                playerHpFill = playerHpSlider ? playerHpSlider.normalizedValue : -1,
                enemyHpFill = enemyHpSlider ? enemyHpSlider.normalizedValue : -1,
                staminaFill = staminaSlider ? staminaSlider.normalizedValue : -1,
                wordRects = new[] { TextRect(Read<TMP_Text>(typing, "wordText")), TextRect(Read<TMP_Text>(typing, "nextWordText")), TextRect(Read<TMP_Text>(typing, "thirdWordText")) },
                wordGlyphRects = new[] { GlyphRect(Read<TMP_Text>(typing, "wordText")), GlyphRect(Read<TMP_Text>(typing, "nextWordText")), GlyphRect(Read<TMP_Text>(typing, "thirdWordText")) },
                multiplier = ScoreSystem.I ? ScoreSystem.I.Multiplier : 1,
                scorePulse = scoreLabel ? scoreLabel.transform.localScale.x : 1,
                scoreZoom = scoreRoot ? scoreRoot.localScale.x : 1,
                scoreColor = scoreLabel ? scoreLabel.color : Color.white,
                redAlpha = arena ? arena.SlayerBackground.a : red && red.enabled ? red.color.a : 0,
                slayerPresentationWeight = SlayerScreenFX.PresentationWeight,
                hudPanelAlpha = hudImage ? hudImage.color.a : 0,
                typingPanelAlpha = Read<Image>(fx, "typingPanel") ? Read<Image>(fx, "typingPanel").color.a : 0,
                environmentVisible = env && env.gameObject.activeInHierarchy,
                hudPanelVisible = hudImage && hudImage.enabled && hudImage.color.a > .001f,
                barTopEdge = topBar ? (Vector2)topBar.TransformPoint(new Vector3(0, topBar.rect.yMin, 0)) : Vector2.zero,
                barBottomEdge = bottomBar ? (Vector2)bottomBar.TransformPoint(new Vector3(0, bottomBar.rect.yMax, 0)) : Vector2.zero,
                barLeft = topBar ? topBar.TransformPoint(new Vector3(topBar.rect.xMin, 0, 0)).x : 0,
                barRight = topBar ? topBar.TransformPoint(new Vector3(topBar.rect.xMax, 0, 0)).x : 0,
                scorePoint = scoreRoot ? (Vector2)scoreRoot.position : Vector2.zero,
                trayPoint = tray ? (Vector2)tray.position : Vector2.zero,
                bodyHead = layout && bodyA && bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.HighestHead) : Vector2.zero,
                bodyFoot = layout && bodyA && bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.LowestFoot) : Vector2.zero,
                bodyCenter = layout && bodyA && bodyB ? (Vector2)CombatViewportCamera.WorldToScreenPoint(Camera.main, layout.BodyCenter) : Vector2.zero,
                tutorial = TutorialRun.Active, tutorialStatus = (int)TutorialRun.Status,
                tutorialPanel = guide ? ScreenRect((RectTransform)guide.transform.Find("Reference960x540/TutorialPrompt")) : default,
                tutorialArrows = ObserveGuide(guide),
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
                audioPaused = AudioListener.pause,
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
                cameraFieldOfView = Camera.main ? Camera.main.fieldOfView : 0,
                cameraPerspective = Camera.main && !Camera.main.orthographic,
                cameraImpactZoom = impact ? impact.Zoom : 0,
                cameraEffectiveHalfHeight = arena ? arena.EffectiveHalfHeight : 0,
                cameraPosition = Camera.main ? Camera.main.transform.position : default,
                cameraImpactKind = impact ? impact.ActiveKind.ToString() : "None",
                cameraImpactLastKind = impact ? impact.LastKind.ToString() : "None",
                cameraImpactSerial = impact ? impact.Serial : 0,
                cameraNormalImpacts = impact ? impact.NormalCount : 0,
                cameraParryImpacts = impact ? impact.ParryCount : 0,
                cameraReceivedImpacts = impact ? impact.ReceivedCount : 0,
                cameraLethalImpacts = impact ? impact.LethalCount : 0,
                cameraImpulseCount = impact ? impact.ImpulseCount : 0,
                worldWidth = arena && arena.WorldTexture ? arena.WorldTexture.width : 0,
                worldHeight = arena && arena.WorldTexture ? arena.WorldTexture.height : 0,
                worldTextureId = arena && arena.WorldTexture ? arena.WorldTexture.GetInstanceID() : 0,
                presentationCameras = FindObjectsOfType<DungeonPresentationCamera>().Length,
                arenaTextures = Resources.FindObjectsOfTypeAll<RenderTexture>().Count(texture => texture.IsCreated() && texture.name.StartsWith("Dungeon ")),
                ambientParticles = env ? env.GetComponentsInChildren<ParticleSystem>().Sum(ps => ps.particleCount) : 0,
                ambientParticleLimit = env ? env.GetComponentsInChildren<ParticleSystem>().Sum(ps => ps.main.maxParticles) : 0,
                lightmaps = LightmapSettings.lightmaps.Length,
                lightProbes = LightmapSettings.lightProbes ? LightmapSettings.lightProbes.count : 0,
                combatPulseIntensity = arenaPulse ? arenaPulse.intensity : 0,
                combatPulseEnabled = arenaPulse && arenaPulse.enabled && arenaPulse.gameObject.activeInHierarchy,
                arenaCinematic = arena && arena.Cinematic,
                arenaPostProcessing = arena && arena.PresentationCamera && arena.PresentationCamera.GetUniversalAdditionalCameraData().renderPostProcessing,
                arenaViewport = arena ? arena.PresentationRect : default,
                worldRenderScale = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urp ? urp.renderScale : -1,
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
