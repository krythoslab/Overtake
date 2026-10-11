# GAUNTLET STATE — Mission Control

**Mission**: Create a completely new, comprehensive master blueprint for a commercially releasable F1-style racing game targeting Steam, iOS, and Android. Blueprint only — NO CODE IMPLEMENTATION.

**Loop**: Requirement → Research → Design → Adversarial Critic → Root-Cause Analysis → Correction → Independent Re-audit → Repeat.

**Started**: 2026-10-11 (session 1)
**Current iteration**: Iteration 4 (verification complete; closing out)
**Status**: Iteration 3 verification found 2 HIGH / 6 MED / 2 LOW — all corrected (D-58…D-68); deliverables completed (design docs created; GAUNTLET_STATE re-synced). Final commit + hash next. **Unresolved critical/high: 0 across three independent adversarial rounds.**

---

## 1. Objectives

1. **O1 — Master blueprint**: Comprehensive, self-consistent, evidence-traced blueprint for a commercial single-seater (F1-style) racing game for Steam (PC/Linux via Proton), iOS, Android.
2. **O2 — Research integrity**: Every load-bearing claim cited with source + date. Confidence tiers: CONFIRMED (primary/official), RATIFIED-CHANGED (approved change), REPORTED (credible secondary), ASSUMPTION, UNKNOWN/IRREDUCIBLE. No fabricated sources.
3. **O3 — Adversarial rigor**: Independent adversarial reviews of the draft; every critical/high defect gets root-cause analysis, correction, and regression check.
4. **O4 — Commercial feasibility**: Platform, licensing, monetization, compliance, performance, multiplayer feasibility all resolved to a shippable plan or an honestly documented unknown.
5. **O5 — Continuity**: This file survives interruption; next session reads this + repo state and continues without restart.

## 2. Completed work (this session)

- Repo verified as clean slate (only stale `.github/workflows/dotnet.yml` from prior code era; removed — blueprint-only repo).
- Research completed with citations (see `docs/10-research/`):
  - F1 2026 Technical Regs (Section C, Iss 20, 2026-08-05) + PU Regs (Iss 6, 2024-03-29) + Section A Iss 03 (2026-06-25) + Section B Iss 08 (2026-08-05).
  - Overtake Mode (replaces DRS): detection/activation lines, event-specific Detection Gap, +0.5 MJ harvest, power curves (1800−5v/7100−20v → 345/355 km/h), disabled under SC and Low Grip; free in practice/qualifying.
  - Active Aero Straight/Corner Mode (all cars, all zones); MGU-K 350 kW; ICE ~400 kW; 100% sustainable fuel; MGU-H removed; fuel flow 3000 MJ/h cap; ES SoC window 4 MJ; min mass 768 kg; car width 1900 mm; tyres −25/−30 mm width, −15/−10 mm diameter; compounds C1–C5; Pirelli sole supplier through 2028 (announced 2026-06-11).
  - 2026 calendar (24 races; Madrid debut subject to homologation; Imola out; Zandvoort final; Sprints: CHN, MIA, CAN, GBR, NED, SIN); Cadillac 11th team → 22-car grid; Squads SQ 12/10/8 min, Q 18/15/12 min; points tables; no fastest-lap point (removed from 2025); reduced-race points columns; Sprint 8→1.
  - 2027 calendar announced 2026-09-16: 24 races, 10 Sprints (BHR, AUS, JPN, MON×2SQ, CAN, GBR, ITA, BRA, QAT, ABU), Portugal (Portimão) and Türkiye (Istanbul) return, Zandvoort/Barcelona out.
  - Licensing: EA/Codemasters exclusive F1 racing-game license; EA Sports skipped F1 26 (paid F1 25 expansion for 2026; next full title F1 27 in 2027). Frontier F1 Manager through 2025. Hutch/Eden mobile licenses separate.
  - Tech: Unity 6.3 LTS (6000.3.0f1, ~Dec 2025; LTS to Dec 2027; Runtime Fee canceled); URP mobile features (On-Tile Post Processing, Adaptive Performance, iOS thermal throttling).
  - Platforms: Steam Deck Verified requires 30fps@800p default, text ≥9px@1280x800, Proton-safe (no incompatible anti-cheat), Steam Machine 30fps@1080p; Steam $100 recoupable per-app fee; 30% store cut.
  - Mobile 2026: Apple EU unified terms effective 2026-10-01 (IAP 26%/15% SBP; alt-pay 20%/10%; link-out 15%/10%; CTC 5%); Google Play split service/billing fees from 2026-06-30 (service 10% of first $1M; non-recurring 20%+5% billing US/EEA/UK; 15%/20% programs; KR 2026-12-31; RoW 2027-09-30).
  - Compliance: IARC single questionnaire (ESRB/PEGI/USK/ClassInd/GRAC/ACB); Korea GRAC/IRCB mandatory rating; China NPPA ISBN + local publisher required; Brazil loot-box under-18 ban 2026; Korea odds disclosure; PEGI odds disclosure; ACB Refused Classification risk.
  - Netcode: authoritative dedicated servers + client prediction + server reconciliation + interpolation for racing (22 cars) — full rollback unsuitable; determinism pitfalls (libm/FMA/fast-math) documented.
- Blueprint drafting: `MASTER_BLUEPRINT.md` (in progress), `docs/00-requirements.md`.

## 3. Defects found & corrections (running log)

Full log: `docs/90-gauntlet/DEFECTS.md` (D-01…D-40). Iteration 1 summary: 3 independent reviewers found **8 CRITICAL / 14 HIGH / 13 MEDIUM / 4 LOW**. All corrected except D-39 (accepted w/ note). Key root causes: (a) unverified regulation values promoted to fact (Q3 13′, piecewise ERS curves, Iss 09 currency) — fixed with primary-source verification; (b) determinism declared not specified — fixed with fixed-point kernel + codegen controls + bounded scope; (c) prediction/authority boundary undefined — fixed with chassis-core/ledger split; (d) brand hygiene contradicted by calendar/pipeline — fixed with fictional rounds + original-content pipeline; (e) budget/revenue, Korea GRAC, PIPA/ECA Digital, consumer law, mobile-model fallback gaps — fixed.

## 4. Failed checks / open risks

- **R-01 (assumption-flagged)**: EA license status beyond 2027 unknown → product must not depend on it; Track B (licensed) is contingency only.
- **R-02**: Madrid 2026 circuit "subject to FIA circuit homologation" — conditional in reference data.
- **R-03**: 2028 calendar/regs not published (as of 2026-10-11) → data slot, no content commitments.
- **R-04**: Deploy-curve numerics REPORTED (article nos. CONFIRMED) → U-03: full-text cross-check at M1.
- **R-05**: Mobile cellular download-size limits / store asset limits re-verify at release.
- **R-06**: EU Apple 12-month payment lock-in → store billing at launch.
- **R-07/R-17**: Korea PC/Steam GRAC obligation CONFIRMED via local agent; greenlight decision (budget vs region-gate).
- **R-08**: China ISBN + local publisher → geo-block CN at launch.
- **R-16**: SDR cross-platform (mobile+PC) eligibility partner-gated → M2 gate + self-hosted relay fallback.
- **R-18**: Mobile premium viability gates + premium-lite fallback at M3.

## 5. Exact next actions

1. **Iteration 2 re-audit**: dispatch 2–3 fresh independent reviewers (no prior context) on the corrected blueprint; rubric = verify D-01…D-40 corrections are real and find NEW defects.
2. Triage new findings → DEFECTS.md (D-41+); RCA + corrections + regression check.
3. Repeat until gate: zero unresolved critical/high across two consecutive fresh reviews.
4. Final: update GAUNTLET_STATE + REVIEW_LOG; commit; record final hash.

## 6. Iteration history

| Iter | Date | Phase | Outcome |
|------|------|-------|---------|
| 1 | 2026-10-11 | Research + Requirements + Design draft | Research corpus complete; MASTER_BLUEPRINT.md + 4 dossiers + requirements written |
| 1 | 2026-10-11 | Critic + RCA + correction | 3 independent reviewers: 8C/14H/13M/4L (40 findings); 39 fixed, 1 accepted (D-39) |
| 2 | 2026-10-11 | Fresh re-audit + RCA + correction | 3 fresh reviewers: 6H/10M/1L; D-41…D-57 all fixed; root cause of 2 false "FIXED" logs identified (parallel-edit collision) and process hardened |
| 3 | 2026-10-11 | Final verification reviews + RCA + correction | 2 final reviewers: 2H/6M/2L; D-58…D-68 all fixed; deliverables completed (design docs, commit) |
