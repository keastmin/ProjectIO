using UnityEngine;
using UnityEngine.InputSystem;

public class DragSystem : MonoBehaviour
{
    public bool IsCanDrag = true;
    [SerializeField] private DragSection _dragSection;

    private Camera _cam;
    private Vector2 _dragStartScreenPosition = Vector3.zero;
    private Vector2 _dragCurrentScreenPosition = Vector3.zero;

    private void Awake()
    {
        _cam = Camera.main;
        _dragSection.DrawEnd();
    }

    private void OnEnable()
    {
        _dragSection.DrawEnd();
    }

    private void OnDisable()
    {
        _dragSection.DrawEnd();
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (IsCanDrag)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                DragStart(mouse);
                ClickScreen();
            }

            if (mouse.leftButton.isPressed)
            {
                Dragging(mouse);
                DragScreen();
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                DragEnd();
            }
        }
    }

    private void DragStart(Mouse mouse)
    {
        _dragStartScreenPosition = _dragCurrentScreenPosition = mouse.position.ReadValue();
        _dragSection.DrawStart(_dragStartScreenPosition);
    }

    private void Dragging(Mouse mouse)
    {
        _dragCurrentScreenPosition = mouse.position.ReadValue();
        _dragSection.Drawing(_dragCurrentScreenPosition);
    }

    private void DragEnd()
    {
        _dragSection.DrawEnd();
    }

    private void ClickScreen()
    {

    }

    private void DragScreen()
    {

    }
}
