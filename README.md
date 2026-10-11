# APEX Game Blueprint — Repository Guide

Blueprint-only repository (no code, by mission constraint).

## Canonical deliverable

- **`MASTER_BLUEPRINT.md`** — the comprehensive master blueprint (single canonical design document; design deep-dives for AI, netcode, tracks, QA are consolidated here to prevent cross-document drift; per-subsystem detail lives in the section that owns it).

## Mission control

- **`GAUNTLET_STATE.md`** — objectives, progress, defects, risks, next actions, iteration state. Read this first when resuming.

## Requirements

- `docs/00-mission/requirements-and-acceptance.md` — F/N requirements + acceptance criteria (the completion gate definition).

## Research dossiers (evidence)

- `docs/10-research/SOURCES.md` — full source register with dates and tiers.
- `docs/10-research/R1-regulations.md` — 2026–2028 regulations dossier.
- `docs/10-research/R2-licensing-commercial-tech.md` — licensing, engine, store economics.
- `docs/10-research/R3-multiplayer-performance.md` — netcode and performance feasibility evidence.
- `docs/10-research/R4-compliance-release.md` — compliance and release checklist.

## Gauntlet records

- `docs/90-gauntlet/DEFECTS.md` — defect log with RCA and corrections (D-01…).
- `docs/90-gauntlet/REVIEW_LOG.md` — review history.

## Loop discipline

Requirement → Research → Design → Adversarial Critic → RCA → Correction → Independent Re-audit → Repeat. The blueprint is edited only through this loop; each correction updates DEFECTS.md and, where affected, GAUNTLET_STATE.md.
