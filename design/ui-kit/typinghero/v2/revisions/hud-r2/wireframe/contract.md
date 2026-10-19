# HUD r2 ? draft review contract

Source: user-supplied HUD r2 implementation plan, 2026-10-19; hud-r1/source/build.py for palette, glyphs, sockets, typing and actors. This is a review artifact, not a product layout approval. Plan authorizes building the draft and all specified geometry; approval stays draft as explicitly requested.

All exact coordinates are in source pixels, 320?180. Structural wireframe uses 3? labeled boxes because pinned flex renderer does not support absolute positioning; boxes indicate grouping, not final geometry. The final review follows these exact rects: HP [3,11,102,21] / [215,11,102,21], secondary [3,34,102,13] / [215,34,102,13], score [111,3,98,22], typing [81,146,158,30]. Combo top y28, round/status top y40. HP fill relative [20,7,77,6]; secondary fill [14,4,84,4]. Names and HP remain above HP. Parry/Space/timer/pause retain r1 coordinates.

Keep silver HP and typing, dark score and stamina under Slayer. Only left combo bar, combo/multiplier and SLAYER / x16 become gold. Normal heart; elite two isolated solid chevrons and two 2px rises; boss Captain silhouette with dark margin and three 3px rises. No track/icon overlap.

Far, mid walls, floor and edge foreground are separate alpha layers. Actor feet y138. Perspective floor y112?145, vanishing point [160,80]. Camera default stopped; integer sinusoid offsets per 4000ms cycle: far ?1, mid/actors ?2, floor/foreground ?4. HUD fixed. Wrong input resets progress, red 180ms, whole typing group horizontal ?1px up to 120ms, repeated error restarts. Reduced motion (system OR checkbox) disables camera and shake, keeps color feedback.

Only content slots vary: rank, Slayer, HP 0/1/50/100%, stress and grayscale. Gallery controls outside HUD; responsive page reflows controls, source viewport stays 320?180 at integer sizes 1/2/4 and fits narrow viewports. Ten screens ? 1/2/4 nearest neighbor. Components authored in Python; SVG exact pixel runs; atlas rects and independent frame borders owned by metadata.json. No gameplay, audio, API, camera-game or Unity integration.
