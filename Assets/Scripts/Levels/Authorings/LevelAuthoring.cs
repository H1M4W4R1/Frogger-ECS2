using Audio.Managed;
using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class LevelAuthoring : MonoBehaviour
    {
        public bool procedurallyGeneratedLevel = true;
        public AudioClip levelAudioTrack;
        
        private class Baker : Baker<LevelAuthoring>
        {
            public override void Bake(LevelAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelTag());
                AddComponent(e, new LevelBuiltTag());

                SetComponentEnabled<LevelBuiltTag>(e, false);

                AddComponent(e, new LevelAudioTrack()
                {
                    track = Jukebox.RegisterClip(authoring.levelAudioTrack)
                });
                
                if (authoring.procedurallyGeneratedLevel)
                    AddComponent(e, new BuildableLevelTag());
            }
        }
    }
}
