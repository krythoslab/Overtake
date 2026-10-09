namespace Overtake.Simulation.Vehicle
{
    public class RegulationProfile
    {
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public Overtake Overtake { get; set; } = new Overtake();
        public Ers Ers { get; set; } = new Ers();
        public Pu Pu { get; set; } = new Pu();
        public Aero Aero { get; set; } = new Aero();
        public Dynamics Dynamics { get; set; } = new Dynamics();
        public Tires Tires { get; set; } = new Tires();
        public Sprint Sprint { get; set; } = new Sprint();
        public RaceQualifying RaceQualifying { get; set; } = new RaceQualifying();
        public Suspension Suspension { get; set; } = new Suspension();

        public class Overtake
        {
            public float GapThreshold_s { get; set; } = 1.0f;
            public float BonusEnergy_MJ { get; set; } = 0.5f;
            public float FollowMaxPower_kW { get; set; } = 350f;
            public float FollowMaxSpeed_kmh { get; set; } = 337f;
            public float LeadTaperStart_kmh { get; set; } = 290f;
            public float LeadTaperEnd_kmh { get; set; } = 355f;
        }

        public class Ers
        {
            public float MguKMaxPower_kW { get; set; } = 350f;
            public float HarvestMaxPower_kW { get; set; } = 350f;
            public float HarvestLimit_MJ_perLap { get; set; } = 8.5f;
            public float HarvestLimitWithOvertake_MJ_perLap { get; set; } = 9.0f;
            public float BatterySoCDeltaLimit_MJ { get; set; } = 4.0f;
            public float DeployRate_MJ_per_s { get; set; } = 0.35f;
            public float HarvestRate_MJ_per_s { get; set; } = 0.35f;
            public float BoostDeployMultiplier { get; set; } = 1.5f;
        }

        public class Pu
        {
            public float IceMaxPower_kW { get; set; } = 400f;
            public float FuelEnergyFlowMax_MJ_per_h { get; set; } = 3000f;
            public string FuelFlowRpmCurve { get; set; } = "0.27 * rpm + 165";
            public float FuelFlowRpmCurveMaxRpm { get; set; } = 10500f;
            public bool HasMguH { get; set; } = false;
        }

        public class Aero
        {
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
            public float FrontWidth_mm { get; set; } = 280f;
            public float RearWidth_mm { get; set; } = 375f;
        }

        public class Sprint
        {
            public float Sq1Duration_s { get; set; } = 720f;
            public float Sq2Duration_s { get; set; } = 600f;
            public float Sq3Duration_s { get; set; } = 480f;
            public int Sq3CarCount { get; set; } = 10;
            public int Sq1EliminationCount { get; set; } = 6;
            public int Sq2EliminationCount { get; set; } = 6;
        }

        public class RaceQualifying
        {
            public float Q1Duration_s { get; set; } = 1080f;
            public float Q2Duration_s { get; set; } = 900f;
            public float Q3Duration_s { get; set; } = 780f;
            public int EliminationPerRound { get; set; } = 5;
        }

        public class Suspension
        {
            public float RideHeight_min_mm { get; set; } = 20f;
            public float RideHeight_max_mm { get; set; } = 50f;
            public float SpringRate_N_per_m { get; set; } = 120000f;
            public float DamperCompression_N_per_m { get; set; } = 80000f;
            public float DamperRebound_N_per_m { get; set; } = 120000f;
            public float AntiRollBarStiffness_Nm_per_rad { get; set; } = 50000f;
            public float MaxTravel_mm { get; set; } = 100f;
            public float BumpStopStiffness_N_per_m { get; set; } = 500000f;
        }
    }
}
