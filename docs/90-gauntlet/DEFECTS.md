# Defect Log (GAUNTLET)

Populated from adversarial reviews. Severity: CRITICAL (blocks completion), HIGH (must fix before gate), MEDIUM (fix or accept w/ risk note), LOW (nitpick).

## Iteration 1 findings (3 independent reviewers, 2026-10-11)

| ID | Sev | Found by | Iteration | Defect | Root cause | Correction | Status |
|----|-----|----------|-----------|--------|------------|------------|--------|
| D-01 | CRIT | Reviewer 1 | 1 | Q3 stated as 12 min; ratified 2026 text is **13 min** | Unverified "standard continuation" value promoted to fact | §7.1 now Q3 13′ with re-verify note; R1 updated | FIXED |
| D-02 | CRIT | Reviewer 1 | 1 | Normal ERS deploy curve stated as single line 1800−5v→345; ratified is piecewise (1800−5v <340; **6900−20v** 340–345; 0 ≥345) | Single obscure secondary (Kwyper first pass) trusted over FIA text structure | §5.1 table + R1 now piecewise with C5.2.7/C5.2.8 (Iss 20, via S-41) | FIXED |
| D-03 | CRIT | Reviewer 2 | 1 | Cross-platform bit-determinism asserted without codegen controls; IL2CPP/FMA divergence unaddressed | Determinism treated as declaration, not spec | §4.2 rewritten: fixed-point int64 kernel math, no Burst for sim, no FP contraction, golden CI gate; determinism scope bounded (server canonical; same-arch checksums only) | FIXED |
| D-04 | CRIT | Reviewer 2 | 1 | Snapshot-checksum auto-reconnect incoherent across ARM/x86 client matrix; no snapshot size budget | Feature built on unproven premise, no failure analysis | §4.2 scope + §10.1: same-arch checksum diagnostics only, ≤12 KB snapshot budget (ASSUMPTION), full-state resync path, reconnect backoff/60 s grace | FIXED |
| D-05 | CRIT | Reviewer 2 | 1 | Leaderboard replay verification unspecified; checksum forgeable; no server re-sim, attestation, or input-delay clamp | Anti-cheat enumerated without mechanism | §10.5 rewritten: server re-simulation protocol, input-delay bounds (8-tick drop), Play Integrity/DeviceCheck attestation, "unverified" boards until protocol ships | FIXED |
| D-06 | CRIT | Reviewer 2 | 1 | Prediction scope contradicts server-owned ledger (energy/tyre/rules/collisions) — reconciliation undefined | No split between predictable core and authoritative state | §5.2 two-layer split (chassis core predictable / authoritative ledger); §10.1 updated | FIXED |
| D-07 | CRIT | Reviewer 3 | 1 | Track pipeline input "survey-lidar/satellite import" contradicts "never traced from real circuits" | Provenance policy written downstream of tool design | §8.2: original-content procedural pipeline, no real-circuit import, per-track provenance log | FIXED |
| D-08 | CRIT | Reviewer 3 | 1 | §8.3 calendar uses real circuit/city names (Melbourne, Madrid, Portimão…) contradicting §14.1/14.2 brand hygiene | Content roadmap drafted before naming gate | §8.3 rewritten to fictional rounds + archetype structure; real calendar confined to R1 dossier with "reference only" banner | FIXED |
| D-09 | HIGH | Reviewer 2 | 1 | No car-vs-car contact model in kernel; 22-car contact golden replays impossible as specified | Collision module enumerated track-only | §5.2: swept-sphere contact model, deterministic ordering, fixed solver iterations, no contact caching, soak test | FIXED |
| D-10 | HIGH | Reviewer 2 | 1 | Netcode essentials absent (clock sync, jitter buffer, bandwidth budget, relay strategy, DDoS, cost); SDR cross-platform partner-gate | Decision D-5 made without wire/backend spec | §10.1: clock sync/tick protocol, uplink/downlink budgets (ASSUMPTION), relay-first transport with SDR eligibility gate (M2) + self-hosted fallback (R-16) | FIXED |
| D-11 | HIGH | Reviewer 2 | 1 | No reconnection/rejoin design despite mobile backgrounding; incident evidence UX missing | Mobile OS realities not carried into netcode | §10.1: 60 s grace, AI takeover, catch-up replay, invulnerability window, server-side 15 s ring buffer + incident-replay evidence view | FIXED |
| D-12 | HIGH | Reviewer 2 | 1 | Kernel ≤2.0 ms/tick unvalidated and in tension with no-SIMD determinism; no server CPU budget | Budget asserted without method | §11.2: M1 kill-criterion + measurement protocol; §10.2/§18: server ≤6 ms/lobby/tick (ASSUMPTION, M2 measure) | FIXED |
| D-13 | HIGH | Reviewer 2 | 1 | Exec summary claims 60 fps on Steam Deck contradicting own 30 fps Verified gate | Exec target not reconciled with platform gates | §1 rewritten: Deck Verified-class; 60 fps recommended PC/mobile | FIXED |
| D-14 | HIGH | Reviewer 3 | 1 | No revenue model vs $8–14M budget; no marketing line | Budgeting as staffing arithmetic | §13.1b revenue sketch + scenarios; §18 marketing $1.5–3.0M; M3 gates | FIXED |
| D-15 | HIGH | Reviewer 3 | 1 | Korea PC/Steam GRAC obligation treated as UNKNOWN though regulator FAQ resolves it | Over-cautious unknown labeling | §14.3/Korea: CONFIRMED obligation via local agent (×1.5 fee); R-17 + U-05 = cost/lead-time unknowns only; greenlight decision required | FIXED |
| D-16 | HIGH | Reviewer 3 | 1 | Privacy frame omits Korea PIPA, Brazil LGPD + ECA Digital age assurance (target markets) | US/EU-centric compliance | §14.4 rewritten with PIPA/LGPD/ECA Digital | FIXED |
| D-17 | HIGH | Reviewer 3 | 1 | Mobile premium pricing has no viability evidence or fallback | Platform-port thinking, no mobile business case | §13.1: conversion/retention gates + premium-lite fallback at M3 (R-18) | FIXED |
| D-18 | HIGH | Reviewer 1 | 1 | Regulation currency check stale: Section B Iss 09 (2026-10-01) supersedes Iss 08; 3-h race cap abolished | Currency frozen at first research pass | §2 + S-02b + U-04: Iss 09 baseline, full-text verification at M1 | FIXED |
| D-19 | HIGH | Reviewer 1 | 1 | Missing race distance/duration rules (305 km / Monaco 260 km / hard stop) — classification % baseless | Session rules incomplete | §7.1 + R1: distance/hard-stop added | FIXED |
| D-20 | HIGH | Reviewer 1 | 1 | Q1/Q2 elimination counts never stated | Only SQ documented | §7.1: eliminate slowest six in Q1 and Q2 (22-car basis) | FIXED |
| D-21 | HIGH | Reviewer 1 | 1 | Tyre allocation/compound-mandate rules absent though strategy depends on them | B6 family omitted | §7.1: allocation sets + ≥2 dry compounds (profile flags) | FIXED |
| D-22 | HIGH | Reviewer 3 | 1 | Track B described as free "content swap" ignoring licensed circuit geometry/likeness/licensor QA | Contingency treated as engineering artifact | §14.1 Track B reframed: business-track gates; engine/rules layer carries over only | FIXED |
| D-23 | HIGH | Reviewer 3 | 1 | §14.2 legal conclusion ("mechanics from rules are not protected expression") unsourced | Legal opinion smuggled into design | Downgraded to ASSUMPTION + counsel FTO pre-greenlight | FIXED |
| D-24 | MED | Reviewer 1 | 1 | +0.5 MJ Overtake harvest labeled CONFIRMED but sourced via secondary | Tier inflation | Downgraded to REPORTED (article confirmed) in §5.4/R1 | FIXED |
| D-25 | MED | Reviewer 1 | 1 | R1 §3 heading claims CONFIRMED while containing REPORTED items; Istanbul asterisk unexplained; issue dates dangling | Section label discipline | R1 §3/§4 fixed: explicit REPORTED markings, asterisk explained, issue dates in §4 | FIXED |
| D-26 | MED | Reviewer 1 | 1 | Red-flag procedures vague (grid re-forming, unlapping, pit closure) and not flagged simplified | Simplifications not marked | §7.3: explicit simplified flags + profile parameters | FIXED |
| D-27 | MED | Reviewer 2 | 1 | Input sampled at render rate breaks replay determinism | Input path coupled to presentation | §11.3: input sampled/quantized at kernel tick with fixed timestamps | FIXED |
| D-28 | MED | Reviewer 2 | 1 | No frame-pacing/loading metrics; "90 min battery" misattributed to Valve | Metrics absent; citation error | §11.3: p99 frame time, sustained-fps definition, load budgets, internal battery target with correct attribution | FIXED |
| D-29 | MED | Reviewer 2 | 1 | 2.5 GB mobile peak memory vs iOS A13/4 GB min tier; no jetsam policy | Memory ceiling ignored | §11.2: per-tier ceilings (iOS min ≤1.1 GB), memory-pressure tests in lab | FIXED |
| D-30 | MED | Reviewer 2 | 1 | iOS thermal/on-tile post-processing sourced to 6000.5 stream while pinning 6.3 LTS | Feature availability unverified on pin | §4.1 note + M1 verification; upgrade plan accommodates | FIXED |
| D-31 | MED | Reviewer 2 | 1 | Launch scope/netcode sequencing (first integration M3, ~4 months soak) too late; no bot backfill; desync metric undefined | Sequencing without acceptance metrics | §18: netcode pilot moved to M2; §16.2: desync/soak metrics; §10.1 bot backfill | FIXED |
| D-32 | MED | Reviewer 2 | 1 | AI placement relative to kernel unspecified though server AI/ghosts/replays need in-kernel AI | Architecture gap | §6: AI executes inside deterministic kernel, seeded RNG | FIXED |
| D-33 | MED | Reviewer 3 | 1 | EU fee table conflates IAP tier with paid-app purchase tier; Play rollout incomplete (AU/JP dates, alt-billing exemption, program enrollment) | Fee modeling imprecise | §13.2 note: base-purchase fees modeled separately; full rollout table cited (R2 dossier) | FIXED |
| D-34 | MED | Reviewer 3 | 1 | No consumer-law module (2019/770, CRD, UCPD, withdrawal/refunds) | Compliance frame incomplete | §14.4 consumer-law module + refund SOPs | FIXED |
| D-35 | MED | Reviewer 3 | 1 | No store-metadata trademark hygiene; mobile refund/trial posture absent; launch window collides with official product | Launch-readiness gaps | §14.1 metadata checklist; §13.4/§14.4 trial/refund; §13.4 window moved to Jun–Aug with differentiation rationale | FIXED |
| D-36 | MED | Reviewer 3 | 1 | Live-ops under-staffed for promissed cadence; ZH locales vs CN geo-block; accessibility legal position absent | Resourcing/consistency gaps | §15 live-ops FTE + on-call; §12 locale reprioritization; §14.4 WCAG 2.2 AA target | FIXED |
| D-37 | LOW | Reviewer 1 | 1 | "768 from 800" imprecise (2025: 798 incl. driver) | Delta sloppiness | R1 fixed to 798 | FIXED |
| D-38 | LOW | Reviewer 1 | 1 | "Monaco two SQ sessions" unexplained jargon | Copy phrasing | R1 clarified (usual Q plus SQ) | FIXED |
| D-39 | LOW | Reviewer 3 | 1 | Patent screen deferred to greenlight while store promises can leak earlier | Timing | Kept at greenlight; store-page commitments gated on §15 no-2028-promise rule | ACCEPTED (risk noted) |
| D-40 | LOW | Reviewer 3 | 1 | Steam DLC $100 per-app fee omitted | Incomplete economics | §13.1 notes per-DLC $100 fee | FIXED |

**Tally iteration 1**: CRITICAL 8 · HIGH 14 · MEDIUM 14 · LOW 4 = **40 findings; 39 fixed, 1 accepted** (D-39).

## Regression check (post-correction)

- Q3 13′ ↔ R1 ↔ §7.1 consistent; deploy curves §5.1 ↔ R1 ↔ S-41 consistent; calendar fictionalization (§8.3) ↔ brand hygiene (§14) consistent; determinism scope (§4.2) ↔ netcode (§10) ↔ input sampling (§11.3) consistent; revenue/budget (§13.1b ↔ §18) consistent; risks (R-16/17/18) ↔ unknowns (U-05/08/09) consistent.

## Iteration 2 findings (3 fresh re-auditors, 2026-10-11) — 21 findings

| ID | Sev | Found by | Iteration | Defect | Root cause | Correction | Status |
|----|-----|----------|-----------|--------|------------|------------|--------|
| D-41 | HIGH | Reviewer A2 | 2 | Real-world names still leaked into product-facing blueprint (§5.2 Monza/Madrid ×2, §7.1 Monaco 260 km, §8.3 Madrid-type/Ramadan, §17 R-02, §2/§5.4 event names) — D-08 incomplete | Fictionalization applied to round names only, not archetypes/citations | All replaced with archetype language + "research reference" framing; real names confined to R1 | FIXED |
| D-42 | HIGH | Reviewer B2 | 2 | Downlink bandwidth arithmetic impossible (22×40 B×60 Hz = 52.8 KB/s vs stated 12–18 KB/s); no interest management | Budget asserted without mechanism | §10.1 tiered send plan (near 30 Hz/mid 15 Hz/far 10 Hz, quantization) target 15–25 KB/s | FIXED |
| D-43 | HIGH | Reviewer B2 | 2 | DDoS/abuse hardening and per-player server cost model absent despite D-10 claim | Correction claimed scope not delivered | §10.2 added hardening + cost model + U-08 linkage | FIXED |
| D-44 | HIGH | Reviewer C2 | 2 | §13.1b revenue model never actually written; D-14 falsely logged FIXED (parallel-edit collision silently lost the edit) | Two parallel edits to same file; no post-edit verification | §13.1b written (scenarios, breakeven 450–500k units-eq); DEFECTS tally corrected | FIXED |
| D-45 | HIGH | Reviewer C2 | 2 | Mobile premium viability gates + premium-lite fallback absent; dangling refs | D-17 partially logged | §13.1 gates (conversion ≥2.5%, D7 ≥25%, refunds ≤5%) + premium-lite definition | FIXED |
| D-46 | HIGH | Reviewer C2 | 2 | No EULA/ToS module (license grant, conduct, UGC terms, termination, arbitration) | Consumer law fixed without user terms | §13.1c EULA/ToS + WFH clauses + esports integrity + embargo SOP | FIXED |
| D-47 | MED | Reviewer A2 | 2 | Overtake taper slope unspecified; "advantage band 340–355" contradicted curve (advantage opens at 290) | Endpoint-only spec; stale prose | §5.1 taper data table (200@345, 100@350); §5.4 advantage 290→355, peak ~+235 kW @337 | FIXED |
| D-48 | MED | Reviewer A2 | 2 | +0.5 MJ harvest and 350 kW ceiling still tier-inflated (CONFIRMED via secondary) in §5.1 | Tier discipline regression | +0.5 MJ → REPORTED; 350 kW re-cited to PU Regs Iss 06/FIA overview (primary) | FIXED |
| D-49 | MED | Reviewer A2 | 2 | Parc fermé, sprint-points ≥2-green-lap condition, penalty catalogue values, grid-penalty procedure, PU element allocation, tyre session mandates, scrutineering all missing | Rules engine enumerated headline rules only | §7.1–7.4 additions (parc fermé, mandates, scrutineering, PU ledger, penalty values + serving state machine, grid-penalty resolution, sprint green-lap condition) | FIXED |
| D-50 | MED | Reviewer B2 | 2 | Clock/interp spec not build-grade (naive RTT/2, no interp-delay formula, no ack base, no adaptation); contact reconciliation semantics undefined | Spec stated intent, not algorithm | §10.1: min-RTT estimator, interp delay = buffer+1 tick, ack base-frame id, redundancy/bitrate adaptation, remote-chassis history + snap-before-contact rule | FIXED |
| D-51 | MED | Reviewer B2 | 2 | Determinism scope tension unexplained (cross-arch CI gate vs same-arch runtime scope) | Contradiction left implicit | §4.2 determinism precondition (build pinning, no libm, no contraction) + build-vs-sim variance rationale | FIXED |
| D-52 | MED | Reviewer B2 | 2 | Fixed-point contract incomplete (rounding, saturation, unit ranges, transcendental tables) | "defined rounding" never defined | §4.2 numerics appendix: round-half-even, saturating wraparound detection, documented ranges, fixed-point coefficient tables in golden contract | FIXED |
| D-53 | MED | Reviewer B2 | 2 | Cloud-save conflict policy absent; FFB/haptics latency budget absent; backfill join protocol absent; baked FP data not checksummed; replay version stamping absent; input-drop clamp margin thin | Secondary-feature gaps | §10.4 save policy; §11.3 FFB ≤20 ms; §10.1 join/backfill + RTT-tied clamp; §4.4 baked-data freeze; §16.7 version stamping | FIXED |
| D-54 | MED | Reviewer C2 | 2 | Korea GRAC: CONFIRMED + UNKNOWN statements coexisted; ×1.5 fee cited only secondarily while tiered CONFIRMED | Correction landed twice with different claims | §14.3 single CONFIRMED obligation statement; cost/lead-time as U-05; R-07/R-17 deduplicated in register intent (cost-side) | FIXED |
| D-55 | MED | Reviewer C2 | 2 | Pirelli supplier/C1–C5 named in product-facing §5.1 as CONFIRMED without fictionalization note | Research values flowed into product spec unchanged | §5.1 → profile-data framing; §14.3 play-level naming rule (fictional supplier, H/M/S archetypes) | FIXED |
| D-56 | MED | Reviewer C2 | 2 | Support/customer-care FTE absent; budget omitted localization/legal/GRAC/support; timeline arithmetic 22 stated as 24; launch year unanchored | Staffing/budget arithmetic slips | §18: support FTE, full budget lines, corrected 22-month arithmetic + Q3–Q4 2028 anchor | FIXED |
| D-57 | LОW | Reviewer A2 | 2 | DEFECTS tally arithmetic wrong (40 rows ≠ 39/38); §5.5 cross-ref §7.7→§7.4; R1 §3 heading CONFIRMED over mixed items | Bookkeeping errors | Tallies corrected; cross-ref fixed; R1 heading amended | FIXED |

**Tally iteration 2**: HIGH 6 · MEDIUM 10 · LOW 1 = 17 findings (D-57 consolidates 5 low items). All FIXED.
**Cumulative**: D-01…D-57; unresolved critical/high: **0**.

## Iteration 3 findings (2 final verification reviewers, 2026-10-11)

| ID | Sev | Found by | Iteration | Defect | Root cause | Correction | Status |
|----|-----|----------|-----------|--------|------------|------------|--------|
| D-58 | HIGH | Reviewer B3 | 3 | Stale downlink figure (12–18 KB/s, 22×40 B arithmetic) survived beside the corrected tiered plan — self-contradiction in §10.1 | D-42 correction added the new plan without removing the old line | §10.1 uplink/downlink bullet re-pointed to tiered plan (15–25 KB/s) | FIXED |
| D-59 | HIGH | Reviewer A3 | 3 | §14.1/§17 pointed at `docs/20-design/` which was empty; AC-7 requires final commit hash | Dangling cross-references; deliverable docs not created | Created `docs/20-design/BRAND_NAMING_CHECKLIST.md` and `docs/20-design/RISK_REGISTER.md`; refs re-pointed; commit at close | FIXED |
| D-60 | MED | Reviewer B3 | 3 | §10.5 flat "8 ticks" clamp contradicted §10.1 RTT-tied clamp | D-50 correction landed in one of two places | §10.5 cross-refs §10.1 clamp; drop rate logged | FIXED |
| D-61 | MED | Reviewer B3 | 3 | §5.1 supplier/compound rows printed Pirelli/C1–C5 with no naming-rule pointer | D-55 framing-only | Rows relabeled "product ships fictional names"; rule in §14.3 | FIXED |
| D-62 | MED | Reviewer B3 | 3 | §13.1b mobile net math wrong ($5.5 impossible); breakeven double-counted marketing | Arithmetic slip; budget definition ambiguity | Net recomputed ($8.0 Play / $7.4 Apple); breakeven stated per budget definition (330–400k in-band) | FIXED |
| D-63 | MED | Reviewer B3 | 3 | No account-deletion/data-export (DSAR) path — store certification risk | Privacy module covered consent, not erasure | §14.4 deletion/export + M3 gate; §10.4 propagation rules | FIXED |
| D-64 | MED | Reviewer A3 | 3 | N-03 (≤150 ms @200 ms RTT) lacked an acceptance metric | Requirement without gate | §10.3 explicit metric + §16.2 emulation-suite measurement | FIXED |
| D-65 | MED | Reviewer A3 | 3 | SOURCES S-31 vs S-41 formula discrepancy (7100 vs 6900−20v) read as contradiction | Register carried superseded draft values | S-31 annotated: superseded detail; mapping to final Iss 20 curves | FIXED |
| D-66 | MED | Reviewer A3/B3 | 3 | §12 cites "vendor TBD at M3, U-list" but no §19 entry; DEFECTS iter-1 tally mis-stated | Cross-reference/bookkeeping slips | U-10 added (ECA age-verification vendor); tally corrected (40 findings; 39 fixed, 1 accepted) | FIXED |
| D-67 | LOW | Reviewer A3 | 3 | GAUNTLET_STATE carried stale Q3 12′/7100−20v/old paths | State file not re-synced after corrections | State file re-synced this iteration | FIXED |
| D-68 | LOW | Reviewer A3 | 3 | Korea statements duplicated in §14.3; R-07/R-17 overlapped | Correction landed twice | Single consolidated Korea statement; R-17 reframed as cost/lead-time under R-07 decision | FIXED |

**Tally iteration 3**: HIGH 2 · MEDIUM 6 · LOW 2 = 10 findings. All FIXED.
**Cumulative**: D-01…D-68; **unresolved critical/high: 0** across three independent adversarial rounds.
