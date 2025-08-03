using UnityEngine;

public class PlayerBuilder : Player
{
    [SerializeField] private Tower _prefabTower;
    [SerializeField] private LayerMask _towerBuildLayerMask;

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            if (HasStateAuthority)
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(ray, out RaycastHit hit, 100f, _towerBuildLayerMask))
                    {
                        Runner.Spawn(_prefabTower, hit.point, Quaternion.identity, Object.InputAuthority);
                    }
                }
            }
        }
    }
}
