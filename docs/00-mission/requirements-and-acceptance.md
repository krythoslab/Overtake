# Requirements & Acceptance Criteria (R0)

**Mission**: Master blueprint for a commercially releasable single-seater (F1-style) racing game — Steam (PC + SteamOS/Proton), iOS, Android. Blueprint only; no code implementation.

## R0.1 Functional requirements (blueprint must specify)

| ID | Requirement |
|----|-------------|
| F-01 | Vehicle simulation faithful to the ratified 2026–2028 rule set (power unit/ERS, active aero, masses, tyres), expressed as data-driven regulation profiles, not hard-coded season logic. |
| F-02 | Race systems: qualifying, sprint, race, safety car/VSC, red flags, penalties, classification, championship points — matching FIA 2026/2027 sporting regulations where stated. |
| F-03 | Overtake mechanic (DRS replacement) modeled per 2026 rules: detection/activation lines, event-specific gap, +0.5 MJ harvest, speed-dependent deploy limits, disabled under SC/low grip. |
| F-04 | AI opponents with racing line, energy/tyre strategy, overtaking and defense behavior, graduated difficulty. |
| F-05 | Track system with authoring pipeline and format spec; content calendar mapping 2026/2027 confirmed rounds and 2028 placeholder. |
| F-06 | Career mode with season, progression, teams/drivers (fictional), contract and R&D loops. |
| F-07 | Multiplayer: cross-play Steam/iOS/Android, dedicated authoritative servers, 22-car grids, anti-cheat posture. |
| F-08 | Input schemes per platform (wheel/pad/keys; touch: tilt + tap-steer; gyro), full assists menu. |
| F-09 | Accessibility (colorblind, remap, difficulty, text scaling, haptics), localization plan. |
| F-10 | Commercial: pricing, monetization (no loot boxes), store pipeline, update/cadence plan. |

## R0.2 Non-functional requirements

| ID | Requirement |
|----|-------------|
| N-01 | Deterministic simulation (fixed timestep; cross-platform bit-identical within tolerance; verified by golden-replay CI). |
| N-02 | Performance targets per platform tier (see blueprint §11) incl. Steam Deck Verified criteria and mobile thermal envelopes. |
| N-03 | Netcode targets: ≤150 ms effective input-to-response on predicted car at 200 ms RTT; desync-free; server-authoritative. |
| N-04 | Content budget caps (download size, install size, memory per tier). |
| N-05 | Compliance: age ratings (IARC + GRAC), privacy (GDPR/COPPA/Apple privacy manifest/Play Data Safety), regional monetization law. |
| N-06 | Security/legal: no reliance on FIA/F1/trademarked IP; brand-hygiene checklist enforced on all content. |
| N-07 | Localization readiness: externalized strings, ≥8 launch languages planned. |
| N-08 | Maintainability: regulation profiles as data; season content patches without engine changes. |

## R0.3 Acceptance criteria (completion gate for the blueprint)

The blueprint is DONE when all pass:

- **AC-1 Requirements coverage**: every F/N requirement has a design section, with platform-spanning decisions (Steam/iOS/Android) addressed.
- **AC-2 Evidence**: every load-bearing factual claim carries a confidence tier + traceable source with publication date; a source register exists; no fabricated citations.
- **AC-3 Consistency**: no internal contradictions between sections (terms, numbers, architecture); single glossary.
- **AC-4 Feasibility**: performance/multiplayer/schedule/budget claims are either evidence-backed or explicitly ASSUMPTION with sensitivity notes; no infeasible targets (e.g., full-bit lockstep across 22 mobile cars).
- **AC-5 Adversarial review**: ≥2 independent adversarial reviews (fresh context each) find no unresolved critical/high defects; medium defects either fixed or accepted with risk note.
- **AC-6 Completeness of unknowns**: every material unknown is enumerated with impact + resolution plan and owner; no silence.
- **AC-7 Deliverables**: MASTER_BLUEPRINT.md + research dossiers + design docs + GAUNTLET_STATE.md + review log + final commit hash.

## R0.4 Definition of Done for this repo (blueprint-only)

- All artifacts are Markdown; the GitHub Actions workflow validates Markdown links only (docs tooling, not product code).
- No engine code, no assets, no project settings — by mission constraint.
