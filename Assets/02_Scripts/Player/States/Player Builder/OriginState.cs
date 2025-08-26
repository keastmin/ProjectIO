using Fusion.Addons.FSM;
using UnityEngine;

public class OriginState : BuilderStateBehaviour
{
    protected override void OnEnterStateRender()
    {
        Debug.Log("Origin State");
    }
}
