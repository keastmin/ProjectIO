using UnityEngine;

public class SandTomb : Monster
{
    [SerializeField] float suckedIntoSpeed = 2f;
    [SerializeField] float suckedIntoRadius = 5f;
    [SerializeField] protected float attackSpeed = 5f;

    float attackElapsedTime = 0f;

    protected override void Update()
    {
        var isInRange = SuckIntoSandTomb();
        if (isInRange) Attack();
    }

    bool SuckIntoSandTomb()
    {
        if (PlayerTransform == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
        if (distanceToPlayer <= suckedIntoRadius)
        {
            Vector3 direction = (transform.position - PlayerTransform.position).normalized;
            PlayerTransform.position += suckedIntoSpeed * Time.deltaTime * direction;
            return true;
        }
        return false;
    }

    void Attack()
    {
        attackElapsedTime += Time.deltaTime * attackSpeed;
        if (attackElapsedTime >= 1f)
        {
            PlayerTransform.GetComponent<LocalRunner>().Health -= 1;
            Debug.Log($"{name} attacks {PlayerTransform.name}");
            attackElapsedTime = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, suckedIntoRadius);
    }
}