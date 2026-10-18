---
name: typinghero-game-loop
description: Implement and verify TypingHero gameplay, balance, typing, combat, scoring, and performance changes using Unity offline tests and WebGL browser profiles on fly-server.
---

Confirm the Git root contains this skill, `Assets/Scripts/Core/TypingManager/TypingManager.cs`,
and Unity 2022.3.25f1 before using the runner. This is a repository-local skill;
do not install it globally or apply it to another game.

Read the affected parts of [gameplay](../../../docs/game/gameplay.md) and
[system map](../../../docs/game/systems.md). Establish observable acceptance
from the request and existing assets; distinguish a new requirement from current
behavior. Implement a small representative path, run the relevant checks, fix
failures using evidence, then extend the change.

Use [verification](../../../docs/game/verification.md) for commands, credentials,
isolation, profile coverage, and performance baseline. Before testing, identify the
changed behavior and affected dependent flows; choose the smallest checks that
prove acceptance. Do not default to full `quick`, full `webgl`, or multiple
viewports. Use browser checks when acceptance needs real input/integration;
add viewports only for affected layout, camera, or responsive behavior. Use
`doctor` when runner/environment verification is needed, and `setup` only when
necessary. Follow the impact-based selection guidance in verification, including
runner filtering limits; do not run a full profile merely because it is convenient.
Expand coverage only for affected shared logic, a relevant failure, a concrete
remaining concern, or an explicit request; briefly state the reason. Reuse passing
results when relevant source, config, dependencies, and environment are unchanged.
Stop when acceptance and relevant checks pass with no concrete unresolved concern.
Run heavy `stress`/`soak` only when explicitly
requested or when a relevant pooling/performance/lifecycle issue requires that
evidence, respecting the user's current test scope. A normal build verifies QA
bridge exclusion when that check is in scope. Distinguish out-of-scope profiles
from unverified acceptance; omitted profiles are not automatic follow-up work.
Do not treat static validation, a live process, or zero executed tests as gameplay
evidence.

Keep input behavior in `TypingManager.ProcessInput`; real input and offline tests
must use that same path. Browser play uses keyboard/mouse only. The QA bridge may
read state, never mutate gameplay; compile it only with `TYPINGHERO_AUTOMATION`.
Keep Firebase fixture interception and external network blocking enabled, including
score writes. Never copy credentials into source or snapshots.

After a change, reconcile the affected memory sections with source and actual
results. Record environment and uncertainty for measurements. Resume long work
from its existing `docs/plans/` checkpoint; do not create another memory/log system.
