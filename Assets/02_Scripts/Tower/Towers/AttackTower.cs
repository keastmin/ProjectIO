using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class AttackTower : Tower, ICanClickObject, ICanDragObject
{
    [Header("공격")]
    [SerializeField] protected float _attackSpeed = 1f; // 공격 주기
    [SerializeField] protected Transform _attackPosition; // 공격 시작 지점

    [Header("타겟 지정")]
    [SerializeField] protected LayerMask _enemyLayer; // 감지할 의 레이어 마스크
    [SerializeField] protected float _targettingRange = 5f; // 감지 가능한 거리

    [Header("타겟에 대한 동작")]
    [SerializeField] protected float _rotateSpeed = 10f; // 타겟을 바라보는 회전 속도

    [Header("선택 시 표시")]
    [SerializeField] protected GameObject _selectedChecker; // 타워 선택 시 표시 오브젝트
    [SerializeField] protected Image _selectedImage; // 타워 선택 시 UI에 표시될 이미지

    [Header("속성 부여")]
    [SerializeField] protected TowerPropertiesType _propertiesType = TowerPropertiesType.None; // 부여된 속성
    [SerializeField] private GameObject _flameEffect;
    [SerializeField] private GameObject _blitzEffect;
    [SerializeField] private GameObject _bioEffect;

    protected bool _isSelectedTower = false; // 타워 선택 여부

    public bool IsSelectedTower
    {
        get { return _isSelectedTower; }
        set
        {
            _isSelectedTower = value;
            ChangeSelectTowerMaterial(_selectedChecker, _isSelectedTower);
        }
    }

    protected Queue<Collider> _targetQueue; // 타겟을 저장하는 큐
    [SerializeField] protected Collider _currTarget; // 현재 타겟

    [Networked] protected TickTimer _attackTick { get; set; } // 공격 주기를 계산하는 타이머

    private void Awake()
    {
        _selectedChecker.SetActive(false);
        _flameEffect.SetActive(false);
        _blitzEffect.SetActive(false);
        _bioEffect.SetActive(false);
    }

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

    #region 속성 부여

    /// <summary>
    /// 타워에 속성을 부여함
    /// </summary>
    /// <param name="increaseType">확률이 증가한 타입</param>
    /// <param name="increaseAmount">확률 증가량(10%와 같은 0 ~ 100% 사이 값)</param>
    /// <returns>부여된 속성</returns>
    public TowerPropertiesType AddProperties(TowerPropertiesType increaseType, float increaseAmount = 0f)
    {
        // 이미 속성이 부여되어 있으면 즉시 반환
        if(_propertiesType != TowerPropertiesType.None)
        {
            return _propertiesType;
        }

        float flameProb = 1f / 3f; // 화염 속성 확률
        float blitzProb = 1f / 3f; // 전격 속성 확률
        float bioProb = 1f / 3f; // 생화학 속성 확률

        // 증가 확률이 있을 경우
        if (increaseType != TowerPropertiesType.None)
        { 
            // 증가량 대입
            switch (increaseType)
            {
                case TowerPropertiesType.Flame: flameProb += increaseAmount / 100f; break;
                case TowerPropertiesType.Blitz: blitzProb += increaseAmount / 100f; break;
                case TowerPropertiesType.Biochemical: bioProb += increaseAmount / 100f; break;
            }

            // 남은 확률 분배
            float remain = 1f - (increaseType == TowerPropertiesType.Flame ? flameProb :
                                (increaseType == TowerPropertiesType.Blitz) ? blitzProb : bioProb);

            float other = remain / 2f;

            if (increaseType != TowerPropertiesType.Flame) flameProb = other;
            if (increaseType != TowerPropertiesType.Blitz) blitzProb = other;
            if (increaseType != TowerPropertiesType.Biochemical) bioProb = other;
        }

        // 속성 부여
        var type = RandomPickProperties(flameProb, blitzProb, bioProb);
        RPC_SetLocalTowerProperties(type);
        RPC_ActivePropertiesEffect(type);

        Debug.Log("속성 부여 완료");

        return _propertiesType;
    }

    #endregion

    #region ICanClickObject 구현

    // 공격 타워를 눌렀을 때 호출되는 메서드
    public void OnLeftMouseDownThisObject()
    {
        ChangeSelectValue(true);
    }

    // 공격 타워를 클릭했을 때 호출되는 메서드
    public void OnLeftMouseUpThisObject()
    {
        var manager = StageManager.Instance;
        if(manager != null)
        {
            // 빌더의 타워 선택을 함수를 호출하여 자신을 선택된 타워로 넘겨줌
            manager.PlayerBuilder.TowerSelected(this);
        }
    }

    // 클릭 취소
    public void OnCancelClickThisObject()
    {
        ChangeSelectValue(false);
    }

    #endregion

    #region ICanDragObject 구현

    public void OnDragSelectedThisObject()
    {
        ChangeSelectValue(true);
    }

    public void OnDragOverThisObject()
    {
        ChangeSelectValue(false);
    }

    public void OnDragCompleteThisObject()
    {
        var manager = StageManager.Instance;
        if (manager != null)
        {
            // 빌더의 타워 선택을 함수를 호출하여 자신을 선택된 타워로 넘겨줌
            manager.PlayerBuilder.TowerSelected(this);
        }
    }

    #endregion

    #region 타워 선택 메서드

    // 타워 선택 토글 메서드
    public void ChangeSelectValue(bool selected)
    {
        IsSelectedTower = selected;
    }

    // 타워 선택시 머티리얼 변경 메서드
    private void ChangeSelectTowerMaterial(GameObject checker, bool selected)
    {
        checker.SetActive(selected);
        
        Debug.Log("타워 선택 표시" + checker.activeSelf);
    }

    #endregion

    #region 헬퍼

    // 확률에 따라 부여할 속성 랜덤 뽑기
    private TowerPropertiesType RandomPickProperties(float flameProb, float blitzProb, float bioProb)
    {
        // 모든 확률의 합: 1이거나 그의 근삿값
        float sum = flameProb + blitzProb + bioProb;

        // 정규화
        flameProb /= sum; blitzProb /= sum; bioProb /= sum;

        // 확률에 따라 속성 리턴
        float r = Random.value; // [0, 1)
        if (r < flameProb) return TowerPropertiesType.Flame;
        if (r < flameProb + blitzProb) return TowerPropertiesType.Blitz;
        return TowerPropertiesType.Biochemical;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetLocalTowerProperties(TowerPropertiesType type)
    {
        _propertiesType = type;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ActivePropertiesEffect(TowerPropertiesType type)
    {
        if (type == TowerPropertiesType.None)
        {
            _flameEffect.SetActive(false);
            _blitzEffect.SetActive(false);
            _bioEffect.SetActive(false);
        }
        else if (type == TowerPropertiesType.Flame)
        {
            _flameEffect.SetActive(true);
        }
        else if (type == TowerPropertiesType.Blitz)
        {
            _blitzEffect.SetActive(true);
        }
        else if (type == TowerPropertiesType.Biochemical)
        {
            _bioEffect.SetActive(true);
        }
    }

    #endregion
}
