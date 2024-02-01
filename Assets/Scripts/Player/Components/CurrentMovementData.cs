using Player.Systems.Enums;
using Unity.Entities;
using Unity.Mathematics;

namespace Player.Components
{
    public struct CurrentMovementData : IComponentData
    {
        public float3 startingPosition;
        public float3 movementVectorNonNormalized;
        public float3 lastJumpTarget;
        
        public JumpDirection direction;
        public bool isMovementRequested;
        public bool isMovementComputing;
        public bool isJumpAnimating;
    }
}