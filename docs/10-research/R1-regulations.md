# Research Dossier R1 — Regulations 2026–2028

**Research-only dossier.** Real series, event, circuit, supplier and manufacturer names are permitted here and marked "reference only" — they must never enter product data or product-facing blueprint copy (§8.2/§14.3). Every entry below carries a direct source URL in `docs/10-research/SOURCES.md` (verified 2026-10-12).

## 1. Power unit & energy (CONFIRMED unless noted)

- 2026 PU: ~50/50 ICE/electrical split (actual 53/47). ICE ≈400 kW; MGU-K up to **350 kW** (3× 2025's 120 kW) [S-12, S-05].
- **MGU-H removed**; fuel **100% advanced sustainable** (drop-in; CCU/waste/biomass) [S-05].
- Fuel energy flow: cap **3000 MJ/h**; below 10,500 rpm curve **EF = 0.27·N(rpm) + 165 MJ/h** [S-04 Art. 5.4.3/5.4.4].
- ES SoC window ≤ **4 MJ** [S-04 Art. 5.4.8; S-01 Art. C5.2.9]; MGU-K mechanical torque ≤ **500 Nm** [S-01 Art. C5.2.11]; MGU-K absolute DC power ≤ 350 kW [S-01 Art. C5.2.6/C5.2.7; S-04 Art. 5.4.6].
- **Deploy limits — verbatim article text, Section C Iss 20 Art. C5.2.8 (CONFIRMED via S-01)**:
  - i. Overtake not active: `P(kW) = 1800 − 5 * car speed (kph)` below 340 kph; `P(kW) = 6900 − 20 * car speed (kph)` for 340 ≤ v < 345 kph; `P(kW) = 0` at ≥ 345 kph.
  - ii. Overtake active / In Override Mode up to: `P(kW) = 7100 − 20 * car speed (kph)` below 355 kph; `P(kW) = 0` at ≥ 355 kph — with the 350 kW absolute cap this is 350 kW flat to 337 km/h (350 kW is reached at 337.5 km/h), ≈200 kW @345, ≈100 kW @350, 0 @355.
  - Consequence: both curves are identical (≈1800−5v) below **290 km/h**; the Overtake advantage band is 290→355 km/h and peaks ~**+235 kW at 337 km/h**.
- **Harvest limits — verbatim article text, Art. C5.2.10 (CONFIRMED via S-01/S-51)**: energy harvested by the ERS-K Recharge at the CU-K HV DC Bus ≤ **8.5 MJ/lap**; i. may be reduced to 8.0 MJ where the FIA determines the maximum achievable per lap is ≤8 MJ; ii. may be reduced to no less than **5.0 MJ for Sprint Qualifying and Qualifying** where harvesting strategies are excessive (subject to Art. B7.2); iii. **up to 0.5 MJ additional per lap** subject to Art. B7.2 (the Overtake bonus → 9.0 MJ ceiling). Vehicle fundamentals in FIA-F1-DOC-034.
- **2026 designated-zone MGU-K envelope (CONFIRMED via S-46, FIA 2026-04-20; implemented from the Miami round and consolidated into Section C Iss 20)**: MGU-K deployment is maintained at **350 kW in key acceleration zones** — from corner exit to braking point, including overtaking zones — and **limited to 250 kW in other parts of the lap**. Supporting 2026 changes: qualifying recharge 8→**7 MJ**; peak superclip (harvest) power 250→**350 kW** (applied in race too); number of events allowed alternative lower energy limits 8→**12**; race **Boost capped at +150 kW** (or the current power level at activation if higher); automatic MGU-K deployment at starts with rear/side flashing-light warning; energy-counter reset at the start of the formation lap. Article numbering for the consolidated zone cap is not yet quoted from the PDF (U-11).
- Mass minimums: ICE ≥130 kg; PU ≥185 kg [S-04].

### 1a. Ratified 2027/2028 power-unit trajectory (RATIFIED-CHANGE — WMSC Macau, 2026-06-23)

Source: FIA/FOM stakeholder package table 2026-06-10 [S-48] + WMSC ratification 2026-06-23 [S-47]; cross-checked by Reuters, ESPN, The Race, Autoweek, Motorsport.com, Motorsport Week, PlanetF1, F1i [S-52…S-58]. The consolidated 2027 technical text is published (Section C Iss 01/02/03 [S-49]); its numeric constants still require a line-by-line cross-check (U-03/U-11).

| Parameter | 2026 | 2027 | 2028 |
|---|---|---|---|
| ICE max power | 400 kW | 420 kW (+5 % fuel flow) | 450 kW (+13 % fuel flow) |
| Fuel energy-flow cap | 3000 MJ/h | 3150 MJ/h (derived) | 3390 MJ/h (derived) |
| MGU-K max deploy (normal) | 350 kW | 300 kW | 300 kW |
| MGU-K max deploy (Overtake Mode) | 350 kW | 350 kW | 350 kW |
| Max harvesting power | 350 kW | 375 kW | 400 kW |
| Power split ICE/MGU-K | 53/47 | 58/42 | 60/40 |
| Designated-zone cap | 350 kW in designated acceleration zones / 250 kW elsewhere | carried forward as profile data | carried forward as profile data |
| Supporting measures | — | PU supply conditions, race operations, PU financial regulations; pre-season testing 3→4 days; wet/low-grip Boost permitted | same framework |

Notes: (a) the deploy *curve formula family* (1800−5v / 6900−20v / 7100−20v) is carried into the 2027 Section C text, so the year ceiling (300 kW) rather than the curve becomes the binding limit at low speed in 2027/2028 — verify the consolidated tables before shipping the 2027 profile (U-03). (b) Secondary reporting disagrees on the 2026 harvesting baseline (250 kW per S-52 vs 350 kW per S-48/S-53); the FIA/FOM table governs. (c) Because the 2028 power-unit step is ratified, the 2028 profile is no longer wholly unknown — only its non-PU regulations and calendar are.
- **Overtake Mode** (DRS replacement): Detection Line + Activation Line; event-published **Detection Gap** (1.0 s in practice 2026); +0.5 MJ harvest (C5.2.10 iii — **CONFIRMED article text**); disabled under Safety Car (restored after all cars pass post-SC line) and **Low Grip Conditions**; in lap-time-classified sessions enabled+activated for all [S-02b/B Art. B7.2; S-30/S-32 REPORTED].
- **Boost**: manual deployment of stored energy, usable anywhere — the defender's counter-tool; race-capped at +150 kW from 2026 mid-season [S-46; S-33, S-32].
- Per-event FIA "Power Unit Information" documents (Race Director, ~4 weeks pre-event) set the parameters below; the 2026 British GP document (S-51) shows the structure: session recharge columns for "Overtake not active / Overtake active" (8.0 / 8.5 MJ; 6.5 MJ for SQ & Q), **Power Limited Distance 3,833 m**, **max PU power-reduction rate 50 kW/s**, Detection/Activation Lines with lap-distance and speed thresholds (Art. B7.2.1b.viii), sectors where a greater initial power reduction is permitted, sectors where a reduction reset is permitted, and the "Base-Standard / Base-Overtake / Alt 1" MGU-K-vs-speed curve set. Earlier-report example values: Detection Gap 1.0 s; qualifying 6.0 MJ; FP/out-laps 8.5 MJ; PLD 2,923 m [S-42 REPORTED]; mid-season adaptive deployment (automated qualifying outlap deployment; scenario-specific 250 kW limits) [S-43 REPORTED]. **Design consequence**: all of these are profile/event data; the kernel has no 2026 constants.

## 2. Chassis & aero (CONFIRMED)

- Min mass **768 kg** (2025: 798 kg incl. driver) [S-11]. Car width **1900 mm** (from 2000) [S-16].
- **Active aero full-time**: front+rear flaps; Straight Mode (open, low drag) / Corner Mode (closed, downforce), zone-based [S-11, S-33]. (Earlier public naming: X-mode/Z-mode [S-11 family].)
- Tyres: 18-inch rims retained; tread **−25 mm front / −30 mm rear**; diameter **−15 mm front / −10 mm rear**; set-of-four ~1.6 kg lighter [S-15, S-16]. Compounds C1–C5 homologated 2025-12-15; wider compound gaps intended for strategy variety [S-17].
- Supplier: Pirelli sole supplier **through 2028** (one-year option exercised 2026-06-11) [S-10, S-14]. Beyond 2028 open tender → 2029+ unknown.

## 3. Championship & sporting (CONFIRMED except items marked REPORTED)

- Championship: ≤24 competitions, ≤24 cars, 2 per team [S-03]. Grid: **22 cars** (Cadillac 11th team from 2026) [S-06, S-38].
- Race points (≥75%): 25-18-15-12-10-8-6-4-2-1; reduced-race columns (6-4-3-2-1 at ≥2L; 13-10-8-6-5-4-3-2-1 at ≥25%; 19-14-12-10-8-6-4-3-2-1 at ≥50%) [S-03]. **No fastest-lap point** (removed from 2025) [S-18, sportrules.org].
- Sprint points (≥50%): 8-7-6-5-4-3-2-1; no points <50% [S-03].
- Sprint weekend (Alternative Format): FP1 only; **SQ 12′/10′/8′** with 7′ breaks; eliminate 6+6 (scales with field); Sprint ≥100 km, 1 h limit (+1.5 h max incl. suspensions); grid from SQ with penalty application order [S-02].
- Qualifying **Q1 18′ / Q2 15′ / Q3 13′** (7′ breaks; Q3 = 13 min per Section B text — reviewer-verified quote from Iss 06/07; **re-verify against Iss 09 at production**); Q1/Q2 eliminate slowest six on 22-car basis [S-02 excerpt + reviewer evidence].
- **Race distance ≥305 km** (Monaco 260 km); **3-hour total cap abolished in Iss 09** — replaced by event-specific hard stop preserving ≥2 h green-flag racing [S-02b; S-45 REPORTED].
- Tyres: Pirelli supplies per driver per event (allocation ~13 dry sets — REPORTED); dry race requires ≥2 dry compounds (Article B6 family, Section B Iss 09 ToC) [S-02b].
- 107% rule in SQ/Q1 (unclassified cases; stewards discretion to admit to race) [S-02].
- Regulation package amended by WMSC e-vote 2026-02-28 after pre-season testing [S-07]; **current issues: C Iss 20 (2026-08-05) / B Iss 09 (2026-10-01) / A Iss 03 (2026-06-25)**; superseded: B Iss 08 (2026-08-05), B Iss 07 (2026-06-25), B Iss 06, B Iss 05 (2026-02-23/27), A Iss 01 (2025-12-10).
- 2026 calendar: 24 rounds, Melbourne (Mar 6-8) → Abu Dhabi (Dec 4-6); Bahrain/KSA April (Ramadan); Madrid Sep 11-13 (homologation-conditional); Sprints: China, Miami, Canada, GB, Netherlands, Singapore [S-08, S-13, S-34, S-35]. **Reference only — the game ships fictional rounds (§8.3); real names never enter product data.**
- 2027 calendar: 24 rounds, Bahrain (Mar 12-14) → Abu Dhabi (Dec 10-12); Portugal (Portimão, Jun 18-20) and Türkiye (Istanbul, Oct 1-3*) return; Barcelona & Zandvoort out; **10 Sprints** (BHR, AUS, JPN, MON, CAN, GBR, ITA, BRA, QAT, ABU; Monaco sprint weekend has two qualifying sessions — its usual Q plus SQ) [S-09, S-36]. *Istanbul subject to circuit homologation. BBC notes Bahrain/Saudi "in doubt" due to regional situation — content contingency flag [S-36 REPORTED]. FIA has signalled an even/odd-year Spain-Belgium rotation intent for 2028+ [S-09 REPORTED].
- 2028: no calendar/non-PU regulations published as of 2026-10-12 → UNKNOWN. **The 2028 power-unit step IS ratified** (ICE 450 kW, MGU-K 300 kW, harvest 400 kW, 60/40 split, fuel flow +13 % — §1a), and Pirelli supply is confirmed through 2028 [S-10].

## 4. Confidence summary

CONFIRMED (primary FIA/F1/Pirelli documents S-01…S-18, S-46, S-51; URLs in SOURCES.md): 2026 PU/chassis/points/format/calendar facts above except where marked REPORTED. **Upgraded in the 2026-10-12 pass**: 2026 deploy curves (Art. C5.2.8 i/ii verbatim), harvest caps (Art. C5.2.10 i–iii verbatim), MGU-K 500 Nm torque (Art. C5.2.11), designated-zone 350/250 kW envelope (FIA statement 2026-04-20 = S-46).
RATIFIED-CHANGE (WMSC 2026-06-23, S-47; package table S-48; consolidated text published as S-49): the 2027/2028 power-unit trajectory in §1a.
REPORTED: Q3 13′ (reviewer-quoted FIA text; re-verify at production), Istanbul homologation caveat & Bahrain/KSA 2027 contingency (S-36), per-event parameter values (S-42, S-43, S-44), 2028 Spain/Belgium rotation intent (S-09), tyre allocation count, deploy-curve reporting (S-31/S-41 — superseded by S-01), Iss 09 analysis (S-45).
UNKNOWN: 2028 calendar and non-PU regulations; consolidated 2027 Section C numeric constants and the article numbers for the designated-zone cap (U-03/U-04/U-11); per-event Detection Gap/line values for future events.
Issue currency (audited against the FIA regulations index, S-60, on 2026-10-12): Section C Iss 20 = 2026-08-05 (WMSC 2026-08-03); Section B Iss 09 = 2026-10-01 (WMSC 2026-09-30); Section A Iss 03 = 2026-06-25; Section D Iss 08 = 2026-10-01; Section F Iss 11 = 2026-10-01; **2027 Section C Iss 01 = 2026-06-25, Iss 02 = 2026-08-05, Iss 03 = 2026-10-01**; PU Regs Iss 06 = 2024-03-29. Superseded: B Iss 08 (2026-08-05), B Iss 07 (2026-06-25), B Iss 06, B Iss 05 (2026-02-23/27), C Iss 19 (2026-06-25), A Iss 01 (2025-12-10).
