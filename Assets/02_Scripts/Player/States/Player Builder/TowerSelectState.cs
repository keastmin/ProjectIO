using Fusion.Addons.FSM;
using UnityEngine;

public class TowerSelectState : StateBehaviour
{
    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime > 1f)
        {
            Machine.TryDeactivateState(StateId);
        }
    }

    protected override void OnEnterStateRender()
    {
        Debug.Log("Tower Select State");
    }
}
