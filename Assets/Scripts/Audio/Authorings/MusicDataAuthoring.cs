using Audio.Components;
using UnityEngine;
using Unity.Entities;
namespace Audio.Authorings
{
    public class MusicDataAuthoring : MonoBehaviour
    {

        private class Baker : Baker<MusicDataAuthoring>
        {
            public override void Bake(MusicDataAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new MusicData()
                {
                    currentBackgroundTrack = -1
                });
            }
        }

        
    }
}
