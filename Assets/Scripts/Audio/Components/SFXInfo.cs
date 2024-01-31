using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Components
{
    public struct SFXInfo : IBufferElementData
    {
        public UniqueAudioClip sfxClip;
    }
}