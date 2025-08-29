using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private Transform _target;
    private float _speed;
    private float _damage;
    private int _hitMask;          // 맞출 레이어(적 레이어 마스크)
    private Collider _targetCollider;

    private Rigidbody _rb;
    private SphereCollider _spc;

    private Vector3 _lastDir;

    [SerializeField] private float _maxLife = 5f;
    private float _life;

    private bool _isHit = false;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            Runner.SetIsSimulated(Object, false);
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        _rb.isKinematic = false;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.None; // Fusion이 보정
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        TryGetComponent(out _spc);
        _spc.isTrigger = true;

        _isHit = false;
    }

    // Tower에서 적 레이어 마스크를 함께 넘겨주세요.
    public void Init(Transform target, float speed, float damage, LayerMask hitMask, Collider targetCollider)
    {
        _target = target;
        _speed = speed;
        _damage = damage;
        _hitMask = hitMask.value;
        _targetCollider = targetCollider;

        _lastDir = (target.position - transform.position).normalized;
        _life = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (_isHit)
            {
                if (_targetCollider.TryGetComponent(out Monster monster))
                {
                    monster.TakeDamage(_damage);
                }

                Runner.Despawn(Object);
            }
            else
            {
                ChaseTarget();
            }
        }
    }

    private void ChaseTarget()
    {
        _life += Runner.DeltaTime;
        if (_life > _maxLife)
        {
            Runner.Despawn(Object);
            return;
        }

        Vector3 dir;
        if (_target)
        {
            Vector3 from = _rb.position;
            Vector3 aimPos = _target.position;

            dir = (aimPos - from).normalized;
            _lastDir = dir;
        }
        else
        {
            dir = _lastDir;
        }

        _rb.linearVelocity = dir * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other == _targetCollider)
            {
                _isHit = true;
            }
        }
    }
}
