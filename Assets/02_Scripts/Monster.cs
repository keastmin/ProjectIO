using UnityEngine;

public class Monster : MonoBehaviour
{
    public Territory Territory;
    public Transform AttackTargetTransform;
    [SerializeField] int health = 10;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float arriveThreshold = 0.1f;

    // Track Monster
    Track track;
    int currentPointIndex = 0;

    // World Monster
    Vector3 patrolPivotPosition;
    float patrolRadius;
    Vector3 patrolTargetPosition;
    bool isPatrolling = false;

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

    void Update()
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

    void FollowTrack()
    {
        if (track.Points == null || track.Points.Length == 0) return;

        Vector3 target = track.Points[currentPointIndex];
        Vector3 moveDir = (target - transform.position);
        moveDir.y = 0; // y축 고정(필요시)
        float distance = moveDir.magnitude;

        if (distance < arriveThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % track.Points.Length;
            return;
        }

        Vector3 move = moveDir.normalized * moveSpeed * Time.deltaTime;
        if (move.magnitude > distance) move = moveDir; // 목표점 초과 방지
        transform.position += move;
        transform.LookAt(target);
    }

    void Patrol()
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

    public void OnTerritoryExpanded(Territory territory, TerritoryExpandingSystem territoryExpandingSystem)
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