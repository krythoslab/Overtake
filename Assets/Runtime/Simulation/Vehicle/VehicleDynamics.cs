using System;
using Overtake.Simulation.Core;

namespace Overtake.Simulation.Vehicle
{
    public class VehicleDynamics : IVehicle
    {
        private readonly VehicleConfig _cfg;
        private VehicleState _state;
        private DeterministicRng _rng;
        private bool _initialized;

        public VehicleDynamics(VehicleSpec spec, RegulationProfile regs)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (regs == null) throw new ArgumentNullException(nameof(regs));

            _cfg = VehicleConfigFactory.Create(spec, regs);
            ValidateConfig(_cfg);
        }

        private static void ValidateConfig(VehicleConfig cfg)
        {
            if (cfg.Mass_kg <= 0f) throw new InvalidOperationException("Mass_kg must be positive.");
            if (cfg.DragCoeffCd < 0f) throw new InvalidOperationException("DragCoeffCd must be non-negative.");
            if (cfg.FrontalArea_m2 <= 0f) throw new InvalidOperationException("FrontalArea_m2 must be positive.");
            if (cfg.RollingResistanceCoeff < 0f) throw new InvalidOperationException("RollingResistanceCoeff must be non-negative.");
            if (cfg.WheelRadius_m <= 0f) throw new InvalidOperationException("WheelRadius_m must be positive.");
            if (cfg.MaxBrakeForce_N < 0f) throw new InvalidOperationException("MaxBrakeForce_N must be non-negative.");
            if (cfg.GearRatios == null || cfg.GearRatios.Length == 0) throw new InvalidOperationException("GearRatios must be non-empty.");
            if (cfg.FinalDriveRatio <= 0f) throw new InvalidOperationException("FinalDriveRatio must be positive.");
            if (cfg.IceMaxPower_kW <= 0f) throw new InvalidOperationException("IceMaxPower_kW must be positive.");
            if (cfg.IdleRpm < 0f) throw new InvalidOperationException("IdleRpm must be non-negative.");
            if (cfg.MaxRpm <= cfg.IdleRpm) throw new InvalidOperationException("MaxRpm must be greater than IdleRpm.");
            if (cfg.ShiftUpRpm <= cfg.ShiftDownRpm) throw new InvalidOperationException("ShiftUpRpm must be greater than ShiftDownRpm.");
            if (cfg.AirDensity_kg_per_m3 <= 0f) throw new InvalidOperationException("AirDensity_kg_per_m3 must be positive.");
            if (cfg.Gravity_m_per_s2 <= 0f) throw new InvalidOperationException("Gravity_m_per_s2 must be positive.");
            if (cfg.MaxAcceleration_m_per_s2 <= 0f) throw new InvalidOperationException("MaxAcceleration_m_per_s2 must be positive.");
            if (cfg.BatteryCapacity_MJ < 0f) throw new InvalidOperationException("BatteryCapacity_MJ must be non-negative.");
            if (cfg.MguKMaxPower_kW < 0f) throw new InvalidOperationException("MguKMaxPower_kW must be non-negative.");
            if (cfg.HarvestLimit_MJ_perLap < 0f) throw new InvalidOperationException("HarvestLimit_MJ_perLap must be non-negative.");
            if (cfg.BatterySoCDeltaLimit_MJ < 0f) throw new InvalidOperationException("BatterySoCDeltaLimit_MJ must be non-negative.");
            if (cfg.FuelEnergyFlowMax_MJ_per_h < 0f) throw new InvalidOperationException("FuelEnergyFlowMax_MJ_per_h must be non-negative.");
            if (cfg.FrontTransitionTime_ms < 0f) throw new InvalidOperationException("FrontTransitionTime_ms must be non-negative.");
            if (cfg.RearTransitionTime_ms < 0f) throw new InvalidOperationException("RearTransitionTime_ms must be non-negative.");
            if (cfg.InitialFuelMass_kg < 0f) throw new InvalidOperationException("InitialFuelMass_kg must be non-negative.");
            if (cfg.FuelFlowRpmCurveMaxRpm < 0f) throw new InvalidOperationException("FuelFlowRpmCurveMaxRpm must be non-negative.");
        }

        public void Reset(uint seed)
        {
            _rng = new DeterministicRng(seed);
            _state = new VehicleState
            {
                posX = 0f,
                posY = 0f,
                posZ = 0f,
                quatX = 0f,
                quatY = 0f,
                quatZ = 0f,
                quatW = 1f,
                speed = 0f,
                rpm = _cfg.IdleRpm,
                gear = 1,
                tireSlipFrontLeft = 0f,
                tireSlipFrontRight = 0f,
                tireSlipRearLeft = 0f,
                tireSlipRearRight = 0f,
                batterySoC_MJ = _cfg.BatteryCapacity_MJ,
                rngState = seed,
                activeAeroMode = ActiveAeroMode.Corner,
                ersMode = ERSMode.Off,
                fuelMass_kg = _cfg.InitialFuelMass_kg,
                activeAeroTransitionTimer_ms = 0f,
                activeAeroFailed = false,
                suspensionTravelFL_mm = 0f,
                suspensionTravelFR_mm = 0f,
                suspensionTravelRL_mm = 0f,
                suspensionTravelRR_mm = 0f,
                rideHeightFL_mm = _cfg.RideHeight_min_mm,
                rideHeightFR_mm = _cfg.RideHeight_min_mm,
                rideHeightRL_mm = _cfg.RideHeight_min_mm,
                rideHeightRR_mm = _cfg.RideHeight_min_mm,
                suspensionVelocityFL_m_per_s = 0f,
                suspensionVelocityFR_m_per_s = 0f,
                suspensionVelocityRL_m_per_s = 0f,
                suspensionVelocityRR_m_per_s = 0f
            };
            _initialized = true;
        }

        public void Update(float dt, in VehicleInput input)
        {
            if (!_initialized) throw new InvalidOperationException("Vehicle not initialized. Call Reset first.");
            if (dt <= 0f || float.IsNaN(dt) || float.IsInfinity(dt))
                throw new ArgumentOutOfRangeException(nameof(dt), "dt must be positive and finite.");

            HandleAutoGear(input);

            float totalMass = _cfg.Mass_kg + _state.fuelMass_kg;

            float engineForce = 0f;
            if (input.throttle > 0f)
            {
                float powerForce = _cfg.IceMaxPower_kW * 1000f / MathF.Max(_state.speed, 0.5f);
                float maxForce = totalMass * _cfg.MaxAcceleration_m_per_s2;
                engineForce = MathF.Min(powerForce, maxForce) * input.throttle;
            }

            float dragForce = 0.5f * _cfg.AirDensity_kg_per_m3 * _cfg.DragCoeffCd * _cfg.FrontalArea_m2 * _state.speed * _state.speed;
            float rollingForce = _cfg.RollingResistanceCoeff * totalMass * _cfg.Gravity_m_per_s2;
            float brakeForce = input.brake * _cfg.MaxBrakeForce_N;

            float netForce = engineForce - dragForce - rollingForce - brakeForce;
            float acceleration = netForce / totalMass;

            _state.speed += acceleration * dt;
            if (_state.speed < 0f) _state.speed = 0f;

            _state.posX += _state.speed * dt;

            if (_state.speed > 0.1f)
            {
                int gearIndex = Math.Clamp(_state.gear - 1, 0, _cfg.GearRatios.Length - 1);
                float wheelAngularSpeed = _state.speed / _cfg.WheelRadius_m;
                float engineAngularSpeed = wheelAngularSpeed * _cfg.FinalDriveRatio * _cfg.GearRatios[gearIndex];
                _state.rpm = engineAngularSpeed * 60f / (2f * MathF.PI);
                _state.rpm = MathF.Clamp(_state.rpm, _cfg.IdleRpm, _cfg.MaxRpm);
            }
            else
            {
                _state.rpm = _cfg.IdleRpm + input.throttle * (_cfg.MaxRpm - _cfg.IdleRpm);
            }

            float slipMag = MathF.Abs(acceleration) / (_cfg.Gravity_m_per_s2 * 0.9f);
            float slipClamped = MathF.Clamp(slipMag, 0f, 1f);
            _state.tireSlipFrontLeft = slipClamped;
            _state.tireSlipFrontRight = slipClamped;
            _state.tireSlipRearLeft = slipClamped;
            _state.tireSlipRearRight = slipClamped;

            UpdateERS(dt, input);
            UpdateActiveAero(dt, input);
            UpdateFuel(dt, input.throttle, input.brake);
            UpdateSuspension(dt, acceleration, totalMass);

            _state.rngState = _rng.GetState();
        }

        private void UpdateSuspension(float dt, float acceleration, float totalMass)
        {
            float staticRideHeight_m = _cfg.RideHeight_min_mm / 1000f;

            float[] staticHeights = { staticRideHeight_m, staticRideHeight_m, staticRideHeight_m, staticRideHeight_m };

            float loadTransferPerWheel = (totalMass * acceleration * 0.5f) / 4f * 0.1f;

            float[] loadTransfer = {
                -loadTransferPerWheel, -loadTransferPerWheel,
                loadTransferPerWheel, loadTransferPerWheel
            };

            float[] currentHeights = {
                _state.rideHeightFL_mm / 1000f,
                _state.rideHeightFR_mm / 1000f,
                _state.rideHeightRL_mm / 1000f,
                _state.rideHeightRR_mm / 1000f
            };

            float[] velocities = {
                _state.suspensionVelocityFL_m_per_s,
                _state.suspensionVelocityFR_m_per_s,
                _state.suspensionVelocityRL_m_per_s,
                _state.suspensionVelocityRR_m_per_s
            };

            float[] springRates = {
                _cfg.FrontSpringRate_N_per_m, _cfg.FrontSpringRate_N_per_m,
                _cfg.RearSpringRate_N_per_m, _cfg.RearSpringRate_N_per_m
            };

            float[] dampers = {
                _cfg.FrontDampingCompression, _cfg.FrontDampingCompression,
                _cfg.RearDampingCompression, _cfg.RearDampingCompression
            };

            float[] dampersRebound = {
                _cfg.FrontDampingRebound, _cfg.FrontDampingRebound,
                _cfg.RearDampingRebound, _cfg.RearDampingRebound
            };

            for (int i = 0; i < 4; i++)
            {
                float equilibriumHeight = staticHeights[i] - loadTransfer[i] / springRates[i];
                float displacement = currentHeights[i] - equilibriumHeight;
                float travel_mm = (float)(displacement * 1000f);

                float springForce = springRates[i] * displacement;
                float damperForce = velocities[i] > 0f ? dampers[i] * velocities[i] : dampersRebound[i] * velocities[i];
                float bumpStopForce = 0f;

                if (travel_mm > _cfg.SuspensionMaxTravel_mm)
                {
                    float excess = travel_mm - _cfg.SuspensionMaxTravel_mm;
                    bumpStopForce = _cfg.BumpStopStiffness_N_per_m * (excess / 1000f);
                }
                else if (travel_mm < 0f)
                {
                    travel_mm = 0f;
                }

                float massPerWheel = totalMass * _cfg.Gravity_m_per_s2 / 4f;
                float totalForce = springForce + damperForce + bumpStopForce;
                float acceleration_local = totalForce / massPerWheel;

                velocities[i] += acceleration_local * dt;
                currentHeights[i] += velocities[i] * dt;

                float newTravel_mm = (currentHeights[i] - staticHeights[i]) * 1000f;
                if (newTravel_mm < 0f) newTravel_mm = 0f;
                if (newTravel_mm > _cfg.SuspensionMaxTravel_mm) newTravel_mm = _cfg.SuspensionMaxTravel_mm;

                float newRideHeight_mm = staticRideHeight_m * 1000f + newTravel_mm;

                switch (i)
                {
                    case 0:
                        _state.suspensionTravelFL_mm = newTravel_mm;
                        _state.rideHeightFL_mm = newRideHeight_mm;
                        _state.suspensionVelocityFL_m_per_s = velocities[i];
                        break;
                    case 1:
                        _state.suspensionTravelFR_mm = newTravel_mm;
                        _state.rideHeightFR_mm = newRideHeight_mm;
                        _state.suspensionVelocityFR_m_per_s = velocities[i];
                        break;
                    case 2:
                        _state.suspensionTravelRL_mm = newTravel_mm;
                        _state.rideHeightRL_mm = newRideHeight_mm;
                        _state.suspensionVelocityRL_m_per_s = velocities[i];
                        break;
                    case 3:
                        _state.suspensionTravelRR_mm = newTravel_mm;
                        _state.rideHeightRR_mm = newRideHeight_mm;
                        _state.suspensionVelocityRR_m_per_s = velocities[i];
                        break;
                }
            }
        }

        private void UpdateERS(float dt, in VehicleInput input)
        {
            switch (_state.ersMode)
            {
                case ERSMode.Off:
                    if (input.ersMode != ERSMode.Off)
                    {
                        _state.ersMode = input.ersMode;
                    }
                    break;

                case ERSMode.Boost:
                    _state.batterySoC_MJ -= _cfg.ERSDeployRate_MJ_per_s * dt;
                    if (_state.batterySoC_MJ <= 0f)
                    {
                        _state.batterySoC_MJ = 0f;
                        _state.ersMode = ERSMode.Off;
                    }
                    else if (input.ersMode == ERSMode.Off)
                    {
                        _state.ersMode = ERSMode.Off;
                    }
                    break;

                case ERSMode.Qualifying:
                    _state.batterySoC_MJ -= _cfg.ERSDeployRate_MJ_per_s * _cfg.ERSBoostDeployMultiplier * dt;
                    if (_state.batterySoC_MJ <= 0f)
                    {
                        _state.batterySoC_MJ = 0f;
                        _state.ersMode = ERSMode.Off;
                    }
                    else if (input.ersMode == ERSMode.Off)
                    {
                        _state.ersMode = ERSMode.Off;
                    }
                    break;

                default:
                    if (input.ersMode == ERSMode.Boost)
                    {
                        _state.ersMode = ERSMode.Boost;
                    }
                    else if (input.ersMode == ERSMode.Off)
                    {
                        _state.ersMode = ERSMode.Off;
                    }
                    else if (input.ersMode != _state.ersMode)
                    {
                        _state.ersMode = input.ersMode;
                    }

                    if (input.brake > 0.5f)
                    {
                        _state.batterySoC_MJ += _cfg.ERSHarvestRate_MJ_per_s * dt;
                        if (_state.batterySoC_MJ > _cfg.BatteryCapacity_MJ)
                            _state.batterySoC_MJ = _cfg.BatteryCapacity_MJ;
                    }
                    break;
            }

            if (_state.batterySoC_MJ < 0.01f && _state.ersMode != ERSMode.Off)
            {
                _state.batterySoC_MJ = 0f;
                _state.ersMode = ERSMode.Off;
            }
        }

        private void UpdateActiveAero(float dt, in VehicleInput input)
        {
            if (!_cfg.ActiveAero)
            {
                _state.activeAeroMode = ActiveAeroMode.Off;
                _state.activeAeroFailed = false;
                return;
            }

            if (_state.activeAeroFailed)
            {
                _state.activeAeroMode = ActiveAeroMode.Corner;
                _state.activeAeroTransitionTimer_ms = 0f;
                return;
            }

            float transitionTime = _state.activeAeroMode == ActiveAeroMode.Corner
                ? _cfg.FrontTransitionTime_ms
                : _cfg.RearTransitionTime_ms;

            if (_state.activeAeroTransitionTimer_ms > 0f)
            {
                _state.activeAeroTransitionTimer_ms -= dt * 1000f;
                if (_state.activeAeroTransitionTimer_ms <= 0f)
                {
                    _state.activeAeroTransitionTimer_ms = 0f;
                    _state.activeAeroMode = input.activeAero;
                }
                return;
            }

            if (input.activeAero != _state.activeAeroMode)
            {
                _state.activeAeroTransitionTimer_ms = transitionTime;
            }
        }

        private void UpdateFuel(float dt, float throttle, float brake)
        {
            float fuelRate_MJ_per_s = _cfg.IceMaxPower_kW * 0.8f;
            float fuelEnergy_MJ = fuelRate_MJ_per_s * dt * MathF.Max(throttle, 0.1f);
            float fuelMass_kg = fuelEnergy_MJ / 44.0f;
            float newFuelMass = _state.fuelMass_kg - fuelMass_kg;
            float minMass = 5.0f;
            if (newFuelMass < minMass)
                newFuelMass = minMass;
            _state.fuelMass_kg = newFuelMass;
        }

        private void HandleAutoGear(in VehicleInput input)
        {
            int targetGear = Math.Clamp(input.gear, 0, _cfg.GearRatios.Length);
            if (targetGear > 0 && targetGear != _state.gear)
            {
                _state.gear = targetGear;
                return;
            }

            if (_state.gear < _cfg.GearRatios.Length && _state.rpm >= _cfg.ShiftUpRpm)
            {
                _state.gear++;
            }
            else if (_state.gear > 1 && _state.rpm <= _cfg.ShiftDownRpm)
            {
                _state.gear--;
            }
        }

        public ref readonly VehicleState GetState()
        {
            return ref _state;
        }
    }
}
