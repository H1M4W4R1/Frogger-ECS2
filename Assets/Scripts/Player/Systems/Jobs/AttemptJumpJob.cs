using Player.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Player.Systems.Jobs
{
    public partial struct AttemptJumpJob : IJobEntity
    {
        public byte foundTileId;
        
        public void Execute(in LocalTransform localTransform, ref CurrentMovementData movementData,
            in PlayerInput input, in PlayerMovementSettings movementSettings)
        {
            // Tile is null
            if (foundTileId == 0) return;
            if (movementData.movementVectorNonNormalized is {x: 0, z: 0}) return;
            
            movementData.isMoving = true;
        }
    }
}