using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform AttackTargetTransform;
    [SerializeField] int health = 10;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float arriveThreshold = 0.1f;

    Track track;
    int currentPointIndex = 0;

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

    void Update()
    {
        if (track != null && track.Points != null && track.Points.Length > 0)
        {
            FollowTrack();
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
}