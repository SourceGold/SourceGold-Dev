using UnityEngine;
using Cinemachine;

public class LockCamera : CinemachineExtension
{
    public float m_YPosition = 0.05f;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {

            //var pos = state.RawPosition;
            //var pos = state.FinalPosition;
            //pos.y = m_YPosition;
            //state.RawPosition = pos;
            //state.FinalPosition = pos;
        }

    }
}
