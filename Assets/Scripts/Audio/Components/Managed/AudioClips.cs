using System;
using System.Collections.Generic;
using Audio.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace Audio.Components.Managed
{
    public class AudioClips : IComponentData
    {
        public List<ManagedAudioClip> clips = new List<ManagedAudioClip>();
    }

    [Serializable]
    public class ManagedAudioClip
    {
        public UniqueAudioClip uniqueId;
        public AudioClip clip;
    }
}