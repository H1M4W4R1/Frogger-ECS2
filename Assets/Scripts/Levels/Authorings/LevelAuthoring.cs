using Audio.Managed;
using Audio.Managed.Components;
using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class LevelAuthoring : MonoBehaviour
    {
        public bool procedurallyGeneratedLevel = true;
        public MusicTrack levelAudioTrack;
        
        private class Baker : Baker<LevelAuthoring>
        {
            public override void Bake(LevelAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelTag());
                AddComponent(e, new LevelBuiltTag());

                SetComponentEnabled<LevelBuiltTag>(e, false);

                AddComponentObject(e, new LevelAudioTrack()
                {
                    track = authoring.levelAudioTrack
                });
                
                if (authoring.procedurallyGeneratedLevel)
                    AddComponent(e, new BuildableLevelTag());
            }
        }
    }
}
