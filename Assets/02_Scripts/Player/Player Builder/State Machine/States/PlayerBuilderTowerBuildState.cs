using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBuilderTowerBuildState : IPlayerState
{
    private PlayerBuilder _player;

    private Tower _tower;
    private TowerGhost _towerGhost;
    private NetworkPrefabRef _towerRef;
    private Cost _towerCost;
    private Vector3 _towerBuildPosition;
    private Vector2Int _towerBuildIndex;
    private bool _canTowerBuild;

    public PlayerBuilderTowerBuildState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        var manager = StageManager.Instance;
        if(manager != null)
        {
            var uiController = manager.UIController;
            if(uiController != null)
            {
                uiController.BuilderUI.ActivationTowerBuildUI(true);
            }
        }

        _canTowerBuild = false;
        _tower = _player.PTowerData.Tower;
        _towerGhost = Object.Instantiate(_player.PTowerData.TowerGhost);
        _towerRef = _player.PTowerData.TowerPrefabRef;
        _towerCost = _player.PTowerData.Tower.Cost;

        _player.DragOff();
    }

    public void Update()
    { 
        // 타워 설치 전 예시 타워 설치
        _canTowerBuild = SnapshotTowerGhost();

        if (Input.GetMouseButtonDown(0) && _canTowerBuild && !EventSystem.current.IsPointerOverGameObject())
        {
            // 좌클릭을 하면 타워 설치
            _player.RPC_TowerBuild(_towerRef, _towerBuildPosition, _towerCost.Mineral);
            StageManager.Instance.GridSystem.ChangeGridCellToTowerState(_towerBuildIndex);
        }

        TransitionTo();
    }

    public void LateUpdate()
    {
        _player.BuilderCamMove();
    }

    public void Render()
    {

    }

    public void NetworkFixedUpdate()
    {

    }

    public void Exit()
    {
        CancelTowerBuild();
    }

    private void TransitionTo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }

    private void CancelTowerBuild()
    {
        Object.Destroy(_towerGhost.gameObject);
        _player.IsStandByTowerBuild = false;
        var manager = StageManager.Instance;
        if (manager != null)
        {
            var uiController = manager.UIController;
            if (uiController != null)
            {
                uiController.BuilderUI.ActivationTowerBuildUI(false);
            }
        }
    }

    // 스크린에서 찍고 있는 마우스가 설정한 레이어에 닿았는지 판별하고 위치를 반환하는 함수
    private bool IsValidMouseRay(out Vector3 mousePosition)
    {
        mousePosition = default;
        bool isValid = false;

        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, _player.EnvironmentalLayer))
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
}
