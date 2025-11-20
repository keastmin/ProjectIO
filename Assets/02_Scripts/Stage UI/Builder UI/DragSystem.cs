using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragSystem : MonoBehaviour
{
    public bool IsCanDrag = true;
    [SerializeField] private DragSection _dragSection;
    [SerializeField] private float _planeHeight = -0.03f;

    private Camera _cam;
    private bool _isDragging = false;
    private Vector3 _startWorldPos = Vector3.zero;

    private Vector2 _startMousePos = Vector2.zero;

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
        // 드래그 중이 아니지만 영역이 활성화 되어있으면 영역을 끔
        if (!_isDragging && _dragSection.gameObject.activeSelf)
        {
            DragEnd();
        }

        // 드래그 상태 초기화
        _isDragging = false;
    }

    /// <summary>
    /// 이 함수가 호출되는 동안에는 드래그를 유지하고 드래그 상태가 아닐 때 처음 호출되면 초기화를 진행
    /// </summary>
    /// <param name="mousePos">마우스 위치</param>
    public void Dragging(Vector2 mousePos)
    {
        // 이 함수가 호출되는 시점에 드래그 중이 아니었다면 드래그 시작 활성화
        if (!_isDragging)
        {
            DragStart(mousePos);
        }

        // 드래그를 유지
        _isDragging = true;

        // 드래그 영역 그리기

    }

    // 드래그 시작 초기화
    private void DragStart(Vector2 mousePos)
    {
        _startMousePos = mousePos;
        SetDragSectionActivation(true);
    }

    // 드래그 영역 활성화 여부 설정
    private void SetDragSectionActivation(bool activation)
    {
        _dragSection.gameObject.SetActive(activation);
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

    // 드래그 종료
    private void DragEnd()
    {
        SetDragSectionActivation(false);
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
