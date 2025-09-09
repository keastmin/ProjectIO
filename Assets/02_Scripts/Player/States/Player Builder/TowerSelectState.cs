using Fusion;
using Fusion.Addons.FSM;
using System;
using UnityEngine;

public class TowerSelectState : BuilderStateBehaviour
{
    private TowerGhost _towerGhost;

    [Networked] private NetworkBool _towerBuild { get; set; }
    [Networked] private NetworkPrefabRef _prefabRef { get; set; }
    [Networked] private Vector3 _buildPos { get; set; }
    [Networked] private Vector2Int _buildIndex { get; set; }
    [Networked] private int _buildCost { get; set; }

    protected override void OnEnterStateRender()
    {
        Debug.Log("Tower Select State");

        if (HasInputAuthority)
        {
            InstantiateTowerGhost();
        }
    }

    protected override void OnFixedUpdate()
    {
        if (_towerBuild)
        {
            // 호스트라면 타워 스폰
            if (HasStateAuthority)
            {
                SpawnTower(); // 타워 스폰
            }

            // 설치한 위치에 해당하는 그리드 셀을 타워 상태로 변경
            StageManager.Instance.GridSystem.ChangeGridCellToTowerState(_buildIndex);
        }
    }

    protected override void OnRender()
    {
        // 로컬에서 타워가 설치될 위치를 타워 예시로 확인
        if (HasInputAuthority)
        {
            Vector3 buildPosition;
            Vector2Int cellIndex;
            bool canTowerCraft = SnapshotTowerGhost(out buildPosition, out cellIndex);

            if (Input.GetMouseButtonDown(0) && canTowerCraft)
            {
                RequestTowerSpawn(ctx.TowerRef, buildPosition, cellIndex, ctx.Tower.Cost.Mineral);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ctx.OwnerBuilder.CancelTowerSelect();
            }
        }
    }

    protected override void OnExitStateRender()
    {
        if (HasInputAuthority)
        {
            DestroyTowerGhost();
        }
        if (HasStateAuthority)
        {
            InitNetworkValue();
        }
    }

    private void InstantiateTowerGhost()
    {
        _towerGhost = Instantiate(ctx.TowerGhost);
    }

    // 스크린에서 찍고 있는 마우스가 설정한 레이어에 닿았는지 판별하고 위치를 반환하는 함수
    private bool IsValidMouseRay(out Vector3 mousePosition)
    {
        mousePosition = default;
        bool isValid = false;

        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, ctx.EnvironmentalLayer))
        {
            mousePosition = hit.point;
            isValid = true;
        }

        return isValid;
    }

    // 스냅샷될 그리드의 인덱스를 반환하는 함수
    private Vector2Int GetSnapshotIndex(Vector3 mousePosition)
    {
        return StageManager.Instance.GridSystem.GetNearGridIndex(mousePosition);
    }

    // 스냅샷될 셀의 위치를 반환하는 함수
    private Vector3 GetSnapshotPosition(Vector2Int index)
    {
        return StageManager.Instance.GridSystem.GetNearGridPosition(index);
    }

    // 타워 설치가 가능한지 판별하는 함수
    private bool IsValidTowerCraft(Vector2Int index, int cost)
    {
        // 해당 셀에 타워 설치가 불가능 하다면 false
        if (!StageManager.Instance.GridSystem.IsPointToTowerCraftValid(index)) return false;

        // 현재 가지고 있는 자원이 설치할 타워의 필요 자원보다 적다면 false
        if (StageManager.Instance.ResourceSystem.Mineral < cost) return false;

        return true;
    }

    // 타워 예시를 스냅샷하는 함수
    private bool SnapshotTowerGhost(out Vector3 buildPosition, out Vector2Int cellIndex)
    {
        buildPosition = default;
        cellIndex = default;

        Vector3 mouseHitPoint;
        bool isValid = IsValidMouseRay(out mouseHitPoint);
        bool canTowerCraft = false;

        if (isValid)
        {
            cellIndex = GetSnapshotIndex(mouseHitPoint);
            buildPosition = GetSnapshotPosition(cellIndex);

            _towerGhost.transform.position = buildPosition;
           
            if(IsValidTowerCraft(cellIndex, ctx.Tower.Cost.Mineral))
            {
                // 타워 설치가 가능하다면 푸른색으로 변경하고 타워 설치 가능 플래그를 true로 변경
                _towerGhost.EnableTower();
                canTowerCraft = true;
            }
            else
            {
                // 타워 설치가 가능하다면 붉은색
                _towerGhost.DisableTower();
            }
        }

        return canTowerCraft;
    }

    // 호스트에게 타워 스폰 플래그를 활성화 요청
    private void RequestTowerSpawn(NetworkPrefabRef towerRef, Vector3 buildPosition, Vector2Int cellIndex, int towerCost)
    {
        RPC_SetTowerInfo(towerRef, buildPosition, cellIndex, towerCost);
    }

    // 호스트에게 코스트만큼 소유한 자원 감소, 타워를 설치한 셀의 상태 변화, 타워 스폰, 빌더의 상태 변화를 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetTowerInfo(NetworkPrefabRef towerRef, Vector3 buildPosition, Vector2Int index, int cost)
    {
        _towerBuild = true;
        _prefabRef = towerRef;
        _buildPos = buildPosition;
        _buildIndex = index;
        _buildCost = cost;
    }

    // 호스트에게 코스트만큼 소유한 자원을 감소시키고, 타워를 스폰하며 빌더의 상태 변화시키기를 요청
    private void SpawnTower()
    {
        StageManager.Instance.ResourceSystem.Mineral -= _buildCost;
        Runner.Spawn(_prefabRef, _buildPos, Quaternion.identity);
        ctx.OwnerBuilder.IsTowerSelect = false;
    }

    // 로컬로 보고있던 예시 타워를 파괴
    private void DestroyTowerGhost()
    {
        if (_towerGhost) Destroy(_towerGhost.gameObject);
    }

    // 호스트가 타워 설치를 위해 가지고 있던 타워 정보를 초기화
    private void InitNetworkValue()
    {
        _towerBuild = false;
        _buildCost = 0;
        _buildIndex = default;
        _buildPos = default;
        _prefabRef = default;
    }
}
