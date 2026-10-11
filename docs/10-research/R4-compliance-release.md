# Research Dossier R4 — Compliance & release requirements

## 1. Age ratings (CONFIRMED)

- **IARC**: single questionnaire via participating storefronts (Steam/Google Play/App Store) → ESRB, PEGI, USK, ClassInd, GRAC, ACB ratings; re-take questionnaire on material content change; IARC ratings not displayable on non-participating storefronts [S-28].
- **Korea (GRAC)**: all distributed games need classification; open-market mobile can self-classify via store IRCBs (Google/Apple are designated); PC/Steam KR distribution needs counsel review (R-07/U-05); fees scale with size/genre/language (non-Korean ×1.5) [S-27]. Apple Korea region-specific ratings; GRAC RCN can override Apple-generated rating [S-29].
- **China**: NPPA ISBN approval + licensed local publisher required before monetization; geo-block CN at launch (R-08).
- **Brazil**: loot-box sales to under-18 banned 2026 (QAwerk compliance survey 2026-07-09) → no loot boxes anyway (§13.3).
- **Australia (ACB)**: can Refuse Classification — another reason to avoid gambling-adjacent mechanics.

## 2. Privacy & platform programs (CONFIRMED)

- Apple: privacy manifest + required-reason API declarations mandatory (since May 2024; policy ongoing 2026). Google Play: Data Safety form; families policy; Play Games Level Up program for improved fee tiers + featuring [S-26].
- GDPR/UK GDPR, COPPA + US state age-assurance laws (e.g., 2026 age-verification expectations noted by QAwerk survey), LGPD, PIPEDA: consent flows, DPA, ROPA, minimization, no ads SDK at launch.
- Cross-play account linking: platform SSO; child-safety gates on social features.

## 3. Release checklist (store-agnostic, condensed)

Steam: build review (Proton), Deck/Machine compat submission, store assets (capsule sizes per Valve), price/tier config, $100 app fee, content survey (mature content disclosure), EULA + privacy policy, cloud save (graphics settings excluded), Steam Input config default. iOS: App Store Connect metadata, privacy manifest, privacy nutrition labels, age questionnaire, export compliance, TestFlight beta, IAP setup (premium app = paid app, no IAP needed for base), iOS 26 SDK build, screenshots per device sizes. Android: Play Console listing, Data Safety, target API level per Play policy, AAB, base ≤200 MB + Play Asset Delivery for streaming, pre-launch reports, internal→closed→open testing tracks, region exclusions (CN initial), billing config. Both: IARC questionnaire, support/privacy URLs, DPA/processor agreements, security contact.

## 4. Certification/estimates (ASSUMPTION)

- Store review accounts: Apple review typically ≤48 h typical but plan 1 week buffer (ASSUMPTION); Steam build review days; no console certification at launch (consoles out of mission scope; design permits later ports via Unity).
