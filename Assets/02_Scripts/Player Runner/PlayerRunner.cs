using Fusion;
using System;
using UnityEngine;

public class PlayerRunner : Player
{
    [Header("이동")]
    [SerializeField] private float _moveSpeed = 6f; // 이동 속도

    private Rigidbody _rigidbody; // 리지드바디

    public event Action<PlayerRunner> OnPositionChanged; // 영역 관련 이벤트

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>(); // 리지드바디 받아오기
    }

    public override void Spawned()
    {
        // 시네머신 시스템의 싱글톤 사용
        if (CinemachineSystem.Instance != null)
        {
            // 시네머신 시스템의 러너 카메라에 자기 자신을 타겟으로 설정
            CinemachineSystem.Instance.SetRunnerCameraTarget(transform);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // 러너 이동
            float speed = data.DashInput.IsSet(NetworkInputData.DASH_INPUT) ? _moveSpeed * 2f : _moveSpeed;
            data.PlayerRunnerDirection.Normalize();
            _rigidbody.linearVelocity = speed * data.PlayerRunnerDirection;
        }

        // 영역에 관한 로직 이벤트 수행
        OnPositionChanged?.Invoke(this);
    }
}
