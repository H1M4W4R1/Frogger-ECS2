using Audio.Components;
using Player.Aspects;
using Player.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Audio.Systems
{
    // Use physics system to make it efficiently processed, however compute it manually
    public partial struct VolumetricSFXPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get audio system
            if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<VolumetricSFXInfo> vSfxInfo))
            {
                // Setup volumes
                foreach (var sfxVolumeData in SystemAPI.Query<VolumetricSFXVolume>())
                {
                    // Check if player is inside volume
                    var isPlayerInside = false;
                    foreach (var player in SystemAPI.Query<PlayerAspect>())
                    {
                        // If player is inside vSFX
                        var pos = player.localTransform.ValueRO.Position;
           
                        if (pos.x > sfxVolumeData.center.x - sfxVolumeData.scale.x / 2 &&
                            pos.x < sfxVolumeData.center.x + sfxVolumeData.scale.x / 2 &&
                            pos.y > sfxVolumeData.center.y - sfxVolumeData.scale.y / 2 &&
                            pos.y < sfxVolumeData.center.y + sfxVolumeData.scale.y / 2 &&
                            pos.z > sfxVolumeData.center.z - sfxVolumeData.scale.z / 2 &&
                            pos.z < sfxVolumeData.center.z + sfxVolumeData.scale.z / 2)
                        {
                            isPlayerInside = true;
                            break;
                        }
                        else
                        {
                            isPlayerInside = false;
                        }
                    }

                    // Acquire current SFXInfo if exists
                    var clipIndex = -1;
                    for (var index = 0; index < vSfxInfo.Length; index++)
                    {
                        var vObject = vSfxInfo[index];
                        if (vObject.sfxClip == sfxVolumeData.clip)
                        {
                            clipIndex = index;
                            break;
                        }
                    }

                    // Play or un-play the clip
                    if (isPlayerInside && clipIndex == -1)
                    {
                        vSfxInfo.Add(new VolumetricSFXInfo() {sfxClip = sfxVolumeData.clip});
                    }
                    else if (!isPlayerInside && clipIndex != -1)
                        vSfxInfo.RemoveAt(clipIndex);
                }
            }
        }
    }
}