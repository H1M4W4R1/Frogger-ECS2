using Audio.Components;
using Player.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Player.Aspects
{
    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly RefRO<PlayerTag> player;
        
        public readonly RefRW<LocalTransform> localTransform;
        public readonly RefRW<PlayerMovement> movement;
        public readonly RefRO<PlayerInput> input;

        public readonly RefRO<SFXTrack> jumpSFX;

        // ref: PlayerMovementSystem
    }
}
