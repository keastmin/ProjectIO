using Fusion;
using Fusion.Addons.FSM;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilder : Player, IStateMachineOwner
{
    public GameObject testObject;

    [Header("States")]
    [SerializeField] private OriginState _originState; // 기본 상태
    [SerializeField] private TowerSelectState _towerSelectState; // 타워 설치 상태
    private StateMachine<StateBehaviour> _builderStateMachine; // 빌더의 State Machine

    [SerializeField] private LayerMask _environmentalLayer;

    [SerializeField] private NetworkPrefabRef _towerRef;
    [SerializeField] private Tower _tower;
    [SerializeField] private TowerGhost _towerGhost;

    // 호스트가 판별하기 위한 변수
    [Networked] public NetworkBool IsTowerSelect { get; set; }

    // 상태 패턴에서 사용할 컨텍스트
    private BuilderContext _builderContext;

    // 컨텍스트에 넘겨줄 프로퍼티
    public NetworkPrefabRef SelcetTowerRef => _towerRef;
    public Tower SelectTower => _tower;
    public TowerGhost SelectTowerGhost => _towerGhost;
    public LayerMask EnvironmentalLayer => _environmentalLayer;

    private BuilderContext PrepareContext()
    {
        if (_builderContext != null) return _builderContext;

        _builderContext = new BuilderContext
        {
            OwnerBuilder = this,
        };

        return _builderContext;
    }

    void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        // 컨텍스트 생성
        var builderContext = PrepareContext();

        // 컨텍스트 연결
        _originState.ctx = builderContext;
        _towerSelectState.ctx = builderContext;

        _builderStateMachine = new StateMachine<StateBehaviour>("Builder State Machine", _originState, _towerSelectState);
        stateMachines.Add(_builderStateMachine);
    }

    public override void FixedUpdateNetwork()
    {
        _builderStateMachine.TryToggleState<TowerSelectState>(IsTowerSelect);
    }

    public void ClickTowerSelectButton(TowerData towerData)
    {
        // 빌더의 입력 권한이 있는 피어만
        if (HasInputAuthority)
        {
            if (!IsTowerSelect) // 타워 선택이 false일 때
            {                
                SetLocalTower(towerData);
                RPC_SelectTower(true);
            }
            else
            {
                RPC_SelectTower(false);
            }
        }
    }

    public void CancelTowerSelect()
    {
        RPC_SelectTower(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SelectTower(NetworkBool isTowerSelect)
    {
        IsTowerSelect = isTowerSelect;
    }

    private void SetLocalTower(TowerData towerData)
    {
        // 로컬 설정
        if (HasInputAuthority)
        {
            _towerRef = towerData.TowerPrefabRef;
            _tower = towerData.Tower;
            _towerGhost = towerData.TowerGhost;
        }
    }
}
