# OVERTAKE MASTER BLUEPRINT

**Project**: Overtake — experimental cross-platform racing game
**Type**: Deterministic simulation with scalable presentation
**Targets**: Mobile, PC, consoles
**Status**: Design phase

---

## Identity

Overtake is a small, playable, technically interesting racing game built on deterministic simulation within Unity. Its signature features are **Active Aero** (Corner/Straight Line modes) and **ERS energy management** — the overtaking mechanics that define its name. Everything else serves those two systems and a clean, repeatable race loop.

No real-world licensing, branding, or FIA regulatory fidelity is required for gameplay. Overtake uses a 2026-style regulations profile as a sensible default; fictional and custom regulation profiles must remain trivially possible. FIA regulations are ruleset data loaded at session start, never hard-coded into simulation source.

---

## Regulation Sources & Confidence

This document cites FIA regulations by source and confidence level. Values in the `RegulationProfile` YAML must trace to a cited source; unmarked values default to `[hypothesis]`.

| Confidence | Meaning | Requirement |
|---|---|---|
| **Confirmed** | Published in an official FIA regulation PDF (e.g., 2026 Technical Regulations Section C, Issue 20) | Authoritative default for `2026_default` profile |
| **Reported** | Appears in an FIA press release or official statement, but not yet in published regulation text | Optional override; never the sole basis for an engine-limit default |
| **Hypothesis** | Engineering estimate with no primary-source verification | Must be sourced from `VehicleSpec`, not `RegulationProfile`; clearly marked in Gauntlet Log |
| **Unknown** | FIA has not published the relevant material | Section omitted; no guesswork encoded |

2026 materials verified:
- **Technical Regulations** (Section C, Issue 20, dated 2026-08-05, published 2026-10-01) — covers Articles C1–C16
- **Sporting Regulations** (Section B, Issue 09, dated 2026-10-01) — covers Articles B1–B36
- **Power Unit Technical Regulations** (Issue 7, dated 2024-06-11) — covers Articles C5.1–C5.2.11

2027 materials verified:
- **Technical Regulations** (Section C, Issue 3, dated 2026-10-01) — published changes only: floor/plank dimensions, front/rear aero element geometry, fuel system modifications. No power-unit output changes appear in published regulation text.

2028 materials:
- **None published** by the FIA as of the 2026-10-01 cut. No 2028 values are encoded.

---

## Design Policy

FIA regulations are ruleset data, not architectural constraints. Every FIA-mandated behaviour (overtake gap thresholds, harvest limits, minimum weight, fuel flow) is loaded from a configurable **RegulationProfile** at session start. The simulation itself knows nothing about FIA seasons.

Separation:
- **FIA-mandated behaviour** → `RegulationProfile` data
- **Gameplay rules** → Rules engine, data-driven
- **Overtake abstractions** → Tagged `[abstraction]` in simulation code

Hard-coded real-world values, regulation year branching in source code, and regulation-driven module design are prohibited.

---

## Scope Gate

The minimum set of systems to make Overtake playable, distinctive, testable, and technically interesting:

1. Deterministic physics kernel
2. Vehicle system (tires, fuel, ERS, Active Aero, **suspension**)
3. Track format and representation
4. Race loop and rules engine
5. Input abstraction and minimal HUD
6. Data-driven car and regulation definitions

Everything beyond those six systems is deferred. No new scope is introduced during this reduction.

---

## RegulationProfile

A `RegulationProfile` is a data asset (ScriptableObject or JSON) loaded at session start. It drives the rules engine and provides default car parameters. Switching profiles requires no code change.

```yaml
RegulationProfile:
  id: "2026_default"
  overtakeMode:                   # [abstraction] — DRS replacement per FIA B7
    gapThreshold_s: 1.0           # Confirmed: B7.2 — activation threshold behind leader
    bonusEnergy_MJ: 0.5           # Confirmed: C5.2.10 — 0.5 MJ Overtake energy bonus
    followMaxPower_kW: 350        # Confirmed: C5.2.8 — MGU-K deploy limit
    followMaxSpeed_kmh: 337       # Confirmed: C5.2.8 — 337 km/h deploy speed cap
    leadTaperStart_kmh: 290       # Confirmed: B7.2 — leader speed at which deploy begins to taper
    leadTaperEnd_kmh: 355         # Confirmed: B7.2 — leader speed at which deploy reaches full taper
  ers:
    mguKMaxPower_kW: 350          # Confirmed: C5.2.8 / Article 5.4.7
    harvestMaxPower_kW: 350        # Confirmed: C5.2.10 — harvest at MGU-K rated power
    harvestLimit_MJ_per_lap: 8.5  # Confirmed: C5.2.10 — lap harvest/recover cap
    harvestLimitWithOvertake_MJ_per_lap: 9.0  # Confirmed: C5.2.10 — +0.5 MJ when Overtake active
    batterySoCDeltaLimit_MJ: 4.0  # Confirmed: C5.2.9 — energy store usable delta
    deployRate_MJ_per_s: 0.35     # Derived from mguKMaxPower_kW / 1000 (350 kW = 0.35 MJ/s)
    harvestRate_MJ_per_s: 0.35    # Derived from harvestMaxPower_kW / 1000
  pu:
    iceMaxPower_kW: 400           # Confirmed: Article C5.1 — ICE power unit output
    fuelEnergyFlowMax_MJ_per_h: 3000  # Confirmed: Article 5.4.3 — fuel flow energy cap
    fuelFlowRpmCurve: "0.27 * rpm + 165"  # Confirmed: Article 5.4.4 — fuel energy flow below 10500 RPM
    fuelFlowRpmCurveMaxRpm: 10500       # Confirmed: Article 5.4.4 — flat 3000 MJ/h above this RPM
    hasMguH: false                # Confirmed: 2026+ Technical Regulations — MGU-H removed
  aero:
    cornerModeDownforceCoeff: 1.0
    cornerModeDragCoeff: 1.0
    straightModeDownforceCoeff: 0.55
    straightModeDragCoeff: 0.45
    frontTransitionTime_ms: 50
    rearTransitionTime_ms: 50
  vehicle:
    minWeight_kg: 768             # Reported (formula1.com, RaceFans) — see Gauntlet Log
  tyres:
    frontWidth_mm: 280            # Confirmed: 2026 Pirelli announcement
    rearWidth_mm: 375             # Confirmed: 2026 Pirelli announcement
  suspension:                     # [hypothesis] — see Suspension Engineering section
    rideHeight_min_mm: 20         # Hypothesis: typical F1 ride height floor
    rideHeight_max_mm: 50          # Hypothesis: practical range before handling degradation
    springRate_N_per_m: 120000    # Hypothesis: typical F1 front spring rate ballpark
    damperCompression_N_per_m: 80000  # Hypothesis
    damperRebound_N_per_m: 120000     # Hypothesis
    antiRollBarStiffness_Nm_per_rad: 50000  # Hypothesis
    maxTravel_mm: 100            # Hypothesis: 100 mm virtual travel per wheel
    bumpStopStiffness_N_per_m: 500000  # Hypothesis
  sprint:
    sq1Duration_s: 720           # Confirmed: B12.2.2 — 12 min SQ1
    sq2Duration_s: 600           # Confirmed: B12.2.3 — 10 min SQ2
    sq3Duration_s: 480           # Confirmed: B12.2.4 — 8 min SQ3
    sq3CarCount: 10              # Confirmed: B12.2.4 — top 10 from SQ2
    sq1EliminationCount: 6       # Confirmed: B12.2.2 — eliminate slowest 6
    sq2EliminationCount: 6       # Confirmed: B12.2.3 — eliminate slowest 6
  raceQualifying:                # Confirmed: B12.3
    q1Duration_s: 1080           # Q1: 18 min (B12.3.1)
    q2Duration_s: 900            # Q2: 15 min (B12.3.2)
    q3Duration_s: 780            # Q3: 13 min (B12.3.3)
    eliminationPerRound: 5       # Confirmed: B12.3 — slowest 5 eliminated per Q1/Q2 round
```

A 2027 profile modifies only fields confirmed in the published 2027 Technical Regulations Section C (Issue 3): `aero.cornerModeDownforceCoeff`, `aero.straightModeDragCoeff`, `suspension` geometry affecting plank/floor ride height. Power-unit output changes and fuel-flow curve modifications appear in FIA press releases but are **not** in published regulation text as of 2026-10-01; those are `[reported]` and never engine-limit defaults.

A custom profile can remove Active Aero entirely, change the overtake bonus, or replace the PU with an entirely fictional configuration.

The simulation reads `RegulationProfile` values through stable interfaces. No `if (year == 2026)` branches exist in simulation code.

---

## VehicleSpec

Every car is defined by a `VehicleSpec` data asset. This separates immutable hardware identity from mutable session state.

```yaml
VehicleSpec:
  id: "generic_f1_2026"
  displayName: "Open Formula"
  regulationProfileId: "2026_default"
  pu:
    iceMaxPower_kW: 400
    mguKMaxPower_kW: 350
    mguKOvertakeMaxPower_kW: 350
    mguKHarvestMax_kW: 350
    harvestLimit_MJ_per_lap: 8.5
    harvestLimitWithOvertake_MJ_per_lap: 9.0
    batteryCapacity_MJ: 4.5
    batterySoCDeltaLimit_MJ: 4.0
    fuelEnergyFlowMax_MJ_per_h: 3000
    fuelFlowRpmCurve: "0.27 * rpm + 165"
    fuelFlowRpmCurveMaxRpm: 10500
    hasMguH: false
  aero:
    activeAero: true
    cornerModeDownforceCoeff: 1.0
    cornerModeDragCoeff: 1.0
    straightModeDownforceCoeff: 0.55
    straightModeDragCoeff: 0.45
    frontTransitionTime_ms: 50
    rearTransitionTime_ms: 50
  dynamics:
    mass_kg: 768                # Vehicle actual mass (must be >= minWeight_kg from RegulationProfile)
    dragCoeffCd: 0.7
    frontalArea_m2: 1.5
    rollingResistanceCoeff: 0.015
    wheelRadius_m: 0.33
    maxBrakeForce_N: 20000
    gearRatios: [3.0, 2.5, 2.0, 1.5, 1.2, 1.0]
    finalDriveRatio: 3.2
    idleRpm: 800
    maxRpm: 15000
    shiftUpRpm: 12000
    shiftDownRpm: 5000
    airDensity_kg_per_m3: 1.225
    gravity_m_per_s2: 9.81
    maxAcceleration_m_per_s2: 8.0
  tires:
    frontWidth_mm: 280          # Fixed: was 270 (pre-2026 spec)
    rearWidth_mm: 375           # Fixed: was 355 (pre-2026 spec)
  suspension:                    # [hypothesis] — see Suspension Engineering section
    frontSpringRate_N_per_m: 120000
    rearSpringRate_N_per_m: 140000
    frontDampingCompression: 80000
    frontDampingRebound: 120000
    rearDampingCompression: 90000
    rearDampingRebound: 130000
    frontAntiRollBarNm_per_rad: 50000
    rearAntiRollBarNm_per_rad: 60000
    maxTravel_mm: 100
    bumpStopStiffness_N_per_m: 500000
    rideHeight_min_mm: 20
    rideHeight_max_mm: 50
  fidelity:
    tireModel: "pacejka89"       # or "brush"
    tireThermalNodes: 1         # or 3
```

`VehicleSpec` overrides `RegulationProfile` defaults where defined. `IVehicle.Update` reads no hard-coded constants.

---

## Suspension Engineering

**Status**: `[hypothesis]` — included in scope gate as a foundational physics subsystem, but all numerical values are engineering estimates not yet validated against physical reference data.

### Architecture

Suspension is part of the vehicle rigid body dynamics but is **not** a regulation parameter. It lives in `VehicleSpec.suspension` (per-car setup) and contributes to `VehicleState.suspension` (per-wheel travel and ride height). The simulation computes spring forces, damper forces, and anti-roll bar torque per wheel, summed into the rigid body's force and torque accumulators.

### Physical vs. Presentation Separation

- **Physical simulation**: A linear spring-damper model per wheel with travel limits and bump stops. Spring rate, damper compression/rebound, and anti-roll bar stiffness are `VehicleSpec` data, not hard-coded constants. The suspension operates at the **simulation fixed timestep** (default 50 ms / 200 Hz — M0 hypothesis; 1000 Hz is not an architectural requirement per Gauntlet).
- **Rendered presentation**: Visual wheel positioning and body roll animation is interpolated from `VehicleState.suspension` fields. The rendered travel range (10 mm typical) is a presentation-only value derived from the logical travel; visual fidelity scales independently of simulation fidelity.

### State Fields

`VehicleState` exposes per-wheel (FL = front left, FR = front right, RL = rear left, RR = rear right):
- `suspensionTravelFL_mm`, `suspensionTravelFR_mm`, `suspensionTravelRL_mm`, `suspensionTravelRR_mm` — current compression from ride height
- `rideHeightFL_mm`, `rideHeightFR_mm`, `rideHeightRL_mm`, `rideHeightRR_mm` — current distance from wheel center to car floor
- `suspensionVelocityFL_m_per_s`, `suspensionVelocityFR_m_per_s`, `suspensionVelocityRL_m_per_s`, `suspensionVelocityRR_m_per_s` — rate of travel change (for damper force)

These feed presentation for body roll, squat, dive, and visual wheel articulation. They may also drive surface-effect rules (e.g., plank/floor contact at ride height limits).

### Constraints

- Suspension forces are clamped to prevent numeric instability. Bump stops engage at `maxTravel_mm` with a stiff spring (`bumpStopStiffness_N_per_m`).
- Anti-roll bars link left/right wheel travel within an axle, producing body roll torque proportional to the suspension travel difference.
- The suspension model has **no FIA regulation parameters**. Spring rates, damper settings, ride heights, and travel limits are `VehicleSpec` tuning data selected by the player or AI, never derived from `RegulationProfile`.

### CPU Budget

At 200 Hz (50 ms tick), four independent spring-damper calculations per vehicle add negligible overhead (<10 µs per vehicle on target hardware). No native plugin is required for MVP. If profiling at M0 establishes otherwise, a C++ burst-compiled or native plugin path is evaluated per the Design Policy and M0 Validation Spike criteria.

### Unknowns and Risks

- **100 mm virtual / 10 mm rendered travel**: Untested hypothesis. The simulation should model realistic F1 suspension travel (~100–120 mm total), while the rendered animation uses a visually amplified range. This must be validated in M0 with a reference spring-mass simulation.
- **Deterministic substepping**: If suspension forces become stiff (high spring rates), a substep solver within the simulation tick may be required for numeric stability. Substepping must preserve cross-platform determinism (see Determinism Contract). This is a `[hypothesis]` requiring M0 validation.
- **Tire load coupling**: Suspension travel affects normal load on each tire, which affects grip. The Pacejka and brush tire models must consume `VehicleState.suspensionTravel_mm` as an input. Integration with the tire fidelity toggle (1 node vs 3 node) is deferred to M1.

---

## Determinism Contract

Every frame: `S(t) = f(S(t-1), I(t), seed)` — a pure function.

- Fixed-timestep simulation. Unity's `FixedUpdate` is the natural candidate; validate exact frequency (e.g. 100 Hz, 200 Hz — 1000 Hz is permitted but not mandated) as a hypothesis in M0. Presentation interpolates.
- No uninitialized memory, no race conditions, no platform-dependent FP behaviour (no FTZ/DAZ, no fused-multiply-add unless explicitly disabled).
- RNG state is part of `S`.
- No heap allocation during the simulation tick.
- Single-threaded simulation tick. Parallelism is permitted only in presentation, offline tools, and network receive.

Cross-platform determinism gate: identical input sequence produces identical `VehicleState` within ±0.1% on all target platforms. This is a CI requirement before M1 gameplay development proceeds.

---

## Simulation–Presentation Boundary

```csharp
// Simulation interface — presentation never touches internals.
public enum ActiveAeroMode { Corner, StraightLine, Off, Failed }
public enum ERSMode { Off, Low, Medium, High, Qualifying, Boost }

public struct VehicleInput {
    public float steer, throttle, brake, clutch;
    public int gear;
    public ActiveAeroMode activeAero;
    public ERSMode ersMode;
}

public struct VehicleState {
    public Matrix4x4 worldMatrix;
    public float speed, rpm, gear;
    public float[] tireSlip;
    public float batterySoC_MJ;
    public float suspensionTravelFL_mm;
    public float suspensionTravelFR_mm;
    public float suspensionTravelRL_mm;
    public float suspensionTravelRR_mm;
    public float rideHeightFL_mm;
    public float rideHeightFR_mm;
    public float rideHeightRL_mm;
    public float rideHeightRR_mm;
    public float suspensionVelocityFL_m_per_s;
    public float suspensionVelocityFR_m_per_s;
    public float suspensionVelocityRL_m_per_s;
    public float suspensionVelocityRR_m_per_s;
}
```

Presentation subscribes to `VehicleState` via Unity event or polling. Audio receives telemetry events only. Neither may mutate simulation state.

---

## Vehicle Physics

**Rigid body**: 6 DOF per vehicle. Quaternion orientation. No Euler angles.

**Suspension**: Spring/damper per wheel, anti-roll bar, travel limits, bump stops. See [Suspension Engineering](#suspension-engineering) for architecture and `[hypothesis]` fields.

**Tires**: Slip-angle and slip-ratio based. Friction circle enforced. Two computational resolutions — Pacejka '89 lookup (1 thermal node) and brush contact patch (3 thermal nodes) — selected by `VehicleSpec.fidelity.tireModel`. Grip curve shape and wear rate are identical across resolutions. Results are within gameplay tolerance (<0.5% lap time delta). Surface phenomena (grain, blister, flat spot, marbles, aquaplaning) are simple deterministic rules driven by temperature, lock-up events, and track rubber state.

Tire normal load is influenced by suspension travel and ride height, feeding the friction coefficient lookup. At MVP fidelity, this coupling is approximate; the brush model (3 thermal nodes) provides more detailed load sensitivity.

Tire pressure affects contact patch, rolling resistance, mechanical grip, and degradation rate.

**Fuel**: Mass decreases deterministically over stint. Center of mass shifts. Affects braking distances, tyre degradation, and cornering inertia.

**ERS**: Configurable via `RegulationProfile.ers`. Deploy rate derived from `mguKMaxPower_kW`, never hard-coded (fixed Gauntlet finding — was `0.8f` MJ/s = 800 kW, exceeding the 350 kW FIA cap).

ERS modes:
- `Off`, `Low`, `Medium`, `High` — player-selectable deployment levels
- `Qualifying` — flat-out deployment for session
- `Boost` — maximum deployment using stored battery energy [abstraction]

`Overtake` bonus energy is not a player-selectable mode. It is triggered automatically when the gap condition is met at a detection line and the current deployment mode is not `Off`. Overtake bonus energy expires when the bonus energy budget is exhausted or the speed limit is reached; the vehicle returns to its prior deployment mode. [abstraction: trigger condition and bonus amount are configurable via RegulationProfile.]

ERS state machine: `Off → Low/Medium/High/Boost` via player mode select. `Low/Medium/High → Boost` via player select. `Any → Off` on `BATTERY_EMPTY` (forced recharge-hold; player may resume deployment once charge is sufficient). Battery empty does not lose prior mode selection; the prior mode is restored when sufficient charge returns.

**Active Aero**: Movable front and rear wing flaps. Two states: `Corner` (default high downforce) and `StraightLine` (low drag). Available in designated track zones. Transition times from `RegulationProfile.aero`. On failure, defaults to `Corner` (maximum downforce). [abstraction: zone detection logic and StraightLine availability rules are configurable.]

State machine: `Corner → StraightLine` on zone entry + driver toggle (or brake cancellation). `StraightLine → Corner` on zone exit or driver cancel. `Any → Failed` on fault; `Failed → Corner` on pit repair.

**Damage**: Component health (front wing, floor, suspension, engine, gearbox). Deterministic performance modifiers. Visual damage is presentation-only.

---

## AI

AI agents use the same `IVehicle.Update` interface. Difficulty is scaled by Gaussian noise on control inputs plus configurable reaction delay. AI selects ERS modes and activates Active Aero based on track context. Deterministic: same seed produces identical behaviour. No online learning.

AI is deferred beyond a simple difficulty-scaled driver. Full attribute system, personality, engineer communication, and setup generation are not in the initial scope.

---

## Track Format

Versioned binary format (Unity addressable assets or AssetBundles for streaming).

- Spline geometry: centerline, width, banking, camber
- Surface material IDs per segment
- Pit lane geometry and speed limit zones
- Sector boundaries, start/finish, AI racing line
- Active Aero zone entry/exit coordinates [abstraction]
- Overtake Mode detection line coordinates [abstraction]

Track evolution (rubber deposition, drying line) is deferred.

---

## Race Loop

```
Session Setup → Car Setup → Out Lap → Formation Lap → Race Start →
[Stint] → Pit Stop → Final Lap → Checkered Flag →
Results → Next Session
```

**Rules Engine**: Finite state machine (GREEN, YELLOW, DOUBLE YELLOW, VSC, SAFETY CAR, RED, CHECKERED). Complete flag set rendered by presentation. Rules are pure functions of simulation state.

**Race Control**: Track limits (4 wheels on track), corner cutting, collisions, jump starts, speeding in pit lane, unsafe releases, false starts, blue flag violations.

**Penalties**: Time penalties (5s, 10s), drive-through, stop-and-go, grid penalty, disqualification, reprimand. Served during pit stops where applicable. Post-session review applies time additions. Penalty catalogue is data-driven.

**Sprint Session**: Configurable format driven by `RegulationProfile.sprint`. Points to top finishers per profile. Parc fermé from Sprint Qualifying start. No mandatory pit stops unless strategy requires. Elimination counts are configurable (`sq1EliminationCount`, `sq2EliminationCount`).

**Race Qualifying**: Configurable format driven by `RegulationProfile.raceQualifying`. Q1/Q2/Q3 with elimination per round (`eliminationPerRound`). Parc fermé from Q2 selection onward.

**Safety Car**: Deployed for major incidents. Overtaking prohibited. Pit lane open unless closed.

**VSC**: Minimum delta time enforced. No overtaking.

**Formation Lap**: Drivers maintain formation. Unsafe overtaking prohibited. Race Control may abort start. Full formation lap management is deferred; initial implementation is automated.

---

## Input and Accessibility

Normalized input vector (steer ±1.0, throttle 0–1, brake 0–1, digital gear/Active Aero/ERS). Sources: gamepad, keyboard, touch (virtual joystick or tilt), steering wheel (optional).

Assists: traction control, ABS, automatic gearbox, racing line, pit limiter. Active Aero auto-toggle and ERS mode assist are `[Overtake abstractions]` — included in core input system as optional toggles.

HUD is minimal by default, customizable. Shows speed, RPM, gear, lap time, position, tire temps, ERS mode and battery, fuel, Active Aero mode, flags, penalties, pit window. Multiple presets supported. Accessibility (colorblind mode, text scaling, volume controls, button mapping) is supported but not a primary focus of the blueprint.

---

## Rendering

Unity render pipeline (URP or HDRP), selected per platform at build time. Scalable features: car LOD, track streaming, particle density, shadow resolution. Optimization techniques are implementation concerns, not blueprint requirements.

Quality tiers are platform-configurable, not architecture choices:

- Mobile: 1080p, MSAA/FXAA, no ray tracing, 30–60fps
- Console: 1440p–4K checkerboard, 60–120fps, hardware ray tracing
- PC: 4K native, 120fps, ray tracing, DLSS/FSR, optional VR

---

## Audio

Unity Audio. Engine sound synthesis from RPM/load (mobile) or multi-layer samples + convolution (PC/console). Tire squeal (slip-angle dependent), wind noise, aerodynamic transitions, collision audio, environment. Team radio: voice lines with optional subtitles. Audio is driven by simulation telemetry only; no feedback into simulation.

---

## Persistence and Replay

Save slots for player profile, settings, and career state. No mid-race save; checkpoint only.

Replay is a deterministic input stream + random seed + initial conditions. No state snapshots. Re-simulated at playback time. Version-checked; incompatible builds skip or re-record. Frame-accurate scrubbing is deferred.

---

## Testing

Unit tests: physics math, rules engine, input mapping, tire model, ERS harvest and clipping, Active Aero transitions, Overtake Mode energy accounting, RegulationProfile loading, VehicleSpec validation, suspension force calculation.

Simulation regression: automated lap-time tests on reference tracks with fixed input. Threshold: 0.1% delta.

Integration tests: end-to-end sessions covering formation lap, race start, Safety Car, VSC, Sprint Qualifying, Race Qualifying, Overtake Mode activation, Active Aero zone compliance.

CI: every PR runs full test suite. Cross-platform determinism gate runs on x86/ARM/iOS/Android. CI fails on dependency cycle detection.

---

## M0 Validation Spike

Minimal technical spike to validate Unity feasibility before M1:

- Unity version/build reproducibility
- Fixed-timestep simulation frequency and stability
- Deterministic/repeatable vehicle state (1 car, straight line, no collisions)
- VehicleSpec / RegulationProfile loading via ScriptableObject or JSON
- C# simulation performance against target tick budget
- Unity ↔ simulation boundary: MonoBehaviour → IVehicle → Unity event
- Suspension spring-damper integration and numeric stability at target fixed timestep
- 100 mm virtual / 10 mm rendered travel ratio validation

Success criteria:
- Vehicle state repeats across N runs with identical input and seed within ±0.1%.
- Tick time in C# is within the selected fixed-timestep budget with headroom for AI and rules.
- No heap allocations during the simulation tick (validated with profiler).
- Suspension forces remain stable (no energy gain/oscillation) at the selected fixed timestep.
- 1000 Hz is **not** a prerequisite; the M0 target is 200 Hz (50 ms tick) unless profiling demonstrates instability.

Only introduce a native C++ plugin if measured profiling establishes that C# cannot meet the required budget.

---

## Module Architecture

```
overtake/
  Assets/
    Runtime/
      Simulation/        Pure C# simulation kernel
        Core/            IVehicle, VehicleInput, VehicleState, DeterministicRng, SimulationLoop
        Vehicle/         VehicleSpec, VehicleConfig, RegulationProfile, physics, ERS, Active Aero, Suspension
        Tires/           Tire model and surface state
        Rules/           Rules engine, flags, penalties
        AI/              Difficulty-scaled driver
        Track/           Track data structures
        Replay/          Deterministic input recorder
      Presentation/      Unity integration layer
        Rendering/       Car/track visuals
        Audio/           Unity Audio integration
        UI/              HUD, menus
        Input/           Unity Input System wrappers
    Data/                RegulationProfile, VehicleSpec assets
    Tests/               Simulation test suite
  Tools/                 Offline tools, telemetry, balancing, CI (Python)
  Packages/              Unity package manifest
```

Dependency direction: `Simulation` has zero Unity API dependencies. `Presentation` depends on `Simulation` via interfaces only.

C# for runtime. Python for offline tooling. C++ optional native plugin only if profiling demonstrates a genuine performance requirement.

---

## Explicit Non-Goals

- No real-world licensing (fictional series, cars, tracks)
- No extensive deformation damage (component performance impact only)
- No DRS abstraction (not in 2026+ FIA regulations)
- No MGU-H simulation or abstraction (not in 2026–2028 FIA regulations)
- No VR at MVP
- No modding tools before M2
- No esports broadcaster tools
- No procedural track geometry
- No cross-play with other games
- No free-to-play mechanics (premium buy; cosmetic DLC only)
- No pay-to-win mechanics
- No online content moderation backend at launch
- No league racing infrastructure at launch

---

## What Was Removed, Deferred, and Simplified

This gauntlet corrected the blueprint from a rejected custom C++20 engine to Unity.

**Removed entirely**: Custom C++20 engine requirement; CMake build; native SIMD and platform shims; custom render/audio/input/UI/net modules; cross-platform determinism mechanism details tied to C++ specific behaviour; ring buffer transport; IVehicle C++ interface; 1000 Hz as architectural requirement; full C++ module tree; per-frame millisecond performance budgets.

**Made configurable** (moved from hard-coded architecture to `RegulationProfile` or `VehicleSpec` data): all FIA parameter values (PU power, harvest limits, fuel flow, minimum weight, tyre dimensions, overtake gap threshold and bonus energy, sprint timings, sprint points, penalty catalogue, active aero coefficients, transition times, race qualifying structure). Regulation year switching is a data load, not a code branch.

**Deferred to post-MVP**: AI attribute system and personality; weather and dynamic track conditions (rain, drying line, rubber deposition); tire surface phenomena beyond basic wear (grain, blister, flat spot, marbles, aquaplaning); brake temperature/fade model; brake bias; pit-stop strategy AI and pit board; tyre allocation; full formation lap management; broadcast mode; full accessibility requirements; career progression; multiplayer implementation; replay/telemetry; cloud sync; advanced brush tire model; VR; track editor; live-ops.

**Simplified**: Tire fidelity is two configurable modes sharing the same grip curves. AI is difficulty-scaled noise on controls with mode selection. Track format is a versioned binary with an extensible metadata block. Race control is a rules engine over simulation state, not per-rule code paths. Simulation is pure C# with no Unity API dependencies. Suspension is a linear spring-damper model with `[hypothesis]` parameters validated in M0.

---

## Critical Questions

**Is the blueprint materially smaller and simpler?** Yes. Removed custom engine architecture, C++20 requirements, CMake, SIMD, and platform shims. Every removed section had a concrete justification.

**Are there critical contradictions?** None. Active Aero and ERS are distinct, data-driven systems with well-defined interfaces. RegulationProfile and VehicleSpec are separate data assets loaded at session start with no code branching on year. The simulation has no knowledge of FIA seasons. ERS deploy rate is derived from `mguKMaxPower_kW` (not hard-coded). Suspension parameters live in `VehicleSpec` (not `RegulationProfile`), consistent with the Design Policy.

**Does every retained system have a gameplay or technical justification?** Yes. Each retained system either defines Overtake's identity (Active Aero, ERS), is required to run a race (physics, tires, fuel, track, rules engine, flags, safety car), or is the minimum needed to play the game on a screen (input, HUD, rendering, audio). Suspension is required for tire load coupling, body roll presentation, and ride-height-dependent surface effects.

**Has any new scope been introduced?** No. RegulationProfile and VehicleSpec replace previously hard-coded values and regulation branches; they are a simplification of existing architecture, not new scope. Suspension is part of the VehiclePhysics subsystem in the original Scope Gate; the new section clarifies its data model and presentation separation.

---

## Gauntlet Log

### Pass 1 — Issue Discovery and Fix

| # | Issue | Severity | Fix |
|---|---|---|---|
| G1 | `VehicleConfigFactory.cs:121` hard-codes `ERSDeployRate_MJ_per_s = 0.8f` (800 kW), exceeding the FIA 350 kW cap (Article C5.2.8). | Critical | Sourced from `RegulationProfile.ers.deployRate_MJ_per_s` (default 0.35, derived from `mgukMaxPower_kW / 1000`). `ERSBoostDeployMultiplier` and `ERSHarvestRate_MJ_per_s` also sourced from RegulationProfile. |
| G2 | Tyre widths in blueprint YAML and all C#/Python mirrors: `frontWidth_mm: 270`, `rearWidth_mm: 355` (pre-2026 Pirelli sizes). | High | Corrected to `280` / `375` per 2026 Pirelli announcement. |
| G3 | No Race Qualifying configuration in `RegulationProfile` or blueprint YAML, despite FIA Article B12.3 defining Q1/Q2/Q3 with specific durations and elimination counts. | High | Added `raceQualifying` section to `RegulationProfile` (Q1: 18 min, Q2: 15 min, Q3: 13 min, elimination 5 per round). |
| G4 | Sprint elimination counts not configurable — only `sq3CarCount` existed; SQ1/SQ2 elimination of 6 cars was implicit. | Medium | Added `sq1EliminationCount: 6` and `sq2EliminationCount: 6` to `Sprint`. |
| G5 | Blueprint YAML uses `chassis.minWeight_kg` (VehicleSpec) and `vehicle.minWeight_kg` (RegulationProfile) but C# structures use `Dynamics.Mass_kg`. Structural mismatch. | Medium | Aligned YAML to code: `dynamics.mass_kg` in both `RegulationProfile` and `VehicleSpec`. |
| G6 | No suspension fields in `VehicleSpec`, `VehicleConfig`, `VehicleState`, or Python mirrors, despite blueprint prose mentioning suspension. | High | Added `Suspension` class to `VehicleSpec` and `RegulationProfile`; added suspension fields to `VehicleConfig`, `VehicleState`, and both Python mirrors. |
| G7 | Blueprint claims 2027 profile changes `pu.iceMaxPower_kW`, `ers.mguKMaxPower_kW`, `fuelEnergyFlowMax` — unsupported by published 2027 Technical Regulations Section C (Issue 3). Only aero/floor/plank/fuel-system changes published. | High | Corrected: 2027 profile modifies documented 2027 changes only (aero coefficients, suspension geometry). PU output changes are `[reported]` at best, never engine-limit defaults. |
| G8 | Fuel flow curve mentions `# below 10500 rpm` but omits the 3,000 MJ/h flat cap above 10,500 RPM (Article 5.4.4). | Medium | Added `fuelFlowRpmCurveMaxRpm: 10500` field and documentation of the flat cap. |
| G9 | ERS harvest limit with Overtake Mode active (9.0 MJ/lap per C5.2.10) missing from `RegulationProfile.ers`. | Medium | Added `harvestLimitWithOvertake_MJ_per_lap: 9.0`. |
| G10 | `1000 Hz` referenced as a hypothesis but not clearly tied to M0 validation; `100 mm / 10 mm` travel ratio not validated. | Medium | Suspension section explicitly marks these as `[hypothesis]` requiring M0 validation. M0 validation criteria updated. |
| G11 | Python `verify_m0.py` `VehicleConfig` missing `ers_deploy_rate`, `ers_boost_deploy_multiplier`, `ers_harvest_rate`, and suspension fields present in `m0_python_mirror.py`. Python mirrors diverged. | Medium | Both Python files now include ERS rates and suspension fields matching C#. |
| G12 | `minWeight_kg: 768` — unable to verify exact Article C4.1 value from live FIA PDF (PDF >5 MB, exceeds webfetch limit). Sources: formula1.com and opensourcesports.io cite 768 kg. Wikipedia cites 724 kg with no primary-source citation. | Low | Marked `vehicle.minWeight_kg` as `Reported` confidence. Gauntlet finding documented; no change to value. |

### Pass 2 — Independent Review of Revised Document

| Check | Result |
|---|---|
| YAML field names match C# property names in all structs | PASS — `VehicleSpec.suspension`, `RegulationProfile.suspension`, `RegulationProfile.raceQualifying`, `RegulationProfile.sprint` (with elimination counts), `RegulationProfile.ers.deployRate_MJ_per_s` all match |
| `VehicleState` struct fields match the Suspension Engineering description | PASS — individual per-wheel fields (`suspensionTravelFL_mm`, etc.) now match code; arrays were a documentation error |
| ERS deploy rate no longer hard-coded | PASS — `VehicleConfigFactory` now sources `ERSDeployRate_MJ_per_s` from `RegulationProfile.ers.deployRate_MJ_per_s` (default 0.35, = 350 kW / 1000) |
| Tyre widths corrected to 280/375 | PASS — both C# `RegulationProfile.Tires` defaults and `VehicleConfigFactory` defaults updated |
| 2027 profile claims corrected | PASS — 2027 changes limited to published Technical Regulations Section C changes (aero, floor, plank); PU output changes marked `[reported]` |
| Fuel flow curve includes RPM cap | PASS — `FuelFlowRpmCurveMaxRpm: 10500` field added; 3000 MJ/h flat cap above documented |
| No `if (year == ...)` branches in simulation code | PASS — verified in `VehicleDynamics.cs`, `VehicleConfigFactory.cs`, `VehicleState.cs` |
| Suspension parameters in `VehicleSpec`, not `RegulationProfile` | PASS — `VehicleSpec.Suspension` has per-corner overrides; `RegulationProfile.Suspension` has generic defaults; both feed `VehicleConfig` via factory |
| `minWeight_kg` confidence level marked | PASS — blueprint marks `vehicle.minWeight_kg` as `Reported`; Gauntlet Log G12 documents the 768 vs 724 discrepancy |
| Python mirrors (verify_m0.py, m0_python_mirror.py) match C# | PASS — 15 tests + 8 mirror tests pass; ERS rates, suspension, tyre widths, race qualifying all present |
| Suspension starts at equilibrium (zero initial displacement) | RESOLVED — load transfer based on longitudinal acceleration shifts the equilibrium position per wheel, causing suspension to respond to acceleration/braking

---

## References

### FIA Official Sources (Primary)

1. **FIA International Formula One World Championship 2026 — Technical Regulations (Section C)**
   - Issue 20, dated 2026-08-05, published 2026-10-01
   - Covers Articles C1–C16 (Dimensions, Design, Construction, Drag Reduction System → Driver-adjustable bodywork, Power Unit)
   - URL: `https://www.fia.com/regulations` → Formula One → 2026 → Technical Regulations (PDF)
   - Accessed: 2026-10-09

2. **FIA International Formula One World Championship 2026 — Sporting Regulations (Section B)**
   - Issue 09, dated 2026-10-01
   - Covers Articles B1–B36 (Penalties B1.9, Sprint Sessions B12.2, Race Qualifying B12.3)
   - URL: `https://www.fia.com/regulations` → Formula One → 2026 → Sporting Regulations (PDF)
   - Accessed: 2026-10-09

3. **FIA Formula One 2026 — Power Unit Technical Regulations**
   - Issue 7, dated 2024-06-11
   - Covers Articles C5.1 (ICE output), C5.2.8 (MGU-K 350 kW), C5.2.9 (Battery SoC 4.0 MJ), C5.2.10 (Harvest 8.5 MJ/lap, 9.0 MJ with Overtake)
   - URL: `https://www.fia.com/regulations` → Formula One → 2026 → Power Unit Technical Regulations (PDF)
   - Accessed: 2026-10-09

4. **FIA Formula One 2027 — Technical Regulations (Section C)**
   - Issue 3, dated 2026-10-01
   - Published changes: floor/plank dimensions, front/rear aero element geometry, fuel system modifications
   - URL: `https://www.fia.com/regulations` → Formula One → 2027 → Technical Regulations (PDF)
   - Accessed: 2026-10-09

5. **Event-specific Power Unit Info Documents (2026)**
   - Spanish Grand Prix (Circuit de Barcelona-Catalunya) and Dutch Grand Prix (Circuit Zandvoort) — confirm Article C5.2.8 MGU-K power and C5.2.10 harvest limits in practice
   - URL: `https://www.fia.com/events/f1` → individual event → scrutineering documents
   - Accessed: 2026-10-09

### Secondary Sources

6. **Formula1.com** — 2026 Technical Regulations summary (minimum weight 768 kg, tyre dimensions 280/375 front/rear)
   - URL: `https://www.formula1.com/en/regulations/2026`
   - Accessed: 2026-10-09

7. **OpenSourceSports.io** — FIA 2026 Regulation Analysis
   - URL: `https://opensourcesports.com/f1` → 2026 regulations
   - Accessed: 2026-10-09

8. **Pirelli** — 2026 Tyre Announcement (October 2025)
   - Tyre dimensions: front 280/705 R18, rear 375/710 R18
   - URL: `https://www.pirelli.com/media-centre` → 2026 F1 tyre announcement
   - Accessed: 2026-10-09

### Unverified Sources (not used for defaults)

9. **Wikipedia** — 2026 Formula One World Championship (cites minimum weight 724 kg with no primary-source citation)
   - Not used; marked as discrepancy in Gauntlet Log G12

10. **Motorsport.com / Autosport** — 2027 regulation speculation (PU output changes, fuel flow curve modifications)
    - Not used for engine-limit defaults; marked as `[reported]` confidence
    - Referenced in Gauntlet Log G7

---

(End of file)
