using System;
using Unity.Mathematics;

namespace Levels.Data
{
    [Serializable]
    public struct LevelMovingThreat
    {
        public byte threatId;
        public int2 tileIndex;

        public bool directionRight; // false: left, true: right
    }
}