using UnityEngine;

public class Stalker : Monster
{
    protected bool isChasing;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackSpeed = 5f;

    float attackElapsedTime = 0f;

    protected override void Update()
    {
        if (isChasing)
        {
            Chase();
            if (Vector3.Distance(transform.position, AttackTargetTransform.position) < attackRange)
            {
                Attack();
            }
        }
        else
        {
            base.Update();

            if (PlayerTransform != null && Vector3.Distance(transform.position, PlayerTransform.position) < 10f)
            {
                StartChasing(PlayerTransform);
            }
        }
    }

    void Attack()
    {
        attackElapsedTime += Time.deltaTime * attackSpeed;
        if (attackElapsedTime >= 1f)
        {
            PlayerTransform.GetComponent<LocalRunner>().Health -= 1;
            Debug.Log($"{name} attacks {AttackTargetTransform.name}");
            attackElapsedTime = 0f;
        }
    }

    public void StartChasing(Transform target)
    {
        AttackTargetTransform = target;
        isChasing = true;
    }

    protected virtual void Chase()
    {
        if (AttackTargetTransform != null)
        {
            Vector3 direction = (AttackTargetTransform.position - transform.position).normalized;
            transform.position += moveSpeed * Time.deltaTime * direction;
            transform.LookAt(AttackTargetTransform);
        }
    }
}