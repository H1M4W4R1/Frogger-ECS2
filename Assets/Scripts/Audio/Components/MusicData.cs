using Unity.Entities;

namespace Audio.Components
{
    public struct MusicData : IComponentData
    {
        public int currentBackgroundTrack;
    }
}