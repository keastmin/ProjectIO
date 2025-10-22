using Fusion;
using UnityEngine;

public class MissileTower : AttackTower
{
    // 총구 위치에서 발포 이펙트 재생
    // 타겟 몬스터에서 피격 이펙트 재생
    // 타겟 몬스터에 데미지 적용

    public Transform MuzzleTransform;
    public Missile MissilePrefab;

    public float AttackRange = 1f;
    public float MissileSpeed = 10f;
    public int MissileDamage = 1;
    public float MissileExplosionRange = 3f;

    TickTimer tickTimer;
    public float elapsedTime = 0;

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            SetTarget();
            LookAtTarget();
            if (CheckTargetInAttackRange())
            {
                Fire();
            }
        }
    }

    void Update()
    {
        if (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * _attackSpeed;
        }
    }

    bool CheckTargetInAttackRange()
    {
        if (_currTarget == null) return false;

        float distance = Vector3.SqrMagnitude(transform.position - _currTarget.transform.position);
        return distance <= AttackRange * AttackRange;
    }

    protected override void Fire()
    {
        if (elapsedTime < 1f) { return; }

        elapsedTime -= 1f;

        var missileObject = Runner.Spawn(MissilePrefab, MuzzleTransform.position, MuzzleTransform.rotation);
        var missile = missileObject.GetComponent<Missile>();
        missile.SetTarget(_currTarget.transform);
        missile.SetSpeed(MissileSpeed);
        missile.SetDamage(MissileDamage);
        missile.ExplosionRange = MissileExplosionRange;
        missile.EnemyLayer = _enemyLayer;
    }
}