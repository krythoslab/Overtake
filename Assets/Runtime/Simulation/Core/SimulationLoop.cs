namespace Overtake.Simulation.Core
{
    public class SimulationLoop
    {
        private const float DefaultFixedDt = 0.02f;
        private readonly float _fixedDt;
        private float _accumulator;

        public SimulationLoop(float fixedDt = DefaultFixedDt)
        {
            if (fixedDt <= 0f || float.IsNaN(fixedDt) || float.IsInfinity(fixedDt))
                throw new System.ArgumentOutOfRangeException(nameof(fixedDt), "Fixed dt must be positive and finite.");
            _fixedDt = fixedDt;
            _accumulator = 0f;
        }

        public void Step(float realDt, System.Action<float> onFixedUpdate)
        {
            if (realDt <= 0f || float.IsNaN(realDt) || float.IsInfinity(realDt))
                return;
            if (onFixedUpdate == null)
                throw new System.ArgumentNullException(nameof(onFixedUpdate));

            _accumulator += realDt;
            while (_accumulator >= _fixedDt)
            {
                onFixedUpdate(_fixedDt);
                _accumulator -= _fixedDt;
            }
        }

        public float GetFixedDt()
        {
            return _fixedDt;
        }

        public float GetAccumulator()
        {
            return _accumulator;
        }
    }
}
