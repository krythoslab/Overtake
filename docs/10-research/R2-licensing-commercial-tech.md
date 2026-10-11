# Research Dossier R2 — Licensing, commercial & tech/platform

## 1. Licensing landscape (CONFIRMED facts; decisions are ASSUMPTION)

- EA (via Codemasters, acquired Feb 2021, $1.2B) holds the exclusive **F1 racing-game** license; Codemasters lineage 2008→2025 with extension options to 2026–27 [S-19, S-37].
- EA strategy announced 2025-11-18: **no F1 26 standalone**; paid expansion on F1 25 covering the 2026 season (new cars, regs, teams, drivers); **next full title EA Sports F1 27** in 2027 ("reimagined") [S-19].
- Other official licensees (historical): Frontier Developments — F1 **Manager** games through 2025 (exclusive management category) [S-37]; mobile: Eden Games' F1 Mobile Racing (via Codemasters deal) [S-37]; Hutch's F1 Clash (separate mobile license, per Traxion survey) [S-37].
- **Implication (design decision)**: unlicensed launch (Track A). Branded launch (Track B) requires FOWC terms; status beyond 2027 UNKNOWN (private contracts) — mission unknown U-01. Cost of F1 licenses historically 8 figures (not publicly confirmed for 2028+) → do not budget around Track B.

## 2. Engine & tech (CONFIRMED)

- Unity 6 line: 6.0 LTS supported through Oct 2026; **6.3 LTS supported 2 years to Dec 2027** (+1y Enterprise); 6000.5.0f1 (2026-06-15) and 6000.6.0f1 (2026-08-31) in-stream; **Runtime Fee canceled** [S-21, S-22].
- Mobile-relevant 6.x features: URP On-Tile Post Processing (tile GPUs: iOS/Android), Adaptive Performance (iOS thermal throttling to 30/15 fps in Serious/Critical states by default), accessibility screen readers, Android storage/HTTP2 options [S-22].
- Unreal 5 remains viable for PC-first but costlier on mobile; no mobile-exclusive feature outweighs Unity ecosystem for 3-platform shipping (studio judgment = ASSUMPTION).

## 3. Steam (CONFIRMED)

- Deck Verified requirements: default config playable framerate (30 fps @1280×800 on Deck), text ≥9 px @1280×800 (12 px recommended), full controller access without setup, Proton/SteamOS support, suspend/resume, no incompatible anti-cheat/DRM [S-20]. Steam Machine Verified = 30 fps @1080p [S-39]. Steam Frame (VR, 2026) = 720p30 standalone [S-39].
- Economics: $100 recoupable per-app fee; 30% revenue share; Next Fest + demo supported (Steamworks program knowledge — store-listed).

## 4. Mobile stores (CONFIRMED 2026 status)

- **Apple EU** (DPLA update 2026-08-18, effective 2026-10-01): IAP commission 26% (15% Small Business Program); alternative payment processing 20% (10%); out-of-app link offers 15% (10%); Core Technology Commission 5% for distribution outside App Store; payment-choice lock 12 months; monthly reporting for alternative payments [S-23, S-24]. Non-EU: standard 30%/15% SBP.
- **Google Play** (from 2026-06-30 US/EEA/UK; AU/JP 2026-09-30; KR 2026-12-31; RoW 2027-09-30): service fee separated from billing fee; service fee 10% first $1M (all transactions incl. subs); non-recurring standard 20% + 5% billing fee (Play Billing) in US/EEA/UK; 15% (new installs)/20% (existing) with Apps/Games programs; paid-app first purchase 20–25% + billing fee [S-25, S-26].
- Strategy: store billing at launch everywhere (simplicity); EU alternative billing optional later (12-month lock).

## 5. Decisions (with confidence)

| Decision | Choice | Confidence | Fallback |
|---|---|---|---|
| Engine | Unity 6.3 LTS + URP | CONFIRMED tech facts; choice ASSUMPTION | mid-project engine change infeasible → de-risk in M1 |
| License stance | Track A unlicensed | CONFIRMED license facts; stance ASSUMPTION | Track B data swap |
| Billing | Store billing default | CONFIRMED policy facts | EU alt-billing later |
| Steam target | Deck + Machine Verified | CONFIRMED criteria | Playable badge acceptable worst case |
| Price | $34.99–39.99 PC / $9.99–14.99 mobile | ASSUMPTION | regional testing in beta |
