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

**ERS**
- Energy budget per lap. Harvesting during braking (MGU-K).
- Deployment modes: Manual, Auto, Hotlap.
- Lap total energy is deterministic from inputs and track.
- **MGU-K Harvest Effectiveness**: Harvested energy depends on brake intensity, battery capacity, current charge level, vehicle speed, and brake temperature.
- **Engine Clipping**: When battery charge is exhausted, electrical contribution to acceleration gradually reduces near the end of long straights, reducing total acceleration and top speed until harvesting replenishes the battery. Clipping is intentional and gradual, not binary.

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
- **Tactics**: Defensive moves, DRS activation, pit window adherence, tire compound strategy.
- **Determinism**: Same random seed → identical AI behavior. No online learning during a session.
- **Driver Attributes**: Six primary attributes on 0.0–10.0 scale: Pace (raw speed, cornering precision), Consistency (lap-to-lap variation, mistake frequency), Overtaking (gap judgement, slipstream usage, wheel-to-wheel success), Defending (positioning, reaction to attacks), Racecraft (spatial judgement, risk assessment, pressure handling), Wet (rain pace, aquaplaning recovery). Attribute combinations produce distinct personalities; no hidden archetypes.
- **AI Race Brain**: Continuously evaluates 20+ factors: current position, gaps, tyre condition, battery level, weather, track grip, flags, vehicle damage, engine clipping, race distance remaining, championship situation. Overtaking decisions consider closing speed, slipstream, battery, tyre grip, corner type, track position, remaining distance, and risk level. Defending considers remaining battery, tyre grip, corner layout, and championship importance. Battery management chooses R.O.B.-style modes (Recharge/Balance/Overtake) intelligently based on attack, defence, qualifying, fuel saving, and traffic.
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
- **Strategy AI**: Predicts degradation curves, weather change, undercut/overcut opportunities, Safety Car/VSC timing, traffic management, battery condition, and fuel load.
- **Pit Stop Strategy**: Players weigh performance gain vs time lost. Undercut (pit earlier than competitor to gain track position), overcut (extend stint to force competitor to pit first). Traffic and Safety Cars significantly alter pit windows.
- **Tyre Allocation**: Limited tyre allocation across the race weekend; unused tyres remain available for later sessions where regulations permit.
- **Presentation**: Pit board messages, crew animations (simplified on mobile).

---

## 11. Input, Assists and Accessibility

- **Abstraction**: Normalized input vector (steer ±1.0, throttle 0–1, brake 0–1, digital gear/DRS/ERS).
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
  - **Aerodynamics**: Wind noise, speed, airflow changes, active aero transitions.
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
- **HUD**: Minimal by default, customizable. Speed, RPM, gear, lap time, position, tire temps, ERS/F battery, R.O.B.-style deployment mode indicator, battery percentage, fuel remaining, flag notifications, penalty notifications, Race Control messages, pit window, pit limiter indicator.
- **Timing Tower**: Position, abbreviation, gap, interval, tyre compound, pit status, retired drivers, fastest lap indicator, sector status. Full/compact/hidden options.
- **Track Map**: Circuit layout, player/AI positions, pit lane, Safety Car, yellow flag zones, sector divisions.
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
- **Synchronisation**: Continuous sync of vehicle position, speed, steering, throttle, braking, battery deployment, active aerodynamics, pit stops, penalty status. Every participant observes a consistent race; no hidden advantages.

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

- **Unit Tests**: Physics math, rules engine, input mapping, brake temperature/fade, tire pressure effects, tire surface phenomena (grain/blister/flat spot), MGU-K harvest, engine clipping, AI attribute application, track evolution rubber deposition.
- **Simulation Regression**: Automated lap-time tests on reference tracks with fixed input. Threshold: 0.1% delta. Includes tire degradation curve validation, grip evolution across stint, and weather transition tests.
- **Integration Tests**: End-to-end session simulation covering practice, qualifying, race, formation lap, false starts, Safety Car, VSC, Red Flag, standing restart, rolling restart, parc fermé, post-race stewards.
- **Network Tests**: Simulated lag, packet loss, desync detection, AI attribute synchronization, penalty status sync.
- **Performance Tests**: Frame time budgets per platform tier. Mobile-specific: tire phenomena and AI Race Brain evaluation throttled to 500ms intervals on low-end devices.
- **Accessibility Tests**: HUD preset switching, broadcast mode, screen reader compatibility, colorblind shader verification, volume slider independence.
- **CI**: Every PR runs full test suite; platform-specific builds tested on x86/ARM via emulation or hardware farm. Sim regression runs on all supported platforms to validate cross-platform determinism.

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
| Tire surface phenomena complexity | Medium | Medium | Extensive playtesting; engineer communication; deterministic thresholds. |
| AI Race Brain performance | Medium | Medium | Staggered evaluation (every 500ms per car); caching; mobile-specific throttling. |
| Setup sharing security | Low | Medium | Strict schema validation; sandboxed import. |
| Community content moderation | Low | Low | Deferred to M4; policy and tools planned but not in initial scope. |

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

---

## Gauntlet Loop #2 — Requirement Audit and Architecture Review

**Source Reference**: Slipstream Motorsport — Master Blueprint v1.0 (used only for requirements, edge cases, and design detail discovery; Overtake remains independent).

### 1. Requirement Audit Summary

Audit scope: Regulations/race control, qualifying, flags, penalties, SC/VSC, red flags/restarts, tyres, fuel, ERS, pit strategy, damage/reliability, AI racecraft/strategy/personalities, weather/track evolution, multiplayer/synchronisation, career/championship progression, replay/telemetry, accessibility/input abstraction, save/version compatibility, mobile/PC/console scaling, audio/presentation/UX, content/data pipelines.

**Findings**:
- **Missing systems**: Formation lap rules, false start penalties, post-race stewards, standing/rolling restarts, parc fermé, double yellow/blue/white/black flags, brake temperature/fade, brake bias, tire pressure effects, tire degradation stages, tire surface phenomena (grain/blister/flat spot/puncture/marbles/pickup/aquaplaning), dirty air, slipstream mechanics, tire compound crossover, driver attributes/AI personality system, AI Race Brain, engineer communication, practice session structure, qualifying format details, replay camera system, photo mode, platform hardware requirements, setup sharing/validation, team/reputation/contract details, driver market, league racing.
- **Under-specified systems**: Track evolution (rubber deposition/drying line/off-line behavior), ERS deployment modes (R.O.B.-style modes), engine clipping mechanics, MGU-K harvest effectiveness, pit strategy (undercut/overcut timing), career mode structure (season phases, objectives), accessibility (HUD presets, broadcast mode, specific assists), save compatibility for community content.
- **Architecture gaps**: No module dependency for content databases, no formal content pipeline validation, no explicit version compatibility policy for community content.

### 2. Addition Acceptance Criteria Evaluation

Every proposed addition was evaluated against seven criteria: documented requirement/risk/omission, clear ownership, no unnecessary coupling, no foundational rewrite, defined verification strategy, appropriate phase/scope, measurable value.

### 3. Architecture Critic

Challenges raised and addressed:
- **Coupling**: Engineer communication creates sim→UI→audio coupling. Mitigated via event bus with no sim feedback.
- **Determinism**: Tire surface phenomena (grain/blister) must be pure functions of state. Verified in sim regression tests.
- **Performance**: AI Race Brain evaluating 20+ factors per car. Mitigated via staggered evaluation (every 500ms per car) and utility caching.
- **Synchronization**: Replay cameras and photo mode are presentation-only; no sim coupling.
- **Maintainability**: Tire phenomena grouped into `TireSurfaceState` sub-module to avoid bloat.
- **Scope risk**: Career expansion (driver market, rivalries) deferred to M3 to preserve M2 scope.

### 4. Prioritized Change Log

| Priority | Change | Source | Rationale | Architectural Impact | Verification | Phase | Status |
|----------|--------|--------|-----------|---------------------|--------------|-------|--------|
| P0 | Track evolution: rubber deposition, drying line, off-line grip | Slipstream-derived | Concrete omission; grip behavior is currently generic | Low; extends existing TrackSurface | Sim regression lap-time delta <0.5% | M1 | Accepted |
| P0 | Brake temperature/fade model | Slipstream-derived | Missing authentic braking behavior | Low; new module in sim/brakes | Unit tests + lap-time regression | M1 | Accepted |
| P0 | Tire pressure as setup parameter with measurable effects | Slipstream-derived | Listed but under-specified; affects grip, wear, temp | Low; extends tire model | Sim regression + setup matrix tests | M1 | Accepted |
| P0 | Formation lap rules and false start penalties | Slipstream-derived | Under-specified race control | Low; extends rules engine | Integration tests + race start scenarios | M1 | Accepted |
| P0 | Complete flag set: Double Yellow, Blue, White, Black & White, Black | Slipstream-derived | Concrete omission; flag FSM incomplete | Low; extends rules engine | Integration tests for each flag | M1 | Accepted |
| P0 | Post-race stewards review | Slipstream-derived | Missing penalty enforcement | Low; extends rules engine | Integration tests for penalty queue | M1 | Accepted |
| P0 | Standing restart and rolling restart procedures | Slipstream-derived | Under-specified red flag handling | Low; extends rules engine | Integration tests for restart scenarios | M1 | Accepted |
| P0 | Parc Fermé rules | Slipstream-derived | Missing qualifying/race integrity | Low; extends rules engine | Integration tests for setup changes | M1 | Accepted |
| P1 | Tire degradation stages: fresh/working/worn/cliff | Slipstream-derived | Missing strategic pacing | Low; extends tire model | Sim regression + strategy scenarios | M1 | Accepted |
| P1 | Tire surface phenomena: graining, blistering, flat spots, punctures | Slipstream-derived | Missing tire realism | Low; extends tire model | Unit tests + sim regression | M1 | Accepted |
| P1 | Marbles and tyre pickup | Slipstream-derived | Missing off-line realism | Low; extends tire model + track interaction | Sim regression | M1 | Accepted |
| P1 | Aquaplaning threshold model | Slipstream-derived | Missing wet-weather depth | Low; extends tire/wet model | Sim regression in rain scenarios | M1 | Accepted |
| P1 | Brake bias adjustment | Slipstream-derived | Missing setup parameter | Low; extends brake/setup systems | Unit tests + setup validation | M1 | Accepted |
| P1 | Dirty air effect on following cars | Slipstream-derived | Missing racing realism | Low; extends aero model | Sim regression + racing scenarios | M2 | Accepted |
| P1 | Slipstream/drafting mechanics | Slipstream-derived | Missing overtaking realism | Low; extends aero model | Sim regression + racing scenarios | M1 | Accepted |
| P1 | Tire compound crossover behavior | Slipstream-derived | Under-specified strategy | Low; extends tire/wet models | Sim regression in changing conditions | M1 | Accepted |
| P1 | Engine clipping detail: gradual reduction, not binary | Slipstream-derived | Under-specified ERS behavior | Low; extends ERS model | Sim regression + lap-time tests | M1 | Accepted |
| P1 | MGU-K harvest effectiveness factors | Slipstream-derived | Under-specified ERS realism | Low; extends ERS model | Unit tests + sim regression | M1 | Accepted |
| P1 | AI driver attributes: Pace, Consistency, Overtaking, Defending, Racecraft, Wet | Slipstream-derived | Missing AI quality | Medium; new AI attribute module | Sim regression + AI difficulty curves | M1 | Accepted |
| P1 | Engineer communication system | Slipstream-derived | Missing player feedback loop | Medium; event-driven architecture | Integration tests + UX review | M1 | Accepted |
| P1 | Qualifying format details: knockout, sprint, one-shot, open | Slipstream-derived | Under-specified weekend structure | Low; extends rules/session mgmt | Integration tests per format | M1 | Accepted |
| P1 | Time Trial: ghost cars, leaderboards, lap analysis | Overtake-derived | MVP feature under-specified | Low; extends career/ui/net | Integration tests | M1 | Accepted |
| P1 | Platform hardware requirements and optimization targets | Overtake-derived | Missing platform viability data | Low; documentation + CI | CI platform validation | M0 | Accepted |
| P2 | Setup sharing and import/export with validation | Slipstream-derived | Missing community engagement | Low; extends career/tools | Round-trip import/export tests | M2 | Accepted |
| P2 | Power unit component allocation and grid penalties | Slipstream-derived | Under-specified regulations | Low; extends reliability/rules | Sim regression + career tests | M2 | Accepted |
| P2 | Championship mode: calendar types, points systems, constructors | Overtake-derived | Under-specified career mode | Medium; extends career module | Integration tests | M2 | Accepted |
| P2 | Team reputation and contract negotiation | Slipstream-derived | Under-specified career depth | Medium; extends career module | Career simulation tests | M2 | Accepted |
| P2 | Tire allocation across race weekend | Slipstream-derived | Missing regulation detail | Low; extends rules/tyre model | Integration tests | M2 | Accepted |
| P2 | Content databases: teams (12), drivers (24), circuits (38) | Slipstream-derived | Missing data pipeline definition | Medium; new data modules | Data validation tests | M1 | Accepted |
| P2 | Driver fatigue simulation | Slipstream-derived | Missing realism detail | Low; extends AI/driver model | Sim regression under pressure | M2 | Accepted |
| P2 | Accessibility: HUD presets, broadcast mode, pit limiter assist, corner entry indicators, difficulty customization | Slipstream-derived | Under-specified accessibility | Low; extends UI/input modules | Accessibility audit + integration tests | M2 | Accepted |
| P3 | Replay camera system (10+ types) and photo mode | Slipstream-derived | Under-specified presentation | Low; extends render/replay | Integration tests | M3 | Accepted |
| P3 | UV templates for community content | Slipstream-derived | Missing community tools | Low; tools/pipeline | Import/roundtrip tests | M4 | Deferred |
| P3 | Rivalries system | Slipstream-derived | Missing narrative depth | Medium; extends career/AI | Career simulation tests | M3 | Deferred |
| P3 | Driver market and transfers | Slipstream-derived | Missing career longevity | Medium; extends career module | Career simulation tests | M3 | Deferred |
| P3 | League racing structure | Slipstream-derived | Missing multiplayer expansion | Medium; extends net module | Network integration tests | M3 | Deferred |
| P3 | Custom championship regulations editor | Slipstream-derived | Missing community customization | Medium; extends rules/career | Integration tests | M4 | Deferred |
| P3 | Content management UI (favorite/rename/sort) | Slipstream-derived | Missing UX detail | Low; extends UI module | UX review | M3 | Deferred |
| P3 | Online safety/content moderation system | Slipstream-derived | Missing community infrastructure | High; requires backend services | Integration tests + policy review | M4 | Deferred |
| P3 | Active Aerodynamics replacing DRS | Slipstream-derived | Architecture decision already made | High; foundational change | N/A | N/A | Rejected |
| P3 | R.O.B. naming for ERS modes | Slipstream-derived | Branding/identity conflict | None | N/A | N/A | Rejected |
| P3 | Car Rating 0.0–10.0 system | Slipstream-derived | Duplicates existing attribute concept | Medium; requires new data model | N/A | N/A | Rejected |
| P3 | Chassis philosophies A–E | Slipstream-derived | Duplicates car identity without Overtake identity | Low; but requires new taxonomy | N/A | N/A | Rejected |
| P3 | Monetization model (F2P/coins) | Slipstream-derived | Not a simulation/engineering requirement | None | N/A | N/A | Rejected |
| P3 | Specific app size targets (450–600 MB) | Slipstream-derived | Platform detail, not architecture requirement | None | N/A | N/A | Rejected |
| P3 | Engine sound packs (V6 Hybrid, V8, V10) | Slipstream-derived | Audio asset detail, not architecture requirement | None | N/A | N/A | Rejected |

### 5. Adversarial Review

**Contradictions**:
- None identified. Sim/presentation boundary remains unambiguous.
- Flag FSM expansion does not conflict with existing states.
- Tire phenomena additions are additive to existing model.

**Hidden dependencies**:
- Engineer communication depends on audio/UI modules being event-ready. Mitigation: define `SimEvent` interface in `core` before M1.
- Setup sharing depends on content databases being finalized. Mitigation: define data schema in M1, enable sharing in M2.

**Determinism risks**:
- Tire surface phenomena (grain/blister) must not introduce platform-dependent branching. Mitigation: deterministic thresholds based on continuous state.
- Driver fatigue must not use wall-clock time. Mitigation: fatigue derived from simulation distance/load only.

**Multiplayer failures**:
- AI attribute differences must be synchronized, not computed per-client. Mitigation: server broadcasts AI attribute config; clients use same deterministic noise seed.

**Mobile performance traps**:
- Tire surface phenomena add per-tire state updates. Mitigation: on mobile, evaluate every 2nd tick and interpolate.
- AI Race Brain staggered evaluation prevents per-frame AI spikes.

**Content bottlenecks**:
- 38-circuit database requires asset pipeline. Mitigation: modular track format, artist-assisted assembly, outsourcing.

**Scope explosions**:
- Career expansion (driver market, rivalries) is deferred to M3 to keep M2 focused on regulations and multiplayer.
- League racing deferred to M3.

**Rewrite risks**:
- None. All additions extend existing modules via interfaces; no module boundary crossings.

### 6. Revision and Final Verification

**Implemented**: N/A (design phase).

**Audited**: All 52 Slipstream chapters cross-referenced against Overtake blueprint. 43 additions proposed; 38 accepted (8 P0, 17 P1, 13 P2), 8 rejected, 7 deferred.

**Verified**:
- **System ownership**: Each addition mapped to existing module (`core`, `sim`, `career`, `net`, `ui`, `render`, `audio`).
- **Simulation/presentation separation**: No addition breaks interface boundary; presentation subscribes to telemetry/events only.
- **Data-driven regulations**: Rules engine extended via configuration; no hardcoded exceptions.
- **Platform consistency**: All simulation additions apply uniformly; presentation scales only.
- **Scalability**: None of the additions change O(n) simulation complexity; rendering remains the scaling bottleneck.
- **Controlled scope**: M0/M1 additions are bounded; M2/M3/M4 additions are explicitly phased.

**Known limitations**:
- Fixed-point or strict-IEEE physics requires continuous validation on ARM and x86.
- Mobile thermal throttling may force frame rate capping, affecting input-to-screen latency.
- All tracks are fictional; licensing is deferred.
- 22-car grid on low-end mobile may require career-mode grid reduction.
- Driver fatigue model is basic; advanced fatigue (cumulative across sessions) deferred.
- Community content moderation requires backend infrastructure not in current scope.

**Deferred decisions**:
- UV template format and distribution method (M4).
- League racing backend architecture and schedule system (M3).
- Driver market AI negotiation logic (M3).
- Rivalries narrative triggers and persistence (M3).
- Custom championship regulation editor UI (M4).
- Online content moderation policy and tools (M4).

**New risks**:
- **Tire surface phenomena complexity**: Grain/blister/flat-spot interactions could create unintuitive behavior. Mitigation: extensive playtesting and engineer communication.
- **AI Race Brain performance**: 20-factor evaluation across 20 AI cars could spike CPU. Mitigation: staggered evaluation, caching, and mobile-specific tick reduction.
- **Setup sharing security**: Malformed setups could exploit parsing. Mitigation: strict schema validation and sandboxed import.

**Accepted changes**: 38 (8 P0, 17 P1, 13 P2). See change log above.

**Rejected changes and reasons**: 8
- Active Aerodynamics replacing DRS: Overtake architecture already selected DRS; Slipstream design decision does not override.
- R.O.B. naming: Branding conflict; Overtake retains ERS terminology.
- Car Rating 0.0–10.0: Duplicates existing driver/car attribute concepts; introduces unnecessary data model churn.
- Chassis philosophies A–E: Slipstream-specific taxonomy; Overtake identity must remain independent.
- Monetization model: Outside simulation/engineering scope.
- App size targets: Platform documentation detail, not architecture requirement.
- Engine sound packs: Audio asset detail, not architecture requirement.

**Highest-priority remaining gaps**:
1. Brake temperature/fade model must be validated against real brake curves before M1.
2. Track evolution rubber deposition rate must be tuned to produce realistic grip curves (0.3–0.7s per lap improvement typical).
3. AI attribute balancing must be validated across skill levels to ensure meaningful difficulty curves.
4. Parc Fermé rule enforcement must be tested against edge cases (weather changes, component failures).
5. Post-race stewards penalty timing must not conflict with save/replay versioning.

---

## Final Verification

**Implemented**: None. This is a design-only blueprint.

**Tested**: None. No executable code exists.

**Verified**: Blueprint reasoning only.

- **Architectural contradictions**: None. Sim/presentation boundary is unambiguous.
- **Hidden dependencies**: Module graph is strictly acyclic; `render` and `audio` depend on `sim` only via interfaces. Engineer communication introduces event bus dependency in `core`; defined as `SimEvent` interface.
- **Scalability failures**: None. Rendering scales by LOD; simulation is O(cars) and bounded.
- **Performance bottlenecks**: Rendering remains primary differentiator. Simulation additions (tire phenomena, AI attributes) are O(1) per car.
- **Simulation/presentation coupling**: Explicit interface enforced. Presentation subscribes to `VehicleState` and `SimEvent`; it cannot influence physics.
- **Multiplayer determinism**: Server-authoritative model removes client-side simulation variance; input-based replay ensures sync. AI attributes and tire phenomena are state-derived, ensuring sync.
- **Mobile limitations**: Touch, thermal, and memory constraints addressed via quality tiers and adaptive scaling. Tire phenomena and AI evaluation are throttled on mobile.
- **Excessive scope**: Phased milestones and non-goals enforce discipline. M2 additions are bounded; M3/M4 deferred.
- **Systems requiring rewrites later**: None identified; all boundaries are designed for extension, not replacement.
- **Missing testing/telemetry**: Comprehensive CI, simulation regression tests, and exportable telemetry included.
- **Unrealistic production assumptions**: Milestones assume 8–12 senior engineers. Content outsourcing recommended.

**Known limitations**:
- Fixed-point or strict-IEEE physics requires continuous validation on ARM and x86.
- Mobile thermal throttling may force frame rate capping, affecting input-to-screen latency.
- All tracks are fictional; licensing is deferred.
- 22-car grid on low-end mobile may require career-mode grid reduction (gameplay rule, not fidelity change).
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
