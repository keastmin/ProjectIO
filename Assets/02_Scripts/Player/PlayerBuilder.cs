using Fusion;
using UnityEngine;

public class PlayerBuilder : Player
{
    [SerializeField] private LayerMask _environmentalLayer;

    [SerializeField] private TowerData _towerData;
    [SerializeField] private Tower _tower;
    [SerializeField] private TowerGhost _towerGhost;

    public override void Spawned()
    {

    }

    public override void FixedUpdateNetwork()
    {
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
