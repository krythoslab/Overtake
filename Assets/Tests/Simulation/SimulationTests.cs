using NUnit.Framework;
using Overtake.Simulation.Core;
using Overtake.Simulation.Vehicle;

namespace Overtake.Simulation.Tests
{
    public class SimulationTests
    {
        private VehicleSpec _spec;
        private RegulationProfile _regs;

        [SetUp]
        public void SetUp()
        {
            _spec = new VehicleSpec
            {
                Id = "test_car",
                DisplayName = "Test Car",
                RegulationProfileId = "test_regs",
                Pu = new VehicleSpec.Pu
                {
                    IceMaxPower_kW = 400f,
                    MguKMaxPower_kW = 350f,
                    BatteryCapacity_MJ = 4.5f
                },
                Dynamics = new VehicleSpec.Dynamics
                {
                    Mass_kg = 768f,
                    DragCoeffCd = 0.7f,
                    FrontalArea_m2 = 1.5f,
                    RollingResistanceCoeff = 0.015f,
                    WheelRadius_m = 0.33f,
                    MaxBrakeForce_N = 20000f,
                    GearRatios = new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f },
                    FinalDriveRatio = 3.2f,
                    IdleRpm = 800f,
                    MaxRpm = 15000f,
                    ShiftUpRpm = 12000f,
                    ShiftDownRpm = 5000f,
                    AirDensity_kg_per_m3 = 1.225f,
                    Gravity_m_per_s2 = 9.81f,
                    MaxAcceleration_m_per_s2 = 8.0f
                }
            };

            _regs = new RegulationProfile
            {
                Id = "test_regs",
                DisplayName = "Test Regulations",
                Dynamics = new RegulationProfile.Dynamics
                {
                    Mass_kg = 768f,
                    DragCoeffCd = 0.7f,
                    FrontalArea_m2 = 1.5f,
                    RollingResistanceCoeff = 0.015f,
                    WheelRadius_m = 0.33f,
                    MaxBrakeForce_N = 20000f,
                    GearRatios = new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f },
                    FinalDriveRatio = 3.2f,
                    IdleRpm = 800f,
                    MaxRpm = 15000f,
                    ShiftUpRpm = 12000f,
                    ShiftDownRpm = 5000f,
                    AirDensity_kg_per_m3 = 1.225f,
                    Gravity_m_per_s2 = 9.81f,
                    MaxAcceleration_m_per_s2 = 8.0f
                }
            };
        }

        private VehicleInput CreateInput(float throttle = 0.8f, float brake = 0f)
        {
            return new VehicleInput
            {
                steer = 0f,
                throttle = throttle,
                brake = brake,
                clutch = 0f,
                gear = 0,
                activeAero = ActiveAeroMode.Corner,
                ersMode = ERSMode.Medium
            };
        }

        [Test]
        public void IdenticalInputsAndSeed_ProduceIdenticalFinalState()
        {
            var sim1 = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var sim2 = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput();

            for (int i = 0; i < 600; i++)
            {
                sim1.Step(0.02f, input);
                sim2.Step(0.02f, input);
            }

            var s1 = sim1.CurrentState;
            var s2 = sim2.CurrentState;

            Assert.AreEqual(s1.speed, s2.speed, 1e-5f, "Speed must match.");
            Assert.AreEqual(s1.posX, s2.posX, 1e-4f, "Position must match.");
            Assert.AreEqual(s1.rpm, s2.rpm, 1e-4f, "RPM must match.");
            Assert.AreEqual(s1.gear, s2.gear, "Gear must match.");
            Assert.AreEqual(s1.batterySoC_MJ, s2.batterySoC_MJ, 1e-5f, "Battery must match.");
            Assert.AreEqual(s1.rngState, s2.rngState, "RNG state must match.");
        }

        [Test]
        public void RepeatedRuns_ProduceIdenticalResults()
        {
            var input = CreateInput();
            VehicleState firstState = default;
            bool first = true;

            for (int run = 0; run < 10; run++)
            {
                var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
                for (int i = 0; i < 600; i++)
                {
                    sim.Step(0.02f, input);
                }
                var state = sim.CurrentState;
                if (first)
                {
                    firstState = state;
                    first = false;
                }
                else
                {
                    Assert.AreEqual(firstState.speed, state.speed, 1e-5f, $"Run {run} speed mismatch.");
                    Assert.AreEqual(firstState.posX, state.posX, 1e-4f, $"Run {run} position mismatch.");
                    Assert.AreEqual(firstState.rpm, state.rpm, 1e-4f, $"Run {run} RPM mismatch.");
                    Assert.AreEqual(firstState.gear, state.gear, $"Run {run} gear mismatch.");
                    Assert.AreEqual(firstState.rngState, state.rngState, $"Run {run} RNG state mismatch.");
                }
            }
        }

        [Test]
        public void FixedTimestep_BehavesConsistently()
        {
            var input = CreateInput();
            var sim50_a = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var sim50_b = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var sim100_a = new SingleCarSimulation(_spec, _regs, 12345u, 0.01f);
            var sim100_b = new SingleCarSimulation(_spec, _regs, 12345u, 0.01f);

            float totalTime = 2.0f;
            int steps50 = (int)(totalTime / 0.02f);
            int steps100 = (int)(totalTime / 0.01f);

            for (int i = 0; i < steps50; i++) { sim50_a.Step(0.02f, input); sim50_b.Step(0.02f, input); }
            for (int i = 0; i < steps100; i++) { sim100_a.Step(0.01f, input); sim100_b.Step(0.01f, input); }

            var s50_a = sim50_a.CurrentState;
            var s50_b = sim50_b.CurrentState;
            var s100_a = sim100_a.CurrentState;
            var s100_b = sim100_b.CurrentState;

            Assert.AreEqual(s50_a.speed, s50_b.speed, 1e-5f, "50Hz speed self-consistency");
            Assert.AreEqual(s50_a.posX, s50_b.posX, 1e-4f, "50Hz position self-consistency");
            Assert.AreEqual(s100_a.speed, s100_b.speed, 1e-5f, "100Hz speed self-consistency");
            Assert.AreEqual(s100_a.posX, s100_b.posX, 1e-4f, "100Hz position self-consistency");
            Assert.AreEqual(s50_a.gear, s100_a.gear, "Gear consistency across timestep frequencies");
        }

        [Test]
        public void VehicleSpecChange_ChangesSimulationResult()
        {
            var heavySpec = CloneSpec(_spec);
            heavySpec.Dynamics.Mass_kg = 900f;
            heavySpec.Pu.IceMaxPower_kW = 400f;

            var sim1 = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var sim2 = new SingleCarSimulation(heavySpec, _regs, 12345u, 0.02f);
            var input = CreateInput();

            for (int i = 0; i < 600; i++)
            {
                sim1.Step(0.02f, input);
                sim2.Step(0.02f, input);
            }

            var s1 = sim1.CurrentState;
            var s2 = sim2.CurrentState;

            Assert.AreNotEqual(s1.speed, s2.speed, 1e-5f, "Heavier car should accelerate slower.");
            Assert.AreNotEqual(s1.posX, s2.posX, 1e-5f, "Heavier car should cover less distance.");
        }

        [Test]
        public void RegulationProfileChange_ChangesSimulationResult()
        {
            var weakRegs = CloneRegs(_regs);
            weakRegs.Dynamics.Mass_kg = 900f;

            var sim1 = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var sim2 = new SingleCarSimulation(_spec, weakRegs, 12345u, 0.02f);
            var input = CreateInput();

            for (int i = 0; i < 600; i++)
            {
                sim1.Step(0.02f, input);
                sim2.Step(0.02f, input);
            }

            var s1 = sim1.CurrentState;
            var s2 = sim2.CurrentState;

            Assert.AreNotEqual(s1.speed, s2.speed, 1e-5f, "RegulationProfile change should affect speed.");
        }

        [Test]
        public void InvalidConfiguration_FailsSafely()
        {
            var badSpec = new VehicleSpec
            {
                Dynamics = new VehicleSpec.Dynamics { Mass_kg = -100f }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                new VehicleDynamics(badSpec, _regs);
            });
        }

        [Test]
        public void ZeroThrottle_CarSlowsDown()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0f);

            for (int i = 0; i < 10; i++)
            {
                sim.Step(0.02f, input);
            }

            Assert.Less(sim.CurrentState.speed, 0.1f, "Car should slow to near stop.");
        }

        [Test]
        public void Braking_ReducesSpeed()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var accel = CreateInput(throttle: 1f);
            var brake = CreateInput(throttle: 0f, brake: 1f);

            for (int i = 0; i < 300; i++) sim.Step(0.02f, accel);
            float speedBeforeBrake = sim.CurrentState.speed;

            for (int i = 0; i < 100; i++) sim.Step(0.02f, brake);

            Assert.Less(sim.CurrentState.speed, speedBeforeBrake, "Braking should reduce speed.");
        }

        private VehicleSpec CloneSpec(VehicleSpec original)
        {
            return new VehicleSpec
            {
                Id = original.Id,
                DisplayName = original.DisplayName,
                RegulationProfileId = original.RegulationProfileId,
                Pu = new VehicleSpec.Pu
                {
                    IceMaxPower_kW = original.Pu.IceMaxPower_kW,
                    MguKMaxPower_kW = original.Pu.MguKMaxPower_kW,
                    BatteryCapacity_MJ = original.Pu.BatteryCapacity_MJ
                },
                Dynamics = new VehicleSpec.Dynamics
                {
                    Mass_kg = original.Dynamics.Mass_kg,
                    DragCoeffCd = original.Dynamics.DragCoeffCd,
                    FrontalArea_m2 = original.Dynamics.FrontalArea_m2,
                    RollingResistanceCoeff = original.Dynamics.RollingResistanceCoeff,
                    WheelRadius_m = original.Dynamics.WheelRadius_m,
                    MaxBrakeForce_N = original.Dynamics.MaxBrakeForce_N,
                    GearRatios = (float[])original.Dynamics.GearRatios.Clone(),
                    FinalDriveRatio = original.Dynamics.FinalDriveRatio,
                    IdleRpm = original.Dynamics.IdleRpm,
                    MaxRpm = original.Dynamics.MaxRpm,
                    ShiftUpRpm = original.Dynamics.ShiftUpRpm,
                    ShiftDownRpm = original.Dynamics.ShiftDownRpm,
                    AirDensity_kg_per_m3 = original.Dynamics.AirDensity_kg_per_m3,
                    Gravity_m_per_s2 = original.Dynamics.Gravity_m_per_s2,
                    MaxAcceleration_m_per_s2 = original.Dynamics.MaxAcceleration_m_per_s2
                }
            };
        }

        private RegulationProfile CloneRegs(RegulationProfile original)
        {
            return new RegulationProfile
            {
                Id = original.Id,
                DisplayName = original.DisplayName,
                Dynamics = new RegulationProfile.Dynamics
                {
                    Mass_kg = original.Dynamics.Mass_kg,
                    DragCoeffCd = original.Dynamics.DragCoeffCd,
                    FrontalArea_m2 = original.Dynamics.FrontalArea_m2,
                    RollingResistanceCoeff = original.Dynamics.RollingResistanceCoeff,
                    WheelRadius_m = original.Dynamics.WheelRadius_m,
                    MaxBrakeForce_N = original.Dynamics.MaxBrakeForce_N,
                    GearRatios = (float[])original.Dynamics.GearRatios.Clone(),
                    FinalDriveRatio = original.Dynamics.FinalDriveRatio,
                    IdleRpm = original.Dynamics.IdleRpm,
                    MaxRpm = original.Dynamics.MaxRpm,
                    ShiftUpRpm = original.Dynamics.ShiftUpRpm,
                    ShiftDownRpm = original.Dynamics.ShiftDownRpm,
                    AirDensity_kg_per_m3 = original.Dynamics.AirDensity_kg_per_m3,
                    Gravity_m_per_s2 = original.Dynamics.Gravity_m_per_s2,
                    MaxAcceleration_m_per_s2 = original.Dynamics.MaxAcceleration_m_per_s2
                }
            };
        }
    }
}
