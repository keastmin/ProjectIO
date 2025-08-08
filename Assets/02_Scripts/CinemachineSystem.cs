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

    private void Update()
    {
        if(StageManager.Instance != null)
        {
            // 러너가 자기 자신이라면
            if (StageManager.Instance.Players[PlayerPosition.Runner] == StageManager.Instance.Runner.LocalPlayer)
            {
                BuilderCamFollow();
            }
            else
            {
                BuilderCamMove();
            }
        }
    }

    // 자기 자신이 러너라면 빌더 카메라는 러너 카메라를 따라감
    private void BuilderCamFollow()
    {
        Vector3 builderCamPos = _runnerCamera.transform.position;
        Debug.Log($"Host: {StageManager.Instance.Runner.IsServer}, " +
            $"Player: {StageManager.Instance.Runner.LocalPlayer}," +
            $"Camera Position: {builderCamPos}");
        _builderCamera.transform.position = builderCamPos;
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
