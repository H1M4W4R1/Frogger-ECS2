using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Components
{
    public struct LocalMusicTheme : IComponentData
    {
        public UniqueAudioClip audioId;
    }
}