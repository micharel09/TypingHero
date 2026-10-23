using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CameraImpactExtension : CinemachineExtension
{
    [SerializeField] CombatCameraImpact impact;
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Noise && impact) state.PositionCorrection += impact.OffsetForState(state);
    }
}
