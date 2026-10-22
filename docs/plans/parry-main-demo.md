# Latest main + original parry playable demo ? 2026-10-22

User requested fetching the latest primary branch and reopening the demo.
Origin has no master branch; origin/HEAD is main. Fetched tip:
`fd3f21de716964e4cb1dc60a4ce21e70e67d29f0` from
`https://github.com/micharel09/TypingHero.git`.

Created Orca child `micharel09/parry-main-demo` at
`C:/Users/ADMIN/orca/workspaces/TypingHero/parry-main-demo`, based on that tip,
parent `C:/Users/ADMIN/orca/workspaces/TypingHero/kaluga`. Parent dirty checkout
and unrelated WIP are untouched. This is a local demo integration; no remote
publication or integration requested.

Transferred only original-sprite parry clips/state transition changes, the
800ms one-word follow-up and iframe/bypass consumers, builder and tests. Three-way
integration retained main's contact snapshots, cleaned enemy contact cels,
physical ember-tail VFX and damage notification event. Success no longer inherits
main's old automatic-attack events. The builder now maps contact defenses to
original frame2 as well, so the main LateUpdate contact hold cannot restore0/1.
HUD guide, pause fixes, dungeon and all other main sources remain on their main base.

Protected56 original asset/import/clip-GUID files against the latest-main Git
blobs; all match main, with no pixel edits by this integration. Refreshed13 older
contract hashes to those unchanged main bytes. Decoded combo_3_end and idle PNG
pixels are identical to the previously reviewed parent source. Static source
2/3/4/5/idle plate re-inspected at nearest2x. Source frame2 has raised blade/grip,
3 lowers that blade slightly,4 retains the authored detached blue blade fragments,
5 releases those fragments, idle restores the lowered hand/support stance.
No source silhouette, equipment, pivot or normal redraw. This is static/source
inspection; perceived continuous motion and new runtime VFX alignment are not
accepted by that observation.

Input snapshot for scoped checks/build:
`8f7076249f0168cee5d1c5fd5851137f62a016ef8396deebc9323f205aa35a72`.
Static56-file/GUID/clip/timer/retired-reference checks and preview fake-clock tests
PASS. Scoped Unity26-case parry/contact run and WebGL demo build pending shared
runner availability. Temporary runner changes only select testFilter, serialize
before tests, and retain the QA player without launching an unrelated browser suite.

Temporary resources: `E:/CodexTemp/TypingHero/parry-main-demo`. Retain source child
and local demo player/host during user review, then close only the owned host/page
and remove owned temp resources. Keep the child while unintegrated source changes
are needed. Original demo remains available until verified replacement is ready.

The first scoped quick attempt timed out after600s waiting for the shared lock;
no tests executed and no acceptance inferred. Retry combines scoped26 tests and
the QA build under one lock acquisition, waiting up to3600s. Other jobs preserved.

The first execution140156 failed before tests: main SlashVolumeAssets still
required attack events in the now-defensive success clip. Updated its attack-only
consumer list, replaced obsolete success-as-attack test cases with a no-word
no-attack-window regression, and retained actual attack slash/contact validators.
Expanded scope31 cases to cover these directly affected consumers. Retry input:
`5d9100b34377d7661d152b2b9082349472c5fba6fd2e4c5d28b0f0562b132019`. No tests/build PASS claimed for140156.

Second execution141205 failed before tests at the protected typing-font guard.
PlayerParryAssets.SaveAssets ran mid-pipeline and flushed loaded dirty font assets.
Internal Apply now defers saving to main PixelUIBuild; the standalone menu wrapper
still saves explicitly. Font protection and strict repeat checks remain unchanged.
Retry input `f3ab734743f8f787ee2044cee7150649660b34b26f9cb3fcd677d725025a35ae`, same31-case scope.

Third execution141448 reproduced the font guard. Byte comparison establishes
that CobblestoneUI64.asset differs ONLY by6014 CRLF-to-LF line endings; normalized
contents are identical. The mid-pipeline-save hypothesis was incorrect. Restored
the original parry Apply/menu structure and wrote the font asset using its exact
canonical origin/main blob (no Git content change). No font data, protection or
acceptance rule changed. Final retry input `e17c70958e4e7cf0ac08e71d65ff7c03293c623fe23bf5eea878717eed6a7382`.

Final execution141745 on e17c70958e4e7cf0ac08e71d65ff7c03293c623fe23bf5eea878717eed6a7382:
strict repeated serialization/font protection PASS, scoped31/31 Unity tests PASS,
0failed/skipped,2.4495452s. Includes the new no-word no-hitbox case, transformed
main contact math/entry frame, existing actual attack contact/damage/spark, and
missing-contact validator. WebGL build now running under the same lock/snapshot.
Post-serialize protected hash audit: 55/56 exact.
The remaining normal-importer meta differs only in trailing spaces on empty
YAML scalar fields added by Unity serialization. Settings/GUID are identical;
original normal PNG and all source sprite pixels/import references are unchanged.
Input contract remains exact56/56 against main; do not claim post-serialize56/56.

## Playable result

Final WebGL141745 build/strict repeated builder PASS. Downloaded32files/
48,027,035bytes; every local SHA256 matches the immutable server player. Imported
its tested22-row ParryContacts.asset into the source checkout, preserving its GUID
and normalizing only empty-field trailing whitespace for Git. All rows now use
original frame2 for defense, including editor play before rebuilding. Numeric
values/references equal the asset used by the passing31 tests and compiled player.
Frozen player input remains e17c7095; resulting source-after-generated-asset hash:
`e6cdb4ad2bb0e996162b56e40fc1c8fe4d57c24fa6c85d83243d6eaac4b3ccb2`.

Native Edge155.0.4283.62 / Playwright1.58.2 / Intel UHD ANGLE D3D11,640x360:
**17/17PASS in15.865s**, real keyboard/mouse, read-only QA bridge, existing unchanged
browser.cjs fixture/external interception harness. Normal and contact entry show
original frame2; no-word success returns idle with no attack; two completed words
release one follow-up and exactly10 damage (180->170), player40HP, two parries.
Pause holds the accepted follow-up and ignores qz input; resume releases it.
Sorting mismatches0;0JS/managed exceptions,0fixture/provider reads/writes. This is
right-facing1x functional integration coverage; dense direction/speed/key/boundary
checks remain the31-case Unity receipt, not new browser coverage of those variants.
First native pilots failed because the harness sent Space during the still-frozen
encounter intro, then waited for a running clock without clicking its Fight button.
Corrected only harness UI sequencing and reused the exact same player; no gameplay
change or relaxed acceptance. Initial4/3 checks are not counted as passes.

Actual Orca menu and native success-contact/paused-queue captures inspected:
latest dungeon/menu/HUD skin, original raised grip/blade and main physical sparks
remain visible. Continuous perceived force/fluidity and every runtime VFX alignment
are unverified; no full difficulty, performance, balance or release claim.

Demo is open in parent kaluga's Orca browser at http://127.0.0.1:8136/,
page e98327a8-914c-45c1-8810-75f3e56380f7; owned host terminal
term_493bae17-a400-4301-9582-111f117ae4ee. Source child stays at
C:/Users/ADMIN/orca/workspaces/TypingHero/parry-main-demo,
branch micharel09/parry-main-demo, based on origin/main fd3f21d. Retain this source
and E:/CodexTemp/TypingHero/parry-main-demo host/player/session/manifest/worker
while user review is open. Close the owned host/page and remove owned temp when
review ends; retain unintegrated source child until its changes are preserved.
No push, merge or publication. Parent WIP untouched.

Cleanup complete: removed four owned server result directories after snapshot,
symlink and active-mount checks; removed temporary helpers/captures/isolated QA
profile and the retired e2 demo after ownership/hash/process checks. Retained only
the current32-file player, host/fixture worker, manifest and session/ready markers.
All32 hashes rechecked unchanged; only the requested user-play host/page remain.
Source child kept for review; parent checkout and all other jobs preserved.

## Remote advance during delivery

Final remote check found main916c752 (tutorial callout cards9927025 and floor-rest
embers7077a35) beyond the initially fetched fd3f21d. Preserved verified fd demo
source in local commit c568bb1, merged latest main in the same child, retaining both
documentation sections. No runtime C# changed upstream; its scene/editor guide and
particle profile changes are retained. No remote push/merge. Rebuilding latest
input `662223b4e2dc26cddad814d0668c6a2f6e258e6e41dc858493e1f69fd3122cf7`, scoped33 cases (prior31 plus two-sided floor-rest cases).
Existing fd demo stays available until this latest replacement is verified.

The first916 attempt stopped at the existing3GiB disk gate before Unity.
Temporary helper now removes only its own just-extracted duplicate input archive
under the shared lock after verifying snapshot662223b4. Source/Library/cache and
other jobs stay intact; disk thresholds and validation unchanged. Retrying same
source input, not claiming the capacity failure as tests/build PASS.

Capacity retry still lacked a few MiB at the strict3GiB gate after archive cleanup.
Verified the previous shared QA output matches all32 files of this task local
e17 demo exactly. Temporary helper may retire only that exact old artifact under
the shared lock (and asserts its full manifest again), retaining the verified
local player and leaving other tasks/cache/source intact. No gate lowered.

Data volume still lacked the configured3GiB, so latest916 verification moves
to an owned /tmp/typinghero-parry-main-demo-916-662223b4 workspace (tmpfs7.5GiB
free before cloning). Same global /data runner.lock, pinned image/CPU/memory,
activated private HOME and unchanged3GiB thresholds. Library/cache are copied
under that lock; originals and shared source are preserved. Only this private
workspace/output is eligible for later cleanup. No dependency or service setup.


## Latest main demo verified (2026-10-22)

Local integration includes main916c75291250c34aed774db1c7ecc489b435b1b0.
Frozen source662223b4e2dc26cddad814d0668c6a2f6e258e6e41dc858493e1f69fd3122cf7:
33/33 scoped Unity cases passed, zero failed/skipped,2.6048547s, including
both cooled-floor-ember cases. Strict builder repeat/font protection and WebGL
build passed on pinned Unity2022.3.25f1. Native Edge155.0.4283.62,
Playwright1.58.2,640x360, NVIDIA GTX1650Ti ANGLE D3D11:20/20 checks passed
in16.998s. Real keyboard/mouse confirmed latest guide cards/text/arrows,
original frame2 on press/contact, no-word recovery without attack, two typed
words queue exactly one10-damage follow-up, pause ignores letters and retains
queue, two parries retain40HP, enemy180->170, valid sorting, zero exceptions
and scoreboard writes. Menu, guide and success entry screenshots inspected.
This is technical/static evidence; continuous perceived motion/force and a full
performance matrix remain unverified. Previous e17 evidence remains separate.
Downloaded32 player files/48,026,728bytes; every SHA256 matched the immutable
build. Reopened user page e98327a8-914c-45c1-8810-75f3e56380f7 at
http://127.0.0.1:8136/; loading finished, no warning, actual menu observed.
Owned private server workspace/results were removed after verifying task marker,
snapshot and no active Docker mounts. Shared Library/cache/source/home preserved.
