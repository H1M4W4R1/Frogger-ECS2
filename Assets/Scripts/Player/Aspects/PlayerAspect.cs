using Player.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Player.Aspects
{
    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly RefRO<PlayerTag> player;
        
        public readonly RefRW<LocalTransform> localTransform;
        public readonly RefRW<PlayerMovementSettings> movement;
        public readonly RefRO<PlayerInput> input;

        public readonly RefRW<CurrentMovementData> movementInformation;

        // ref: PlayerMovementSystem
    }
}
