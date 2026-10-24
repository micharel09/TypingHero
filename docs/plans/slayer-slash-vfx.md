# Slayer recipient-hit spark correction

## Current correction ? 2026-10-25

User rejected the sword-crescent artwork: this effect must confirm Skeleton receiving
player damage. User selected the existing contact spark, with the previous Slayer-only
scope (normal and lethal contacts). Historical slash acceptance below is superseded;
it does not validate the corrected asset.

Base origin/main903344c; isolated worktree
`C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-received-hit-spark`, branch
`micharel09/slayer-received-hit-spark`. Original dirty checkout and old demo preserved.

Implemented: both stable prefab GUIDs now reference exactly NormalHitSpark's existing
Pimen source cels1..6 (48x48, center pivot,100PPU). Hit150ms/32reference pixels,
finisher180ms/40pixels at16PPU; per-cel25/30ms, common scale5.5555556/6.9444444.
Existing pre-damage tint capture, optional lethal fallback, eight-impact pool,
recipient-contact position, pause and result cleanup code are unchanged. Builder
reuses stable source IDs without reimport/slicing; removed rejected tbbk sheet,
its notice and the extra build credits append. Existing Pimen packaging remains.

Source SHA256 `9c5e7e9b1776e982263b993861be1f239c315e5e77644d7883aeb92eab03dc08`.
Static validation PASS: exact six-frame references match normal prefab; widths,
durations and no rejected GUID verified; source PNG/importer, Player prefab,
spawner and runtime component bytes match base. `git diff --check` passes.
Scoped PlayMode assertions updated to require contact-spark references and six-cel
playback. They have NOT been executed for this revision.

Artifacts: `E:/CodexTemp/TypingHero/slayer-received-hit-spark/` contains eight
numbered preview sheets, source-review.json, static-checks.json, and isolated runner
adapters run.py/server.py. Previews show reference-pixel scale and3x nearest zoom;
they do not establish WebGL renderer output or playback timing.

BLOCKED: SSH fly-server100.115.189.84 and its configured LAN192.168.11.17 both
connect-timeout; Tailscale reports quanubutu offline, last seen2026-10-24T11:54:12.1Z.
Scoped `run.py webgl` also fails before reaching Unity. No local Unity Editor found.
Unity tests, strict two-pass builder, real-input1280x720 WebGL, appearance/disappearance/
overlap comparison with VFX disabled and demo remain UNVERIFIED. No integration
into main until these acceptance checks pass. The old demo is NOT the corrected build.

Resume: run the retained isolated runner when fly-server is online (no setup needed),
import only scoped generated prefab serialization if it differs, inspect actual
spark contacts/rapid hits/lethal tint/Pause-Resume/normal return, complete per-frame
runtime evidence, then fetch/integrate/recheck/push main and verify ancestry. Retain
this worktree and artifacts until acceptance and remote integration are complete.

### Static frame evidence ? corrected asset

Directly inspected all48/48 cels and40/40 consecutive pairs, eight sheets below.
F5 is transparent; release follows without a loop. Static silhouette/continuity
passes; playback, renderer and gameplay acceptance remain unverified.

| Variant | Frame | Duration | Source phase | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-right-white | F0 | 25ms | Existing spark source cel1 | Compact star, white core and short amber rays; right mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F0 |
| hit-right-white | F1 | 25ms | Existing spark source cel2 | Warm star, open core and separated short flecks; right mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F1 |
| hit-right-white | F2 | 25ms | Existing spark source cel3 | Thin central core with short surrounding rays; right mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F2 |
| hit-right-white | F3 | 25ms | Existing spark source cel4 | Sparse orange rays and points around fixed pivot; right mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F3 |
| hit-right-white | F4 | 25ms | Existing spark source cel5 | Few isolated small points; center largely clear; right mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F4 |
| hit-right-white | F5 | 25ms | Existing spark source cel6 | Fully transparent cell; right mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-white.png F5 |
| hit-right-red | F0 | 25ms | Existing spark source cel1 | Compact star, red core and short red rays; right mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F0 |
| hit-right-red | F1 | 25ms | Existing spark source cel2 | Red star, open core and separated short flecks; right mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F1 |
| hit-right-red | F2 | 25ms | Existing spark source cel3 | Thin central core with short surrounding rays; right mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F2 |
| hit-right-red | F3 | 25ms | Existing spark source cel4 | Sparse red rays and points around fixed pivot; right mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F3 |
| hit-right-red | F4 | 25ms | Existing spark source cel5 | Few isolated small points; center largely clear; right mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F4 |
| hit-right-red | F5 | 25ms | Existing spark source cel6 | Fully transparent cell; right mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-right-red.png F5 |
| hit-left-white | F0 | 25ms | Existing spark source cel1 | Compact star, white core and short amber rays; left mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F0 |
| hit-left-white | F1 | 25ms | Existing spark source cel2 | Warm star, open core and separated short flecks; left mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F1 |
| hit-left-white | F2 | 25ms | Existing spark source cel3 | Thin central core with short surrounding rays; left mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F2 |
| hit-left-white | F3 | 25ms | Existing spark source cel4 | Sparse orange rays and points around fixed pivot; left mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F3 |
| hit-left-white | F4 | 25ms | Existing spark source cel5 | Few isolated small points; center largely clear; left mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F4 |
| hit-left-white | F5 | 25ms | Existing spark source cel6 | Fully transparent cell; left mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-white.png F5 |
| hit-left-red | F0 | 25ms | Existing spark source cel1 | Compact star, red core and short red rays; left mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F0 |
| hit-left-red | F1 | 25ms | Existing spark source cel2 | Red star, open core and separated short flecks; left mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F1 |
| hit-left-red | F2 | 25ms | Existing spark source cel3 | Thin central core with short surrounding rays; left mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F2 |
| hit-left-red | F3 | 25ms | Existing spark source cel4 | Sparse red rays and points around fixed pivot; left mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F3 |
| hit-left-red | F4 | 25ms | Existing spark source cel5 | Few isolated small points; center largely clear; left mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F4 |
| hit-left-red | F5 | 25ms | Existing spark source cel6 | Fully transparent cell; left mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/hit-left-red.png F5 |
| finisher-right-white | F0 | 30ms | Existing spark source cel1 | Compact star, white core and short amber rays; right mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F0 |
| finisher-right-white | F1 | 30ms | Existing spark source cel2 | Warm star, open core and separated short flecks; right mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F1 |
| finisher-right-white | F2 | 30ms | Existing spark source cel3 | Thin central core with short surrounding rays; right mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F2 |
| finisher-right-white | F3 | 30ms | Existing spark source cel4 | Sparse orange rays and points around fixed pivot; right mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F3 |
| finisher-right-white | F4 | 30ms | Existing spark source cel5 | Few isolated small points; center largely clear; right mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F4 |
| finisher-right-white | F5 | 30ms | Existing spark source cel6 | Fully transparent cell; right mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-white.png F5 |
| finisher-right-red | F0 | 30ms | Existing spark source cel1 | Compact star, red core and short red rays; right mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F0 |
| finisher-right-red | F1 | 30ms | Existing spark source cel2 | Red star, open core and separated short flecks; right mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F1 |
| finisher-right-red | F2 | 30ms | Existing spark source cel3 | Thin central core with short surrounding rays; right mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F2 |
| finisher-right-red | F3 | 30ms | Existing spark source cel4 | Sparse red rays and points around fixed pivot; right mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F3 |
| finisher-right-red | F4 | 30ms | Existing spark source cel5 | Few isolated small points; center largely clear; right mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F4 |
| finisher-right-red | F5 | 30ms | Existing spark source cel6 | Fully transparent cell; right mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-right-red.png F5 |
| finisher-left-white | F0 | 30ms | Existing spark source cel1 | Compact star, white core and short amber rays; left mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F0 |
| finisher-left-white | F1 | 30ms | Existing spark source cel2 | Warm star, open core and separated short flecks; left mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F1 |
| finisher-left-white | F2 | 30ms | Existing spark source cel3 | Thin central core with short surrounding rays; left mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F2 |
| finisher-left-white | F3 | 30ms | Existing spark source cel4 | Sparse orange rays and points around fixed pivot; left mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F3 |
| finisher-left-white | F4 | 30ms | Existing spark source cel5 | Few isolated small points; center largely clear; left mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F4 |
| finisher-left-white | F5 | 30ms | Existing spark source cel6 | Fully transparent cell; left mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-white.png F5 |
| finisher-left-red | F0 | 30ms | Existing spark source cel1 | Compact star, red core and short red rays; left mirror | Core opens into warmer separated rays | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F0 |
| finisher-left-red | F1 | 30ms | Existing spark source cel2 | Red star, open core and separated short flecks; left mirror | Central light shrinks while rays thin | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F1 |
| finisher-left-red | F2 | 30ms | Existing spark source cel3 | Thin central core with short surrounding rays; left mirror | Core breaks into sparse orange traces | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F2 |
| finisher-left-red | F3 | 30ms | Existing spark source cel4 | Sparse red rays and points around fixed pivot; left mirror | Remaining traces reduce to isolated points | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F3 |
| finisher-left-red | F4 | 30ms | Existing spark source cel5 | Few isolated small points; center largely clear; left mirror | All residual points disappear | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F4 |
| finisher-left-red | F5 | 30ms | Existing spark source cel6 | Fully transparent cell; left mirror | No loop; release after final cell | static pass | E:/CodexTemp/TypingHero/slayer-received-hit-spark/finisher-left-red.png F5 |

## Historical rejected slash implementation

# Slayer slash impact replacement

Base origin/main46d4dda. Worktree C:/Users/ADMIN/orca/workspaces/TypingHero/slayer-slash-vfx.
Original dirty checkout preserved; no itch.io publication.

## Historical contract (rejected artwork)

Original tbbk CC0 nine-frame64x47 sheet; hit150ms/finisher180ms; visible width32/40px
at16PPU. Center pivot, direction mirror, immutable captured hit tint, no random transforms.
Replace only Slayer hit/lethal bindings. Keep parry, non-Slayer VFX and gameplay.
Eight impact leases per owner; pause/tutorial freeze; finish after Slayer exit;
clear on result overlay/disable/restart/unload.

Source receipt: E:/CodexTemp/TypingHero/slayer-vfx-plan-20261024/asset-source-review.json.
PNG SHA2567642aa4698828f6cc908b3fd2873c3b3d8555138bae188991c6b775ae0c96915.
Attribution/license beside imported asset. ScourgeBringer research remains limited
to the previous recorded scope; no complete reference-match claim.

## Acceptance checklist

Scoped Unity routing/contact/pool/lifecycle; stable two-pass LF builder; direct
per-frame/per-transition review of all eight duration/direction/tint variants;
WebGL1280x720 real input with read-only bridge/provider isolation; local demo;
commit/push/integration/remote ancestry/cleanup. Runtime/asset verification is complete in the final receipts below; integration is verified in the final receipt below.

## Static visual review (2026-10-24)

Directly opened original source sheet and all eight adaptation sheets under
E:/CodexTemp/TypingHero/slayer-vfx-implementation/. Both native reference size and
3x nearest-neighbor are shown in every sheet. All72/72 adapted cels and64/64
adjacent pairs inspected. These are CPU nearest-neighbor previews of the selected
scale/mirror/tint, **not runtime renderer or playback acceptance**. F0 is only
one/two pixels at native hit size; its faint preparation is original artwork.
No clipping, pivot recentering or detached physics fragments observed. At this initial checkpoint runtime
readability/overlap/on-off were pending; final native evidence below supersedes it.

### hit-right-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-right-white | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-right-white.png F0 |
| hit-right-white | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-right-white.png F1 |
| hit-right-white | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-right-white.png F2 |
| hit-right-white | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-right-white.png F3 |
| hit-right-white | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-right-white.png F4 |
| hit-right-white | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-right-white.png F5 |
| hit-right-white | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-right-white.png F6 |
| hit-right-white | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-right-white.png F7 |
| hit-right-white | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side right; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-right-white.png F8 |

### hit-right-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-right-red | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-right-red.png F0 |
| hit-right-red | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-right-red.png F1 |
| hit-right-red | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-right-red.png F2 |
| hit-right-red | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-right-red.png F3 |
| hit-right-red | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-right-red.png F4 |
| hit-right-red | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-right-red.png F5 |
| hit-right-red | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-right-red.png F6 |
| hit-right-red | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-right-red.png F7 |
| hit-right-red | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side right; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-right-red.png F8 |

### hit-left-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-left-white | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-left-white.png F0 |
| hit-left-white | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-left-white.png F1 |
| hit-left-white | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-left-white.png F2 |
| hit-left-white | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-left-white.png F3 |
| hit-left-white | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-left-white.png F4 |
| hit-left-white | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-left-white.png F5 |
| hit-left-white | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-left-white.png F6 |
| hit-left-white | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-left-white.png F7 |
| hit-left-white | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side left; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-left-white.png F8 |

### hit-left-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| hit-left-red | F0 | 16.666667ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | hit-left-red.png F0 |
| hit-left-red | F1 | 16.666667ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | hit-left-red.png F1 |
| hit-left-red | F2 | 16.666667ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | hit-left-red.png F2 |
| hit-left-red | F3 | 16.666667ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | hit-left-red.png F3 |
| hit-left-red | F4 | 16.666667ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | hit-left-red.png F4 |
| hit-left-red | F5 | 16.666667ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | hit-left-red.png F5 |
| hit-left-red | F6 | 16.666667ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | hit-left-red.png F6 |
| hit-left-red | F7 | 16.666667ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | hit-left-red.png F7 |
| hit-left-red | F8 | 16.666667ms | Source F8, fixed center | Short low trace; no block debris; leading side left; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | hit-left-red.png F8 |

### finisher-right-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-right-white | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-right-white.png F0 |
| finisher-right-white | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-right-white.png F1 |
| finisher-right-white | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-right-white.png F2 |
| finisher-right-white | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-right-white.png F3 |
| finisher-right-white | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-right-white.png F4 |
| finisher-right-white | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-right-white.png F5 |
| finisher-right-white | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-right-white.png F6 |
| finisher-right-white | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-right-white.png F7 |
| finisher-right-white | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side right; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-right-white.png F8 |

### finisher-right-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-right-red | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side right; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-right-red.png F0 |
| finisher-right-red | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side right; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-right-red.png F1 |
| finisher-right-red | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side right; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-right-red.png F2 |
| finisher-right-red | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side right; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-right-red.png F3 |
| finisher-right-red | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side right; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-right-red.png F4 |
| finisher-right-red | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side right; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-right-red.png F5 |
| finisher-right-red | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side right; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-right-red.png F6 |
| finisher-right-red | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side right; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-right-red.png F7 |
| finisher-right-red | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side right; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-right-red.png F8 |

### finisher-left-white

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-left-white | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; original gray/white | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-left-white.png F0 |
| finisher-left-white | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; original gray/white | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-left-white.png F1 |
| finisher-left-white | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; original gray/white | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-left-white.png F2 |
| finisher-left-white | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; original gray/white | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-left-white.png F3 |
| finisher-left-white | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; original gray/white | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-left-white.png F4 |
| finisher-left-white | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; original gray/white | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-left-white.png F5 |
| finisher-left-white | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; original gray/white | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-left-white.png F6 |
| finisher-left-white | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; original gray/white | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-left-white.png F7 |
| finisher-left-white | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side left; original gray/white | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-left-white.png F8 |

### finisher-left-red

| Variant | Frame | Duration | Reference | Observed frame | Next transition | Status | Evidence |
|---|---|---|---|---|---|---|---|
| finisher-left-red | F0 | 20ms | Source F0, fixed center | Small diagonal seed near the upper edge; leading side left; red with original brightness levels | Extends horizontally from the same upper region | Static reviewed; final runtime receipt below | finisher-left-red.png F0 |
| finisher-left-red | F1 | 20ms | Source F1, fixed center | Thin upper stroke, wider and still gray; leading side left; red with original brightness levels | Leading edge curls down into the side arc | Static reviewed; final runtime receipt below | finisher-left-red.png F1 |
| finisher-left-red | F2 | 20ms | Source F2, fixed center | Thicker upper arc with rounded leading edge; leading side left; red with original brightness levels | Opens into the largest bright crescent | Static reviewed; final runtime receipt below | finisher-left-red.png F2 |
| finisher-left-red | F3 | 20ms | Source F3, fixed center | White crescent, open back; tiny tears beside upper rim; leading side left; red with original brightness levels | Same arc dims gray and upper rim breaks up | Static reviewed; final runtime receipt below | finisher-left-red.png F3 |
| finisher-left-red | F4 | 20ms | Source F4, fixed center | Gray crescent with rim tears confined beside the arc; leading side left; red with original brightness levels | Upper/back rim disappears, leaving lower arc and leading streaks | Static reviewed; final runtime receipt below | finisher-left-red.png F4 |
| finisher-left-red | F5 | 20ms | Source F5, fixed center | Lower crescent and three slim leading streaks; leading side left; red with original brightness levels | Leading streaks shorten while lower arc remains | Static reviewed; final runtime receipt below | finisher-left-red.png F5 |
| finisher-left-red | F6 | 20ms | Source F6, fixed center | Thin lower arc with short upright streaks; leading side left; red with original brightness levels | Lower arc narrows and leading tip shortens | Static reviewed; final runtime receipt below | finisher-left-red.png F6 |
| finisher-left-red | F7 | 20ms | Source F7, fixed center | Narrow lower arc and small remaining tip; leading side left; red with original brightness levels | Collapses to a short low trace | Static reviewed; final runtime receipt below | finisher-left-red.png F7 |
| finisher-left-red | F8 | 20ms | Source F8, fixed center | Short low trace; no block debris; leading side left; red with original brightness levels | Nonloop end to transparent/pool; boundary verified in final tests | Static reviewed; final runtime receipt below | finisher-left-red.png F8 |


## Runner checkpoint

First60s lock wait expired before Unity ran (no test/build claim). Retried with
3600s lock wait; shared jobs preserved. Snapshot adapter only normalizes known
text extensions toLF; PNG/font bytes stay intact. Source/build/results isolated
under /tmp/typinghero-slayer-slash-vfx; shared runner lock, pinned image, Library,
cache and private license reused. No changes to repository runner/toolchain.
Static review hashes retained in external artifact root/static-review-hashes.json.

## Initial serialize receipt

Unity2022.3.25f1/fly-server20261024T111720-serialize PASS, snapshot
24a74dd60b207b7f448dac521c95ebe4d8db8b1442212d7f41b2e1f369361270.
Strict repeated builder passed for the initial implementation. This initial
snapshot predates the final root-metadata stability list, tint fallback correction,
expanded regression tests and QA observations; it is not final acceptance.
Receipt archived locally in initial-serialization.tar.gz. Final combined scoped
checks/build started20261024T112049-webgl, input
3f0bb14c2649b8b63ed5bf8a8f796efe0189a8ff658c22d170517c1fbd06c9a6.

## Scoped test pilot

20261024T112049-webgl: strict serialize stage succeeded;50PlayMode cases ran,
49PASS/1teardown failure in OrdinarySlayerContactRejectsClosedDeadAndReadingTargets.
The manually activated PlayerSlayerMode fixture lacked an Animator; OnDisable.Exit
therefore logged NullReferenceException after all behavior assertions passed.
Fixed by undoing the manual active flag at the end of that fixture; no production
fix was needed. Original XML saved as tests-initial.xml. WebGL build did not run.

Imported Unity-generated SlayerHits assets/metas and only the two new role
bindings into Player.prefab. SampleScene inherits that prefab with no Slayer
binding overrides, so no scene edit is needed. Full scene builder output contains
unrelated regenerated UI IDs; retained the authored scene instead. Skeleton.prefab
output has no diff. New asset GUID/sprite IDs are now included in the retry input.

## Final scoped Unity receipt

Unity2022.3.25f1/fly-server20261024T113433-webgl, canonicalLF input
`dc32bba70c576eaf074b00dc3e38f6ba9e675fe7ddf5f4f45c610270ee5d24f1`.
Strict repeated SerializeOnly PASS including every SlayerHits file/metadata.
PlayMode **50/50PASS**,0failed/skipped,8.8784652s:18SlayerHitVfxTests,
18CombatFeedbackTests,9NormalStrikeEffectsTests,5directly affected ParryClashTests.
Covers original PNG hash/slices/pivot/scale/timing, every scheduled cel, both
mirrors, immutable RGB tint/opaque renderer alpha, real contact/duplicate/dead/
closed/reading guards, lethal pre-damage capture, legacy/non-Slayer routing,
shared eight-impact oldest-first reclaim,50/100/200ms cadence, pause/reading,
reuse, post-finish tail, result/disable/unload cleanup and old parry isolation.
No gameplay, typing/balance or broad animation suite change/claim.
WebGL build and native visual verification remain pending at this checkpoint.

## Native and selected output review scope (recorded before frame inspection)

Immutable player9b76edc5beddd984,33/33local/server SHA256 match. Native2 PASS:
Edge156.0.4314.42/NVIDIA GTX1650Ti/ANGLE D3D11/1280x720, real keyboard/mouse,
read-only bridge; no managed/JS exceptions or provider calls. Native1 driver
paused after the150ms tail had already expired; no runtime failure. Native2
pauses immediately after the render-frame contact observation and verifies an
unchanged frame_3/age0.06154000014/tint during pause, then resumes.

Video: video/24179a6590825748b109d10a5280aa59.webm, SHA256
544ad07cda76ff7daf7424b6010b3955ee53b1e022f1cb50082ba8271d3aeaac. Original ffprobe PTS retained; passthrough frame decode,
no fps resampling or dropped held/repeated frames. Event clock is approximate;
windows include context and are validated visually, not treated as precise input
latency measurements. These are output review, separate from reference research.

| Segment | Start/end seconds | Frames / pairs | Reason | Inspected |
|---|---|---|---|---|
| hit-pause-resume | 25.5?27.85 | 59/58 | Slash birth, pause hold, resume and complete tail | pending |
| normal-exit | 36.4?38.3 | 48/47 | Full mode exit and actors without impact | pending |
| white-rapid | 55.65?56.85 | 30/29 | First complete word after second entry, overlapping white hits | pending |
| red-finisher | 61.3?64.1 | 70/69 | Final three words, overlapping red hits and lethal tail | pending |

207frames/203pairs selected across four complete behavior windows; review pending.

Visual validation found the event clock increasingly differs from encoded PTS:
initial white-rapid contains idle preparation, and red-finisher ends before the
actual lethal tail. Before reviewing extra frames, expanded coverage to55.50?60.20s
and61.30?video end68.60s; retained original frames/PTS and their review evidence.
No window was shortened. Extensions listed in runtime-review/manifest.json.
The unchanged large ivory weapon trail is distinguishable from the smaller gray/
red hit crescent; empty-impact samples retain that original trail. Do not label
that preserved artwork as a new Slayer hit or claim it was removed.

## Final visual acceptance

All72/72static adaptation cels and64/64source-adjacent pairs inspected at native
reference size and3xnearest; all407/407selected encoded output frames and403/403
adjacent pairs inspected, including extension boundaries. Four final windows:
25.50?27.85s (59/58),36.40?38.30s (48/47),55.50?60.20s (118/117),
61.30?68.60s (182/181). Numbered source images and per-frame runtime notes/PTS/
sheet hashes are in runtime-review/manifest.json and per-frame-review.csv.

Small gray/white crescent opens at contact, becomes a thin lower trace and clears;
white-to-yellow/red rapid hits overlap without producing falling fragments or a
continuous new opaque body mask. Existing ivory weapon trail and parry particles
remain visible by design; empty-impact samples show that original large trail
without the new crescent, providing an on/off comparison in the same scene.
Native observations saw up to two concurrent slash impacts; Unity tests cover the
full eight-impact limit. The larger red finisher remains red as Slayer ends,
vanishes before enemy death/results, and leaves no retained effect. Source alpha,
fixed pivot, no random motion and all nine original cels are preserved.

Both directions and endpoint tints/durations were directly reviewed in the static
variants; native real-input production combat covers its default right-facing
path. Encoded25fps output quantizes visible cels, so this is not a claim that every
16.667/20ms source cel appears in that recording. All nine scheduled cels and both
mirror flags are independently verified by Unity. No full character-animation,
reference-game equivalence, performance/stress/soak or subjective audio claim.
No artwork, shader, controller or lighting-map changes outside the new asset root.
Only trailing YAML whitespace in new assets/metas was trimmed for diff hygiene;
all generated values/bindings/IDs match the accepted Unity export.

## Demo and integration checkpoint

Local immutable QA demo: http://127.0.0.1:8364/ . Host PID25340,
E:/CodexTemp/TypingHero/slayer-vfx-implementation/host.py. Fresh native browser
smoke PASS: player loads at1280x720, service worker controls navigation, explicit
Firebase GET returns X-Clackblade-Demo:local-fixture,0real external requests or
page errors. Source player33hashes remain unchanged; only host sizing/bootstrap
and provider fixtures wrap it. Host/evidence remain for user review; stop PID25340
and remove this task artifact directory only when the demo/evidence are no longer
needed. No itch.io publication.

Upstream7d69938 changes production-only Brotli and its scan, plus documentation.
QA remains uncompressed; runtime VFX/gameplay sources and assets are unaffected.
Preserve both that upstream behavior and this task's appended source notice when
integrating. Reuse the scoped/runtime results for that unrelated merge; no second
QA/full-suite build is required by an unchanged QA path.

## Remote integration receipt

Feature commit67a94d9; merged upstream7d69938 as09e3389. Push to origin/main was
fast-forward7d69938?09e33897fe27aee15d024648f57b891f6f64e286, then fetched and
verified HEAD==origin/main and67a94d9is an ancestor. Both verification entries
were kept when resolving their documentation-only conflict. All245C#source files
other than the reviewed production build helper still match the accepted LF input.
No VFX/gameplay source or asset changed after the passing build; QA compression
semantics stay unchanged. Source PNG hash and generated sprite/prefab bindings
were rechecked. The original dirty checkout was never checked out or modified.

A final documentation-only receipt commit records this integration and cleanup.
Retain remote task branch micharel09/slayer-slash-vfx (no automatic remote deletion).
Remove the clean task-created worktree/local branch after confirming that receipt
on remote main. Remove only /tmp/typinghero-slayer-slash-vfx after verifying no
active container mounts it; all required player/source/receipts are already archived
under the local demo/evidence root. Shared Library/toolchain/cache/license and all
pre-existing resources remain untouched. Demo host/evidence retention is described
above; its cleanup condition is completion of user review.
