using System;
using Unity.Mathematics;

namespace Levels.Data
{
    [Serializable]
    public struct LevelStaticThreat
    {
        public byte threatId;
        public int2 tileIndex;
    }
}