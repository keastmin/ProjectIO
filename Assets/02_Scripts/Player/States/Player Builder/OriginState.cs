using Fusion.Addons.FSM;
using UnityEngine;

public class OriginState : BuilderStateBehaviour
{
    protected override void OnEnterStateRender()
    {
        Debug.Log("Origin State");
    }

    protected override void OnRender()
    {
        if (HasInputAuthority)
        {
            ClickInteractableObject();
        }
    }

    // Interactable Object 컴포넌트가 있는 오브젝트를 클릭했을 때
    private void ClickInteractableObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var cam = Camera.main;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 5000f))
            {
                var interactable = hit.collider.GetComponentInParent<InteractableObject>();
                if(interactable != null)
                {
                    Debug.Log("클릭");
                }
            }
        }
    }
}
