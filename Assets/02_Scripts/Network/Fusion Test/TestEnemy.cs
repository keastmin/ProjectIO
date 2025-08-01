using Fusion;
using UnityEngine;

public class TestEnemy : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _minNextMoveDelay = 3f;
    [SerializeField] private float _maxNextMoveDelay = 6f;

    private NetworkCharacterController _cc;
    private Vector3 _targetPosition;
    private bool _waiting = false;
    private float _waitUntil = 0f;
    private float _nextMoveDelay = 0f;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        // 서버(권한자)만 로직 실행
        if (Object.HasStateAuthority)
        { 
            _targetPosition = GetRandomPosition();
            _nextMoveDelay = Random.Range(_minNextMoveDelay, _maxNextMoveDelay);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return; // 권한자만 처리

        if (_waiting)
        {
            if (_nextMoveDelay >= _waitUntil)
            {
                _waitUntil = 0f;
                _waiting = false;
                _targetPosition = GetRandomPosition();
                _nextMoveDelay = Random.Range(_minNextMoveDelay, _maxNextMoveDelay);
            }
            else
            {
                // 대기 중 (아무 것도 안 함)
                return;
            }
        }

        // 목표점으로 이동
        Vector3 direction = (_targetPosition - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance < 0.1f)
        {
            // 목표 도착, 랜덤 대기 진입
            _waiting = true;
            _waitUntil += Runner.DeltaTime;
            return;
        }

        // 방향 벡터 정규화 후 이동
        direction.Normalize();
        _cc.Move(direction * _moveSpeed * Runner.DeltaTime);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 randomDirection = Random.insideUnitSphere * 5f; // 5f는 이동 범위
        randomDirection.y = 0f;
        Vector3 randomPosition = currentPosition + randomDirection;
        return randomPosition;
    }
}
