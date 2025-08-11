using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private Transform _target;
    private float _speed;
    private int _hitMask;          // 맞출 레이어(적 레이어 마스크)

    private Rigidbody _rb;
    private float _radius = 0.08f; // SphereCast 반경(없으면 기본값)
    private Vector3 _lastDir;

    [SerializeField] private float _maxLife = 5f;
    private float _life;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            //_rb.isKinematic = true;
            _rb.useGravity = false;
        }

        // 붙어있는 콜라이더에서 탄두 반경 추정(있으면)
        if (TryGetComponent(out SphereCollider sc))
            _radius = sc.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
    }

    // Tower에서 적 레이어 마스크를 함께 넘겨주세요.
    public void Init(Transform target, float speed, LayerMask hitMask)
    {
        _target = target;
        _speed = speed;
        _hitMask = hitMask.value;

        _lastDir = (target.position - transform.position).normalized;
        _life = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        _life += Runner.DeltaTime;
        if (_life > _maxLife)
        {
            Runner.Despawn(Object);
            return;
        }

        // 멀티피어/애디티브 대응: 내 씬의 물리씬을 사용
        var ps = gameObject.scene.GetPhysicsScene();
        Physics.SyncTransforms();

        Vector3 from = transform.position;

        // 타겟 추적(간단 예측: 타겟 RB 있으면 약간 리드)
        Vector3 aimPos;
        if (_target)
        {
            aimPos = _target.position;
            if (_target.TryGetComponent(out Rigidbody trb))
            {
                float t = Vector3.Distance(from, aimPos) / Mathf.Max(0.0001f, _speed);
                aimPos += trb.linearVelocity * t; // 간단한 리드
            }
            _lastDir = (aimPos - from).normalized;
        }
        else
        {
            aimPos = from + _lastDir * 50f;
        }

        Vector3 dir = (aimPos - from).normalized;
        float step = _speed * Runner.DeltaTime;

        // 관통 방지: SphereCast로 이동 경로 검사
        if (ps.SphereCast(from, _radius, dir, out RaycastHit hit, step, _hitMask, QueryTriggerInteraction.Collide))
        {
            if (_rb) _rb.MovePosition(hit.point);
            else transform.position = hit.point;

            Runner.Despawn(Object); // 타격 시 제거
            return;
        }

        // 충돌 없으면 이동
        Vector3 to = from + dir * step;
        if (_rb) _rb.MovePosition(to);
        else transform.position = to;

        if (dir.sqrMagnitude > 1e-6f)
            transform.forward = dir;
    }
}
