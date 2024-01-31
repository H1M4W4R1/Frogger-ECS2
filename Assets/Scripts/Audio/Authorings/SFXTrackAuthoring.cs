using Audio.Components;
using Audio.Managed;
using UnityEngine;
using Unity.Entities;
using Player.Components;

namespace Audio.Authorings
{
    public class SFXTrackAuthoring : MonoBehaviour
    {
        public AudioClip clip;

        private class Baker : Baker<SFXTrackAuthoring>
        {
            public override void Bake(SFXTrackAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new SFXTrack()
                {
                    sfxId = Synth.AddSFX(authoring.clip),
                });
            }
        }

        
    }
}
