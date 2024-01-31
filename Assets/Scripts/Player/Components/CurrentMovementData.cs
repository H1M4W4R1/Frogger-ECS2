using Player.Systems.Enums;
using Unity.Entities;
using Unity.Mathematics;

namespace Player.Components
{
    public struct CurrentMovementData : IComponentData
    {
        public float3 startingPosition;
        public float3 movementVectorNonNormalized;
        
        public JumpDirection direction;
        public bool isMoving;
        public bool isComputing;
        public bool isAnimating;
    }
}