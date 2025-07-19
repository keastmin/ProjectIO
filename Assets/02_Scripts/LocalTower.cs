using UnityEngine;

public enum TowerType
{
    Basic,
    Sniper,
    Splash
}

public enum ProjectileType
{
    Basic,
    Explosive,
    Piercing,
}

public class LocalTower : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float range = 5f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] Transform canonFirePositionTransform;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] LayerMask targetLayerMask;

    float elapsedTime = 0f;

    void Update()
    {
        var monsterTransform = FindNearestMonster();
        if (monsterTransform != null)
        {
            // 타워가 몬스터를 바라보도록 회전
            var d = monsterTransform.position - transform.position;
            d.y = 0;
            Vector3 direction = d.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= fireRate)
        {
            elapsedTime = 0f;
            TryShoot(monsterTransform);
        }
    }

    Transform FindNearestMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, targetLayerMask);
        Transform nearestMonster = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestMonster = hit.transform;
            }
        }

        return nearestMonster;
    }

    void TryShoot(Transform targetTransform)
    {
        // 2. 가장 가까운 몬스터가 없으면 발사하지 않음
        if (targetTransform == null) return;

        // 3. 발사체 생성 및 몬스터 방향으로 발사
        var projectile = Instantiate(projectilePrefab, canonFirePositionTransform.position, Quaternion.identity);
        projectile.SetTarget(targetTransform.GetComponent<Monster>().AttackTargetTransform);
        projectile.SetSpeed(projectileSpeed);
        projectile.SetDamage(damage);
    }
}