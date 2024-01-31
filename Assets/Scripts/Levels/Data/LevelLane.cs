using System;
using Levels.Components;
using Unity.Collections;

namespace Levels.Data 
{
    [Serializable]
    public struct LevelLane
    {
        public byte tileIdentifier;
        public NativeArray<byte> movingThreats;

        public bool isFinishLane;
    }
}