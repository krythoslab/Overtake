using System;

namespace Overtake.Simulation.Vehicle
{
    public class VehicleConfig
    {
        public float Mass_kg;
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
    }

    public static class VehicleConfigFactory
    {
        public static VehicleConfig Create(VehicleSpec spec, RegulationProfile regs)
        {
            var sD = spec != null && spec.Dynamics != null ? spec.Dynamics : null;
            var rD = regs != null && regs.Dynamics != null ? regs.Dynamics : null;
            var sPu = spec != null && spec.Pu != null ? spec.Pu : null;
            var rPu = regs != null && regs.Pu != null ? regs.Pu : null;

            float Val(float specVal, float regVal)
            {
                return specVal != 0f ? specVal : regVal;
            }

            int[] ValGears(float[] specGears, float[] regGears)
            {
                return specGears != null && specGears.Length > 0
                    ? Array.ConvertAll(specGears, g => g)
                    : Array.ConvertAll(regGears, g => g);
            }

            return new VehicleConfig
            {
                Mass_kg = Val(sD?.Mass_kg ?? 0f, rD?.Mass_kg ?? 768f),
                DragCoeffCd = Val(sD?.DragCoeffCd ?? 0f, rD?.DragCoeffCd ?? 0.7f),
                FrontalArea_m2 = Val(sD?.FrontalArea_m2 ?? 0f, rD?.FrontalArea_m2 ?? 1.5f),
                RollingResistanceCoeff = Val(sD?.RollingResistanceCoeff ?? 0f, rD?.RollingResistanceCoeff ?? 0.015f),
                WheelRadius_m = Val(sD?.WheelRadius_m ?? 0f, rD?.WheelRadius_m ?? 0.33f),
                MaxBrakeForce_N = Val(sD?.MaxBrakeForce_N ?? 0f, rD?.MaxBrakeForce_N ?? 20000f),
                GearRatios = ValGears(sD?.GearRatios, rD?.GearRatios ?? new float[] { 3.0f, 2.5f, 2.0f, 1.5f, 1.2f, 1.0f }),
                FinalDriveRatio = Val(sD?.FinalDriveRatio ?? 0f, rD?.FinalDriveRatio ?? 3.2f),
                IceMaxPower_kW = Val(sPu?.IceMaxPower_kW ?? 0f, rPu?.IceMaxPower_kW ?? 400f),
                IdleRpm = Val(sD?.IdleRpm ?? 0f, rD?.IdleRpm ?? 800f),
                MaxRpm = Val(sD?.MaxRpm ?? 0f, rD?.MaxRpm ?? 15000f),
                ShiftUpRpm = Val(sD?.ShiftUpRpm ?? 0f, rD?.ShiftUpRpm ?? 12000f),
                ShiftDownRpm = Val(sD?.ShiftDownRpm ?? 0f, rD?.ShiftDownRpm ?? 5000f),
                AirDensity_kg_per_m3 = Val(sD?.AirDensity_kg_per_m3 ?? 0f, rD?.AirDensity_kg_per_m3 ?? 1.225f),
                Gravity_m_per_s2 = Val(sD?.Gravity_m_per_s2 ?? 0f, rD?.Gravity_m_per_s2 ?? 9.81f),
                MaxAcceleration_m_per_s2 = Val(sD?.MaxAcceleration_m_per_s2 ?? 0f, rD?.MaxAcceleration_m_per_s2 ?? 8.0f),
                BatteryCapacity_MJ = sPu?.BatteryCapacity_MJ ?? 4.5f
            };
        }
    }
}
