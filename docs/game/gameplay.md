# Current gameplay

## Dark fantasy HUD — local candidate (2026-10-21)

The latest local presentation supersedes older palette/font notes below. Combat HUD
uses coal `#121014`, iron `#302B2B`, antique bronze `#A17B4F/#D1AD73`, ivory
`#F2E7CF`, blood HP `#C34B55/#ED8080`, teal stamina/typing `#73B6AA` and gold
combo/parry `#E5BE72`; error/damage stays `#F16B67`. Score total/popup use configured
ivory/gold tiers; the visible multiplier is gold from its first tier. HP glyphs
retain their original face with a hard coal underlay for contrast. Pause uses
isolated coal/bronze modal and button rims. Shared menu art is unchanged.

User follow-up: health/status rails are82 native pixels wide instead of102
(19.6% shorter), retain their height and crisp sliced borders, and stay in their
outer corners. Latest follow-up scales both side status groups (names, HP, icons,
stamina) to80%: effective HP width65.6px. Both player/enemy HP
digits and the difficulty/DUEL line/background are hidden. The two name
backplate Images are disabled; names remain visible over the arena. Score/combo and typing
tray keep their original scale/geometry; stamina tutorial highlight follows80%.
Name content, Cobblestone source sizes56/48/42, Blackbit title only, other geometry, three typing cells,
hidden PARRY/Space/clock, bindings and feedback timing remain unchanged. No gameplay math
or input changes. Native focused HUD11 checks and production combat/pause smoke
passed at320/640 DPR1. Compact follow-up production combat/damage4 captures also
passed and were visually inspected. Latest scale follow-up: six combat/damage
captures at320/640/400x300 pass; score region pixel-identical at320/640.
No full suite rerun or upload. [Evidence and local handoff](../plans/hud-dark-fantasy.md).

Current source brand: **CLACKBLADE**, slogan **TYPE TO SLASH**. Gameplay and stable technical
identifiers remain unchanged by the rename. Native menu960x540/640x360 and
same-artifact name reload verified. Public release: https://micharel09.itch.io/clackblade
(upload19876564). Endless online reads still401; Firebase unchanged.

## Combat HUD readability (2026-10-20)

- CombatR7Build keeps existing geometry and supplies VT323 static bitmap atlases at native8 for HP/difficulty-duel/multiplier,10 for clock and12 for total score. Point/Clamp, one-pixel gutter; no autosize/wrap. BlackletterTitleBuild protects the concrete TypingManager, CombatR7Status and CombatHUDLayout bindings. Names, PARRY, decorative/menu/modal text and all three complete typing text components retain their previous treatment.
- Four flat non-raycast backplates use #14111E/.88 at native player(3,0,102,11), enemy(215,0,102,11), difficulty(108,45,104,13), clock(256,145,32,14). They render before HUD/tray/labels; CombatR7Status synchronizes the difficulty Image with its label. Score-frame alone is tinted(.75,.75,.75,1); score text/pulse, Slayer colors, HP/parry/stamina feedback remain unchanged.
- Strict two-pass serialization, actual atlas pixels/settings, fixed-font content/bounds and existing IDs/typing-component preservation verified. Same-player native feedback, focused restart/death25/25 and score-cluster209/209 cover current-scene lifecycle and four sizes; see checkpoint for the full feedback driver timeout and qualified rerun coverage. HUD stays screen space; existing floor native y138/tray y146, camera, cue and popup mechanisms are unchanged. Representative2.5D lighting/parallax/foreground occlusion remains unverified. [Checkpoint](../plans/score-hud-cluster.md).

## Unified score cluster (2026-10-19; local source)

- CombatR7Build keeps the total at its existing center-top position. ScoreCluster now owns ComboHUD and its existing decay slider: total → multiplier → thin decay track. PlayerStatus contains name/HP only; its former combo frame/socket/icon remain inactive to preserve IDs. Enemy stamina is unchanged.
- ScoreUI_ComboHUD hides both multiplier and decay at ×1 and reveals both above ×1. ScoreSystem, ScoreRules, thresholds, expiry/timeout, input, damage and providers are unchanged. CombatR7Status swaps only the grouped fill between normal and Slayer sprites.
- At960×540, ComboHUD is(0,69,294,54) relative to ScoreCluster; multiplier occupies(0,0,294,30), decay(12,36,270,12). ROUND moves fromnative y38 to46 to clear the added row. Total position/pulse, names, HP and typing tray remain fixed.
- Repeated builder and native209/209 scoped UI checks pass at320×180,640×360,1280×720,400×300, including real parry renewal, pause, expiry/timeout, restart and natural Slayer exit. See [verification](verification.md) and [checkpoint](../plans/score-hud-cluster.md). Published with the HUD release on 2026-10-20, upload19892576.

## Combat HUD feedback (2026-10-20)

- Dòng dưới score cluster hiển thị difficulty của run và DUEL n/6; Tutorial dùng TUTORIAL. Slayer giữ dòng, InputBlocked/results ẩn dòng; EndlessOverlay giữ intro/Continue riêng.
- CombatHUDFeedback quan sát combat: Space sáng khi cùng predicate TryBeginParry cho phép, giảm sáng khi khóa, vàng theo cue thật khi ready hoặc window đang mở, xanh ngọc120ms từ OnParrySuccess. Success iframe80ms, input/window/grace/SFX không đổi.
- Stamina dùng shield v2 nguyên bản10×10; cost thật>0→0 kèm stun sống mới đổi warning. Hết stun trả shield. Theo quyết định mới, không hiện chữ GUARD BREAK vì Slayer đã báo trạng thái. Node cũ giữ ID/binding, để trống và disable; không còn timer/first-break flag. Icon vẫn đổi theo stamina/stun thật, pause giữ trạng thái.
- Số HP sống≤25% đỏ; viền Lerp đỏ về màu gốc180ms từ damage thật, hit mới reset. Init/carry/restore không phát damage event. Theo quyết định user, HUD không có xử lý reduced-motion riêng; không đổi motion cũ ở typing/UI khác. Component hủy event/reset khi disable; logic stamina/HP/scoring giữ nguyên.
- Local source751de45: scoped Unity4/4 và native2353 PASS trên normal path giữ nguyên; xem [checkpoint](../plans/score-hud-cluster.md) cho provenance, các lượt không đạt và giới hạn. Release child uses the approved palette/public UI64de5b3a baseline. Published upload19892576 on 2026-10-20; production/CDN/native receipts are in verification.md.

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
- Finite first-boss QA: current-snapshot Quick116/116, five-level matrix4413/30wins
  and pending/tutorial86 pass; see verification and Endless checkpoint for exact input,
  offline-test status and cleanup. No production change by this task.
- Historical repeating-mode Quick99 passes config/persistence/run/recovery/combo/provider behavior.
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

## World lighting / parry clash

### Pimen flash with reusable floor physics (2026-10-21)

- Default BCompact retains the approved licensed Pimen Hit Effect01 core/outer
  cels, material, pivot, size, tint and200ms timing from c12c3de. Budget24/clash,
  192/8leases: core1/main2/front12/outer1/embers8. Eighteen directional particles
  (sixteen rays and two short fragments) launch from captured sword contact
  toward the attacking enemy. The continuous -20..50degree fan adds+/-3degree
  jitter at14-24WU/s. Actor position owns direction, independently of facing scale;
  movement after emission cannot redirect the existing particles.
- Four additional1-2pixel specks scatter around contact on the player side,
  radius.12WU/offset.08WU, speed1-2.5WU/s, horizontal/depth drag6/s,
  max450ms/no bounce. Horizontal displacement stays below.6WU. The twelve
  strong reverse-X rays are removed. Front carries eight near rays plus four
  specks; Embers carries eight far rays. Back remains the Pimen outer renderer.
- PhysicalParticleProfile is an Inspector-editable ScriptableObject; production
  uses Assets/Art/VFX/ParryClash/ParryPhysics.asset. Groups configure emission,
  count, speed, depth, pixel dimensions, lifetime and per-group floor physics.
  PhysicalParticleSimulation snapshots settings and evaluates absolute-age paths
  independently of combat/ParticleSystem rendering. ParryClashPixel renders the
  resulting positions, projected-velocity silhouettes and hot-to-cool tint.
  Builder fills only an absent profile reference, preserving custom profiles and
  tuning. BCompact reserves22physical slots; other consumers choose their capacity.
- Sixteen main rays use120ms launch drag20/s, flight horizontal/depth drag2/s,
  gravity9.81, one optional22%vertical/55%tangent bounce and180ms rest/fade.
  Lifetime derives from the complete floor trajectory, capped5s. Two short
  fragments end120-180ms. Floor only in v1; no per-particle GameObject/Rigidbody.
  Zero drag/gravity and floor-level/upward/below-floor births have focused tests.
- Birth renders at age0 before advancing physics/contact time; tiny specks may
  start inside their configured region. A first following delta>33.3ms takes one
  16.7ms step so a cold render cannot skip launch; subsequent frames use full
  unscaled delta. Pause/tutorial reading freeze age. Fixed22state/22particle
  buffers, stable identities and reused Custom1 data avoid per-frame managed
  allocation; local seeds do not consume gameplay random. Clear/release/disable
  reset states. Completion waits for both authored contacts and the physical tail.
- ParryContactLibrary v4 still captures transformed blade pixels before pose
  changes and holds the approved cels70ms. Enemy SlashVolume suppression200ms,
  hitboxes/50ms hitstop, Point light5/70ms hold/280ms fade, bloom and one-pixel
  camera recoil remain. Audio gain/jitter/ducking, outgoing normal/Slayer/lethal
  effects and legacy Current/A/B/C remain unchanged.
- Focused Unity37/37 and strict repeated builders pass. Native Edge155/Intel UHD
  D3D11 at1280x720 passes15right+15left checks with real Space, pause and second
  parry cleanup. Left uses an editor-only mirrored-actor build fixture. All
  55right+55left captured frames/54+54transitions directly inspected; no long
  reverse spray, visible floor penetration or residue observed. Captures/probe
  timing do not certify every engine frame or performance. [Evidence and authoring
  contract](../plans/parry-physical-particles.md).

### Historical parry iterations (superseded by the candidate above)

Earlier pixel-ray prototype published to itch.io at user request2026-10-19; not merged.
Current/A/B/C and BCompact comparison clips remain art-review samples. BCompact24 code is published in the 2026-10-19 release; BCompact24 is selected by the production parry owner after the default-selection correction.
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
- BCompact remains value4 and is now the production parry owner default; the builder
  pins player/scene owners to it. Current remains an explicit legacy variant. Five existing systems
  emit contact2/main2/front10/back10/embers0,24total/192over eight leases. Twenty-two sparks
  launch together from the sword contact. Keep the original10forward spark draws in
  -16/+45degree lobes at14-24speed; append12reverse-X falling sparks stratified
  -12..36degrees with shared +/-15degree rotation and +/-2degree jitter. Reverse X
  uses32-44speed times existing energy; Y uses14-24speed. Six reverse sparks per
  depth layer include three long/three short rays. Reverse only horizontal travel; Y/Z remain independent. Mirror X only. Front Z is negative,
  Back positive; existing foreground/background actor sorting is preserved.
  Two front fragments expire120-180ms; the other20remain the same particles through
  launch braking120ms, gravity9.81fall, one22%vertical/55%X/Zbounce and180msrest.
  Exponential launch drag is20/s, except reverse X uses10/s; it transitions
  continuously to X/Zdrag2/s and ballistic Y.
  Second ground contact stops all axes at groundY+1/32. Full trajectory sets lifetime;
  yellow/orange/dark-red cooling and alpha fade from first ground contact through rest.
  Before the voxel preview revision, projected speed sets integer16PPU lengths/widths down to1x1; depth scale is clamped
  .75-1.5before pixel rounding. Floor clearance limits silhouette; square1x1 masks
  remain visible. No replacement particles, blur, camera or gameplay changes.
  Fixed22state/10particle buffers and reused custom data avoid per-frame allocation.
  Compact groups disable automatic gravity/collision/velocity-limit/size/color and
  particle simulation; configuration restores on preset reuse. Manual completion
  ignores IsAlive for those frozen systems, preserving exactly-once lease return.
  Unscaled age obeys pause/reading; Clear/re-emission/disable/restart discard state.
  Integrated Quick117/117 verifies24/192,20single-bounce paths and24seeds/two directions at300ms:
  >=8reverse sparks travel>=2.5Xunits and>=6retain2pixel length. Native24particle normal QA passes20keyboard clashes at640/1280 and B<->compact
  pause/restart; mirrored native QA was canceled by user. Prior mirror receipts
  cover16particles only. Version1migration, all prefab
  bytes/GUIDs/references and Inspector tuning remain unchanged. F6 affects next emission.
  Comparison clips remain review samples; BCompact24 is now used in production parries.
  Prior mirror p95 concerns remain separate and unresolved.
- Historical voxel preview replaced BCompact's flat core/ring and billboard sparks
  with one shared closed cube mesh/material. Two offset contact cuboids (maximum
  edge .5WU) hold alpha60ms and expire200ms; no expanding ring. Twenty-two fragments
  use 2-3source-pixel edges, 1:1:1 or1:2:1 proportions, three-axis rotation and
  hard-normal face shading with cooling emission. Analytical launch/fall/bounce
  timing and RNG draws remain unchanged; renderer floor support accounts for mesh
  volume and settles rotation onto a flat face by the end of the existing bounce.
  Manual unscaled ages obey pause; reuse restores legacy mesh/material/vertex
  streams, render alignment and size/rotation/simulation modules. Voxel migration1
  preserves existing GUIDs/tuning and creates only its shared mesh/material refs.
  Frozen compact contact completion uses particleCount, avoiding IsAlive on a
  stopped simulation clock. Earlier flat-ring art was rejected by the user;
  its technical receipts do not qualify the voxel appearance.
  Compact light peaks5, holds70ms, smoothstep-fades to0at280ms (half2.5at175ms).
  Each of eight pulse states owns its timing/profile; mixed overlap takes strongest,
  never sums. Legacy profiles keep140ms quadratic falloff and existing intensity cap;
  light radius and world bloom are unchanged. Voxel focused Quick19 passes;
  WebGL build and Edge155 preview pass12checks (two real noF6 parries at1280/640, no exceptions/provider writes); voxel faces were inspected at both sizes. Earlier flat-ring native/performance
  receipts are historical, not voxel validation. Publication is deferred until
  visual review; see combat-feedback.
### Shared 2.5D presentation

- Full 2.5D conversion source wires Normal6/Slayer8/Lethal14 closed-mesh impacts.
  Player received4/received-lethal6 and their light pulses are removed (2026-10-21);
  PlayerDamageVoxel/ReceivedImpact stay detached after repeated builder runs.
  Their scripts/assets/GUIDs remain for compatibility. Red Hurt, HP/SFX and the
  white sprite death remain unchanged. Slayer tint is captured
  before damage can exit the mode; normal lethal has no Slayer emission. New impact
  random is isolated from gameplay random. Eight active leases per emitter bound
  impact particles to112outgoing, alongside192parry particles. Normal-strike
  accents have a separate eight-lease cap (at most24particles), for328total.
  SlashVolume follows the actual source Sprite cel through SlashLibrary;9atlases
  have slash-only RGBA separated into durable layers and thin closed meshes.
  Actor pixels outside the authored masks, sprite metadata and clip events/timing
  are preserved. Screen flash/cues/HUD remain2D; afterimages retain3sprite snapshots,
  120/140/160ms lifetimes and25ms cadence, with shallow Z offsets and12active cap.
  Existing hit SFX, lethal shake and damage remain. Legacy PixelSparks/CodeManu and
  unused Pimen prefabs remain unwired; original Pimen01 supplies the current parry.
  Prior focused27/shared native70 receipts describe these unchanged consumers;
  earlier parry native66 includes historical received smoke. Particle removal
  passes scoped Unity42 and keyboard178 checks at1280x720/640x360; see verification.
  Existing broader visual direction remains with its original review.

### Normal strike accents (local preview, 2026-10-21)

- `NormalStrikeEffects` listens to the existing attack-window opening: one
  three-cluster gray dust burst, even on a miss. Its stable body-foot anchor is
  one native world pixel behind the actor center; the builder supplies floor Y.
  Dust moves backward, stays low, and fades over180ms. No sprite/event timing changes.
- Confirmed nonlethal normal hits use dedicated NormalHitSpark.prefab, original
  Pimen `Hit Effect 01 1.png` Sprite subassets0..6, equal160/7ms holds, once only.
  Existing Pixel.mat is unlit, white renderer color, sorting order3 above trail2.
  Fixed48x48canvas/center pivot and a single8.333333scale preserve original art:
  widest36source pixels at100PPU becomes3world units=48pixels at16PPU.
  Source F6 is transparent; art disappears at137.142857ms, lease returns at160ms.
  Contact/world offset and horizontal facing are retained; no jitter/core/code rays.
  Source PNG/importer and parry/Slayer/lethal assets remain unchanged.
  Normal player trail RGBx.75/alphax.55/emission.05 preserves cel shape/timing;
  gray foot dust remains180ms. No event, damage, audio or hitstop changes.
- Normal sprites share the existing eight-impact cap; dust has a separate eight-lease
  budget in the existing owner pool.
  Pause/tutorial reading freeze age; results, owner disable/restart/scene unload
  and Slayer activation clear them. Builder preparation reuses assets and binds
  a single component. Duration is simulation age; visible endpoints are quantized
  to render frames and pause/reading can hold them. Evidence and local-only
  handoff status are in `docs/plans/combat-feedback.md`.
- ParryClashBurst owns the stock contact clock and analytical unscaled debris clock,
  pause/tutorial hold, clear and completion;
  PooledVFX owns guarded release. Eight parry leases reclaim the oldest when full.
  ClashLightingRig holds eight pulse states, renders strongest capped pulse through
  the active scene's Light2D or3D combat Point light. In the3D arena a winning compact
  parry uses its dedicated contact Point light and mutes the other combat pulse. Disabled or
  released owners cancel immediately.
- The integrated dungeon uses the task's full3D environment: pixel-stone room,
  5degree/FOV35 perspective, owned320x180HDR Point world RT and presentation camera.
  Three steady fire-centered sources plus the latest main transient combat/parry
  pulses shade actors and floor. Slayer hides the room, steady sources and their
  rig together, then restores them and the camera; standalone sparks remain owned
  by the latest gameplay/VFX systems. Final QA114427/native25 and6targeted offline
  checks pass. No integrated performance/FPS or publication claim.

- WorldLightingBuild runs after ShogunTheme: lit actor/near-environment sprites,
  ambient/key/fill plus clash rig, fixed-body-foot contact shadows and thresholded
  world bloom. The preserved WorldMenuBackdrop is disabled by the final dungeon
  UI pass; the menu uses four original gameplay dungeon layers as UGUI images.
  Overlay HUD remains separate. Camera lens/zoom unchanged.
- WorldLightingBuild also follows actual player/enemy Animator controller clip
  references, including `Art/male_hero/individual_sheets/player_parry_success.anim`.
  Its four recovery cels now have aligned normal/material companions; missing
  material binding had made the whole cel metallic and dark. Albedo, pivots,
  slicing and timing are unchanged. Parry hold70ms/end280ms is unchanged.
- DungeonFullEffectsBuild no longer creates WetStoneReflection and disables the
  serialized legacy node, renderer and component. Stone lighting/material highlights
  and cast shadows remain; no reflection capture camera/RT through pause or Slayer.
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

## UI presentation (dungeon skin 2026-10-19; legacy Shogun migration retained)

- Final builder order is ShogunTheme, WorldLightingBuild, CombatR7Build, then
  DungeonUISkin for menu/help/Collection/tutorial/pause/results/Continue only.
  Original Japan assets/objects remain preserved and disabled. The final
  BlackletterUILayout/DungeonMainMenuBuild pass presents a static320x180 dungeon
  background, brand logo56 at top left, and five unboxed Cobblestone30 menu items
  at bottom right. Play/Tutorial open a centered420x320 preparation card using
  the existing name/difficulty/validation and Start dispatch. Initial focus is
  Play; arrows/Tab navigate, Enter opens, Back/Escape restores the opener. Only
  Start depends on name validity. Settings/Top1000 retain their compact modals.
  Combat HUD, physics and character animation remain unchanged.
- Reference960x540: arena928x340; foot-local HUD and tray geometry are specified
  in the HUD layout section below. Camera
  ortho340/84 preserves42px/world unit; Slayer ortho retains.78 of normal.
  Impact pixels use the16PPU world scale documented above; camera framing is unchanged.
- Top1000 is a modal in menu/pause, with the existing1000-row dataset,19-row
  visible pool,refresh and scroll retention. Close restores prior focus/screen;
  pause remains paused. Modal R/M/Escape cannot reach the previous screen.
  Hidden pause controls are noninteractive and selection clears on resume,
  preventing WASD/Space from submitting a hidden Main Menu button.
- Non-combat panels/modal/keycaps use byte-identical v2 components; small controls
  and rows use the original R7 secondary rim after its ten-pixel socket reserve,
  with border2 and no new raster. Focus/selected states use the gold rim; runtime
  validation/disabled tints, result colors, row tints and tab selection share the
  coal/ivory/gold palette. Collection retains its four tabs and table/text lists.
  The prompt background encloses both existing instruction lines through y80;
  text/action/highlight and the corrected live duel-label positions stay intact.
  Existing1px label depression and120ms unscaled panel reveal remain. Four tray
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


## Integrated dungeon and lantern

Hanging combat lantern integrated2026-10-21 from branch
`micharel09/combat-hanging-light`:
  Original KayKit FBX472triangles unchanged; derived frame446 plus thin glass40,
  total486submitted lantern triangles. Chamber center3.37WU above floor; actual
  fire/light center3.405WU, centered horizontally and2WU in front of combat.
  Main24/range16/flicker0, rear16/range15/+/-2percent. Private fire material;
  iron/wood emission0. Glass URP Lit black/alpha.12/emission(1,.38,.08),
  no specular, ZWrite off, no cast/receive shadow. Environment child follows
  Slayer hide/restore, excluded from flame flicker. Within the task dungeon baseline,
  camera/floor/post/API receive no further lantern tuning.
  Isolated QA110751 strict twice-run builder and native22gameplay/9camera checks PASS;
  reused lighting lifecycle2/2. All9camera endpoints plus163frames/162transitions
  in selected complete pan reviewed; no observed alpha pop or large floor outline.
  One20s run per player, same Intel UHD/D3D11/1280x720: p9533.225->32.575ms;
  not isolated GPU cost/equivalence/FPS evidence. No publication. Integrated verification: final QA114427/playerfdc8253ed7a62b1f,25native checks
  and6scoped offline checks PASS; see [checkpoint](../plans/combat-hanging-light.md).
