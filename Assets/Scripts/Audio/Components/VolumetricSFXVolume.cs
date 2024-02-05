using Audio.LowLevel;
using Unity.Entities;
using Unity.Mathematics;

namespace Audio.Components
{
    public struct VolumetricSFXVolume : IComponentData
    {
        public UniqueAudioClip clip;
        public float3 center;
        public float3 scale;
    }
}