# Slayer: strong entry, soft exit

## Accepted scope

- Enter450ms: EaseOutCubic zoom, SmoothStep background/silhouette/bars.
- Exit650ms: SmoothStep, coordinated endpoints and unchanged stable framing/colors/zoom.
- Fixed HUD/typing geometry; fade panel/caption opacity only.
- Unscaled presentation clock excludes Pause; results allow exit to finish.
- Reverse from displayed values, ignore duplicate requests; Restart/Menu/unload/disable restore baseline.
- Presentation only: preserve input, stun/Slayer duration, damage, scoring and entry shake. No slow motion.
- One QA WebGL build, real input at1280x720 and640x360. Inspect every frame and consecutive transition in selected complete entry/exit clips.
- Design timings belong to CLACKBLADE, not measurements of a reference game. No external reference research or sprite animation changes.
- Commit/push/integrate main after verification. No itch.io upload.

## Workspace and implementation

Worktree: C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-soft-transitions
Branch: micharel09/slayer-soft-transitions
Base: origin/main1f22565e3124bd57cd44d75238767746b95d2784
Original dirty checkout remains untouched.

SlayerScreenFX owns one timeline with separate zoom/layer weights. DungeonWorldRender blends the background before DungeonActor/VFX. The room stays active; atmosphere/rune/reflection and weather alpha fade. Volume weight fades and restores its authored baseline. No scene clones.

Silhouette groups consume presentation weight independently of boolean gameplay signals; original/dynamic colors and exclusions survive reversal. HUD/caption opacity fades without moving geometry or changing input. Disable cancels progress and restores baseline immediately.

Builder calls one-time migration. Legacy non-default zoomDuration is retained in both directions; default becomes.45/.65. New Inspector timing survives repeated builders.

QA probe remains read-only and automation-only; records presentation weight/background alpha per rendered frame. Existing redAlpha observes the render pass; panel visibility accounts for alpha.

## Verification checkpoint2026-10-23

- Initial doctor failed at2.9GiB, below required3GiB. No other task artifacts were deleted.
- On user resume, free space5.1GiB. Shared runner was busy; scoped checks queued with its existing lock preserved.
- Scoped Quick20261023T084300:7/7PASS,0fail/skip, input231ffc87560fb4835927c63b5cfd1218f171e5173f6805a93921b26075f878ab.
  Covers initial transition tests, interrupted bars, actual scene Pause/results/three lifecycle cycles, camera-impact composition.
- Final scoped Quick20261023T090657:8/8PASS,0fail/skip,9.297s,
  input72394ff2f4b5a3c6bef8e7dd99f1175588737da22a801981217b31793a9744f5.
  Includes final weather/Volume property-block restoration and filtered per-renderer
  silhouette material weight. Dungeon Sprite fades lighting/flash toward its base
  silhouette color so active room specular cannot remain at the black endpoint.
- Intermediate runs retained as failed: test assembly initially lacked direct Volume
  reference (fixture changed to reflection); material block allocation in field
  initializer was moved to presentation startup; weather color expectation now uses
  the actual pre-transition renderer baseline instead of an unroundtripped literal.
  No thresholds, packages or gameplay contracts were weakened.
- git diff --check and JS syntax checks pass. Final scoped Unity checks pass; QA build/browser/frame acceptance still pending.
- Initial WebGL20261023T091007 stopped in the protected font gate before player
  output. CobblestoneUI64.asset: local2217448bytes/6014CRLF versus Linux2211434bytes;
  exact equality after CRLF-to-LF normalization. External adapter now normalizes
  known text snapshot files like prior project adapters. Source files and strict
  protected-font/repeated-builder checks remain unchanged. Build retry pending.
- No commit/push/merge or publication yet.

## Task resources

E:/CodexTemp/TypingHero/slayer-soft-transitions/run.py is an external adapter using the unchanged runner lock/toolchain. It adds supported Unity testFilter and build-only handoff; repository runner is unchanged.

Scoped filter: SlayerTransitionTests;ShogunPresentationTests.SlayerBarRevealSurvivesInterruptedExitPauseAndDisable;DungeonSessionTests.WorldSurvivesThreeGameplayCyclesAndResetsSlayerPause;CombatCameraImpactTests.PerspectiveCompositionLeavesBaseLensOwnedBySlayerAndPause

transition.cjs is a task-only real-input native QA driver using the existing fixture harness. Provider writes remain intercepted and external requests blocked. No bridge mutations.

## Original verification checklist (completed in receipts below)

1. Final filtered Unity checks and strict repeated builder.
2. One QA WebGL build and native1280/640 entry/exit, Pause/Resume,180/500ms hitches, lethal/results, Restart/Menu; verify input/score and camera impact.
3. Before visual review, record sources, exact start/end timestamps, purpose, frame/transition totals; inspect100% of selected complete segments, preserving original timing/held frames. Currently0output frames reviewed.
4. Reconcile only affected docs/game sections after runtime evidence.
5. Commit/push branch; fetch/integrate newer main, recheck affected changes, fast-forward origin/main, verify ancestry. Clean task worktree/local branch only after clean verified integration. Keep relevant review artifacts with cleanup conditions.

## Final QA player and pre-review selection

QA20261023T092633 built successfully with strict repeated builder and protected typing assets unchanged. Normalized input668e6246dcced708e7a562f06ea85bb849117e68da0bed7d17bd79dbdbf8f46b. Local immutable player: E:/CodexTemp/TypingHero/slayer-soft-transitions/player-colorfix.

Native-r4:32/32PASS,1280x720 and640x360, Edge155/D3D11/GTX1650Ti. Real keyboard input covers natural entry/exit,180ms hitch, Pause/Resume/timer, lethal/results with500ms hitch, Restart/Menu. No exceptions/provider writes. This is functional evidence, not every-frame visual acceptance yet.

Previous accepted function run(native-r3) used an overly bright raw HDR red(181). Added explicit sRGB-to-linear conversion. The final screenshot has the unchanged authored red at117 versus legacy118, a1/255 quantization difference; no intentional palette change or bit-identical color claim.

Selected complete segments below were recorded before visual review. Source videos are final native-r4 transition-1280.webm and transition-640.webm under the task artifact directory. Each includes stable preparation and a settled endpoint; pause/hitch and actor cleanup are included instead of narrowing them away. Full original frame timestamps and held intervals will be retained. At selection time0frames had been visually reviewed.

| Width | Source | Segment | Start(s) | End(s) | Reason |
|---|---|---|---:|---:|---|
| 1280 | transition-1280.webm | entry | 9.741605 | 10.749585 | complete entry with preparation and settled endpoint |
| 1280 | transition-1280.webm | exit | 19.837055 | 21.055635 | complete exit with preparation and settled endpoint |
| 1280 | transition-1280.webm | hitch-pause-entry | 29.740495 | 31.984860 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 1280 | transition-1280.webm | lethal-exit | 34.322525 | 35.584630 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |
| 640 | transition-640.webm | entry | 9.629875 | 10.644875 | complete entry with preparation and settled endpoint |
| 640 | transition-640.webm | exit | 19.733040 | 20.948290 | complete exit with preparation and settled endpoint |
| 640 | transition-640.webm | hitch-pause-entry | 29.609815 | 31.761550 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 640 | transition-640.webm | lethal-exit | 34.184360 | 35.394115 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |

Only the75new serialized Slayer timing/curve/version lines are reconciled from builder output into SampleScene. Unrelated generated tutorial-arrow/scene churn is not included; existing object IDs/references remain unchanged.

## Pre-integration visual receipt

Directly inspected all479frames and471consecutive transitions in the8selected
segments above, including sheet-boundary pairs and retained held-frame durations.
1280:49/48entry,62/61exit,98/97hitch+Pause entry,31/30lethal exit.
640:50/49entry,62/61exit,94/93hitch+Pause entry,33/32lethal exit.
No observed transition pop or stuck tint/bars; HUD/tray geometry stays fixed.
All sources/counts/manual observations and SHA256s are in native-r4/review/review-inventory.json.

origin/main advanced to1a7895c(sprite normals/spatial shadows) during validation.
The scoped implementation is verified on its original base; merge and recheck
render consumers before remote integration. Preserve both sides' intended behavior.

## Integration checkpoint

Task commitacda5b4 is pushed to origin/micharel09/slayer-soft-transitions.
Merged origin/main1a7895c as13d7304; the only conflict was appended verification
records, resolved by retaining the complete lighting receipt before the Slayer
heading. No source conflict and no blanket ours/theirs choice.

Integrated QA20261023T094605: build/strict repeated builder PASS, normalized
inputfeafdf96f9f5a77e39f7e07a502671f3be22c51c677ab89b12a4f9f8b2ec0bc6.
All29new authored normal-map PNGs match the immutable builder output byte-for-byte
(integrated-normal-identity.json). Native integrated run and final render review
are in progress; origin/main has not received Slayer yet.

## Integrated selection before visual review

Native-integrated32/32PASS on the merged player at both requested sizes, no
exceptions/provider reads/writes. This rechecks the changed lighting consumers;
prior timing/migration tests remain applicable because that runtime code and its
timing configuration are unchanged by the merge.

Sources: native-integrated/transition-1280.webm and transition-640.webm.
Selected before inspection, including complete preparation/end and all original
held intervals. At this selection checkpoint0integrated frames have been reviewed.

| Width | Source | Segment | Start(s) | End(s) | Reason |
|---|---|---|---:|---:|---|
| 1280 | transition-1280.webm | entry | 9.588630 | 10.587555 | complete entry with preparation and settled endpoint |
| 1280 | transition-1280.webm | exit | 19.678835 | 20.879970 | complete exit with preparation and settled endpoint |
| 1280 | transition-1280.webm | hitch-pause-entry | 29.580335 | 31.723325 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 1280 | transition-1280.webm | lethal-exit | 33.990975 | 35.303240 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |
| 640 | transition-640.webm | entry | 9.600555 | 10.598025 | complete entry with preparation and settled endpoint |
| 640 | transition-640.webm | exit | 19.702815 | 20.917115 | complete exit with preparation and settled endpoint |
| 640 | transition-640.webm | hitch-pause-entry | 29.624520 | 31.883780 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 640 | transition-640.webm | lethal-exit | 34.238970 | 35.448610 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |

## Final integrated acceptance

Integrated player13d7304 / QA20261023T094605 / inputfeafdf96f9f5a77e39f7e07a502671f3be22c51c677ab89b12a4f9f8b2ec0bc6:
strict repeated builder PASS;32/32native checks at both sizes,0exceptions/provider IO.
Reviewed483/483frames and475/475consecutive transitions across all8segments above,
including held frames, sheet-boundary pairs,180/500ms hitches and Pause/Resume.
1280:51/50entry,62/61exit,96/95hitch+Pause entry,35/34lethal exit.
640:48/47entry,63/62exit,98/97hitch+Pause entry,30/29lethal exit.
No observed transition pop or stuck state. Both normal palette and baseline restore
with the merged authored normals/shadows; all29normal PNGs are byte-identical to
origin/main's authored inputs after builder. Native-integrated/review inventory
records exact source frame ranges, timestamps, SHA256s and manual observations.

The previous479frame receipt remains historical. Reuse scoped Unity8/8 because
merge changes only lighting/shadow inputs, not timing/tint/lifecycle logic; final
render consumers were rechecked above. No full Quick/performance/stress/soak or
external-reference-match claim. Only the requested two viewports were exercised.

Acceptance complete; pending final documentation commit, remote fast-forward
verification and clean task-worktree removal. No itch.io upload.

Retain E:/CodexTemp/TypingHero/slayer-soft-transitions and immutable final server
result20261023T094605-webgl for review/reproduction. They can be removed after
review when no longer needed. Disposable browser profiles and redundant local
players/transfer archives will be removed after integration. Shared Unity source,
Library, toolchain and other jobs are preserved.

## Latest-main follow-up

Before final push, main advanced again toe882aba/668d3c2, adding guard-break and
body-fall dust cues. These intersect entry and lethal exit. Keep the verified
normal/shadow merge and all cue changes; recheck the new render consumers.
Timing/Pause/tint/lifecycle code is unchanged; reuse its scoped passing evidence.
Do not report remote integration until this follow-up passes and ancestry is verified.

## Final cue-integration selection before review

Merged latest main668d3c2 as c6f6802. QA20261023T100031, normalized input5613e279864d13272436f8a143f5d1692ba5f8f0e1558e39d2adc0f24b49fce1:
strict builder PASS; native-latest32/32PASS at1280/640,0exceptions/provider IO.
Select all8complete transition windows again so the new guard-break/body-fall
cues are inspected together with Pause/hitches and the restored normal view.
Source videos: native-latest/transition-1280.webm and transition-640.webm.
At selection time0latest frames have been visually inspected.

| Width | Source | Segment | Start(s) | End(s) | Reason |
|---|---|---|---:|---:|---|
| 1280 | transition-1280.webm | entry | 10.001850 | 11.012585 | complete entry with preparation and settled endpoint |
| 1280 | transition-1280.webm | exit | 20.091810 | 21.294650 | complete exit with preparation and settled endpoint |
| 1280 | transition-1280.webm | hitch-pause-entry | 32.992700 | 35.258120 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 1280 | transition-1280.webm | lethal-exit | 37.532935 | 38.743010 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |
| 640 | transition-640.webm | entry | 9.688670 | 10.702400 | complete entry with preparation and settled endpoint |
| 640 | transition-640.webm | exit | 19.784350 | 21.002415 | complete exit with preparation and settled endpoint |
| 640 | transition-640.webm | hitch-pause-entry | 29.685400 | 31.840510 | complete hitch-pause-entry with preparation and settled endpoint including 180ms hitch and Pause/Resume |
| 640 | transition-640.webm | lethal-exit | 34.095975 | 35.434905 | complete lethal-exit with preparation and settled endpoint including actor cleanup and 500ms hitch |


## Final cue-integration acceptance

Current runtime c6f6802 preserves main668d3c2 lighting and guard-break/body-fall cues.
QA20261023T100031-webgl strict repeat/protected-font checks PASS; normalized input
5613e279864d13272436f8a143f5d1692ba5f8f0e1558e39d2adc0f24b49fce1.
Native32/32PASS at1280x720 and640x360, zero exceptions/provider IO.
Directly inspected488/488frames and480/480consecutive transitions in all8windows
selected above, preserving original timestamps/held frames and sheet boundaries.
1280:53/52entry,65/64exit,99/98hitch+Pause,32/31lethal exit.
640:50/49entry,63/62exit,90/89hitch+Pause,36/35lethal exit.
No observed background/tint pop or stuck bars; guard-break sparks and body-fall
particles remain separate from the background blend. Pause holds, Resume settles,
results exit and Restart/Menu restore baseline. Camera impact composition passes.
All29authored normal PNGs remain byte-identical after builder. Reuse scoped Unity8/8
because the merged cues do not change the tested presentation clock/lifecycle logic;
the affected render consumers were checked in this final player. No gameplay timing,
input, damage or scoring changes; no itch.io upload.

Authoritative current artifacts: E:/CodexTemp/TypingHero/slayer-soft-transitions/
acceptance.json and native-latest/review/review-inventory.json (per-source SHA256,
frame ranges, counts, observations). Retain these artifacts and immutable server
result20261023T100031-webgl for review; remove after review when no longer needed.
Earlier483/479frame receipts are historical, superseded by this488frame receipt.
