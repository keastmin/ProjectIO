using System.Collections;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : NetworkBehaviour
{
    [SerializeField] float destroyTime = 5f; // 발사체가 파괴되는 시간

    float epsilon = 0.1f;
    public float ExplosionRange;
    public LayerMask EnemyLayer;
    Transform targetTransform;
    float speed;
    int damage;

    void Start()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyTime);
        Die();
    }

    public void SetTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public override void FixedUpdateNetwork()
    {
        if (targetTransform != null && targetTransform.IsUnityNull() == false)
        {
            FollowTarget();
            SpeedUp();
            if (CheckArrival())
            {
                if (Object.HasStateAuthority)
                {
                    var colliders = HitTestWithAttackRange();
                    foreach (var collider in colliders)
                    {
                        ApplyDamageToTarget(collider);
                    }
                }
                Die();
            }
        }
    }

    void Die()
    {
        StopAllCoroutines();
        Runner.Despawn(Object);
    }

    void FollowTarget()
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, targetTransform.position);
        var step = Time.deltaTime * speed;
        if (step > distance)
        {
            step = distance;
        }
        transform.position += step * direction;
        transform.LookAt(targetTransform);
    }

    void SpeedUp()
    {
        speed *= 1.2f;
    }

    bool CheckArrival()
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        return distance < epsilon;
    }

    Collider[] HitTestWithAttackRange()
    {
        var HitColliders = Physics.OverlapSphere(transform.position, ExplosionRange, EnemyLayer);
        return HitColliders;
    }

    void ApplyDamageToTarget(Collider collider)
    {
        if (collider.TryGetComponent(out NetworkObject hitNo))
        {
            collider.GetComponent<Monster>()?.TakeDamage(damage);
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.TryGetComponent<LocalMonster>(out var monster))
    //     {
    //         monster.TakeDamage(damage);
    //         Destroy(gameObject); // Destroy the projectile after hitting the target
    //     }
    // }
}