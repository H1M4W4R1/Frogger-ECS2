using Audio.Components;
using Audio.Components.Managed;
using UnityEngine;
using Unity.Entities;

namespace Audio.Authorings
{
    public class BackgroundMusicAuthoring : MonoBehaviour
    {
        public ManagedAudioClip clip;

        private class Baker : Baker<BackgroundMusicAuthoring>
        {
            public override void Bake(BackgroundMusicAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);

                AddComponent<ClipsRegistered>(e);
                SetComponentEnabled<ClipsRegistered>(e, false);
                
                // Register managed clips (it will be processed by system)
                var clipsComponent = new AudioClips();
                clipsComponent.clips.Add(authoring.clip);
                AddComponentObject(e, clipsComponent);
                
                // Register theme player
                AddComponent(e, new LocalMusicTheme()
                {
                    audioId = authoring.clip.uniqueId
                });

            }
        }

        
    }
}
