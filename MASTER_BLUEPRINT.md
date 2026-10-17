# APEX — MASTER BLUEPRINT
### A commercially releasable single-seater racing game for Steam, iOS, and Android

**Status**: Design blueprint — pre-production
**Date**: 2026-10-12 (iteration 5: regulation-currency correction — ratified 2027/2028 power-unit package and the 2026 designated-zone MGU-K envelope added; AT-R2/AT-R4 made year-aware, envelope phrasing clarified, per-year profile metadata added, AT-D7 rewritten, not-re-verified sources barred from defaults; see `docs/90-gauntlet/DEFECTS.md` D-69…D-90)
**Repo state**: blueprint-only (no code, by mission constraint)
**Working title**: APEX (placeholder; trademark clearance required before announcement)

---

## 1. Executive summary

APEX is a premium single-seater racing game built on the **ratified 2026–2028 rule set of open-wheel top-tier racing** (active aerodynamics, the three-year hybrid rebalance ratified by the FIA World Motor Sport Council on 2026-06-23, 22-car grids, sprint weekends), delivered as a data-driven product for Steam (Windows/Linux-Proton), iOS, and Android.

Three strategic decisions define the product:

1. **Dual-track licensing strategy (Track A default).** The exclusive video-game license for the real-world Formula 1 World Championship is held by EA (Codemasters); EA is shipping a paid F1 25 expansion covering 2026 and its next full title in 2027 (EA announcement, 2025-11-18). APEX therefore ships **unlicensed**: fictional teams, drivers, circuits, and a fictional championship, with mechanics faithful to the real regulations. A **Track B contingency** keeps every regulated behavior as swappable data so that, if an FOWC license is ever obtainable, branding is a content swap, not a re-architecture.
2. **Deterministic simulation as the technical foundation.** One fixed-timestep, cross-platform-deterministic physics/AI/rules kernel powers single-player, replays, ghosting, network reconciliation, and leaderboard verification. Racing games with 22 cars require authoritative servers + prediction/reconciliation/interpolation — full rollback is unsuitable at this player count (see §10 and Netcode research dossier).
3. **Regulation profiles as data.** "2026", "2027", "2028" are data profiles (masses, tyre sizes, energy rules, points tables, calendars). Content updates ship as content; the engine never branches on year.

Targets: Steam Deck **Verified-class** performance (30 fps @1280×800 default config) with 60 fps competitive play on recommended PC and mid-to-high-end mobile; 120 fps on high-end PC. Premium monetization with **no loot boxes** (driven by 2026 compliance posture in Korea/Brazil/EU).

---

## 2. Evidence & confidence framework

Every load-bearing claim in this blueprint carries a tier:

| Tier | Meaning | Usage |
|---|---|---|
| **CONFIRMED** | Primary/official source (FIA PDF, F1/FOM release, Pirelli press, Apple/Google/Valve docs, company announcement) | May drive defaults. |
| **REPORTED** | Credible secondary (major motorsport media, trade press) citing primary | Allowed with attribution; flagged when it is the sole source. |
| **RATIFIED-CHANGE** | Officially approved change (e.g., WMSC decisions) with effective date | Drives calendar/rule content. |
| **ASSUMPTION** | Studio estimate or design choice | Labeled; sensitivity noted; never cited as fact. |
| **UNKNOWN** | Not publicly determinable as of 2026-10-12 | Enumerated in §19 with resolution plan. |

Full source register with dates and direct URLs: `docs/10-research/SOURCES.md` (compiled from research dossiers R1–R5). Every load-bearing entry carries a direct `https://` URL that was fetched and returned HTTP 200 on 2026-10-12; entries whose URL could not be re-verified are explicitly flagged and are REPORTED-only. Primary FIA/F1/Pirelli documents are preferred over secondary reporting wherever both exist.

**Regulation currency check (as of 2026-10-12)** — CONFIRMED:
- FIA 2026 F1 Regulations **Section C (Technical), Issue 20, dated 2026-08-05** (WMSC approval 2026-08-03; fia.com).
- **Section B (Sporting), Issue 09, dated 2026-10-01** (WMSC approval 2026-09-30) — supersedes Issue 08 (2026-08-05). Issue 09 changes include: **abolition of the fixed 3-hour race time limit** (replaced by an event-specific hard stop), race-control power to extend disrupted Free Practice sessions, codified rain-hazard timelines (Rain Hazard declared ≥2 h before Q/SQ), and removal of the planned 2027 gearbox homologation limits (reported analysis of Iss 09 — verify full text at production).
- **Section A (General), Issue 03, dated 2026-06-25** (fia.com); Section A Issue 01 dated 2025-12-10 exists.
- **2027 F1 Regulations Section C (Technical)**: Iss 01 (2026-06-25), Iss 02 (2026-08-05, WMSC 2026-08-03), **Iss 03 (2026-10-01, WMSC 2026-09-30)** published — the consolidated 2027 technical text; 2027 Section B/A not yet on the FIA index at this date.
- 2026 Section D (Financial – F1 Teams) Iss 08 (2026-10-01) and Section F (Operational) Iss 11 (2026-10-01) published; 2026 Power Unit Technical Regulations **Issue 06, dated 2024-03-29** (fia.com).
- FIA statement: **2026 regulation amendments approved by WMSC e-vote, 2026-02-28** (fia.com) — reflected in the issues above.
- **RATIFIED-CHANGE (WMSC, Macau, 2026-06-23) — 2027/2028 power-unit rebalance**: ICE max power **400 → 420 kW (2027) → 450 kW (2028)** via fuel-flow **+5 % (2027) / +13 % (2028)**; MGU-K max deploy **350 → 300 kW (2027), held at 300 kW (2028)**; MGU-K **Overtake ceiling 350 kW in all three years**; max harvesting power **350 → 375 kW (2027) → 400 kW (2028)**; ICE/MGU-K power split **53/47 → 58/42 (2027) → 60/40 (2028)**. Supporting measures cover PU supply, race operations and PU-financial regulations.
- **2026 mid-season refinements (FIA, 2026-04-20; applied from the Miami event; consolidated into Section C Iss 20)**: MGU-K deployment held at **350 kW in designated acceleration zones** (corner-exit to braking point, including overtaking zones) and **limited to 250 kW elsewhere**; maximum recharge reduced 8 → **7 MJ** for qualifying; peak harvest ("superclip") power raised 250 → **350 kW**; race **Boost capped at +150 kW**; alternative lower energy limits permitted at **12 events** (up from 8); automatic MGU-K deployment at starts plus rear/side flashing-light warning.
- FIA/FOM **2027 calendar approved, announced 2026-09-16** (fia.com / formula1.com). Pirelli sole tyre supplier **through 2028** (FIA/F1/Pirelli, 2026-06-11).
- **Energy deployment is event-parameterized**: per-event FIA "Power Unit Information" documents (issued by the Race Director ~4 weeks ahead) set Detection Gap, detection/activation line positions, per-session energy allocations, sectors where alternative power curves apply, Power Limited Distance and maximum PU power-reduction rate (research reference: a mid-season 2026 event used Detection Gap 1.0 s; qualifying allocation 6.0 MJ; FP/out-laps 8.5 MJ; Power Limited Distance 2,923 m — REPORTED via Total Motorsport 2026-06-26; real event names appear only in research dossiers). → All such values are profile/event data, never constants.
- **Consolidated-text caveat (U-03/U-04 extended)**: the 2026 numbers below are quoted from Section C Iss 20 article text; the 2027/2028 package is ratified and published (2027 Section C Iss 03, 2026-10-01) but its consolidated numeric constants (deploy-curve tables, designated-zone and power-limited-period article numbering) still require a full-text cross-check before production — the profile carries them as data so this is a content update, not a re-architecture.

---

## 3. Requirements traceability

Requirements F-01…F-10, N-01…N-08 are defined in `docs/00-mission/requirements-and-acceptance.md`. Mapping to blueprint sections:

| Req | Sections | Req | Sections |
|---|---|---|---|
| F-01 | §4.3, §5, §16.1 | F-06 | §9 |
| F-02 | §7 | F-07 | §10 |
| F-03 | §4.3, §5.1, §5.4, §7.6, §16.1 | F-08 | §12 |
| F-04 | §6 | F-09 | §12 |
| F-05 | §8 | F-10 | §13 |
| N-01…N-08 | §4, §10, §11, §13, §14, §16 | | |

---

## 4. Technology & architecture decisions

### 4.1 Engine: Unity 6.3 LTS (Decision D-1)

**Decision**: Unity **6.3 LTS (6000.3.0f1)**, URP, IL2CPP, C# 9.
**Evidence**: Unity 6.3 LTS released ~2025-12 (unity.com/releases/unity-6) with two-year LTS support **to December 2027** (+1 year for Enterprise/Industry). Unity 6 canceled the Runtime Fee (unity.com, 2024-09). Unity 6 ships mobile-relevant features: On-Tile Post Processing for tile-based GPUs (iOS/Android), Adaptive Performance providers including iOS thermal-based frame-rate governance, accessibility (screen reader support), Android HTTP/2/Force-internal-storage build options, and URP mobile bloom filters.
**Alternatives rejected**: Unreal 5 (D-1a) — higher mobile GPU cost and larger build footprint at equal fidelity; no mobile-exclusive advantage to offset porting risk on A13/SD865-class hardware (the §11.1 minimum tiers). Custom engine (D-1b) — 9–15 added engineer-years before first content; unacceptable at this budget (ASSUMPTION: studio sizes in §18). Add risk note: Unity 6.3 LTS mobile-shipping maturity (R-11 extended to cover maturity, not just EOL).
**Risk**: Unity 6.3 LTS support ends Dec 2027; upgrade windows planned in §16.6. The iOS thermal FPS-governance provider and URP On-Tile Post Processing are documented on the 6000.5 stream (2026-06-15); **availability on the pinned 6000.3 LTS must be verified in M1** or the engine pin moves (upgrade plan §16.6 accommodates either).

### 4.2 Simulation core: deterministic kernel (D-2)

- **Language/placement**: C# simulation kernel isolated from engine rendering; Burst/Jobs permitted for visualization-only math, never for simulation (FP divergence risk: compiler vectorization/FMA/libm differences across x86/ARM — see Netcode dossier).
- **Simulation frequency vs rendering frequency (normative)**:
  - **Simulation** runs at a **fixed 60 Hz tick** on all platforms (16 666.67 µs), advancing only whole ticks. The tick count is monotonic and part of the kernel state; nothing in the kernel reads wall-clock time, `Time.deltaTime`, or frame counters.
  - **Rendering** runs at the display refresh rate (60/90/120 Hz, or dynamic-resolution/scaled frame rates). The presentation layer **interpolates** between the two most recent authoritative kernel snapshots; it may extrapolate at most one tick and must never feed an extrapolated value back into the kernel.
  - **Input** is sampled and timestamped at the **simulation tick boundary** (§11.3), never at render rate: a render frame that produces no tick consumes its input on the next tick; a tick with no render frame still advances.
  - A **120 Hz PC mode** is optional and must produce bit-identical results to 60 Hz: it is *not* a different rate but a render-only refinement, since two 60 Hz steps compose exactly (each step is a pure function of state + input). No "simulate at 120 Hz" mode is permitted — a variable/half-rate sim breaks replay equivalence.
  - Server authoritative ticks are also 60 Hz (§10.1), so client and server share the same tick semantics; remote-car interpolation is a rendering concern only.
- **Numerics**: simulation state math in **fixed-point integer units** (int64 Q32.32 signed) with **defined rounding (round-half-to-even), saturating wraparound detection on every add/multiply, and documented unit ranges per quantity (position ±1e6 m, velocity ±200 m/s, energy ±1e5 MJ, power ±1e6 kW)**; floating-point doubles permitted **only in offline tools** (track fitting, balance telemetry), never in kernel state; no fast-math; transcendentals (sin/cos/exp/pow) from an in-house library with **fixed iteration counts and precomputed fixed-point coefficient tables** (part of the golden contract); RNG = SplitMix64 with state in kernel.
- **Determinism precondition (stating what makes the CI gate achievable)**: the simulation assembly is build-pinned (engine version, compiler version, flags), uses no libm/BLAS at runtime, and excludes auto-vectorization and FP contraction of sim math (IL2CPP: Burst disabled for the sim assembly; emitted-code audits in CI). Given these preconditions, cross-platform golden checksums are a meaningful gate; the **runtime same-arch scope** (§10.1) exists because production builds drift through toolchain hotfixes mid-season — runtime divergence is a build-variance signal, not a sim-variance one.
- **Codegen control**: simulation assembly compiled without FP contraction or auto-vectorization of sim math (IL2CPP: disable Burst for sim assembly; verify emitted code has no FMA contraction; CI fails the build if cross-platform golden checksums diverge). Editor play-mode and device builds must pass the identical golden-replay suite before merge.
- **Separation**: kernel has zero Unity types; presentation interpolates at render rate.
- **Scope of determinism**: replays, ghosts, AI, rules, and **server-authoritative state** are deterministic and replay-verifiable; runtime desync checksums (snapshot checksums) are **same-architecture diagnostics only** (ARM clients vs x86 servers are not bit-comparable; server is canonical).

**Deterministic acceptance tests (§16.1, gating merge)** — each is a CI gate with a recorded golden checksum:

| ID | Test | Pass condition |
|---|---|---|
| AT-D1 | Fixed-tick independence | Same input sequence replayed through 60 Hz, 90 Hz and 120 Hz *render* frame schedules (tick sequence forced by input timestamps, not frames) yields identical state checksums |
| AT-D2 | Tick/render decoupling | Kernel state hash unchanged when the render loop is stubbed, throttled, or run 3× faster; no kernel call reads frame delta |
| AT-D3 | Input-rate independence | Identical logical inputs delivered at 30/60/120 input Hz, quantized to tick boundaries, produce identical checksums |
| AT-D4 | Golden replay suite | ≥20 scripted scenarios (§16.1) replay bit-identically on Editor + device builds and across x86/ARM |
| AT-D5 | Numerics contract | Unit tests pin round-half-to-even rounding, saturation detection, unit ranges, and transcendental iteration counts/tables |
| AT-D6 | RNG isolation | Kernel state hash unchanged when the render layer is seeded differently; kernel RNG consumes exactly the documented number of draws per tick |
| AT-D7 | Fixed-tick time-source and render-rate independence | Feeding an identical tick input sequence stretched/compressed across arbitrary wall-clock durations yields an identical state checksum; for the same tick sequence and same inputs at tick boundaries, rendering at 60/90/120 Hz (or with frame drops) produces identical kernel state checksums and identical authoritative outputs. The kernel ticks by tick count/input only, never reads wall-clock, deltaTime, or frame counters; it does not advance from render rate. |
| AT-D8 | Static build gate | Emitted-code audit: no FMA contraction / auto-vectorized sim math; build fails on drift |

### 4.3 Data-driven regulation profiles (D-3)

`RegulationProfile` (JSON/Addressables) holds: masses, PU limits, ERS curves, tyre compounds/sizes, aero modes, points tables, weekend formats, penalties, calendar. Switching profile = loading data. Engine code contains **no** season/year branching (mirrors the prior repo's documented philosophy — retained deliberately).

**Profile manifest (normative field list; every field is data, none is a code constant). Year-specific fields carry metadata: confidence (CONFIRMED/RATIFIED-CHANGE/REPORTED/ASSUMPTION/UNKNOWN), sourceRef (S-#), verifiedOn (ISO date), and sourceVerified (bool). Values with confidence==UNKNOWN must remain explicit placeholders (not copied from other years); zone/event parameters (designatedAccelZone, nonDesignatedZone, powerLimitedDistanceM, sectorAltCurveZones, etc.) for 2027/2028 have confidence=UNKNOWN until U-03/U-11 land.**

| Group | Fields |
|---|---|
| Identity/version | `profileId` (2026/2027/2028), `sourceIssues[]` (e.g. `C-ISS-20-2026-08-05`, `C-2027-ISS-03-2026-10-01`), `ratificationRef` (WMSC date), `schemaVersion`, `goldenChecksum` |
| Power-unit envelope | `iceMaxPowerKw[year]`, `fuelFlowMultiplier` (1.00/1.05/1.13 → cap 3000/3150/3390 MJ/h, derived), `mgukDeployCeilingKw[mode,year]` — normal 350/300/300 and Overtake 350/350/350 (mode- and year-dependent, never composed with `min()` — see D-84), `mgukHarvestCeilingKw[year]` (350/375/400), `mgukTorqueLimitNm` (500), `powerSplit` (53-47/58-42/60-40) |
| Deploy curves | `normalDeployCurve[]` (piecewise speed→kW table), `overtakeDeployCurve[]`, `overtakeFlatToKph` (337), `overtakeZeroKph` (355) — stored as interpolation tables, never formulas in code |
| Zone/parameter caps | `designatedAccelZoneKw` (350), `nonDesignatedZoneKw` (250), `boostCapKw` (+150 race), `superclipHarvestKw` (350), `qualifyingRechargeMj` (7), `eventsWithAltEnergyLimits` (12) |
| Energy budget | `esSocWindowMj` (4), `baseRechargeMjPerLap` (8.5), `overtakeRechargeBonusMj` (+0.5), `perSessionRechargeMj[]` (event data), `powerLimitedDistanceM`, `maxPuPowerReductionKwPerS` |
| Event parameters (per round, published ~4 weeks ahead) | `detectionGapS`, `detectionLineS`, `activationLineS`, `sectorAltCurveZones[]` (lap-distance windows + speed thresholds), `sectorReductionResetZones[]`, `scLowGripOvertakeDisabled` (bool) |
| Non-PU | masses, tyre sizes/compounds, aero zones, points tables, weekend formats, penalties, calendar rounds |

**Rules**: (a) every numeric field carries `confidence` + `sourceRef` (S-register id) + `verifiedOn`; (b) profile diffs are reviewed as content and re-run the regulation-conformance tests (§16.1); (c) adding a year is a data drop, never an engine change (N-08); (d) profiles with `UNKNOWN` fields must ship explicit placeholder markers, not silently inherited values (prevents 2028 drift).

### 4.4 Toolchain & CI (D-4)

- Unity 6.3 LTS; Addressables + CDN (mobile asset streaming); Git LFS for binary art.
- CI: schema validation of all gameplay JSON; **baked-data freeze**: AI lines and track fits (produced by FP tools) are checksummed and committed — regeneration requires a determinism golden approval, so offline tool FP variance cannot silently change kernel inputs; determinism golden replays; performance capture jobs on device labs; Markdown link check for this blueprint repo (docs-only tooling, not product code).
- Analytics: event schema reviewed with counsel (privacy manifest / Data Safety alignment).

---

## 5. Vehicle simulation specification

### 5.1 Regulation values (2026 profile defaults)

Values trace to FIA documents cited; "Reported" = secondary source. The profile is data; these are defaults. 2026 rows quote article text from **Section C Issue 20 (2026-08-05)** and **2026 PU Technical Regulations Issue 06 (2024-03-29)**; 2027/2028 rows carry the **WMSC-ratified (2026-06-23) power-unit package** published as the 2027 Section C series (Iss 01 2026-06-25 → Iss 03 2026-10-01). Consolidated 2027/2028 constants remain subject to the U-03/U-04 full-text check; zone-cap fields (designatedAccelZoneKw, nonDesignatedZoneKw) and any year-dependent zone/event parameters for 2027/2028 must be marked confidence=UNKNOWN, sourceVerified=false, with explicit sourceRef and verifiedOn until the consolidated text is read and article numbers cited (U-11). No silent inheritance from 2026.

| Parameter | Value | Confidence | Source |
|---|---|---|---|
| Minimum car mass | 768 kg | CONFIRMED | F1.com beginner's guide to 2026 regs (2026-03-04) |
| Car max width | 1900 mm (from 2000 mm) | CONFIRMED | Pirelli 2026 rules page (2026-02-12) |
| MGU-K max deploy | **350 kW (2026) → 300 kW (2027, 2028)**; Overtake ceiling stays 350 kW in all three years | CONFIRMED (2026) / RATIFIED-CHANGE (2027–28) | Section C Iss 20 Art. C5.2.6; WMSC 2026-06-23 + FIA/FOM package table (2026-06-10) |
| ICE max power | **~400 kW (2026) → 420 kW (2027) → 450 kW (2028)** | CONFIRMED (2026) / RATIFIED-CHANGE (2027–28) | F1.com 2026-06-10/2026-06-23; F1.com PU explainer (2024-06-06) |
| ES SoC window | ≤ 4 MJ | CONFIRMED | 2026 PU Regs (2024-03-29); Section C Iss 20 Art. C5.2.9 |
| Overtake bonus harvest | +0.5 MJ/lap on top of the 8.5 MJ base | CONFIRMED (article text) | 2026 Tech Regs Iss 20 Art. C5.2.10 iii: "Up to 0.5MJ of additional energy may be harvested in each lap subject to the conditions specified in Article B7.2" |
| Base harvest/lap | 8.5 MJ at the CU-K HV DC Bus; reducible to 8.0 MJ (event-determined), and to ≥5.0 MJ for SQ/Q where the FIA judges harvesting excessive | CONFIRMED (article text) | Section C Iss 20 Art. C5.2.10 + i/ii |
| MGU-K mechanical torque | ≤ 500 Nm | CONFIRMED (article text) | Section C Iss 20 Art. C5.2.11 |
| Deploy limit (absolute ceiling) | ERS-K DC power ≤ **350 kW** (all cases, 2026) | CONFIRMED | Section C Iss 20 Art. C5.2.6/C5.2.7; 2026 PU Regs Iss 06 Art. 5.4.6 |
| Deploy taper onset | Normal curve tapers from **290 km/h**; Overtake curve holds 350 kW to **337 km/h** (350 kW reached at 337.5 km/h on the 7100−20v line, so the flat segment binds) | CONFIRMED (derived from article text) | Section C Iss 20 Art. C5.2.8 i/ii; F1.com 2024-06-06 |
| Deploy limit (Overtake off) | **1800−5·v** below 340 km/h; **6900−20·v** for 340≤v<345; **0** ≥345 km/h | CONFIRMED (article text) | Section C Iss 20 Art. C5.2.8 i, verbatim |
| Deploy limit (Overtake on) | **7100−20·v** below 355 km/h (capped at 350 kW ⇒ 350 kW flat to 337 km/h, ≈200 kW @345, ≈100 kW @350, 0 @355); **0** ≥355 km/h | CONFIRMED (article text) | Section C Iss 20 Art. C5.2.8 ii, verbatim |
| Designated-zone cap (2026, from Miami) | MGU-K deploy **350 kW in designated acceleration zones** (corner-exit to braking point, incl. overtaking zones) and **250 kW elsewhere** | CONFIRMED (FIA statement; zone geometry is event data) | FIA "Refinements to the 2026 FIA Formula 1 regulations", 2026-04-20; consolidated into Section C Iss 20 |
| Boost cap (race) | +150 kW above the car's activation power level, or the current level if higher | CONFIRMED (FIA statement) | FIA 2026-04-20 refinements |
| Superclip (harvest at full throttle) | peak 350 kW (was 250 kW); qualifying recharge 8 → 7 MJ | CONFIRMED (FIA statement) | FIA 2026-04-20 refinements |
| Fuel energy flow | ≤3000 MJ/h above 10,500 rpm; below: EF = 0.27·N(rpm) + 165 MJ/h — cap scales with the ratified fuel-flow multipliers: ×1.00 (2026), ×1.05 (2027), ×1.13 (2028) ⇒ 3000 / 3150 / 3390 MJ/h (derived) | CONFIRMED (base) / RATIFIED-CHANGE (multipliers) | 2026 PU Regs (2024-03-29) Arts. 5.4.3/5.4.4; WMSC 2026-06-23 |
| Fuel | 100% advanced sustainable fuel | CONFIRMED | FIA 2026 PU doc; F1.com |
| MGU-H | removed | CONFIRMED | FIA 2026 PU doc |
| Active aero | full-time, front+rear flaps; Straight Mode (low drag) / Corner Mode (downforce) | CONFIRMED | F1.com beginner's guide (2026-03-04) |
| Tyre rim | 18 in; tread −25 mm front, −30 mm rear; diameter −15/−10 mm | CONFIRMED | Pirelli press (2025-12-09); Pirelli rules page (2026-02-12) |
| Compounds | C1–C5 + intermediate/wet (2026) — product ships fictional supplier name + H/M/S(/W/I) archetype names per §14.3 | CONFIRMED | Pirelli press (2025-12-09, 2025-11-24) |
| Supplier | Sole-supplier structure (Pirelli through 2028) — product ships fictional supplier | CONFIRMED | FIA/F1/Pirelli (2026-06-11) |
| ICE/PU min masses | ICE ≥130 kg; PU ≥185 kg | CONFIRMED | 2026 PU Regs (2024-03-29) |

**Power-unit / ERS trajectory 2026–2028 (WMSC-ratified 2026-06-23; FIA/FOM package table 2026-06-10)** — RATIFIED-CHANGE, one row per profile year. The 2026 column is primary-verified against Section C Iss 20 article text; the 2027/2028 columns are ratified and published (2027 Section C Iss 03, 2026-10-01) but their consolidated constants stay subject to the U-03/U-04 full-text cross-check before production.

| Parameter | 2026 | 2027 | 2028 |
|---|---|---|---|
| ICE max power | 400 kW | **420 kW** (+5 % fuel flow) | **450 kW** (+13 % fuel flow) |
| MGU-K max deploy (normal) | 350 kW | **300 kW** | 300 kW |
| MGU-K max deploy (Overtake Mode) | 350 kW | 350 kW | 350 kW |
| Max harvesting power | 350 kW | **375 kW** | **400 kW** |
| Power split ICE/MGU-K | 53/47 | **58/42** | **60/40** |
| Deploy speed curves | C5.2.8 i/ii (1800−5v; 6900−20v; 7100−20v) | carried over 2026 formula family, ceiling 300 kW — **consolidated constants to re-verify (U-03)** | same, ceiling 300 kW |
| Designated acceleration zone / elsewhere | 350 kW / 250 kW | carried forward as profile data | carried forward as profile data |

Secondary reporting disagrees on the 2026 harvesting baseline (motorsport.com 2026-06-11 states "375 kW, up from 250 kW"; the FIA/FOM table and ESPN state 350 kW). **The FIA/FOM table is used** (350 → 375 → 400) and the discrepancy is logged in SOURCES.md as a tier note, not silently dropped.

### 5.2 Kernel architecture

Modules: chassis (rigid body + 4-wheel load transfer), tyre (brush-based combined-slip with load sensitivity, thermal + wear state), aero (CoF/CoD maps × aero-mode blend with flap transition timing), powertrain (engine torque map, 8-speed gearbox, MGU-K deploy/harvest controller per F-03), brakes (pedal→torque, temp, fade), fuel (mass burn + flow cap), collision (swept-sphere car-vs-track surfaces **and** car-vs-car contacts), track interface (surfaces, banking, active-aero zones, detection/activation lines).

**Two-layer split** (required by §10 netcode and §4.2 determinism):
- **Chassis core** (rigid body, tyres, aero, powertrain mechanics, car-vs-car contacts): the only layer eligible for client prediction.
- **Authoritative ledger** (energy budget/harvest accounting, tyre thermal/wear, fuel, session rules, penalties, classification, Overtake gating): server-owned; clients render server values; corrections smoothed over ≤300 ms; reconciliation of own-car chassis replays pending inputs through the chassis core only.

**Contact model spec**: swept-sphere broadphase with deterministic ordering (sorted by entity id + contact id); fixed solver iteration count (4) with fixed-point accumulators; contact caching disabled (no warm-start persistence across steps) to prevent history-dependent divergence; 22-car contact soak test is golden-replay mandatory.

**Model class**: gameplay-simulation (arcade-leaning-SIM): physically structured, data-tuned to regulation-consistent lap-time targets. Validated against published top-series performance envelopes: top speed ≥340 km/h on low-downforce ("power track") archetypes, 0–100 km/h ≈2.6 s, 0–200 ≈5.5 s, downforce envelope ~1500–2500 kg at power-track speeds — **ASSUMPTION** targets for tuning validation, not regulation facts, and no real-world circuit is referenced by name (see §8.2/§14.2).

### 5.3 Tyre model

Brush/Stijn model with Pacejka-style lateral force curves per compound (research codes C1–C5; product uses fictional H/M/S archetype names per §14.3), temperature window, wear accumulation per slip energy, surface effects (track evolution "rubber-in"), kerbs/run-off grip. 2026 width/diameter reduction ⇒ slightly lower contact patch ⇒ characteristic oversteer-on-entry bias (derived design note from Pirelli comments on narrower tyres — REPORTED).

### 5.4 Overtake mode (F-03) — simulation contract

Per 2026 Sporting Regs Art. B7.2 (Section B, Iss 09, 2026-10-01) and Tech Regs C5.2.8/C5.2.10 (Section C, Iss 20, 2026-08-05):

1. Each track defines a **Detection Line** and **Activation Line** (game-track data; real-world analogues are event-published by the FIA — research reference: a mid-season 2026 event used Detection Gap 1.0 s with detection/activation lines in the final-corner sequence — REPORTED via Total Motorsport 2026-06-26).
2. At the Detection Line, if gap to car ahead < **Detection Gap** (event value; default 1.0 s), the player's Overtake is **armed** and activates at the Activation Line, remaining active until the next Detection Line where the gap exceeds it.
3. Active Overtake grants: harvest cap +0.5 MJ that lap (**8.5 → 9.0 MJ**, Art. C5.2.10 iii — CONFIRMED article text) and the alternate deploy curve (350 kW flat to 337 km/h vs taper from 290 km/h). Advantage opens at **290 km/h** (normal curve begins tapering there; for 2026 both curves identical below; for 2027/2028 normal is capped at 300 kW so equality is year-dependent), peaks ~+235 kW near 337 km/h, and persists to 355 km/h.
4. **Disabled for all cars** under Safety Car (restored only after every car has passed the line post-SC) and in **Low Grip Conditions** (wet). Leader (no car ahead in relevant gap context) gets no Overtake; defenders counter with manual **Boost** (stored-energy deployment, usable anywhere).
5. In practice/qualifying ("lap time classified sessions") Overtake is enabled+active for all, no proximity test (per B7.2.3b as described in secondary analysis — REPORTED; real-world 2026 practice/qualifying runs on the Overtake curve — REPORTED via Total Motorsport 2026-06-26). Per-event FIA documents may also set **session energy allocations** (research reference: qualifying 6.0 MJ; FP/out-laps 8.5 MJ at a 2026 event — REPORTED) and power-limited periods; these are profile data.
6. Detection Gap, line positions, session allocations, and SC/low-grip toggles are **profile data** so post-2026 tweaks (the FIA explicitly retained levers — Motorsport.com 2026-01-08) are content updates. Mid-2026 real-world changes (automated qualifying outlap deployment; scenario-specific 250 kW limits — REPORTED via ScuderiaFans 2026-05-19) are modeled as profile toggles, demonstrating why rules must not be code.

**MGU-K deploy envelope (normative)** — the rules do not apply a single ceiling: the permitted ERS-K DC power is the **minimum of four independent, independently-parameterized limits** (P_ceiling(mode,year), P_speedCurve(v,mode), P_zone(v,s), P_powerLimitedPeriod(v,s)). Boost (+150 kW over the activation power level in race conditions) is a driver-initiated overlay, not a ceiling; Overtake ceiling is mode- and year-dependent (350 kW for Overtake in 2027/2028), so it must not be composed as a naive min with normal year ceilings. Evaluated every sim tick (each is a profile/event field, so the profile is the only thing that changes between 2026/2027/2028 and between events):

```
P_allowed(v, mode, s) = min(
    P_ceiling(mode, year),     # mode- and year-dependent absolute cap (see table below)
    P_speedCurve(v, mode),     # 1800−5v (<340) / 6900−20v (340–345) / 0 (≥345)   [Overtake off]
                              # 7100−20v (<355, capped at 350) / 0 (≥355)          [Overtake on]
    P_zone(v, s),              # 350 kW inside designated acceleration zones (corner-exit→braking point,
                              #   incl. overtaking zones); 250 kW elsewhere         [2026 profile; event geometry]
                              # 2027/2028 zone caps NOT consolidated-text-verified (R-04/U-03/U-11). Carry-forward of 350/250 kW is an ASSUMPTION;
                              #   profile marks these confidence=ASSUMPTION, sourceRef=U-11, verifiedOn=null; unknown => UNKNOWN placeholder, not silent inheritance.
    P_powerLimitedPeriod(v, s) # event-declared power-limited sectors: reduced envelope + rate limits
)

P_ceiling(mode, year): 2026 → normal 350 kW / Overtake 350 kW
                    2027 → normal 300 kW / Overtake 350 kW
                    2028 → normal 300 kW / Overtake 350 kW
```

Critical subtlety (D-84): the **Overtake ceiling does not inherit the normal year ceiling**. The ratified package lowers *normal* deploy to 300 kW from 2027 while explicitly keeping Overtake Mode at 350 kW, so the cap must be looked up per mode — a naive `min(yearCeiling, overtakeCeiling)` would wrongly clamp 2027 Overtake to 300 kW. `Boost` (+150 kW over the activation power level, race conditions) is a **driver-initiated overlay** on deploy, not a further reduction of the ceiling, and stays available when Overtake is disabled (SC / Low Grip Conditions).

Interactions the kernel must implement and the tests must pin:

- **Speed curve × year ceiling**: the ceiling is the binding term at low speed in 2027/2028 (300 kW normal), while in 2026 the *speed curve* binds first (350 kW is reached at 290 km/h). The simulator must not hard-code either; the profile supplies both tables, and the ceiling lookup must be mode-aware (see D-84 above).
- **Speed curve × Overtake**: below 290 km/h the two curves are identical (both ≈1800−5v), so Overtake confers **no** advantage below the taper onset; the advantage band is 290 → 355 km/h, peaking ~+235 kW at 337 km/h. HUD must not display an advantage below 290 km/h.
- **Overtake × year**: in 2027/2028 Overtake still reaches 350 kW while the normal ceiling is 300 kW; the curves diverge at low speeds (no equality below 290 km/h as in 2026) and the Overtake advantage band widens at mid-range speed (the whole 350 kW curve is available). AT-R2 and AT-R4 must assert year-specific branches using profile data.
- **Overtake × zone cap**: when Overtake is active but the car is *outside* a designated acceleration zone, the 250 kW zone cap binds — Overtake is not a blanket upgrade. Zone membership is lap-distance/speed-threshold event data (per-event "Power Unit Information" docs publish the sectors).
- **Overtake × power-limited period**: in a declared power-limited sector the FIA may permit a *greater* initial reduction and a reset of the reduction in named sectors (event data); the envelope is reduced, not raised. Penalties for exceeding the allowed power-reduction rate (kW/s) are ledger-level, not chassis-level.
- **Boost × Overtake**: Boost is the defender's counter and is capped at **+150 kW** over the activation power level in race conditions; Overtake is disabled under SC and Low Grip Conditions, but Boost is not (it is the safety valve against power drop-off).
- **Event parameters**: Detection Gap, line positions, sector alternative-curve zones, Power Limited Distance, max PU power-reduction rate, and per-session recharge allocations are all **per-round data** published ~4 weeks ahead. The product mirrors the *structure* (archetype-generated), never the real events (U-09).
- **Energy budget coupling**: deploy is additionally limited by the SoC window (≤4 MJ) and per-lap recharge cap (8.5 MJ base, 9.0 MJ with active Overtake, event-reducible to 7 MJ in qualifying / ≥5 MJ in SQ-Q); the deploy controller must be energy-aware so it cannot "borrow" energy the ledger will not allow.

**Acceptance tests (regulation conformance, §16.1)** — table-driven against the ratified profile; each test must fail if any single limit is dropped:

| ID | Test | Pass condition |
|---|---|---|
| AT-R1 | Curve vs article text | Reconstructed curve matches Section C Iss 20 C5.2.8 i/ii at 0/200/290/337/340/345/350/355 km/h within 1 kW |
| AT-R2 | Mode-aware ceiling switch | With only the year set to 2027 (no code change): peak normal deploy = 300 kW **and** peak Overtake deploy = 350 kW; a naive `min(year, overtake)` implementation that returns 300 kW in Overtake fails this test |
| AT-R3 | Zone cap interaction | At 250 km/h inside a designated zone → 350 kW; at 250 km/h outside → 250 kW; with Overtake active outside a zone → still 250 kW |
| AT-R4 | Overtake advantage band | **2026 profile (ceilings equal at 350 kW)**: Normal vs Overtake deploy equal below 290 km/h; maximum advantage ≈235 kW at 337 km/h; both reach 0 at their cutoff speeds (345 km/h / 355 km/h). **2027/2028 profiles (ceilings differ, 300 kW vs 350 kW)**: equality below 290 km/h does **not** hold (normal capped at 300 kW, Overtake at 350 kW); curves are reconstructed from profile data (speed curve × mode-aware ceiling). Zone caps for 2027/2028 are not consolidated-text-verified (U-11) and must be treated as UNKNOWN placeholders. |
| AT-R5 | Energy ledger | Per-lap recharge ≤ 8.5 MJ base, ≤ 9.0 MJ with active Overtake, ≤ 7.0 MJ when the event sets a qualifying limit; SoC window never exceeds 4 MJ; no deploy while SoC at minimum |
| AT-R6 | Power-limited period | In a declared power-limited sector the envelope and reduction rate follow the event table; exceeding the rate is flagged by the ledger |
| AT-R7 | Boost cap | Race Boost adds ≤150 kW over the activation level (or the current level if higher); Boost remains available under SC/Low Grip when Overtake is disabled |
| AT-R8 | Profile-only change | Changing any of the above via profile data passes all other conformance tests unchanged (proves no year/event constants in code) |

**Exploit defenses**: gap measured server-side from official classification timing; activation requires crossing the line (no "hovering"); energy ledger audited per lap; zone membership and power-limited periods evaluated server-side from authoritative lap distance; all state in replays.

### 5.5 Damage & failure

Detachable-bodywork and tyre punctures (simplified), gearbox damage on misses, drive-through/penalty interplay handled by rules engine (§7.4). Player-facing "damage" model is arcade-sim: performance deltas, no tear-down UI (scope control).

---

## 6. AI & opponents (F-04)

1. **Racing line**: offline-optimized line per track (curvature/speed solver on track graph with grip model from §5), stored as track data; AI follows line with per-driver offsets (defensive/attacking variants). **AI executes inside the deterministic kernel** (server-side AI for sub-22-human grids, replay/ghost fidelity) under the same fixed-point determinism rules as physics; AI randomness seeded from kernel RNG only.
2. **Controller**: pure-pursuit steering + lookahead speed governor from grip ellipse (downforce-dependent lateral accel limit, aero-mode aware), brake-point model with error terms.
3. **Energy & tyres**: per-driver deploy/harvest policy (deploy maps keyed to segments; Overtake-mode aware), pit-window optimizer (tyre deg × fuel effect × traffic), wet-weather competency parameter.
4. **Behavior**: staged overtaking (draft + Overtake-mode attack, defensive line covering, late-brake dive with mistake probability), respect/collision avoidance, safety-car/low-grip behavior, blue-flag/yellow-flag compliance, track-limits policing.
5. **Difficulty**: 8 tiers from "Rookie" to "Simulation"; parameters: pace (s/lap spread ±0.0–1.5 around base), mistake rate, aggression, strategy competence. Rubber-banding **off by default** (data flag; competitive integrity §10.5).
6. **Validation**: AI speed distribution must bracket human reference laps; no impossible sectors; pit strategies vary across tyre allocations; soak-tested for 24-race seasons without degenerate strategies.

---

## 7. Race systems & rules engine (F-02)

### 7.1 Weekend formats (2026/2027 profile data)

- Conventional: FP1/FP2/FP3 (1 h each; FP3 ≥18 h after FP2; race control may extend disrupted FP — Iss 09), Qualifying **Q1 18′ / Q2 15′ / Q3 13′** (7′ breaks; Q3 duration 13′ per Section B — REVIEWER-verified against Iss 06/07 text; re-verify at production), race.
- Elimination: Q1 and Q2 each eliminate the **slowest six** (22-car basis; scales with field size per B2.4.2/B2.2.3).
- Sprint ("Alternative Format"): FP1 1 h; **SQ 12′/10′/8′** (7′ breaks; eliminate 6, 6 — 22-car basis, scales with field size); Sprint ≥100 km, 1 h limit (+1.5 h max incl. suspensions); grid from SQ with penalty application order.
- Race distance: **least whole laps exceeding 305 km** (street archetype 260 km); Iss 09 replaced the fixed 3-hour cap with an **event-specific hard-stop time** (≥2 h green-flag racing preserved) — profile field.
- **Tyre session mandates** (profile flags; real-world 2026: SQ1/SQ2 new medium, SQ3 soft — REPORTED): qualifying-segment compound mandates + dry-race ≥2-compound rule (Article B6 family, Iss 09).
- **Parc fermé**: two lockdown windows per weekend (post-SQ→Sprint, post-Q→Race — profile data); changes during lockdown → pit-lane start (simplified consequence, data flag).
- **Scrutineering/weighbridge**: post-session legality checks (min mass 768 kg per §5.1); failure → DSQ with classification table effects (§7.2). Simplified presentation (no garage-return flow).
- **PU element allocation** (Article B8.2 family, Iss 09): per-driver season component-usage ledger + simplified usage-grid penalties (career mode surface).
- Pit stops: pit-lane speed limit (profile), minimum pit-stop time, release procedure (no release into traffic path), tyre-change rules (all four), penalties applied via §7.4.
- Tyre allocation (Article B6 family): dry sets per driver (profile; real-world allocation ~13 dry sets — REPORTED), mandatory use of ≥2 dry compounds in a dry race (profile flag), intermediate/wet as needed.
- Formats, durations, distances, grid procedures are data (Section B, Iss 09, 2026-10-01; Section A Iss 03).

### 7.2 Classification & points (data tables)

- Race points ≥75%: 25-18-15-12-10-8-6-4-2-1; reduced-distance columns (≥2 laps / ≥25% / ≥50%); **no fastest-lap point** (removed from 2025 — FIA WMSC 2024-10-17, CONFIRMED).
- Sprint points (≥50%): 8-7-6-5-4-3-2-1; additionally requires leader to complete ≥2 consecutive green laps (no SC/VSC) — Section A Iss 03 A2.2.2.
- Classification ≥90% distance; 107% qualifying rule with steward discretion entry; dead-heat splitting; count-back on wins/podiums.
- Championship: max 24 competitions, ≤24 cars, 2 per team (Section A, Iss 03).

### 7.3 Starts, SC/VSC, red flags

Standing starts (5-light sequence, jump-start detection with penalty integration per §7.4), formation-lap procedures (aborted start, extra formation lap — profile flags), Safety Car (with 22-car procedural order, lapped-car handling **simplified** — data flag), VSC (delta-time enforcement), red-flag suspension/resumption with partial-points rules and **simplified grid re-forming** (cars line up in current order from pit-lane/grid per profile; unlapping before restart simplified to one-lap catch-up window — data flags), pit-lane closure during red flags (profile), release & pit speed limits, blue-flag/lap-down handling, DNF/DNS handling. All flags are profile parameters so regulation drift (Iss 09 hard-stop, FP extensions) is a data change.

### 7.4 Penalties & stewards (player-facing)

- **Catalogue (2026-profile defaults)**: 5 s / 10 s time penalties; drive-through; stop-go (simplified to time-equivalent); grid drops; reprimands (5 → 10-place drop).
- **Values by offence (REPORTED, 2026 Penalty Guidelines)**: jump start 5 s (minor) / 10 s / DT / 10 s stop-go by margin; track-limits strikes → lap deletion (Q) or 5 s (race); causing collision 5–10 s / DT; ignoring flags 5 s / DT. Penalties unservable under SC/VSC (carried); within final 3 laps (time race) → converted +20 s / +30 s equivalent.
- **Serving state machine**: penalty accrues → served at pit stop or added post-session; grid penalties resolve at final grid (1 h before formation lap) with application order (SQ/Q result → penalties → unserved carry-over).
- **Stewards**: automated incident detection (rules engine) + on-demand player review; evidence from server ring buffer (§10.1) when online, kernel replays offline.
- Fictional "Stewards' Office" to avoid FIA branding.

### 7.5 Weather & track evolution

Dynamic weather model (rain intensity, drying line), Low-Grip Conditions flag (gates Overtake off), tyre compound switching (dry/intermediate/wet), track rubber-in evolution affecting grip.

### 7.6 Telemetry, HUD, timing

Sector times, gaps, ERS/battery SoC, tyre temps/wear, fuel, Overtake status, active-aero mode, blue/yellow flags, delta-to-best; broadcast-style overlays optional (off by default for perf).

### 7.7 Rules engine properties

Deterministic, replayable, unit-testable rule table (state machine per session); all thresholds data-driven; no hard-coded 2026 constants in code.

---

## 8. Tracks & content pipeline (F-05)

### 8.1 Track data format

Centerline spline (arc-length parameterized) + width function + elevation + banking + camber; surface map (asphalt/kerb/run-off grip), active-aero zone list (start/end + mode semantics), **Overtake Detection/Activation lines**, pit lane (entry/exit, speed limit, box positions, release rules), grid slots (22, scaling), start gantry + lights, marshal flag zones, camera spline set, LOD/art bounds, AI line + speed profile, sector/sector-time checkpoints, weather sensitivities, time-of-day variants.

### 8.2 Authoring pipeline

**Original-content pipeline (IP-safe by construction)**: procedural track generator (archetype parameters: length, corner mix, elevation, banking) → spline fitting/editing tool → AI-line solver → validation harness (line continuity, banking sanity, pit-release legality, Overtake-line placement) → tuning passes vs AI reference times. **No survey-lidar/satellite import of real circuits; no circuit-derived geometry; every track carries a provenance log entry** (generator seed + editor actions) for IP clearance (§14.2). Reference to real-world calendars exists only in research dossiers (R1), never as build inputs.

### 8.3 Calendar (content roadmap, not engine logic)

| Season | Rounds | Notes |
|---|---|---|
| 2026 profile | 24 fictional rounds (R1 "Season Opener" → R24 "Finale") | Weekend archetypes mirror the confirmed real-world 2026 calendar structure (FIA/FOM 2025-06-09): season-opening flyaway, Middle East rounds (seasonally scheduled per region), consolidated European leg, Americas triple-header, night finale; a debut-round archetype (homologation-conditional in reference data) is included |
| 2027 profile | 24 fictional rounds | Announced real-world structure (FIA 2026-09-16): pre-season Feb, spring flyaway opening, European leg with new host venues, expanded Sprint program (10 events vs 6), late-season flyaways |
| 2028 profile | placeholder data slot | **PU step ratified** (WMSC 2026-06-23): ICE 450 kW, MGU-K deploy 300 kW, harvest 400 kW, split 60/40, fuel flow +13 % — all profile data. Calendar and non-PU regulations **UNKNOWN** as of 2026-10-12; Pirelli supply confirmed through 2028 |

Round-level Sprint flags are per-round data (2026: 6 Sprints; 2027: 10 Sprints — CONFIRMED real-world counts; fictional rounds carry equivalent flags).

Launch content target: **12–16 fictional circuits** across archetypes at 1.0; remainder via seasonal content (§15).

### 8.4 Circuits must-express the 2026 rule set

Every track definition must include active-aero zones, Overtake lines, pit-lane rules, and SC/low-grip semantics so regulation fidelity is a *track property*, not special-case code.

---

## 9. Career & meta progression (F-06)

- **Driver career**: create driver → junior series options (optional, DLC-later) → F1-equivalent seat; results drive contract market; performance generates R&D points; rivalries and narrative beats (light-touch).
- **Team career**: two-car team, staff upgrades (strategists, engineers), budget from sponsorship + results, regulation-era planning (2026→2027 development reset schedule as data).
- **Season structure**: 24 rounds incl. sprint weekends, championship points (Tables §7.2), driver market, cost-cap abstraction (data: budget over season).
- **Meta**: ghosting/time trials (deterministic replay of own best), daily/weekly challenges, liveries (cosmetic IAP only), cross-progression via account (§10.4).
- **Progression economy**: deterministic, no random rewards; unlocks via performance milestones (aligns with §13 no-loot-box stance and regional law).

---

## 10. Multiplayer architecture (F-07)

### 10.1 Model choice (Decision D-5): dedicated authoritative servers + prediction/reconciliation/interpolation

**Rationale**: 22-car races make full-state rollback uneconomic (resim window × sim state × misprediction rate — see Netcode dossier); racing demands cheat resistance for leaderboards; cross-platform (mobile + PC) rules out host-authoritative P2P (host advantage, NAT asymmetry, mobile backgrounding). Industry pattern for vehicle games: client-side prediction of own car with server reconciliation; remote cars interpolated/extrapolated from authoritative snapshots (kartphysics UE5 replication post 2026-09-28; HDM Stuttgart racing-architecture study 2023-02-27).

- Tick: server 60 Hz; clients render interpolated at display rate.
- Wire: UDP. **Transport plan**: Steam Datagram Relay for Steam clients only after Valve confirms cross-platform (mobile+PC) SDR eligibility for this title (Valve SDR terms partner-gate some cross-platform use — gate at M2); otherwise self-hosted relay nodes (multi-provider cloud, anycast DNS) with fallback direct UDP. QUIC fallback for restrictive mobile networks. **Relay-first design** (no hole-punching dependency) for NAT robustness.
- Uplink: input-only ~64–100 B/packet, 60/s ≈ 6 KB/s per client. Downlink: delta-compressed snapshots with the tiered send plan below (~15–25 KB/s per client — ASSUMPTION validated in M2 load tests).
- **Bandwidth plan (corrected)**: 22 cars × 40 B × 60 Hz = 52.8 KB/s raw is too high; shipping plan: **near cars** (within ~60 m + any in contact) at 30 Hz with 24 B quantized deltas; **mid ring** at 15 Hz; **far cars** (positions + heading only) at 10 Hz; events/penalties as reliable messages. Aggressive interest management + quantization targets **≈15–25 KB/s downlink** (ASSUMPTION — validated in M2 load tests; tiered plan is the mechanism that makes the budget achievable).
- Clock sync: server-authoritative tick number in every packet; clients estimate offset via **min-RTT-filtered** RTT/2 sampling (handles asymmetric routes; Kalman-style smoothing), with drift disciplining; interpolation delay = jitter buffer (3 ticks) + 1 tick, i.e., remote cars render ~4 ticks (66 ms) stale plus RTT; delta snapshots carry an ack base-frame id; redundancy/bitrate adaptation under loss (FEC-lite: duplicate critical fields every 4th snapshot).
- Own car: **chassis core only** predicted locally (per §5.2 split); on snapshot, reconcile (snap + replay pending inputs through chassis core; energy/tyre/rules values accepted from server and smoothed ≤300 ms).
- Remote cars: interpolate between snapshots; collision-relevant extrapolation bounded (≤150 ms), flagged in HUD during high-jitter.
- **Contact reconciliation**: client keeps a 4-tick remote-chassis history buffer; when reconciling own car against a remote contact, the contact is re-resolved with the server's authoritative remote state at the same tick (snap-before-contact replay); residual divergence ≤ collision epsilon is smoothed, larger divergence snaps. Clients never resolve contacts authoritatively — server values win.
- **Input-delay clamp (revised)**: inputs older than (estimated RTT/2 + 6 ticks + buffer) are dropped — tied to the connection estimate, not a flat 8 ticks; drop rate logged as a soak metric (§16.2).
- **Mid-session join/backfill**: joining players receive lobby state snapshot, take the next free grid slot (or AI slot on release), and — for championship modes — enter scoring with a backfill-eligibility flag (data flag; default: backfilled humans score but are visibly marked). Matchmaking pools sized per region with bot-fill targets.
- Batteries/energy, penalties, classification, Overtake gating: **server-owned**. Clients send intent; server decides outcome (never trust client).
- Determinism: server is canonical (fixed-point kernel); runtime snapshot checksums are **same-architecture diagnostics only** (client↔client) — ARM mobile clients never bit-compare against x86 servers; divergence beyond tolerance triggers full-state resync (snapshot size budget ≤ 12 KB — ASSUMPTION to validate).
- **Reconnection/rejoin**: 60 s grace after disconnect (mobile backgrounding/lock screen); car transitions to server-side AI control immediately; on return, full-state resync + catch-up at replay speed (≤2 s) with invulnerability window 1.5 s (data flag). Lobby backfill and bot-fill for quitters (matchmaking requirement).
- **Incident evidence**: server retains rolling 15 s ring buffer of authoritative snapshots + inputs per car; penalties/appeals resolve against server truth, with a player-facing incident-replay view (server-rendered positions, not client memory) — explicitly closes the "client-stale-view" gap (remote cars render ~1 RTT stale).

### 10.2 Grid sizes & scalability

Default online race: 22 cars (regulation parity); Quick Race modes at 8/12/22. Server: one 60 Hz authoritative process per lobby (CPU-bound, no GPU); autoscale lobby fleet per region.

**Server operations & cost model** (closes D-10 gap): per-lobby CPU budget ≤6.0 ms/tick (ASSUMPTION, M2-measured); target ≥8 lobbies per 4-vCPU node; per-concurrent-player cost model built from soft-launch telemetry (U-08). **DDoS/abuse hardening**: rate limits per IP/account, relay-node fronting (absorb volumetric attacks at relay layer), region autoscale policy, lobby-fleet isolation by region, abuse-report pipeline. No custom kernel-level edge infra at launch (cloud provider WAF/DDoS in front).

### 10.3 Regions & latency

EU, NA-East, NA-West, SA-Brazil, Asia-East, Asia-SE, MENA (ASSUMPTION: provider footprint; final per partner). Target p95 RTT ≤120 ms within region; telemetry dashboards per region. **N-03 acceptance metric**: ≤150 ms effective input-to-response on the predicted car at 200 ms RTT, measured in the emulation suite (§16.2); remote-car interpolation lag ≈ RTT + 4 ticks by design and is HUD-flagged.

### 10.4 Accounts & cross-play

Single identity service: device accounts (Apple/Google) + Steam OpenID linkable at any time; cross-progression (single account, shared cloud save with **save-versioning, last-writer-wins + merge conflict policy, 24 h offline-divergence grace, and entitlement sync across linked platforms**); display names global-unique with moderation; friends via platform APIs + in-game; parental/consent gates for UGC features per §14.4.

### 10.5 Anti-cheat & competitive integrity

No kernel anti-cheat (Deck/Proton + mobile compatibility; Proton incompatibility is a Steam Deck Verified failure mode — Valve docs). Posture:
- **Server validation**: input plausibility (magnitude/rate clamps, impossible-state detection), energy ledger audit per lap, sector-gate position checks, Overtake-line arming audit.
- **Input-delay bounds**: reconciliation inputs are dropped when older than (estimated RTT/2 + 6 ticks + buffer) — the connection-tied clamp defined in §10.1 (anti input-delay peeking); drop rate is a logged soak metric.
- **Replay verification protocol**: leaderboard submissions are input logs; the server **re-simulates** them on the canonical kernel and compares trajectory checksums (deterministic re-run, ~22-car race ≈ server-CPU cost TBD in M2). A forged client checksum proves nothing — verification is re-simulation, not checksum trust.
- **Device attestation** for mobile (Play Integrity / DeviceCheck) on leaderboard-eligible sessions; failure → unranked-but-playable.
- Until the protocol ships: global boards marked **"unverified"**, ranked boards gated behind verification at launch+1 season.
- Cheat reports carry server evidence; anomaly detection + statistical review; ban queue with appeal path.

### 10.6 Offline/async

Full offline single-player + ghosts (deterministic replays = ghost cars). Weekly leaderboards operate on server-verified replays (§10.5); daily challenges async.

---

## 11. Platform targets & performance (N-02)

### 11.1 Tiers & targets (all ASSUMPTION-tier targets to be validated in vertical slice)

| Platform | Tier | Target | Notes |
|---|---|---|---|
| Steam (PC) | Minimum | 1080p60 fps, Low, GTX 1050 Ti / RX 570 class, 8 GB RAM | HDD OK |
| Steam (PC) | Recommended | 1440p60 High, RTX 2060 / RX 5700 class, 16 GB RAM | |
| Steam Deck | Verified gate | 30 fps @ 1280×800 default, stable frame pacing, Proton/SteamOS OK, no incompatible AC | Valve Deck Verified checklist |
| Steam Machine | Verified gate | 30 fps @ 1080p | Valve (GDC 2026) |
| iOS | Minimum | iPhone 11 (A13) 60 fps Medium 30 fps fallback low-end; iPad 9th gen | thermal throttling governance |
| iOS | Recommended | iPhone 13 (A15)+ 60 fps High | |
| Android | Minimum | Snapdragon 865-class 60 fps Low (30 fps fallback tier below) | |
| Android | Recommended | Snapdragon 8 Gen 2-class 60 fps High | |

### 11.2 Budgets

- Simulation: 60 Hz; kernel cost budget ≤2.0 ms CPU/tick on recommended tiers (21 cars + AI + rules) — **M1 kill-criterion**: if the fixed-point kernel (no SIMD) exceeds 2.0 ms/tick in the vertical slice, descope grid size or revisit engine before further investment. Server-side: separate budget ≤6.0 ms CPU per lobby per tick (22 cars + AI + rules) on the chosen cloud shape (ASSUMPTION; measure in M2).
- Render: URP; on-tile post-processing for tile GPUs (iOS/Android); dynamic resolution scaling (0.65–1.0 render scale); LOD policy (3 car LODs, crowd 2), texture streaming; shader LOD.
- Memory: PC min 8 GB system / GPU 4 GB; mobile per-tier ceilings with OS-jetsam headroom: iOS minimum tier (A13/4 GB) app budget **≤1.1 GB** (jetsam-safe), recommended tier ≤1.8 GB; Android minimum (SD865-class) ≤1.3 GB, recommended ≤2.2 GB; Deck 64 GB storage note (install ≤ 2.5 GB total acceptable; ≤1.5 GB preferred); memory-pressure/OOM tests in §16.3 lab matrix.
- Thermals: frame governance (Unity Adaptive Performance thermal providers — feature availability on pinned LTS per §4.1 note), battery-saver presets, background suspension safety (pause on mobile; online rejoin per §10.1), thermal soak test suite.

### 11.3 Frame pacing, input latency & measurement gates

- Input sampled and quantized **at the 60 Hz kernel tick** with fixed tick timestamps (never at render rate — replay determinism requirement); display refresh (60/120 Hz) only affects rendering.
- Fixed-step sim decoupled from render; steering pipeline ≤1 frame on high-end.
- **Acceptance metrics (per tier, gating)**: sustained fps = 95th percentile of 1-second averages over a 10-minute soak ≥ target; p99 frame time ≤ 1.5× target frame budget; zero hitches >100 ms per 10-min soak; input-to-photon ≤50 ms on recommended PC (ASSUMPTION target); cold/warm load-to-race ≤90 s cold / ≤30 s warm on mobile (ASSUMPTION).
- **Force feedback / haptics**: FFB loop latency (physics→torque command) ≤20 ms on PC wheel setups (ASSUMPTION); mobile haptics ≤30 ms; FFB torque update at ≥60 Hz on recommended PC.
- Battery: internal target ≥2.0 h continuous racing at default config on Deck and on recommended mobile (studio target, **not** a Valve criterion — restated per review; Valve publishes no battery-duration bar).

---

## 12. Input, accessibility, UX (F-08/F-09)

- Schemes: wheel+pedals (FFB, 900°, per-car steering lock), gamepad, keyboard, touch (tilt-steer, tap-steer, virtual pedals, gyro fine-steer), all remappable.
- **Age-gate UX**: store-page age declarations per platform; first-run age-eligibility statement (ECA Digital-compliant for Brazil: reliable age verification over self-declaration — vendor TBD at M3, U-list); parental-consent flow where account age < threshold; no UGC/social for unverified minors.
- Assists: steering, braking, traction, active-aero assist (auto mode switching), ERS auto-deploy, racing line, ideal-line speed, ABS/TC pseudo-ABS, damage off; assists record on results (integrity).
- Accessibility: colorblind presets (flag/tyre/ERS colors), full remap, text scaling (Deck legibility ≥9 px @1280×800 min, 12 px recommended — Valve), screen-reader support (Unity 6.3 accessibility), haptics tuning, difficulty granularity, reduced-motion camera options, subtitles/captions for team radio.
- Localization: externalized strings; launch languages (ASSUMPTION): EN, FR, DE, ES, IT, PT-BR, JA, KO, RU, PL, TR, NL. **Chinese locales deprioritized** while China is geo-blocked (no ISBN path at launch). Korea localization eases GRAC/rating handling and market access.

---

## 13. Commercial & monetization (F-10)

### 13.1 Pricing (ASSUMPTION, to be validated vs. comp set)

- Steam premium: **$34.99–39.99 / €34.99–39.99** launch price; regional pricing per Steam tiers; PPP-aware regional pricing for priority markets (PT-BR).
- Mobile premium: **$9.99–14.99** one-time purchase. Mobile premium racing is a weak category (no comp evidence of strong premium mobile racing sales in 2026); **M3 gate**: closed-beta install→purchase conversion ≥2.5%, D7 ≥25%, refunds ≤5% (ASSUMPTION gates). On failure: **premium-lite fallback** = free base (time trial + 2 circuits + 6-car races) + one-time full-game unlock $14.99–19.99 — still no loot boxes; decision locked at M3, not post-launch.
- Season/regulation content packs (2027, 2028 profiles + new circuits): paid DLC $9.99–14.99 (mirrors EA's 2026 expansion strategy within our own IP — do not ship the 2026 profile as paid DLC; it is launch content). Each separately published DLC app on Steam incurs its own $100 recoupable fee.
- Cosmetics (liveries, suits, helmet): store IAP, no randomized rewards.

### 13.1b Revenue model sketch (ASSUMPTION — validate in M1 business case)

Net per unit: Steam $34.99 ≈ $24.5 net (30% cut); mobile $9.99 ≈ $8.0 net (Play 20% service fee) / ≈ $7.4 net (Apple 26% IAP; 15% under Small Business Program) — Play first-$1M 10% service tier ignored in the table (downside protection noted).

| Scenario | Steam units | Mobile units | Gross net vs $8M low budget | vs $14M high |
|---|---|---|---|---|
| Downside | 120k | 100k | ~$3.6M → FAIL | FAIL |
| Base | 350k | 250k | ~$10.3M → PASS | FAIL |
| Upside | 700k | 600k | ~$21.5M | PASS |

Breakeven: budget definition = $8–14M **including** marketing (per §18). Breakeven ≈ **330–400k total units-equivalent** at the low budget; the 450–500k figure applies only if marketing is additive on top of an $8M production budget. No randomized-reward revenue; DLC adds ~15% LTV in base case (ASSUMPTION). All figures are planning assumptions; the M3 gates and M1 business-case re-run are the controls.

### 13.1c Commercial legal hygiene

- **EULA/ToS** (M3 gate): IP ownership & license grant, account termination, user-conduct rules, UGC license terms, governing law/arbitration, age-eligibility statement, chargeback/refund-abuse policy.
- Work-for-hire/IP-assignment clauses for all outsourced art/audio; engine audio originals only (no recorded real-car samples).
- Esports ladders: prize-competition rules + anti-betting-integrity clause; no real-money wagering features.
- Press/influencer embargo + disclosure process owned by marketing.

### 13.2 Store economics (CONFIRMED, 2026 status)

- Steam: $100 recoupable per-app credit; 30% revenue share; Steam Next Fest, demos supported.
- Apple: App Store Small Business Program 15% (≤$1 M/yr); standard 30% (non-EU). EU (from 2026-10-01, Apple DPLA update 2026-08-18): IAP 26% (15% SBP), alternative payment 20% (10%), link-out 15% (10%), Core Technology Commission 5% outside App Store; payment-choice lock 12 months.
- Google Play (from 2026-06-30 US/EEA/UK; KR 2026-12-31; RoW 2027-09-30): service fee 10% of first $1 M (also subscriptions); non-recurring standard 20% + 5% billing fee (Play Billing) US/EEA/UK; 15%/20% with Play programs; paid-app first purchase 20–25% + billing fee.
- Decision: default to store billing everywhere at launch (compliance simplicity); evaluate alternative billing in EU only after legal review (12-month lock-in cost — R-06).

### 13.3 Monetization principles

**No loot boxes / no randomized paid rewards** (Korea odds-disclosure enforcement 2026; Brazil under-18 loot-box ban 2026; PEGI disclosure obligations; ACB RC risk) — competitive differentiation + compliance simplicity. Cosmetics deterministic; season packs content-complete at purchase.

### 13.4 Release plan

Demo (Steam Next Fest + mobile beta via TestFlight/Play Internal→Closed), soft-launch (2 mobile regions for monetization/telemetry), global launch Steam + iOS + Android same week; launch window: **Q3–Q4 2028** (ASSUMPTION, anchored to the 24-month schedule from the 2026-10 M0 greenlight). Rationale: avoids direct collision with the official product's 2027 release cycle; lands after EA's reimagined title has set the season's pricing/marketing expectations; keeps Q4 certification buffers. Differentiation: lower price, cross-play parity, one base game + optional season packs (no annual re-purchase), regulation-agnostic fictional IP. Competitive set to re-assess at M3 (2028 landscape unknown — U-02 family).

---

## 14. Legal & compliance (N-05/N-06)

### 14.1 Licensing position (CONFIRMED + Derived)

- F1 racing-game license: EA (Codemasters) — exclusive; EA strategy: paid F1 25 expansion for 2026, next full title EA Sports F1 27 (EA 2025-11-18). Frontier held F1 **Manager** license through 2025. Mobile: Hutch (F1 Clash), Eden (F1 Mobile Racing) via Codemasters/F1 deals.
- **Track A (chosen)**: unlicensed product. Never use: "F1", "Formula 1", FIA logo, team/driver/circuit names, sponsor marks, championship marks, broadcast-derived graphics. Fictional "world championship" framing with original IP. Legal review gate on every content naming decision (checklist: `docs/20-design/BRAND_NAMING_CHECKLIST.md`). **Store-metadata hygiene**: title, subtitle, keywords, description, screenshots, capsule art, and trailer VO must not reference real series/brands (trademark clearance + keyword-conflict check before each store update).
- **Track B (contingency)**: if FOWC license obtainable (status beyond 2027 UNKNOWN — R-01), swap the branding content layer (teams, drivers, liveries, championship marks) — the *engine and rules layer* carry over, but Track B additionally requires **licensed circuit geometry, likeness/broadcast rights, licensor QA, and contractual cost/term** — these are business-track gates (rights availability, cost, term, asset sourcing), **not** a free content swap. No planning may assume Track B.

### 14.2 IP hygiene

- Original name/logo/liveries; no circuit-derived geometry or naming; no driver/team likenesses; no sponsor marks; team radio text original; no FIA regulation PDFs embedded (cite, never redistribute — FIA texts are copyrighted; **whether regulation-derived mechanics/tables/UI naming carry IP exposure is an ASSUMPTION pending counsel FTO opinion** — commissioned pre-greenlight; the doc no longer asserts it as settled).
- Music: licensed or original; no stadium anthems.
- Patent screen: counsel review of active-aero/override mechanics against sport-internal IP at greenlight (defensive; rules ≠ patents but game-mechanic patents exist).

### 14.3 Age ratings

- IARC single questionnaire via storefronts (ESRB, PEGI, USK, ClassInd, GRAC, ACB) — CONFIRMED process; re-rate on material content change; maintain rating inventory.
- Korea: GRAC rating via store IRCB routing is available for open-market mobile (Apple/Google designated IRCBs) — CONFIRMED. PC/Steam KR: GRAC's FAQ requires all distributed games to hold classification, with non-Korean companies applying via a local agent (fees ×1.5 for non-Korean applicants) — obligation CONFIRMED, cost/lead-time UNKNOWN (U-05). **Greenlight decision (R-07/R-17)**: (a) budget GRAC classification via local agent, or (b) region-gate Steam KR availability.
- China: NPPA ISBN + local publisher required — **out of scope at launch; geo-block CN** (R-08).
- **Play-level naming**: in-product supplier name is fictional ("sole supplier in profile data"); compound archetypes H/M/S(/W/I) map to profile compound data. Real supplier/compound designations exist only in research dossiers (R1) and are labeled "product ships fictional names" wherever they appear in this blueprint — never in shipped product data or store pages (§14.2). Penalty offence naming likewise fictional ("Stewards' Office").
- Korea (GRAC) — single statement: mobile open-market games route through store IRCB ratings (Apple/Google designated IRCBs) — CONFIRMED. PC/Steam KR: GRAC's FAQ requires all distributed games to hold classification; non-Korean companies apply via a local agent (fees ×1.5 for non-Korean applicants) — obligation CONFIRMED, cost/lead-time UNKNOWN (U-05). **Greenlight decision (R-07)**: budget GRAC classification via local agent, or region-gate Steam KR availability. Mobile routing is materially simpler; the decision only concerns PC/Steam KR.
- Expected rating: Everyone/Teen (racing, contact damage, mild language in team radio — gate radio text).
- **Korea PC/Steam (CONFIRMED obligation)**: GRAC's published FAQ requires all distributed games (incl. PC online) to hold a classification; non-Korean companies cannot apply directly and must contract a local agent/publisher; fees scale ×1.5 for non-Korean applicants (GRAC enforcement page). **Decision required at greenlight (R-07)**: (a) budget GRAC classification via local agent, or (b) region-gate Steam storefront availability in Korea. Mobile uses store IRCB routing (Apple/Google) — confirmed simpler.

### 14.4 Privacy & platform programs

- GDPR (EU/UK), COPPA + US state age-assurance laws, **Korea PIPA** (incl. under-14 parental consent), **Brazil LGPD + ECA Digital age assurance** (in force 2026-03; reliable verification over self-declaration — REPORTED), Canada PIPEDA; DPA + ROPA; data minimization; analytics consent UX.
- **Consumer law**: EU Digital Content Directive 2019/770 (conformity + legal guarantee), Consumer Rights Directive pre-contract disclosure, UCPD (incl. in-game purchase transparency), EU withdrawal rights and Steam refund posture; per-region refund SOP documented.
- Mobile trial/refund posture: no trial build on stores; optional "premium-lite" fallback (§13.1); refund SOP per store policy.
- **Analytics processors**: inventory of analytics/crash vendors with DPAs, data-residency and transfer decisions, retention schedule (M3 compliance gate).
- **Account deletion / data rights (cert-required)**: in-app account deletion path (Apple 5.1.1(v) style; Play data-deletion requirements), data export (DSAR) SOP, deletion propagation to leaderboards/UGC with grace windows — M3 gate; manifest entries for all collected categories.
- Apple: privacy manifest + required-reason APIs; ATT if cross-app tracking (avoid); Play Data Safety form; Play Families/ads no-ads at launch (no ad SDK → simpler).
- Accounts: platform SSO; no email required for play; support multiple identity links.
- Accessibility programs: apply to Apple/Google inclusion features; **legal position**: counsel-signed accessibility target — WCAG 2.2 AA for store/account surfaces, EAA scope assessed for EU commerce surfaces (M3).
- Localization priority reconciled with market access: PT-BR and KO prioritized (target markets); Chinese locales deprioritized while China is geo-blocked (§14.3).

---

## 15. Live operations & content roadmap

- **Content cadence**: quarterly track/circuit drops + livery packs; annual regulation-profile update (2026→2027→2028) with data + content only; balance patches from telemetry (tuning, AI, assists).
- **Regulation-profile releases**: the 2027 profile is already fully specified at power-unit level (WMSC-ratified 2026-06-23 package) and its consolidated technical text is published (2027 Section C Iss 03, 2026-10-01) — the 2027 seasonal pack is a content drop, not a rules rework. The 2028 profile ships only when the FIA publishes the non-PU 2028 regulations; its power-unit step is already ratified data. Store page must not promise 2028 content before publication (§15 2028 readiness).
- **Live-ops ownership** (fixes draft gap): dedicated live-ops/community manager (1 FTE) + on-call rota for server incidents (target 99.9% lobby availability; p1 response ≤30 min — ASSUMPTION SLA); seasonal content pipeline staffed from existing teams with one dedicated content engineer.
- **Esports/community**: weekly time-trial ladders, community events; open telemetry API for standings; mods: **no** track/vehicle mods at launch (IP hygiene), livery-only UGC with moderation (R-09). **UGC scope (defined)**: livery image uploads only (no meshes/tracks/code); pre-moderation with human review queue + automated hashing against known-bad-content databases; platform reporting tools; moderation SLA ≤24 h; UGC license terms per EULA (§13.1c); no monetization of UGC.
- **Compiler discipline**: every live change passes determinism regression before ship (N-01); hotfix path for sim bugs with golden-replay re-validation.
- **2028 readiness**: Pirelli supply confirmed through 2028; the 2028 **power-unit step is ratified** (ICE 450 kW, MGU-K 300 kW, harvest 400 kW, 60/40 split, fuel flow +13 %) and is carried as profile data; the remaining 2028 regulations are UNKNOWN → keep those profile slots empty (with explicit `UNKNOWN` markers, not inherited values) until WMSC/FIA publish; do not promise 2028 content on store page.

---

## 16. Testing & quality strategy (N-01…N-07)

### 16.1 Simulation validation

- Unit: kernel invariants (energy conservation deltas ≤ tolerance, fuel accounting vs flow cap, tyre-load monotonicity).
- Golden replays: ≥20 scripted scenarios (race starts, SC, low-grip, red flag, 22-car contact) deterministic across platforms; checksums in CI.
- Determinism gates AT-D1…AT-D8 (§4.2): sim-frequency/render-frequency independence, input-rate independence, tick composition, numerics contract, RNG isolation, codegen audit.
- Regulation conformance tests: points tables, SQ/Q elimination, classification, Overtake gating — table-driven against Section A/B extracts (snapshot tests against the ratified tables).
- **Power-unit conformance tests AT-R1…AT-R8 (§5.4)**: MGU-K deploy envelope against Section C Iss 20 article text (C5.2.8/C5.2.10), the four limits (P_ceiling(mode,year) × P_speedCurve × P_zone × P_powerLimitedPeriod), with Boost as a driver-initiated overlay (not a ceiling), the 2027/2028 ceiling switch, energy-ledger caps, and profile-only changeability. These run on every profile commit and on every regulation patch before ship (§15, §16.7).
- Performance envelope checks: lap-time targets within tolerance bands per archetype (ASSUMPTION bands from §5.2).

### 16.2 Multiplayer testing

- Network emulation suite (loss/jitter/spike), desync fuzzing, reconnect tests, **full-fidelity 22-car contact determinism offline** (golden replays; reduced-fidelity CI only for lobby transport/load paths — the two are explicitly distinct), cross-play matrix Steam ↔ iOS ↔ Android, region soak tests, server-authority exploit drills (input spoofing, energy tampering, input-delay peeking), input-drop-rate soak metric (§10.1 clamp), desync-free ≥99.9% of sessions acceptance bar.

### 16.3 Device & performance lab

- Deck/Machine verified checklist automation; iOS lab A13→A19 spread; Android lab (SD865/8Gen1/8Gen2/8Gen3 + Mali tier); thermal soak (60 min), battery profiling, crash-free sessions ≥99.5% target (ASSUMPTION).

### 16.4 Compliance QA

- Rating consistency audits (content vs. disclosures), privacy manifest/data-safety review per release, age-gate tests, regional store policy checklist, IARC re-rate SOP on material change.

### 16.5 Player testing

- Internal playtest cadence; closed beta (mobile soft-launch telemetry); difficulty curve telemetry (retention, assist usage); controller vs touch parity tests.

### 16.6 Engine upgrade policy

Unity 6.3 LTS to Dec 2027 support window: with launch targeted Q3–Q4 2028, the upgrade decision lands at **M3 (pre-content-complete)**: adopt the then-current LTS at M3 if its mobile/perf profile validates in the M3 beta, else remain on 6.3 LTS with an extended-support plan; feature freezes around upgrades; dependency pinning; upgrade spike tasks scheduled; a deferred upgrade is itself a tracked risk (R-11) with an in-service mitigation (security patches via hotfix stream where available).

### 16.7 Replay & anti-cheat versioning

Replays carry kernel-version + regulation-profile stamps; leaderboard archive entries remain re-simulable by retaining re-sim-able kernel versions for ≥2 seasons; live sim patches require golden-replay re-validation before ship (§15) and a compatibility note for prior submissions.

---

## 17. Risk register (top 19; full register: `docs/20-design/RISK_REGISTER.md`)

| ID | Risk | Sev | Mitigation |
|---|---|---|---|
| R-01 | EA license dynamics beyond 2027 unknown | High | Track A unlicensed default; Track B swap-ready; brand hygiene |
| R-02 | Reference-data conditionality (one 2026 real-world debut event subject to FIA homologation) | Low | Calendar data flags conditional rounds; content packs flexible |
| R-03 | 2028 non-PU regulations unknown (the 2028 PU step *is* ratified — WMSC 2026-06-23) | Medium | PU step carried as profile data; other 2028 slots kept as explicit `UNKNOWN` markers; no public 2028 promises |
| R-04 | 2026 deploy/harvest constants now primary-verified (Section C Iss 20 article text); 2027/2028 consolidated constants still need full-text cross-check | Medium | AT-R1…AT-R8 conformance tests; profile data fields; cross-check the 2027 Section C Iss 03 text at M1 |
| R-05 | Store size limits change | Medium | Streaming architecture; re-verify pre-submission |
| R-06 | EU 12-month payment lock-in (Apple) | Medium | Launch on store billing; revisit post-launch |
| R-07 | Korea PC/Steam GRAC classification obligation (CONFIRMED requirement; cost/lead time open) | Medium | Greenlight decision: local-agent budget vs region-gate (§14.3) |
| R-08 | China requires ISBN + publisher | Low | Geo-block CN at launch |
| R-09 | UGC/mod IP risk | Medium | Livery-only UGC, moderation, no asset imports |
| R-10 | Cross-platform determinism slippage | High | §4.2 numerics discipline + AT-D1…AT-D8 + golden CI gate |
| R-11 | Unity 6.3 LTS EOL Dec 2027 | Medium | §16.6 upgrade plan |
| R-12 | Mobile 60 fps infeasible on min tier | Medium | Tiering + adaptive + 30 fps fallback modes |
| R-13 | AI/balance quality below bar vs genre | High | §6 validation harness; playtest KPIs |
| R-14 | No-loot-box revenue model under-monetized | Medium | Packs + cosmetics; revisit with regional legal |
| R-15 | 22-car netcode complexity | High | §10.1 model; exploit drills; soak |
| R-16 | Steam Datagram Relay cross-platform (mobile+PC) eligibility not guaranteed | Medium | §10.1 transport plan; Valve confirmation gate at M2; self-hosted relay fallback budgeted |
| R-17 | Korea GRAC agent cost/lead-time (cost-side of R-07 decision) | Medium | Agent quotes at greenlight; budget line in §18 |
| R-18 | Mobile premium model underperforms | High | §13.1b gates + premium-lite fallback decision at M3 |
| R-23 | Regulation drift between ratified announcements and consolidated regulation text (2027/2028 PU package; mid-2026 designated-zone refinement) | Medium | Profile carries `sourceIssues[]` + confidence + `verifiedOn`; U-03/U-04/U-11 full-text cross-check at M1; AT-R1…AT-R8 fail on drift |

---

## 18. Budget, team, schedule (all ASSUMPTION)

- Team: 28–36 FTE peak (programming 10–13, design 4–5, art 8–10, QA 3–4, production/ops 2–3 incl. dedicated live-ops/community 1 + **support/customer-care 1** (refunds, appeals, age-assurance, GRAC agent interface), audio 1–2). Comparable mid-size sim-racing studios (ASSUMPTION band).
- Timeline: M0 (this blueprint, 2026-10) → M1 vertical slice (2 tracks, 8 cars, 1 mode; **determinism + kernel budget kill-criteria**) 5 months → M2 (22 cars, career alpha, 8 tracks, **netcode pilot + load tests + SDR eligibility gate**) 7 months → M3 (beta, multiplayer, compliance submissions, mobile-model gate, **Unity LTS upgrade decision**) 6 months → M4 (content-complete, certs) 4 months → launch Q3–Q4 2028. **Total 22 months** from greenlight (corrected arithmetic) + cert/store buffer ≈ 24 months to launch.
- Budget band: $8–14M = salaries + outsourcing + **marketing $1.5–3.0M** + localization (11 non-EN languages) + legal (FTO opinion, EULA, Korean agent) + rating/GRAC fees + localization + server capex/opex (ASSUMPTION; no publisher).
- Kill criteria: vertical-slice KPI failure (frame targets, feel tests, netcode desync-free <99.9% of sessions in soak), kernel budget >2.0 ms/tick unresolved, or license/brand legal blocker.

---

## 19. Open questions & irreducible unknowns (honesty register)

| # | Unknown | Why unknown | Impact | Resolution |
|---|---|---|---|---|
| U-01 | EA/F1 license terms beyond 2027 | Private contracts | Track B feasibility only | None from outside; design remains Track A |
| U-02 | 2028 calendar + non-PU regulations | Not published as of 2026-10-12 (2028 PU step *is* ratified) | Content roadmap tail | Watch WMSC/FIA releases; PU step already carried as data |
| U-03 | Deploy-curve numeric constants + zone-cap year specificity | **Resolved for 2026**: Section C Iss 20 Art. C5.2.8 i/ii text verified verbatim (1800−5v / 6900−20v / 0; 7100−20v / 0) and C5.2.10 harvest caps verbatim; 2026 designated-zone 350/250 kW cap structure CONFIRMED (S-46). **Open for 2027/2028**: consolidated constants in 2027 Section C Iss 03 (2026-10-01) not yet cross-checked line-by-line. Zone-cap tables and any year-dependent zone/event parameters remain with **confidence=UNKNOWN** and **sourceVerified=false** until the consolidated text is read. No values silently inherited across years. | Balance tables; AT-R1, AT-R3 | Read 2027 Section C Iss 03 PDF at M1, quote article numbers for zone cap (U-11), record year-specific confidence/sourceRef/verifiedOn per profile field. |
| U-04 | Full text of 2026 Iss 09 changes (2026-10-01) and 2027 Section A/B | Summary-level reporting | Rule details | Read FIA PDFs at M1; profile fields already parameterized |
| U-11 | Article numbering for the 2026 designated-zone 350/250 kW MGU-K cap and the power-limited-period envelope in the consolidated 2026/2027 Section C text | FIA statement (2026-04-20) and per-event "Power Unit Information" documents are CONFIRMED, but the exact consolidated article numbers for the zone cap were not quoted from the PDF in this pass | AT-R2/AT-R3 traceability | Cite the article numbers when the full-text check (U-03) lands |
| U-05 | Korea agent cost/lead time for GRAC PC classification | Vendor quotes pending | KR market decision | Greenlight decision (R-07/R-17) |
| U-06 | Store cellular download limits at ship time | Platform policy drift | Streaming design | Re-verify at submission |
| U-07 | Mobile tier device spread at launch (2027 hardware) | Market drift | Perf tiers | Annual device-tier refresh |
| U-08 | Multiplayer concurrent-scale + SDR cross-platform eligibility | No user data; Valve partner-gated terms | Server cost model | M2 gate; soft-launch telemetry |
| U-09 | Per-event FIA deployment-parameter sets (gap/lines/allocations) for 2026+ events | FIA publishes per-event ~4 weeks ahead; our game mirrors structure, not events | Authenticity flavor | Pattern-based generation from archetypes (no dependency) |
| U-10 | Brazil ECA Digital age-verification vendor/UX | Vendor market immature; law in force 2026-03 | KR/BR compliance + cert | Vendor selection + UX test at M3 (§12 age-gate) |

---

## 20. Appendices

### 20.1 Glossary (single source of truth)

**Overtake Mode** — 2026 proximity energy aid replacing DRS (armed within Detection Gap at Detection Line; +0.5 MJ harvest; elevated deploy curve; SC/low-grip disabled). **Override / Overtake Override Mode** — the regulation's term for the elevated-deploy state used by Overtake (Art. C5.2.8 ii); the product uses the fictional name "Slipstream Boost" (§14.3). **Boost** — manual deployment of stored energy usable anywhere (defender's counter); race-capped at +150 kW. **Active Aero** — full-time front/rear flap modes: Straight Mode (low drag) / Corner Mode (downforce), zone-driven. **Overtake Detection/Activation Lines** — track-defined gate points. **Low Grip Conditions** — wet/standing condition flag disabling Overtake. **Regulation Profile** — data bundle encoding a season's rules. **Kernel** — deterministic sim core. **Golden replay** — recorded input log replayable bit-identically. **ERS-K / MGU-K** — the kinetic energy-recovery chain and its motor (deploy/harvest power limits per §5.1/§5.4). **ES (Energy Store)** — the battery; SoC window ≤4 MJ. **Designated acceleration zone** — event-defined lap sectors (corner-exit → braking point, incl. overtaking zones) where MGU-K deploy stays at the full ceiling (350 kW in 2026); elsewhere it is capped lower (250 kW in 2026). **Power-limited period** — event-declared sector during which the PU power-reduction envelope and rate limits apply (distance and rate published per event). **Superclip / super-clipping** — harvesting at full throttle (peak 350 kW from 2026 mid-season), which trades acceleration for energy. **Simulation tick** — fixed 60 Hz kernel step (§4.2); rendering and input are subordinate to it.

### 20.2 Review process & records

All adversarial reviews, defect logs, RCA notes, and iteration history live in `docs/90-gauntlet/`. Mission control: `GAUNTLET_STATE.md`. This blueprint must not be edited except through the gauntlet loop (defect → RCA → correction → re-audit).

### 20.3 Source register summary (full register in research docs)

FIA 2026 Regs Sections A/B/C/D/F: C Iss 20 (2026-08-05, WMSC 2026-08-03), **B Iss 09 (2026-10-01)**, A Iss 03 (2026-06-25), D Iss 08 (2026-10-01), F Iss 11 (2026-10-01); **FIA 2027 Regs Section C Iss 01 (2026-06-25) / Iss 02 (2026-08-05) / Iss 03 (2026-10-01)**; 2026 PU Regs Iss 06 (2024-03-29); FIA "Refinements to the 2026 FIA Formula 1 regulations" (2026-04-20 — designated-zone 350/250 kW, qualifying 7 MJ, superclip 350 kW, Boost +150 kW); FIA/FOM power-unit package table (2026-06-10) + **WMSC ratification (2026-06-23, Macau)** for the 2027/2028 rebalance (ICE 420/450 kW, MGU-K 300 kW, Overtake 350 kW, harvest 375/400 kW, splits 58/42 and 60/40); FIA statements (2025-03-07 Cadillac; 2026-02-28 amendments; 2026-06-11 Pirelli); per-event FIA "Power Unit Information" documents (Detection Gap, lines, sector zones, Power Limited Distance, reduction rate); F1.com (2026-03-04, 2024-06-06, 2025-06-10, 2026-06-10, 2026-06-23, 2026-09-16); Pirelli (2025-11-24, 2025-12-09, 2026-02-12, 2026-06-11); Reuters/Motorsport.com (2025-06-10, 2026-01-08); Sky Sports (2025-12-17); EA (2025-11-18); Traxion (2024-09-01); unity.com (Unity 6 releases, Runtime Fee cancellation); partner.steamgames.com (Deck Verified); heise/GDC (2026-03-12); Apple (2026-08-12, 2026-08-18 DPLA); Android Developers Blog (2026-06-24); Play Console Help (2026); GRAC; IARC FAQ; QAwerk compliance survey (2026-07-09); kartphysics.com (2026-09-28); HdM Stuttgart (2023-02-27); BBC (2026-09-16). Direct URLs for every entry: `docs/10-research/SOURCES.md`.

### 20.4 Document control

Change this blueprint only via gauntlet loop. Any correction must update: (1) the section, (2) GAUNTLET_STATE.md defect log, (3) requirements traceability if affected.

