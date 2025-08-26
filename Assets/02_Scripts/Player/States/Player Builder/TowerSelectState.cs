using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class TowerSelectState : BuilderStateBehaviour
{
    private TowerGhost _towerGhost;
    private bool _canBuildTower;

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
            SnapshotTowerGhost();

            if (Input.GetMouseButtonDown(0))
            {
                SpawnTower();
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

    private void SnapshotTowerGhost()
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, ctx.EnvironmentalLayer))
        {
            var p = HexagonGridSystem.Instance
                    ? HexagonGridSystem.Instance.GetNearGridPosition(hit.point)
                    : hit.point;

            if (HexagonGridSystem.Instance.IsPointToTowerCraftValid(p) && StageManager.Instance.ResourceSystem.Mineral >= ctx.Tower.Cost)
            {
                _towerGhost.EnableTower();
                _canBuildTower = true;
            }
            else
            {
                _towerGhost.DisableTower();
                _canBuildTower = false;
            }

            _towerGhost.transform.position = p;
        }
    }

    private void SpawnTower()
    {
        if (_canBuildTower)
        {
            RPC_SpawnTower(ctx.TowerRef, _towerGhost.transform.position, ctx.Tower.Cost);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SpawnTower(NetworkPrefabRef towerRef, Vector3 buildPos, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        Runner.Spawn(towerRef, buildPos, Quaternion.identity);
        ctx.OwnerBuilder.IsTowerSelect = false;
    }

    private void DestroyTowerGhost()
    {
        if (_towerGhost) Destroy(_towerGhost.gameObject);
    }
}
