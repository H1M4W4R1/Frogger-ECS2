using UnityEngine;
using Unity.Entities;

namespace Audio.Authorings.SFX
{
    public class PlayerSFXAuthoring : SFXTrackAuthoringBase
    {
        public AudioClip jumpSFX;
        protected override void _RegisterClips()
        {
            ClearClips();
            AddClip(jumpSFX, PlayerSFX.JUMP); 
        }

        protected class Baker : Baker<PlayerSFXAuthoring>
        {
            public override void Bake(PlayerSFXAuthoring authoring)
            {
                authoring.Bake(this, authoring);
            }
        }
    }
}