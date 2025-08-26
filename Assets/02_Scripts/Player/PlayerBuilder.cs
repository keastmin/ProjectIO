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

    [Networked] public NetworkPrefabRef TowerRef { get; set; }
    [Networked] public int TowerCost { get; set; }
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

            if (TowerRef != default)
            {
                if (mouseButton0 && HexagonGridSystem.Instance.IsPointToTowerCraftValid(towerPosition))
                {
                    if (TowerCost <= StageManager.Instance.ResourceSystem.Mineral)
                    {
                        if (HasStateAuthority)
                        {
                            SpawnTower(towerPosition, TowerCost);
                            TowerRef = default;
                            TowerCost = 0;
                        }
                        if (HasInputAuthority)
                        {
                            Destroy(_towerGhost.gameObject);
                        }
                    }
                }
                else if (mouseButton1)
                {
                    if (HasStateAuthority)
                    {
                        TowerRef = default;
                    }
                    if (HasInputAuthority)
                    {
                        Destroy(_towerGhost.gameObject);
                    }
                }
            }
        }
    }

    private void Update()
    {
        SnapshotEXTower(TowerCost);
    }

    private void SpawnTower(Vector3 towerPosition, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        Runner.Spawn(TowerRef, towerPosition, Quaternion.identity);
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
        var towerCost = towerData.Tower.Cost;
        var towerRef = towerData.TowerPrefabRef;

        RPC_SetNetworkTower(towerRef, towerCost);
        SetLocalTower(towerData.TowerGhost);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetNetworkTower(NetworkPrefabRef towerRef, int cost)
    {
        // 스폰은 호스트가
        if (HasStateAuthority)
        {
            TowerRef = towerRef;
            TowerCost = cost;
        }
    }

    private void SetLocalTower(TowerGhost towerGhost)
    {
        // 미리보기는 로컬에서만
        if (HasInputAuthority)
        {           
            _towerGhost = Instantiate(towerGhost);
        }
    }

    private bool GetNetworkPrefabRef(out NetworkPrefabRef prefabRef, Tower tower)
    {
        prefabRef = default;

        var no = tower.Object;
        if (no == null) 
        {
            Debug.Log("tower에 오브젝트가 없음");
            return false; 
        }

        var typeId = no.NetworkTypeId;
        if (!typeId.IsPrefab)
        {
            Debug.Log("tower에 네트워크 타입 ID가 없음");
            return false;
        }
        var prefabId = typeId.AsPrefabId;

        var guid = StageManager.Instance.Runner.Prefabs.GetGuid(prefabId);
        if (!guid.IsValid)
        {
            Debug.Log("tower에 prefabsId가 없음");
            return false;
        }

        prefabRef = (NetworkPrefabRef)guid;
        return prefabRef.IsValid;
    }
}
