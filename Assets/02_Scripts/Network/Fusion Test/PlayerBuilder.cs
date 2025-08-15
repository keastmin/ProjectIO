using UnityEngine;

public class PlayerBuilder : Player
{
    [SerializeField] private Tower _tower;
    [SerializeField] private LayerMask _environmentalLayer;

    private Tower _snapshotTower;

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data) && HexagonGridSystem.Instance)
        {
            bool mouseButton0 = data.MouseButton0.IsSet(NetworkInputData.MOUSEBUTTON0);
            if (HasStateAuthority && mouseButton0)
            {
                Vector3 towerPosition = HexagonGridSystem.Instance.GetNearGridPosition(data.MousePosition);
                Runner.Spawn(_tower, towerPosition, Quaternion.identity);
            }
        }
    }

    public override void Render()
    {
        if (!HasInputAuthority) return;
        if (_snapshotTower == null) _snapshotTower = Instantiate(_tower);

        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f, _environmentalLayer))
        {
            var p = HexagonGridSystem.Instance
                    ? HexagonGridSystem.Instance.GetNearGridPosition(hit.point)
                    : hit.point;
            _snapshotTower.transform.position = p;
        }
    }
}
