using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Components
{
    public struct BackgroundMusicInfo : IComponentData
    {
        public UniqueAudioClip backgroundMusicIdentifier;
    }
}