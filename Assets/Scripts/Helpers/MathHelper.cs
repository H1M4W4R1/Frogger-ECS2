using LowLevel;
using Unity.Burst;

namespace Helpers
{
    [BurstCompile]
    public static class MathHelper
    {
        /// <summary>
        /// Used to recompute float into tile position.
        /// May still require a bit of upgrade, at last for 2 it should work fine 
        /// </summary>
        [BurstCompile]
        public static int TileInt(float f)
        {
            var fInt = (int) f;
            if (fInt % ConstConfig.TILE_SIZE != 0)
                fInt++;

            return fInt;
        }
    }
}