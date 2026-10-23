## Reusable world lighting (2026-10-22)

- Scene selection, world presentation, individual lights, flame sequences and combat
  lighting now use authored profiles under `Assets/Data/Lighting`. Native materials,
  post, bake and URP assets remain their authorities. Builders apply these values;
  they do not regenerate the approved dungeon style. See [authoring guide](lighting-profiles.md).
- World loading accepts a `WorldDefinition`; camera/arena/brain/environment bindings
  use semantic roles. Fixture names/counts and atlas layouts are independent of the
  default room. Profile changes require Editor apply/reload; this adds no map UI or
  hardware quality tiers.
- The approved ten-frame flame, palette, timing, fixed anchors, 24/16 light strengths
  and pause/Slayer phase are preserved. Each instance owns its clock and UV mesh.
  Existing camera impact tuning remains unchanged; pixel recoil uses the active
  world's target height, and zoom composes with its profile lens.

## Combat camera impact (2026-10-23)

- `WorldDefinition.cameraImpact` supplies six authored `CombatCameraProfile` presets before gameplay activation. Dungeon defaults retain normal2%/140ms, parry3%/180ms plus1logical pixel/100ms, received (including fatal)1pixel/100ms without zoom, lethal5%/250ms or captured Slayer lethal2% plus2pixels/100ms. Slayer entry preserves its base lens tween and the migrated200ms Rumble/camera-space gain with30ms delay. Rapid Slayer hits add no transient pulse. See [authoring guide](camera-impact-profiles.md).
- `CameraImpactTimeline` owns each instance's prepared curves, single prioritized envelope and displacement. Requests capture double timestamps; zoom excludes manual Pause and shake uses gameplay time plus the configured HitStopper gate. A skipped attack/hold renders its peak before running release. Only the bound world camera can acknowledge that peak, with a matching serial; later hitches never replay an acknowledged peak. Equal/weaker requests never extend ownership. Runtime has no combat default table or missing-profile preset selection.
- `CameraImpactExtension` applies the sample directly at Cinemachine Noise. Logical pixels use this frame's CameraState lens/position and actual WorldTexture height; no180px/window-size assumption. Combat emits no old impulses; the dedicated channel2 listener is gone and legacy channel1 remains available. Final half-height is base/(1+impactZoom), with the existing analytic foot anchor and fixed HUD/typing. Profile edits apply on bind/reload and survive repeated builders.
- Existing hitbox/parry/DamageAccepted consumers and captured Slayer classification remain. Pause holds pending hitstop/render acknowledgment through exact freeze transitions. Reading/results/Restart/Menu/unbind/disable clear pending and active output, including destroyed-root teardown; existing Slayer exits still complete through results. Public gameplay/provider/storage contracts, damage/typing/scoring/audio/animation are unchanged.
- Focused Unity camera/lens37/37 and three-cycle lifecycle1/1 pass; producer and two-world binding evidence, builder preservation and final WebGL status are recorded in the [camera checkpoint](../plans/camera-impact.md). WebGL verification is in progress.

# Current gameplay

## Original parry on latest main (2026-10-22; local demo)

Both parry clips enter original combo_3_end_2 and play2->3->4->5->idle at400/950ms.
Defense stays250ms. After success, one completed word in[0,800) unpaused real ms
queues one existing attack after recovery; hit/death/Slayer/early interruption
cancel it. Existing hitstop extends animation wall time. Main contact hold uses
that same frame2, with current physical-spark/clean-enemy-cel presentation retained.
Success itself opens no attack window. Latest HUD guide/pause/dungeon remain on
main fd3f21d; parent WIP preserved. Scoped Unity31/native17 PASS; local candidate,
no release. [Evidence and demo resources](../plans/parry-main-demo.md).

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
tray keep their original scale/geometry; tutorial arrows follow the actual scaled HUD RectTransforms.
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
  gameplay reload resets combo, queues, stamina, cue, stun, Slayer and effect bindings; world ownership is retained.
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

- Build scenes: `Assets/Scenes/MainMenu.unity`, `SampleScene.unity`, and additive
  `DungeonWorld.unity`. DungeonSession loads the world once before enabling menu
  input. Start loads inactive gameplay additively, binds camera/environment/actors
  (including native Canvas.worldCamera), activates actor roots and then establishes
  framing from live colliders. The room, camera, presenter and320x180 RT survive
  Restart, lesson/Skip, Endless Continue and return to menu. Menu contains no actors
  or combat HUD/controller. Internal transitions are serialized and additive;
  gameplay is the active scene while the world reapplies its fog/ambient settings.
  Teardown clears both directions of scene bindings, restores Slayer environment,
  camera/pause/reading/time scale and reloads the menu's official saved identity.
  See [world lifecycle checkpoint](../plans/shared-dungeon-session.md).
  In the Editor, Play from active SampleScene adopts that live scene into a fresh
  Endless Fight screen, retaining unsaved Inspector edits. The initial decision
  waits one frame so a simultaneously opened MainMenu keeps ownership. Direct Play
  resets transient tutorial/run/pause state without saving tutorial progress and
  supports the configured disabled domain reload. Player builds keep the menu entry.
  MainMenuUI initially selects Play; Play/Tutorial open the shared preparation
  card and focus the name input. Names use shared 3-10 limits, with raw overlimit
  UI rejection before normalization and TMP characterLimit=10 for editing.
  PlayerIdentity retains its existing normalization/clamping API and PlayerPrefs
  key. Only Start saves the normalized name; Back/Escape retain the text/difficulty
  draft until the menu scene is left. Existing saved names over10 load intact,
  show an error and disable Start until corrected; no automatic migration.
  With no saved name, EnsureLoaded creates and saves Guest plus four uppercase
  hex characters through TrySetName. Clicking background restores the last selected
  main action while no card/modal/scene transition blocks the menu; hover never
  changes keyboard position. Browser clipboard paste is currently unsupported
  in the existing WebGL player; this follow-up does not add a clipboard bridge.
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
- First Play opens the single English HUD guide for unseen players. Menu Tutorial
  reopens it; How to Play remains a separate quick reference. TutorialProgress_v1
  retains Unseen=0,Skipped=1,Completed=2; both legacy viewed values bypass first-Play
  onboarding. Only Start Battle saves Completed. MusicVolume/SfxVolume default to1,
  persist from menu/pause sliders and multiply independent fade/duck/scene envelopes.
  Restart retains these and the player name.
- HitStopper briefly lowers scaled time and restores the original pre-hitstop value.
  Replacing an expired coroutine before it resumes must preserve that original
  value, including slow motion; Quick regression covers both stacking settings.

Change only behavior explicitly requested. Update this file after relevant verified changes.

## Single HUD tutorial guide

- TutorialRun selects a frozen SampleScene guide without lesson/practice/retry
  state or combat preparation. Four separate callout cards sit beside the HUD
  instead of sharing one large background. All groups are visible together: Your HP,
  Enemy HP / Guard, Typing / Timer and Score / Combo. Five static gold arrows
  follow real HUD RectTransforms after layout, including separate HP and guard
  destinations. Existing dungeon, HUD, fonts, panel style and Continue listener remain.
- The callouts explain HP/defeat, HP/guard/Slayer, typing errors/time and score/combo,
  with Space/parry and Esc/pause copy plus one Start Battle / Enter action.
  No lesson tests, additional pages, animation or illustrations; How to Play is separate.
- Reading uses the existing GamePauseController input/time/audio/score lock.
  Pause hides the guide; Resume preserves the lock and returns to the same panel.
  Restart reloads the guide. Menu abandons it without marking it viewed.
- Start Battle guards repeated confirmation, saves Completed, ends tutorial mode
  and reloads through DungeonSession. The reading lock remains through teardown;
  held Enter is consumed before fresh gameplay activation. The selected-difficulty
  encounter intro starts with40HP,180enemy HP,100guard and zero score/statistics.
- LeaderboardSubmitter still rejects tutorial submissions before resolving a
  provider. Public gameplay/provider APIs, Firebase schema and saved key stay unchanged.
- EndlessOverlay clones only the original encounter intro content: all guide copy
  and the guide panel background remain hidden in the clone. Intro width/action
  typography retain their original bounds; live combat has no reading panel.
- Verification and scoped scene/asset evidence: [tutorial guide checkpoint](../plans/tutorial-hud-guide.md).

## World lighting / parry clash

### Reusable MetalClash preset (2026-10-22)

- `ImpactVfxEmitter.Play(ImpactVfxProfile, ImpactVfxContext)` emits without a player
  or ParrySystem. Context captures point, left/right direction, floorY, seed,
  optional recipient ellipse and optional world scale (1.25 default).
- `Assets/Art/VFX/ParryClash/MetalClash.asset` references the approved existing
  ParryPhysics/prefab. Profile stores hot/warm/cooled colors, final fade and local
  pulse intensity/hold/duration/range/offset. Defaults preserve24particles,1px,
  rest<=400ms,lifetime<=1.1s and Point5/70ms hold/280ms expiry.
- `ImpactVfxBurst`/`ImpactVfxPixel` own rendering; `ParryClashBurst` retains its
  serialized script GUID as a compatibility subclass, including historical presets.
  Parry's adapter calls the shared emitter with existing leases; other callers use
  the emitter's bounded eight-lease pool. Builder fills absent references only.
- Physical states and appearance are copied at emission; simultaneous profiles
  and later asset edits cannot change an existing burst/pulse. One-floor/one-bounce
  solver, fixed buffers, unscaled pause-held updates and cleanup remain. Audio,
  damage, recoil and hitstop remain owned by gameplay.
- Scoped evidence/status: [preset checkpoint](../plans/metal-clash-preset.md).

### Pimen flash with thin separated physical sparks (2026-10-22)

- Production BCompact emits24/clash,192/eight leases: contact1/fragments2/front12/
  outer1/far8. The same approved Pimen core/outer remain at sword contact for200ms.
  Eighteen rays/fragments start at the captured contact and travel horizontally
  toward and beyond the recipient independently of facing or later actor movement.
- Six groups in `Assets/Art/VFX/ParryClash/ParryPhysics.asset`: two fragments
  (8-12WU/s,1-3px,100-160ms), two near/two far leads (24-28WU/s,6-9px,
  280-400ms), six near/six far secondary rays (14-20WU/s,2-5px,850-1100ms),
  four1-2px body-perimeter specks (1-2.5WU/s,max450ms). All launch on success.
  Independent angle ranges: fragments-35..35, leads-12..18, secondary-30..35degrees;
  angleJitter0. All rays have base width1px at16PPU. VelocityOnly omits artificial
  Z enlargement; projected velocity still controls orientation and silhouette.
- Independent cooling caps ray length down to1px permanently; later falling speed
  cannot re-stretch it. Tint cools pale yellow/orange independently of floor:
  leads220-300ms, secondary/specks140-220ms, fragments80-120ms. At300ms living
  main rays are1px orange embers (RGB166/51/6), readable during fall/bounce.
  Alpha stays opaque until the final80ms, then fades to zero. Cooling is captured
  and capped by lifetime. This corrects the user-rejected invisible physical tail.
- Main rays: gravity9.81,120ms launch drag5/s, flight X/Z drag3/s, one optional
  bounce22%vertical/55%tangent; leads rest<=80ms, secondary rest<=400ms,
  always capped by group lifetime.
  Near Z=-6..-4, far Z=2..4WU/s; fragments Z=-1..1 and no floor collision.
  Specks retain floor collision/gravity/drag6 without bounce. Captured body ellipse:
  center=body.center+0.2height up, halfAxes=(body.extents.x+1/16,0.3height),
  initial velocity outward. Emitted state does not follow moving actors.
- PhysicalParticleSimulation snapshots group settings and evaluates absolute age.
  AngleSampling.Stratified=0, PixelScaleMode.LegacyDepth=0, coolingDuration=(0,0)
  preserve old/custom behavior and random draw order. Builder fills only missing
  profile references. Fixed40state/particle/identity slots remain for custom profiles;
  production uses22physical slots. Near-zero drag integration remains unchanged.
- Birth renders at age0; first slow-following frame keeps its existing step cap.
  Pause/tutorial reading freeze age, hitstop continues. Continuous Z, preset restore,
  oldest-lease reclaim, disable/unload cleanup and contact+tail completion remain.
  All physical particles expire by1100ms effect age; next frame returns the lease.
  Reused buffers retain zero managed allocations for warmed particle updates.
- Counter hit remains100ms on the first slash cel. Immediate confirmed hit/dedup,
  bright normal spark six cels/160ms, damage/stamina/stun, Slayer/lethal profiles,
  contact metadata, audio, shared shader and bloom remain unchanged. ContactLibrary,
  70ms contact hold,50ms hitstop,1px recoil and Point5/70ms hold/280ms fade remain.
- First thin24 candidate focused Unity89/89 and native100checks pass. Its867
  frames/863pairs were inspected, but user feedback rejects the invisible physical
  tail. Final orange ember follow-up passes focused Unity90/90, native100checks,
  and direct review of1159frames/1155pairs across both directions with VFX visible/hidden.
  Integrated latest-main follow-up passes94/94 focused tests plus WebGL/strict
  repeated builder. User approved the audible demo on2026-10-22; independent subjective listening by
  the agent remains unavailable. See [combat-feedback checkpoint](../plans/combat-feedback.md). Historical
  42-particle receipts stay version-specific; they do not qualify this thin24 output.

- Ground-rest follow-up: user requests a longer visible floor hold. Only near/far
  secondary lifetime/rest change; original launch, cooling, palette, size and XYZ
  physics remain. Focused particle Unity65/65, native36checks, normal/mirror WebGL/strict repeat
  and direct128frame/126transition review PASS. Visible stationary embers remain
  readable229-256ms in these captures; final live age1.043-1.065s. See the same
  combat-feedback checkpoint. Prior94/1159frame receipts
  describe the approved750ms version and unchanged consumers.

### Historical parry iterations (superseded by the approved version above)

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

### Normal strike accents and contact confirmation (2026-10-22)

- `NormalStrikeEffects` listens to the existing attack-window opening: one
  three-cluster gray dust burst, even on a miss. Its stable body-foot anchor is
  one native world pixel behind the actor center; the builder supplies floor Y.
  Dust moves backward, stays low, and fades over180ms. Normal opening is at0ms;
  counter opening remains100ms, its first slash cel. Whoosh/dust follow opening.
- Confirmed nonlethal normal hits use dedicated NormalHitSpark.prefab, original
  Pimen `Hit Effect 01 1.png` Sprite subassets1..6, equal160/6ms holds, once only.
  Emit displays bright frame_1 immediately and cannot advance on its birth frame.
  Existing Pixel.mat is unlit, white renderer color, sorting order3 above trail2.
  Fixed48x48canvas/center pivot and a single8.333333scale preserve original art:
  widest36source pixels at100PPU becomes3world units=48pixels at16PPU.
  Source F6 is transparent; art disappears at133.333333ms, lease returns at160ms.
  Contact/world offset and horizontal facing are retained; no jitter/core/code rays.
  Source PNG/importer and parry/Slayer/lethal assets remain unchanged.
  Normal player trail RGBx.75/alphax.55/emission.05 preserves cel shape/timing;
  gray foot dust remains180ms. Damage values, stamina, stun and routing remain.
- Normal/counter opening enables the hitbox, synchronizes transforms, emits the
  opening hook and immediately queries overlap. Overlap and later triggers share
  TryHit, collision exclusions and per-swing target deduplication. Slayer's legacy
  OnAttackStart retains trigger cadence. Explicit animation-event Sprite references
  select authored SlashLibrary contact segments; transformed segment/collider
  contact is captured before Hurt/Die, then shared by damage feedback and impact
  audio. Geometry positions feedback only; existing colliders determine eligibility.
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

- The canonical builder applies ShogunTheme/CombatR7/UI, then DungeonArenaBuild
  and WorldLightingAuthoring against existing authored assets. It preserves the
  world definition, profile values, native materials/post/URP, disabled legacy
  WetStoneReflection and secondary sprite-map bindings. The old procedural
  WorldLightingBuild and room/style generators are not part of rebuilding.
- Actor controller clips, including the four parry-recovery cels outside the
  standard folders, retain their normal/material companions, albedo, pivots and
  timing. Existing parry hold70ms/end280ms remains unchanged.
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

- Final presentation order is ShogunTheme, CombatR7Build, then
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
  Settings and Top1000 deactivate the CompactPauseLayout panel after saving focus
  (Settings first reparents audio into its modal); Close restores audio geometry,
  activates Pause and restores the opener. PanelReveal cannot reveal the inactive
  panel. Main-menu modals keep their existing visibility behavior.
  Resume keeps compact geometry and pause/time/audio locks through camera exit,
  disables interaction immediately, and rejects repeated Resume/Restart/Menu and
  Escape/R/M during that transition. It hides the canvas before restoring result
  geometry and clearing hidden selection, then releases pause while retaining any
  tutorial/encounter reading lock. Only one camera tween runs at a time.
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
  486frame/glass triangles plus2central pixel-flame triangles; two rear pixel cards add4triangles. Chamber center3.37WU above floor; actual
  fire/light center3.405WU, centered horizontally and2WU in front of combat.
  Main24/range16/+/-6percent; rear16/range15/+/-2percent, all continuous Perlin.
  Current local review replaces rejected NYKNCK/Pixie art with the approved Pinterest
  ten-frame loop (100ms/cel), material-tinted brick red/orange/pale yellow, bottom anchored
  on individual private runtime UV meshes; animation offsets0/.17/.33seconds.
  Maximum projected source height front10/rear12native pixels, width80percent;
  depth-tested flame draw follows environment blur. Current replacement passes focused Unity2/native35checks; user demo approval
  remains pending. Integration receipts below refer to earlier art.
  Rear bodies reuse their existing fire-free shadow mesh; emission follows each local light.
  Pause and Slayer freeze/resume phase; destruction releases only the private mesh.
  Original fire mesh/material/GUIDs stay intact. Private fire material;
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


Stronger parry follow-up2026-10-22: integrated normal6d86af46 passes56/56 focused
physical/parry cases; normal/mirror builds and repeated authoring pass. Four
real-input VFX/hidden variants pass64 checks, with direct995frame/991transition
review of the complete parry/counter/physical tail. Dense recipient-backward fan
passes behind player on both sides. Final source/clip/timestamp limits and
per-frame receipts are in docs/plans/combat-feedback.md; no FPS/soak claim.
