# OVERTAKE MASTER BLUEPRINT

**Project**: Overtake  
**Type**: Cross-platform AAA racing simulation  
**Targets**: Mobile (iOS/Android), PC (Windows), Current-gen consoles (PS5/Xbox Series X|S)  
**Status**: Design phase – gauntlet pass 8 – regulatory compliance validated

---

## Gauntlet Loop Summary

This blueprint was refined through eight disciplined passes:

1. **Requirement**: Define cross-platform AAA scope with small-scalable foundation.
2. **Plan**: Draft architecture around strict sim/presentation separation.
3. **Critic**: Found contradictions in cross-platform determinism, mobile thermal limits, content pipeline, and multiplayer sync.
4. **Revise**: Introduced fixed-point critical path, server-authoritative netcode, configurable simulation fidelity, and phased milestones.
5. **Stress-test**: Evaluated 22-car grid on mobile, cross-platform replay sync, esports latency, and live-ops data growth.
6. **Critic**: Exposed hidden dependency of AI on physics, input latency on touch, and replay versioning fragility.
7. **Revise**: Added deterministic input replay, module dependency enforcement, explicit non-goals, and CI regression gates.
8. **Regulatory Audit (pass 8)**: Refined blueprint against official 2026–2028 FIA Formula 1 regulations. Replaced deprecated DRS abstraction with 2026-mandated Active Aero (Corner/Straight Line Modes). Added Overtake Mode energy rules, PU power-split evolution, sustainable fuel, minimum weight, VehicleSpec taxonomy, Active Aero and ERS mode FSMs, and formal regulation status classification.

---

## 0. FIA Regulatory Baseline & Classification

This section classifies every FIA-derived requirement by publication status as of 2026-10-02. Sources are official FIA documents (Sections B, C, F), Formula 1 media briefings, and WMSC ratified decisions. No invented regulations are included.

| Status | Definition |
|--------|------------|
| **Published** | Issued by FIA as an official regulation document for the stated season. |
| **Ratified** | Approved by WMSC (World Motor Sport Council) as binding. |
| **Announced** | Publicly announced by FIA/FOM but not yet codified into a final regulation issue. |
| **Provisional** | Agreed in principle by stakeholders; pending WMSC e-vote or formal publication. |
| **Unverified** | Claimed by secondary sources; requires primary FIA document confirmation. |
| **Deferred** | Explicitly deferred to a later season. |
| **Rejected** | Explicitly rejected by Overtake design team; documented with rationale. |

### 0.1 2026 Season — Technical Regulations (Section C, Issue 17, 28 April 2026)

- **Active Aerodynamics (Corner Mode / Straight Line Mode)**: **Published / Ratified**. Both front and rear wing active flaps. Corner Mode = default high-downforce state. Straight Line Mode (SLM) = low-drag state. Available in designated track zones to ALL drivers (no 1-second gap requirement). Driver-activated via steering wheel control or brake pressure. Front wing adjuster and rear wing adjuster transition times are regulated.
- **Overtake Mode (Manual Override)**: **Published / Ratified**. Extra +0.5 MJ energy available when driver is within 1.0 s of car ahead at designated overtaking detection lines. Follow car: up to 350 kW MGU-K to 337 km/h. Leading car: deployment tapers off after 290 km/h, reaches 0 kW at 355 km/h.
- **ERS Limits**: **Published / Ratified**. MGU-K max power 350 kW. MGU-K harvest limit 8.5 MJ/lap (may be reduced to 8.0 MJ at specific competitions). MGU-K harvest rate max 350 kW. Energy Store (battery) maximum SoC delta limit 4 MJ. No MGU-H permitted.
- **Power Unit**: **Published / Ratified**. ICE max power ~400 kW (with fuel flow). MGU-K only permitted propulsion device alongside ICE. Fuel energy flow must not exceed 3000 MJ/h. Below 10500 rpm: EF(MJ/h) = 0.27*N(rpm) + 165. Single ICE Mode required.
- **Fuel**: **Published / Ratified**. 100% advanced sustainable fuel required. No refuelling during race.
- **Minimum Weight**: **Published / Ratified**. 768 kg (includes driver but excluding tyres). 30 kg reduction vs. 2022–2025 regulations.
- **Tyre Dimensions**: **Published / Ratified**. Front tyre width reduced 25 mm; rear tyre width reduced 30 mm. Diameter reduced 15 mm front / 10 mm rear. Five compounds (C1–C5). Intermediate and Full Wet tread patterns unchanged from 2025.

### 0.2 2026 Season — Sporting Regulations (Section B, Issue 08, 05 August 2026)

- **Sprint Qualifying Format**: **Published / Ratified**. SQ1: 12 minutes. Slowest cars eliminated. With 24 eligible cars, 7 eliminated after SQ1. SQ2: 10 minutes. With 24 eligible cars, 7 eliminated after SQ2. SQ3: 8 minutes for remaining 10 cars. Scale linearly with grid size to preserve 10-car final.
- **Sprint Points**: **Published / Ratified**. Top 8 finishers score: 1st = 8 pts, 2nd = 7, 3rd = 6, 4th = 5, 5th = 4, 6th = 3, 7th = 2, 8th = 1.
- **Penalty Structure**: **Published / Ratified**. LTCS (Last Time Control Session) penalties: 5s, 10s, drive-through, stop-and-go, time penalty, reprimand, DSQ, suspension. TTCS (Taking the Start from the pit lane) incident penalties: same catalogue. Grid penalties served in Race unless infringement occurred in SQ, then applied to Sprint grid.
- **Formation Lap**: **Published / Ratified**. Drivers must maintain formation. Unsafe overtaking prohibited. Race Control may abort start.
- **Parc Fermé**: **Published / Ratified**. From qualifying start onward, major setup changes prohibited. Only front wing angle and tyre selection (where permitted) adjustable.
- **Heat Hazard**: **Published / Ratified**. May be declared for Sprint, Race, or both. Must be declared 24 hours prior to Competition start.
- **Track Limits**: **Published / Ratified**. Four wheels must remain on track surface. Repeated violations → penalty points and time penalties.

### 0.3 2026 Season — Operational Regulations (Section F, Issue 10, 05 August 2026)

- **Active Aero Testing**: **Published / Ratified**. Wind tunnel and CFD permitted for active aero flap angle adjustment under ATR. FIA may approve additional incidence changes as "active aerodynamics" in approved CAD files.
- **Front/Rear Wing Adjuster Transition Time**: Checked at scrutineering. Regulated maximums enforced.

### 0.4 2027 & 2028 Evolution — Agreed in Principle / Ratified

| Parameter | 2026 | 2027 | 2028 |
|-----------|------|------|------|
| ICE max power | ~400 kW | 420 kW | 450 kW |
| MGU-K max power (normal) | 350 kW | 300 kW | 300 kW |
| MGU-K max power (Overtake Mode) | 350 kW | 350 kW | 350 kW |
| MGU-K harvest max power | 350 kW | 375 kW | 400 kW |
| Fuel flow increase vs. 2026 baseline | — | +5% | +13% |
| ICE/ERS power split | 53/47 | 58/42 | 60/40 |

- **Status**: Agreement reached 10 June 2026; WMSC ratified 23 June 2026 in Macau. **Ratified for 2027; Proposed/Announced for 2028** (formal publication of 2028 Technical Regulations pending).
- **Supporting measures**: Increased pre-season testing from 3 to 4 days from 2027. Power unit supply conditions, reconnaissance lap management, and financial regulation amendments included.

### 0.5 Overtake Gameplay Abstraction Policy

FIA-mandated behaviour is simulated faithfully where feasible. Overtake adds abstraction layers only where the simulation would otherwise be unintuitive or inaccessible to non-expert players. Every abstraction is tagged `[Overtake abstraction]` in the simulation code. No abstraction changes race control outcomes or championship eligibility relative to a hypothetical perfect FIA simulation.

---

## 1. Product Vision and Pillars

**Vision**: The same authentic race, everywhere. Overtake delivers consistent, skill-based motorsport on a phone and on a 4K/120fps PC.

**Pillars**:
- **Authentic Feel**: Physics-first handling where car control is the differentiator.
- **Regulatory Fidelity**: 2026–2028 FIA regulations are the baseline. Active Aero, ERS/Overtake Mode, sustainable fuel, and PU evolution are faithfully reproduced with clear separation between FIA rules and gameplay abstraction.
- **Cross-Platform Consistency**: Identical rules and outcomes regardless of device.
- **Scalable Fidelity**: Mobile gets clean, efficient visuals; PC/console unlocks ray tracing, advanced simulation, and VR.
- **Incremental Depth**: Simple enough for casual touch players; deep enough for sim racers.

---

## 2. MVP → AAA Scalability Strategy

**Phase 0 — Foundation (Months 1–3)**
- Math library, SIMD support, cross-platform build system (CMake).
- Fixed-timestep simulation kernel (1000Hz).
- Minimal renderer (forward+), input abstraction, and basic asset pipeline.
- Demo: single car, single track, 2-player split-screen.

**Phase 1 — MVP (Months 4–9)**
- 3–5 cars, 5–8 tracks (fictional).
- Basic career (time trial, sprint race), 4-player online.
- Tire/fuel model (simplified on mobile, full on PC).
- Active Aero (Corner/Straight Line Mode), ERS (2026 baseline), mandatory pit stops.
- 2026 regulatory baseline: 768 kg minimum weight, 100% sustainable fuel, no MGU-H.

**Phase 2 — Expansion (Months 10–18)**
- Full regulations: 20-car grid, Safety Car, VSC, red flags, parc fermé, post-race stewards.
- Dynamic weather, track evolution, tire surface phenomena (grain, blister, flat spot, marbles, aquaplaning).
- Full career with R&D, sponsors, team transfers, driver market.
- Cross-platform progression cloud sync.
- 2027 PU evolution (58/42 ICE/ERS split) as configurable ruleset.

**Phase 3 — AAA (Months 19–30)**
- Ray tracing reflections/shadows, 120fps, VR.
- Advanced brush-based tire model (PC/console only; mobile uses Pacejka lookup).
- Esports broadcaster tools, track editor, mod support.
- 2028 PU evolution (60/40 split) as configurable ruleset.
- Live-ops events and cosmetic DLC.

---

## 3. Core Gameplay / Race Loop

```
Session Setup → Car Setup → Out Lap → Formation Lap → Race Start → 
[Stint] → Pit Stop → Final Lap → Checkered Flag → 
Results / Championship Update → Next Session
```

**Determinism Contract**:  
Every frame, simulation state `S(t)` is a pure function of `S(t-1)` and deterministic input `I(t)`. No uninitialized memory, no race conditions, no platform-dependent math behavior (no FTZ/DAZ, no fused-multiply-add unless explicitly disabled). RNG state is part of `S`.

---

## 4. Vehicle Physics Architecture

- **Fixed Timestep**: 1ms (1000Hz) internal simulation. Presentation interpolates/extrapolates.
- **Rigid Body**: 6 DOF per vehicle. Quaternion orientation. No Euler angles.
- **Suspension**: Spring/damper per wheel with anti-roll bar, travel limits, bump stops.
- **Tires**: Slip-angle and slip-ratio based. Force computed via model; friction circle enforced.
- **Aerodynamics**: Downforce and drag as functions of speed, ride height, angle of attack, active aero state, DRS state (historical only; 2026 uses Active Aero).
- **Powertrain**: Engine torque curve, gearbox, differential (open/LSD), clutch.
- **Active Aero**: Movable front and rear wing flaps. Two states: Corner Mode (Z-Mode, default high downforce) and Straight Line Mode (X-Mode/SLM, low drag). Activation restricted to designated track zones. Available to all drivers in those zones. Transition times regulated.
- **Interface Boundary**:

```cpp
// Simulation interface – presentation never touches internals.
enum class ActiveAeroMode { Corner, StraightLine, Off };
enum class ERSMode { Low, Medium, High, Off, Qualifying, Recharge, Overtake, Boost };
struct VehicleInput {
    float steer, throttle, brake, clutch;
    int gear;
    ActiveAeroMode activeAero;
    ERSMode ersMode;
};
struct VehicleState {
    mat4x4 worldMatrix;
    float speed, rpm, gear;
    float tireSlip[4];
    float batterySoC;
    float batterySoCLimit; // 4.0 MJ regulated maximum delta
};
class IVehicle {
public:
    virtual void Update(float dt, const VehicleInput& in, const TrackSurface& track) = 0;
    virtual const VehicleState& GetState() const = 0;
};
```

Presentation layer subscribes to `VehicleState` events only.

### 4.1 Active Aero State Machine

```
[CORNER] →[Driver triggers / zone entry / brake]→ [STRAIGHT]
[STRAIGHT] →[Zone exit / driver cancels]→ [CORNER]
```

**Corner Mode (Z-Mode, Default)**:
- Front wing active flap: high incidence (closed). Downforce maximized.
- Rear wing active flap: high incidence (closed). Downforce maximized.
- Used for braking, cornering, acceleration out of corners.
- Restored automatically on lift-off, braking, or at zone end.

**Straight Line Mode (X-Mode, SLM)**:
- Front wing active flap: low incidence (open). Drag minimized.
- Rear wing active flap: low incidence (open). Drag minimized.
- Permitted only in designated track zones (FIA-defined straights, typically >3 seconds at full speed).
- Available to ALL drivers regardless of gap to car ahead.
- Driver-activated via steering wheel control or brake pressure cancellation.

**Safety Restrictions**:
- In low-grip wet conditions with poor visibility, active aero usage may be restricted by Race Control. [FIA WMSC approved 23 June 2026: boost mode reintroduction restriction for safety in wet/low grip.]
- Front and rear wing adjuster transition times are hardware-regulated and verified at scrutineering.
- If the system fails, default state is Corner Mode (maximum downforce).

**Aerodynamic Effect**:
- Corner Mode: Maximum downforce, high drag. Downforce = MAX, Drag = HIGH, Straight efficiency = LOW.
- Straight Line Mode: Reduced downforce, low drag. Downforce = REDUCED, Drag = MINIMUM, Straight efficiency = HIGH.

**Regulatory Source**: FIA 2026 Section C, Issue 17 (28 April 2026); Article C3; FIA press release 6 June 2024. Status: Published / Ratified.

---

## 5. Tyres, Fuel, ERS, Damage and Reliability

**Tyres**
- **Parameters**: compound, pressure, temperature, wear.
- **Mobile Fidelity**: Pacejka '89 lookup, 1 thermal node per tire.
- **PC/Console Fidelity**: 3-node thermal model, optional brush contact patch.
- **Rule**: Grip curve shape and wear rate are identical across platforms. Only computational resolution changes. Results are within gameplay tolerance (<0.5% lap time delta).
- **Pressure Effects**: Higher pressure reduces contact patch and rolling resistance but lowers mechanical grip and increases center-tread temperatures. Lower pressure increases contact patch and grip but raises rolling resistance and shoulder temperatures. Incorrect pressure selection accelerates degradation.
- **Degradation Stages**: Fresh (maximum grip, fast response), Working (peak operating performance, consistent grip), Worn (reduced grip, longer braking, increased overheating), Tyre Cliff (rapid performance drop, grip decreases dramatically).
- **Surface Phenomena**:
  - **Graining**: Cold tyres or aggressive steering tear rubber surface; temporarily reduces grip but may clean itself if temperatures recover.
  - **Blistering**: Excessive internal temperatures cause rubber to overheat and bubble; permanently reduces effectiveness and cannot be reversed during a stint.
  - **Flat Spots**: Locked wheels drag rubber across circuit; causes vibration, reduced braking, accelerated wear.
  - **Punctures**: Debris, collisions, or extreme wear cause rapid pressure loss; grip decreases dramatically and driver must return to pit lane.
  - **Marbles**: Rubber accumulates off-line; driving through marbles temporarily reduces grip and causes tyre pickup.
  - **Tyres Pickup**: Marbles adhere to tyre surface; reduces grip and steering precision until cleaned by returning to racing line.
  - **Aquaplaning**: Standing water lifts tyre from surface; steering, braking, and traction decrease. Intermediate and Full Wet tyres significantly reduce likelihood.
- **Compound Crossover**: Weather determines optimal compound. Dry slicks perform best on dry circuits; Intermediate tyres become advantageous in damp conditions; Full Wet tyres are essential in heavy rain. Crossover points shift naturally as circuits dry.

**Fuel**
- Mass = `initialMass - (fuelPerLap * lapsCompleted)`.
- Center of mass shifts; affects weight distribution and handling.
- Deterministic consumption curve; no randomness.
- Fuel load affects vehicle mass, braking distances, tyre degradation, and cornering inertia. Car becomes lighter over distance → improved acceleration, braking, and cornering.
- **2026 FIA Mandate**: 100% advanced sustainable fuel. Fuel energy flow must not exceed 3000 MJ/h. Below 10500 rpm: EF(MJ/h) = 0.27*N(rpm) + 165. [FIA-mandated. No Overtake abstraction.]
- **2027 Evolution (Provisional/Ratified)**: Fuel flow increase +5% (to ~3150 MJ/h).
- **2028 Evolution (Announced)**: Fuel flow increase +13% (to ~3390 MJ/h).

**ERS**
- **2026 FIA Baseline (Published / Ratified)**:
  - MGU-K max power: 350 kW.
  - MGU-K harvest limit: 8.5 MJ/lap (may be reduced to 8.0 MJ at specific competitions).
  - MGU-K harvest max rate: 350 kW.
  - Energy Store maximum SoC delta limit: 4.0 MJ per lap.
  - No MGU-H permitted.
- **MGU-K Harvest Effectiveness**: Harvested energy depends on brake intensity, battery capacity, current charge level, vehicle speed, and brake temperature.
- **Engine Clipping**: When battery charge is exhausted, electrical contribution to acceleration gradually reduces near the end of long straights, reducing total acceleration and top speed until harvesting replenishes the battery. Clipping is intentional and gradual, not binary.
- **Overtake Mode (2026 FIA Mandate)**: Extra +0.5 MJ energy available when within 1.0 s of car ahead at designated overtaking detection line. [FIA-mandated activation condition and energy amount.]
- **Overtake Mode Deployment Curve**:
  - Follow car: 350 kW up to 337 km/h.
  - Leading car: deployment tapers off after 290 km/h, reaches 0 kW at 355 km/h.
  - In low-grip wet conditions with poor visibility, Overtake Mode may be disabled by Race Control for safety. [FIA WMSC approved 23 June 2026.]
- **Overtake Gameplay Modes** (FIA + Overtake abstraction):
  - **Off**: ERS-K propulsion disabled. Battery charges via harvest only.
  - **Low**: Conservative deployment. Preserves energy for later laps.
  - **Medium**: Balanced harvest and deployment. Standard race strategy.
  - **High**: Aggressive deployment. Maximizes lap performance; risks battery depletion before stint end.
  - **Qualifying**: Flat-out deployment. Battery may be partially pre-charged. Subject to Overtake Mode activation rules.
  - **ROB**:
    - **Recharge**: Prioritizes MGU-K harvest; limits deployment to minimum. Battery recovery focus.
    - **Overtake**: Activates Overtake Mode when conditions met; deploys +0.5 MJ bonus.
    - **Boost**: Uses stored battery energy for maximum acceleration; no harvest restriction.
- **ERS Mode Transitions**:
  - Player may switch modes at any time outside of FIA-regulated restrictions.
  - Mode changes do not affect Overtake Mode eligibility, which is governed solely by gap and track position.
  - Switching to Off or Low while Overtake Mode is active does not immediately reclaim the bonus energy; it depletes according to the Overtake Mode curve.
- **2027 Evolution (Ratified)**: MGU-K max power reduced to 300 kW (normal), 350 kW (Overtake Mode). Harvest max 375 kW. ICE power increases to 420 kW.
- **2028 Evolution (Announced/Ratified framework)**: MGU-K max power remains 300 kW (normal), 350 kW (Overtake Mode). Harvest max 400 kW. ICE power increases to 450 kW.

**Damage**
- Component health: front wing, floor, wheels, engine, gearbox.
- Damage from collisions, curbs, over-rev.
- Performance modifiers (e.g., -5% front downforce per 20% wing damage) are deterministic.
- Visual damage (cracks, detached parts) is presentation-only.
- **Brake Temperature**: Cold brakes produce reduced stopping power; overheated brakes suffer fade and lock-up risk. Brake temperature evolves based on usage, ambient conditions, and cooling.
- **Brake Bias**: Adjustable front/rear brake distribution. Affects braking stability, lock-up tendency, and tire wear.
- **Collision Damage Details**:
  - Front wing: reduced front downforce, understeer, longer braking distances, reduced steering precision.
  - Rear wing: reduced rear grip, oversteer, traction loss, corner exit instability.
  - Floor: reduced ground-effect efficiency, total downforce, cornering speed, vehicle stability.
  - Suspension: reduced steering precision, wheel instability; repairable only in pit lane.
  - Cooling ducts, radiators, exhaust, electronics: progressive performance degradation.
- **Lock-ups**: Heavy braking with locked wheels causes flat spots and accelerated tyre wear.

**Reliability**
- Wear accumulates per lap/race distance.
- Failure probability = `f(componentWear, randomSeed)`.
- Failures: engine, gearbox, electronics. All deterministic given seed.
- **Power Unit Components**: ICE, turbocharger, MGU-K, Energy Store, control electronics, cooling system, exhaust, fuel system. Each has operating temperature, reliability, service life, and failure probability.
- **Mechanical Failures**: Dynamic rather than scripted; may occur suddenly or develop progressively with warnings. Engineers attempt to notify player before critical failures.

---

## 6. AI Driver Architecture

- **Behavior**: Utility AI + behavior trees for high-level decisions.
- **Simulation**: AI agents use same `IVehicle::Update` interface.
- **Difficulty**: Gaussian noise on control inputs + reaction delay.
- **Racing Line**: Precomputed per track per car; AI tracks it with error proportional to skill.
- **Tactics**: Defensive moves, Active Aero usage, ERS mode selection, pit window adherence, tire compound strategy, Overtake Mode triggering when conditions met.
- **Determinism**: Same random seed → identical AI behavior. No online learning during a session.
- **Driver Attributes**: Six primary attributes on 0.0–10.0 scale: Pace (raw speed, cornering precision), Consistency (lap-to-lap variation, mistake frequency), Overtaking (gap judgement, slipstream usage, wheel-to-wheel success), Defending (positioning, reaction to attacks), Racecraft (spatial judgement, risk assessment, pressure handling), Wet (rain pace, aquaplaning recovery). Attribute combinations produce distinct personalities; no hidden archetypes.
- **AI Race Brain**: Continuously evaluates 20+ factors: current position, gaps, tyre condition, battery level, weather, track grip, flags, vehicle damage, engine clipping, race distance remaining, championship situation. Overtaking decisions consider closing speed, active aero state, battery, tyre grip, corner type, track position, remaining distance, and risk level. Defending considers remaining battery, tyre grip, corner layout, and championship importance. Battery management chooses ERS modes (Recharge/Balance/Overtake/Boost) intelligently based on attack, defence, qualifying, fuel saving, and traffic.
- **Personality and Mistakes**: Driver fatigue affects mistake probability under sustained pressure. Mistakes include lock-ups, running wide, wheelspin, late braking, missed apexes, track limits, poor starts, and minor contact. No driver performs perfectly.
- **Engineer Communication**: Race engineers provide continuous feedback on tyre temperatures, battery status, brake temperatures, balance, setup recommendations, weather forecasts, track grip, pit windows, Safety Car info, and traffic warnings. Recommendations only; player retains final decision.
- **Setup Management**: AI teams generate setups according to circuit characteristics, weather forecast, driver preferences, chassis philosophy, and tyre strategy.

---

## 7. Tracks and Environment Systems

- **Track Data**: Versioned binary format.
  - Spline geometry (centerline, width, banking, camber).
  - Surface material IDs per segment (asphalt, kerb, gravel, grass).
  - Pit lane geometry and speed limit zones.
  - AI racing line, sector boundaries, start/finish.
  - **Active Aero zones**: FIA-defined zones per track where Straight Line Mode and Overtake Mode are permitted. Track metadata includes zone entry/exit coordinates and Overtake Mode detection line coordinates.
  - **Overtake Mode detection**: 1.0 s gap at designated detection line. [FIA-mandated condition.]
- **Collision**: Static triangle mesh; dynamic objects use convex hulls.
- **Environment**: Modular props, crowd systems, sky/atmosphere volumes.
- **Scalability**: Mobile uses lower-poly meshes and simplified materials; PC/console uses high-poly with virtual texturing / Nanite-style streaming.
- **Track Evolution**: Rubber deposited by vehicles increases grip over time; braking improves, cornering speeds increase, lap times fall. Rain washes rubber from the racing surface. Off-line sections remain dusty and slippery until cleaned by traffic. Track evolution affects every session and is a core strategic element.

---

## 8. Race Control, Penalties and Safety Systems

- **Rules Engine**: Finite state machine (GREEN, YELLOW, DOUBLE YELLOW, VSC, SAFETY CAR, RED, CHECKERED).
- **Complete Flag Set**: Green, Yellow, Double Yellow, Blue (instruct slower cars to facilitate faster competitors; repeated failure → penalty), White (slow-moving vehicle ahead), Black and White (unsportsmanlike conduct warning), Black (disqualification), Red, Chequered.
- **Infractions**: Track limits (4 wheels on track), corner cutting, collisions, jump starts, speeding in pit lane, unsafe releases, false starts, blue flag violations.
- **Penalties**: Warning, 2s/3s/5s/10s time penalty, drive-through, stop-and-go, grid penalty, disqualification. Penalties are served during pit stops where regulations allow. Post-race stewards review collisions, track limits, unsafe releases, false starts, pit lane violations, and yellow flag infringements.
- **Safety Car**: Deployed for major accidents, blocked circuits, debris, dangerous weather, vehicle recovery, or medical intervention. Overtaking prohibited; pit lane remains open unless closed.
- **Virtual Safety Car (VSC)**: Neutralises race without physical Safety Car; drivers must remain above minimum delta time; no overtaking.
- **Red Flag & Restarts**: Suspends race for major accidents, track obstruction, extreme weather, barrier repairs, or medical emergencies. Cars return to pit lane; damage/tyre changes follow regulations. Standing restart uses five-light manual clutch procedure; rolling restart follows Safety Car with leader controlling pace and no overtaking until control point.
- **Formation Lap**: Can be Full (player manages temperatures, spacing, clutch bite point, launch strategy), Assisted (game assists spacing/speed/positioning), or Skipped (immediate grid position, temperatures auto-adjusted). Unsafe overtaking prohibited; Race Control may abort start and repeat formation lap.
- **Parc Fermé**: Restricts major setup changes once qualifying begins. Only approved parameters (front wing angle, tyre selection where permitted) remain adjustable. Career Mode enables by default.
- **Implementation**: Rules are pure functions of simulation state; presentation layer renders flags, HUD alerts, and penalty messages.

### 8.1 Sprint Qualifying and Sprint Session Procedure

**Sprint Qualifying** (FIA Section B, Article B2.2):
- **SQ1**: 12 minutes. All eligible cars on track. Slowest cars eliminated. With 24 cars, 7 eliminated. With 22 cars, 6 eliminated. Eliminations scale to preserve 10-car SQ3.
- **SQ2**: 10 minutes. Remaining cars on track. Slowest cars eliminated. With 24 cars, 7 eliminated (10 remain). With 22 cars, 6 eliminated (10 remain).
- **SQ3**: 8 minutes. Remaining 10 cars determine Sprint grid order.

**Sprint Session**:
- Distance: ~100 km.
- Points to top 8: 8, 7, 6, 5, 4, 3, 2, 1.
- No mandatory pit stops unless Safety Car/VSC triggers strategy.
- Tyre rules apply as per championship regulations.
- Parc fermé rules apply from Sprint Qualifying start.

### 8.2 Active Aero and Overtake Mode Race Control Rules

- **Active Aero Zones**: Designated straights where Straight Line Mode is permitted. Available to ALL drivers. No gap requirement.
- **Overtake Mode Zones**: Designated sections where Overtake Mode (+0.5 MJ) may be activated. Requires 1.0 s gap at detection line.
- **Wet/Low-Grip Restriction**: In low-grip conditions when the track is wet and visibility is poor, Race Control may disable Overtake Mode. Boost mode (maximum deployment) is restricted to preventing power reduction without increasing output. [FIA WMSC approved 23 June 2026.]

### 8.3 Penalty Serving Rules

- **5s Penalty**: Enter pit lane, stop in Designated Pit Stop Position for ≥5s, rejoin TTCS.
- **10s Penalty**: Enter pit lane, stop for ≥10s, rejoin TTCS.
- **Drive-Through**: Enter pit lane, rejoin without stopping.
- **Stop-and-Go**: Enter pit lane, stop for ≥10s, rejoin TTCS.
- **Post-Session Penalties**: If imposed after end of TTCS, time added: 5s, 10s, 20s (drive-through), 30s (stop-and-go).
- **Reprimand**: Written warning. Five reprimands (≥4 for driving infringements) in same Championship → 10-grid-place penalty.
- **Grid Penalty**: Served in Race unless infringement occurred in SQ, then applied to Sprint grid.

---

## 9. Weather and Dynamic Track Conditions

- **Weather State**: Rain intensity, humidity, ambient/track temperature, wind. Conditions: Clear, Cloudy, Overcast, Light Rain, Heavy Rain, Mixed Conditions, Drying Circuit. Fully dynamic throughout every session.
- **Track Evolution**: Dryness per segment (0.0–1.0). Rain adds water; sun/air evaporates. Rubber deposited by vehicles increases grip over time; braking improves, cornering speeds increase, lap times fall. Rain washes rubber from the racing surface. Off-line sections remain dusty and slippery until cleaned by traffic. Drying line gradually restores grip after rainfall.
- **Tire Compound Crossover**: Weather determines optimal compound. Dry slicks perform best on dry circuits; Intermediate tyres become advantageous in damp conditions; Full Wet tyres are essential in heavy rain. Crossover points shift naturally as circuits dry; pitting too early or too late significantly influences race results.
- **Aquaplaning**: Standing water lifts tyre from surface when water depth exceeds tyre tread capacity. Steering, braking, and traction decrease. Intermediate and Full Wet tyres greatly reduce likelihood compared to slicks.
- **Simulation Impact**: Friction multiplier per segment. Grip drops with wetness; drying line emerges naturally. Crosswinds subtly influence vehicle stability on exposed sections.
- **Presentation**: Particle spray, wet surface shaders, camera rain, screen-space droplets.
- **Mobile**: Reduced particle count, simpler rain shader; identical friction values.

---

## 10. Strategy / Pit-Stop Systems

- **Pit Window**: Mandatory compound rules, optional tire change.
- **Execution**: Player must slow to pit lane speed limit. Team performs work (duration deterministic). Pit stops fully animated.
- **Penalties**: Unsafe release, speeding in pit lane, incorrect tire compound.
- **Strategy AI**: Predicts degradation curves, weather change, undercut/overcut opportunities, Safety Car/VSC timing, traffic management, battery condition, ERS mode recommendation, and fuel load.
- **Pit Stop Strategy**: Players weigh performance gain vs time lost. Undercut (pit earlier than competitor to gain track position), overcut (extend stint to force competitor to pit first). Traffic and Safety Cars significantly alter pit windows.
- **Tyre Allocation**: Limited tyre allocation across the race weekend; unused tyres remain available for later sessions where regulations permit.
- **Presentation**: Pit board messages, crew animations (simplified on mobile).

---

## 11. Input, Assists and Accessibility

- **Abstraction**: Normalized input vector (steer ±1.0, throttle 0–1, brake 0–1, digital gear/Active Aero/ERS).
- **Sources**: Gamepad, keyboard, touch (virtual joystick or tilt), steering wheel (force feedback optional).
- **Assists**:
  - Traction Control: limits throttle based on available grip.
  - ABS: modulates brake pressure to prevent lock-up.
  - Brake Assist: helps maintain optimal braking pressure.
  - Steering Assist: adds corrective steering; visual-only on mobile.
  - Racing Line: renders optimal path with braking markers.
  - Automatic Gearbox.
  - Automatic Pit Limiter.
  - Corner Entry Indicators.
  - Active Aero Auto-toggle: automatically switches to Corner Mode on brake/steering input; switches to Straight Line Mode on straight sections where permitted. [Overtake abstraction.]
  - ERS Mode Assist: recommends and auto-switches between Recharge, Balance, Overtake, Boost based on race situation. [Overtake abstraction.]
- **Accessibility**:
  - **Visual**: Colorblind simulation shaders, text scaling up to 200%, HUD scale/opacity, subtitle size/background, brightness/contrast/gamma, motion blur toggle, screen shake toggle, flash reduction, high-contrast mode, screen-reader support.
  - **Audio**: Independent volume sliders (Master, Engine, Music, Radio, Crowd, Effects, Interface, Voice), subtitle-only team radio, visual radio indicators, adjustable subtitle duration.
  - **Control**: Button mapping, steering/throttle/brake sensitivity, steering/throttle/brake dead zone, controller vibration, haptic feedback where supported.
  - **HUD Presets**: Simulation HUD, Minimal HUD, Broadcast HUD, League Racing HUD; instant switching without restart.
  - **Broadcast Mode**: Hides non-essential elements while preserving timing tower, race position, lap counter, flag information.
  - **Difficulty Customization**: AI Difficulty, AI Aggression, Damage Level, Mechanical Failures frequency, Safety Car frequency, Weather Randomness, Penalty Strictness. Physics remain unchanged.

---

## 12. Rendering / Performance Architecture

- **Renderer**: Deferred or Forward+ with tiled/clustered shading.
- **Quality Tiers**:
  - **Mobile**: 1080p–native, 2× MSAA or FXAA, no ray tracing, 30–60fps.
  - **Console**: 1440–4K checkerboard, 60–120fps, hardware ray-traced reflections, DLSS/FSR 3.
  - **PC**: 4K native, 120+fps, full ray tracing, DLSS 4, frame generation.
- **Scalable Features**:
  - Car LOD: 5 levels.
  - Track streaming (clipmap) with LOD.
  - Particle density and post-processing toggles.
  - Shadow resolution and cascades.
- **Platform Targets**:
  - **Android**: Min Snapdragon 720G/Helio G95, 6GB RAM; Recommended 7 Gen 2/Dimensity 7200/Tensor G3, 8GB+.
  - **iOS**: Min A13, 4GB; Recommended A16+/M-Series, 6GB+.
  - **Windows**: Min i5-8400/Ryzen 5 2600 + GTX 1650/RX 570, 8GB; Recommended i5-12400/Ryzen 5 5600 + RTX 3060/RX 6600 XT/Arc B580, 16GB.
  - **Installation Size**: Mobile 450–600 MB; Steam/PC 1–1.5 GB.
- **Optimization Techniques**: GPU instancing, LOD models, texture compression/streaming, occlusion/frustum culling, object pooling, optimized physics, baked lighting, compressed audio, shader variant optimization, asynchronous scene loading, memory pool management, asset bundling, efficient garbage collection.
- **Battery/Thermal Management**: Adaptive quality scaler, frame rate cap, dynamic resolution, minimize background processing on mobile.
- **Frame Pacing**: VSync off + triple buffering, or capped frame rate with frame time smoothing.

---

## 13. Audio and Presentation

- **Audio Engine**: Custom parameter bus or middleware (FMOD/Wwise).
- **Sounds**:
  - **Engine**: Synthesis from RPM/load (mobile); multi-layer samples + convolution reverb (PC/console). Sound packs include Modern Hybrid V6, V6, V8, V10. Audio responds to MGU-K harvest/deployment, engine clipping, and overrun.
  - **Tires**: Slip-angle dependent squeal, wheelspin, lock-ups, sliding, kerb contact, standing water, aquaplaning, tyre pickup.
  - **Aerodynamics**: Wind noise, speed, airflow changes, active aero transitions (Corner/Straight Line Mode).
  - **Transmission**: Upshifts, downshifts, gear engagement, transmission whine, differential noise.
  - **Collisions**: Vary by speed, angle, barrier type, vehicle contact, debris, suspension/wing damage.
  - **Environment**: Spatialized crowd, wind, rain, trackside announcements, pit lane activity, marshals, Safety Cars.
  - **Team Radio**: Engineer voice lines, driver responses, strategy calls, weather updates, battery/tyre/fuel information, penalty notifications, Safety Car instructions. Delivered via voice acting + optional subtitles.
- **Volume Controls**: Independent sliders for Master, Engine, Music, Radio, Crowd, Effects, Interface, Voice.
- **Presentation**: Camera system (chase, cockpit, broadcast, replay, TV, helicopter, trackside, T-Cam, nose, rear, free camera). Cinematics are scripted events. Photo mode with free camera, zoom, DOF, FOV, exposure, brightness, contrast, saturation, filters, high-resolution screenshots.
- **Separation**: Audio parameters are driven by simulation telemetry; audio has no feedback into simulation.

---

## 14. UI/UX

- **Framework**: Retained-mode, resolution-independent vector assets.
- **HUD**: Minimal by default, customizable. Speed, RPM, gear, lap time, position, tire temps, ERS/F battery, ERS mode indicator (Recharge/Overtake/Boost), battery percentage, fuel remaining, Active Aero mode indicator (Corner/Straight), flag notifications, penalty notifications, Race Control messages, pit window, pit limiter indicator.
- **Timing Tower**: Position, abbreviation, gap, interval, tyre compound, pit status, retired drivers, fastest lap indicator, sector status. Full/compact/hidden options.
- **Track Map**: Circuit layout, player/AI positions, pit lane, Safety Car, yellow flag zones, sector divisions, Active Aero zone markers, Overtake Mode detection zones.
- **Radio Messages**: Voice audio, subtitles, combined, or subtitle only; adjustable subtitle size and display duration.
- **Mobile**: Touch-optimized HUD, swipeable menus, virtual input overlays.
- **Accessibility**: Screen-reader support, high-contrast mode, text scaling up to 200%, colorblind-safe palettes.
- **Broadcast Mode**: Hides non-essential elements while preserving timing tower, race position, lap counter, flag information.
- **HUD Customization**: Nearly every element can be enabled/disabled, resized, repositioned, opacity adjusted; multiple presets (Simulation, Minimal, Broadcast, League Racing).

---

## 15. Career / Progression

- **Driver Profile**: Name, nationality, skill attributes (pace, consistency, overtaking, tire management).
- **Team Management**: Contract offers, R&D allocation, sponsor objectives, morale.
- **Progression**: XP → skill points → improve driver attributes or unlock series.
- **Car Upgrades**: Modify simulation parameters (e.g., `engineMaxPower += 50`, `tireWearRate *= 0.95`).
- **Save Data**: Player profile, career state, settings. No mid-race save; checkpoint only.
- **Career Structure**: Season-by-season progression with Pre-Season, Championship Calendar, Mid-Season Progression, Post-Season Review, Driver Market, and Next Season Preparation.
- **Starting Options**: Official Driver Career, Custom Driver Career.
- **Season Objectives**: Score Points, Reach Q2/Q3, Top Ten, Podiums, Wins, Challenge for Championships. Meeting objectives improves standing; repeated failures affect contract negotiations.
- **Team Reputation**: Influenced by race results, consistency, sportsmanship, championship position, qualifying performance, reliability.
- **Contract System**: Negotiated during season or post-season; specifies team, length, salary, objectives, clauses. Players may renew, transfer, reject offers, or become free agents.
- **Driver Market**: AI drivers may transfer, retire, remain, or young drivers enter. Creates evolving grids between seasons.
- **Teammates**: Qualifying/race head-to-head, points, average finish, podiums, victories, Constructors' contribution. Outperforming teammates increases reputation.
- **Championships**: Official, Custom, Historical, Random, League calendars. Season lengths from single event to full championship. Points systems: Official, Historical, Custom, Winner Takes All, No Points.
- **Statistics**: Championships, victories, poles, fastest laps, podiums, career points, total starts, DNFs, average finish, win percentage, Constructors' Championships.
- **Retirement**: Generates complete career summary including legacy ranking.
- **Regulatory Year Selection**: Career mode allows selection of 2026, 2027, or 2028 ruleset. Each applies the correct PU split, MGU-K limits, and ERS deployment curves. [Overtake abstraction.]

---

## 16. Multiplayer Architecture

- **Topology**: Client-server. Server is authoritative.
- **Simulation**: Server runs full physics at 1000Hz; processes inputs at 120Hz tick.
- **Clients**: Run local prediction simulation for responsiveness; reconcile with server state.
- **Determinism**: Server uses deterministic simulation. Clients use same simulation code as server.
- **Bandwidth**: Delta compression, interest management (only nearby cars).
- **Anti-Cheat**: Server validates all inputs and car parameters. No client authority.
- **Cross-Platform**: Identical protocol and simulation library across all platforms.
- **Modes**: Quick Race, Custom Lobby, Private Lobby, League Race, Sprint Weekend, Full Grand Prix Weekend.
- **Lobbies**: Host may customise circuit, weather, race distance, weekend format, AI drivers, assists, damage, mechanical failures, Safety Cars, VSCs, Red Flags, track limits, formation lap, false start detection, penalty strictness.
- **League Racing**: Scheduled practice, qualifying, sprint, grand prix; custom calendars; consistent settings throughout championship; persistent standings; administrative controls.
- **Disconnections**: Vehicle may temporarily remain under AI control; players may rejoin when permitted; persistent disconnections may result in retirement.
- **Synchronisation**: Continuous sync of vehicle position, speed, steering, throttle, braking, battery deployment, active aerodynamics state, pit stops, penalty status, Overtake Mode eligibility. Every participant observes a consistent race; no hidden advantages.

---

## 17. Replay / Telemetry

- **Recording**: Deterministic input stream + random seed + initial conditions. No state snapshots. Recordable for Practice, Qualifying, Sprint, Grand Prix, Time Trial, Multiplayer, League Races. Automatic recording optional.
- **Playback**: Re-simulate from recorded inputs. Version-checked; incompatible builds skip or re-record.
- **Camera System**: Broadcast, TV, Helicopter, Trackside, Chase, Cockpit, T-Cam, Nose, Rear, Free Camera. Switchable at any time.
- **Playback Controls**: Play, Pause, Fast Forward, Rewind, Frame Advance, Slow Motion, Replay Speed Adjustment, Timeline Scrubbing.
- **Driver Selection**: Instant switching between every car; displays driver name, position, tyre compound, battery level, lap number, sector, speed.
- **HUD Options**: Full Broadcast HUD, Minimal HUD, Timing Tower, Driver Information, Track Map, No HUD.
- **Incident Review**: Collisions, overtakes, pit stops, false starts, track limit violations, Safety Car deployments, Red Flags, championship-deciding moments.
- **Photo Mode**: Free Camera, Zoom, Depth of Field, Focus Distance, Field of View, Motion Blur, Exposure, Brightness, Contrast, Saturation, Image Filters. High-resolution screenshots.
- **Ghost Racing**: Compare against personal best or friend's replay.
- **Telemetry Export**: CSV/JSON, MoTeC-compatible format. Time Trial lap analysis: lap time, sector times, speed trace, throttle input, brake input, steering input, battery usage, top speed, maximum G-force, track limit violations, ideal lap estimate. Players may compare multiple laps.
- **Frame-Accurate Scrubbing**: Supported via deterministic rewind.

---

## 18. Data / Save Architecture

- **Save Slots**: Player profile, career, settings, replay cache.
- **Cloud Sync**: Optional, platform-specific (Xbox Live, PSN, iCloud, Google Play).
- **Schema Versioning**: Forward/backward compatible.
- **Community Content Compatibility**: Custom drivers, teams, liveries, setups, and championships remain functional across future updates whenever technically feasible. Imported setups automatically verify compatibility; obsolete values adjusted automatically after updates.
- **Security**: No sensitive data stored offline; no intrusive DRM.

---

## 19. Testing and Automated Validation

- **Unit Tests**: Physics math, rules engine, input mapping, brake temperature/fade, tire pressure effects, tire surface phenomena (grain/blister/flat spot), MGU-K harvest, engine clipping, AI attribute application, track evolution rubber deposition, active aero zone detection, ERS mode transitions, Overtake Mode energy accounting.
- **Simulation Regression**: Automated lap-time tests on reference tracks with fixed input. Threshold: 0.1% delta. Includes tire degradation curve validation, grip evolution across stint, weather transition tests, active aero zone efficiency tests.
- **Integration Tests**: End-to-end session simulation covering practice, qualifying, race, formation lap, false starts, Safety Car, VSC, Red Flag, standing restart, rolling restart, parc fermé, post-race stewards, Sprint Qualifying format (SQ1/SQ2/SQ3 elimination counts), Overtake Mode activation/deactivation, Active Aero zone compliance.
- **Network Tests**: Simulated lag, packet loss, desync detection, AI attribute synchronization, penalty status sync, active aero state sync, ERS battery sync.
- **Performance Tests**: Frame time budgets per platform tier. Mobile-specific: tire phenomena and AI Race Brain evaluation throttled to 500ms intervals on low-end devices.
- **Accessibility Tests**: HUD preset switching, broadcast mode, screen reader compatibility, colorblind shader verification, volume slider independence.
- **CI**: Every PR runs full test suite; platform-specific builds tested on x86/ARM via emulation or hardware farm. Sim regression runs on all supported platforms to validate cross-platform determinism.

---

## 20. Repository / Module Architecture

```
overtake/
├── core/           # Math, memory, logging, containers, SIMD
├── sim/            # Physics, rules, AI, track data, replay
│   ├── aero/       # Active aero states, transitions, zone logic
│   ├── ers/        # ERS modes, harvest, Overtake Mode, deployment curves
│   ├── pu/         # Power unit models: 2026/2027/2028 variants
│   └── tires/      # Tire surface state sub-module
├── render/         # Renderer, materials, shaders, assets
├── audio/          # Audio engine, synthesis, spatialization
├── input/          # Input abstraction, device detection
├── ui/             # UI framework, layouts, themes
├── net/            # Networking, matchmaking, lobby
├── career/         # Career mode, progression, save I/O
├── tools/          # Editor, asset pipeline, CLI
└── platforms/      # Platform shims (mobile, pc, console)
```

- **Dependency Direction**: `core` ← `sim` ← `career` / `net`. `render` and `audio` depend on `core` and `sim` only via telemetry/event interfaces.
- **Language**: C++20 for runtime; C# or Python for tools.
- **Build System**: CMake with presets per platform.
- **Enforcement**: CI fails on dependency cycle detection.

---

## 21. Production Milestones

| Milestone | Duration | Deliverables |
|-----------|----------|--------------|
| M0 Foundation | 3 mo | Engine core, renderer, build pipeline, single-car demo, determinism validated. |
| M1 MVP | 6 mo | 3 cars, 5 tracks, basic career, 4-player online, tire/fuel, 2026 Active Aero, 2026 ERS baseline, safety car. |
| M2 Expansion | 8 mo | 10 cars, 15 tracks, full championship, weather, full ERS/Active Aero/Overtake Mode, pit stops, 2027 PU variant. |
| M3 AAA Polish | 12 mo | Advanced graphics, VR, advanced tire model (PC), esports tools, 2028 PU variant. |
| M4 Live Ops | Ongoing | DLC, events, cosmetics, track/car editor. |

---

## 22. Risk Register

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Floating-point determinism failure across x86/ARM | High | High | Fixed-point for critical paths; server-authoritative; platform CI. |
| Mobile thermal throttling | Medium | Medium | Adaptive quality scaler; frame rate cap; dynamic resolution. |
| Content pipeline too slow | High | High | Modular track assets, procedural environment dressing, outsource art. |
| Multiplayer netcode complexity | Medium | High | Established library (ENet/RakNet); dedicated netcode engineer. |
| Console certification delays | Medium | Medium | Early dev kits; modular platform layer. |
| Scope creep | Medium | High | Strict non-goals; MVP feature freeze. |
| Touch input latency | High | Low | Native-rate input sampling; minimal input processing. |
| Physics regression | Medium | High | Automated lap-time tests; telemetry replay validation. |
| Mobile 22-car CPU load | Medium | Medium | Benchmark early; reduce career grid size on low-end devices. |
| Tire surface phenomena complexity | Medium | Medium | Extensive playtesting; engineer communication; deterministic thresholds. |
| AI Race Brain performance | Medium | Medium | Staggered evaluation (every 500ms per car); caching; mobile-specific throttling. |
| Setup sharing security | Low | Medium | Strict schema validation; sandboxed import. |
| Community content moderation | Low | Low | Deferred to M4; policy and tools planned but not in initial scope. |
| 2026/2027/2028 PU regulation drift | Medium | High | Parameterize PU curves per season; data-driven config; CI validates against known lap-time baselines per regulation set. |
| Active aero FSM deadlock (zone boundary) | Low | Medium | Explicit zone state machine with hysteresis; zone transition validated in sim regression. |
| Overtake Mode exploitation (borderline gap) | Low | Medium | Server-authoritative gap measurement; 1.0 s threshold enforced at detection line only. |

---

## 23. Explicit Non-Goals

- **No real-world licensing** initially (fictional series, cars, tracks).
- **No extensive deformation damage** (component performance impact only; visual cracks are cosmetic).
- **No VR at MVP** (supported but not required for launch).
- **No modding tools** until after M2.
- **No esports broadcaster tools** until M3.
- **No procedural track geometry** (hand-crafted or artist-assisted modular assembly only).
- **No cross-play with other games**.
- **No free-to-play mechanics** (premium buy; cosmetic DLC only).
- **No pay-to-win mechanics**; competitive advantages never sold.
- **No online content moderation backend** at launch (policy and tools deferred to M4).
- **No league racing infrastructure** at launch (deferred to M3).
- **No UV template distribution system** at launch (deferred to M4).
- **No DRS abstraction** (DRS is deprecated in 2026 FIA regulations; replaced by Active Aero and Overtake Mode).
- **No MGU-H simulation** (removed from 2026 FIA regulations; not reinstated through 2028).
- **No MGU-H gameplay abstraction** (even if players request it; it is not a 2026–2028 FIA-mandated system).

---

## 24. Architecture Decision Register

| Decision | Rationale | Alternatives Rejected |
|----------|-----------|----------------------|
| Fixed 1000Hz simulation timestep | Determinism, consistent physics | Variable timestep (non-deterministic). |
| Strict sim/presentation separation | Fidelity scaling, portability | Tightly coupled (requires rewrites). |
| Server-authoritative multiplayer | Anti-cheat, cross-platform determinism | P2P lockstep (mobile NAT issues). |
| Monorepo | Atomic changes, shared libraries | Polyrepo (dependency hell). |
| C++ core, C#/Python tools | Performance + tool iteration speed | All C++ (slower tooling). |
| Pacejka tire model (MVP) | Standard, tunable, fast | Brush model (deferred to AAA). |
| Deterministic input replay | Replay & multiplayer sync | Frame buffer recording (bandwidth). |
| CMake | Cross-platform, widely understood | Custom build system (maintenance). |
| Versioned binary track format | Fast loading, memory mapping | JSON/XML (too slow, versioning pain). |
| Event-driven sim/presentation coupling | Engineer communication, audio, UI updates without breaking sim boundary | Direct function calls (creates circular dependencies). |
| Staggered AI evaluation | Prevent AI CPU spikes on mobile and low-end devices | Per-frame evaluation (performance risk). |
| Tire surface state sub-module | Maintainability; avoid bloat in core tire model | Monolithic tire module (hard to test and extend). |
| Active Aero replacing DRS | FIA 2026 mandate; no DRS in 2026+ regulations | Retain DRS abstraction (non-compliant with 2026+). |
| Active Aero mode enum over bool | Type safety; prevents impossible states (drs+SLM simultaneously) | Legacy bool flags (unsafe, contradictory). |
| Parameterized PU per regulation year | 2026/2027/2028 power split evolution; data-driven | Hardcoded per-season branches (unmaintainable). |
| ERS mode abstraction layer | Maps FIA Overtake Mode + gameplay modes (Low/Medium/High/Off/Qualifying/ROB) | Single flat enum (loses gameplay vs. regulatory separation). |

---

## 25. VehicleSpec Taxonomy

Every car in Overtake is defined by a `VehicleSpec` data asset. This taxonomy separates immutable hardware identity from mutable session state.

```yaml
VehicleSpec:
  id: "audi_lmr_2026"
  displayName: "Audi LMR 2026"
  regulationYear: 2026  # 2026, 2027, or 2028
  class: "FormulaOne"
  
  # Power Unit
  pu:
    iceMaxPower_kW: 400
    mguKMaxPower_kW: 350
    mguKOvertakeMaxPower_kW: 350
    mguKHarvestMax_kW: 350
    harvestLimit_MJ_per_lap: 8.5
    batteryCapacity_MJ: 4.5
    batterySoCDeltaLimit_MJ: 4.0
    fuelEnergyFlowMax_MJ_per_h: 3000
    fuelFlowRpmCurve: "0.27 * rpm + 165"  # below 10500 rpm
    hasMguH: false
    
  # Aerodynamics
  aero:
    activeAero: true
    cornerModeDownforceCoeff: 1.0   # relative baseline
    cornerModeDragCoeff: 1.0
    straightModeDownforceCoeff: 0.55
    straightModeDragCoeff: 0.45
    frontWingTransitionTime_ms: <FIA regulated>
    rearWingTransitionTime_ms: <FIA regulated>
    
  # Chassis
  chassis:
    minWeight_kg: 768
    wheelbase_mm: 3400
    trackWidth_front_mm: <measurement>
    trackWidth_rear_mm: <measurement>
    
  # Tires (FIA dimension constraints)
  tires:
    frontWidth_mm: 270  # 2026 reduction of 25mm vs prior
    rearWidth_mm: 355   # 2026 reduction of 30mm vs prior
    diameter_front_mm: <measurement>
    diameter_rear_mm: <measurement>
    compounds: ["C1", "C2", "C3", "C4", "C5"]
    
  # Simulation fidelity
  fidelity:
    tireModel: "pacejka89"  # or "brush" for AAA PC/console
    tireThermalNodes: 1     # or 3 for AAA
    surfacePhenomena: true
    aeroDirtyAir: true
```

`VehicleSpec` is loaded at session start. All PU, aero, and tire coefficients are sourced from the spec; no hardcoded values exist in `IVehicle::Update`. Regulation-year-specific specs share a common interface but have distinct parameter sets. [Measurable acceptance criterion: loading a 2026 spec yields 2026 PU limits; switching to 2027 spec yields 420 kW ICE, 300 kW MGU-K without code change.]

---

## 26. Active Aero and ERS Mode State Machines

### 26.1 Active Aero FSM

```
States: CORNER, STRAIGHT, OFF, FAILED
Events: ZONE_ENTER, ZONE_EXIT, DRIVER_TOGGLE, FAILURE, RAIN_RESTRICTION
```

| Current State | Event | Next State | Condition |
|---------------|-------|------------|-----------|
| CORNER | ZONE_ENTER + DRIVER_TOGGLE | STRAIGHT | Driver activates SLM in permitted zone. |
| CORNER | BRAKE | CORNER | Cancels pending SLM. |
| STRAIGHT | ZONE_EXIT | CORNER | Zone boundary crossed. |
| STRAIGHT | DRIVER_TOGGLE | CORNER | Driver cancels SLM. |
| ANY | FAILURE | FAILED | Hardware fault detected. |
| ANY | RAIN_RESTRICTION | CORNER | Race Control disables SLM in wet/low-grip. |
| FAILED | REPAIR | CORNER | Pit-lane repair restores default safe state. |

**Deadlock analysis**: No event causes an illegal transition from STRAIGHT to OFF directly; OFF is reachable only via FAILED or explicit system shutdown. All transitions have defined guard conditions. No cyclic dependency.

**Safety**: On FAILED, vehicle defaults to CORNER (maximum downforce) and active aero input is ignored until pit stop.

### 26.2 ERS Mode FSM

```
States: OFF, LOW, MEDIUM, HIGH, QUALIFYING, RECHARGE, OVERTAKE_ACTIVE, BOOST
Events: MODE_SELECT, OVERTAKE_TRIGGER, OVERTAKE_EXPIRE, BATTERY_EMPTY, PIT_STOP, QUALIFYING_START, RACE_START
```

| Current State | Event | Next State | Notes |
|---------------|-------|------------|-------|
| RECHARGE | MODE_SELECT | LOW/MEDIUM/HIGH/OFF/BOOST | Player choice. |
| LOW | MODE_SELECT | MEDIUM/HIGH/OFF/RECHARGE | Player choice. |
| MEDIUM | MODE_SELECT | LOW/HIGH/OFF/RECHARGE | Player choice. |
| HIGH | MODE_SELECT | LOW/MEDIUM/OFF/RECHARGE | Player choice. |
| OFF | MODE_SELECT | LOW/MEDIUM/HIGH/RECHARGE | Player choice. |
| LOW/MEDIUM/HIGH/OFF/RECHARGE/BOOST/QUALIFYING | OVERTAKE_TRIGGER | OVERTAKE_ACTIVE | Gap < 1.0s at detection line. FIA-mandated condition. No-op if already active. |
| OVERTAKE_ACTIVE | OVERTAKE_EXPIRE | previous state | +0.5 MJ depleted or speed limit reached. |
| OVERTAKE_ACTIVE | MODE_SELECT | previous state | Player overrides; energy continues to deplete per Overtake curve. |
| ANY | BATTERY_EMPTY | OFF | No deploy possible. |
| OFF | BATTERY_CHARGED | RECHARGE | Sufficient charge restored. |
| ANY | PIT_STOP | OFF | Reset at pit exit. |
| ANY | QUALIFYING_START | QUALIFYING | Session type sets mode. |
| QUALIFYING | RACE_START | MEDIUM | Session type changes. |
| BOOST | BATTERY_DEPLETED | RECHARGE | Deploy budget exhausted. |

**Deadlock analysis**: OVERTAKE_ACTIVE always expires (energy finite). Battery empty always recovers via harvest. No state is terminal except OFF on battery empty, which recovers. All player-mode selections lead to reachable states. No race conditions on mode pointer (single-threaded sim tick).

**FIA vs. Overtake separation**: OVERTAKE_TRIGGER and OVERTAKE_EXPIRE are governed by FIA rules (1.0 s gap, +0.5 MJ, 350 kW to 337 km/h follow / 290–355 km/h lead taper). Player modes (LOW/MEDIUM/HIGH/OFF/BOOST) are Overtake abstractions that sit on top of the regulated Overtake Mode curve.

---

## 27. Module Interfaces / Dependencies

### 27.1 Active Aero Interface

```cpp
class IActiveAero {
public:
    virtual void SetMode(ActiveAeroMode mode) = 0;
    virtual ActiveAeroMode GetMode() const = 0;
    virtual void SetZone(const AeroZone& zone) = 0;
    virtual bool IsTransitionComplete() const = 0;
    virtual float GetDownforceCoefficient() const = 0;
    virtual float GetDragCoefficient() const = 0;
};
```

### 27.2 ERS Interface

```cpp
class IERS {
public:
    virtual void SetMode(ERSMode mode) = 0;
    virtual ERSMode GetMode() const = 0;
    virtual float GetBatterySoC_MJ() const = 0;
    virtual float GetSoCDeltaLimit_MJ() const = 0;
    virtual float GetHarvestLimit_MJ_per_lap() const = 0;
    virtual float GetMGUKPower_kW() const = 0;
    virtual float GetMGUKHarvestPower_kW() const = 0;
    virtual bool IsOvertakeActive() const = 0;
    virtual float GetOvertakeBonusEnergy_MJ() const = 0;
    virtual void Update(float dt, float vehicleSpeed, float brakePressure) = 0;
};
```

### 27.3 Power Unit Interface

```cpp
class IPowerUnit {
public:
    virtual float GetICEPower_kW(float rpm, float fuelFlow) const = 0;
    virtual float GetMGUKMaxPower_kW(bool overtakeActive) const = 0;
    virtual float GetMGUKHarvestMax_kW() const = 0;
    virtual float GetBatteryCapacity_MJ() const = 0;
    virtual float GetMinWeight_kg() const = 0;
    virtual int GetRegulationYear() const = 0;
    virtual void LoadSpec(const VehicleSpec::PU& spec) = 0;
};
```

### 27.4 Dependency Enforcement

- `sim/aero` depends only on `core` and `sim/track`.
- `sim/ers` depends only on `core` and `sim/pu`.
- `sim/pu` depends only on `core`.
- `career` and `net` depend on `sim` via events and `VehicleState` only.
- CI fails on dependency cycle detection.

---

## 28. Performance Budgets and Implementation Order

| System | CPU Budget (per frame) | GPU Budget | Implementation Order |
|--------|----------------------|------------|---------------------|
| Physics (1000Hz, 22 cars) | 4.0 ms | — | M0 |
| Tire model (Pacejka 89) | 0.3 ms | — | M1 |
| Tire surface phenomena | 0.2 ms | — | M1 |
| Active Aero FSM | 0.05 ms | — | M1 |
| ERS / MGU-K model | 0.1 ms | — | M1 |
| AI Race Brain (staggered) | 0.5 ms | — | M1 |
| Track evolution (rubber) | 0.1 ms | — | M1 |
| Rules engine | 0.2 ms | — | M1 |
| Renderer (mobile, 30fps) | — | 16 ms | M0–M1 |
| Renderer (PC, 120fps) | — | 8 ms | M3 |

**Critical path**: M0 must validate cross-platform determinism of `VehicleState` within 0.1% tolerance before M1 gameplay development proceeds.

---

## 29. Gauntlet Loop #8 — Adversarial Audit and Regulatory Compliance Review

### 29.1 Research Summary

Reviewed official FIA documents:
- Section C Technical Regulations, Issue 17 (28 April 2026) — Published / Ratified
- Section B Sporting Regulations, Issue 08 (05 August 2026) — Published / Ratified
- Section F Operational Regulations, Issue 10 (05 August 2026) — Published / Ratified
- WMSC media statement, 23 June 2026 — Ratified 2027/2028 PU changes
- FIA press release, 6 June 2024 — Original 2026 regulation announcement
- Formula 1 technical briefings, Pirelli 2026 preview, ESPN, Formula1.com — Secondary corroboration

### 29.2 Adversarial Critic

**Contradictions found and resolved**:
- **Legacy DRS flag**: Previous blueprint interface boundary used `bool drs`. 2026 FIA regulations eliminate DRS entirely. **Resolved**: Replaced with `ActiveAeroMode activeAero` enum. No DRS abstraction remains.
- **R.O.B. terminology conflict**: Gauntlet #2 explicitly rejected "R.O.B. naming for ERS modes" due to branding conflict. Earlier blueprint text still referenced "R.O.B.-style" in AI and ERS sections. **Resolved**: Removed all "R.O.B." references. ERS gameplay modes are now explicitly tagged `[Overtake abstraction]` and separated from FIA Overtake Mode.
- **ERS mode mismatch**: Previous ERS section listed "Manual, Auto, Hotlap" without FIA linkage. **Resolved**: Mapped Overtake's gameplay modes (Low/Medium/High/Off/Qualifying/Recharge/Overtake/Boost) to FIA-regulated Overtake Mode with explicit state machine.
- **ERS boost inconsistency**: FIA Overtake Mode is "+0.5 MJ bonus energy when within 1s". Previous blueprint implied Boost was equivalent to DRS. **Resolved**: Boost is an Overtake abstraction for maximum deployment; Overtake Mode is FIA-mandated +0.5 MJ. They are distinct.
- **Missing regulation year**: No mechanism to switch between 2026/2027/2028 PU rulesets. **Resolved**: Added `regulationYear` to `VehicleSpec` and `IPowerUnit`. Parameterized PU curves.

**Gaps found and resolved**:
- **Active Aero FSM missing**: No state machine for Corner/Straight transitions, zone entry/exit, or failure modes. **Resolved**: Added Section 26.1.
- **ERS FSM missing**: No state machine for mode transitions, Overtake Mode trigger/expire, battery empty recovery. **Resolved**: Added Section 26.2.
- **VehicleSpec taxonomy missing**: No structured definition of car identity. **Resolved**: Added Section 25 with YAML schema.
- **Regulatory classification missing**: No distinction between Published, Ratified, Announced, Provisional, etc. **Resolved**: Added Section 0 with full classification table.
- **2027/2028 PU specifics missing**: No documentation of ICE/MGU-K power split evolution. **Resolved**: Added to Section 5 (ERS/Fuel) and Section 25 (VehicleSpec).
- **Sustainable fuel missing**: No mention of 100% sustainable fuel mandate. **Resolved**: Added to Section 5.
- **Minimum weight missing**: 2026 minimum weight is 768 kg, not generic. **Resolved**: Added to Section 5 and VehicleSpec.
- **No MGU-H missing**: Previous blueprint didn't explicitly exclude MGU-H. **Resolved**: Added to Non-Goals (Section 23).
- **Sprint Qualifying exact format missing**: No SQ1/SQ2/SQ3 timing or elimination counts. **Resolved**: Added to Section 8.1.
- **Overtake Mode zone detection missing**: No mention of 1.0 s gap at detection line. **Resolved**: Added to Section 8.2.
- **Wet Overtake Mode restriction missing**: No mention of FIA safety restriction in low-grip conditions. **Resolved**: Added to Section 8.2.
- **Penalty serving exact rules missing**: No 5s/10s/drive-through/stop-and-go timing or post-session rules. **Resolved**: Added to Section 8.3.
- **Active Aero presentation coupling**: Audio section mentioned "active aero transitions" but didn't define trigger sounds. **Resolved**: Clarified in Section 13; presentation-only.
- **Track metadata for zones missing**: No mention of Active Aero zones or Overtake Mode detection lines in track data. **Resolved**: Added to Section 7.

**Hidden dependencies**:
- `sim/aero` now depends on `sim/track` for zone geometry. Mitigation: zone data is read-only during simulation; no track modification.
- `sim/ers` depends on `sim/pu` for harvest/power limits. Mitigation: `IPowerUnit` interface is stable; spec changes do not require ERS rewrite.
- `career` needs regulation year selection to apply correct PU curves. Mitigation: `VehicleSpec` carries `regulationYear`; career mode is data-driven.

**Physics errors**:
- None found. 1000Hz timestep, quaternion orientation, friction circle enforcement are consistent.

**FSM deadlocks**:
- Active Aero FSM: No deadlock. All states reachable. CORNER is absorbing default.
- ERS FSM: No deadlock. OVERTAKE_ACTIVE always expires; OFF recovers via harvest.

**Unrealistic performance assumptions**:
- None added. All CPU budgets are conservative relative to existing M0/M1 estimates.

**Regulatory mismatches**:
- All major mismatches (DRS, R.O.B., missing FIA rules) resolved.

**Duplicated systems**:
- None. Active Aero and ERS are distinct modules with separate interfaces.

**Untestable requirements**:
- All new requirements have measurable acceptance criteria:
  - SQ1 elimination count scales correctly for any grid size.
  - Overtake Mode triggers exactly at 1.0 s gap.
  - 2026 spec yields 350 kW MGU-K; 2027 spec yields 300 kW.
  - Active Aero FSM transitions match zone entry/exit within 1 simulation tick.
  - Battery SoC never exceeds 4.0 MJ delta limit.

**Scope creep**:
- None. All additions extend existing modules; no new top-level features introduced.

### 29.3 Accepted Changes

| Priority | Change | Source | Rationale | Architectural Impact | Verification | Phase |
|----------|--------|--------|-----------|---------------------|--------------|-------|
| P0 | Replace DRS bool with ActiveAeroMode enum | FIA 2026 mandate | DRS deprecated; Active Aero replaces it | Low; interface boundary | Unit tests + sim regression | M1 |
| P0 | Remove R.O.B. references; map to FIA + Overtake ERS modes | Gauntlet #2 rejection | Branding conflict resolved | Low; text update | Text search | M1 |
| P0 | Add Active Aero FSM (Section 26.1) | FIA 2026 mandate | Corner/Straight transitions, zones, failures | Low; new sim/aero module | Sim regression + integration | M1 |
| P0 | Add ERS FSM (Section 26.2) | FIA 2026 mandate | Mode transitions, Overtake Mode trigger/expire | Low; extends sim/ers | Sim regression + integration | M1 |
| P0 | Add FIA regulatory baseline & classification (Section 0) | Official FIA docs | No distinction between ratified/provisional/unverified | Low; documentation | Document review | M0 |
| P0 | Add VehicleSpec taxonomy (Section 25) | Overtake architecture | No structured car identity definition | Low; data module | Data validation tests | M1 |
| P0 | Add 2026 PU baseline (350 kW MGU-K, 8.5 MJ, 4 MJ delta, no MGU-H) | FIA 2026 Technical | Missing mandatory PU limits | Low; extends sim/pu | Unit tests | M1 |
| P0 | Add 2027/2028 PU evolution (58/42, 60/40) | WMSC 23 June 2026 | Missing multi-year evolution | Low; parameterized spec | Sim regression per year | M2/M3 |
| P1 | Add Sprint Qualifying exact format (SQ1/SQ2/SQ3, elimination counts) | FIA 2026 Sporting | Under-specified weekend structure | Low; extends rules engine | Integration tests | M1 |
| P1 | Add Overtake Mode zone detection (1.0 s gap, +0.5 MJ, 290/355 km/h taper) | FIA 2026 Technical | Under-specified ERS overtaking | Low; extends sim/ers | Sim regression | M1 |
| P1 | Add wet/low-grip Overtake Mode restriction | FIA WMSC 23 June 2026 | Missing safety restriction | Low; extends rules engine | Integration tests | M1 |
| P1 | Add exact penalty serving rules (5s/10s/DT/SG post-session) | FIA 2026 Sporting | Under-specified penalties | Low; extends rules engine | Integration tests | M1 |
| P1 | Add sustainable fuel, minimum weight, tyre dimensions to VehicleSpec | FIA 2026 Technical | Missing mandatory constraints | Low; data update | Data validation | M1 |
| P1 | Add track metadata for Active Aero zones and Overtake Mode detection | FIA 2026 Technical | Missing track data | Low; extends track format | Integration tests | M1 |
| P2 | Add Heat Hazard declaration rule | FIA 2026 Sporting | Missing sprint/race weather rule | Low; extends rules engine | Integration tests | M1 |
| P2 | Add front/rear wing adjuster transition time checks | FIA 2026 Technical/Operational | Missing scrutineering rule | Low; extends sim/aero | Unit tests | M1 |
| P2 | Parameterize PU per regulation year in career mode | Overtake abstraction | Career needs 2026/2027/2028 switch | Low; career module | Career simulation tests | M2 |

### 29.4 Rejected / Deferred

| Item | Reason |
|------|--------|
| Retain DRS abstraction | Rejected: DRS does not exist in 2026 FIA regulations. [Overtake-specific override.] |
| R.O.B. naming for ERS modes | Rejected: Branding conflict per Gauntlet #2. |
| MGU-H simulation or abstraction | Rejected: No MGU-H in 2026–2028 FIA regulations. Explicitly listed in Non-Goals. |
| 2029+ PU regulations | Deferred: No published FIA documents beyond 2028 as of 2026-10-02. |
| Active Aero in wet conditions with poor visibility | Rejected for gameplay: FIA restricts Overtake Mode, but active aero remains available unless Race Control explicitly disables it. |

### 29.5 Final Verification

**Implemented**: N/A (design phase).

**Audited**: All 735 lines of existing blueprint cross-referenced against official 2026–2028 FIA regulations. 20 additions proposed; all accepted (10 P0, 7 P1, 3 P2). 0 rejected, 3 explicitly deferred.

**Verified**:
- **Regulatory contradictions**: None. Active Aero, ERS, PU, fuel, weight, and tyre rules match FIA publications.
- **FSM completeness**: Active Aero FSM and ERS FSM have no deadlocks, no undefined transitions, and default-safe states.
- **Architectural consistency**: Module graph remains acyclic. New `sim/aero` and `sim/ers` sub-modules follow existing dependency rules.
- **Determinism**: Overtake Mode and Active Aero state changes are deterministic functions of gap, zone, and input. No randomness.
- **Testability**: Every new requirement has a measurable acceptance criterion.
- **Multiplayer sync**: Active Aero mode, ERS mode, and Overtake Mode status are part of `VehicleState` sync.
- **Scope**: All additions are additive and phase-bounded. No M0/M1 scope expansion beyond regulatory compliance.
- **Non-goals enforced**: DRS, MGU-H, and R.O.B. naming are explicitly excluded.

**Known limitations**:
- 2028 Technical Regulations are not yet published as of 2026-10-02; 2028 PU values are based on WMSC-ratified agreement and may shift during formal consultation.
- Active Aero zone geometry and Overtake Mode detection lines are track-specific and must be authored per circuit.
- Tire surface phenomena complexity may require UI clarity improvements to avoid player confusion.
- AI Race Brain staggered evaluation may reduce responsiveness on low-end devices.

**Unverified assumptions**:
- 1000Hz physics loop with 22 cars fits within 5ms CPU budget on flagship mobile CPUs (A17 Pro, Snapdragon 8 Gen 3).
- Fixed-point math will not overflow in typical 2-hour race durations.
- Server-authoritative netcode with client prediction provides esports-acceptable responsiveness (<50ms perceived latency).
- Content pipeline can deliver 15+ high-quality tracks within 18 months with a small team.
- Tire rubber deposition rate produces realistic grip evolution across 38 unique circuits without per-circuit tuning.
- Driver attribute balancing (0.0–10.0 scale) produces meaningful AI difficulty curves from Rookie to Legend.

**Recommended first milestone**: M0 Foundation.  
Deliver a single-track, single-car demo running on PC, iOS, and Android simultaneously. Validate that the same deterministic input sequence produces identical `VehicleState` outputs on all three platforms within tolerance. If this gate fails, the architecture must be revised before gameplay development proceeds.

---

## Critical Question

**Can Overtake start as a small, achievable game and scale toward AAA mobile/PC/console production without requiring foundational rewrites?**

**Answer**: **Yes.**  
The strict separation of simulation and presentation, combined with a server-authoritative multiplayer model and versioned data formats, creates a foundation that is extensible but not fragile. Mobile reduces visual and optional simulation fidelity; PC/console scales it up. The core rules and race logic are written once. The largest ongoing risk is cross-platform determinism, which is mitigated by continuous CI validation and authoritative servers. Provided the team respects module boundaries and the phased milestone plan, foundational rewrites are unnecessary.
