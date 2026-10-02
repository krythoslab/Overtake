using Overtake.Simulation.Core;
using Overtake.Simulation.Vehicle;

namespace Overtake.Simulation.Vehicle
{
    public class SingleCarSimulation
    {
        private readonly VehicleDynamics _vehicle;
        private readonly float _fixedDt;
        private float _accumulator;
        private uint _seed;

        public VehicleState CurrentState => _vehicle.GetState();

        public SingleCarSimulation(VehicleSpec spec, RegulationProfile regs, uint seed, float fixedDt = 0.02f)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (regs == null) throw new ArgumentNullException(nameof(regs));
            if (fixedDt <= 0f || float.IsNaN(fixedDt) || float.IsInfinity(fixedDt))
                throw new ArgumentOutOfRangeException(nameof(fixedDt));

            _vehicle = new VehicleDynamics(spec, regs);
            _fixedDt = fixedDt;
            _seed = seed;
            Reset();
        }

        public void Reset()
        {
            _vehicle.Reset(_seed);
            _accumulator = 0f;
        }

        public void Step(float realDt, in VehicleInput input)
        {
            if (realDt <= 0f || float.IsNaN(realDt) || float.IsInfinity(realDt))
                return;
            _accumulator += realDt;
            while (_accumulator >= _fixedDt)
            {
                _vehicle.Update(_fixedDt, input);
                _accumulator -= _fixedDt;
            }
        }
    }
}
