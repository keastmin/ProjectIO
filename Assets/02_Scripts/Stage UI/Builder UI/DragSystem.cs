using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragSystem : MonoBehaviour
{
    public bool IsCanDrag = true;
    [SerializeField] private DragSection _dragSection;
    [SerializeField] private float _planeHeight = -0.03f;

    private Camera _cam;
    private bool _isDragging = false;
    private Vector3 _startWorldPos = Vector3.zero;

    private void Awake()
    {
        _cam = Camera.main;
        DragEnd();
    }

    private void OnEnable()
    {
        DragEnd();
    }

    private void OnDisable()
    {
        DragEnd();
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
            }

            if (mouse.leftButton.isPressed)
            {
                Dragging(mouse);
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                DragEnd();
            }
        }
    }

    private void DragStart(Mouse mouse)
    {
        var mousePos = mouse.position.ReadValue(); // UI 스크린 상의 마우스 위치
        if (TryPojectToCameraPlane(_cam, mousePos, _planeHeight, out _startWorldPos) && !_isDragging)
        {
            _isDragging = true;
            _dragSection.DrawStart(_startWorldPos, mousePos);
            NotifyDragStart();
        }
    }

    private void Dragging(Mouse mouse)
    {
        if (_isDragging)
        {
            var mousePos = mouse.position.ReadValue();
            _dragSection.Drawing(mousePos);
            NotifyDragSectionedTowers(mousePos);
        }
    }

    private void DragEnd()
    {
        if (_isDragging)
        {
            _isDragging = false;
            _dragSection.DrawEnd();
            NotifyDragEnd();
        }
    }

    private Plane MakePlane(float height)
    {
        Vector3 normal = Vector3.up;
        Vector3 pointOnPlane = normal * height;
        return new Plane(normal, pointOnPlane);
    }

    private bool TryPojectToCameraPlane(Camera cam, Vector2 screen, float height, out Vector3 world)
    {
        var plane = MakePlane(height);
        var ray = cam.ScreenPointToRay(screen);
        if(plane.Raycast(ray, out float enter))
        {
            world = ray.GetPoint(enter);
            return true;
        }
        world = default;
        return false;
    }

    // 플레이어 빌더에게 드래그가 시작되었음을 알림
    private void NotifyDragStart()
    {
        if(IsPlayerBuilderExist(out PlayerBuilder builder))
        {
            builder.StartDragSelect();
        }
    }

    // 드래그 영역 안에 들어온 타워들을 플레이어 빌더에게 전송
    private void NotifyDragSectionedTowers(Vector2 mousePos)
    {
        if(IsPlayerBuilderExist(out PlayerBuilder builder))
        {
            // 시작 월드 좌표와 종료 월드 좌표를 플레이어 빌더에게 전달
            TryPojectToCameraPlane(_cam, mousePos, _planeHeight, out Vector3 endWorldPos);
            builder.DraggingSelect(_startWorldPos, endWorldPos);
        }
    }

    // 플레이어 빌더에게 드래그가 종료되었음을 알림
    private void NotifyDragEnd()
    {
        if(IsPlayerBuilderExist(out PlayerBuilder builder))
        {
            builder.EndDragSelect();
        }
    }

    // 플레이어 빌더 인스턴스 존재 여부 확인
    private bool IsPlayerBuilderExist(out PlayerBuilder builder)
    {
        var manager = StageManager.Instance;
        if (manager != null)
        {
            var playerBuilder = manager.PlayerBuilder;
            if(playerBuilder != null)
            {
                builder = playerBuilder;
                return true;
            }
        }
        builder = null;
        return false;
    }
}
