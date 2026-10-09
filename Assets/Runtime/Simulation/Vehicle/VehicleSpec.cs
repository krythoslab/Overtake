namespace Overtake.Simulation.Vehicle
{
    public class VehicleSpec
    {
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string RegulationProfileId { get; set; } = "";

        public Pu Pu { get; set; } = new Pu();
        public Aero Aero { get; set; } = new Aero();
        public Dynamics Dynamics { get; set; } = new Dynamics();
        public Tires Tires { get; set; } = new Tires();
        public Suspension Suspension { get; set; } = new Suspension();
        public Fidelity Fidelity { get; set; } = new Fidelity();

        public class Pu
        {
            public float? IceMaxPower_kW { get; set; }
            public float? MguKMaxPower_kW { get; set; }
            public float? MguKOvertakeMaxPower_kW { get; set; }
            public float? MguKHarvestMax_kW { get; set; }
            public float? HarvestLimit_MJ_perLap { get; set; }
            public float? HarvestLimitWithOvertake_MJ_perLap { get; set; }
            public float? BatteryCapacity_MJ { get; set; }
            public float? BatterySoCDeltaLimit_MJ { get; set; }
            public float? DeployRate_MJ_per_s { get; set; }
            public float? HarvestRate_MJ_per_s { get; set; }
            public float? BoostDeployMultiplier { get; set; }
            public float? FuelEnergyFlowMax_MJ_per_h { get; set; }
            public string FuelFlowRpmCurve { get; set; }
            public float? FuelFlowRpmCurveMaxRpm { get; set; }
            public bool? HasMguH { get; set; }
        }

        public class Aero
        {
            public bool? ActiveAero { get; set; }
            public float? CornerModeDownforceCoeff { get; set; }
            public float? CornerModeDragCoeff { get; set; }
            public float? StraightModeDownforceCoeff { get; set; }
            public float? StraightModeDragCoeff { get; set; }
            public float? FrontTransitionTime_ms { get; set; }
            public float? RearTransitionTime_ms { get; set; }
        }

        public class Dynamics
        {
            public float? Mass_kg { get; set; }
            public float? DragCoeffCd { get; set; }
            public float? FrontalArea_m2 { get; set; }
            public float? RollingResistanceCoeff { get; set; }
            public float? WheelRadius_m { get; set; }
            public float? MaxBrakeForce_N { get; set; }
            public float[] GearRatios { get; set; }
            public float? FinalDriveRatio { get; set; }
            public float? IdleRpm { get; set; }
            public float? MaxRpm { get; set; }
            public float? ShiftUpRpm { get; set; }
            public float? ShiftDownRpm { get; set; }
            public float? AirDensity_kg_per_m3 { get; set; }
            public float? Gravity_m_per_s2 { get; set; }
            public float? MaxAcceleration_m_per_s2 { get; set; }
        }

        public class Tires
        {
            public float? FrontWidth_mm { get; set; }
            public float? RearWidth_mm { get; set; }
        }

        public class Suspension
        {
            public float? FrontSpringRate_N_per_m { get; set; }
            public float? RearSpringRate_N_per_m { get; set; }
            public float? FrontDampingCompression { get; set; }
            public float? FrontDampingRebound { get; set; }
            public float? RearDampingCompression { get; set; }
            public float? RearDampingRebound { get; set; }
            public float? FrontAntiRollBarNm_per_rad { get; set; }
            public float? RearAntiRollBarNm_per_rad { get; set; }
            public float? MaxTravel_mm { get; set; }
            public float? BumpStopStiffness_N_per_m { get; set; }
            public float? RideHeight_min_mm { get; set; }
            public float? RideHeight_max_mm { get; set; }
        }

        public class Fidelity
        {
            public string TireModel { get; set; } = "pacejka89";
            public int TireThermalNodes { get; set; } = 1;
        }
    }
}
