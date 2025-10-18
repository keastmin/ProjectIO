using Fusion;
using Fusion.Addons.FSM;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBuilder : Player
{
    [SerializeField] private LayerMask _environmentalLayer;

    // 타워
    private Tower _tower;
    private TowerGhost _towerGhost;
    private NetworkPrefabRef _towerRef;
    private Cost _towerCost;
    private Vector3 _towerBuildPosition;
    private Vector2Int _towerBuildIndex;
    private bool _canTowerBuild;

    // UI와의 상호작용 변수
    private bool _isStandByTowerBuild = false;
    public bool IsStandByTowerBuild => _isStandByTowerBuild;

    public override void Render()
    {
        if (HasInputAuthority) // 이 객체에 입력 권한이 있을 때에만 작동
        { 
            if (_isStandByTowerBuild) // 타워 대기중일 때
            {
                // 타워 설치 전 예시 타워 설치
                _canTowerBuild = SnapshotTowerGhost();

                if (Input.GetMouseButtonDown(1))
                {
                    // 우클릭을 하면 타워 설치 대기 취소
                    CancelStandByTowerBuild();
                }
                else if(Input.GetMouseButtonDown(0) && _canTowerBuild && !EventSystem.current.IsPointerOverGameObject())
                {
                    // 좌클릭을 하면 타워 설치
                    RPC_TowerBuild(_towerRef, _towerBuildPosition, _towerCost.Mineral);
                    StageManager.Instance.GridSystem.ChangeGridCellToTowerState(_towerBuildIndex);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    OnClickInteractableObject();
                }
            }
        }
    }

    #region 타워 설치 로직

    // 타워 설치 대기 상태로 돌입
    public void StandByTowerBuild(TowerData towerData)
    {
        // 이미 타워 설치 대기중이었다면
        if(_isStandByTowerBuild)
            CancelStandByTowerBuild();
        
        _isStandByTowerBuild = true;
        _tower = towerData.Tower;
        _towerRef = towerData.TowerPrefabRef;
        _towerGhost = Instantiate(towerData.TowerGhost);
        _towerCost = towerData.Tower.Cost;
        _canTowerBuild = false;
    }

    // 타워 설치 대기 취소
    private void CancelStandByTowerBuild()
    {
        _isStandByTowerBuild = false;
        _tower = null;
        _towerRef = default;
        Destroy(_towerGhost.gameObject);
    }

    // 스크린에서 찍고 있는 마우스가 설정한 레이어에 닿았는지 판별하고 위치를 반환하는 함수
    private bool IsValidMouseRay(out Vector3 mousePosition)
    {
        mousePosition = default;
        bool isValid = false;

        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, _environmentalLayer))
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
    private bool SnapshotTowerGhost()
    {
        Vector3 mouseHitPoint;
        bool isValid = IsValidMouseRay(out mouseHitPoint);
        bool canTowerCraft = false;

        if (isValid)
        {
            _towerBuildIndex = GetSnapshotIndex(mouseHitPoint);
            _towerBuildPosition = GetSnapshotPosition(_towerBuildIndex);

            _towerGhost.transform.position = _towerBuildPosition;

            if (IsValidTowerCraft(_towerBuildIndex, _tower.Cost.Mineral))
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

    // 호스트에게 코스트만큼 소유한 자원을 감소 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_TowerBuild(NetworkPrefabRef towerRef, Vector3 buildPos, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        Runner.Spawn(towerRef, buildPos, Quaternion.identity);
    }

    #endregion

    #region 월드 오브젝트 상호작용

    private void OnClickInteractableObject()
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hit, 5000))
        {
            var inter = hit.collider.GetComponentInParent<IInteractableObejct>();
            if (inter != null)
            {
                inter.OnClickThisObject();
            }
        }
    }

    #endregion

    public override void FixedUpdateNetwork()
    {

    }
}
