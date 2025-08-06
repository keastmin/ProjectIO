using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSystem : NetworkBehaviour
{
    public static CinemachineSystem Instance { get; private set; }

    [SerializeField] private CinemachineCamera _playerRunnerCamera;

    private CinemachineCamera _runnerCamera;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public override void Spawned()
    {
        InitCamera();
    }

    // 시네머신 카메라 생성
    private void InitCamera()
    {
        // 러너 카메라 생성
        _runnerCamera = Instantiate(_playerRunnerCamera);        
    }

    // 러너 카메라의 타겟을 설정
    public void SetRunnerCameraTarget(Transform targetTransform)
    {
        _runnerCamera.Target.TrackingTarget = targetTransform;
    }
}
