using System;

namespace Stellar_Sprites
{
    public class SS_Random
    {
        [ThreadStatic]
        private static Random _local;

        public SS_Random(int seed)
        {
            if (_local == null)
            {
                _local = new Random(seed);
            }
        }

        public int NextInt()
        {
            return _local.Next();
        }

        public bool NextBool()
        {
            return _local.NextDouble() >= 0.5;
        }

        public UnityEngine.Color NextColor()
        {
            return new UnityEngine.Color(Range(0.0f, 1f), Range(0.0f, 1f), Range(0.0f, 1f));
        }

        public float Range(float minimum, float maximum)
        {
            return (float)_local.NextDouble() * (maximum - minimum) + minimum;
        }

        public int Range(int minimum, int maximum)
        {
			return _local.Next(minimum, maximum);
        }

        public int RangeEven(int minimum, int maximum)
        {
			return 2 * Range(minimum / 2, maximum / 2);
        }
    }
}