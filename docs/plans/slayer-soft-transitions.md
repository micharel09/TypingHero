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

## Remaining acceptance

1. Final filtered Unity checks and strict repeated builder.
2. One QA WebGL build and native1280/640 entry/exit, Pause/Resume,180/500ms hitches, lethal/results, Restart/Menu; verify input/score and camera impact.
3. Before visual review, record sources, exact start/end timestamps, purpose, frame/transition totals; inspect100% of selected complete segments, preserving original timing/held frames. Currently0output frames reviewed.
4. Reconcile only affected docs/game sections after runtime evidence.
5. Commit/push branch; fetch/integrate newer main, recheck affected changes, fast-forward origin/main, verify ancestry. Clean task worktree/local branch only after clean verified integration. Keep relevant review artifacts with cleanup conditions.

## Final QA player and pre-review selection

QA20261023T092633 built successfully with strict repeated builder and protected typing assets unchanged. Normalized input668e6246dcced708e7a562f06ea85bb849117e68da0bed7d17bd79dbdbf8f46b. Local immutable player: E:/CodexTemp/TypingHero/slayer-soft-transitions/player-colorfix.

Native-r4:32/32PASS,1280x720 and640x360, Edge155/D3D11/GTX1650Ti. Real keyboard input covers natural entry/exit,180ms hitch, Pause/Resume/timer, lethal/results with500ms hitch, Restart/Menu. No exceptions/provider writes. This is functional evidence, not every-frame visual acceptance yet.

Previous accepted function run(native-r3) used an overly bright raw HDR red(181). Added explicit sRGB-to-linear conversion. The final screenshot has the unchanged authored red at117 versus legacy118, a1/255 quantization difference; no intentional palette change or bit-identical color claim.

Selected complete segments below were recorded before visual review. Source videos are final native-r4 transition-1280.webm and transition-640.webm under the task artifact directory. Each includes stable preparation and a settled endpoint; pause/hitch and actor cleanup are included instead of narrowing them away. Full original frame timestamps and held intervals will be retained. Currently0frames visually reviewed.

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
