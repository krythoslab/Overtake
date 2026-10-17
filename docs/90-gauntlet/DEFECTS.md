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
| D-09 |**
**Cumulative**: D-01…D-84; unresolved critical/high: **0** across four independent rounds.

## Iteration 6 findings (reopen; D-85…D-90, 2026-10-16)

| ID | Sev | Found by | Iteration | Defect | Evidence | Root cause | Correction | Regression check | Status |
|----|-----|----------|-----------|--------|----------|------------|------------|------------------|--------|
| D-85 | MED | Re-audit 6 | 6 | §5.4/AT-R2 and AT-R4 conflict: in 2027/2028 normal deploy caps at 300 kW while Overtake is 350 kW, yet AT-R4 claimed equality below 290 km/h and 2026-style band | MASTER_BLUEPRINT §5.4 AT-R4 row; §5.1 three-year table | AT-R4 was written for the 2026 profile and asserted as universal | AT-R4 scoped to 2026 (equality below 290) with explicit 2027/2028 non-equality note; year-specific assertions table-driven | AT-R2 asserts both 300/350 branches; AT-R4 must fail if re-scoped to 2027 without revision | FIXED |
| D-86 | MED | Re-audit 6 | 6 | §5.4 defines four envelope limits but §16.1 specified a five-limit minimum that could reintroduce D-84 | §16.1 five-limit phrasing vs §5.4 min() | Iteration-5 wording not propagated | §16.1 reworded to four-limit minimum (mode- and year-aware ceiling inclusive of Overtake) | Doc-ref check + AT-R2/AT-R3 cross-check | FIXED |
| D-87 | MED | Re-audit 6 | 6 | Zone-cap fields silently carry 2026 350/250 kW into 2027/2028 despite unverified consolidated text | §5.4 P_zone annotation | No confidence/source metadata; silent inheritance | Added ASSUMPTION annotation + confidence/sourceRef/verifiedOn=null; unknown must be UNKNOWN placeholder | §5.1 rule (d) preserved; profile review checklist | FIXED |
| D-88 | LOW | Re-audit 6 | 6 | AT-D7 "two 60 Hz steps == one 120 Hz step pair" ill-defined/ill-posed | §4.2 AT-D7 old text | Duplicate of AT-D1 render equivalence | Replaced with fixed-tick time-source independence test (wall-clock stretch invariant) | AT-D1 unchanged; AT-D7 must fail if kernel reads wall clock | FIXED |
| D-89 | LOW | Re-audit 6 | 6 | Defect counts/severity tallies/iteration rows/D-84 records/GAUNTLET_STATE/REVIEW_LOG needed reconciliation with actual IDs and merged commit | DEFECTS tallies vs git log | Hand-maintained tallies | Cumulative totals updated to D-90; merged-main hash recorded f741b3b | Cross-file tally recompute | FIXED |
| D-90 | MED | Re-audit 6 | 6 | Load-bearing claims cite sources marked not re-verified (S-07/S-11/S-16/S-19/S-22/S-25/S-27/S-29/S-30…S-45 etc.) | SOURCES.md status column | Source downgrades never reflected in claim confidence | Claims that depend on not-re-verified sources flagged REPORTED/ASSUMPTION and recorded as unknowns (R-04/U-03/U-11); no default driven by them | Citation audit cross-check | FIXED (downgrade only) |

**Tally iteration 6**: MED 5 · LOW 1 = **6 findings (D-85…D-90). All FIXED.**
**Cumulative**: D-01…D-90; unresolved critical/high: **0** across independent rounds.
