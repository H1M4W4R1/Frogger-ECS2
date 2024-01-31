using Levels.Components;
using Unity.Entities;

namespace Audio.Managed.Systems
{
    public partial class AudioMusicSystem : SystemBase
    {
        public void OnCreate(ref SystemState state)
        {
            RequireForUpdate<LevelAudioTrack>();
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithAll<LevelAudioTrack>()
                .ForEach((in LevelAudioTrack track) =>
                {
                    Jukebox.Play(track.track);
                }).Run();
        }
    }
}