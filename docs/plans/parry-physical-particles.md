# Parry physical particles ? 2026-10-21

## Acceptance and decisions

- 24 particles: two approved Pimen contact particles, eighteen attacker-facing rays/fragments, four 1?2px specks near the player.
- Continuous -20..50deg fan with +/-3deg jitter, 14?24WU/s; two fragments live120?180ms. Specks1?2.5WU/s, radius.12WU/offset.08WU toward player, drag6/s, max450ms and <.6WU horizontal displacement.
- Reusable Inspector profile and presentation-independent absolute-age solver; floor only, one optional bounce. No per-particle GameObjects/Rigidbodies or per-frame allocation.
- Preserve production contact snapshots/Pimen art, unscaled pause-held birth clock, lighting, all outgoing effects, and eight leases/192particles. Only migrate BCompact.
- Base origin/main edfc3b6 includes newer Pimen/outgoing normal effects than the planning checkout; preserve these. Parent checkout/WIP untouched.

## Implementation

- Orca child C:/Users/ADMIN/orca/workspaces/TypingHero/parry-physical-particles, branch micharel09/parry-physical-particles.
- PhysicalParticleProfile asset groups feed PhysicalParticleSimulation; ParryClashPixel renders the resulting states using existing Compact material.
- Back renderer remains reserved for Pimen outer contact; physical near rays/specks share Front, far rays use Embers. Buffers expanded to22 to accommodate12near particles safely.
- Builder only fills an absent profile reference, preserving authored tuning.

## Authoring contract

Select Assets/Art/VFX/ParryClash/ParryPhysics.asset or duplicate it with Unity. Edit groups in Inspector; physics settings are copied at emission, so edits affect subsequent bursts. The generic solver capacity is supplied by its consumer; BCompact reserves22physical slots plus2authored contacts. Keep its group count sum<=22. Renderer channels1/MainRays,2/Front and4/Embers carry fragments, near rays/specks and far rays;0/3remain Pimen contact renderers. Size ranges use source pixels at16PPU. Flight drag affects horizontal/depth motion; Y remains ballistic after the launch phase. No wall/actor collision is implemented in v1.

## Verification status

Implementation and scoped verification complete. Shared runner was initially occupied. Canceled only this task's pending initial process982780 (verified task command, lock-wait state and no children) before any Unity execution, replacing it with the final snapshot.

Canonical input8dfc8fc887f21b993b5cbbfaa8aa81841c123391479b0a1373a5208eb2a7d360, run20261021T145852-webgl: focused ParryClashTests + PhysicalParticleTests then QA build/repeated builder in one exclusive run. External adapter only normalizes known text inputs to LF and selects the existing Unity test filter; no product/runner source changes. Focused Unity37/37 PASS,0failed/skipped; both QA builds/repeated builders pass. Native1280x720 actual-keyboard right/left15checks each, all110captured frames/108transitions and the five post-merge integration tests pass. Full source/limits are recorded below.

## Resources

Task artifacts: E:/CodexTemp/TypingHero/parry-physical-particles (runner adapters, receipts, captures). Shared server jobs/cache/source/license must be preserved. Retain task worktree until verified integration, then remove only clean task-owned resources.


## Final scoped acceptance

- Canonical Unity37/37PASS,0fail/skip,13.2257312s. Normal145852 and mirror150937 QA builds/strict repeated builder PASS. Both task-owned preset files match canonical Unity serialization; comparison receipt source-comparison.json shows all mirror inputs identical except PixelSparkMirrorQA.cs.
- Edge155.0.4283.62/Playwright1.58.2/Intel UHD D3D11/1280x720, normal-battle-r3 and mirror-battle15/15each PASS;0exceptions/provider reads/writes/external requests. Real Space, full24cap, direction, health, pause and second parry cleanup verified. Tutorial normal-native-r2 additionally passes15checks, but upper-fan occlusion excludes it from final visual acceptance.
- Selected complete final segments: normal-battle-r3/flight-right and mirror-battle/flight-left, each frame0000..0054. Each contains55/55directly inspected frames and54/54consecutive transitions across sheet00..06, preserving held/repeated frames. Full anticipation, birth/hold, attacker-facing spread, falling/cooling, floor tail and expiry included; no long reverse spray, visible floor penetration or residue observed. Detailed per-frame findings: each *-review/per-frame.csv and review.json.
- VFR silent exports from those exact sequences preserve all55frames and source timing within1ms container precision. parry-right.mp4:1.828s, SHA256fd1245f9a2d1017202784077703c63b6fb1eaad2b227a869a5cf3779ba767043. parry-left.mp4:1.832s, SHA256345ed2151d42a6b3a37a52b82f8d9c67a9c4e499c30be84bf99df61320def0c4. Original post-render PNGs/JSON retain exact callback timestamps. Native webm recordings include the complete input/pause sessions.
- Captured-frame review does not certify unobserved engine frames or FPS/performance. Initial native setup failed before execution due missing copied snapshot; r1 missed the second cue due fixed2.2s delay; battle pilot waited on the wrong intro flag; r2's .02..15s probe window missed10Hz samples. These are retained failed harness pilots, not product PASS. Final driver waits for full particle expiry, handles encounterReading and observes launch/early-flight samples0..400ms; physics/gameplay source never changed after canonical tests.
- Integrated ee5bd8f compact menus/pause cleanly in82df496. Focused integration20261021T154930-quick, input66299c77545a38c5508e4f14765b7c207d32e7450fd6459acc1ba77e12986566, passes5/5/0fail/skip in4.4420984s: real TogglePause/Resume freezes gameplay, particle/pulse pause/reuse, attacker context and profile binding. Scene VFX components and both actor prefabs/instances remain unchanged from canonical acceptance.
- Later origin/main1611145 adds only dungeon MainMenu presentation/preparation-card work. Merged cleanly; all relevant VFX, pause, combat scene, actor/preset, selected test and version/package inputs match the passing integration snapshot exactly, so results are reused. New menu documentation and code preserved. Final diff whitespace/new-link checks pass.

## Delivery and cleanup

- Feature branch micharel09/parry-physical-particles and origin/main both verified at8126f0f62f7c1568f93f5000a3b5203c02ccb4d4; implementation515df92 and final task HEAD are ancestors of origin/main. This final checkpoint closure is documentation-only. Parent dirty main is preserved.
- Retain E:/CodexTemp/TypingHero/parry-physical-particles for review/reproduction: both silent VFR clips, original native captures/JSON, per-frame CSV/sheets, XML/browser receipts, isolated normal/mirror players, input archives and external runner/fixture scripts. Disposable after the user no longer needs comparison or reproduction.
- Six task Edge profiles were removed only after verifying no task Edge process remained. Task remote root /data/typinghero-game-loop/task-parry-physical-particles was removed after copying final receipts and verifying no running container mounted it; shared source/library/cache/license/other jobs are preserved. Clean task worktree/local branch cleanup follows verified remote integration; remote task branch is retained for review and may be deleted manually once no longer needed.
