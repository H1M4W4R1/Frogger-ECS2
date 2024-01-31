using Audio.Components;
using Audio.Components.Managed;
using Audio.LowLevel;
using Unity.Entities;

namespace Audio.Systems.Managed
{
    public partial class AudioClipRegistrationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<AudioClips>();
            RequireForUpdate<ClipsRegistered>();
        }

        protected override void OnUpdate()
        {
            foreach (var (managedClips, e) in 
                     SystemAPI
                         .Query<AudioClips>()
                         .WithDisabled<ClipsRegistered>()
                         .WithEntityAccess())
            {
                // Register clips and mark as processed
                foreach (var clip in managedClips.clips)
                    AudioClipLibrary.AddClip(clip.clip, clip.uniqueId); 

                SystemAPI.SetComponentEnabled<ClipsRegistered>(e, true);
            }
        }
    }
}