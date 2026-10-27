## Typed READY summoning (2026-10-27)

- After the completed death animation, a session-owned amber pixel seal fades in
  with environment gain1 to .85 over240ms. Actors and HUD retain their brightness.
  READY is centered in a compact borrowed pixel frame (native8px horizontal/4px
  vertical padding around the complete word), with a steady cream
  TYPE READY TO CONTINUE label 6px above it. Rest remains unlimited, Enter is inert.
- The original64x64 Magic circle build frame11 by a-small-smart-potato replaces
  the procedural ring/marks. Native PNG colors and structure are preserved on a
  floor quad; Point/Clamp, no mipmaps/compression. Whole-ring alpha rises over80ms
  per correct letter (.24 waiting, then.392/.544/.696/.848/1); errors reset the word
  and alpha in120ms. No breathing, rotation or looping motion.
  Batched/case-insensitive input shares ProcessInput and never rewards combat stats.
- Full READY locks letters immediately. The fully painted circle anchors a180ms
  hold and one existing-clip low whoosh through SfxPlayer/preferences, then one
  Continue. Duel-only charcoal fade200ms leads to existing smoke200-600ms and
  intact enemy/shadow344ms; seal fades out by600ms. The first word still receives
  at least300ms of actually visible preview before combat.
- Cached spawn foot/height outlive the dead enemy. The private textured quad lies15mm
  above the floor with a64x16native-world-pixel quad footprint; the camera and sprites remain.
  Pause/focus freeze clocks/progress. Menu/Restart cancel callback, cue and quad/material,
  restore environment gain independently of Slayer. Tutorial/terminal victory remain.
- Asset/prompt follow-up:6/6 scoped cases pass, final64px geometry case rechecked;
  strict QA/repeat build and184native checks pass. All477frames/475transitions
  of normal/Captain scopes directly inspected. Historical procedural-seal receipts remain separate
  in [the existing checkpoint](../plans/duel-intro.md).

## Duel opening (2026-10-26)

- Play lifts the menu and covers the transition250ms. After gameplay initialization,
  arena/player reveal0-450ms, settle to850ms, smoke850-1350ms. The whole opponent
  pose and contact shadow appear together at1030ms, inside the smoke peak.
  Stationary name30/DUEL18 enters1100-1200ms, holds until1720ms and fades to1880ms.
  Status HUD1820-1980ms, word tray1900-2100ms, readable preview2100-2400ms.
- Typed READY/retry:200ms cover plus scene load (READY uses a charcoal alpha fade; retry retains the existing wipe), reveal0-200ms, smoke200-600ms,
  intact opponent/shadow at344ms. Title enters420-500ms, holds until680ms,
  fades to820ms. Status HUD540-640ms, word tray620-700ms, preview700-1000ms;
  total1200ms plus loading is retained. Enemy HUD name stays hidden while the
  introduction label is visible, then fades80ms independently from HP/guard/score.
- Private smoke uses18fixed stepped elliptical mesh puffs (6core/12edge), existing
  StrikeAccent material and collider-relative scale: about1.1H wide/up to.55H tall.
  Low anticipationu0-.12, rise.12-.28, peak.28-.48 (100ms full/80ms short), intact
  revealu.36, then low sideways dissipation. No feet-up shader clip or label motion.
  Camera stays at the authored gameplay framing; cover/menu retain eased upward
  motion. First word stays stationary/readable300ms with existing next-word ink,
  returning to active ink100ms at handoff. Accepted letters supersede that cue.
  Reading time starts after LateUpdate first applies full tray alpha, independently
  from the timeline. A frame that reveals the word contributes no preceding time;
  later stalls while the word remains visible count. Combat waits for both timeline
  end and at least300ms of visible, unfrozen preview. A late reveal extends only the
  final preview; smooth totals retain the authored rhythm within frame timing.
  No countdown or first-key timer gate is added.
- Gameplay locks before gate activation. Words/configuration initialize normally;
  HP, typing timer, score, letters and combat time cannot advance in the intro.
  At handoff the first attack is scheduled500ms after combat opens; typing/parry
  discard intro input, block the handoff frame and require held keys to be released.
- Pause/focus loss hold intro, particles and UI alpha, including the final handoff
  frame; Resume continues from the same milestone. Accumulated preview reading
  time is preserved; the clock reanchors on Resume and excludes the paused interval.
  Menu/restart keep the paused heading suppressed through navigation/cover and
  restore renderer/HUD alpha while destroying the private mesh/effect. Tutorial
  uses only the arena reveal and retains its own reading lock/confirmation.
- Existing dust rendering and heading typography are reused. Strike dust prefab,
  character sprites/clips, camera/world ownership, difficulty, providers and saved
  contracts are unchanged. No VS/READY/FIGHT intro overlay, skip, extra confirmation or new intro audio. The READY completion cue is described above.
- Shared Audio_Services preserves its music component when menu SFX already exists.
  Same-cue music keeps its live voice/playhead across Continue/retry. Music stopped
  by the existing Menu action can start on the next Play. READY completion uses a separate SfxEvent over an existing whoosh clip; no new clip or waiting loop is added.
- Actual300ms guard follow-up: Unity10/10intro tests PASS,0fail/skip; exact raw
  snapshot90a0a643 builds QA with strict repeat/font guards intact. Native1280x720
  real input125checks PASS,0exceptions/provider IO; all593frames/583transitions in
  ten affected preview/handoff/cancellation scopes directly reviewed. Includes
  pre-alpha timeline overshoots, already-visible stalls, paused reading credit,
  final-frame Pause and Menu/Restart during the extra wait. Runtime timestamps
  establish the300ms minimum; encoded frame counts are visual evidence only.
- Earlier smoke/name follow-up: final Ubuntu Unity7/7DuelIntroTests PASS,0failed/skipped;
  exact284C#/shader/package/version inputs match the worktree. Includes actual scene
  locks, whole-pose/shadow reveal, property-block preservation, identity exclusivity,
  pause/focus at smoke/reveal/name/final frame, cancellation and tutorial. Final
  WebGL/repeat builder PASS; player10a1876ff58157a5 native83checks PASS with zero
  exceptions/provider IO at1280x720. All566video frames/561pairs in five complete
  scopes reviewed; navigation-heading resurrection was rejected and corrected.
  [Checkpoint](../plans/duel-intro.md).

## Reusable world lighting (2026-10-22)

- Wall torches use authored cinematic SinePair flicker: base8/range15/amplitude.18,
  independent phases and matching flame emission. Two fixed TorchBeam cards are
  inactive and no longer receive atmosphere updates. Central lantern now uses intensity16; flame
  atlas/timing and actor self-shadow remain unchanged. A constant shadowless cool
  Spot fill and palette-masked damp Floor1..8 add localized floor response without
  re-enabling the rejected rune, haze, beams or planar reflection. Pause holds the clock;
  Slayer composite conceals the continuing world phase and restores it without
  resetting. [Evidence](../plans/combat-hanging-light.md).


- DungeonWorld selects the authored ActorDepth library for normal-player self-shadow.
  DungeonSprite saves strength0.65/depth scale0.15/bias0.01; world without a library
  retains the previous rendering path. The user accepted demo8293 with rune and
  blue haze disabled. QA F8 restores each material's configured value; normal
  builds exclude the control. Integration evidence/status is in the
  [lighting checkpoint](../plans/combat-hanging-light.md).

## Pause and guidance cleanup (2026-10-24)

- Manual Pause shows PAUSE plus Resume/Restart/Settings/Top 1000/Main Menu without
  an enclosing panel face or rim. Game Over/Victory use a centered600x366 card,
  title30, score40, secondary text20 and stat values24. Results group Defeated/Time
  and Accuracy/Parries in two aligned label/value columns. The short difficulty/
  outcome line excludes internal attack-state names; NEW BEST replaces the historical
  best line when a record is set. Restart is wider than Main Menu (304/232); smaller
  Top1000/Settings controls sit below. MatchResult remains hidden for diagnostics.
  Terminal results hide the surrounding combat HUD, typing tray and Pause icon
  through thirteen CanvasGroups; world/dimmer and result controls remain visible.
  HUD stays hidden through result modals and camera exit. Restart/Menu supply a
  fresh scene; ordinary Pause does not alter HUD visibility or disable its logic.
  Result audio uses the existing Settings modal; embedded sliders/duplicate key hints
  are hidden. Manual Pause hides/restores the result-only group; Resume restores
  these authored result bounds. Modal freeze rules remain. Scoped verification is
  recorded in the compact-menus checkpoint.
- The single current tutorial is the only how-to screen. Main Menu has Play,
  Tutorial, Top 1000 and Settings; the legacy How to Play modal is removed.
- Fresh encounters now use the session-owned opening described below; Continue,
  retry and tutorial completion use its short variant. There is no Fight/Enter gate.
- Live combat has no persistent DUEL x/6 label/backplate. Nonterminal deaths use the typed READY ritual below; CLEARED/HP/NEXT and the Continue button are removed.
  Combat cues, typing feedback, finite six-opponent progression and scoring remain.
- Scoped Unity 12/12 and repeat builder pass; native WebGL acceptance is recorded
  in [the menu checkpoint](../plans/compact-menus.md).


- Scene selection, world presentation, individual lights, flame sequences and combat
  lighting use authored profiles (defaults under `Assets/Data/Lighting`; selected
  assets may live elsewhere). Native materials,
  post, bake and URP assets remain their authorities. Builders apply these values;
  they do not regenerate the approved dungeon style. See [authoring guide](lighting-profiles.md).
- World loading accepts a `WorldDefinition`; camera/arena/brain/environment bindings
  use semantic roles. Fixture names/counts and atlas layouts are independent of the
  default room. Profile changes require Editor apply/reload; this adds no map UI or
  hardware quality tiers.
- The approved ten-frame flame, palette, timing, fixed anchors
  and pause/Slayer phase are preserved. The REPLACED-inspired pass changes the
  MainLantern key24->16 and adds a separate constant cool floor fill64; other torch
  profiles retain their authored strengths. Each instance owns its clock and UV mesh.
  Camera impacts now read `WorldDefinition.cameraImpact` (`CombatCameraProfile`); defaults
  remain unchanged. Presets and curves prepare at binding; accepted requests retain that snapshot.
  Pixel recoil uses the active world's target height; zoom composes with its lens.
  Blur center/half-width/depth edge and pinned cloth are world-profile values;
  haze/rune shapes and drift/scan are authored on their existing materials.

## Combat camera impact (2026-10-23)

- `WorldDefinition.cameraImpact` supplies six authored `CombatCameraProfile` presets before gameplay activation. Dungeon defaults retain normal2%/140ms, parry3%/180ms plus1logical pixel/100ms, received (including fatal)1pixel/100ms without zoom, lethal5%/250ms or captured Slayer lethal2% plus2pixels/100ms. Slayer entry preserves its base lens tween and the migrated200ms Rumble/camera-space gain with30ms delay. Rapid Slayer hits add no transient pulse. See [authoring guide](camera-impact-profiles.md).
- `CameraImpactTimeline` owns each instance's prepared curves, single prioritized envelope and displacement. Requests capture double timestamps; zoom excludes manual Pause and shake uses gameplay time plus the configured HitStopper gate. A skipped attack/hold renders its peak before running release. Only the bound world camera can acknowledge that peak, with a matching serial; later hitches never replay an acknowledged peak. Equal/weaker requests never extend ownership. Runtime has no combat default table or missing-profile preset selection.
- `CameraImpactExtension` applies the sample directly at Cinemachine Noise. Logical pixels use this frame's CameraState lens/position and actual WorldTexture height; no180px/window-size assumption. Combat emits no old impulses; the dedicated channel2 listener is gone and legacy channel1 remains available. Final half-height is base/(1+impactZoom), with the existing analytic foot anchor and fixed HUD/typing. Profile edits apply on bind/reload and survive repeated builders.
- Existing hitbox/parry/DamageAccepted consumers and captured Slayer classification remain. Pause holds pending hitstop/render acknowledgment through exact freeze transitions. Reading/results/Restart/Menu/unbind/disable clear pending and active output, including destroyed-root teardown; existing Slayer exits still complete through results. Public gameplay/provider/storage contracts, damage/typing/scoring/audio/animation are unchanged.
- Integrated focused Unity58/58 and final WebGL110/110 pass at1280x720/640x360, including the real final-key180ms stall, lifecycle and two-world binding. Same-player compositor follow-up16/16 and direct review of512window-frames/502pairs complete the camera evidence; see the [camera checkpoint](../plans/camera-impact.md).

# Current gameplay

## Original parry and dedicated follow-up (2026-10-23)

Parry enters original combo_3_end_2; unsuccessful recovery stays2->3->4->5->idle
at400ms. Success uses combo_3_1 -> parry_lift -> combo_3_0 -> combo_2_end_0..3
-> idle/follow-up at950ms; both success clips share this seven-cel sequence;
defense stays250ms and success holds its first cel800ms. One completed word in
[0,800) unpaused real ms queues one dedicated `parry_followup` after recovery.
No word means no attack. Pause retains queue/budget; hit/death/Slayer/interruption/
disable clear it. Ordinary words after idle still use combo_1; Slayer is unchanged.

The nine-cel combo_2_1..5 + combo_2_end_0..3 sequence omits combo_2_0 at the
user's request, starts0 with fixed20ms crossfade, ends566.667ms, opens at109.677ms
on combo_2_3, updates contact164.516ms and closes219.355ms. A repeated end_3 key550ms preserves its final73.118ms hold. Existing10damage,
stamina, deduplication, hitstop, contact/lighting/flash/camera/audio consumers remain.
Follow-up end/exit cancels only its own opening cel; a newer normal/Slayer window
and its delayed close survive. Replacement clears the old bypass before opening
the new window. Disable closes unconditionally, once.

Pose5-removal follow-up: pinned Unity37/37, strict repeat/WebGL and native1280x720
21/21focused real-input checks PASS. Previous cancellation/consumer receipts remain
attributed to their builds. Direct-handoff correction: success transitions directly
to follow-up at950ms when queued, without an intermediate idle0 frame; no queue
returns to idle. Focused Unity42/42 and native22/22 pass on the corrected build.
[Frame evidence, scope and demo resources](../plans/parry-main-demo.md).

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
  and results show the level. Ready and Restart retain the run level; only menu
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
  Sword/shield thrust now holds pose0 for800ms for the per-strike1s warning;
  the legacy cue/audio still begins in the final300ms.
  Each form owns Idle/Walk/Attack1/Attack2/Hurt/Die; stun/Slayer reuse that form's Hurt.
  The original skeleton controller/GUIDs remain for the tutorial configuration.
- After each of the first five death animations, the existing current-word slot
  shows READY alone (next/third words blank, countdown hidden). The CLEARED/HP/NEXT
  result card and Ready button are removed. Type the whole word, case insensitive;
  Enter cannot continue. Errors reset READY with existing typing feedback, and
  partial progress survives Pause/Resume. Waiting is indefinite; Ready letters do
  not change combat HP/score/time/accuracy or trigger attacks, combo or Slayer.
  The existing Continue callback loads the next opponent once, discarding trailing
  input, followed by the short intro with at least300ms of visible word preview
  and held-input release protection; there is no second confirmation.
  Carry only HP/score/combat time/letter counts/parries/progress;
  gameplay reload resets combo, queues, stamina, cue, stun, Slayer and effect bindings; world ownership is retained.
  Boss victory ends the run without healing or a seventh opponent. After the death
  animation, VICTORY / BOSS DEFEATED shows the final statistics with Restart, Menu
  and leaderboard; READY/Enter cannot resume it. Intro/HUD show DUEL x/6.
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
- Every Classic/Endless strike, including each combo hit, warns at least1s before
  contact at authored speed by extending the existing preparation cel as needed.
  The32x16logical key sits above the enemy's stable collider head, with an8px gap
  and pixel-snapped projection through the rendered camera/viewport. It first shows
  a white outline and white SPACE text;
  at200ms before contact it fills ivory to signal the press timing. Teal means an
  active parry window, gray/X means locked. Pause holds the displayed state.
  **The outline is advance warning, not buffered input**: early Space retains the
  existing manual pose/window and may expire before impact. Each strike needs a
  separately timed press. Legacy cue/audio stays in the final300ms, once per strike;
  Esc/F8/Esc compares the cue visuals for this session only.
  [Verification and visual scope](../plans/parry-cue-demo.md).
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
  held Enter is discarded during the intro and requires release at handoff. The selected-difficulty
  encounter intro starts with40HP,180enemy HP,100guard and zero score/statistics.
- LeaderboardSubmitter still rejects tutorial submissions before resolving a
  provider. Public gameplay/provider APIs, Firebase schema and saved key stay unchanged.
- EndlessOverlay clones only the original encounter intro content: all guide copy
  and the guide panel background remain hidden in the clone. Intro width/action
  typography retain their original bounds; live combat has no reading panel.
- Verification and scoped scene/asset evidence: [tutorial guide checkpoint](../plans/tutorial-hud-guide.md).

## World lighting / parry clash

### Reusable MetalClash preset (2026-10-23)

- `ImpactVfxEmitter.Play(ImpactVfxProfile, ImpactVfxContext)` emits without a player
  or ParrySystem. Context captures point, left/right direction, floorY, seed,
  optional recipient ellipse and optional world scale (profile default1.25).
- `Assets/Art/VFX/ParryClash/MetalClash.asset` references the approved existing
  ParryPhysics/prefab. Profile owns pixel/world scale, depth scaling, cooling colors,
  final fade, arbitrary flash layers (renderer ID/count/lifetime/tint/pixel size/
  world cap/grid/start frame/frame curve/color-alpha gradient), and local light
  enabled/color/intensity/hold/duration/decay/range/offset. MetalClash preserves
  24particles,1px,rest<=400ms,lifetime<=1.1s and Point5/70ms hold/280ms expiry.
- Production Player uses the dedicated `ParryImpact.asset` and prefab/physics:
  one seeded38/39/40logical-pixel Pimen core with +/-3degree rotation,
  capped2.5world units, fixed48x48 canvas/pivot24,24,
  six visible cells at80ms each and a seventh transparent cell; expiry480ms.
  Frame curve0->6/7 over480ms preserves the full six-cell timing. Core layers are
  only Hit Effect+Color, with white tint/material gain1 and constant alpha; original
  white/yellow/orange RGBA retained. Detached ray/fragment/speck PNGs are exact
  original Pimen layer crops, Point/Clamp/no mipmaps; recipe and hashes are in
  `PimenSource.json` and `tools/game_loop/export_parry_pimen.py`.
  Five bindings: core0,fragment1,near rays2,body specks3,far rays4; no shock arc.
  There are22unchanged physical trajectories plus one core,23total/184at eight
  leases. Leads10-14px/initial2px live550-700ms; secondary4-7px live850-1100ms,
  with unchanged cooling, final80ms fade, gravity/drag/bounce/floor rest and seed.
  NearZ velocities-6..-4 and far2..4 remain actual world-space perspective/depth;
  actor cutout writes depth and Pimen uses the same depth buffer with LEqual.
  Detached contact/recipient snapshots, local light, bloom, audio, hitstop and camera
  remain unchanged. Mirror is captured on core0 at emission; pool/pause clocks own
  the complete tail. [Pimen/depth evidence](../plans/parry-pimen-depth.md).
  Windows Unity2022.3.25f1 verification:44/44focused tests, strict repeated builder
  in both QA builds, and three consecutive real-input parries per orientation PASS.
  Complete source/frame/transition and encoded-clip review is recorded in that checkpoint.
  Builder creates only a missing stock prefab and fills null/stock profile defaults;
  repeated runs preserve authored prefab/custom Inspector references. Standalone
  MetalClash remains unchanged. [Earlier evidence](../plans/parry-impact-burst.md).
- Prefab renderer bindings supply unique IDs, particle systems and materials;
  generic rendering reserves no contact slots and supports zero or multiple flashes.
  Camera/world configuration supplies the native raster width. PPU supplies the
  floor clearance/offset and particle geometry; Compact shader UV masks remain code.
- `ImpactVfxBurst`/`ImpactVfxPixel` own generic rendering. `ParryClashBurst` retains
  the original serialized script GUID, historical Current/A/B/C fields and parry
  metadata. Production parry calls the same public emitter API and uses
  profile.prefab; legacy presets/PixelSpark retain their compatibility pool.
- Emitter defaults to eight active leases across prefab pools, reclaiming the
  oldest at capacity. Physics exceeding40 states or missing/overlapping/duplicate
  bindings fail before leasing; renderer capacity is also validated. Builders fill
  absent references; versioned compatibility migration preserves authored settings.
- Physical states, presentation values and curve keys are copied at emission.
  Simultaneous profiles and later asset edits cannot change a living burst/pulse.
  Local light bypasses gameplay event selection; the scene rig still owns actual
  light templates and overlap budget. A burst retains its lease until particles,
  flashes and registered local pulses finish; missing/disabled rigs or rejected/
  evicted pulses add no wait. Explicit cleanup and capacity eviction cancel immediately.
  Legacy 2D rig clear restores its captured base color before the next enable.
  Each rig owns a double-precision realtime clock excluding pause/reading; pulses
  capture their start timestamp, so a receiving frame's pre-emission time is not
  charged to a new pulse. Post-emission stalls still count toward profile duration.
  Freeze events preserve active fractions; directly observed timeScale0 transitions
  conservatively rebase without catch-up. Positive-scale hitstop remains unscaled.
  One-floor/one-bounce solver, fixed buffers,
  unscaled pause-held updates and cleanup remain. Audio, damage, recoil and hitstop
  remain owned by gameplay.
- Authoring/API: [lighting and impact profiles](lighting-profiles.md).
  Scoped evidence/status: [preset checkpoint](../plans/metal-clash-preset.md).

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

### Slayer contact-first presentation (sweep timeline 2026-10-26)

- Retain original Met.Pxl B PNG/GUIDs/center pivot and32/40reference-pixel scale.
  Game sequence F4/F5/F0/F1/F2/F3 uses30/40/60/45/35/30ms for hits (240ms),
  30/40/80/55/45/30ms for finishers (280ms). F4 appears immediately; peak starts70ms.
  Before retiring old output, a primary with the same recipient/anchor/lifetime
  and age<70ms selects F0 instead (170/210ms). Exactly70ms selects the full sweep.
  Age begins at0 for either start; residuals/other targets never select the mode.
  No queue, rewind, transform animation or input/damage delay is added.
  Both Slayer B roles emit pure black#000000 independently of combo tint;
  source alpha, residual fading, sword trail and afterimages remain.
- Body contrast follows the same primary decision before pool retirement. Isolated
  hits hold ivory130ms (finisher150ms), then fade20ms. A chained F0 hit fades from
  contact over20ms; a chained F4 hit adds another20ms flash at70ms. New contact
  replaces the entire old schedule, including a pending peak. Pause holds age and
  weight; lifetime change, disable and presentation/result cleanup cancel it.
  Legacy/no-active-emitter contacts retain the existing40ms pulse. No separate
  cadence estimator or public/serialized tuning is introduced. Scoped Unity
  coverage is recorded in verification; final visual acceptance remains pending.
- Slayer S1 uses combo_1_0/1/2 then combo_1_end_0..3; S2 uses combo_2_3/4/5
  then combo_2_end_0..3; S3 uses the final strike combo_3_11 then combo_3_end_0..5.
  Each has seven40ms poses, ends280ms and holds its recovered final pose; the
  duplicate key270ms preserves the last hold at100samples/second. The current
  Player prefab's AttackSpeedMul is1. Each new correct key interrupts recovery
  immediately. Apply the contact pose before the existing
  deduplicated damage window; collision/damage ordering remains unchanged.
  Local impact axes(1,-1),(1,0),(1,1) follow the selected swing and source facing;
  no independent eight-angle random selection. Capture the actual damage recipient's
  EnemyHitAnchor point/lifetime and direction/tint before damage. B follows its
  authored chest after Animator poses, with fixed angle/world size and black RGB.
  A child renderer compensates each cel's baked alpha centroid; no collider-center
  placement. Hidden/destroyed recipients leave the last point; new lifetimes reclaim
  old slashes. Four pose profiles cover ske and the three Endless forms, including
  held parry cels assigned outside Animator; six roles reuse those profiles.
- Each recipient has one primary plus one<=35%alpha residual lasting<=30ms;
  finisher replaces both. Original luminous sword geometry expires120ms per swing
  even when the final cel is held; another typed strike resets that envelope.
- Skeleton positional jitter is disabled; existing hurt pose progression remains.
  Compositor-owned ivory#FFF3CF pulse holds130ms normal/150ms finisher then fades20ms
  for isolated hits. A valid chained primary selects contact fade20ms for F0, or
  contact20ms plus peak70-90ms for F4. Each contact replaces the entire old schedule.
  Sprite alpha/depth, baseline RGB and unrelated property-block values remain.
  Pause/reading hold presentation clocks; results/disable/restart/unload clear them.
- Public input/scoring/damage/stamina/camera/audio and Slayer no-hitstop policy remain.
  Normal/parry/legacy routes retain their shared budget and legacy40ms pulse.
  Clean81selected tests, QA/repeat builder, native/export and six technical playback
  checks passed; latest-main integration passed22intro/Slayer-transition cases.
  The user accepted visuals; main83d6e0a6 was verified. External raw evidence was
  deleted at the user's request; [completion record](../plans/slayer-slash-vfx.md).

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
  pulses shade actors and floor. Slayer now blends the active room to red before
  actors/VFX instead of disabling its sources/rig; standalone sparks remain owned
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

- SlayerScreenFX coordinates450ms entry/650ms exit with independent presentation
  progress. Entry zoom uses EaseOutCubic; exit zoom and color/bars use SmoothStep.
  DungeonWorldRender blends red before actors/VFX; the room stays active, with
  weather/atmosphere/Volume and HUD/tray background opacity fading together.
  Silhouette tint includes per-renderer lighting suppression without tinting
  excluded sparks/afterimages. It frames both bodies with full-width black
  bars. Fixed standing-body landmarks use collision bounds inset .28 world units
  above and .12 below; padding is8 reference pixels at960x540. Animated weapons,
  sprite poses and VFX do not change those landmarks. CombatHUDLayout owns their
  projection for HUD and filmframe; typing/status sit on the lower bar and tutorial
  highlights follow their targets. Authored red/tint/framing endpoints remain;
  the HDR path differs from the legacy screenshot by one8-bit red level.
  Duplicate requests do not restart; reversal begins at displayed values.
  Unscaled presentation excludes Pause; results allow exit to finish.
  Disable/unload/Restart/Menu cancel old progress and restore baseline immediately.
- Normal hit-flash during the650ms exit is composed with the remaining silhouette.
  The group owns final RGB while presentation weight>0; hit-flash caches untinted
  baseline, keeps its remaining duration at the endpoint, and preserves live alpha.
  Gameplay Slayer still suppresses normal hit-flash. Scoped14 Unity cases/native23
  plus75frame/73pair review cover this follow-up in the coordinated checkpoint.
- ScoreCluster stays at screen scale1 in both modes; its camera scale binder is disabled.
  ScoreUI_Flash retains the120ms award-only text pulse. ScoreRules still owns white/
  x2 yellow/x3 red, original points/rounding and x3 cap. Combo sits below the total;
  floating awards use the latest tier tint and the enemy-head anchor.
- The SCORE caption and former corner labels stay hidden in both modes.
  Exit/disable restores saved background opacity and Volume/weather properties.
  HUD/typing anchors and score canvas scale1 stay fixed throughout; only the
  existing floating-award anchor follows the enemy head.
  Final builder migrates existing scenes and protects black bars from theme tint.
- Quick46/46 and browser acceptance cover these paths; four render sizes and x3
  exit frames were inspected. See verification and the slayer-restore checkpoint.
  No gameplay timing, damage, scoring formula, public API, GUID or package changes.

- Coordinated transition checks(2026-10-23): scoped Unity8/8, strict builder and
  native32/32 at1280x720/640x360 pass. Integrated review covers488/488frames and480/480adjacent
  transitions across full entry/exit, Pause/Resume with180ms hitch and lethal
  exit with500ms hitch. Input/stun/Slayer/damage/scoring code is unchanged.
  [Checkpoint](../plans/slayer-soft-transitions.md) records sources and limits.

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
  Pause freezes/resumes phase; Slayer now keeps hidden room clocks active behind
  its blended background. Destruction releases only the private mesh.
  Original fire mesh/material/GUIDs stay intact. Private fire material;
  iron/wood emission0. Three lantern materials share LanternLit (URP14 PBR);
  glass black/alpha.12/emission(1,.38,.08),
  no specular, ZWrite off, no cast/receive shadow. Environment child follows
  Slayer background blend, excluded from flame flicker. Within the task dungeon baseline,
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


## Space attack warning

`ParryCuePresenter` reads `SkeletonController.TryGetAttackTelegraph` and the existing
`CanBeginParry`/`IsWindowActive`; it never changes combat or typing input. Original
Kenney CC0 caps form a 32x16 logical key above the enemy collider head, with an8px
gap and pixel-snapped rendered-camera projection. The tray is fixed at (61,126),
198x30, near the fighters. White outline/text means advance warning for each strike;
ivory fill begins 200ms before authored contact; teal means an active parry window;
gray/X hides the label while locked. No valid attack means hidden.
Prediction caches Anim_DealDamage events by controller and follows Animator
speed/progress plus the calculated preparation hold to provide1s at authored speed;
outgoing transitions cancel the cue. Early Space is not buffered; the existing
manual250ms window can expire before contact. Each strike needs its own press.
The original cue object/SFX stay intact; only its Graphics are suppressed.
Previous actor Graphics are restored and released on target rebind/disable.
Esc -> F8 -> Esc compares key/legacy ! in-session, default key on application start;
no PlayerPrefs/settings/tutorial state added. Pause holds presentation;
terminal/Slayer/encounter state clears it. The user accepted this default after
the isolated demo and requested pre-main review. Acceptance and captures:
[checkpoint](../plans/parry-cue-demo.md).
