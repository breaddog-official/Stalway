using Unity.Burst;

namespace Breaddog.Extensions
{
    [BurstCompile]
    public static class RandomE
    {

        #region Random Numbers

        public static long RandomLong(int? seed = null)
        {
            if (seed != null)
                UnityEngine.Random.InitState(seed.Value);

            int value1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            int value2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            return value1 + ((long)value2 << 32);
        }

        public static ulong RandomUlong(int? seed = null)
        {
            if (seed != null)
                UnityEngine.Random.InitState(seed.Value);

            int value1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            int value2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            return (ulong)value1 + ((ulong)value2 << 32);
        }

        public static ulong RandomUint(int? seed = null)
        {
            if (seed != null)
                UnityEngine.Random.InitState(seed.Value);

            int value1 = UnityEngine.Random.Range(0, int.MaxValue);
            int value2 = UnityEngine.Random.Range(0, int.MaxValue);
            return (uint)value1 + (uint)value2;
        }

        #endregion

    }
}
