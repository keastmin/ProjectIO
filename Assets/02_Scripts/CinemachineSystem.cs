using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSystem : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _playerRunnerCamera;
    [SerializeField] private CinemachineCamera _playerBuilderCamera;

    private CinemachineCamera _runnerCamera;
    private CinemachineCamera _builderCamera;

    void Awake()
    {
        InitCamera();
    }

    // 시네머신 카메라 생성
    private void InitCamera()
    {
        // 러너 카메라 생성
        _runnerCamera = Instantiate(_playerRunnerCamera);

        // 빌더 카메라 생성
        _builderCamera = Instantiate(_playerBuilderCamera);
    }

    // 러너 카메라의 타겟을 설정
    public void SetRunnerCameraTarget(Transform targetTransform)
    {
        _runnerCamera.Target.TrackingTarget = targetTransform;
    }

    // 자신의 포지션에 따라 시네머신 우선순위를 설정 ---------------------------------------
    private void SetCinemachinePriority(CinemachineCamera cinemachine, int priority)
    {
        cinemachine.Priority = priority;
    }

    public void SetCinemachinePriority(PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.Builder:
                SetCinemachinePriority(_runnerCamera, 0);
                SetCinemachinePriority(_builderCamera, 1);
                break;
            case PlayerPosition.Runner:
                SetCinemachinePriority(_runnerCamera, 1);
                SetCinemachinePriority(_builderCamera, 0);
                break;
        }
    }
    // ---------------------------------------------------------------------------------
}
