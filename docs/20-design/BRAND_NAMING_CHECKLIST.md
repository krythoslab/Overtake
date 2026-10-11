# Brand & Naming Checklist (design doc)

Applied to every piece of content, metadata, and copy before it ships. Referenced by MASTER_BLUEPRINT §14.1. Owner: legal + content leads. Rule: **fictional product; regulation-*faithful* mechanics only**.

## 1. Banned tokens (never in product, metadata, or marketing)

- Series/brand marks: "F1", "Formula 1", "Formula One", "FIA", "Grand Prix" as part of a series mark, "FOM", "FOWC", "Formula One World Championship".
- Team names and close variants (any current 2026–2028 entrant), driver names and likenesses, personnel names.
- Circuit names, nicknames, and close transliterations of any real circuit, including street-circuit monikers.
- Sponsor/trademark marks (tyre supplier, fuel/energy, timing, beverage, airline, watch, crypto, etc.).
- Real event names, host city names paired with racing terms ("X Grand Prix"), and event logos.
- Broadcast-derived graphics language ("box box", official timing screen layouts copied verbatim).

## 2. Allowed

- Generic motorsport vocabulary: "grand prix" as lowercase generic descriptor is still cleared case-by-case; "sprint" is generic but reviewed in combination.
- Rule-derived mechanics expressed as original names: Overtake Mode analog = "Slipstream Boost"; active aero modes = "Speed Mode / Corner Mode" (original names); detection/activation lines = "Boost Gates".
- Fictional team/driver/circuit names passing the confusion test (not evocative of a real entity).
- Generic compound archetypes: H / M / S (+ W / I). Real supplier and compound codes stay in research dossiers only.

## 3. Per-asset gate

For each track, car, livery, team, driver, UI string, store screenshot, trailer, and press copy:
1. Run banned-token scan (automated + human).
2. Confusion test: would a reasonable consumer associate this with the real series/team/circuit? If yes → rewrite.
3. Keyword-conflict check on store metadata (title/subtitle/keywords vs. Class 9 and Class 41 trademark databases).
4. Provenance log entry (for tracks: generator seed + editor actions; for art: WFH contract reference).
5. Sign-off recorded in content ticket; re-run on any post-launch content update.

## 4. Research boundary

Regulation facts live in `docs/10-research/` (real names permitted, marked "reference only"). Build inputs never read from real-event data; fictional calendars derive structure, not identity (§8.2–8.3).

## 5. Enforcement

Automated CI token scan on content JSON/strings/markdown store copy (docs tooling); legal re-review at every season pack; store updates re-run §3 gate (trademark drift happens).
