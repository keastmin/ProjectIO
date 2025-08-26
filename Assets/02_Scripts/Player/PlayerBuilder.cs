using Fusion;
using Fusion.Addons.FSM;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilder : Player, IStateMachineOwner
{
    [Header("States")]
    [SerializeField] private OriginState _originState; // 기본 상태
    [SerializeField] private TowerSelectState _towerSelectState; // 타워 설치 상태
    private StateMachine<StateBehaviour> _builderStateMachine; // 빌더의 State Machine

    [SerializeField] private LayerMask _environmentalLayer;

    [SerializeField] private TowerData _towerData;
    [SerializeField] private Tower _tower;
    [SerializeField] private TowerGhost _towerGhost;

    void IStateMachineOwner.CollectStateMachines(List<IStateMachine> stateMachines)
    {
        _builderStateMachine = new StateMachine<StateBehaviour>("Builder State Machine", _originState, _towerSelectState);
        stateMachines.Add(_builderStateMachine);
    }

    // 이 객체가 스폰될 때 수행되는 함수
    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
    {
        _builderStateMachine.TryActivateState(_towerSelectState);
        
        if (GetInput(out NetworkInputData data) && HexagonGridSystem.Instance)
        {
            bool mouseButton0 = data.MouseButton0.IsSet(NetworkInputData.MOUSEBUTTON0);
            bool mouseButton1 = data.MouseButton1.IsSet(NetworkInputData.MOUSEBUTTON1);
            Vector3 towerPosition = HexagonGridSystem.Instance.GetNearGridPosition(data.MousePosition);

            if (_tower)
            {
                if (mouseButton0 && HexagonGridSystem.Instance.IsPointToTowerCraftValid(towerPosition))
                {
                    if (_tower.Cost <= StageManager.Instance.ResourceSystem.Mineral)
                    {
                        if (HasStateAuthority)
                        {
                            SpawnTower(towerPosition, _tower.Cost);
                            _tower = null;
                        }
                        if (HasInputAuthority)
                        {
                            _towerData = null;
                            Destroy(_towerGhost.gameObject);
                        }
                    }
                }
                else if (mouseButton1)
                {
                    if (HasStateAuthority)
                    {
                        _tower = null;
                    }
                    if (HasInputAuthority)
                    {
                        _towerData = null;
                        Destroy(_towerGhost.gameObject);
                    }
                }
            }
        }
    }

    private void Update()
    {
        SnapshotEXTower(5);
    }

    private void SpawnTower(Vector3 towerPosition, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        Runner.Spawn(_tower, towerPosition, Quaternion.identity);
    }

    // 타워 예시를 스냅샷 해보는 메서드
    private void SnapshotEXTower(int cost)
    {
        if (!HasInputAuthority) return;
        if (!HexagonGridSystem.Instance) return;
        if (!_towerGhost) return;

        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, _environmentalLayer))
        {
            var p = HexagonGridSystem.Instance
                    ? HexagonGridSystem.Instance.GetNearGridPosition(hit.point)
                    : hit.point;

            if (HexagonGridSystem.Instance.IsPointToTowerCraftValid(p) && StageManager.Instance.ResourceSystem.Mineral >= cost)
            {
                _towerGhost.EnableTower();
            }
            else
            {
                _towerGhost.DisableTower();
            }

            _towerGhost.transform.position = p;
        }
    }

    public void SetTowerData(TowerData towerData)
    {
        _towerData = towerData;
        SetTower(_towerData.Tower);
        SetTowerGhost(_towerData.TowerGhost);
    }

    private void SetTower(Tower tower)
    {
        // 스폰은 호스트가
        if (HasStateAuthority)
        {
            _tower = tower;
        }
    }

    private void SetTowerGhost(TowerGhost towerGhost)
    {
        // 미리보기는 로컬에서만
        if (HasInputAuthority)
        {
            _towerGhost = Instantiate(towerGhost);
        }
    }
}
