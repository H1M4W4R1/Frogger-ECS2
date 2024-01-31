using Audio.Managed;
using Audio.Managed.Components;
using Unity.Entities;

namespace Levels.Components
{
    public class LevelAudioTrack : IComponentData
    {
        public MusicTrack track;
    }
}