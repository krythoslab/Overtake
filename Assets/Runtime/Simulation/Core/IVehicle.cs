using Overtake.Simulation.Vehicle;

namespace Overtake.Simulation.Core
{
    public interface IVehicle
    {
        void Update(float dt, in VehicleInput input);
        ref readonly VehicleState GetState();
        void Reset(uint seed);
    }
}
