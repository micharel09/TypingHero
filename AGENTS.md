# CLACKBLADE

## Project rule scope

This is a standalone personal Unity game project. Do not apply the global
working agreement at `E:/codex-data/AGENTS.md` or its supplementary chapters
to this repository. This project-specific exception takes precedence over that
global agreement, including when it is supplied as inherited context.
Use this file, applicable project instructions, and the user's task requirements.
External skills explicitly referenced by this file remain applicable; do not
automatically import global web/mobile workflows or design standards.
System and developer instructions still apply.

## Git workflow

- For a new task that changes files, verify the repository and remote, fetch
  `origin`, and create a task branch in an independent worktree from the latest
  `origin/main`. Reuse that branch/worktree when continuing the same task.
  The destination is `micharel09/TypingHero`, primary branch `main`; explicit
  task-specific instructions take precedence.
- In Orca, use the `orca-cli` skill to manage worktrees. Do not launch additional
  agents or terminals unless the task requires and authorizes them.
- Preserve existing checkouts, indexes, and uncommitted changes. Do not
  automatically stash/reset them or include unrelated changes in task commits.
- Complete scoped checks, commit task changes, and push the task branch. Fetch
  again before merging; merge newer `origin/main` into the task branch, resolve
  conflicts using both sides' intended behavior, and recheck affected behavior.
  Never select `ours`/`theirs` indiscriminately. Ask only when evidence cannot
  resolve a necessary product decision.
- For direct integration, push the verified task HEAD to `origin/main` as a
  fast-forward, without checking out or modifying a dirty local `main`. If the
  remote advances and rejects the push, fetch, integrate, recheck, and retry.
  Stop and report if the same blocker recurs three times without progress.
- If protection requires a PR, create and merge it using the permitted mechanism
  after mandatory checks/reviews pass. Never bypass protection or force-push main.
- Verify remote integration: for normal merges, confirm task commits are ancestors
  of `origin/main`; for squash merges, verify the PR merged into `main`, its result
  commit on the remote, and the corresponding task changes.
- Commit, push, and merge task changes are authorized by default in this repo;
  do not ask again per task. Report completion only after remote verification;
  otherwise distinguish implementation status from the blocked merge and cause.
- After verified integration, remove only clean task-created worktrees and local
  branches whose results are preserved on the remote. Do not delete pre-existing
  resources or remote branches automatically. For resources retained, report the
  path/branch, reason, and cleanup condition.

## Testing scope

Test only the changed behavior and directly affected dependent flows. Choose the
smallest checks that establish the requested result; do not default to full test
suites, full profiles, multiple viewports, WebGL builds, or stress/soak runs.
Expand coverage only for a relevant failure, concrete evidence of wider impact,
or an explicit user request. Briefly state the reason before running broader checks.
Reuse passing results when the relevant source, configuration, dependencies, and
environment have not changed. Stop once the requested behavior and necessary
checks pass with no concrete unresolved concern.

For documentation or rule changes, review the content and diff and run
`git diff --check` for the affected files; do not run game or runtime tests.
This section governs test selection when applying project skills. Preserve the
sprite frame and transition review requirements below.

## Game reference research

Research the full feature, scene, or animation being referenced, rather than the
entire game unless requested. Before evaluation, list the required scenes,
states, behaviors, and variants from the task; use this checklist to measure
coverage, not an unbounded claim to have found every image online.

- Find and directly inspect images covering every checklist item. Also find
  videos showing relevant behavior/motion; images alone are insufficient.
- Before frame review, record each video source, start/end timestamps, and why
  the segment was chosen. Include the complete behavior, preparation, execution,
  ending, and relevant state transitions. Review the whole video when requested.
- Inspect 100% of frames and consecutive transitions in each selected segment,
  preserving original timing and held/repeated frames. Do not sample or inspect
  only key poses. Do not narrow segments to hide errors or reduce review work.
- Descriptions, reviews, thumbnails, external summaries, ordinary playback, and
  script-generated counts do not replace direct visual inspection.
- In the task's existing evidence record, record sources, scope/checklist coverage,
  inspected images, timestamps, total frame/transition counts, and counts actually
  inspected. Keep reference-video research distinct from output-sprite review.
- Missing sources/images or uninspected frames leave the affected comparison
  **unverified**. Identify gaps; do not claim complete research or a reference
  match until coverage is complete.

## Game workflows

For gameplay, balance, typing, combat, scoring, or performance work, read and apply
[typinghero-game-loop](.agents/skills/typinghero-game-loop/SKILL.md).
Use the source-backed memory in [docs/game](docs/game/gameplay.md); update only
the affected sections after verified changes. Long tasks use `docs/plans/`.

For creating, editing, or reviewing any sprite animation (idle, movement, attacks,
hit reactions, death), read and apply
[sprite-frame-review](.agents/skills/sprite-frame-review/SKILL.md).
Inspect every frame and every consecutive transition against the original request
and reference; record per-frame evidence for the final asset before claiming pass.
Playback, selected key poses, and technical checks do not replace this review.
This route excludes UI motion and color-only edits to static sprites.

Unity version and package versions are owned by `ProjectSettings/ProjectVersion.txt`
and `Packages/manifest.json`. Preserve asset GUIDs and serialized references.
