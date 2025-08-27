using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class TowerSelectState : BuilderStateBehaviour
{
    private TowerGhost _towerGhost;
    private Vector2Int _index; // 타워를 설치할 셀의 인덱스
    private bool _canBuildTower; // 타워 설치 가능 여부

    protected override void OnEnterStateRender()
    {
        Debug.Log("Tower Select State");

        if (HasInputAuthority)
        {
            InstantiateTowerGhost();
        }
    }

    protected override void OnRender()
    {
        if (HasInputAuthority)
        {
            Vector3 buildPosition;
            Vector2Int cellIndex;
            bool canTowerCraft = SnapshotTowerGhost(out buildPosition, out cellIndex);

            if (Input.GetMouseButtonDown(0) && canTowerCraft)
            {
                SpawnTower(ctx.TowerRef, buildPosition, cellIndex, ctx.Tower.Cost);
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
        return HexagonGridSystem.Instance.GetNearGridIndex(mousePosition);
    }

    // 스냅샷될 셀의 위치를 반환하는 함수
    private Vector3 GetSnapshotPosition(Vector2Int index)
    {
        return HexagonGridSystem.Instance.GetNearGridPosition(index);
    }

    // 타워 설치가 가능한지 판별하는 함수
    private bool IsValidTowerCraft(Vector2Int index, int cost)
    {
        // 해당 셀에 타워 설치가 불가능 하다면 false
        if (!HexagonGridSystem.Instance.IsPointToTowerCraftValid(index)) return false;

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
           
            if(IsValidTowerCraft(cellIndex, ctx.Tower.Cost))
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

    // 타워를 스폰하는 함수
    private void SpawnTower(NetworkPrefabRef towerRef, Vector3 buildPosition, Vector2Int cellIndex, int towerCost)
    {
        RPC_SpawnTower(towerRef, buildPosition, cellIndex, towerCost);

        // 만약 호스트가 아니라면 로컬에서도 셀의 상태를 바꿈
        if(!HasStateAuthority && HasInputAuthority)
        {
            HexagonGridSystem.Instance.ChangeGridCellToTowerState(cellIndex);
        }
    }

    // 호스트에게 코스트만큼 소유한 자원 감소, 타워를 설치한 셀의 상태 변화, 타워 스폰, 빌더의 상태 변화를 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SpawnTower(NetworkPrefabRef towerRef, Vector3 buildPosition, Vector2Int index, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        HexagonGridSystem.Instance.ChangeGridCellToTowerState(index);
        Runner.Spawn(towerRef, buildPosition, Quaternion.identity);
        ctx.OwnerBuilder.IsTowerSelect = false;
    }

    private void DestroyTowerGhost()
    {
        if (_towerGhost) Destroy(_towerGhost.gameObject);
    }
}
