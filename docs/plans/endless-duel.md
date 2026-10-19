# First-boss completion (2026-10-19; current scope)

Approved: five levels, four normal duels -> elite -> boss. Boss victory ends the
run after its death animation, with VICTORY/BOSS DEFEATED and final statistics.
Restart/Menu/leaderboard only; no healing, Continue or seventh duel. Restart keeps
difficulty and resets run data. Tutorial/Classic and existing UI/input edits stay.
Internal EndlessRun.RepeatAfterBoss=false retains the dormant loop and heal code.
Existing Endless endpoints, best/pending keys, payload/order/retry remain; old
multi-round records remain valid and may outrank six-defeat runs. No Firebase
production changes, publication or fixes to the two deferred review issues.

Implementation: completed-run guard in data/director, per-encounter defeat guard,
per-run result-save guard, terminal victory detection in CombatSession, existing
locked result overlay after death cleanup, DUEL x/6 copy and kills/score rows.
Offline regression covers repeated completion/death/Continue, six wins without
healing and five-difficulty restart/best/pending. Browser driver now targets one
boss and repeated Enter, Victory/table at both sizes, Restart/early death/Classic.

Verification in progress: shared runner initially busy; queued Quick and a fresh
QA snapshot. Previous QA receipts below do not certify this change. Current task
temporary artifacts: E:/CodexTemp/TypingHero/first-boss; retain only until written
verification/cleanup. No branch/worktree/commit/push created.

---

# Endless Duel - approved completion plan (2026-10-18)

## Five difficulty levels — verified QA candidate (2026-10-19)

Scope: Endless only; menu/name → difficulty → Fight → typing/parry/Slayer →
Continue → boss/rounds → death/table/Restart. Preserve dirty HUD/audio, GUIDs,
roster/HP/damage/words/scoring, tutorial and Classic. QA only; this task has not
built production, published to itch, written production scores or deployed Firebase.
Other concurrent tasks have their own release receipts; do not attribute them here.

### Implementation and parameter basis

| Level / stable ID | Enemy recovery divisor | Combo (asset base 1.5s) |
|---|---:|---:|
| Very Easy / very_easy | .60 | 2.500s |
| Easy / easy | .80 | 1.875s |
| Normal / normal | 1.00 | 1.500s |
| Hard / hard | 1.25 | 1.200s |
| Very Hard / very_hard | 1.50 | 1.000s |

- Existing role/elite/round recovery is clamped first, then divided by pace.
  Normal preserves existing timing. No animation/contact/cue/chain/parry/timeScale,
  roster/HP/damage/words/scoring changes. Combo renewal, expiry and HUD use one
  effective value; the shared ScoreRules asset is never mutated.
- Shogun selector under name: mouse/Tab/ShiftTab/left/right/Enter, persisted ID,
  default Normal. Snapshot before combat; Continue retains HP/statistics/level;
  Restart resets progress and retains level. Menu changes the next run only.
  Intro/run/results display it. Tutorial ignores the multiplier.
- Local best/pending partition by ID. Normal keeps exact legacy keys and
  leaderboards/endless_v1/{uid}; others use endless_v2/{difficulty}/{uid}.
  Existing payload/sort/ETag/three-conflict/read-abort/tie-boundary behavior retained.
  Pending retry uses saved partitions. Modal filter is independent and rejects old
  responses/status after filter/close. Opponents/achievements remain cumulative.
  docs/game/endless-firebase-index.json is an index-only deployment fragment;
  no permissions changed or deployment performed.
- Generated scenes imported from QA030621 after serialized comparison: menu287→333
  documents, combat685→706; zero old IDs removed, all45/115 old GUID references
  preserved. Only selector/binder/layout documents changed beyond whitespace;
  existing HUD/audio settings retained. Deterministic repeat-builder passes.

### Verification receipts

- Quick033637:93/93PASS, input
  9455fb437249988ae96aac8dfb0ea273f4143b2c184a1f1e57b584972baa768f.
  Quick063104:98/98PASS, input
  ef38bc0d9f9fc01626cf299a1a4d78132dbfe3306921399725f17f0ac53925a8.
  Includes config/persistence/run/recovery-cap/combo/pause/tutorial, all five local
  and provider partitions, legacy Normal, ETag/conflict/read-error/pending and
  >1000-row boundary ties. Four concurrent guest-identity tests also pass.
- QA033808 build/repeat-builder PASS, immutable input
  87eadc5b14ef5e2221e8cf8fac6a8dc7da650c6b70e6bebda015269d6eb7610d.
  Native Edge155 / ANGLE D3D11 / GTX1650Ti / Playwright1.58.2, real keys/mouse,
  read-only QA bridge, Firebase fixture and external traffic blocking throughout.
  VeryEasy18/Easy6 full scenarios in matrix-final; Hard6/VeryHard18 full scenarios
  in matrix-high (4883checksPASS). Normal18 completed every required flow in
  matrix-words3, but that process failed an invalid fast-duration comparison across
  different random words. It is not claimed as a passing combined profile.
- Same QA033808: pending/reload plus actual six-lesson tutorial129checksPASS;
  real earned Hard score503/reload/switch toVeryEasy/retry original Hard ETag and
  payload/clear; tutorial remains1.5s. UI supplemental40checksPASS: all five mouse/
  keyboard selectors, focus loss/return in reading and combat, menu level change,
  five death→own table→Restart flows,960x540/640x360. No Classic writes/exceptions.
- Current QA054721 build/repeat-builder PASS, input
  b8dd70adae741e7f2d9b670eb3a1eb18d218630aeebaee809d281b10e1bd89ae.
  Adds read-only Animator current/next/speed/time/transition/enabled/hit-state fields.
  Normal5rounds/30wins6326checksPASS; deliberate post-Slayer Heavy hits3rounds/
  18wins4025checksPASS; late Heavy parry3rounds/18wins3755checksPASS.
  These did not reproduce a persistent idle lock. CPU4 slowdown diagnostic died
  at6wins due to missed defense; it is not a passing performance/acceptance run.
- Actual-controller test PlayerHitLockTests exercises7 starting states ×4 incoming
  crossfade offsets ×pause/no-pause, cancels to Slayer's idle transition. Targeted
  test and final98-case Quick pass on existing code. This does not establish a
  cause/fix for the earlier WebGL symptom.
- Harness rereads current words after parry/promotion and acknowledges accepted
  characters. Short native key operations permit Space between letters. Recovery
  compares the same unfinished enemy only; actual sample interval is its tolerance.
  Word-input fast elapsed is observational, not a controlled balance comparison.

### Fixed candidate acceptance receipts (2026-10-19)

- QA071020/input956fec7a879998b7c018f0a0b21a71cc8d4932d0c03cc03ea2c834269decffa1:
  full native five-level matrix **22063checksPASS**,66wins, required rounds3/1/3/1/3,
  plus each level's actual deaths/best retention/Restart/tables/filter races/errors/
  Classic/collections. Five fixture Endless writes, zero Classic writes/exceptions.
- Same player: pending503/reload/original-partition retry + six tutorial lessons
  **129PASS**; all-five pointer/keyboard/focus/death/table/Restart **40PASS**;
  tick/error/reading/pause/mute buffer routing **11PASS**. Both960x540/640x360 layouts
  inspected (menu, intro, Slayer, result and table); no unhandled JS/managed errors.
- Runtime combo clock samples per VeryHard/Normal/VeryEasy/Easy/Hard:
  721/794/834/240/248; recovery deadline samples9/9/8/2/4; median frame13.36–13.38ms.
  Largest decay-minus-clock error <.00003s; actual recovery deadlines within sampled
  frame intervals. Per-win checks also verify every configured role/elite/boss/round
  formula, HP/stat carry and10HP boss heal. No frame-rate/device benchmark claim.
- Optional deliberate-hit driver bookkeeping was tightened after this matrix to
  cancel an unfulfilled observation when its encounter ends, so it cannot withhold
  defense in the next encounter. Main acceptance/input/timing/positive-state-speed
  checks unchanged. Focused latest-driver Normal6wins2476checksPASS, one actual post-Slayer Heavy hit35→27, no exceptions (exec16802).
- Quick081526 from the exact archived QA input **99/99PASS**,0failures/skips
  (exec61419). Same956fec7a input as QA071020 and native matrix, independent of
  checkout drift; same shared runner lock respected. Earlier99PASS receipt below
  is retained as the before/after fix history.

### Post-Slayer idle cause and fix (current)

- QA054721 final matrix: VeryEasy18/Easy6/Normal18/Hard6 complete scenarios PASS;
  VeryHard stopped after15wins at Berserker41HP/player35HP. Whole process failed
  at13156checks; no exception, not final acceptance. Trace shows current SlayerS2,
  next hit, Animator enabled/speed1/timeScale1, but state speedMultiplier0 and
  normalized time frozen at.801514 while gameplay clock advances >1second.
- Actual controller AttackSpeedMul defaults0. Slayer saves it on enter, sets3.5,
  restores0 on exit before idle crossfade; only Slayer attack states use it.
  Changed the controller's neutral default to1 (one serialized value, GUID/clip/
  contact/parry unchanged). No timeout unlock or gameplay-state QA setter.
- Actual Slayer lifecycle regression now asserts its restored outgoing state can
  advance and subsequent incoming hit returns to idle. Negative Quick070730:
  1pass/1fail, expected >0 but actual0. Positive full Quick070913:99/99PASS,
  input037c3766f88e042cd86e319b018ce4ac6dc1d584c16426e3f8c29e84ebe482b5.
  QA071020 build/repeat-builder PASS, immutable input956fec7a879998b7c018f0a0b21a71cc8d4932d0c03cc03ea2c834269decffa1. Full matrix and shared tutorial/pending/UI/audio reruns pass on the fixed player. Earlier passing diagnostic runs did not expose
  this particular frozen transition and do not certify the fix.
- Native typing audio11checksPASS on QA054721: actual20ms tick/102ms error buffer
  starts on accepted/rejected tutorial input; reading/pause/mute emit neither;
  no score writes/exceptions. Measures routing, not isolated loudness/clipping.
- Clock samples on QA054721 Normal: median13.43ms/frame (1206combo/9recovery
  samples) versus CPU4 diagnostic32.77ms/frame (209combo/9recovery before death).
  Observed decay-clock errors <.0001s; recovery deadlines within frame sampling.
  The slowdown run's death is not a full-profile PASS or a device benchmark.

### Type Knight research: runtime / bytecode / inference

- Original Steam app1006220, build18291088, menu1.3.2.0; not established as the2020
  devlog version. EXE SHA256
  82f8b87a1db53859aa2e66cf85ae838db0dc3bf33adf111bf99fa6a689a4985d;
  data.win cd6c412fa1fccd1d529b715920aa890ed16c988b8a2c542083eca0ad9de6f703.
  Both match after transfer. Originals rehashed before cleanup: both unchanged.
- Own container typinghero-typeknight-difficulty, existing tlbb-client-wine:20260925,
  Wine9.0, overridden entrypoint, own research mount/fresh prefix, Xvfb/Openbox.
  No other game, Steam credentials or Windows saves copied. noVNC loopback16080.
  SteamAPI_InitSafe fails but unmodified game runs, receives words, kills, combo,
  pause, wave transitions, chests and narrative input. Not a Wine/Steam blocker.
  Logical16:9, Xvfb1280x720/client1280x700;20px decoration/strip caveat.
  Dummy audio/software rendering cannot establish real-device performance.
- Runtime: two trials at each of five levels, steady first14kills and next-wave
  words plus intentional typing stop. Reading pauses, native corrections and
  random words limit continuous-play/WPM conclusions; this is agent operation,
  not human playtesting. Measured xdotool delay100 text calls averaged about50ms
  per character (~20CPS within words),350ms gaps in the standard matrix.
- Standard steady-trial native delivery (includes corrections, not all accepted):
  VeryEasy18text entries/36chars/19.27intra-word CPS; Easy14/41/19.17;
  Normal16/75/19.53; Hard14/85/19.57; VeryHard14/111/19.60.
  Observed target lengths2 /2–3 /3–5 /5–7 /7–9 for the first-wave skeletons.
  Normal literal entry boutsvoted queued two intended words; that10char entry is
  not a10char enemy word. VeryEasy/Normal corrections explain extra deliveries.
  Actual per-text timestamps sum1.869/2.139/3.840/4.344/5.664s;350ms inter-word
  gaps, reading pauses and transition waits are excluded from intra-word CPS.
- Runtime40px combo bar slopes approximately6/8/10/12.5/15px per second, consistent
  with6.667/5/4/3.2/2.667s windows. First zero after post-input capture anchor:
  6.448/4.953/3.849/3.147/2.552s. These are not precise keystroke-to-expiry times;
  nominal100ms samples use actual timestamps and have anchor/pixel quantization.
- Bytecode only: REA rejects data.win. Official UndertaleModTool CLI0.9.2.0,
  release ZIP SHA256 e7573e45d107be34f81f955c6e4afc3c7c8f2628e5a6f307a871e3825b3dfb40,
  verified against GitHub release metadata; code-only dump, no game writes/patches.
  objGestionnaireJeu Create has pace .6/.8/1/1.25/1.5; scrInitialiserTempsCombo
  base4/pace; objScore Step uses delta seconds, holds with no enemies; kills renew.
  Source also changes target word lengths1/2/4/6/8 and spawn cadence, outside the
  approved TypingHero adaptation. No expansion to those mechanics is proposed.
- Runtime approach samples: tracked the same untargeted first-wave skeleton label
  across four actual timestamps spanning~1.49s (same render geometry):55.45/66.94/
  94.36/117.34/139.81px/s. Subpixel/label quantization and the source elastique pressure term limit exact
  ratio claims; Easy differs from a pure .8×Normal comparison and needs followup.
  Correction:75–125% random velocity belongs to death knockback, not live approach.
  Source live base movement20×pace×width + elastique is bytecode evidence.
- Inference: runtime combo ratios support the planned divisor, while TypingHero
  keeps its own1.5s base. Exact comparable approach/attack/cue measurements remain incomplete;
  do not claim their verification. Shield defense is verified below.
  Followup Hard defense-retry passed nine waves/narrative and later died at13730
  score/33WPM/84.82% accuracy after driver errors. Fresh VeryEasy followup has
  cleared thirteen waves and acquired the shield; potion taped restored the health
  bar earlier. Real red-word oafish defense reflected the ghost chain, score9716→9721,
  HP bar unchanged through the sampled attack. Prior brainwash defense followed by
  native offensive words lowered the shield and the chain dealt damage. Corrected followup driver:
  wait500ms after resume for the game's saved pause text to restore, THEN clear
  Ctrl+Backspace;150ms word gaps only in followup.
  Earlier followup damage/queued text is not evidence about source balance.
  Source uses red-word shield against magic; attacking lowers it (tips/bytecode);
  actual shield block is runtime-verified (defense-veryeasy/133-measure,100ms samples). No state edit or auto word completion.

### Source followup measurements and limits

- Native passive first-wave followup:45s per trial, nominal100ms samples with
  actual capture timestamps. Menu screenshots establish levels; source remembers
  its previous selection. Folder normal was actually Hard, folder hard was another
  VeryEasy trial. Relabelled by menu evidence; normal-verified is the new Normal
  trial. A failed initial capture leaked X display handles; corrected by closing
  XImage/display after each capture. That failed trial has no timing receipt.
- First visible received-hit flash after starting the run, including preparation,
  spawn and approach (preceding-frame/current-frame bounds): VeryEasy22.490–22.573s;
  Easy20.266–20.351; Normal19.057–19.171; Hard18.372–18.458;
  VeryHard17.756–17.855. This is not a parry window or isolated approach duration.
- Visible attack-pose observations: VeryEasy mu begins raised-axe pose around
  20.306s, hit22.573; Easy pit18.162→20.351; Normal chord18.167→20.061 (the other
  lane islam hits earlier); Hard cheers around16.053→18.458. VeryHard's stacked
  lanes raise axes around15.3–15.7s and received hits begin17.8s. High-level overlap
  prevents attributing every hit/cue to one skeleton; no exact attack-period claim.
  Skeleton bytecode applies damage at image_index>=8 and swing sound at4; sprite
  timing cannot be inferred from those indices alone.
- Right-edge visible-entry samples: VeryEasy9.859/12.854s (gap2.995s);
  Easy8.857/10.348/12.494/14.951 (gaps1.491/2.146/2.457);
  Normal8.101/9.066/10.284/11.867/13.573. Detection uses actual screenshot pixels;
  sprite/text clipping adds uncertainty. Hard/VeryHard entries overlap and are
  undercounted by this detector; their exact spawn intervals remain unverified.
  Bytecode base/alive increments in VeryEasy→VeryHard order are2/1/.5/.1/0 and
  1/.5/.333/.165/.0825; these are distinct from runtime measurements.
- Repeat after native input stops: Easy duo label18→20s moves155px/1.998s≈77.57px/s;
  Normal dabs16→18s184px/2.024s≈90.91px/s. The original Easy66.94px/s anomaly is
  not explained conclusively. Player/background movement, elastique pressure and
  label/frame sampling prevent treating screen-space speed as a pure pace ratio.
- Parameter decision: retain the approved five divisors and TypingHero's own1.5s
  combo base. Runtime combo ratios independently support them; source also changes
  words/spawning and has different combat. No evidence warrants adding those
  mechanics or changing TypingHero's animation/contact/parry contract. Precise
  isolated source high-level spawn/attack periods and Easy anomaly cause are still
  unverified; the research is not a complete controlled balance study.

### Handoff and owned resources

- Implementation and fixed-candidate TypingHero QA are complete at input956fec7a
  above: Quick99/99, required66wins/22063checks, network/tutorial129, UI40, audio11.
  Preserve all concurrent checkout changes. Later TypingManager/PixelUIBuild/
  CombatHUDLayout/test rewrites are outside this acceptance; they were not reverted.
- Retained product: Builds/EndlessDifficulty-20261019/ and sibling ZIP (ignored),
  exact source-input archive SHA256
  c4cd772d1c558e054bd5a10434989d5453b07d3626d15e07c6918426a7c93199;
  ZIP SHA256 f20d0cc20a2019967904c06fbf32ab7f9b6ad50fba47b53a1f0eeb5e18d33ce3.
  602 product files plus manifest, ZIP CRC verified; includes player, source,
  native fixture harness, existing Playwright licenses and reproduction commands.
  No screenshot/video/report/XML corpus copied into the product. Keep until the
  user finishes QA review or supersedes this candidate.
- Research played all five levels twice, wave transitions and real shield reflection;
  exact remaining measurement limits are stated above. No human-playtest or real-
  device performance claim. No production build/upload/Firebase deployment by this task.
- Cleanup verified: own container, loopback SSH tunnel, Wine prefix/game copy
  and17 hash-verified owned runner-result folders removed. Local task captures
  and research downloads are removed before final handoff. Steam originals,
  other task results, shared source/cache/Library/toolchains/default bridge and
  prior release artifacts remain. No branch/worktree/agent created.

## Base-mode acceptance and scope (historical)

Complete typing -> parry -> Slayer -> kill -> elite -> boss -> harder round -> death/restart.
Use approved gif-pack sword/axe/sword-shield, both attacks, original 96x64 cels,
100ms/cel, Point/uncompressed alpha. Keep player power, words, score rules, max40HP;
boss heals10. Four normals + elite + boss, one enemy. No production Firebase writes,
publication, spear, upgrades, inventory, shop or campaign. Preserve existing HUD edits.

Roles: Swordsman180HP (Attack1, alternate Attack2 from round2), DoubleStrike200HP
(1->2), Heavy240HP/8damage (1), Berserker200HP/5damage (2->1->2), Guard200HP
(alternating singles, hit stamina4/parry25), Captain550HP. Others keep hit stamina.
Stamina100/150elite/200boss. Elite300HP uses a known role and shorter recovery.
Boss >70% singles/doubles; <=70% adds triples; <35% rotates order by round.
Finish current chain before phase changes. Interrupt chains on stun/Slayer/death.
Round1: sword,double,heavy,guard,elite sword,boss. Round2: berserker,double,guard,
heavy,elite heavy,boss. Round3+: four consecutive roles rotating start each round;
elites rotate five known roles. Recovery1.7s, heavy/berserker2.5s; subtract120ms/round
for at most8steps, floor700ms. Never shorten300ms cue or scale HP/damage.

EndlessRun owns progress/stats; role assets own definitions; SkeletonController
executes patterns; EndlessDuel directs lifecycle. Preserve actor/bindings/GUIDs.
Initial Fight only; subsequent Continue loads and starts next duel once. Carry only
HP,score,letters,correct,parries,combat time,progress. Reset all encounter state.
Death summary includes killer/attack; local versioned encounter/kill records and
achievements (first boss, triple parry, undamaged boss). Reuse Shogun modal/list/theme.
Default leaderboard Endless; Classic remains separate, menu/pause focus retained.

Online contract leaderboards/endless_v1/{uid}: name,defeated,score,updatedAt; use
LB_LocalUID. Order defeated DESC,score DESC,time ASC,uid ASC. Fetch1000 progress
then entire boundary tie group, sort before trim. ETag conditional best write,
read-error abort, maximum3 conflicts. Submit death only, save local first; retain
best pending result on error and retry opening table or completing next run.
Loading/empty/error/retry UI; prepare defeated index without deploying it.

## Milestones and checks

1. Real assets: all three forms, attacks/contact/cue, Slayer/death, original+mirror
playback/feet/grips/VFX, no old-sword flash or cropping.
2. Roster/lifecycle: >=3 rounds and pressure cap, pause/read locks, single Continue,
HP/stats/heal and fresh combat bindings after transitions/restart/menu/tutorial.
3. Profiles/online: local persistence, achievements, >1000 fixtures and boundary
ties, ETag conflicts, network errors/retry, Classic isolation, keyboard/mouse/focus.
4. Build/QA: Unity tests; WebGL960x540/640x360; production excludes QA bridge;
200ms/letter normals15-30s,boss45-75s. Human playtest>=3different typing speeds,
after2attempts recognition/tactics/death/retry evidence. Missing human evidence
must remain explicitly unverified. Review scoped diff; update affected docs/game.

## Active checkpoint

- Existing working tree is dirty with completed representative Endless and concurrent
  HUD/audio work. No reset/checkout or new worktree. Baseline receipts below apply
  only to the old representative implementation, not the expanded roster.
- Re-read runtime/tests/build pipeline and inspected all six attack strips.
  Contact cels (zero-based): sword/shield Attack1=5, Attack2=2; axe Attack1=6,
  Attack2=4. Attack2 sword/shield needs100ms held first pose before100ms/cel clip.
- Implemented role ScriptableObjects and three controllers/18 original sprite strips,
  both attacks, contact events, early-thrust preparation, chain/phase executor,
  one-Continue transition, role/elite records and three achievements.
- Added separate Endless result/provider, boundary tie fetch, ETag best writes,
  persistent best-pending queue and modal tabs/collection using existing theme.
  Prepared index fragment docs/game/endless-firebase-index.json; NOT deployed.
- Expanded Quick tests cover roster/cap, all six actual attack cues/pause/impact,
  pending retry/read failure/conflicts and >1000 boundary ties via loopback HTTP.
- First QA build 20261018T161702-webgl (snapshot
  89dca383eb0091351ffec9e3841a88d7177267a0f5655a1198ad5a3493b5580a)
  compiles and preserves an immutable player. It predates final timing/collection
  fixes and added tests; do not use it as final acceptance evidence.
- The shared runner was busy with another task. Task-local blocking-lock wrapper
  queued safely without changing runner policy, bypassing locks or stopping jobs.
  Own sessions: first WebGL16073, queued Quick31323. Current first native download
  session81104. Scratch E:/CodexTemp/TypingHero/endless-duel retained for QA.
- All18 runtime PNGs byte-identical to design/gif-pack. Imported/generated assets
  pulled from first builder only within new owned asset paths; EndOpponent GUID
  matches its generated role assets. Existing HUD/audio edits remain preserved.
- Remaining: resolve any test/browser failures, final Unity serialization/build,
  native keyboard >=3 rounds and UI/network at960x540/640x360, production QA exclusion,
  update affected docs/game, scoped diff/cleanup. Human >=3-person playtest explicitly
  unverified; requested observations asynchronously, no data received yet.

### UI source/reuse mapping

Approved completion scope explicitly reuses Shogun modal, scroll list, buttons and
presentation while adding Endless/Classic/profiles/achievements. Source/API:
ShogunTheme.Leaderboard + PixelUILeaderboard.Build + PixelUIFactory.Box/Button/Label
-> EndlessCollectionBuild configures those same existing UGUI instances and row
prefab; LeaderboardUIBinder switches their real data sources. LeaderboardModal
retains focus/previous-screen ownership. PanelReveal120ms unscaled and existing
UIButtonFeedback/skin are reused, with no new motion implementation or dependency.
The new progress/score columns and descriptive profiles use the existing panel's
size API at600px, 28px virtual rows and404px viewport, centered scaling at16:9;
the right-side existing Close/Refresh controls and collection selectors stay in
that modal. This supersedes the old Classic-only232px presentation for the requested
new data, not the arena/HUD/tray contract. No new visual reference or theme chosen.
Final runtime keyboard/focus/error/empty/retry and960x540/640x360 review pending.

### Expanded verification checkpoint

- Quick162419:86/87 passed. Hurt clip expected500ms, actual600ms. Cause: redundant
  end key adds one extra sampling interval. Removed duplicate end key and explicitly
  set stop time to100ms * cel count; did not change frames, cue or test expectation.
- Serialize + Quick162627:87/87 PASS, no skipped tests, snapshot
  a92b1031e0405bd16c5b3dc5dc9e0d7f5542959e59af4ddc674be6be5e0cbe81.
  Includes all six actual attacks/cue/pause/single-impact, real scene continuation,
  all online loopback fixtures and retained audio/HUD tests.
- Generated scene comparison: no existing IDs removed,14 existing documents changed
  per scene solely for collection references/layout/header and20 added documents.
  Whitespace-only Unity serialization was normalized before importing. Existing
  audio/HUD configuration remains unchanged. Close/Refresh positions explicitly
  updated after discovering the original conditional creation kept their old x620.
- Initial software browser161702:16checks then keyboard pilot died at Heavy; not PASS.
  Native Edge155 first pilot:28checks, first five wins, death at boss243HP.
  70ms reaction pilot:28checks, first five wins, death at boss80HP. No exceptions,
  Classic writes0; one mocked Endless death write. These are diagnostic failures.
- Source review identified an actual integration issue: parry-riposte ForceInterrupt
  switches enemy to Hurt; old EndAttack treated that as immediate recovery, making
  the next cue arrive while player success pose remained locked. Fix preserves the
  authored swing-end clock through Hurt and cancels its spent contact. Stun/Slayer/
  death still cancel chains. Added actual-controller regression requiring the second
  cue to respect the first clip duration. Current implementation reuses asset attack
  arrays and caches clip durations once per encounter.
- New Quick88 is queued (session74948). QA163005/session62073 and production79792
  predate the riposte fix and cannot be final artifacts. Final fixed QA build-only
  is queued62850; this uses the existing Unity build/repeat-builder and preserves
  immutable player, followed by native browser acceptance separately. Shared locks
  remain honored; no unrelated jobs stopped.
- Browser pilot now uses70ms cue reaction, rechecks actual parry readiness before
  Space, targets3complete rounds, records video and approved-texture samples, and
  checks both modal sizes, collection/Classic/focus/error/empty/retry paths. Probe
  additions are read-only and remain entirely behind the automation define.

### Verification resumed 2026-10-19

- Final Quick 20261018T164341: 88/88 PASS, snapshot
  da5b7eaa907fd9aee4cfa7e3f808f0c8df091dfa8649b8351b188dc2b887eb54.
  Includes actual animation timing, riposte-chain regression, modal same-frame
  shortcut isolation, persistence and loopback online contracts.
- QA 20261018T164534-webgl: compile/repeat-builder PASS on the same snapshot;
  native browser acceptance is separate. Production 20261018T165155-production,
  snapshot4afa1031b50e4a1daaffbf3f932a251ce8100a15250b462d37c40a594127f697:
  normal build exits successfully, ZIP integrity passes, no GameLoopProbe,
  typingHeroQA or TypingHeroQA_Publish markers in its player archive.
- Imported final generated owned art/resources. Compared scenes by serialized
  object ID: only Close/Refresh RectTransform positions changed in each scene;
  no object removed, no additional HUD/audio change. Row prefab is unchanged.
- Native final first attempt stopped after first win: player eventually remained
  idle after Slayer and pilot died against DoubleStrike at77HP. Cause unconfirmed;
  do not count this as PASS. A traced diagnostic cleared11opponents before its
  tool process was interrupted by a new turn; no completed report was produced.
  Resumed identical immutable QA player with tracing, real input and fixtures only.
  Three-round browser acceptance and final UI review remain pending.
- Scoped diff whitespace and browser/endless driver syntax pass. Human playtest
  evidence still absent; it is not established by automated results.

### Handoff checkpoint 2026-10-19

- Native Edge155/ANGLE D3D11, Playwright1.58.2, immutable QA164534:18wins across
  three rounds,95parries,35HP carried through round1, boss heal to40, subsequent
  wins at40HP. Continue/reset/death/restart/best/menu and collection modal flows
  exercised with real keyboard/mouse. Captured all15 form/action samples (Idle,
  Hurt, both attacks, Die). Normal/Slayer feet/status and modal screenshots reviewed
  at960x540/640x360. No JS/managed exceptions, Classic writes0; Endless writes1,
  intercepted by fixture. No real Firebase writes.
- This full driver report has status FAILED after8934checks: its final fast-input
  comparison used stale word snapshots after parry/promotion and sent129wrong
  letters (377total,248correct). This is not a full-suite PASS. Fixed only the
  driver: reread after parry and acknowledge input before choosing the next letter.
  Targeted fast comparison on the SAME player:9checks PASS, all input correct,
  12.725s versus17.726s reference, win then menu abandonment emits no submit.
  Runtime source was unchanged; no unnecessary rebuild/retest of passed contracts.
- >=200ms/letter reference per round: normals17.73/17.26/19.38/15.03s,
  17.07/16.39/16.53/19.22s,15.01/17.42/17.85/16.83s; elites26.54/26.14/25.21s;
  bosses46.63/45.06/44.99s (last within one frame of45s). Automated timing fits
  intended range; human balance/recognition/retry still unverified.
- Earlier post-Slayer idle diagnostic failure was NOT reproduced in the18-win run.
  Traced DoubleStrike samples contain no idle/non-pose state with canParry=false.
  Its original cause is unconfirmed; do not report it fixed. Preserve that failure
  receipt as a known verification limit for further reproduction/human playtest.
- Build prepared at Builds/EndlessDuel-20261019/TypingHero-WebGL.zip (ignored),
  hash6acf4d41e41c52dd137ee9500db0a54cf6d127f51e3b439c5eadaa3dc09b9327.
  manifest.json records limits; verification/ retains compact reports/screenshots.
  Production player contains no QA bridge. No publication or Firebase config change.
- Acceptance remains partial because human playtest and the earlier intermittent
  idle observation are unverified. Implementation and offline/provider checks are
  complete; no claim of full acceptance. HUD/audio changes remain preserved.
- Cleanup: removed owned local scratch E:/CodexTemp/TypingHero/endless-duel after
  preserving compact reports, original idle-failure video, selected UI screenshots
  and Quick XML in Builds/EndlessDuel-20261019/verification/. Removed nine owned
  immutable server result folders after matching snapshot hashes. Shared runner
  source/cache and other tasks' results remain untouched; no owned browser process
  remains. Retained ignored delivery folder is intentional, for handoff and further
  human/intermittent-failure verification; remove when superseded by accepted build.

## Historical representative-loop checkpoint (superseded scope)

# Endless Duel

## Scope and acceptance

Implement the approved survival loop on the existing Unity combat: four normal
duels, elite, boss, repeat with bounded pressure. Fixed player power and word pool;
carry HP, score, accuracy, parries and combat time. Boss heals a fixed 10 HP capped
at maximum. Explicit Continue between opponents; death summarizes survival and a
separate local Endless best ordered by defeated opponents then score. Never submit
Endless totals to the single-match Firebase table. No upgrades or new roster art.

Representative roles use the verified existing heavy-swing animation: swordsman,
double strike, heavy, veteran swordsman, elite and captain. Existing art tint/scale,
role labels and attack cadence distinguish roles; do not invent missing animations.
Normal HP 180–240, elite 300, boss 550 are initial playtest tuning, not measured
duration claims. Cue-to-impact stays at least the existing 300 ms.

## Implementation

- EndlessRun owns cross-scene run checkpoint and bounded encounter definitions.
- CombatSession owns scene statistics; an Endless director checkpoints after enemy
  death and reloads combat only on Continue. Scene recreation resets all serialized
  parry/Slayer/stun/target/UI bindings and retains only explicit run data.
- SkeletonController owns attack chains and heavy windup using existing events.
- Reuse tutorial prompt typography/buttons for encounter intro/intermission and
  current role; retain existing pause/result UI and tutorial behavior.
- Restart/menu clear the checkpoint. Tutorial stays independent.

## Verification and handoff

Implemented: six encounters/round, three representative attack roles plus veteran,
elite and captain; preserved player power/word pool; carried HP/statistics; explicit
Continue; boss heal10 capped at40; readable heavy cue; clean parry/Slayer/stamina
transition; defeat summary and separate local personal best. New state lives in
EndlessRun/EndlessDuel, presentation in EndlessOverlay; original asset GUIDs and
provider schema stay intact. No roster expansion before human playtest.

- Final gameplay Quick74/74 PASS, snapshot6bf307dc0f686dd407084ecfe65470241147c53a64dc29f98ed6c4dd5a545d7b.
- Final typography Quick74/74 PASS, snapshotc65964ff9d500fe713121c04620f7bf1cc659a1c35bb47b7896366a858f7c200.
- QA build/repeat-builder20261018T143344 PASS on0b72b04665673ec594fb6dc0d7323a53a9f3deb9fa5370c5275ad13122cae822.
- Native Edge155/NVIDIA GTX1650Ti/ANGLE D3D11, Playwright1.58.2:41checks PASS,
  217.24s. Real keyboard and mouse, all six wins, round2, HP35 retained then boss
  heals to40; Slayer pause; progressed death6/4412/100.0%/26parries; restart and
  zero-letter defeat preserve best6/4412; return menu. No fixture writes or managed/
  JS exceptions. Result20px text and roles reviewed at960x540/640x360.
- Controlled >=200ms/letter reference: normals19.18/16.98/20.25/16.48s,
  elite26.16s,boss48.32s. Software Chromium153 earlier passed40checks with normals
  21?29s and boss61.83s. Final software attempt missed a60ms action key between
  ~94ms frames; final native driver acknowledges two rendered frames per action.
  No gameplay thresholds relaxed. Driver8b3932374757be7474519303ed18317d12cb9affd02ed3acd7427092a28250fb.
- Sole post-build product edit is literal-only Swordsman hint wording: it no longer
  promises a single swing after round3. Mechanics/layout unchanged; longer hints
  already fit the same validated template. No runtime suite repeated for copy alone.
- Runner contracts8/8 PASS on Linux; Windows fcntl import is unsupported as documented.
  Scoped diff/new-file whitespace and driver syntax checks pass.

Resolved deviations: slow heavy initially exposed a447ms cue, longer than the250ms
parry window. Kept slow windup and delayed cue to300ms; actual scene test measures
240?400ms with frame sampling and exactly8damage. Death cleanup now hides the stale
stamina bar. Fixture cleanup removes only test-created persistent audio objects.
Native asset-GC build hang recovered with a fresh editor; concurrent HUD snapshots
and shared-runner contention were handled without overwriting other work. A task-
local queued client waited on the same exclusive lock, never bypassing it.

Human recognition and desire to retry remain unverified. Automated reference times
fit the intended range, but cannot establish human enjoyment. This is the complete
representative Endless mode, using existing swing art/tint/scale rather than a new
animation roster. No production deployment is part of this task.

Temporary local QA artifacts,11 owned server result folders and the owned server
player copy were removed after recording these results; shared source/cache and
other tasks' results were preserved. No task branch/worktree was created.
Concurrent HUD/design changes are owned by another task and preserved.
