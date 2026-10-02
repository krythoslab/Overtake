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
        public Fidelity Fidelity { get; set; } = new Fidelity();

        public class Pu
        {
            public float IceMaxPower_kW { get; set; } = 400f;
            public float MguKMaxPower_kW { get; set; } = 350f;
            public float MguKOvertakeMaxPower_kW { get; set; } = 350f;
            public float MguKHarvestMax_kW { get; set; } = 350f;
            public float HarvestLimit_MJ_perLap { get; set; } = 8.5f;
            public float BatteryCapacity_MJ { get; set; } = 4.5f;
            public float BatterySoCDeltaLimit_MJ { get; set; } = 4.0f;
            public float FuelEnergyFlowMax_MJ_per_h { get; set; } = 3000f;
            public string FuelFlowRpmCurve { get; set; } = "0.27 * rpm + 165";
            public bool HasMguH { get; set; } = false;
        }

        public class Aero
        {
            public bool ActiveAero { get; set; } = true;
            public float CornerModeDownforceCoeff { get; set; } = 1.0f;
            public float CornerModeDragCoeff { get; set; } = 1.0f;
            public float StraightModeDownforceCoeff { get; set; } = 0.55f;
            public float StraightModeDragCoeff { get; set; } = 0.45f;
            public float FrontTransitionTime_ms { get; set; } = 50f;
            public float RearTransitionTime_ms { get; set; } = 50f;
        }

        public class Dynamics
        {
            public float Mass_kg { get; set; } = 768f;
            public float DragCoeffCd { get; set; } = 0.7f;
            public float FrontalArea_m2 { get; set; } = 1.5f;
            public float RollingResistanceCoeff { get; set; } = 0.015f;
            public float WheelRadius_m { get; set; } = 0.33f;
            public float MaxBrakeForce_N { get; set; } = 20000f;
            public float[] GearRatios { get; set; } = new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f };
            public float FinalDriveRatio { get; set; } = 3.2f;
            public float IdleRpm { get; set; } = 800f;
            public float MaxRpm { get; set; } = 15000f;
            public float ShiftUpRpm { get; set; } = 12000f;
            public float ShiftDownRpm { get; set; } = 5000f;
            public float AirDensity_kg_per_m3 { get; set; } = 1.225f;
            public float Gravity_m_per_s2 { get; set; } = 9.81f;
            public float MaxAcceleration_m_per_s2 { get; set; } = 8.0f;
        }

        public class Tires
        {
            public float FrontWidth_mm { get; set; } = 270f;
            public float RearWidth_mm { get; set; } = 355f;
        }

        public class Fidelity
        {
            public string TireModel { get; set; } = "pacejka89";
            public int TireThermalNodes { get; set; } = 1;
        }
    }
}
