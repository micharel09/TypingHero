# Host resource quotas and cleanup timer

## Goal and acceptance

- New ClackBlade build and QA containers, the native Ubuntu smoke scope, and new
  mobile runner jobs have no artificial CPU or memory quotas.
- Existing active jobs are left running without restart or replacement.
- Daily cleanup runs at 12:30 Asia/Ho_Chi_Minh and catches up once after the
  host has been off; the cleanup script and its allowlist stay unchanged.
- Preserve existing network, credential, process-cleanup, disk-preflight,
  timeout, and service sandbox protections.

## Decisions

- Remove only `--cpus`/`--memory`, systemd CPU/memory quota properties, and the
  smoke runner's low CPU/I/O priorities. Keep browser shared memory at 1 GiB.
- Move the cron schedule to a persistent systemd timer using the existing
  `host-storage-monitor.timer` pattern.
- Apply mobile runner runtime limits only after confirming no `Runner.Worker`
  is active; do not restart its listener.

## Checkpoint

- Canonical repository: `micharel09/TypingHero`, base `origin/main` at
  `652ad93b26dc65ade1c91d22b4e2ee68e7314e24`.
- Implementation worktree: `E:/Workspaces/ClackBlade/host-unlimited-resources`,
  branch `codex/host-unlimited-resources`; the original checkout's unrelated
  dirty changes were preserved.
- Ubuntu host `quanubutu`: cron cleanup exists, systemd cleanup units do not;
  mobile runner has `CPUQuota=200%` and `MemoryMax=6G`; no active
  `Runner.Worker` or smoke session was observed.
- Repository quotas were removed from toolchain, Unity and browser containers;
  browser shared memory remains 1 GiB. Activation wrapper still pins hostname
  and MAC, and has no compute quota injection. Historical measurements remain
  labeled with their former limits.
- Ubuntu smoke runner keeps its lock, scope cleanup and 600-second deadline,
  with no CPU/memory/swap quotas or low CPU/I/O priorities. It resolves cgroup
  limits through ancestors and rejects any finite CPU, memory or swap limit.
- Mobile runner unit has no persistent CPU/RAM caps. Runtime values are infinity;
  PID and invocation ID stayed unchanged during the update. Cleanup timer is
  active with `Persistent=yes`, next at 2026-11-04 05:30 UTC; the exact old cron
  file is removed. Cleanup script hash remains
  `94f30d6a7bccb96b54e121e3d5f9fb697ee3371b38181d13fc228571965328f4`.
- Ubuntu evidence: 10/10 ClackBlade runner contract tests and 6/6 smoke-scope
  tests pass. An uncapped disposable Docker probe reports `cpu.max=max`,
  `memory.max=max`, and `nproc=4`. `systemd-analyze verify` and the Vietnam-time
  calendar check pass; verify emitted two unrelated legacy `CPUAccounting=`
  warnings for XFS scrub units. No runner worker or smoke session is active.
- Resolved failed attempts: an SSH one-liner lost quoting, an early README patch
  did not match and wrote nothing, and one test run exposed a missing `json`
  import. A first scope test also showed that `cpu.max` is inherited from a
  parent when absent in the leaf; the reader now checks the whole visible
  hierarchy. All affected commands/checks were corrected and rerun successfully.
- Final source review and `git diff --check` pass; only the four intended
  repository files and this task checkpoint are changed.
- Commit `bf36c8424273363979337dfef3248dfe080a78ee` is verified on both
  `origin/main` and the remote task branch after a fresh fetch; it is an
  ancestor of `origin/main`.
- Registered Ubuntu task temp completed as `deleted / verified-absent`. The
  task worktree and local branch are removed after this integration; the remote
  task branch is retained per repository instructions.

## Status

Implementation and rollout are complete. No active jobs were stopped or
recreated. The dirty parent checkout remains unchanged.
