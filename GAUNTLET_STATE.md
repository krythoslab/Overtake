# GAUNTLET STATE — Mission Control

**Mission**: Create a completely new, comprehensive master blueprint for a commercially releasable F1-style racing game targeting Steam, iOS, and Android. Blueprint only — NO CODE IMPLEMENTATION.

**Loop**: Requirement → Research → Design → Adversarial Critic → Root-Cause Analysis → Correction → Independent Re-audit → Repeat.

**Started**: 2026-10-11 (session 1)
**Current iteration**: Iteration 5 (regulation-currency re-audit complete; close-out)
**Status**: Iteration 3 closed D-58…D-68. Mission reopened 2026-10-12 for an independent defect sweep: verified the blueprint against FIA primary sources, found and fixed D-69…D-79 (5 HIGH / 4 MED / 1 LOW) and D-80…D-84 (1 MED / 4 LOW), added traceable URLs to `docs/10-research/SOURCES.md`, and fixed the CI link checker so it can actually fail. **Unresolved critical/high: 0 across four independent rounds.**

---

## 1. Objectives

1. **O1 — Master blueprint**: Comprehensive, self-consistent, evidence-traced blueprint for a commercial single-seater (F1-style) racing game for Steam (PC/Linux via Proton), iOS, Android.
2. **O2 — Research integrity**: Every load-bearing claim cited with source + date **+ direct URL**. Confidence tiers: CONFIRMED (primary/official), RATIFIED-CHANGE (approved change), REPORTED (credible secondary), ASSUMPTION, UNKNOWN/IRREDUCIBLE. No fabricated sources; sources whose URL cannot be re-verified are flagged and may not drive a default.
3. **O3 — Adversarial rigor**: Independent adversarial reviews of the draft; every critical/high defect gets root-cause analysis, correction, and regression check.
4. **O4 — Commercial feasibility**: Platform, licensing, monetization, compliance, performance, multiplayer feasibility all resolved to a shippable plan or an honestly documented unknown.
5. **O5 — Continuity**: This file survives interruption; next session reads this + repo state and continues without restart.

## 2. Completed work (this session)

- Repo verified as clean slate (only stale `.github/workflows/dotnet.yml` from prior code era; removed — blueprint-only repo).
- Research completed with citations (see `docs/10-research/`):
  - F1 2026 Technical Regs (Section C, Iss 20, 2026-08-05; WMSC 2026-08-03) + PU Regs (Iss 6, 2024-03-29) + Section A Iss 03 (2026-06-25) + Section B Iss 09 (2026-10-01; supersedes Iss 08) + Section D Iss 08 / Section F Iss 11 (2026-10-01).
  - **Article text verified verbatim (2026-10-12)**: deploy limits Art. C5.2.8 i/ii (1800−5·v below 340 km/h; 6900−20·v for 340–345; 0 ≥345 — and with Overtake active 7100−20·v below 355, capped at 350 kW ⇒ 350 kW flat to 337 km/h, 0 at 355); ES SoC ≤4 MJ (C5.2.9); harvest ≤8.5 MJ/lap with +0.5 MJ when Overtake conditions apply, reducible to 8 MJ and to ≥5 MJ in SQ/Q (C5.2.10 i–iii); MGU-K torque ≤500 Nm (C5.2.11).
  - **2026 designated-zone MGU-K envelope** (FIA statement 2026-04-20, consolidated into Section C Iss 20): 350 kW in designated acceleration zones (corner-exit → braking point, incl. overtaking zones), 250 kW elsewhere; qualifying recharge 8→7 MJ; superclip peak 250→350 kW; race Boost capped at +150 kW; alternative energy limits at 12 events; automatic start deployment + flashing lights.
  - **RATIFIED 2027/2028 power-unit package** (WMSC Macau 2026-06-23; FIA/FOM table 2026-06-10): ICE 400→420 kW (2027, +5 % fuel flow)→450 kW (2028, +13 %); MGU-K deploy 350→300 kW (held at 300 in 2028); Overtake ceiling 350 kW in all three years; max harvesting power 350→375→400 kW; split 53/47→58/42→60/40. Consolidated 2027 technical text published as Section C Iss 01/02/03 (Iss 03 = 2026-10-01).
  - Overtake Mode (replaces DRS): detection/activation lines, event-specific Detection Gap, +0.5 MJ harvest, speed-dependent deploy limits, disabled under SC and Low Grip; free in practice/qualifying. Active Aero Straight/Corner Mode; 100 % sustainable fuel; MGU-H removed; min mass 768 kg; car width 1900 mm; tyres −25/−30 mm width, −15/−10 mm diameter; compounds C1–C5; Pirelli sole supplier through 2028 (2026-06-11).
  - 2026 calendar (24 races; Madrid debut subject to homologation; Imola out; Zandvoort final; Sprints: CHN, MIA, CAN, GBR, NED, SIN); Cadillac 11th team → 22-car grid; Squads SQ 12/10/8 min, Q 18/15/12 min; points tables; no fastest-lap point; reduced-race points columns.
  - 2027 calendar announced 2026-09-16: 24 races, 10 Sprints, Portugal (Portimão) and Türkiye (Istanbul) return, Zandvoort/Barcelona out.
  - Licensing: EA/Codemasters exclusive F1 racing-game license; EA Sports skipped F1 26 (paid F1 25 expansion for 2026; next full title F1 27 in 2027). Frontier F1 Manager through 2025. Hutch/Eden mobile licenses separate.
  - Tech: Unity 6.3 LTS (6000.3.0f1, ~Dec 2025; LTS to Dec 2027; Runtime Fee canceled); URP mobile features (On-Tile Post Processing, Adaptive Performance, iOS thermal throttling).
  - Platforms: Steam Deck Verified requires 30fps@800p default, text ≥9px@1280×800, Proton-safe; Steam Machine 30fps@1080p; Steam $100 recoupable per-app fee; 30 % store cut.
  - Mobile 2026: Apple EU unified terms effective 2026-10-01 (IAP 26 %/15 % SBP; alt-pay 20 %/10 %; link-out 15 %/10 %; CTC 5 %); Google Play split service/billing fees from 2026-06-30.
  - Compliance: IARC single questionnaire; Korea GRAC/IRCB mandatory rating; China NPPA ISBN + local publisher required; Brazil loot-box under-18 ban 2026; Korea odds disclosure; PEGI odds disclosure; ACB Refused Classification risk.
  - Netcode: authoritative dedicated servers + client prediction + server reconciliation + interpolation for racing (22 cars) — full rollback unsuitable; determinism pitfalls documented.
- Blueprint drafting: `MASTER_BLUEPRINT.md`, `docs/00-mission/requirements-and-acceptance.md` (the requirements document's live location; an obsolete reference to a pre-move requirements path was removed in the 2026-10-12 sweep — see D-75).
- **2026-10-12 reopen sweep**: regulation claims re-verified against FIA primary sources; SOURCES.md gained a URL column (every load-bearing URL fetched, HTTP 200); the `docs` workflow link checker was fixed (pipeline-subshell bug) and extended to cover backtick document paths; DEFECTS D-69…D-83 logged and closed.

## 3. Defects found & corrections (running log)

Full log: `docs/90-gauntlet/DEFECTS.md` (D-01…D-83). Iteration 1 summary: 3 independent reviewers found **8 CRITICAL / 14 HIGH / 13 MEDIUM / 4 LOW**; 39 fixed, 1 accepted (D-39). Key root causes then: unverified regulation values promoted to fact; determinism declared not specified; prediction/authority boundary undefined; brand hygiene contradicted by calendar/pipeline; budget/compliance gaps — all fixed. Iteration 2: 6 HIGH / 22 MEDIUM / 13 LOW reviewer items consolidated into 17 rows (D-41…D-57), all fixed; root cause of 2 false "FIXED" logs identified (parallel-edit collision) and process hardened. Iteration 3: 10 findings (D-58…D-68), all fixed. **Iteration 4 (2026-10-12 reopen)**: 10 findings (D-69…D-79) — omitted ratified 2027/2028 PU package, missing 2026 designated-zone MGU-K envelope, tier-inflated deploy/harvest sourcing, zero URLs in the source register, unfailable CI link checker, vacuous link gate, stale state/bookkeeping, undefined sim-vs-render frequency — all fixed. **Iteration 5**: 5 findings (D-80…D-84) — requirements/glossary/R1 drift plus a defect caught *inside* the iteration-4 correction (the deploy envelope would have clamped 2027 Overtake to 300 kW) — all fixed.

## 4. Failed checks / open risks

- **R-01 (assumption-flagged)**: EA license status beyond 2027 unknown → product must not depend on it; Track B (licensed) is contingency only.
- **R-02**: Madrid 2026 circuit "subject to FIA circuit homologation" — conditional in reference data.
- **R-03**: 2028 **calendar and non-PU regulations** unknown (the 2028 power-unit step *is* ratified) → data slot with explicit `UNKNOWN` markers; no content commitments.
- **R-04**: 2026 deploy/harvest constants primary-verified against Section C Iss 20 article text; **2027/2028 consolidated constants** still need a line-by-line cross-check (U-03/U-11).
- **R-05**: Mobile cellular download-size limits / store asset limits re-verify at release.
- **R-06**: EU Apple 12-month payment lock-in → store billing at launch.
- **R-07/R-17**: Korea PC/Steam GRAC obligation CONFIRMED via local agent; greenlight decision (budget vs region-gate).
- **R-08**: China ISBN + local publisher → geo-block CN at launch.
- **R-09**: UGC scope livery-only with moderation.
- **R-16**: SDR cross-platform (mobile+PC) eligibility partner-gated → M2 gate + self-hosted relay fallback.
- **R-18**: Mobile premium viability gates + premium-lite fallback at M3.
- **R-23**: Regulation drift between ratified announcements and consolidated text → profile carries `sourceIssues[]` + confidence + `verifiedOn`; full-text cross-check at M1.

## 5. Exact next actions

1. **CLOSED** — content corrections committed in the iteration-5 corrections commit (see §5b for hashes). All artifacts committed; internal document-reference check passes (16 references).
2. If resuming: completion gate is met (0 unresolved critical/high across four independent rounds; AC-1…AC-7 verified). Remaining work is production-phase only: M1 business case + FTO opinion + FIA full-text verification of the consolidated 2027 Section C constants and the 2026 designated-zone article numbering (U-03/U-04/U-11) + Korean agent decision (R-07) + SDR eligibility gate (R-16) — none of which can be advanced from public sources alone.

## 5b. Completion gate status

| Acceptance criterion | Status | Evidence |
|---|---|---|
| AC-1 Requirements coverage | PASS | §3 traceability; F/N mapped to sections; F-01/F-03 updated for the 2027–2028 trajectory and the zone envelope |
| AC-2 Evidence | PASS | `docs/10-research/SOURCES.md` (S-01…S-64, dated, tiered, **direct URLs re-verified 2026-10-12**) |
| AC-3 Consistency | PASS | Regression checks iter 1–4; internal document-reference check green (checker fixed and proven to fail on a broken fixture) |
| AC-4 Feasibility | PASS | Targets tiered/assumption-flagged; infeasible claims corrected (D-42/62) |
| AC-5 Adversarial review | PASS | 11 independent reviews across 4 rounds; 83 defects, 82 fixed + 1 accepted-with-note; 0 unresolved C/H |
| AC-6 Unknowns | PASS | §19 U-01…U-11 + risks R-01…R-23 (full register `docs/20-design/RISK_REGISTER.md`) |
| AC-7 Deliverables + hash | PASS | see commit hashes below |

**Commit hashes (reconciled with `git log`)**:
- Iteration 1–3 close-out: `4fbefe2` ("docs: close gauntlet gate — completion status in GAUNTLET_STATE") = `4fbefe2260eceee805e6dbb74fc43d78fbe52b9c`.
- Iteration 4–5 corrections (all content, tooling, source-register and bookkeeping fixes in this file's companion documents): recorded in the close-out commit immediately following it, which also re-syncs `GAUNTLET_STATE.md` and `docs/90-gauntlet/REVIEW_LOG.md`. Run `git log --oneline` for the authoritative list; `git rev-parse HEAD` is the tree this state file describes.

## 6. Iteration history

| Iter | Date | Phase | Outcome |
|------|-------|-------|---------|
| 1 | 2026-10-11 | Research + Requirements + Design draft | Research corpus complete; MASTER_BLUEPRINT.md + 4 dossiers + requirements written |
| 1 | 2026-10-11 | Critic + RCA + correction | 3 independent reviewers: 8C/14H/13M/4L (40 findings); 39 fixed, 1 accepted (D-39) |
| 2 | 2026-10-11 | Fresh re-audit + RCA + correction | 3 fresh reviewers: 6H/22M/13L items → D-41…D-57 all fixed; root cause of 2 false "FIXED" logs identified (parallel-edit collision) and process hardened |
| 3 | 2026-10-11 | Final verification reviews + RCA + correction | 2 final reviewers: 2H/6M/2L; D-58…D-68 all fixed; deliverables completed (design docs, commit) |
| 4 | 2026-10-12 | Reopen: primary-source verification + tooling/bookkeeping re-audit | Regulation claims verified against FIA documents; ratified 2027/2028 PU package and 2026 zone envelope added; SOURCES.md URLs added; link checker fixed and proven; D-69…D-79 fixed |
| 5 | 2026-10-12 | Independent re-audit of corrected sections + adjacent systems | 4 LOW findings (D-80…D-83) fixed; completion gate re-run and passing |

(End of file - total 84 lines)
