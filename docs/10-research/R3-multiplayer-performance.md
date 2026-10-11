# Research Dossier R3 — Multiplayer & performance feasibility

## 1. Netcode model evidence

- **Authoritative dedicated servers + client prediction + server reconciliation + interpolation** is the established pattern for online racing: HDM Stuttgart survey of racing architectures (2023-02-27) — hosted-P2P fails on host trust/cheating and connection variability; dedicated server gives capacity predictability and integrity [S-40].
- **Rollback unsuitable at 22 cars**: full-state rollback demands bit-perfect determinism + cheap full-state save/restore; resim cost scales with misprediction window × sim state; with 20+ remote inputs to guess, near-full resim runs most frames (Mighty netcode tutorial analysis; OPODIS 2025 formalization of rollback properties) [S-40]. Kart-game UE5 replication writeup (2026-09-28) documents even 2–8 vehicle prediction pitfalls (contact cache during replay, gameplay-state rewind completeness) → at 22 cars, own-car-only prediction + interpolated remotes is the sound choice [S-40].
- **Determinism pitfalls** (for replays/anti-cheat/rollback-adjacent features): FP divergence across compilers/architectures from transcendentals (libm), FMA contraction, -ffast-math, x87 vs SSE legacy; mitigations: fixed-point or tightly controlled doubles, no fast-math, identical iteration counts, seeded RNG call parity [S-40]. APEX kernel adopts this discipline (§4.2).
- **Latency targets**: own-car immediate (prediction); remote interpolation delay ≈1 RTT; HUD indicates high-jitter; bounded extrapolation ≤150 ms.

## 2. Performance evidence & feasibility

- Deck = custom AMD APU (Zen2 4C/8T, RDNA2, 8 GB), native 1280×800 60 Hz panel; Verified gate 30 fps @800p default [S-20, S-39]. Steam Machine ≈6× Deck performance (Valve GDC 2026 claim) [S-39]. → A 22-car 60 Hz racing sim with streaming is credible at 30 fps Deck / 60 fps PC min (ASSUMPTION until M1 evidence; budget table §11.2).
- Mobile: Unity 6.3 Adaptive Performance iOS thermal governors (auto 30/15 fps at Serious/Critical) [S-22] → mobile thermal envelope is manageable with governance + 30 fps fallback tiers.
- Memory: streaming-first asset strategy keeps initial iOS/Android installs within store cellular limits (limits vary by platform policy; re-verify pre-submission — U-06).
- Cross-platform: one simulation kernel; renderers scaled per tier; feature parity in gameplay (rulesets identical), parity *not* required in fidelity settings.

## 3. Multiplayer feasibility verdict (ASSUMPTION, testable at M2)

- 22-car lobbying, 60 Hz authoritative server, input-only uplink: feasible on commodity cloud VMs (CPU-bound, no GPU); per-lobby cost estimate to be validated in M2 load tests.
- Cross-play identity: platform SSO link (Steam OpenID ↔ device accounts) — standard practice (ASSUMPTION of regulatory acceptance; no store rule prohibits cross-play among Steam/iOS/Android).
- Anti-cheat without kernel drivers: server validation + replay verification — compatible with Deck Verified criteria (no incompatible AC) [S-20].
