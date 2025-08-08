using Fusion;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    [SerializeField] private LayerMask _enemyLayer; // 공격할 레이어 마스크
    [SerializeField] private float _attackRange = 5f; // 공격 가능 범위
    [SerializeField] private float _attackSpeed = 1f; // 초당 공격 횟수
    [SerializeField] private float _attackDamage = 5f; // 공격력

    public override void Spawned()
    {
        Debug.Log("타워 스폰됨");
    }

    private void OnDrawGizmos()
    {
        
    }
}
