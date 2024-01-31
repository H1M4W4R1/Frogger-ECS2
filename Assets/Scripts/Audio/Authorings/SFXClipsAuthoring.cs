using System.Collections.Generic;
using Audio.Components;
using Audio.Components.Managed;
using UnityEngine;
using Unity.Entities;

namespace Audio.Authorings
{
    public class SFXClipsAuthoring : MonoBehaviour
    {
        public List<ManagedAudioClip> sfxClips = new List<ManagedAudioClip>();

        private class Baker : Baker<SFXClipsAuthoring>
        {
            public override void Bake(SFXClipsAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);

                AddComponent<ClipsRegistered>(e);
                SetComponentEnabled<ClipsRegistered>(e, false);
                
                // Register managed clips (it will be processed by system)
                var clipsComponent = new AudioClips()
                {
                    clips = authoring.sfxClips // Ref-link to original list
                };
                AddComponentObject(e, clipsComponent);
                
            }
        }

        
    }
}
