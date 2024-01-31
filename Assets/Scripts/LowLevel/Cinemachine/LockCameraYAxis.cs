using Cinemachine;
using UnityEngine;

namespace LowLevel.Cinemachine
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Y co-ordinate
    /// </summary>
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class LockCameraYAxis : CinemachineExtension
    {
        [Tooltip("Lock the camera's Z position to this value")]
        public float yPosition = 10;
 
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
            {
                var pos = state.RawPosition;
                pos.y = yPosition;
                state.RawPosition = pos;
            }
        }
    }
}