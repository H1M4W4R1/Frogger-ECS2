using System;
using Unity.Mathematics;

namespace Levels.Data
{
    [Serializable]
    public struct LevelPickup
    {
        public byte pickupId;
        public int2 tileIndex;
    }
}