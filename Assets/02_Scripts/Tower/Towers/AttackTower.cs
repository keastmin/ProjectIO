using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AttackTower : Tower, IInteractableObject
{
    [Header("공격")]
    [SerializeField] protected float _attackSpeed = 1f; // 공격 주기
    [SerializeField] protected Transform _attackPosition; // 공격 시작 지점

    [Header("타겟 지정")]
    [SerializeField] protected LayerMask _enemyLayer; // 감지할 의 레이어 마스크
    [SerializeField] protected float _targettingRange = 5f; // 감지 가능한 거리

    [Header("타겟에 대한 동작")]
    [SerializeField] protected float _rotateSpeed = 10f; // 타겟을 바라보는 회전 속도

    protected int level; // 타워의 레벨

    protected Queue<Collider> _targetQueue; // 타겟을 저장하는 큐
    [SerializeField] protected Collider _currTarget; // 현재 타겟

    [Networked] protected TickTimer _attackTick { get; set; } // 공격 주기를 계산하는 타이머

    // 타워에서 타겟까지의 magnitude를 반환하는 메서드
    private float GetTowerToTargetMagnitude(Vector3 monsterPos, Vector3 targetPos)
    {
        return (monsterPos - targetPos).sqrMagnitude;
    }

    // 타겟이 유효한지 검증하는 메서드
    private bool IsValidTarget()
    {
        float mag = GetTowerToTargetMagnitude(_currTarget.transform.position, transform.position);
        return (mag <= _targettingRange * _targettingRange);
    }

    // 타겟 설정 메서드
    protected virtual void SetTarget()
    {
        // 현재 타겟이 있고, 타겟이 유효하지 않다면 타겟을 null로 비움
        if (_currTarget != null && !IsValidTarget())
        {
            _currTarget = null;
        }

        // 타겟이 없다면 타겟 설정
        if (_currTarget == null)
        {
            Collider[] monsterCollider = Physics.OverlapSphere(
                transform.position,
                _targettingRange,
                _enemyLayer
            );

            float best = float.MaxValue;
            Collider bestMonsterCollider = null;

            // 감지된 몬스터의 콜라이더 중 가장 가까운 콜라이더 판별
            foreach (var mc in monsterCollider)
            {
                Vector3 monsterPos = mc.transform.position;
                float mag = GetTowerToTargetMagnitude(monsterPos, transform.position);

                if (best > mag)
                {
                    best = mag;
                    bestMonsterCollider = mc;
                }
            }

            // 가장 가까운 몬스터가 있다면 타겟 설정
            if (bestMonsterCollider != null)
            {
                _currTarget = bestMonsterCollider;
            }
        }
    }

    // 타겟을 향해 회전하는 메서드
    protected virtual void LookAtTarget() 
    {
        // Y축만 회전
        if (_currTarget != null)
        {
            Vector3 dir = _currTarget.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                var look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, look, _rotateSpeed * Runner.DeltaTime);
            }
        }
    }

    protected virtual void Fire() { }

    #region IInteractableObject 구현

    // 공격 타워를 클릭했을 때 호출되는 메서드
    public void OnClickThisObject()
    {
        var manager = StageManager.Instance;
        if(manager != null)
        {
            manager.PlayerBuilder.ClickAttackTower(this);
        }
    }

    public void OnDragThisObject()
    {

    }
    #endregion
}
