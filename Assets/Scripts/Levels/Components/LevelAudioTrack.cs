using Audio.Managed;
using Unity.Entities;

namespace Levels.Components
{
    public struct LevelAudioTrack : IComponentData
    {
        public int track;
    }
}