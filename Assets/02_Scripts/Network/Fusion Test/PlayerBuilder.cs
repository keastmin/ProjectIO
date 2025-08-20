using UnityEngine;

public class PlayerBuilder : Player
{
    [SerializeField] private LayerMask _environmentalLayer;

    [SerializeField] private Tower _towerPrefab;
    [SerializeField] private TowerGhost _towerGhostPrefab;

    private bool _isTowerGhostEnabled = false;

    private TowerGhost _towerGhost;

    public override void Spawned()
    {
        _towerGhost = Instantiate(_towerGhostPrefab);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data) && HexagonGridSystem.Instance)
        {
            bool mouseButton0 = data.MouseButton0.IsSet(NetworkInputData.MOUSEBUTTON0);
            Vector3 towerPosition = HexagonGridSystem.Instance.GetNearGridPosition(data.MousePosition);

            if (HasStateAuthority && mouseButton0 && HexagonGridSystem.Instance.IsPointToTowerCraftValid(towerPosition))
            {
                SpawnTower(towerPosition, 5);
            }
        }
    }

    private void Update()
    {
        SnapshotEXTower(5);
    }

    private void SpawnTower(Vector3 towerPosition, int cost)
    {
        if (ResourceSystem.Instance && ResourceSystem.Instance.Mineral >= cost)
        {
            ResourceSystem.Instance.Mineral -= cost;
            Runner.Spawn(_towerPrefab, towerPosition, Quaternion.identity);
        }
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

            if (HexagonGridSystem.Instance.IsPointToTowerCraftValid(p) && ResourceSystem.Instance.Mineral >= cost)
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

    public void OnClickTowerSlotButton(Tower tower, TowerGhost towerGhost)
    {

    }
}
