# Current gameplay

Current source brand: **CLACKBLADE**, slogan **TYPE TO SLASH**. Gameplay and stable technical
identifiers remain unchanged by the rename. Native menu960x540/640x360 and
same-artifact name reload verified. Public release: https://micharel09.itch.io/clackblade
(upload19871730). Endless online reads still401; Firebase unchanged.

## Endless Duel (2026-10-19)

- SampleScene starts Endless except during the independently configured tutorial.
  Player starts40HP; power, words and scoring remain unchanged. Each run has
  four normals, elite300HP/150stamina and captain550HP/200stamina; normals100stamina.
- Role assets in Assets/Resources/Endless own stats/forms/attack arrays. Swordsman180HP
  usesAttack1 and alternatesAttack2 from round2; DoubleStrike200HP uses1->2;
  Heavy240HP usesaxe1/8damage; Berserker200HP usesaxe2->1->2/5damage;
  Guard200HP alternates shield singles, takes full typing damage but only4stamina/hit
  versus25/parry. Other roles preserve12stamina/hit. No full-body role tint or scale.
- Current run: swordsman,double,heavy,guard,elite swordsman,captain, then victory.
  Internal RepeatAfterBoss=false disables repeat and boss healing. Dormant Round2:
  berserker,double,guard,heavy,elite heavy,captain. Round3+ rotates four consecutive
  roles and elite through the five-role list. Elite uses its known base attacks.
  Captain selects singles/doubles above70%HP, adds triples at70% and below, rotates
  order by round below35%. Phase is sampled only when starting a complete pattern.
- Endless menu selects Very Easy/Easy/Normal/Hard/Very Hard, default Normal; local
  preference snapshots before combat. Pace factors are .6/.8/1/1.25/1.5. Intro, run
  and results show the level. Continue and Restart retain the run level; only menu
  changes a new run. Tutorial remains independent.
- Recovery1.7s or2.5s for axe roles; elite subtracts200ms. Each round subtracts120ms
  for at most8steps, floor700ms, then divide by difficulty pace. Combo window is the
  immutable ScoreRules base1.5s divided by the same factor:2.5/1.875/1.5/1.2/1s.
  Renewal, expiry and HUD share that value; animation/contact/chain/parry windows
  and Time.timeScale are unchanged. Enemy HP, damage and300ms cues do not scale.
  Chains keep120ms between clips. Riposte Hurt preserves the original swing-end
  clock instead of accelerating the next strike into the player's success pose.
  Stun, Slayer and death cancel remaining strikes; recovery starts a fresh pattern.
- All18 approved gif-pack strips are imported unchanged:96x64 cels,100ms/cel,
  Point/16PPU/uncompressedRGBA32. Sword/shield contact cels5/2; axe6/4 (zero-based).
  Early sword/shield thrust holds pose0 for100ms before playback to retain300ms cue.
  Each form owns Idle/Walk/Attack1/Attack2/Hurt/Die; stun/Slayer reuse that form's Hurt.
  The original skeleton controller/GUIDs remain for the tutorial configuration.
- Initial Fight only. Continue after the first five death animations loads AND starts the
  next opponent once. Carry only HP/score/combat time/letter counts/parries/progress;
  scene reload resets combo, queues, stamina, cue, stun, Slayer and effect bindings.
  Boss victory ends the run without healing or a seventh opponent. After the death
  animation, VICTORY / BOSS DEFEATED shows the final statistics with Restart, Menu
  and leaderboard; Enter/Continue cannot resume it. Intro/HUD show DUEL x/6.
  Death shows killer/attack and defeated,
  score, accuracy, parries, combat time and local best. Restart clears statistics
  while retaining difficulty; menu clears the run.
- Local best partitions by difficulty (progress before score). Normal retains
  EndlessBestDefeated_v1/EndlessBestScore_v1; other levels append /{difficulty ID}.
  EndlessProfile_v1_ stores base-role/elite encounters/kills and first-boss,
  triple-chain-parry and undamaged-boss achievements. They add no player power.
- The existing modal lists community Endless by default, Classic separately, plus
  opponent profiles and achievements. Closing consumes that frame's shortcuts.
  Normal contract is leaderboards/endless_v1/{LB_LocalUID}; four other levels use
  leaderboards/endless_v2/{difficulty}/{uid}. Payload stays name,defeated,
  score,updatedAt, ordered progress DESC/score DESC/time ASC/uid ASC. Fetch completes
  the1000th-place progress tie before sorting/trimming; best writes use ETag with
  at most3conflicts. Read errors never become empty records. Local best is saved
  before terminal victory/death networking; EndlessPending_v1 retains the best unsent result
  per difficulty (Normal retains its legacy key, others append /{ID}). Retry on
  table open/another completed run uses the saved partition, regardless of menu selection.
  Community rows show defeated/score, without derived round. Existing multi-round
  records are preserved and can rank above new six-defeat results.
  Modal filter defaults to run/menu level and ignores previous requests after a
  filter change or close. Classic contract stays unchanged.
- Difficulty Quick99 passes config/persistence/run/recovery/combo/provider behavior.
  Native required-round coverage and final-candidate progress are recorded in
  docs/plans/endless-duel.md and verification.md. Five-level selectors, modal races,
  durable pending/reload, six tutorial lessons and both sizes have fixture coverage.
  A WebGL trace reproduced post-Slayer idle/hit lock with a frozen outgoing Slayer
  state: AttackSpeedMul's authored default0 was saved/restored on Slayer exit.
  Its neutral default is now1; active Slayer remains3.5 and clips/contact are intact.
  Actual lifecycle regression has negative/positive receipts. On the same fixed
  archived input, Quick99/99 and native matrix22063checks/66wins pass.
  Pending/tutorial129, selector/focus40 and audio11 also pass on that player.
  Later concurrent UI/input edits are outside this frozen candidate.
  HP/recovery values remain playtest tuning. Automated reference targets are
  normals15-30s/boss45-75s at200ms/letter; three-person recognition/retry evidence
  remains explicitly unverified. No production Firebase change or publication.

## Previous single-duel baseline

Source-backed inventory, 2026-10-16. Quick and browser flow have runtime evidence;
see verification for profile status. These are current behaviors, not new requirements.

- Build scenes: `Assets/Scenes/MainMenu.unity`, then `SampleScene.unity`.
  MainMenuUI auto-focuses name input; Enter or Play validates and loads SampleScene.
  PlayerIdentity persists the name via PlayerPrefs, normalizes allowed characters,
  and clamps to 10 characters (menu validation defaults differ; inspect serialized values).
  With no saved name, EnsureLoaded creates and saves `Guest` plus four uppercase
  hex characters through TrySetName. Existing names stay unchanged; the menu's
  existing field supports immediate Enter/Play or an edited name with the same validation.
- TypingManager keeps current/next/third in a synchronous queue. ProcessInput is
  the only letter path: comparison ignores case and nonletters are ignored.
  Completing a word emits the existing correct/advance events once, then promotes
  the queue in the same call. Input continues during presentation; a multiword
  burst in one frame paints the final queue without stacking slides. A single
  completion presents the new queue at+66native pixels and settles over100ms
  cubic-out. No fade/hold/spawn delays or separate type-ahead queue remain.
- The current word stays fully visible, with accepted positions colored in place.
  Wrong input resets its progress, retains next/third and the remaining timer,
  and holds red180ms. Another error restarts the feedback; the next correct input
  clears it. SetPractice resets queue, rewards, feedback, slide and burst state.
  The three58px cells/gap8 have native centers94/160/226; whole-word raster
  advance determines integer alignment, never the remaining suffix. VT32316 uses
  a static Point bitmap atlas with1px blank gutter for TMP's bitmap quad padding.
  Whole-row masking contains slides; no outside-pool per-cell clipping rule exists.
- Scene attackTimeLimit remains3seconds. Timeout promotes immediately and breaks
  combo without awarding a completed word. The word clock uses scaled delta time;
  presentation/error clocks use unscaled time. Pause/reading/end locks stop input
  and clocks. Browser system prefers-reduced-motion cancels slides immediately
  while retaining error colors, with no new setting. OnInputResult remains one
  event per letter attempt, including retyping; TypingInputSfx keeps the existing
  Kenney tick_002/error_004 path, gain/cooldown/voice limits and muted-voice guard.
- Slayer rewards a character position once per current-word occurrence; mistakes
  preserve reward history and promotion starts a new occurrence. Tutorial rewards
  cannot be farmed by retyping. Quick104/104 passes the input/event/SFX/reward and
  imported-font/pool/HP-fill checks. Final r7 WebGL/release acceptance remains
  pending in docs/plans/ui-kit-v2.md; this is not a publication or WPM claim.
- WordPoolConfig splits lines, trims, lowercases and removes duplicates by default.
  Empty pools return null; TypingManager falls back to `type`.
- PlayerInputGate combines hit, dead and multiple parry-pose locks; dead/hit/pose
  block attacks and opening a parry. typing continues during parry. PlayerController keeps at most one completed-word
  slash until the pose ends and CanAttack is true; pause holds it, death/Slayer/exit clears it.
- Space triggers the parry pose configured by `Assets/Data/Attack/PC_Parry.asset`.
  Animation events open/close the window; strikes have early/late grace. Successful
  parries cancel enemy strikes briefly, grant an iframe and emit scoring/VFX events.
  PlayerHealth also treats damage during an active window/success as parry success.
- Enemy stamina drains from hits/parries; stun activates Slayer. Slayer attacks on
  newly rewarded character positions, with damage/tint progression; retyping cannot farm rewards. Read PlayerSlayerMode,
  EnemyStamina and EnemyStunController together before changing this lifecycle.
  Scene enemy stamina is 250; each parry costs 25. Class/prefab defaults differ.
- Player body and sword share one sprite. `PlayerAttackSorting` raises that sprite
  to order1 for the normal attack and all three Slayer states tagged `Attack`,
  plus `player_parry` and `player_parry_success` tagged `Parry`;
  idle/hit/death restore the captured original order0, matching skeleton order0
  on the existing sorting layer. Transition destination takes precedence, pause
  holds the displayed order, and disabling the component restores the original.
  This follows the attack/parry animation state, independently of the damage window.
- ScoreRules asset: word 10, parry 30, Slayer character 2; multiplier starts at 1,
  caps at 3, adds .15 after combo activation; 5 words activate normal combo,
  Slayer threshold 0, combo window 1.5 seconds. Timeout resets combo, not total score.
- SampleScene overrides player HP to 40 and skeleton strike damage to 5.
  Skeleton HP is 850 (calibrated median90.706s; see core-match plan).
  Skeleton restarts its one heavy swing at time zero; cue/swing sound at .7s,
  parry resolution and one damage impact at 1s. Gold cue stays in the combat area.
  CombatSession freezes score, unpaused combat seconds, letter accuracy and successful
  parries at the first lethal hit. No letters displays an em dash. Input/score stop
  during death; the existing death event shows the victory/defeat overlay.
  Death cancels the stun coroutine even when Slayer has disabled its component,
  so the return to idle cannot interrupt the death clip.
  Escape pauses/resumes, R restarts from overlay, M returns to menu.
- Skeleton art uses36 original-pose frames: Idle8,Attack1 10,Hurt5,Die13, with
  cool hero-armor shading inside unchanged source alpha. Sword/grip/slash RGBA,
  import metadata and animation/events remain unchanged. Hurt1 retains its baked
  full-body flash; Stun uses Hurt3/4/3 and hitBySlayer uses Hurt1/3. Source/native
  assets and the approved sample hashes live in `design/skeleton-hero/`.
- First Play enters the six-lesson English interactive tutorial. Menu Tutorial
  replays it; How to Play remains a separate quick reference. TutorialProgress_v1
  stores Unseen=0,Skipped=1,Completed=2 independently of HowToPlaySeen. Completing
  the last challenge saves completion; skipping a replay cannot erase completion.
  MusicVolume/SfxVolume default to1,persist from menu/pause sliders,and multiply
  independent fade/duck/scene envelopes. Restart retains these and the name.
- HitStopper briefly lowers scaled time and restores the original pre-hitstop value.
  Replacing an expired coroutine before it resumes must preserve that original
  value, including slow motion; Quick regression covers both stacking settings.

Change only behavior explicitly requested. Update this file after relevant verified changes.

## Interactive tutorial

- TutorialRun selects tutorial versus ordinary SampleScene and retains only the
  in-session lesson checkpoint. TutorialDirector verifies BLADE plus an actual hit;
  two words with the existing3s timer; two actual target-root parries; a final parry
  from25/250 stamina causing real stun/Slayer; FIRE completion plus a Slayer hit;
  then two words and one parry in the same challenge. Enter/click only starts
  practice or advances an already passed lesson. SPACE remains combat Parry.
- Reading uses a distinct GamePauseController lock. InputBlocked combines that
  lock with pause,including resume's camera transition; Resume keeps reading
  frozen. UI confirm events retain existing pause-safe audio routing. Practice
  uses existing TypingManager.ProcessInput,animations,hitboxes and Parry/Slayer
  timing. First attack practice holds enemy attacks and the word timer.
- Ordinary lesson transitions,timeout and getting hit reload the current scene
  checkpoint at full HP. Stamina break to Slayer stays live in the same scene and
  holds its real duration while reading. Expiry or Restart during Slayer repeats
  the stamina-break checkpoint. Previous lessons survive an attempt retry.
- Skip Tutorial and final Start Battle load an ordinary fresh40/850HP match with
  250stamina,zero score/statistics and no active Slayer/queued attack. Name and
  audio preferences remain. LeaderboardSubmitter rejects tutorial submissions at
  its existing public entry point; Firebase schema/public provider API are unchanged.
- TutorialOverlay keeps the existing HUD/arena/tray anchors. Gold emphasizes the
  tray,cue or stamina; mixed practice switches emphasis when the cue appears.
  The background covers only the instruction strip so the real cue remains
  visible in the gap between action buttons. Character art/animation/VFX remain.
  See the tutorial continuation in combat-feedback and verification for actual
  profile and release receipts.

## World lighting / parry clash prototype (2026-10-18)

Earlier pixel-ray prototype published to itch.io at user request2026-10-19; not merged.
Current thicker-ray revision, Current/A/B/C comparison and BCompact are separate art-review samples and are not published.
Full surface art, B/C and parent integration remain separate.

- Successful OnParrySuccess emits ParryClash at ParrySparkAnchor: contact2,
  main12/front28/back16/embers8 (66 total, five systems). At16PPU main hot head/
  body/tail are .1875/.125/.0625WU; front head/body .09375/.0625WU; back remains
  .03125WU at the head, darker and behind actors. Count/head-width/lifetime/
  brightness remain editable per layer. Hard UV masks are selected once50/30/20
  (bevel/offset/short tail), with a hot leading head and sub-bloom body. Width
  holds through65% then steps down once. No world-position snapping.
- Burst fan rotation ?6deg, spread90?110%, energy ?8%; angles jitter within strata.
  Custom1 carries mask/body ratio/head source pixels/independent length ?15%.
  Three of the12main rays use4/3pixel head/body, short55?70% length multiplier
  and Z toward camera4?6WU/s; distinct XY strata keep these fragments separate.
  Back Z goes away2?4WU/s. Mirror changes only X. Lifetime and placement remain.
  Inspector groups expose fan and camera-fragment settings. Shape migration v1
  updates the same prefab/vertex streams once, preserving later tuning, all26
  object IDs and GUID references; no extra systems/materials. Contact flash,
  world bloom/light strength and consuming ember plane remain unchanged.
- Spark Inspector presets Current/A/B/C keep Current as default. A alternates
  55?75%/115?135% spread with 65?130% ray lengths. B uses -16/+45 degree
  clusters with ?8 degree jitter, alternating 95?130%/45?80% dominant lengths.
  C rotates three 110?130% long rays among nine ordinary main rays; the other
  six use45?75%, front uses40?75%. A/B/C use ?15 degree burst rotation and
  ?15% velocity energy. Near-camera widths/depth/masks and all budgets stay fixed.
  Owner-local per-preset sequences survive lease reuse. Automation-only F6 cycles
  presets with a label; emitted shapes remain unchanged. Separate length-support
  migration v1 reserves130% and compensates the shader crop, preserving Current.
- BCompact remains value4; Current is the serialized default. Five existing systems
  emit contact2/main2/front6/back6/embers0,16total/128over eight leases. Fourteen sparks
  launch together from the sword contact in the -16/+45degree lobes,14-24speed
  with existing15degree/15%burst variation. Keep10forward sparks and append4reverse-X
  falling sparks (two Front/two Back); each depth layer has one long/one short reverse
  ray. Reverse only horizontal travel; Y/Z remain independent. Mirror X only. Front Z is negative,
  Back positive; existing foreground/background actor sorting is preserved.
  Two front fragments expire120-180ms; the other12remain the same particles through
  launch braking120ms, gravity9.81fall, one22%vertical/55%X/Zbounce and180msrest.
  Exponential launch drag20/s transitions continuously to X/Zdrag2/s and ballistic Y.
  Second ground contact stops all axes at groundY+1/32. Full trajectory sets lifetime;
  yellow/orange/dark-red cooling and alpha fade from first ground contact through rest.
  Projected speed sets integer16PPU lengths/widths down to1x1; depth scale is clamped
  .75-1.5before pixel rounding. Floor clearance limits silhouette; square1x1 masks
  remain visible. No replacement particles, blur, camera or gameplay changes.
  Fixed14state/6particle buffers and reused custom data avoid per-frame allocation.
  Compact groups disable automatic gravity/collision/velocity-limit/size/color and
  particle simulation; configuration restores on preset reuse. Manual completion
  ignores IsAlive for those frozen systems, preserving exactly-once lease return.
  Unscaled age obeys pause/reading; Clear/re-emission/disable/restart discard state.
  Quick78/78 and native40keyboard clashes/two directions/two sizes verify this revision;
  B<->compact pause/restart passes both directions. Version1migration, all prefab
  bytes/GUIDs/references and Inspector tuning remain unchanged. F6 affects next emission.
  This is a review sample; prior mirror p95 concerns remain separate and unresolved.
- Normal/Slayer/lethal particle assignments are empty in builder, player/enemy
  prefabs and SampleScene. Existing hit SFX, lethal shake, slash, flash, afterimages
  and damage remain. Legacy PixelSparks/CodeManu/Pimen assets are retained unwired.
- ParryClashBurst owns unscaled simulation, pause/tutorial hold, clear and completion;
  PooledVFX owns guarded release. Eight parry leases reclaim the oldest when full.
  ClashLightingRig holds eight pulse states, renders strongest capped pulse through
  one Light2D, and immediately cancels a disabled/released owner.
- WorldLightingBuild runs after ShogunTheme: lit actor/near-environment sprites,
  ambient/key/fill plus clash rig, fixed-body-foot contact shadows and thresholded
  world bloom. Menu background sprites follow the original UI rectangles via
  WorldMenuBackdrop; overlay HUD remains separate. Camera lens/zoom unchanged.
- Secondary _NormalMap textures preserve source GUIDs/rects/pivots/PPU. Current
  maps are draft alpha/material-region bevels, independent of painted luminance;
  anatomical/armor/roof authoring and complete frame/mirror review remain milestone B.
- Quick75, repeated builder and real-keyboard native ten-clash/direction/two-size sample
  pass; see verification and the combat-feedback plan. This is prototype evidence,
  not full tutorial/result/transition/performance/release acceptance.

## Retained combat-feedback baseline (2026-10-17)

The four particle profiles below describe retained legacy assets, superseded by the
prototype bindings above. Nonparticle feedback and gameplay contracts remain.


- PlayerWeaponHitbox captures Slayer/IgnoreHitstop before damage and chooses impact
  after damage, including a lethal hit that immediately exits Slayer. Closed/disabled
  hitboxes, invalid layers, dead targets and locked matches do not emit feedback;
  multiple colliders still count as one target per attack.
- Existing impact/parry pools now own separate scene roots and reclaim active/idle
  instances on disable/unload. PooledVFX resets Animator/color/scale/speed and guards
  duplicate release events. Unscaled animation runs during hitstop and holds on pause.
  PixelSparkBurst updates after pause/tutorial input locks and before particle simulation;
  both groups must finish before release. Results, disable/restart and unload clear them.
- Normal flash is ivory, .06 seconds and at least two render frames; pause holds its
  remaining duration. The existing Slayer silhouette/flash exclusion remains.
- Afterimages retain source sprites/combo tint, with three images every .025 seconds,
  .12-second first life plus .02 per image, .4 initial alpha and .6 falloff. Pause
  holds spawning/fading; Slayer exit, match death, disable and scene unload reclaim
  active images and cancel pending bursts.
- New Kenney impact events use the existing SFX pool/mixer/preferences independently
  of whoosh and parry: medium 000?002 at .65 normal / .45 Slayer, heavy 000?002 at .8
  lethal, +/- .5 semitone jitter. Existing clang uses +/- 1 semitone.
- Impacts use Inspector-editable `Assets/Art/VFX/PixelSparks` prefabs: orange Normal24
  (.45–.65s), ivory Slayer32 (.35–.55s), pale-blue Parry40 (.6–.85s), orange Lethal56
  (.75–.95s). Two world Particle Systems emit70% square dots and30% axis-aligned
  streaks at16PPU, order30; a binary white texture and unlit straight-alpha material
  keep flat stepped colors. Existing camera zoom scales them with characters.
  Force follows attacker-to-target context, with80% forward; gravity18WU/s² and
  fixed standing-foot collision planes give .4 bounce/.4 damping. Eight active bursts
  per owner recycle oldest without suppressing confirmed-hit audio. Slayer exit
  preserves the emitted profile; lethal wins over Slayer. Original Pimen assets,
  licenses and slash animation remain; the old impact prefabs are no longer wired.
  Legacy animated prefabs now use preserved CodeManu CC0 strips; their paths/GUIDs
  remain for compatibility and notices remain packaged, without parallel emission.
- Final combined Quick67/67 verifies physical profiles, real damage context,
  pause/hitstop/pool cleanup, legacy animation compatibility and builder retention.
  Native WebGL24,212checks passes1280x720/640x360 all four profiles, normal/Slayer,
  pause/results/restart; two-direction bounce/playback was inspected separately.
  Direct speaker listening remains unverified. Receipts and release state are in
  [verification](verification.md) and [combat-feedback checkpoint](../plans/combat-feedback.md).

## Shogun presentation (2026-10-17)

- Final builder stage is ShogunTheme/ShogunEnvironment. Original Tiny Pixel Japan
  layers retain their bytes/GUIDs; renderer tint provides jade/coal/ivory, parallax
  follows the camera, dark edge-tree slices leave combat readable. Only old
  environment renderers are disabled; physics/character animation stay intact.
- Reference960x540: arena928x340; foot-local HUD and tray geometry are specified
  in the HUD layout section below. Camera
  ortho340/84 preserves42px/world unit; Slayer ortho retains.78 of normal.
  Impact pixels use the16PPU world scale documented above; camera framing is unchanged.
- Top1000 is a modal in menu/pause, with the existing1000-row dataset,19-row
  visible pool,refresh and scroll retention. Close restores prior focus/screen;
  pause remains paused. Modal R/M/Escape cannot reach the previous screen.
  Hidden pause controls are noninteractive and selection clears on resume,
  preventing WASD/Space from submitting a hidden Main Menu button.
- Menu/help/pause/results/volumes share flat themed controls. Buttons have visible
  hover/focus and1px label depression; panels reveal over120ms unscaled. Four tray
  edges flash180ms on incorrect input/timeout and hold on pause; text does not shake.
  Flat bars use zero fill inset so4px stamina/combo and6px HP/timer remain visible.
  Enemy cue is larger/bold gold; its animation-event timing is unchanged.
- Original Kenney UI Audio click1/switch2 use the existing mixer/SFX preferences
  at.45 gain. UI events ignore listener pause and expire while paused; return/reuse
  resets that policy. Combat sounds continue to obey pause. Existing impact gains,
  whoosh/clang separation,music,flash/hitstop/afterimages remain in place.
- Quick39,WebGL66,native Windows34,gameplay iframe/fullscreen6 and production
  smoke8 passed.40/850HP,typing/damage/scoring/timers/parry/stamina/statistics,
  Firebase schema,public contracts and packages remain unchanged. Direct speaker
  listening remains unverified; see verification/checkpoint for limits and receipts.

## Slayer cinematic restoration (2026-10-18)

- SlayerScreenFX hides/restores the Shogun environment and HUD/tray backgrounds,
  stretches the camera horizontally, and frames both bodies with full-width black
  bars. Fixed standing-body landmarks use collision bounds inset .28 world units
  above and .12 below; padding is8 reference pixels at960x540. Animated weapons,
  sprite poses and VFX do not change those landmarks. CombatHUDLayout owns their
  projection for HUD and filmframe; typing/status sit on the lower bar and tutorial
  highlights follow their targets. Original red overlay/tint remains.
- ScoreCluster stays at screen scale1 in both modes; its camera scale binder is disabled.
  ScoreUI_Flash retains the120ms award-only text pulse. ScoreRules still owns white/
  x2 yellow/x3 red, original points/rounding and x3 cap. Combo sits below the total;
  floating awards use the latest tier tint and the enemy-head anchor.
- The SCORE caption and former corner labels stay hidden in both modes.
  Exit/disable restores saved environment and tray background. HUD positions
  for actors/tray follow the camera; the total keeps canvas scale1 and uses the
  upper-frame layout during Slayer (revision5 below).
  Final builder migrates existing scenes and protects black bars from theme tint.
- Quick46/46 and browser acceptance cover these paths; four render sizes and x3
  exit frames were inspected. See verification and the slayer-restore checkpoint.
  No gameplay timing, damage, scoring formula, public API, GUID or package changes.

## Combat HUD layout (2026-10-18, revision5)

- CombatHUDLayout owns status, score/combo/decay, popup anchor and tray positions.
  ShogunTheme migrates existing UGUI objects in place; GUIDs/slider bindings remain.
- At960x540 each status is112px wide and12px below its fixed body foot. Both names
  are hidden and PlayerNameHUD is disabled so rank refresh cannot reactivate the name.
  HP6 stays red; enemy stamina4 uses Gold(#DCC47C) at y8. Player/enemy groups are6/12px.
  Fixed collider landmarks(.28/.12 world inset) still drive Slayer framing.
- Normal: unframed total font24 at top/right24, including maximum award pulse;
  combo16 right-aligned below, hidden at x1. Slayer: total48/combo24 with glyph gap4,
  total/combo/decay centered horizontally; the whole visible group is vertically
  centered in the upper black frame, including the actual120ms award pulse bounds.
  Position/font sizes follow SlayerScreenFX's220ms bar reveal; interrupted entry/exit
  uses the existing reveal, normal restores24/16/margins24. Last frame geometry is
  retained if lethal removes actors during close. CombatHUDLayout owns layout and
  canvas scale1 independently of camera zoom. Immediate updates, tier colors and
  ScoreUI_Flash pulse duration remain unchanged.
- One active FloatingScore accumulates exact received deltas. Font18 popup follows
  projected enemy head right24/up12 after layout, rises at most18px without restarting
  its rise on awards, then holds150ms/fades450ms after the final award. An award during
  fade restores alpha and adds to the same instance. Pause holds elapsed/rise; lethal
  score freeze allows fade to finish, retaining the final anchor after enemy destruction.
  ResetScore/restart clears the popup and accumulation. Score rules/OnScored are unchanged.
- Tray672x56 is centered12px below the lowest status with no score-row reserve.
  Three word labels/timer/error feedback and tutorial highlights retain their behavior.
- Approved r5 Slayer exception/reference/geometry and preserved r1-r4:design/layouts/shogun/web/contract.md.
  Revision5 Quick74/74 plus targeted QA/native checks cover four sizes, real rapid
  Slayer, pause/resume/lethal/restart; pixel capture confirms visible gap4 and exact
  group center. Receipts, staged browser results and limits are in verification.md
  and combat-hud plan. Tutorial reading panels retain their existing overlay behavior.
