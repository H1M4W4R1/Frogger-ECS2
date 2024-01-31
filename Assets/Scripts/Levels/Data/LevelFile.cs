using System;
using Unity.Collections;

namespace Levels.Data
{
    [Serializable]
    public struct LevelFile
    {
        // Level lanes
        public NativeArray<LevelLane> lanes;

        public NativeArray<LevelStaticThreat> staticThreats;
        public NativeArray<LevelMovingThreat> movingThreats;
        public NativeArray<LevelPickup> pickups;
    }
}