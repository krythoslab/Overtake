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
                        MguKOvertakeMaxPower_kW = 350f,
                        MguKHarvestMax_kW = 350f,
                        HarvestLimit_MJ_perLap = 8.5f,
                        HarvestLimitWithOvertake_MJ_perLap = 9.0f,
                        BatteryCapacity_MJ = 4.5f,
                        BatterySoCDeltaLimit_MJ = 4.0f,
                        FuelEnergyFlowMax_MJ_per_h = 3000f,
                        FuelFlowRpmCurve = "0.27 * rpm + 165",
                        FuelFlowRpmCurveMaxRpm = 10500f,
                        DeployRate_MJ_per_s = 0.35f,
                        HarvestRate_MJ_per_s = 0.35f,
                        BoostDeployMultiplier = 1.5f,
                        HasMguH = false
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
                    Ers = new RegulationProfile.Ers
                    {
                        MguKMaxPower_kW = 350f,
                        HarvestMaxPower_kW = 350f,
                        HarvestLimit_MJ_perLap = 8.5f,
                        HarvestLimitWithOvertake_MJ_perLap = 9.0f,
                        BatterySoCDeltaLimit_MJ = 4.0f,
                        DeployRate_MJ_per_s = 0.35f,
                        HarvestRate_MJ_per_s = 0.35f,
                        BoostDeployMultiplier = 1.5f
                    },
                    Pu = new RegulationProfile.Pu
                    {
                        IceMaxPower_kW = 400f,
                        FuelEnergyFlowMax_MJ_per_h = 3000f,
                        FuelFlowRpmCurve = "0.27 * rpm + 165",
                        FuelFlowRpmCurveMaxRpm = 10500f,
                        HasMguH = false
                    },
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

        private VehicleInput CreateInput(float throttle = 0.8f, float brake = 0f, ERSMode ersMode = ERSMode.Medium, ActiveAeroMode activeAero = ActiveAeroMode.Corner)
        {
            return new VehicleInput
            {
                steer = 0f,
                throttle = throttle,
                brake = brake,
                clutch = 0f,
                gear = 0,
                activeAero = activeAero,
                ersMode = ersMode
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

        [Test]
        public void ERS_Boost_DrainsBattery()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0.5f, ersMode: ERSMode.Boost);
            input.gear = 1;

            float initialBattery = sim.CurrentState.batterySoC_MJ;
            for (int i = 0; i < 100; i++)
            {
                sim.Step(0.02f, input);
            }

            Assert.Less(sim.CurrentState.batterySoC_MJ, initialBattery, "ERS Boost should drain battery.");
        }

        [Test]
        public void ERS_BatteryEmpty_ForcesOff()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0.1f, ersMode: ERSMode.Boost);
            input.gear = 1;

            for (int i = 0; i < 2000; i++)
            {
                sim.Step(0.02f, input);
            }

            Assert.AreEqual(ERSMode.Off, sim.CurrentState.ersMode, "ERS should turn off when battery is empty.");
            Assert.AreEqual(0f, sim.CurrentState.batterySoC_MJ, 1e-5f, "Battery should be at zero.");
        }

        [Test]
        public void ERS_Harvest_IncreasesBattery()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var drain = CreateInput(throttle: 0.5f, brake: 0f, ersMode: ERSMode.Boost);
            drain.gear = 1;
            for (int i = 0; i < 200; i++) sim.Step(0.02f, drain);

            var input = CreateInput(throttle: 0f, brake: 1f, ersMode: ERSMode.Medium);
            input.gear = 1;

            float initialBattery = sim.CurrentState.batterySoC_MJ;
            for (int i = 0; i < 100; i++)
            {
                sim.Step(0.02f, input);
            }

            Assert.Greater(sim.CurrentState.batterySoC_MJ, initialBattery, "Braking with ERS Medium should harvest energy.");
        }

        [Test]
        public void ERSDeployRate_SourcedFromRegulationProfile()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0.5f, ersMode: ERSMode.Boost);
            input.gear = 1;

            float initialBattery = sim.CurrentState.batterySoC_MJ;
            sim.Step(0.02f, input);
            float actualDrain = initialBattery - sim.CurrentState.batterySoC_MJ;
            float expectedDrain = 0.35f * 0.02f;

            Assert.AreEqual(expectedDrain, actualDrain, 1e-5f, "ERS deploy rate should be 0.35 MJ/s (350 kW), not the old 0.8 MJ/s (800 kW).");
        }

        [Test]
        public void Suspension_Travel_ChangesWithAcceleration()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 1f, ersMode: ERSMode.Off, activeAero: ActiveAeroMode.Corner);
            input.gear = 1;

            float initialTravelFL = sim.CurrentState.suspensionTravelFL_mm;

            for (int i = 0; i < 500; i++) sim.Step(0.02f, input);

            float finalTravelFL = sim.CurrentState.suspensionTravelFL_mm;
            Assert.AreNotEqual(initialTravelFL, finalTravelFL, 1e-5f, "Suspension travel should change under acceleration forces.");
        }

        [Test]
        public void FuelMass_DecreasesOverTime()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0.8f);
            input.gear = 1;

            float initialFuel = sim.CurrentState.fuelMass_kg;
            for (int i = 0; i < 600; i++)
            {
                sim.Step(0.02f, input);
            }

            Assert.Less(sim.CurrentState.fuelMass_kg, initialFuel, "Fuel mass should decrease.");
        }

        [Test]
        public void ActiveAero_TransitionTimer_BlocksModeChange()
        {
            var sim = new SingleCarSimulation(_spec, _regs, 12345u, 0.02f);
            var input = CreateInput(throttle: 0.5f, activeAero: ActiveAeroMode.StraightLine);
            input.gear = 1;

            sim.Step(0.02f, input);
            Assert.AreEqual(ActiveAeroMode.Corner, sim.CurrentState.activeAeroMode, "Should still be Corner during transition.");

            sim.Step(0.02f, input);
            Assert.AreEqual(ActiveAeroMode.Corner, sim.CurrentState.activeAeroMode, "Should still be Corner during transition.");

            sim.Step(0.02f, input);
            Assert.AreEqual(ActiveAeroMode.Corner, sim.CurrentState.activeAeroMode, "Should still be Corner during transition.");

            sim.Step(0.02f, input);
            Assert.AreEqual(ActiveAeroMode.StraightLine, sim.CurrentState.activeAeroMode, "Should transition to StraightLine after timer.");
        }

        [Test]
        public void Determinism_PreservedWithERSAndActiveAero()
        {
            var input = CreateInput(throttle: 0.8f, ersMode: ERSMode.Boost, activeAero: ActiveAeroMode.StraightLine);
            input.gear = 1;

            VehicleState firstState = default;
            bool first = true;

            for (int run = 0; run < 5; run++)
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
                    Assert.AreEqual(firstState.batterySoC_MJ, state.batterySoC_MJ, 1e-5f, $"Run {run} battery mismatch.");
                    Assert.AreEqual(firstState.ersMode, state.ersMode, $"Run {run} ERS mode mismatch.");
                    Assert.AreEqual(firstState.activeAeroMode, state.activeAeroMode, $"Run {run} active aero mode mismatch.");
                    Assert.AreEqual(firstState.fuelMass_kg, state.fuelMass_kg, 1e-4f, $"Run {run} fuel mass mismatch.");
                }
            }
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
                    MguKOvertakeMaxPower_kW = original.Pu.MguKOvertakeMaxPower_kW,
                    MguKHarvestMax_kW = original.Pu.MguKHarvestMax_kW,
                    HarvestLimit_MJ_perLap = original.Pu.HarvestLimit_MJ_perLap,
                    HarvestLimitWithOvertake_MJ_perLap = original.Pu.HarvestLimitWithOvertake_MJ_perLap,
                    BatteryCapacity_MJ = original.Pu.BatteryCapacity_MJ,
                    BatterySoCDeltaLimit_MJ = original.Pu.BatterySoCDeltaLimit_MJ,
                    DeployRate_MJ_per_s = original.Pu.DeployRate_MJ_per_s,
                    HarvestRate_MJ_per_s = original.Pu.HarvestRate_MJ_per_s,
                    BoostDeployMultiplier = original.Pu.BoostDeployMultiplier,
                    FuelEnergyFlowMax_MJ_per_h = original.Pu.FuelEnergyFlowMax_MJ_per_h,
                    FuelFlowRpmCurve = original.Pu.FuelFlowRpmCurve,
                    FuelFlowRpmCurveMaxRpm = original.Pu.FuelFlowRpmCurveMaxRpm,
                    HasMguH = original.Pu.HasMguH
                },
                Tires = new VehicleSpec.Tires
                {
                    FrontWidth_mm = original.Tires?.FrontWidth_mm,
                    RearWidth_mm = original.Tires?.RearWidth_mm
                },
                Suspension = new VehicleSpec.Suspension
                {
                    FrontSpringRate_N_per_m = original.Suspension?.FrontSpringRate_N_per_m,
                    RearSpringRate_N_per_m = original.Suspension?.RearSpringRate_N_per_m,
                    FrontDampingCompression = original.Suspension?.FrontDampingCompression,
                    FrontDampingRebound = original.Suspension?.FrontDampingRebound,
                    RearDampingCompression = original.Suspension?.RearDampingCompression,
                    RearDampingRebound = original.Suspension?.RearDampingRebound,
                    FrontAntiRollBarNm_per_rad = original.Suspension?.FrontAntiRollBarNm_per_rad,
                    RearAntiRollBarNm_per_rad = original.Suspension?.RearAntiRollBarNm_per_rad,
                    MaxTravel_mm = original.Suspension?.MaxTravel_mm,
                    BumpStopStiffness_N_per_m = original.Suspension?.BumpStopStiffness_N_per_m,
                    RideHeight_min_mm = original.Suspension?.RideHeight_min_mm,
                    RideHeight_max_mm = original.Suspension?.RideHeight_max_mm
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
                Ers = new RegulationProfile.Ers
                {
                    MguKMaxPower_kW = original.Ers.MguKMaxPower_kW,
                    HarvestMaxPower_kW = original.Ers.HarvestMaxPower_kW,
                    HarvestLimit_MJ_perLap = original.Ers.HarvestLimit_MJ_perLap,
                    HarvestLimitWithOvertake_MJ_perLap = original.Ers.HarvestLimitWithOvertake_MJ_perLap,
                    BatterySoCDeltaLimit_MJ = original.Ers.BatterySoCDeltaLimit_MJ,
                    DeployRate_MJ_per_s = original.Ers.DeployRate_MJ_per_s,
                    HarvestRate_MJ_per_s = original.Ers.HarvestRate_MJ_per_s,
                    BoostDeployMultiplier = original.Ers.BoostDeployMultiplier
                },
                Pu = new RegulationProfile.Pu
                {
                    IceMaxPower_kW = original.Pu.IceMaxPower_kW,
                    FuelEnergyFlowMax_MJ_per_h = original.Pu.FuelEnergyFlowMax_MJ_per_h,
                    FuelFlowRpmCurve = original.Pu.FuelFlowRpmCurve,
                    FuelFlowRpmCurveMaxRpm = original.Pu.FuelFlowRpmCurveMaxRpm,
                    HasMguH = original.Pu.HasMguH
                },
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
