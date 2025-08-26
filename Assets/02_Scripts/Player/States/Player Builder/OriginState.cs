using Fusion.Addons.FSM;
using UnityEngine;

public class OriginState : StateBehaviour
{
    protected override bool CanExitState(StateBehaviour nextState)
    {
        return Machine.StateTime > 3f;
    }

    protected override void OnEnterStateRender()
    {
        
    }
}
