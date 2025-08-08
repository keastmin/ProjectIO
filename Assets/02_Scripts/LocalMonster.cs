using UnityEngine;

public class LocalMonster : MonoBehaviour
{
    public LocalTerritory Territory;
    public Transform AttackTargetTransform;
    public Transform PlayerTransform;
    [SerializeField] protected int health = 10;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float arriveThreshold = 0.1f;

    // Track Monster
    protected Track track;
    protected int currentPointIndex = 0;

    // World Monster
    protected Vector3 patrolPivotPosition;
    protected float patrolRadius;
    protected Vector3 patrolTargetPosition;
    protected bool isPatrolling = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // 몬스터가 죽으면 제거
        }
    }

    public void SetTrack(Track track)
    {
        this.track = track;
        currentPointIndex = 0;
    }

    public void SetPatrolPivotPosition(Vector3 position)
    {
        patrolPivotPosition = position;
    }

    public void SetPatrolRadius(float radius)
    {
        patrolRadius = radius;
    }

    protected virtual void Update()
    {
        if (track != null)
        {
            FollowTrack();
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void FollowTrack()
    {
        if (track.Vertices == null || track.Vertices.Length == 0) return;

        Vector3 target = track.Vertices[currentPointIndex];
        Vector3 moveDir = (target - transform.position);
        moveDir.y = 0; // y축 고정(필요시)
        float distance = moveDir.magnitude;

        if (distance < arriveThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % track.Vertices.Length;
            return;
        }

        Vector3 move = moveDir.normalized * moveSpeed * Time.deltaTime;
        if (move.magnitude > distance) move = moveDir; // 목표점 초과 방지
        transform.position += move;
        transform.LookAt(target);
    }

    protected virtual void Patrol()
    {
        if (isPatrolling == false)
        {
            var randomTargetPosition = patrolPivotPosition + Random.insideUnitSphere * patrolRadius;
            if (!Territory.IsPointInPolygon(randomTargetPosition))
            {
                patrolTargetPosition = randomTargetPosition;
                patrolTargetPosition.y = transform.position.y;
                isPatrolling = true;
            }
        }
        else
        {
            Vector3 direction = (patrolTargetPosition - transform.position).normalized;
            transform.position = transform.position + Time.deltaTime * moveSpeed * direction;

            if (Vector3.Distance(transform.position, patrolTargetPosition) < arriveThreshold)
            {
                isPatrolling = false; // 목표 위치에 도달하면 다시 순찰 시작
            }
        }
    }

    public void OnTerritoryExpanded(LocalTerritory territory, LocalTerritoryExpandingSystem territoryExpandingSystem)
    {
        if (territory.IsPointInPolygon(transform.position))
        {
            territoryExpandingSystem.OnTerritoryExpandedEvent -= OnTerritoryExpanded;
            Destroy(gameObject); // 영역이 확장되면 몬스터 제거
        }
    }

    void OnDrawGizmosSelected()
    {
        if (track == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(patrolPivotPosition, 0.1f);
            Gizmos.DrawWireSphere(patrolPivotPosition, patrolRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(patrolTargetPosition, 0.1f);
        }
    }
    void OnDrawGizmos()
    {
        if (track == null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, patrolTargetPosition + Vector3.up * 0.1f);
        }
    }
}