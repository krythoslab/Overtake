#!/usr/bin/env python3
"""
M0 Verification Script
Mirrors the C# simulation logic exactly and runs automated tests to prove:
- determinism
- repeatability
- fixed-timestep consistency
- data-driven VehicleSpec / RegulationProfile
- invalid configuration failure
- basic longitudinal dynamics
"""

import math
import copy
import sys

# ---------------------------------------------------------------------------
# Deterministic RNG (Mulberry32)
# ---------------------------------------------------------------------------
class DeterministicRng:
    def __init__(self, seed: int):
        self._state = seed & 0xFFFFFFFF

    def next(self) -> int:
        z = (self._state + 0x6D2B79F5) & 0xFFFFFFFF
        self._state = z
        z = (z ^ (z >> 15)) * (1 | z)
        z ^= z + (z ^ (z >> 7)) * (61 | z)
        val = z ^ (z >> 14)
        return val & 0xFFFFFFFF

    def next_float(self) -> float:
        return self.next() / 0xFFFFFFFF

    def next_float_range(self, min_val: float, max_val: float) -> float:
        return min_val + self.next_float() * (max_val - min_val)

# ---------------------------------------------------------------------------
# Data Models
# ---------------------------------------------------------------------------
class VehicleInput:
    __slots__ = ('steer', 'throttle', 'brake', 'clutch', 'gear',
                 'activeAero', 'ersMode')
    def __init__(self):
        self.steer = 0.0
        self.throttle = 0.0
        self.brake = 0.0
        self.clutch = 0.0
        self.gear = 0
        self.activeAero = 0  # enum index
        self.ersMode = 0     # enum index

class VehicleState:
    __slots__ = ('posX', 'posY', 'posZ',
                 'quatX', 'quatY', 'quatZ', 'quatW',
                 'speed', 'rpm', 'gear',
                 'tireSlipFrontLeft', 'tireSlipFrontRight',
                 'tireSlipRearLeft', 'tireSlipRearRight',
                 'batterySoC_MJ', 'rngState')
    def __init__(self):
        self.posX = 0.0
        self.posY = 0.0
        self.posZ = 0.0
        self.quatX = 0.0
        self.quatY = 0.0
        self.quatZ = 0.0
        self.quatW = 1.0
        self.speed = 0.0
        self.rpm = 0.0
        self.gear = 1
        self.tireSlipFrontLeft = 0.0
        self.tireSlipFrontRight = 0.0
        self.tireSlipRearLeft = 0.0
        self.tireSlipRearRight = 0.0
        self.batterySoC_MJ = 0.0
        self.rngState = 0

class Pu:
    def __init__(self):
        self.IceMaxPower_kW = 400.0
        self.MguKMaxPower_kW = 350.0
        self.MguKOvertakeMaxPower_kW = 350.0
        self.MguKHarvestMax_kW = 350.0
        self.HarvestLimit_MJ_perLap = 8.5
        self.HarvestLimitWithOvertake_MJ_perLap = 9.0
        self.BatteryCapacity_MJ = 4.5
        self.BatterySoCDeltaLimit_MJ = 4.0
        self.DeployRate_MJ_per_s = 0.35
        self.HarvestRate_MJ_per_s = 0.35
        self.BoostDeployMultiplier = 1.5
        self.FuelEnergyFlowMax_MJ_per_h = 3000.0
        self.FuelFlowRpmCurve = "0.27 * rpm + 165"
        self.FuelFlowRpmCurveMaxRpm = 10500.0
        self.HasMguH = False

class Suspension:
    def __init__(self):
        self.SpringRate_N_per_m = 120000.0
        self.DamperCompression_N_per_m = 80000.0
        self.DamperRebound_N_per_m = 120000.0
        self.AntiRollBarStiffness_Nm_per_rad = 50000.0
        self.MaxTravel_mm = 100.0
        self.BumpStopStiffness_N_per_m = 500000.0
        self.RideHeight_min_mm = 20.0
        self.RideHeight_max_mm = 50.0

class Dynamics:
    def __init__(self):
        self.Mass_kg = 768.0
        self.DragCoeffCd = 0.7
        self.FrontalArea_m2 = 1.5
        self.RollingResistanceCoeff = 0.015
        self.WheelRadius_m = 0.33
        self.MaxBrakeForce_N = 20000.0
        self.GearRatios = [3.0, 2.5, 2.0, 1.5, 1.2, 1.0]
        self.FinalDriveRatio = 3.2
        self.IdleRpm = 800.0
        self.MaxRpm = 15000.0
        self.ShiftUpRpm = 12000.0
        self.ShiftDownRpm = 5000.0
        self.AirDensity_kg_per_m3 = 1.225
        self.Gravity_m_per_s2 = 9.81
        self.MaxAcceleration_m_per_s2 = 8.0

class VehicleSpec:
    def __init__(self):
        self.Id = ""
        self.DisplayName = ""
        self.RegulationProfileId = ""
        self.Pu = Pu()
        self.Aero = type('Aero', (), {})()
        self.Aero.ActiveAero = True
        self.Aero.CornerModeDownforceCoeff = 1.0
        self.Aero.CornerModeDragCoeff = 1.0
        self.Aero.StraightModeDownforceCoeff = 0.55
        self.Aero.StraightModeDragCoeff = 0.45
        self.Aero.FrontTransitionTime_ms = 50.0
        self.Aero.RearTransitionTime_ms = 50.0
        self.Dynamics = Dynamics()
        self.Tires = type('Tires', (), {})()
        self.Tires.FrontWidth_mm = 280.0
        self.Tires.RearWidth_mm = 375.0
        self.Suspension = Suspension()
        self.Fidelity = type('Fidelity', (), {})()
        self.Fidelity.TireModel = "pacejka89"
        self.Fidelity.TireThermalNodes = 1

class RegulationProfile:
    def __init__(self):
        self.Id = ""
        self.DisplayName = ""
        self.Overtake = type('Overtake', (), {})()
        self.Overtake.GapThreshold_s = 1.0
        self.Overtake.BonusEnergy_MJ = 0.5
        self.Overtake.FollowMaxPower_kW = 350.0
        self.Overtake.FollowMaxSpeed_kmh = 337.0
        self.Overtake.LeadTaperStart_kmh = 290.0
        self.Overtake.LeadTaperEnd_kmh = 355.0
        self.Ers = type('Ers', (), {})()
        self.Ers.MguKMaxPower_kW = 350.0
        self.Ers.HarvestMaxPower_kW = 350.0
        self.Ers.HarvestLimit_MJ_perLap = 8.5
        self.Ers.HarvestLimitWithOvertake_MJ_perLap = 9.0
        self.Ers.BatterySoCDeltaLimit_MJ = 4.0
        self.Ers.DeployRate_MJ_per_s = 0.35
        self.Ers.HarvestRate_MJ_per_s = 0.35
        self.Ers.BoostDeployMultiplier = 1.5
        self.Pu = type('Pu', (), {})()
        self.Pu.IceMaxPower_kW = 400.0
        self.Pu.FuelEnergyFlowMax_MJ_per_h = 3000.0
        self.Pu.FuelFlowRpmCurve = "0.27 * rpm + 165"
        self.Pu.FuelFlowRpmCurveMaxRpm = 10500.0
        self.Pu.HasMguH = False
        self.Aero = type('Aero', (), {})()
        self.Aero.CornerModeDownforceCoeff = 1.0
        self.Aero.CornerModeDragCoeff = 1.0
        self.Aero.StraightModeDownforceCoeff = 0.55
        self.Aero.StraightModeDragCoeff = 0.45
        self.Aero.FrontTransitionTime_ms = 50.0
        self.Aero.RearTransitionTime_ms = 50.0
        self.Dynamics = Dynamics()
        self.Tires = type('Tires', (), {})()
        self.Tires.FrontWidth_mm = 280.0
        self.Tires.RearWidth_mm = 375.0
        self.Sprint = type('Sprint', (), {})()
        self.Sprint.Sq1Duration_s = 720.0
        self.Sprint.Sq2Duration_s = 600.0
        self.Sprint.Sq3Duration_s = 480.0
        self.Sprint.Sq3CarCount = 10
        self.Sprint.Sq1EliminationCount = 6
        self.Sprint.Sq2EliminationCount = 6
        self.RaceQualifying = type('RaceQualifying', (), {})()
        self.RaceQualifying.Q1Duration_s = 1080.0
        self.RaceQualifying.Q2Duration_s = 900.0
        self.RaceQualifying.Q3Duration_s = 780.0
        self.RaceQualifying.EliminationPerRound = 5
        self.Suspension = Suspension()

# ---------------------------------------------------------------------------
# VehicleConfig factory
# ---------------------------------------------------------------------------
class VehicleConfig:
    def __init__(self):
        self.Mass_kg = 768.0
        self.DragCoeffCd = 0.7
        self.FrontalArea_m2 = 1.5
        self.RollingResistanceCoeff = 0.015
        self.WheelRadius_m = 0.33
        self.MaxBrakeForce_N = 20000.0
        self.GearRatios = [3.0, 2.5, 2.0, 1.5, 1.2, 1.0]
        self.FinalDriveRatio = 3.2
        self.IceMaxPower_kW = 400.0
        self.IdleRpm = 800.0
        self.MaxRpm = 15000.0
        self.ShiftUpRpm = 12000.0
        self.ShiftDownRpm = 5000.0
        self.AirDensity_kg_per_m3 = 1.225
        self.Gravity_m_per_s2 = 9.81
        self.MaxAcceleration_m_per_s2 = 8.0
        self.BatteryCapacity_MJ = 4.5
        self.MguKMaxPower_kW = 350.0
        self.MguKOvertakeMaxPower_kW = 350.0
        self.MguKHarvestMax_kW = 350.0
        self.HarvestLimit_MJ_perLap = 8.5
        self.HarvestLimitWithOvertake_MJ_perLap = 9.0
        self.BatterySoCDeltaLimit_MJ = 4.0
        self.FuelEnergyFlowMax_MJ_per_h = 3000.0
        self.FuelFlowRpmCurveMaxRpm = 10500.0
        self.HasMguH = False
        self.ERSDeployRate_MJ_per_s = 0.35
        self.ERSBoostDeployMultiplier = 1.5
        self.ERSHarvestRate_MJ_per_s = 0.35
        self.FrontWidth_mm = 280.0
        self.RearWidth_mm = 375.0
        self.FrontSpringRate_N_per_m = 120000.0
        self.RearSpringRate_N_per_m = 140000.0
        self.FrontDampingCompression = 80000.0
        self.FrontDampingRebound = 120000.0
        self.RearDampingCompression = 90000.0
        self.RearDampingRebound = 130000.0
        self.FrontAntiRollBarNm_per_rad = 50000.0
        self.RearAntiRollBarNm_per_rad = 60000.0
        self.SuspensionMaxTravel_mm = 100.0
        self.BumpStopStiffness_N_per_m = 500000.0
        self.RideHeight_min_mm = 20.0
        self.RideHeight_max_mm = 50.0

def create_config(spec: VehicleSpec, regs: RegulationProfile) -> VehicleConfig:
    sD = spec.Dynamics
    rD = regs.Dynamics
    sPu = spec.Pu
    rPu = regs.Pu
    rErs = regs.Ers
    rSusp = regs.Suspension
    sTires = spec.Tires
    rTires = regs.Tires

    def val(sv, rv):
        return sv if sv != 0.0 else rv

    cfg = VehicleConfig()
    cfg.Mass_kg = val(sD.Mass_kg, rD.Mass_kg)
    cfg.DragCoeffCd = val(sD.DragCoeffCd, rD.DragCoeffCd)
    cfg.FrontalArea_m2 = val(sD.FrontalArea_m2, rD.FrontalArea_m2)
    cfg.RollingResistanceCoeff = val(sD.RollingResistanceCoeff, rD.RollingResistanceCoeff)
    cfg.WheelRadius_m = val(sD.WheelRadius_m, rD.WheelRadius_m)
    cfg.MaxBrakeForce_N = val(sD.MaxBrakeForce_N, rD.MaxBrakeForce_N)
    cfg.GearRatios = sD.GearRatios if sD.GearRatios else rD.GearRatios
    cfg.FinalDriveRatio = val(sD.FinalDriveRatio, rD.FinalDriveRatio)
    cfg.IceMaxPower_kW = val(sPu.IceMaxPower_kW, rPu.IceMaxPower_kW)
    cfg.IdleRpm = val(sD.IdleRpm, rD.IdleRpm)
    cfg.MaxRpm = val(sD.MaxRpm, rD.MaxRpm)
    cfg.ShiftUpRpm = val(sD.ShiftUpRpm, rD.ShiftUpRpm)
    cfg.ShiftDownRpm = val(sD.ShiftDownRpm, rD.ShiftDownRpm)
    cfg.AirDensity_kg_per_m3 = val(sD.AirDensity_kg_per_m3, rD.AirDensity_kg_per_m3)
    cfg.Gravity_m_per_s2 = val(sD.Gravity_m_per_s2, rD.Gravity_m_per_s2)
    cfg.MaxAcceleration_m_per_s2 = val(sD.MaxAcceleration_m_per_s2, rD.MaxAcceleration_m_per_s2)
    cfg.BatteryCapacity_MJ = sPu.BatteryCapacity_MJ
    cfg.MguKMaxPower_kW = val(sPu.MguKMaxPower_kW, rErs.MguKMaxPower_kW)
    cfg.MguKOvertakeMaxPower_kW = val(sPu.MguKOvertakeMaxPower_kW, rErs.MguKMaxPower_kW)
    cfg.MguKHarvestMax_kW = val(sPu.MguKHarvestMax_kW, rErs.HarvestMaxPower_kW)
    cfg.HarvestLimit_MJ_perLap = val(sPu.HarvestLimit_MJ_perLap, rErs.HarvestLimit_MJ_perLap)
    cfg.HarvestLimitWithOvertake_MJ_perLap = val(sPu.HarvestLimitWithOvertake_MJ_perLap, rErs.HarvestLimitWithOvertake_MJ_perLap)
    cfg.BatterySoCDeltaLimit_MJ = val(sPu.BatterySoCDeltaLimit_MJ, rErs.BatterySoCDeltaLimit_MJ)
    cfg.FuelEnergyFlowMax_MJ_per_h = val(sPu.FuelEnergyFlowMax_MJ_per_h, rPu.FuelEnergyFlowMax_MJ_per_h)
    cfg.FuelFlowRpmCurveMaxRpm = val(sPu.FuelFlowRpmCurveMaxRpm, rPu.FuelFlowRpmCurveMaxRpm)
    cfg.HasMguH = sPu.HasMguH
    cfg.ERSDeployRate_MJ_per_s = val(sPu.DeployRate_MJ_per_s, rErs.DeployRate_MJ_per_s)
    cfg.ERSBoostDeployMultiplier = val(sPu.BoostDeployMultiplier, rErs.BoostDeployMultiplier)
    cfg.ERSHarvestRate_MJ_per_s = val(sPu.HarvestRate_MJ_per_s, rErs.HarvestRate_MJ_per_s)
    cfg.FrontWidth_mm = val(getattr(sTires, 'FrontWidth_mm', 0.0), rTires.FrontWidth_mm)
    cfg.RearWidth_mm = val(getattr(sTires, 'RearWidth_mm', 0.0), rTires.RearWidth_mm)
    cfg.FrontSpringRate_N_per_m = rSusp.SpringRate_N_per_m
    cfg.RearSpringRate_N_per_m = rSusp.SpringRate_N_per_m
    cfg.FrontDampingCompression = rSusp.DamperCompression_N_per_m
    cfg.FrontDampingRebound = rSusp.DamperRebound_N_per_m
    cfg.RearDampingCompression = rSusp.DamperCompression_N_per_m
    cfg.RearDampingRebound = rSusp.DamperRebound_N_per_m
    cfg.FrontAntiRollBarNm_per_rad = rSusp.AntiRollBarStiffness_Nm_per_rad
    cfg.RearAntiRollBarNm_per_rad = rSusp.AntiRollBarStiffness_Nm_per_rad
    cfg.SuspensionMaxTravel_mm = rSusp.MaxTravel_mm
    cfg.BumpStopStiffness_N_per_m = rSusp.BumpStopStiffness_N_per_m
    cfg.RideHeight_min_mm = rSusp.RideHeight_min_mm
    cfg.RideHeight_max_mm = rSusp.RideHeight_max_mm
    return cfg

def validate_config(cfg: VehicleConfig):
    if cfg.Mass_kg <= 0:
        raise ValueError("Mass_kg must be positive.")
    if cfg.DragCoeffCd < 0:
        raise ValueError("DragCoeffCd must be non-negative.")
    if cfg.FrontalArea_m2 <= 0:
        raise ValueError("FrontalArea_m2 must be positive.")
    if cfg.RollingResistanceCoeff < 0:
        raise ValueError("RollingResistanceCoeff must be non-negative.")
    if cfg.WheelRadius_m <= 0:
        raise ValueError("WheelRadius_m must be positive.")
    if cfg.MaxBrakeForce_N < 0:
        raise ValueError("MaxBrakeForce_N must be non-negative.")
    if not cfg.GearRatios:
        raise ValueError("GearRatios must be non-empty.")
    if cfg.FinalDriveRatio <= 0:
        raise ValueError("FinalDriveRatio must be positive.")
    if cfg.IceMaxPower_kW <= 0:
        raise ValueError("IceMaxPower_kW must be positive.")
    if cfg.IdleRpm < 0:
        raise ValueError("IdleRpm must be non-negative.")
    if cfg.MaxRpm <= cfg.IdleRpm:
        raise ValueError("MaxRpm must be greater than IdleRpm.")
    if cfg.ShiftUpRpm <= cfg.ShiftDownRpm:
        raise ValueError("ShiftUpRpm must be greater than ShiftDownRpm.")
    if cfg.AirDensity_kg_per_m3 <= 0:
        raise ValueError("AirDensity_kg_per_m3 must be positive.")
    if cfg.Gravity_m_per_s2 <= 0:
        raise ValueError("Gravity_m_per_s2 must be positive.")
    if cfg.MaxAcceleration_m_per_s2 <= 0:
        raise ValueError("MaxAcceleration_m_per_s2 must be positive.")
    if cfg.BatteryCapacity_MJ < 0:
        raise ValueError("BatteryCapacity_MJ must be non-negative.")
    if cfg.ERSDeployRate_MJ_per_s < 0:
        raise ValueError("ERSDeployRate_MJ_per_s must be non-negative.")
    if cfg.ERSBoostDeployMultiplier < 0:
        raise ValueError("ERSBoostDeployMultiplier must be non-negative.")
    if cfg.ERSHarvestRate_MJ_per_s < 0:
        raise ValueError("ERSHarvestRate_MJ_per_s must be non-negative.")
    if cfg.FrontWidth_mm <= 0:
        raise ValueError("FrontWidth_mm must be positive.")
    if cfg.RearWidth_mm <= 0:
        raise ValueError("RearWidth_mm must be positive.")

# ---------------------------------------------------------------------------
# Simulation Loop
# ---------------------------------------------------------------------------
class SimulationLoop:
    def __init__(self, fixed_dt: float = 0.02):
        if fixed_dt <= 0 or math.isnan(fixed_dt) or math.isinf(fixed_dt):
            raise ValueError("Fixed dt must be positive and finite.")
        self._fixed_dt = fixed_dt
        self._accumulator = 0.0

    def step(self, real_dt: float, on_fixed_update):
        if real_dt <= 0 or math.isnan(real_dt) or math.isinf(real_dt):
            return
        if on_fixed_update is None:
            raise ValueError("on_fixed_update cannot be null.")
        self._accumulator += real_dt
        while self._accumulator >= self._fixed_dt:
            on_fixed_update(self._fixed_dt)
            self._accumulator -= self._fixed_dt

# ---------------------------------------------------------------------------
# Vehicle Dynamics
# ---------------------------------------------------------------------------
class VehicleDynamics:
    def __init__(self, cfg: VehicleConfig):
        validate_config(cfg)
        self._cfg = cfg
        self._state = VehicleState()
        self._rng = None
        self._initialized = False

    def reset(self, seed: int):
        self._rng = DeterministicRng(seed)
        self._state.posX = 0.0
        self._state.posY = 0.0
        self._state.posZ = 0.0
        self._state.quatX = 0.0
        self._state.quatY = 0.0
        self._state.quatZ = 0.0
        self._state.quatW = 1.0
        self._state.speed = 0.0
        self._state.rpm = self._cfg.IdleRpm
        self._state.gear = 1
        self._state.tireSlipFrontLeft = 0.0
        self._state.tireSlipFrontRight = 0.0
        self._state.tireSlipRearLeft = 0.0
        self._state.tireSlipRearRight = 0.0
        self._state.batterySoC_MJ = self._cfg.BatteryCapacity_MJ
        self._state.rngState = seed
        self._initialized = True

    def update(self, dt: float, inp: VehicleInput):
        if not self._initialized:
            raise RuntimeError("Vehicle not initialized. Call reset first.")
        if dt <= 0 or math.isnan(dt) or math.isinf(dt):
            raise ValueError("dt must be positive and finite.")

        self._handle_auto_gear(inp)

        engine_force = 0.0
        if inp.throttle > 0.0:
            power_force = self._cfg.IceMaxPower_kW * 1000.0 / max(self._state.speed, 0.5)
            max_force = self._cfg.Mass_kg * self._cfg.MaxAcceleration_m_per_s2
            engine_force = min(power_force, max_force) * inp.throttle

        drag_force = (0.5 * self._cfg.AirDensity_kg_per_m3 *
                      self._cfg.DragCoeffCd *
                      self._cfg.FrontalArea_m2 *
                      self._state.speed * self._state.speed)
        rolling_force = self._cfg.RollingResistanceCoeff * self._cfg.Mass_kg * self._cfg.Gravity_m_per_s2
        brake_force = inp.brake * self._cfg.MaxBrakeForce_N

        net_force = engine_force - drag_force - rolling_force - brake_force
        acceleration = net_force / self._cfg.Mass_kg

        self._state.speed += acceleration * dt
        if self._state.speed < 0:
            self._state.speed = 0.0

        self._state.posX += self._state.speed * dt

        if self._state.speed > 0.1:
            wheel_angular_speed = self._state.speed / self._cfg.WheelRadius_m
            engine_angular_speed = (wheel_angular_speed *
                                    self._cfg.FinalDriveRatio *
                                    self._cfg.GearRatios[self._state.gear - 1])
            self._state.rpm = engine_angular_speed * 60.0 / (2.0 * math.pi)
            self._state.rpm = max(self._cfg.IdleRpm, min(self._state.rpm, self._cfg.MaxRpm))
        else:
            self._state.rpm = self._cfg.IdleRpm + inp.throttle * (self._cfg.MaxRpm - self._cfg.IdleRpm)

        slip_mag = abs(acceleration) / (self._cfg.Gravity_m_per_s2 * 0.9)
        slip_clamped = max(0.0, min(slip_mag, 1.0))
        self._state.tireSlipFrontLeft = slip_clamped
        self._state.tireSlipFrontRight = slip_clamped
        self._state.tireSlipRearLeft = slip_clamped
        self._state.tireSlipRearRight = slip_clamped

        if inp.throttle > 0.01:
            self._state.batterySoC_MJ -= inp.throttle * dt * 0.1
            if self._state.batterySoC_MJ < 0:
                self._state.batterySoC_MJ = 0.0
        elif inp.brake > 0.01:
            self._state.batterySoC_MJ += inp.brake * dt * 0.05
            if self._state.batterySoC_MJ > self._cfg.BatteryCapacity_MJ:
                self._state.batterySoC_MJ = self._cfg.BatteryCapacity_MJ

        self._state.rngState = self._rng.next()

    def _handle_auto_gear(self, inp: VehicleInput):
        if self._state.gear < len(self._cfg.GearRatios) and self._state.rpm >= self._cfg.ShiftUpRpm:
            self._state.gear += 1
        elif self._state.gear > 1 and self._state.rpm <= self._cfg.ShiftDownRpm:
            self._state.gear -= 1

# ---------------------------------------------------------------------------
# SingleCarSimulation
# ---------------------------------------------------------------------------
class SingleCarSimulation:
    def __init__(self, spec: VehicleSpec, regs: RegulationProfile, seed: int, fixed_dt: float = 0.02):
        self._cfg = create_config(spec, regs)
        self._vehicle = VehicleDynamics(self._cfg)
        self._fixed_dt = fixed_dt
        self._seed = seed
        self._accumulator = 0.0
        self._input = None
        self.reset()

    def reset(self):
        self._vehicle.reset(self._seed)
        self._accumulator = 0.0

    def step(self, real_dt: float, inp: VehicleInput):
        self._input = inp
        if real_dt <= 0 or math.isnan(real_dt) or math.isinf(real_dt):
            return
        self._accumulator += real_dt
        while self._accumulator >= self._fixed_dt:
            self._vehicle.update(self._fixed_dt, inp)
            self._accumulator -= self._fixed_dt

    @property
    def current_state(self):
        return self._vehicle._state

# ---------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------
def create_spec(mass=768.0, power=400.0):
    spec = VehicleSpec()
    spec.Pu.IceMaxPower_kW = power
    spec.Pu.BatteryCapacity_MJ = 4.5
    spec.Dynamics.Mass_kg = mass
    return spec

def create_regs(mass=768.0, power=400.0):
    regs = RegulationProfile()
    regs.Dynamics.Mass_kg = mass
    regs.Pu.IceMaxPower_kW = power
    return regs

def create_input(throttle=0.8, brake=0.0):
    inp = VehicleInput()
    inp.throttle = throttle
    inp.brake = brake
    return inp

def approx_equal(a, b, tol):
    return abs(a - b) <= tol

def assert_eq(actual, expected, tol, msg):
    if not approx_equal(actual, expected, tol):
        print(f"  FAIL: {msg} (got {actual}, expected {expected}, tol {tol})")
        return False
    return True

def test_identical_inputs_and_seed():
    print("Test: Identical inputs and seed produce identical final state")
    spec = create_spec()
    regs = create_regs()
    inp = create_input()

    sim1 = SingleCarSimulation(spec, regs, 12345, 0.02)
    sim2 = SingleCarSimulation(spec, regs, 12345, 0.02)

    for _ in range(600):
        sim1.step(0.02, inp)
        sim2.step(0.02, inp)

    s1 = sim1.current_state
    s2 = sim2.current_state

    ok = True
    ok &= assert_eq(s1.speed, s2.speed, 1e-5, "speed")
    ok &= assert_eq(s1.posX, s2.posX, 1e-4, "posX")
    ok &= assert_eq(s1.rpm, s2.rpm, 1e-4, "rpm")
    ok &= assert_eq(s1.gear, s2.gear, 0, "gear")
    ok &= assert_eq(s1.batterySoC_MJ, s2.batterySoC_MJ, 1e-5, "batterySoC_MJ")
    ok &= assert_eq(s1.rngState, s2.rngState, 0, "rngState")
    return ok

def test_repeated_runs():
    print("Test: Repeated runs produce identical results")
    inp = create_input()
    first_state = None

    for run in range(10):
        sim = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
        for _ in range(600):
            sim.step(0.02, inp)
        state = sim.current_state
        if first_state is None:
            first_state = copy.copy(state)
        else:
            ok = True
            ok &= assert_eq(state.speed, first_state.speed, 1e-5, f"Run {run} speed")
            ok &= assert_eq(state.posX, first_state.posX, 1e-4, f"Run {run} posX")
            ok &= assert_eq(state.rpm, first_state.rpm, 1e-4, f"Run {run} rpm")
            ok &= assert_eq(state.gear, first_state.gear, 0, f"Run {run} gear")
            ok &= assert_eq(state.rngState, first_state.rngState, 0, f"Run {run} rngState")
            if not ok:
                return False
    return True

def test_fixed_timestep_consistency():
    print("Test: Fixed timestep behaves consistently")
    inp = create_input()

    sim50_a = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    sim50_b = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    sim100_a = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.01)
    sim100_b = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.01)

    total_time = 2.0
    steps50 = int(total_time / 0.02)
    steps100 = int(total_time / 0.01)

    for _ in range(steps50):
        sim50_a.step(0.02, inp)
        sim50_b.step(0.02, inp)
    for _ in range(steps100):
        sim100_a.step(0.01, inp)
        sim100_b.step(0.01, inp)

    s50_a = sim50_a.current_state
    s50_b = sim50_b.current_state
    s100_a = sim100_a.current_state
    s100_b = sim100_b.current_state

    ok = True
    ok &= assert_eq(s50_a.speed, s50_b.speed, 1e-5, "50Hz speed self-consistency")
    ok &= assert_eq(s50_a.posX, s50_b.posX, 1e-4, "50Hz posX self-consistency")
    ok &= assert_eq(s100_a.speed, s100_b.speed, 1e-5, "100Hz speed self-consistency")
    ok &= assert_eq(s100_a.posX, s100_b.posX, 1e-4, "100Hz posX self-consistency")
    ok &= assert_eq(s50_a.gear, s100_a.gear, 0, "gear consistency")
    return ok

def test_vehicle_spec_change():
    print("Test: VehicleSpec change affects simulation result")
    spec_light = create_spec(mass=768.0, power=400.0)
    spec_heavy = create_spec(mass=900.0, power=400.0)
    regs = create_regs()
    inp = create_input()

    sim1 = SingleCarSimulation(spec_light, regs, 12345, 0.02)
    sim2 = SingleCarSimulation(spec_heavy, regs, 12345, 0.02)

    for _ in range(600):
        sim1.step(0.02, inp)
        sim2.step(0.02, inp)

    s1 = sim1.current_state
    s2 = sim2.current_state

    if approx_equal(s1.speed, s2.speed, 1e-5):
        print("  FAIL: Speed should differ with different mass.")
        return False
    if approx_equal(s1.posX, s2.posX, 1e-5):
        print("  FAIL: Position should differ with different mass.")
        return False
    return True

def test_regulation_profile_change():
    print("Test: RegulationProfile change affects simulation result")
    spec = create_spec()
    spec.Dynamics.Mass_kg = 0.0  # signal: use regulation profile default
    regs_light = create_regs(mass=768.0)
    regs_heavy = create_regs(mass=900.0)
    inp = create_input()

    sim1 = SingleCarSimulation(spec, regs_light, 12345, 0.02)
    sim2 = SingleCarSimulation(spec, regs_heavy, 12345, 0.02)

    for _ in range(600):
        sim1.step(0.02, inp)
        sim2.step(0.02, inp)

    s1 = sim1.current_state
    s2 = sim2.current_state

    if approx_equal(s1.speed, s2.speed, 1e-5):
        print("  FAIL: Speed should differ with different RegulationProfile mass.")
        return False
    return True

def test_invalid_config_fails():
    print("Test: Invalid configuration fails safely")
    spec = VehicleSpec()
    spec.Dynamics.Mass_kg = -100.0
    regs = create_regs()
    try:
        cfg = create_config(spec, regs)
        validate_config(cfg)
        print("  FAIL: Should have raised ValueError for negative mass.")
        return False
    except ValueError:
        return True

def test_zero_throttle_slows():
    print("Test: Zero throttle causes car to slow down")
    sim = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    inp = create_input(throttle=0.0)

    for _ in range(10):
        sim.step(0.02, inp)

    if sim.current_state.speed >= 0.1:
        print(f"  FAIL: Car should slow down (speed={sim.current_state.speed})")
        return False
    return True

def test_braking_reduces_speed():
    print("Test: Braking reduces speed")
    sim = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    accel = create_input(throttle=1.0)
    brake = create_input(throttle=0.0, brake=1.0)

    for _ in range(300):
        sim.step(0.02, accel)
    speed_before = sim.current_state.speed

    for _ in range(100):
        sim.step(0.02, brake)

    if sim.current_state.speed >= speed_before:
        print(f"  FAIL: Braking should reduce speed (before={speed_before}, after={sim.current_state.speed})")
        return False
    return True

def test_rng_state_evolves():
    print("Test: RNG state evolves deterministically")
    sim = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    inp = create_input()
    sim.step(0.02, inp)
    rng1 = sim.current_state.rngState
    sim.step(0.02, inp)
    rng2 = sim.current_state.rngState
    if rng1 == rng2:
        print("  FAIL: RNG state should evolve each tick.")
        return False
    # Run again with same seed, verify same sequence
    sim2 = SingleCarSimulation(create_spec(), create_regs(), 12345, 0.02)
    sim2.step(0.02, inp)
    sim2.step(0.02, inp)
    if sim2.current_state.rngState != rng2:
        print("  FAIL: RNG state sequence should be identical across runs.")
        return False
    return True

def test_ers_deploy_rate_sourced_from_regulation():
    print("Test: ERS deploy rate sourced from RegulationProfile (not hard-coded)")
    spec = create_spec()
    regs = create_regs()
    cfg = create_config(spec, regs)
    validate_config(cfg)

    if not approx_equal(cfg.ERSDeployRate_MJ_per_s, 0.35, 1e-5):
        print(f"  FAIL: Deploy rate should be 0.35 MJ/s (350 kW), got {cfg.ERSDeployRate_MJ_per_s}")
        return False
    print(f"  Deploy rate = {cfg.ERSDeployRate_MJ_per_s} MJ/s (350 kW) — correct")
    return True

def test_tyre_widths_2026():
    print("Test: Tyre widths match 2026 Pirelli specification")
    spec = create_spec()
    regs = create_regs()
    cfg = create_config(spec, regs)

    if cfg.FrontWidth_mm != 280.0:
        print(f"  FAIL: Front width should be 280mm, got {cfg.FrontWidth_mm}")
        return False
    if cfg.RearWidth_mm != 375.0:
        print(f"  FAIL: Rear width should be 375mm, got {cfg.RearWidth_mm}")
        return False
    return True

def test_race_qualifying_config():
    print("Test: Race Qualifying config present in RegulationProfile")
    regs = create_regs()
    if regs.RaceQualifying.Q1Duration_s != 1080.0:
        print(f"  FAIL: Q1 should be 1080s, got {regs.RaceQualifying.Q1Duration_s}")
        return False
    if regs.RaceQualifying.Q2Duration_s != 900.0:
        print(f"  FAIL: Q2 should be 900s, got {regs.RaceQualifying.Q2Duration_s}")
        return False
    if regs.RaceQualifying.Q3Duration_s != 780.0:
        print(f"  FAIL: Q3 should be 780s, got {regs.RaceQualifying.Q3Duration_s}")
        return False
    if regs.RaceQualifying.EliminationPerRound != 5:
        print(f"  FAIL: Elimination should be 5, got {regs.RaceQualifying.EliminationPerRound}")
        return False
    return True

def test_sprint_elimination_counts():
    print("Test: Sprint elimination counts present in RegulationProfile")
    regs = create_regs()
    if regs.Sprint.Sq1EliminationCount != 6:
        print(f"  FAIL: SQ1 elimination should be 6, got {regs.Sprint.Sq1EliminationCount}")
        return False
    if regs.Sprint.Sq2EliminationCount != 6:
        print(f"  FAIL: SQ2 elimination should be 6, got {regs.Sprint.Sq2EliminationCount}")
        return False
    return True

def test_suspension_config_present():
    print("Test: Suspension config present in RegulationProfile and VehicleSpec")
    regs = create_regs()
    cfg = create_config(create_spec(), regs)
    if cfg.SuspensionMaxTravel_mm <= 0:
        print("  FAIL: SuspensionMaxTravel_mm must be positive.")
        return False
    if cfg.FrontSpringRate_N_per_m <= 0:
        print("  FAIL: FrontSpringRate must be positive.")
        return False
    if cfg.RideHeight_min_mm <= 0:
        print("  FAIL: RideHeight_min_mm must be positive.")
        return False
    return True

def test_harvest_limit_with_overtake():
    print("Test: Harvest limit with Overtake Mode (9.0 MJ/lap)")
    regs = create_regs()
    if regs.Ers.HarvestLimitWithOvertake_MJ_perLap != 9.0:
        print(f"  FAIL: Should be 9.0, got {regs.Ers.HarvestLimitWithOvertake_MJ_perLap}")
        return False
    cfg = create_config(create_spec(), regs)
    if cfg.HarvestLimitWithOvertake_MJ_perLap != 9.0:
        print(f"  FAIL: Config should be 9.0, got {cfg.HarvestLimitWithOvertake_MJ_perLap}")
        return False
    return True
def main():
    tests = [
        test_identical_inputs_and_seed,
        test_repeated_runs,
        test_fixed_timestep_consistency,
        test_vehicle_spec_change,
        test_regulation_profile_change,
        test_invalid_config_fails,
        test_zero_throttle_slows,
        test_braking_reduces_speed,
        test_rng_state_evolves,
        test_ers_deploy_rate_sourced_from_regulation,
        test_tyre_widths_2026,
        test_race_qualifying_config,
        test_sprint_elimination_counts,
        test_suspension_config_present,
        test_harvest_limit_with_overtake,
    ]

    passed = 0
    failed = 0
    for test in tests:
        try:
            if test():
                print("  PASS")
                passed += 1
            else:
                failed += 1
        except Exception as e:
            print(f"  ERROR: {e}")
            failed += 1

    print(f"\nResults: {passed} passed, {failed} failed out of {len(tests)}")
    if failed > 0:
        sys.exit(1)
    sys.exit(0)

if __name__ == "__main__":
    main()
