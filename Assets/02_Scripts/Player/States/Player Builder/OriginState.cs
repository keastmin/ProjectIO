using Fusion.Addons.FSM;
using UnityEngine;

public class OriginState : BuilderStateBehaviour
{
    bool isClick = false;

    protected override void OnEnterStateRender()
    {
        Debug.Log("Origin State");
        isClick = false;
    }

    protected override void OnFixedUpdate()
    {
        if (HasInputAuthority && isClick)
        {
            ClickInteractableObject();
            isClick = false;
        }
    }

    protected override void OnRender()
    {
        if (HasInputAuthority)
        {
            ClickFlagOn();
        }
    }

    private void ClickFlagOn()
    {
        isClick = isClick | Input.GetMouseButtonDown(0);
    }

    // Interactable Object 컴포넌트가 있는 오브젝트를 클릭했을 때
    private void ClickInteractableObject()
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000f))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractableObejct>();
            if (interactable != null)
            {
                interactable.OnClickThisObject();
            }
        }
    }
}
