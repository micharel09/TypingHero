# Evidence-based diagnosis - 2026-10-19

Inspected the actual public page, authenticated owner editor and dashboard through the existing browser profile. Public page ID 3857509 belongs to micharel09. No credentials, customer records or production leaderboard writes were collected.

## Three different questions

| Funnel stage | Evidence | What it establishes |
| --- | --- | --- |
| Exposure / discovery | Game appears in an exact-name itch search. Absent from 54 loaded project cards for `typing` and 36 first-loaded cards in HTML5 typing browse. | Search indexing works; broad-keyword presence was weak in these snapshots. Not proof of zero impressions or permanent rank. |
| Impression to click | No impressions or CTR visible in the accessible dashboard. | Low CTR is unverified. Neither Views nor Browser Plays supplies an impression denominator. |
| Page to play | Dashboard header: 211 Views, 45 Browser Plays. | Real recorded activity, but not unique users, completed runs or retention. Header totals and 30-day referrers are not established as the same cohort. |

The 30-day referrer window shown was September 20 through October 19. Facebook-domain rows sum to 28 (19 l.facebook.com, 5 m.facebook.com, 2 facebook.com, 1 www.facebook.com, 1 lm.facebook.com). A separate self-referral URL with fbclid contributes 1: 29 across all displayed rows, **not 29 external Facebook visits**. No additional external organic source was shown. Missing referrers cannot be classified as invited or organic automatically.

Working hypothesis: the project lacks a repeatable stream of relevant strangers. This is better supported than a claim that the thumbnail is the main bottleneck. Invitation-driven traffic is the developer's report; analytics alone does not identify relationships.

## Specific presentation problems and implemented fixes

| Finding before | Exact action | Status |
| --- | --- | --- |
| Multiple projects called Typing Hero, including Hickery's prototype and an educational/source-code product | Added descriptive subtitle `Type to Slash`; preserved recognizable title and slug | Published; new title visible in exact-name search |
| Old cover did not expose a clear word-to-sword sequence at small size | Cover A uses actual BLADE sword-hit frame with letter keys and Space instruction. B uses actual parry stance/cue | A published; B exported alternative; both checked at 315x250 |
| Old gallery/inline GIF represented another HUD/visual snapshot | Replaced gallery with current production captures and a genuine BLADE-to-hit GIF; GIF also placed directly after opening CTA | Published; all five inline/gallery image elements loaded |
| First impression needed stronger hierarchy around attack, parry and stamina | Rewrote opening, mechanics, keyboard requirement and Run game CTA; retained credits | Published and text verified |
| Generic Indie tag consumed a slot while score competition is real | Replaced Indie with native High Score; retained eight accurate tags | Published; no ranking guarantee |
| Session classified as A few seconds | Changed to A few minutes, an expected session category rather than measured retention | Saved and public metadata verified |
| Tutorial exists but accessibility field empty | Selected Interactive tutorial only | Saved and public metadata verified |
| AI disclosure previously No; new store copy is AI-assisted text | Selected Yes / Text & Dialog; final art remains actual gameplay pixels | Public metadata now AI Assisted, Text; no unsupported art/audio/code provenance claim |
| Public, HTML, Action, Keyboard, English already correct | Retained existing values, $0/donate and current playable ZIP | No evidence of a visibility or browser eligibility blocker |

The current build starts from an empty player-name menu; no confirmed launch blocker was established. Do not advertise prior Guest auto-name or Endless work. No gameplay redesign was needed or performed. Automated parry capture misses do not establish a game defect or explain traffic.

## Competition actually examined

- [Type Knight](https://chaikadev.itch.io/type-knight): paid typing/knight combat with strong visual identity. Its creator also describes parrying; typing plus parry is not a defensible world-first claim.
- [WPM Dungeon](https://clementine-labs-gaming.itch.io/wpm-dungeon): browser typing combat, word-length choices and typing report. Typing Hero should lead with watching an enemy's timing, breaking stamina and Slayer, rather than generic typing practice.
- [Epistory](https://plug-in-digital.itch.io/epistory): paid typing adventure with broader exploration positioning. Typing Hero's immediate free browser duel is a different invitation, not a promise to match its scope.

These public presentations give positioning references, not competitor CTR or proof of causal growth.

## Eligibility and next exact fixes

[Getting indexed](https://itch.io/docs/creators/getting-indexed) distinguishes public access from search eligibility. Here exact-name search positively confirms current indexing. Do not repost a duplicate project or repeatedly reset releases to chase visibility.

[Creator quality guidance](https://itch.io/docs/creators/quality-guidelines) supports accurate metadata and gameplay media. Suggested tags are classification, not a ranking guarantee. Retain the nine tags in `StorePage/configuration.json`; add no unrelated trend tags, false mobile support or unverified accessibility claims.

[HTML5 documentation](https://itch.io/docs/creators/html5) describes browser packaging. The existing Run game points to upload 19865461 and the downloaded production binary boots offline. Preserve this embed until a separately verified release is authorized.

Trailer URL stays empty until a real YouTube/Vimeo upload exists. Social drafts require manual posting under each community's current rules. The useful next distribution test is one real typing clip and a narrowly framed first-play question, measured against a fresh day-0 baseline. No analytics evidence currently warrants another thumbnail redesign.
