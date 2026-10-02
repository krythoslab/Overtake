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
    }
}
