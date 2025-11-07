using System.Collections;
using Fusion;
using UnityEngine;

public class CenterTower : AttackTower
{
    [Header("발사체")]
    [SerializeField] private Bullet _bullet;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private int _bulletCount;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            InitSentryTower();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            SetTarget();
            LookAtTarget();
            Fire();
        }
    }

    protected override void Fire()
    {
        if (_attackTick.ExpiredOrNotRunning(Runner) && _currTarget != null)
        {
            _attackTick = TickTimer.CreateFromSeconds(Runner, 1 / _attackSpeed);
            StartCoroutine(FireRoutine());
        }
    }

    IEnumerator FireRoutine()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            Runner.Spawn(_bullet, _attackPosition.position, Quaternion.identity,
                            null, (Runner, o) =>
                            {
                                o.GetComponent<Bullet>().Init(_currTarget.transform, _bulletSpeed, _bulletDamage, _enemyLayer, _currTarget);
                            });
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void InitSentryTower()
    {
        // 공격 타이머 초기화
        _attackTick = TickTimer.CreateFromSeconds(Runner, 1 / _attackSpeed);
    }
}