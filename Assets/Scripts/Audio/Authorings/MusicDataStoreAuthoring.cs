using Audio.Components;
using UnityEngine;
using Unity.Entities;
using Player.Components;

namespace Audio.Authorings
{
    public class MusicDataStoreAuthoring : MonoBehaviour
    {

        private class Baker : Baker<MusicDataStoreAuthoring>
        {
            public override void Bake(MusicDataStoreAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);

                AddBuffer<SFXInfo>(e);
                AddComponent<BackgroundMusicInfo>(e);

            }
        }

        
    }
}
