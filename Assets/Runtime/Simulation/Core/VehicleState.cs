namespace Overtake.Simulation.Vehicle
{
    public struct VehicleState
    {
        public float posX;
        public float posY;
        public float posZ;
        public float quatX;
        public float quatY;
        public float quatZ;
        public float quatW;
        public float speed;
        public float rpm;
        public int gear;
        public float tireSlipFrontLeft;
        public float tireSlipFrontRight;
        public float tireSlipRearLeft;
        public float tireSlipRearRight;
        public float batterySoC_MJ;
        public uint rngState;
        public ActiveAeroMode activeAeroMode;
        public ERSMode ersMode;
        public float fuelMass_kg;
        public float activeAeroTransitionTimer_ms;
        public bool activeAeroFailed;

        public float suspensionTravelFL_mm;
        public float suspensionTravelFR_mm;
        public float suspensionTravelRL_mm;
        public float suspensionTravelRR_mm;
        public float rideHeightFL_mm;
        public float rideHeightFR_mm;
        public float rideHeightRL_mm;
        public float rideHeightRR_mm;
        public float suspensionVelocityFL_m_per_s;
        public float suspensionVelocityFR_m_per_s;
        public float suspensionVelocityRL_m_per_s;
        public float suspensionVelocityRR_m_per_s;
    }
}
