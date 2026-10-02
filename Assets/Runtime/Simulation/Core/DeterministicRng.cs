namespace Overtake.Simulation.Core
{
    public struct DeterministicRng
    {
        private uint _state;

        public DeterministicRng(uint seed)
        {
            _state = seed;
        }

        public uint Next()
        {
            uint z = _state + 0x6D2B79F5u;
            _state = z;
            z = (z ^ (z >> 15)) * (1u | z);
            z ^= z + (z ^ (z >> 7)) * (61u | z);
            return z ^ (z >> 14);
        }

        public float NextFloat()
        {
            return Next() / (float)uint.MaxValue;
        }

        public float NextFloat(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            uint range = (uint)(maxExclusive - minInclusive);
            uint limit = uint.MaxValue - (uint.MaxValue % range);
            uint val;
            do
            {
                val = Next();
            } while (val >= limit);
            return minInclusive + (int)(val % range);
        }
    }
}
