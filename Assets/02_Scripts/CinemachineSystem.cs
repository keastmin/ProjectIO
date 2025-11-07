using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSystem : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _playerRunnerCamera;
    [SerializeField] private CinemachineCamera _playerBuilderCamera;

    private CinemachineCamera _runnerCamera;
    private CinemachineCamera _builderCamera;

    public void InitCinemachineCamera(PlayerPosition myPosition, PlayerRunner runner, PlayerBuilder builder)
    {
        InstantiateCinemachineCamera();
        SetPriority(myPosition);
        SetTrackingTarget(runner);
        SetPlayerCinemachineRef(runner, builder);
    }

    // 시네머신 생성
    private void InstantiateCinemachineCamera()
    {
        _runnerCamera = Instantiate(_playerRunnerCamera);
        _builderCamera = Instantiate(_playerBuilderCamera);
    }

    // 내 역할군에 따라 시네머신 우선순위 결정
    private void SetPriority(PlayerPosition myPosition)
    {
        switch (myPosition)
        {
            case PlayerPosition.Runner:
                _runnerCamera.Priority = 1;
                _builderCamera.Priority = 0;
                break;
            case PlayerPosition.Builder:
                _runnerCamera.Priority = 0;
                _builderCamera.Priority = 1;
                break;
        }
    }

    // 시네머신의 타겟을 설정
    public void SetTrackingTarget(Transform transform)
    {
        _runnerCamera.Target.TrackingTarget = transform;
    }
    private void SetTrackingTarget(PlayerRunner runner)
    {
        _runnerCamera.Target.TrackingTarget = runner.transform;
    }

    // 플레이어에게 시네머신에 참조 전달
    private void SetPlayerCinemachineRef(PlayerRunner runner, PlayerBuilder builder)
    {
        builder.SetCinemachineRef(_builderCamera);
    }

    private void LateUpdate()
    {
    }

    // 자기 자신이 빌더라면 마우스로 화면 제어
    private void BuilderCamMove()
    {
        float border = 20f;         // 화면 끝 감지 영역(픽셀)
        float moveSpeed = 10f;      // 카메라 이동 속도(유닛/초)
        Vector3 moveDir = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 왼쪽
        if (mousePos.x < border)
            moveDir += Vector3.left;
        // 오른쪽
        if (mousePos.x > screenWidth - border)
            moveDir += Vector3.right;
        // 아래쪽
        if (mousePos.y < border)
            moveDir += Vector3.back; // 월드 Z-축이 앞으로라면 back
        // 위쪽
        if (mousePos.y > screenHeight - border)
            moveDir += Vector3.forward; // 월드 Z-축이 앞으로라면 forward

        if (moveDir != Vector3.zero)
        {
            // 월드 공간에서 카메라 이동 (XZ 플레인 기준)
            Vector3 move = moveDir.normalized * moveSpeed * Time.deltaTime;
            _builderCamera.transform.position += new Vector3(move.x, 0f, move.z);
        }
    }
}
