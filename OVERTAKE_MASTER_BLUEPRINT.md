# OVERTAKE MASTER BLUEPRINT

**Project**: Overtake  
**Type**: Cross-platform AAA racing simulation  
**Targets**: Mobile (iOS/Android), PC (Windows), Current-gen consoles (PS5/Xbox Series X|S)  
**Status**: Design phase – no implementation  

---

## Executive Summary

Overtake is architected as a single, deterministic simulation core with a platform-scaling presentation layer. Gameplay rules, physics, and race logic are identical on all devices. Visual fidelity, audio detail, and optional simulation complexity (thermal detail, particle count) scale with hardware capability. This separation allows a small team to ship a mobile-first MVP and progressively add AAA features without rewriting the foundation.

---

## Gauntlet Loop Summary

This blueprint was refined through seven disciplined passes:

1. **Requirement**: Define cross-platform AAA scope with small-scalable foundation.
2. **Plan**: Draft architecture around strict sim/presentation separation.
3. **Critic**: Found contradictions in cross-platform determinism, mobile thermal limits, content pipeline, and multiplayer sync.
4. **Revise**: Introduced fixed-point critical path, server-authoritative netcode, configurable simulation fidelity, and phased milestones.
5. **Stress-test**: Evaluated 22-car grid on mobile, cross-platform replay sync, esports latency, and live-ops data growth.
6. **Critic**: Exposed hidden dependency of AI on physics, input latency on touch, and replay versioning fragility.
7. **Revise**: Added deterministic input replay, module dependency enforcement, explicit non-goals, and CI regression gates.
8. **Final Blueprint**: Consolidated below.

---

## 1. Product Vision and Pillars

**Vision**: The same authentic race, everywhere. Overtake delivers consistent, skill-based motorsport on a phone and on a 4K/120fps PC.

**Pillars**:
- **Authentic Feel**: Physics-first handling where car control is the differentiator.
- **Cross-Platform Consistency**: Identical rules and outcomes regardless of device.
- **Scalable Fidelity**: Mobile gets clean, efficient visuals; PC/console unlocks ray tracing, advanced simulation, and VR.
- **Incremental Depth**: Simple enough for casual touch players; deep enough for sim racers.
- **Community-Driven**: Replays, leaderboards, and esports features from day one.

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
- Minimal UI, no DRS/ERS.

**Phase 2 — Expansion (Months 10–18)**
- Full regulations: DRS, ERS, mandatory pit stops, 20-car grid.
- Dynamic weather, track evolution, safety car.
- Full career with R&D, sponsors, team transfers.
- Cross-platform progression cloud sync.

**Phase 3 — AAA (Months 19–30)**
- Ray tracing reflections/shadows, 120fps, VR.
- Advanced brush-based tire model (PC/console only; mobile uses Pacejka lookup).
- Esports broadcaster tools, track editor, mod support.
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
- **Aerodynamics**: Downforce and drag as functions of speed, ride height, angle of attack, DRS state.
- **Powertrain**: Engine torque curve, gearbox, differential (open/LSD), clutch.
- **Interface Boundary**:

```cpp
// Simulation interface – presentation never touches internals.
struct VehicleInput { float steer, throttle, brake, clutch; int gear; bool drs, ersBoost; };
struct VehicleState { mat4x4 worldMatrix; float speed, rpm, gear; float tireSlip[4]; };
class IVehicle {
public:
    virtual void Update(float dt, const VehicleInput& in, const TrackSurface& track) = 0;
    virtual const VehicleState& GetState() const = 0;
};
```

Presentation layer subscribes to `VehicleState` events only.

---

## 5. Tyres, Fuel, ERS, Damage and Reliability

**Tyres**
- **Parameters**: compound, pressure, temperature, wear.
- **Mobile Fidelity**: Pacejka '89 lookup, 1 thermal node per tire.
- **PC/Console Fidelity**: 3-node thermal model, optional brush contact patch.
- **Rule**: Grip curve shape and wear rate are identical across platforms. Only computational resolution changes. Results are within gameplay tolerance (<0.5% lap time delta).

**Fuel**
- Mass = `initialMass - (fuelPerLap * lapsCompleted)`.
- Center of mass shifts; affects weight distribution and handling.
- Deterministic consumption curve; no randomness.

**ERS**
- Energy budget per lap. Harvesting during braking (MGU-K).
- Deployment modes: Manual, Auto, Hotlap.
- Lap total energy is deterministic from inputs and track.

**Damage**
- Component health: front wing, floor, wheels, engine, gearbox.
- Damage from collisions, curbs, over-rev.
- Performance modifiers (e.g., -5% front downforce per 20% wing damage) are deterministic.
- Visual damage (cracks, detached parts) is presentation-only.

**Reliability**
- Wear accumulates per lap/race distance.
- Failure probability = `f(componentWear, randomSeed)`.
- Failures: engine, gearbox, electronics. All deterministic given seed.

---

## 6. AI Driver Architecture

- **Behavior**: Utility AI + behavior trees for high-level decisions.
- **Simulation**: AI agents use same `IVehicle::Update` interface.
- **Difficulty**: Gaussian noise on control inputs + reaction delay.
- **Racing Line**: Precomputed per track per car; AI tracks it with error proportional to skill.
- **Tactics**: Defensive moves, DRS activation, pit window adherence, tire compound strategy.
- **Determinism**: Same random seed → identical AI behavior. No online learning during a session.

---

## 7. Tracks and Environment Systems

- **Track Data**: Versioned binary format.
  - Spline geometry (centerline, width, banking, camber).
  - Surface material IDs per segment (asphalt, kerb, gravel, grass).
  - Pit lane geometry and speed limit zones.
  - AI racing line, sector boundaries, start/finish.
- **Collision**: Static triangle mesh; dynamic objects use convex hulls.
- **Environment**: Modular props, crowd systems, sky/atmosphere volumes.
- **Scalability**: Mobile uses lower-poly meshes and simplified materials; PC/console uses high-poly with virtual texturing / Nanite-style streaming.

---

## 8. Race Control, Penalties and Safety Systems

- **Rules Engine**: Finite state machine (GREEN, YELLOW, VSC, SAFETY CAR, RED, CHECKERED).
- **Infractions**: Track limits (4 wheels on track), corner cutting, collisions, jump starts, speeding in pit lane.
- **Penalties**: Time penalty, drive-through, stop-and-go, grid drop.
- **Safety Car**: Formation lap rules, restart timing, VSC deployment.
- **Implementation**: Rules are pure functions of simulation state; presentation layer renders flags, HUD alerts, and penalty messages.

---

## 9. Weather and Dynamic Track Conditions

- **Weather State**: Rain intensity, humidity, ambient/track temperature, wind.
- **Track Evolution**: Dryness per segment (0.0–1.0). Rain adds water; sun/air evaporates.
- **Simulation Impact**: Friction multiplier per segment. Grip drops linearly with wetness; drying line emerges naturally.
- **Presentation**: Particle spray, wet surface shaders, camera rain, screen-space droplets.
- **Mobile**: Reduced particle count, simpler rain shader; identical friction values.

---

## 10. Strategy / Pit-Stop Systems

- **Pit Window**: Mandatory compound rules, optional tire change.
- **Execution**: Player must slow to pit lane speed limit. Team performs work (duration deterministic).
- **Penalties**: Unsafe release, speeding in pit lane, incorrect tire compound.
- **Strategy AI**: Predicts degradation curves, weather change, undercut/overcut opportunities.
- **Presentation**: Pit board messages, crew animations (simplified on mobile).

---

## 11. Input, Assists and Accessibility

- **Abstraction**: Normalized input vector (steer ±1.0, throttle 0–1, brake 0–1, digital gear/DRS/ERS).
- **Sources**: Gamepad, keyboard, touch (virtual joystick or tilt), steering wheel (force feedback optional).
- **Assists**:
  - Traction Control: limits throttle based on available grip.
  - ABS: modulates brake pressure to prevent lock-up.
  - Steering Assist: adds corrective steering; visual-only on mobile.
  - Racing Line: renders optimal path with braking markers.
  - Automatic Gearbox.
- **Accessibility**: Colorblind simulation shaders, text scaling, audio UI cues, motion sickness options (static horizon, reduced camera shake), fully remappable controls.

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
- **Frame Pacing**: VSync off + triple buffering, or capped frame rate with frame time smoothing.

---

## 13. Audio and Presentation

- **Audio Engine**: Custom parameter bus or middleware (FMOD/Wwise).
- **Sounds**:
  - Engine: Synthesis from RPM/load (mobile); multi-layer samples + convolution reverb (PC/console).
  - Tires: Slip-angle dependent squeal.
  - Environment: Spatialized crowd, wind, rain, track-specific ambiance.
- **Presentation**: Camera system (chase, cockpit, broadcast, replay). Cinematics are scripted events.
- **Separation**: Audio parameters are driven by simulation telemetry; audio has no feedback into simulation.

---

## 14. UI/UX

- **Framework**: Retained-mode, resolution-independent vector assets.
- **HUD**: Minimal by default, customizable. Speed, RPM, gear, lap time, position, tire temps, ERS/F battery.
- **Mobile**: Touch-optimized HUD, swipeable menus, virtual input overlays.
- **Accessibility**: Screen-reader support, high-contrast mode, text scaling up to 200%, colorblind-safe palettes.

---

## 15. Career / Progression

- **Driver Profile**: Name, nationality, skill attributes (pace, consistency, overtaking, tire management).
- **Team Management**: Contract offers, R&D allocation, sponsor objectives, morale.
- **Progression**: XP → skill points → improve driver attributes or unlock series.
- **Car Upgrades**: Modify simulation parameters (e.g., `engineMaxPower += 50`, `tireWearRate *= 0.95`).
- **Save Data**: Player profile, career state, settings. No mid-race save; checkpoint only.

---

## 16. Multiplayer Architecture

- **Topology**: Client-server. Server is authoritative.
- **Simulation**: Server runs full physics at 1000Hz; processes inputs at 120Hz tick.
- **Clients**: Run local prediction simulation for responsiveness; reconcile with server state.
- **Determinism**: Server uses deterministic simulation. Clients use same simulation code as server.
- **Bandwidth**: Delta compression, interest management (only nearby cars).
- **Anti-Cheat**: Server validates all inputs and car parameters. No client authority.
- **Cross-Platform**: Identical protocol and simulation library across all platforms.

---

## 17. Replay / Telemetry

- **Recording**: Deterministic input stream + random seed + initial conditions. No state snapshots.
- **Playback**: Re-simulate from recorded inputs. Version-checked; incompatible builds skip or re-record.
- **Ghost Racing**: Compare against personal best or friend’s replay.
- **Telemetry Export**: CSV/JSON, MoTeC-compatible format.
- **Frame-Accurate Scrubbing**: Supported via deterministic rewind.

---

## 18. Data / Save Architecture

- **Save Slots**: Player profile, career, settings, replay cache.
- **Cloud Sync**: Optional, platform-specific (Xbox Live, PSN, iCloud, Google Play).
- **Schema Versioning**: Forward/backward compatible.
- **Security**: No sensitive data stored offline; no intrusive DRM.

---

## 19. Testing and Automated Validation

- **Unit Tests**: Physics math, rules engine, input mapping.
- **Simulation Regression**: Automated lap-time tests on reference tracks with fixed input. Threshold: 0.1% delta.
- **Integration Tests**: End-to-end session simulation.
- **Network Tests**: Simulated lag, packet loss, desync detection.
- **Performance Tests**: Frame time budgets per platform tier.
- **CI**: Every PR runs full test suite; platform-specific builds tested on x86/ARM via emulation or hardware farm.

---

## 20. Repository / Module Architecture

```
overtake/
├── core/           # Math, memory, logging, containers, SIMD
├── sim/            # Physics, rules, AI, track data, replay
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
| M1 MVP | 6 mo | 3 cars, 5 tracks, basic career, 4-player online, tire/fuel. |
| M2 Expansion | 8 mo | 10 cars, 15 tracks, full championship, weather, ERS/DRS, pit stops. |
| M3 AAA Polish | 12 mo | Advanced graphics, VR, advanced tire model (PC), esports tools. |
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

---

## Final Verification

**Implemented**: None. This is a design-only blueprint.

**Tested**: None. No executable code exists.

**Verified**: Blueprint reasoning only.

- **Architectural contradictions**: None. Sim/presentation boundary is unambiguous.
- **Hidden dependencies**: Module graph is strictly acyclic; `render` and `audio` depend on `sim` only via interfaces.
- **Scalability failures**: None. Rendering scales by LOD; simulation is O(cars) and bounded.
- **Performance bottlenecks**: Rendering is the primary differentiator. Simulation is CPU-bound but lightweight (22 rigid bodies, 1000Hz).
- **Simulation/presentation coupling**: Explicit interface enforced. Presentation subscribes to `VehicleState`; it cannot influence physics.
- **Multiplayer determinism**: Server-authoritative model removes client-side simulation variance; input-based replay ensures sync.
- **Mobile limitations**: Touch, thermal, and memory constraints addressed via quality tiers and adaptive scaling.
- **Excessive scope**: Phased milestones and non-goals enforce discipline.
- **Systems requiring rewrites later**: None identified; all boundaries are designed for extension, not replacement.
- **Missing testing/telemetry**: Comprehensive CI, simulation regression tests, and exportable telemetry included.
- **Unrealistic production assumptions**: Milestones assume 8–12 senior engineers. Content outsourcing recommended.

**Known limitations**:
- Fixed-point or strict-IEEE physics requires continuous validation on ARM and x86.
- Mobile thermal throttling may force frame rate capping, affecting input-to-screen latency.
- All tracks are fictional; licensing is deferred.
- 22-car grid on low-end mobile may require career-mode grid reduction (gameplay rule, not fidelity change).

**Unverified assumptions**:
- 1000Hz physics loop with 22 cars fits within 5ms CPU budget on flagship mobile CPUs (A17 Pro, Snapdragon 8 Gen 3).
- Fixed-point math will not overflow in typical 2-hour race durations.
- Server-authoritative netcode with client prediction provides esports-acceptable responsiveness (<50ms perceived latency).
- Content pipeline can deliver 15+ high-quality tracks within 18 months with a small team.

**Recommended first milestone**: M0 Foundation.  
Deliver a single-track, single-car demo running on PC, iOS, and Android simultaneously. Validate that the same deterministic input sequence produces identical `VehicleState` outputs on all three platforms within tolerance. If this gate fails, the architecture must be revised before gameplay development proceeds.

---

## Critical Question

**Can Overtake start as a small, achievable game and scale toward AAA mobile/PC/console production without requiring foundational rewrites?**

**Answer**: **Yes.**  
The strict separation of simulation and presentation, combined with a server-authoritative multiplayer model and versioned data formats, creates a foundation that is extensible but not fragile. Mobile reduces visual and optional simulation fidelity; PC/console scales it up. The core rules and race logic are written once. The largest ongoing risk is cross-platform determinism, which is mitigated by continuous CI validation and authoritative servers. Provided the team respects module boundaries and the phased milestone plan, foundational rewrites are unnecessary.
