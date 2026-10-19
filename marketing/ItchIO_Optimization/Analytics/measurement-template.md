# Day 0 / 7 / 14 measurement worksheet

Duplicate this table for the actual campaign; preserve raw values and screenshots privately if desired. Do not save account/customer data in the repository. `null` means unavailable, not zero.

| Measure | Day 0 | Day 7 | Day 14 | Definition / source |
| --- | --- | --- | --- | --- |
| Captured at / timezone | | | | Use the same timezone |
| Exact selected window | | | | Fixed calendar boundaries, no overlap for week comparisons |
| Header Views total | | | | Record whether total or windowed; do not assume |
| Header Browser Plays total | | | | Starts, not unique players |
| Window Views | | | | Only if dashboard provides that window |
| Window Browser Plays | | | | Same window as views |
| Known QA page loads / live starts | | | | Log actual test actions; contribution to itch counts may remain unknown |
| External referrers by domain | | | | Keep self-referrals separate |
| Known directly shared/invited traffic | | | | Only where verifiable; not all Facebook is personally invited |
| Unknown/direct/no-referrer | | | | Do not relabel as organic |
| Clip impressions or feed exposure | | | | Name platform's exact metric; not itch impressions |
| Clip views | | | | Platform views are not landing-page visits |
| Link clicks | | | | Only when destination/platform actually reports them |
| Ratings / collections / comments | | | | Voluntary; no solicited positive ratings |
| First-duel feedback | | | | Used/skipped tutorial, confusion timestamp, browser |
| Retention / completed runs | null | | | Unknown without real observation/instrumentation |

For monotonic totals with unchanged definitions: delta V = V_end - V_start; delta B = B_end - B_start. Compare equal-duration windows and allow for reporting delay. A display correction/reset invalidates simple subtraction. Separate QA actions, but do not subtract guessed counts that the analytics may already exclude.

A **page-to-start proxy** is delta B / delta V only if periods, scope and definitions align and delta V > 0. It is not a unique-user conversion rate. Keep numerator/denominator beside every percentage. The observed 45/211 should not be presented as proven matched-cohort conversion.

CTR = clicks / impressions only on a surface where both metrics actually exist for the same placement and period. The accessible itch dashboard has no impression denominator, so itch search CTR stays null. A Short view-to-visit ratio is a different metric; mixed referrers cannot establish per-clip attribution. Keep a post log with exact date, channel and URL; UTM-style query parameters are optional labels only, not guaranteed itch attribution.

Do not use self-generated visits/plays to hit the learning target. No new analytics dependency or tracking endpoint was added. Obtain consent for any future new tracking design separately from this marketing task.
