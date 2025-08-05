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
                    Runner.Spawn(_prefabTower, data.mousePosition, Quaternion.identity, Object.InputAuthority);
                }
            }
        }
    }
}
