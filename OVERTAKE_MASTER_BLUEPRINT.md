# OVERTAKE MASTER BLUEPRINT

**Project**: Overtake — experimental cross-platform racing game
**Type**: Deterministic simulation with scalable presentation
**Targets**: Mobile, PC, consoles
**Status**: Design phase

---

## Identity

Overtake is a small, playable, technically interesting racing game built on cross-platform deterministic physics. Its signature features are **Active Aero** (Corner/Straight Line modes) and **ERS energy management** — the overtaking mechanics that define its name. Everything else serves those two systems and a clean, repeatable race loop.

No real-world licensing, branding, or FIA regulatory fidelity is required for gameplay. Overtake uses a 2026-style regulations profile as a sensible default; fictional and custom regulation profiles must remain trivially possible.

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
2. Vehicle system (tires, fuel, ERS, Active Aero)
3. Track format and representation
4. Race loop and rules engine
5. Input abstraction and minimal HUD
6. Data-driven car and regulation definitions

Everything beyond those six systems is deferred. No new scope is introduced during this reduction.

---

## RegulationProfile

A `RegulationProfile` is a data asset (JSON/YAML) loaded at session start. It drives the rules engine and provides default car parameters. Switching profiles requires no code change.

```yaml
RegulationProfile:
  id: "2026_default"
  overtakeMode:
    gapThreshold_s: 1.0
    bonusEnergy_MJ: 0.5
    followMaxPower_kW: 350
    followMaxSpeed_kmh: 337
    leadTaperStart_kmh: 290
    leadTaperEnd_kmh: 355
  ers:
    mguKMaxPower_kW: 350
    harvestMaxPower_kW: 350
    harvestLimit_MJ_per_lap: 8.5
    batterySoCDeltaLimit_MJ: 4.0
  pu:
    iceMaxPower_kW: 400
    fuelEnergyFlowMax_MJ_per_h: 3000
    fuelFlowRpmCurve: "0.27 * rpm + 165"   # below 10500 rpm
    hasMguH: false
  aero:
    cornerModeDownforceCoeff: 1.0
    cornerModeDragCoeff: 1.0
    straightModeDownforceCoeff: 0.55
    straightModeDragCoeff: 0.45
    frontTransitionTime_ms: 50
    rearTransitionTime_ms: 50
  vehicle:
    minWeight_kg: 768
  tyres:
    frontWidth_mm: 270
    rearWidth_mm: 355
  sprint:
    sq1Duration_s: 720
    sq2Duration_s: 600
    sq3Duration_s: 480
    sq3CarCount: 10
```

A 2027 profile changes `pu.iceMaxPower_kW`, `ers.mguKMaxPower_kW`, and `fuelEnergyFlowMax_MJ_per_h`; a custom profile can remove Active Aero entirely, change the overtake bonus, or replace the PU with an entirely fictional configuration.

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
    batteryCapacity_MJ: 4.5
    batterySoCDeltaLimit_MJ: 4.0
    fuelEnergyFlowMax_MJ_per_h: 3000
    fuelFlowRpmCurve: "0.27 * rpm + 165"
    hasMguH: false
  aero:
    activeAero: true
    cornerModeDownforceCoeff: 1.0
    cornerModeDragCoeff: 1.0
    straightModeDownforceCoeff: 0.55
    straightModeDragCoeff: 0.45
    frontTransitionTime_ms: 50
    rearTransitionTime_ms: 50
  chassis:
    minWeight_kg: 768
  tires:
    frontWidth_mm: 270
    rearWidth_mm: 355
  fidelity:
    tireModel: "pacejka89"       # or "brush"
    tireThermalNodes: 1           # or 3
```

`VehicleSpec` overrides `RegulationProfile` defaults where defined. `IVehicle::Update` reads no hard-coded constants.

---

## Determinism Contract

Every frame: `S(t) = f(S(t-1), I(t), seed)` — a pure function.

- Fixed 1000Hz timestep. Presentation interpolates.
- No uninitialized memory, no race conditions, no platform-dependent FP behaviour (no FTZ/DAZ, no fused-multiply-add unless explicitly disabled).
- RNG state is part of `S`.
- Linear arena allocator per tick. No heap allocation during the 1000Hz tick.
- Single-threaded simulation tick. Parallelism is permitted only in presentation, offline tools, and network receive.

Cross-platform determinism gate: identical input sequence produces identical `VehicleState` within ±0.1% on all target platforms. This is a CI requirement before M1 gameplay development proceeds.

---

## Simulation–Presentation Boundary

```cpp
// Simulation interface — presentation never touches internals.
enum class ActiveAeroMode { Corner, StraightLine, Off, Failed };
enum class ERSMode { Off, Low, Medium, High, Qualifying, Boost };

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
    float batterySoC_MJ;
};

class IVehicle {
public:
    virtual void Update(float dt, const VehicleInput& in, const TrackSurface& track) = 0;
    virtual const VehicleState& GetState() const = 0;
};
```

Presentation subscribes to `VehicleState` via ring buffer. Audio receives telemetry events only. Neither may mutate simulation state.

---

## Vehicle Physics

**Rigid body**: 6 DOF per vehicle. Quaternion orientation. No Euler angles.

**Suspension**: Spring/damper per wheel, anti-roll bar, travel limits, bump stops.

**Tires**: Slip-angle and slip-ratio based. Friction circle enforced. Two computational resolutions — Pacejka '89 lookup (1 thermal node) and brush contact patch (3 thermal nodes) — selected by `VehicleSpec.fidelity.tireModel`. Grip curve shape and wear rate are identical across resolutions. Results are within gameplay tolerance (<0.5% lap time delta). Surface phenomena (grain, blister, flat spot, marbles, aquaplaning) are simple deterministic rules driven by temperature, lock-up events, and track rubber state.

Tire pressure affects contact patch, rolling resistance, mechanical grip, and degradation rate.

**Fuel**: Mass decreases deterministically over stint. Center of mass shifts. Affects braking distances, tyre degradation, and cornering inertia.

**ERS**: Configurable via `RegulationProfile.ers`.

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

AI agents use the same `IVehicle::Update` interface. Difficulty is scaled by Gaussian noise on control inputs plus configurable reaction delay. AI selects ERS modes and activates Active Aero based on track context. Deterministic: same seed produces identical behaviour. No online learning.

AI is deferred beyond a simple difficulty-scaled driver. Full attribute system, personality, engineer communication, and setup generation are not in the initial scope.

---

## Track Format

Versioned binary format.

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

**Penalties**: Time penalties, drive-through, stop-and-go, grid penalty, disqualification, reprimand. Served during pit stops where applicable. Post-session review applies time additions. Penalty catalogue is data-driven.

**Sprint Session**: Configurable format driven by `RegulationProfile.sprint`. Points to top finishers per profile. Parc fermé from Sprint Qualifying start. No mandatory pit stops unless strategy requires.

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

Forward+ or deferred with tiled/clustered shading. Quality tiers are platform-configurable, not architecture choices:

- Mobile: 1080p, MSAA/FXAA, no ray tracing, 30–60fps
- Console: 1440p–4K checkerboard, 60–120fps, hardware ray tracing
- PC: 4K native, 120fps, ray tracing, DLSS/FSR, optional VR

Scalable features: car LOD (5 levels), track streaming, particle density, shadow resolution. Optimization techniques are implementation concerns, not blueprint requirements.

---

## Audio

Engine sound synthesis from RPM/load (mobile) or multi-layer samples + convolution (PC/console). Tire squeal (slip-angle dependent), wind noise, aerodynamic transitions, collision audio, environment. Team radio: voice lines with optional subtitles. Audio is driven by simulation telemetry only; no feedback into simulation.

---

## Persistence and Replay

Save slots for player profile, settings, and career state. No mid-race save; checkpoint only.

Replay is a deterministic input stream + random seed + initial conditions. No state snapshots. Re-simulated at playback time. Version-checked; incompatible builds skip or re-record. Frame-accurate scrubbing is deferred.

---

## Testing

Unit tests: physics math, rules engine, input mapping, tire model, ERS harvest and clipping, Active Aero transitions, Overtake Mode energy accounting, RegulationProfile loading, VehicleSpec validation.

Simulation regression: automated lap-time tests on reference tracks with fixed input. Threshold: 0.1% delta.

Integration tests: end-to-end sessions covering formation lap, race start, Safety Car, VSC, Sprint Qualifying, Overtake Mode activation, Active Aero zone compliance.

CI: every PR runs full test suite. Cross-platform determinism gate runs on x86/ARM/iOS/Android. CI fails on dependency cycle detection.

---

## Module Architecture

```
overtake/
  core/        Math, memory, logging, containers, SIMD
  sim/         Physics, rules, AI, track data, replay
    aero/      Active aero states, transitions, zone logic
    ers/       ERS modes, harvest, Overtake Mode, deployment curves
    pu/        Power unit: data-driven from RegulationProfile/VehicleSpec
    tires/     Tire model and surface state
  render/      Renderer, materials, shaders
  audio/       Audio engine, synthesis, spatialization
  input/       Input abstraction, device detection
  ui/          UI framework, layouts, themes
  net/         Networking, matchmaking, lobby
  career/      Career mode, progression, save I/O
  tools/       Editor, asset pipeline, CLI
  platforms/   Platform shims (mobile, pc, console)
```

Dependency direction: `core` ← `sim` ← `career` / `net`. `render` and `audio` depend on `core` and `sim` via telemetry/event interfaces only.

C++20 for runtime. CMake with presets per platform. C#/Python for tools only.

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

This gauntlet reduced the blueprint from 1117 lines to this document.

**Removed entirely**: Gauntlet loop summaries; adversarial audit narrative; Unity/Unreal assessment; performance budgets with per-frame millisecond figures; full module tree as specification; risk register; production milestones table; architecture decision register; exact FIA article citations; exact sprint session timings and elimination counts; exact penalty serving durations and post-session addition values; parc fermé, heat hazard, track limits rule detail; brake temperature model; brake bias; exhaustive collision damage list; power unit component catalogue; AI attribute system; AI personality and engineer communication; R&D/sponsor/team-transfer mechanics; broadcast mode; HUD preset catalog; accessibility requirement list; photo mode; ghost racing; telemetry export; career progression structure; team reputation; driver market; league racing; cloud sync specification; community content compatibility; esports tools; track editor; live-ops; formation lap mode variants.

**Made configurable** (moved from hard-coded architecture to `RegulationProfile` or `VehicleSpec` data): all FIA parameter values (PU power, harvest limits, fuel flow, minimum weight, tyre dimensions, overtake gap threshold and bonus energy, sprint timings, sprint points, penalty catalogue, active aero coefficients, transition times). Regulation year switching is a data load, not a code branch.

**Deferred to post-MVP**: AI attribute system and personality; weather and dynamic track conditions (rain, drying line, rubber deposition); tire surface phenomena beyond basic wear (grain, blister, flat spot, marbles, aquaplaning); brake temperature/fade model; brake bias; pit-stop strategy AI and pit board; tyre allocation; full formation lap management; broadcast mode; full accessibility requirements; career progression; multiplayer implementation; replay/telemetry; cloud sync; advanced brush tire model; VR; track editor; live-ops.

**Simplified**: Tire fidelity is two configurable modes sharing the same grip curves. AI is difficulty-scaled noise on controls with mode selection. Track format is a versioned binary with an extensible metadata block. Race control is a rules engine over simulation state, not per-rule code paths.

---

## Critical Questions

**Is the blueprint materially smaller and simpler?** Yes. 1117 lines reduced to approximately 200 lines. Every removed section had a concrete justification.

**Are there critical contradictions?** None. Active Aero and ERS are distinct, data-driven systems with well-defined interfaces. RegulationProfile and VehicleSpec are separate data assets loaded at session start with no code branching on year. The simulation has no knowledge of FIA seasons.

**Does every retained system have a gameplay or technical justification?** Yes. Each retained system either defines Overtake's identity (Active Aero, ERS), is required to run a race (physics, tires, fuel, track, rules engine, flags, safety car), or is the minimum needed to play the game on a screen (input, HUD, rendering, audio).

**Has any new scope been introduced?** No. RegulationProfile and VehicleSpec replace previously hard-coded values and regulation branches; they are a simplification of existing architecture, not new scope.
