using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Components
{
    public struct VolumetricSFXInfo : IBufferElementData
    {
        public UniqueAudioClip sfxClip;
    }
}