# Leaderboard top 1000

## Outcome

The published itch.io WebGL game fetches up to 1000 leaderboard entries and lets the player scroll beyond the initial visible rows. Keep the existing panel layout.

## Acceptance

- `SampleScene` requests 1000 entries; show the actual number returned, without synthetic players.
- Mouse wheel or dragging inside the list reaches ranks below 9.
- An automatic refresh after 8 seconds does not reset the list to the top.
- Cloud WebGL build succeeds; upload the verified ZIP to `https://micharel09.itch.io/typing-hero` and verify the published interaction.

## Findings and checkpoint

- The first release changed the fetch count only. Its menu loaded, but scrolling was not verified.
- Production read-only query returned 65 entries on 2026-10-16.
- Reproduced on the published build: entered the screenshot's player name `Asd`, immediately paused with Escape, and sent a native mouse wheel event over the leaderboard at a 400x700 desktop viewport. Ranks remained 1–9. Gameplay stayed paused; no combat was performed. The bounded capture contained no PUT/PATCH requests (iframe/worker coverage is limited).
- The leaderboard's nested canvas lacked `GraphicRaycaster`; its ScrollView background Image was disabled. Every refresh also explicitly reset the scroll position.
- Commit `4279a01` adds the canvas raycaster, enables a transparent ScrollView hit target, and resets to the top only on the first non-empty population.
- `git diff --check` passed; serialized scene object IDs are unique. These checks do not prove runtime interaction.
- Build run: `https://github.com/micharel09/TypingHero/actions/runs/38838079060` succeeded. ZIP root is `build/WebGL/WebGL/`; downloaded artifact integrity passed and `index.html`, data and WASM files are present.
- Published `TypingHero-WebGL-top1000-scroll.zip` to the existing public itch.io project, selected it as the browser-playable file, removed the previous upload, and saved. The preceding ZIP remains in Downloads for rollback.
- Live verification on Edge instance `61a46c05`: used the same screenshot name `Asd`, immediately paused gameplay, and sent native mouse-wheel events over the list at a 400x600 desktop viewport. The view advanced from ranks 1–9 to the final ranks 59–67 (two new records appeared while work was in progress). An earlier attempt at 400x700 hit below the list and correctly caused no list movement.
- After waiting 11 seconds, the final ranks remained visible. Captured top-1000 refresh requests continued returning HTTP 200. The bounded network captures contained no PUT/PATCH requests; iframe/worker and retained console/operation coverage is limited. No combat was performed.
- Latest ZIP: `C:/Users/ADMIN/Downloads/TypingHero-WebGL-top1000-scroll.zip`. Play URL: `https://micharel09.itch.io/typing-hero`.
- Browser sessions ended and emulation was cleared. Temporary screenshots and captures were removed after recording these results. Acceptance checks passed; no work remains.
