# HUD r4 — draft review

Source: user HUD r4 plan, 19 October 2026; reuse r3 palette, scene, actor feet138, Parry/timer, ranks and camera. Review only; no Unity integration or approval claim.

Fixed source canvas320×180. Typing198×30 at61,146, border4; current starts65,151. Current/next/third VT32316, gap8, colors ivory/muted/metal. Correct letters jade in place; error resets current and holds queue, red180ms, repeated error restarts. No motion during character progress.

Queue updates synchronously. On completion new row starts at old next position and slides left100ms ease-out cubic; input remains live. Another completion finishes prior transition before starting the next. Multiword event paints final state only. Reduced motion snaps. Current remains complete; previews clip at right for out-of-pool stress. Current pool219 words: longest54px, worst triple178px within190px interior. Never change pool or font to fit.

Gallery offers real pool, normal/stress and r3 geometry with shared input. PNG/SVG/atlas/metadata and1×/2×/4× exports use nearest-neighbor. Wireframe is grouping at3×; exact geometry above is owned by Pillow/gallery, not wireframe renderer. Bundle remains draft.
