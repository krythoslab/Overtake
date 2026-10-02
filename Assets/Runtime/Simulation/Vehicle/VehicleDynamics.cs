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
                rngState = seed
            };
            _initialized = true;
        }

        public void Update(float dt, in VehicleInput input)
        {
            if (!_initialized) throw new InvalidOperationException("Vehicle not initialized. Call Reset first.");
            if (dt <= 0f || float.IsNaN(dt) || float.IsInfinity(dt))
                throw new ArgumentOutOfRangeException(nameof(dt), "dt must be positive and finite.");

            HandleAutoGear(input);

            float engineForce = 0f;
            if (input.throttle > 0f)
            {
                float powerForce = _cfg.IceMaxPower_kW * 1000f / MathF.Max(_state.speed, 0.5f);
                float maxForce = _cfg.Mass_kg * _cfg.MaxAcceleration_m_per_s2;
                engineForce = MathF.Min(powerForce, maxForce) * input.throttle;
            }

            float dragForce = 0.5f * _cfg.AirDensity_kg_per_m3 * _cfg.DragCoeffCd * _cfg.FrontalArea_m2 * _state.speed * _state.speed;
            float rollingForce = _cfg.RollingResistanceCoeff * _cfg.Mass_kg * _cfg.Gravity_m_per_s2;
            float brakeForce = input.brake * _cfg.MaxBrakeForce_N;

            float netForce = engineForce - dragForce - rollingForce - brakeForce;
            float acceleration = netForce / _cfg.Mass_kg;

            _state.speed += acceleration * dt;
            if (_state.speed < 0f) _state.speed = 0f;

            _state.posX += _state.speed * dt;

            if (_state.speed > 0.1f)
            {
                float wheelAngularSpeed = _state.speed / _cfg.WheelRadius_m;
                float engineAngularSpeed = wheelAngularSpeed * _cfg.FinalDriveRatio * _cfg.GearRatios[_state.gear - 1];
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

            if (input.throttle > 0.01f)
            {
                _state.batterySoC_MJ -= input.throttle * dt * 0.1f;
                if (_state.batterySoC_MJ < 0f) _state.batterySoC_MJ = 0f;
            }
            else if (input.brake > 0.01f)
            {
                _state.batterySoC_MJ += input.brake * dt * 0.05f;
                if (_state.batterySoC_MJ > _cfg.BatteryCapacity_MJ) _state.batterySoC_MJ = _cfg.BatteryCapacity_MJ;
            }

            _state.rngState = _rng.Next();
        }

        private void HandleAutoGear(in VehicleInput input)
        {
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
