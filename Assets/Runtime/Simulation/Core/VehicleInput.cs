namespace Overtake.Simulation.Vehicle
{
    public enum ActiveAeroMode
    {
        Corner,
        StraightLine,
        Off,
        Failed
    }

    public enum ERSMode
    {
        Off,
        Low,
        Medium,
        High,
        Qualifying,
        Boost
    }

    public struct VehicleInput
    {
        public float steer;
        public float throttle;
        public float brake;
        public float clutch;
        public int gear;
        public ActiveAeroMode activeAero;
        public ERSMode ersMode;
    }
}
