using System;

namespace Overtake.Simulation.Vehicle
{
    public class VehicleConfig
    {
        public float Mass_kg;
        public float InitialFuelMass_kg;
        public float DragCoeffCd;
        public float FrontalArea_m2;
        public float RollingResistanceCoeff;
        public float WheelRadius_m;
        public float MaxBrakeForce_N;
        public float[] GearRatios;
        public float FinalDriveRatio;
        public float IceMaxPower_kW;
        public float IdleRpm;
        public float MaxRpm;
        public float ShiftUpRpm;
        public float ShiftDownRpm;
        public float AirDensity_kg_per_m3;
        public float Gravity_m_per_s2;
        public float MaxAcceleration_m_per_s2;
        public float BatteryCapacity_MJ;
        public float MguKMaxPower_kW;
        public float MguKOvertakeMaxPower_kW;
        public float MguKHarvestMax_kW;
        public float HarvestLimit_MJ_perLap;
        public float BatterySoCDeltaLimit_MJ;
        public float FuelEnergyFlowMax_MJ_per_h;
        public string FuelFlowRpmCurve;
        public bool HasMguH;
        public bool ActiveAero;
        public float CornerModeDownforceCoeff;
        public float CornerModeDragCoeff;
        public float StraightModeDownforceCoeff;
        public float StraightModeDragCoeff;
        public float FrontTransitionTime_ms;
        public float RearTransitionTime_ms;
        public float FrontWidth_mm;
        public float RearWidth_mm;
        public float ERSDeployRate_MJ_per_s;
        public float ERSBoostDeployMultiplier;
        public float ERSHarvestRate_MJ_per_s;
    }

    public static class VehicleConfigFactory
    {
        public static VehicleConfig Create(VehicleSpec spec, RegulationProfile regs)
        {
            var sD = spec != null && spec.Dynamics != null ? spec.Dynamics : null;
            var rD = regs != null && regs.Dynamics != null ? regs.Dynamics : null;
            var sPu = spec != null && spec.Pu != null ? spec.Pu : null;
            var rPu = regs != null && regs.Pu != null ? regs.Pu : null;
            var sAero = spec != null && spec.Aero != null ? spec.Aero : null;
            var rAero = regs != null && regs.Aero != null ? regs.Aero : null;
            var sTires = spec != null && spec.Tires != null ? spec.Tires : null;
            var rTires = regs != null && regs.Tires != null ? regs.Tires : null;

            float Val(float? specVal, float regVal)
            {
                return specVal ?? regVal;
            }

            string ValStr(string specVal, string regVal)
            {
                return !string.IsNullOrEmpty(specVal) ? specVal : regVal;
            }

            bool ValBool(bool? specVal, bool regVal)
            {
                return specVal ?? regVal;
            }

            float[] ValGears(float[] specGears, float[] regGears)
            {
                if (specGears != null && specGears.Length > 0)
                    return (float[])specGears.Clone();
                if (regGears != null && regGears.Length > 0)
                    return (float[])regGears.Clone();
                return new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f };
            }

            return new VehicleConfig
            {
                Mass_kg = Val(sD?.Mass_kg, rD?.Mass_kg ?? 768f),
                InitialFuelMass_kg = 100f,
                DragCoeffCd = Val(sD?.DragCoeffCd, rD?.DragCoeffCd ?? 0.7f),
                FrontalArea_m2 = Val(sD?.FrontalArea_m2, rD?.FrontalArea_m2 ?? 1.5f),
                RollingResistanceCoeff = Val(sD?.RollingResistanceCoeff, rD?.RollingResistanceCoeff ?? 0.015f),
                WheelRadius_m = Val(sD?.WheelRadius_m, rD?.WheelRadius_m ?? 0.33f),
                MaxBrakeForce_N = Val(sD?.MaxBrakeForce_N, rD?.MaxBrakeForce_N ?? 20000f),
                GearRatios = ValGears(sD?.GearRatios, rD?.GearRatios ?? new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f }),
                FinalDriveRatio = Val(sD?.FinalDriveRatio, rD?.FinalDriveRatio ?? 3.2f),
                IceMaxPower_kW = Val(sPu?.IceMaxPower_kW, rPu?.IceMaxPower_kW ?? 400f),
                IdleRpm = Val(sD?.IdleRpm, rD?.IdleRpm ?? 800f),
                MaxRpm = Val(sD?.MaxRpm, rD?.MaxRpm ?? 15000f),
                ShiftUpRpm = Val(sD?.ShiftUpRpm, rD?.ShiftUpRpm ?? 12000f),
                ShiftDownRpm = Val(sD?.ShiftDownRpm, rD?.ShiftDownRpm ?? 5000f),
                AirDensity_kg_per_m3 = Val(sD?.AirDensity_kg_per_m3, rD?.AirDensity_kg_per_m3 ?? 1.225f),
                Gravity_m_per_s2 = Val(sD?.Gravity_m_per_s2, rD?.Gravity_m_per_s2 ?? 9.81f),
                MaxAcceleration_m_per_s2 = Val(sD?.MaxAcceleration_m_per_s2, rD?.MaxAcceleration_m_per_s2 ?? 8.0f),
                BatteryCapacity_MJ = sPu?.BatteryCapacity_MJ ?? 4.5f,
                MguKMaxPower_kW = Val(sPu?.MguKMaxPower_kW, rPu?.MguKMaxPower_kW ?? 350f),
                MguKOvertakeMaxPower_kW = Val(sPu?.MguKOvertakeMaxPower_kW, rPu?.MguKMaxPower_kW ?? 350f),
                MguKHarvestMax_kW = Val(sPu?.MguKHarvestMax_kW, rPu?.HarvestMaxPower_kW ?? 350f),
                HarvestLimit_MJ_perLap = Val(sPu?.HarvestLimit_MJ_perLap, rPu?.HarvestLimit_MJ_perLap ?? 8.5f),
                BatterySoCDeltaLimit_MJ = Val(sPu?.BatterySoCDeltaLimit_MJ, rPu?.BatterySoCDeltaLimit_MJ ?? 4.0f),
                FuelEnergyFlowMax_MJ_per_h = Val(sPu?.FuelEnergyFlowMax_MJ_per_h, rPu?.FuelEnergyFlowMax_MJ_per_h ?? 3000f),
                FuelFlowRpmCurve = ValStr(sPu?.FuelFlowRpmCurve, rPu?.FuelFlowRpmCurve ?? "0.27 * rpm + 165"),
                HasMguH = ValBool(sPu?.HasMguH, rPu?.HasMguH ?? false),
                ActiveAero = ValBool(sAero?.ActiveAero, rAero?.ActiveAero ?? true),
                CornerModeDownforceCoeff = Val(sAero?.CornerModeDownforceCoeff, rAero?.CornerModeDownforceCoeff ?? 1.0f),
                CornerModeDragCoeff = Val(sAero?.CornerModeDragCoeff, rAero?.CornerModeDragCoeff ?? 1.0f),
                StraightModeDownforceCoeff = Val(sAero?.StraightModeDownforceCoeff, rAero?.StraightModeDownforceCoeff ?? 0.55f),
                StraightModeDragCoeff = Val(sAero?.StraightModeDragCoeff, rAero?.StraightModeDragCoeff ?? 0.45f),
                FrontTransitionTime_ms = Val(sAero?.FrontTransitionTime_ms, rAero?.FrontTransitionTime_ms ?? 50f),
                RearTransitionTime_ms = Val(sAero?.RearTransitionTime_ms, rAero?.RearTransitionTime_ms ?? 50f),
                FrontWidth_mm = Val(sTires?.FrontWidth_mm, rTires?.FrontWidth_mm ?? 270f),
                RearWidth_mm = Val(sTires?.RearWidth_mm, rTires?.RearWidth_mm ?? 355f),
                ERSDeployRate_MJ_per_s = 0.8f,
                ERSBoostDeployMultiplier = 1.5f,
                ERSHarvestRate_MJ_per_s = 0.5f
            };
        }
    }
}
