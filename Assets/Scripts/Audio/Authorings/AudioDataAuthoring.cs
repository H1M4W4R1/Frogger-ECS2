using Audio.Components;
using UnityEngine;
using Unity.Entities;
namespace Audio.Authorings
{
    public class AudioDataAuthoring : MonoBehaviour
    {

        private class Baker : Baker<AudioDataAuthoring>
        {
            public override void Bake(AudioDataAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new MusicData()
                {
                    currentBackgroundTrack = -1
                });
                AddBuffer<SFXPlayerData>(e);
            }
        }

        
    }
}
