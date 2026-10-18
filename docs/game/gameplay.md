# Current gameplay

Source-backed inventory, 2026-10-16. Quick and browser flow have runtime evidence;
see verification for profile status. These are current behaviors, not new requirements.

- Build scenes: `Assets/Scenes/MainMenu.unity`, then `SampleScene.unity`.
  MainMenuUI auto-focuses name input; Enter or Play validates and loads SampleScene.
  PlayerIdentity persists the name via PlayerPrefs, normalizes allowed characters,
  and clamps to 10 characters (menu validation defaults differ; inspect serialized values).
- TypingManager displays current/next/third words. Letters compare case-insensitively;
  nonletters are ignored. An incorrect letter restores the original word without
  refilling the timer, hurting HP or breaking combo. Incomplete type-ahead resets
  independently; completed next words stay completed.
  Finishing current emits correct-word and advance events, then promotes next.
  Type-ahead consumes next characters during promotion and carries them into current.
  Scene attackTimeLimit is 3 seconds; timeout advances and breaks score combo.
  UI tween time is unscaled, word timer uses scaled delta time; both hold on pause.
  ProcessInput emits OnInputResult for each letter attempt, including retyping.
  Slayer rewards each position only once per word occurrence, carrying that
  progress through promotion. A new occurrence starts with new reward progress.
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

## Combat feedback implementation (2026-10-17)

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
- Quick 35/35 includes all seven real Pimen prefab clips and pool reuse/reset.
  WebGL54/native Windows16 and production passed; direct speaker listening remains unverified. Details in [combat-feedback checkpoint](../plans/combat-feedback.md).
  The2026-10-18 size/parry-sorting continuation passed Quick52/52 and focused
  native WebGL5314checks at960x540/640x360, including actual contact frames,
  Slayer/lethal, pause and restart; see [sorting checkpoint](../plans/player-attack-sorting.md).

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
- ScoreCluster restores camera-relative zoom without competing with the score
  text pulse. Exit returns its base scale independently of the camera exit tween.
  ScoreUI_Flash runs only after actual AddScore, never decay/reset UI refreshes.
  ScoreRules still owns white/x2 yellow/x3 red, original points/rounding and x3 cap.
  Multiplier follows the rendered score width; floating awards use the same tint
  near the cluster. Their pivot now agrees with the canvas anchor coordinates.
- The SCORE caption and former corner labels stay hidden in both modes.
  Exit/disable restores saved environment and tray background. HUD positions
  continuously follow the rendered camera through CombatHUDLayout.
  Final builder migrates existing scenes and protects black bars from theme tint.
- Quick46/46 and browser acceptance cover these paths; four render sizes and x3
  exit frames were inspected. See verification and the slayer-restore checkpoint.
  No gameplay timing, damage, scoring formula, public API, GUID or package changes.

## Combat HUD layout (2026-10-18)

- CombatHUDLayout is the only runtime position owner for player/enemy status,
  score/multiplier/decay, popup anchor and typing tray. ShogunTheme reparents the
  existing UGUI objects in place; HP/stamina sliders and their bindings are reused.
- At960x540 each status is112px wide,12px below its own fixed body foot; name16px
  auto-fits down to8px,HP6px at y18,enemy stamina4px at y26. Both groups are30px
  high. Old long labels/panel are hidden. The same collider landmarks(.28/.12
  world inset) drive Slayer framing; no sword/sprite/VFX bounds are measured.
- Score/multiplier center on their actual preferred text widths including award
  pulse and cluster zoom. In both modes the fixed48px score row starts8px below
  the lowest status; score/multiplier font24,gap8,decay below. Scale/pulse/tint
  retain their existing owners. Popup is12px right,font18,rise12; FloatingScore
  resolves the live anchor after CombatHUDLayout each LateUpdate, preserving fade.
- Tray672x56 is horizontally centered,8px below the fixed score row. Digits,
  multiplier visibility and award pulse cannot move its vertical position. The
  three existing word labels keep their typing order/animations; timer640x6 is at
  y44. Error border and tutorial tray/stamina highlights follow the new geometry.
- Approved r3 geometry/reference and preserved r1/r2:design/layouts/shogun/web/contract.md.
  Four normal/Slayer render sizes and required keyboard paths have been verified;
  receipts/limits are in verification.md and docs/plans/combat-hud.md.
