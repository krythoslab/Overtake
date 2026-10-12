# Risk Register (full) — design doc

Extends MASTER_BLUEPRINT §17 (top 19 shown there). Severity Blocker/High/Medium/Low; every risk has mitigation + detection signal.

| ID | Risk | Sev | Detection signal | Mitigation / trigger |
|---|---|---|---|---|
| R-01 | EA/F1 license dynamics beyond 2027 unknown; license acquisition could change market dynamics | High | FOM/Codemasters announcements | Track A default; Track B gated as business track; brand hygiene |
| R-02 | Reference-data conditionality (one 2026 real-world debut event subject to FIA homologation) informs archetype design | Low | FIA homologation decisions | Conditional-round flag in reference data; content packs flexible |
| R-03 | 2028 **calendar and non-PU regulations** unknown; the 2028 power-unit step is ratified (WMSC 2026-06-23) | Medium | WMSC/FIA publications; profile `verifiedOn` stamps | PU step carried as ratified profile data; other 2028 slots kept as explicit `UNKNOWN` markers (never silently inherited); no public 2028 promises (store page rule §15) |
| R-04 | 2026 deploy/harvest constants now primary-verified against Section C Iss 20 article text (C5.2.8/C5.2.10/C5.2.11); **2027/2028 consolidated constants** (2027 Section C Iss 03, 2026-10-01) still need a line-by-line cross-check | Medium | AT-R1 regression failure on a profile diff | U-03/U-11; profile data fields + AT-R1…AT-R8 conformance tests |
| R-05 | Store size/cellular limits drift | Medium | Store policy updates | Streaming architecture; re-verify at submission (U-06) |
| R-06 | EU Apple 12-month payment lock-in | Medium | Policy calendar | Store billing at launch; revisit post-launch |
| R-07 | Korea PC/Steam GRAC obligation (CONFIRMED; cost/lead-time open) | Medium | Greenlight decision | Local-agent budget vs region-gate; decision recorded pre-greenlight |
| R-08 | China ISBN + local publisher required | Low | Policy unchanged | Geo-block CN at launch |
| R-09 | UGC/mod IP risk | Medium | Reports; takedown notices | Livery-only UGC, moderation SLA ≤24 h, hashing, no asset imports |
| R-10 | Cross-platform determinism slippage | High | Golden CI divergence | §4.2 numerics discipline + build pinning + CI gate (kill-criterion M1) |
| R-11 | Unity 6.3 LTS EOL Dec 2027 + mobile maturity | Medium | Support calendar; beta telemetry | M3 upgrade decision; security-patch fallback; engine spike in M2 |
| R-12 | Mobile 60 fps infeasible on min tier | Medium | Device-lab profiling | Tiering + adaptive + 30 fps fallback modes |
| R-13 | AI/balance quality below genre bar | High | Playtest KPIs; AI speed distribution | §6 validation harness; difficulty telemetry |
| R-14 | No-loot-box revenue model under-monetized | Medium | Revenue telemetry | Packs + cosmetics; regional legal re-check |
| R-15 | 22-car netcode complexity | High | Desync-free <99.9% soak failures | §10.1 model; exploit drills; M2 pilot |
| R-16 | SDR cross-platform eligibility partner-gated | Medium | Valve response (M2 gate) | Self-hosted relay fallback budgeted |
| R-17 | Korea GRAC agent cost/lead-time (cost-side of R-07) | Medium | Agent quotes | Greenlight budget line |
| R-18 | Mobile premium model underperforms | High | M3 beta gates (conversion/D7/refunds) | Premium-lite fallback, locked at M3 |
| R-19 | Account-deletion/DSAR certification risk | Medium | Store review feedback | §14.4 path + M3 gate |
| R-20 | Trademark drift in live content/metadata | Medium | Quarterly legal scan | §tokens scan CI + per-update gate |
| R-21 | Launch competitive set shift (2028 landscape) | Medium | M3 competitive review | Differentiation + price re-test at M3 |
| R-22 | Live-ops incident cost / availability | Medium | p1 incidents; availability <99.9% | On-call rota; autoscale; relay fronting |
| R-23 | Regulation drift between ratified announcements and consolidated regulation text (2027/2028 power-unit package; 2026 mid-season designated-zone refinement) | Medium | Full-text cross-check at M1; AT-R1…AT-R8 conformance failure | Profile carries `sourceIssues[]` + confidence + `verifiedOn`; U-03/U-04/U-11; corrections are content updates, never engine changes |

**Regulation evidence note (2026-10-12)**: every load-bearing regulation claim in `MASTER_BLUEPRINT.md` §5.1/§5.4 now cites a direct URL in `docs/10-research/SOURCES.md` that was fetched and returned HTTP 200. The 2026 values are quoted from FIA Section C Iss 20 article text; the 2027/2028 values come from the WMSC-ratified (2026-06-23) package table with the consolidated 2027 text published as Section C Iss 03 (2026-10-01). Sources marked `not re-verified` are REPORTED-only and must be re-sourced before driving a default.

Kill criteria (M1): determinism gate fails, kernel >2.0 ms/tick unresolved, feel-test KPIs below floor, or brand/legal blocker.
