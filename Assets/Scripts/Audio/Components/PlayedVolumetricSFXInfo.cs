using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Components
{
    public struct PlayedVolumetricSFXInfo : IBufferElementData
    {
        public UniqueAudioClip sfxClip;
    }
}