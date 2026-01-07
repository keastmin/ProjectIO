using Fusion;
using UnityEngine;

public sealed class SentryGunTower : AttackTower
{
    // 총구 위치에서 발포 이펙트 재생
    // 타겟 몬스터에서 피격 이펙트 재생
    // 타겟 몬스터에 데미지 적용

    [Header("발사체")]
    [SerializeField] private Bullet _bullet;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletDamage;

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
            _attackTick = TickTimer.CreateFromSeconds(Runner, _attackSpeed);
            Runner.Spawn(_bullet, _attackPosition.position, Quaternion.identity, 
                         null, (Runner, o) =>
                         {
                             o.GetComponent<Bullet>().Init(_currTarget.transform, _bulletSpeed, _bulletDamage, _enemyLayer, _currTarget);
                         });
        }
    }

    private void InitSentryTower()
    {
        // 공격 타이머 초기화
        _attackTick = TickTimer.CreateFromSeconds(Runner, _attackSpeed);
    }
}