using Fusion;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _firePosition;

    [Networked] private NetworkObject _target { get; set; }
    [Networked] private TickTimer _tick { get; set; }

    private Collider[] _hits;

    public override void Spawned()
    {
        if (_firePosition == null) _firePosition = transform;
        _hits = new Collider[64];
        _tick = TickTimer.CreateFromSeconds(Runner, 1f / Mathf.Max(0.0001f, _attackSpeed));
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        var ps = gameObject.scene.GetPhysicsScene();

        // Transform 기반 이동(NMA/Translate 등) 반영
        Physics.SyncTransforms();

        // 현재 타겟 검증: 무효면 새로 획득
        if (!IsTargetValid(_target))
        {
            _target = AcquireTarget(ps);
        }

        // Y축만 회전
        if (_target)
        {
            Vector3 dir = _target.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                var look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, look, _rotateSpeed * Runner.DeltaTime);
            }
        }

        // 발사
        if (_tick.ExpiredOrNotRunning(Runner))
        {
            _tick = TickTimer.CreateFromSeconds(Runner, 1f / Mathf.Max(0.0001f, _attackSpeed));

            if (_target && _bullet)
            {
                Runner.Spawn(_bullet,
                            _firePosition.position,
                            Quaternion.identity,
                            null,
                            (runner, o) =>
                            {
                                o.GetComponent<Bullet>().Init(_target.transform, _bulletSpeed, _enemyLayer);
                            });
            }
        }
    }

    private bool IsTargetValid(NetworkObject no)
    {
        if (no == null) return false;
        if (!no) return false;
        if (!no.gameObject.activeInHierarchy) return false;

        // 레이어/거리 체크
        if (((1 << no.gameObject.layer) & _enemyLayer) == 0) return false;

        float sq = (no.transform.position - transform.position).sqrMagnitude;
        return sq <= _attackRange * _attackRange;
    }

    private NetworkObject AcquireTarget(PhysicsScene ps)
    {
        int count = ps.OverlapSphere(
            transform.position,
            _attackRange,
            _hits,
            _enemyLayer,                      // LayerMask -> int로 암시 변환
            QueryTriggerInteraction.Collide   // 트리거 포함
        );

        float best = float.MaxValue;
        NetworkObject bestNO = null;

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            _hits[i] = null; // 버퍼 정리

            if (!col) continue;

            // 콜라이더가 자식에 있을 수 있으므로 부모에서 찾기
            var no = col.GetComponentInParent<NetworkObject>();
            if (no == null || !no.gameObject || !no.gameObject.activeInHierarchy) continue;

            float d = (no.transform.position - transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestNO = no;
            }
        }

        return bestNO;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
