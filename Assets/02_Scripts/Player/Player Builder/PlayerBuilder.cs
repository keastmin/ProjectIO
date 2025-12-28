using Fusion;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerBuilderTowerBuild), typeof(PlayerBuilderGridInteractor))]
public class PlayerBuilder : Player
{
    [Header("Camera")]
    [SerializeField] private float _cameraBorderThickness = 10f;
    [SerializeField] private float _cameraMoveSpeed = 10f;
    [SerializeField] private float _cameraMaxZoomDistance = 11f;
    [SerializeField] private float _cameraWheelPerZoomDistance = 10f;
    [SerializeField] private float _zoomSmoothTime = 0.12f; // 작을수록 더 빠르게 붙음

    [Header("Drag")]
    [SerializeField] private float _dragThresholdPixel = 5f; 
    [SerializeField] private float _dragDetectorHeight = 1f;
    [SerializeField] private float _dragDetectBoxThickness = 0.1f;
    [SerializeField] private LayerMask _dragDetectLayer;

    [Header("Reference")]
    [SerializeField] private PlayerBuilderUI _builderUI; // UI 참조
    [SerializeField] private DragSystem _dragSystem; // 드래그 시스템 참조
    [SerializeField] private PlayerBuilderMover _builderMover; // 빌더 무버 참조
    [SerializeField] private HexagonGrid _hexagonGrid; // 그리드 참조

    [Space(10)]

    [SerializeField] private CinemachineCamera _cineCam;
    [SerializeField] private LayerMask _environmentalLayer;

    public LayerMask EnvironmentalLayer => _environmentalLayer;

    // 카메라 줌인/줌아웃 한계 변수
    private float _camZoomDistance = 0f; // 0=초기 위치, + = 전방으로 이동한 누적 거리
    private float _zoomTargetDistance = 0f;                  // 목표 전방 이동량(0~_cameraMaxZoomDistance)
    private float _zoomVel = 0f;                             // SmoothDamp용 내부 속도 캐시

    // 드래그 정보
    private bool _isClick = false;
    private Vector2 _startMousePoint = Vector2.zero;
    private Vector2 _currentMousePoint = Vector2.zero;
    private int _dragObjectCount = 0;

    // 선택된 타워 정보
    private Collider[] _dragSelectedColliders;
    private HashSet<AttackTower> _selectedAttackTower = new();
    public HashSet<AttackTower> SelectedAttackTower => _selectedAttackTower;
    public int SelectedAttackTowerCount => _selectedAttackTower.Count;

    // UI와의 상호작용 변수
    public bool IsOpeningLaboratory { get; set; }

    // 상태머신
    public PlayerBuilderStateMachine StateMachine;
    public bool IsSelectTower { get; set; }

    #region 컴포넌트

    private PlayerBuilderTowerBuild _builderTowerBuild; // 타워 건설 도움 컴포넌트

    #endregion

    #region 프로퍼티

    #region 컴포넌트 프로퍼티

    public PlayerBuilderTowerBuild BuilderTowerBuild => _builderTowerBuild;
    public HexagonGrid Grid => _hexagonGrid;

    #endregion

    public PlayerBuilderUI BuilderUI => _builderUI;
    public bool IsClick => _isClick;
    public Vector2 StartMousePoint => _startMousePoint;
    public Vector2 CurrentMousePoint => _currentMousePoint;
    public float DragThresholdPixel => _dragThresholdPixel;
    public float DragDetectHeight => _dragDetectorHeight;

    #endregion

    #region 월드 상호작용 오브젝트

    public ICanClickObject ClickObject;
    public HashSet<ICanDragObject> DragObjectHash = new();

    #endregion

    private void Awake()
    {
        _dragSelectedColliders = new Collider[100];
        StateMachine = new PlayerBuilderStateMachine(this);
        InitializeReference();
    }

    private void Start()
    {
        StateMachine.InitStateMachine();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public override void Render()
    {
        StateMachine.Render();
    }

    public override void FixedUpdateNetwork()
    {
        StateMachine.NetworkFixedUpdate();
    }

    private void LateUpdate()
    {
        StateMachine.LateUpdate();
    }

    #region 초기화 로직

    private void InitializeReference()
    {
        TryGetComponent(out _builderMover);
    }

    // 외부에서 참조를 주입하는 함수
    public void PlayerBuilderReferenceInjection(PlayerBuilderUI builderUI, HexagonGrid hexagonGrid)
    {
        _builderUI = builderUI;  
        _dragSystem = builderUI.DragSystem;
        _hexagonGrid = hexagonGrid;
        
        // 타워 건설 관련 컴포넌트 초기화
        TryGetComponent(out _builderTowerBuild);
        _builderTowerBuild.Init(_builderUI);
    }

    #endregion

    #region 클릭

    // 월드를 향해 좌클릭을 눌렀을 때 이미 선택된 오브젝트들을 초기화 하고 새로운 정보 수집
    public void ClickLeftMouseDownOnWorld()
    {
        // 이미 클릭된 오브젝트가 있을 때
        if (ClickObject != null)
        {
            // 이미 등록된 클릭 오브젝트 해제
            ClickObject.OnCancelClickThisObject();
            ClickObject = null;
        }

        // 이미 선택된 드래그 오브젝트가 있을 때
        if (_dragObjectCount > 0)
        {
            foreach(var dragObj in DragObjectHash)
            {
                dragObj.OnDragOverThisObject();
            }
            DragObjectHash.Clear();
        }

        // 선택된 공격타워 해쉬 초기화
        ResetAttackTowerHashSet();

        // 새로운 오브젝트 수집 시도
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 5000))
        {
            var inter = hit.collider.GetComponentInParent<ICanClickObject>();
            if (inter != null)
            {
                ClickObject = inter;
                ClickObject.OnLeftMouseDownThisObject();
            }
        }
    }

    // 월드를 향해 좌클릭을 뗐을 때
    public void ClickLeftMouseUpOnWorld()
    {
        if (ClickObject != null)
            ClickObject.OnLeftMouseUpThisObject();
    }

    #endregion

    #region 드래그

    /// <summary>
    /// 클릭 여부 설정 함수
    /// </summary>
    /// <param name="isClick">클릭 여부</param>
    public void SetClickValue(bool isClick)
    {
        _isClick = isClick;
        _startMousePoint = Input.mousePosition;
        _currentMousePoint = _startMousePoint;
    }

    /// <summary>
    /// 현재 마우스 위치 설정 함수
    /// </summary>
    /// <param name="mousePos">현재 마우스 위치</param>
    public void SetCurrentMousePoint(Vector2 mousePos)
    {
        _currentMousePoint = mousePos;
    }

    public void DragStart()
    {
        if (_dragSystem == null) return;

        _dragSystem.DragStart();
        Dragging();
    }

    public void Dragging()
    {
        if (_dragSystem == null) return;

        _dragSystem.Dragging(_startMousePoint, _currentMousePoint);
    }

    /// <summary>
    /// 드래그를 종료하는 함수
    /// </summary>
    public void DragEnd()
    {
        if (_dragSystem == null) return;

        _dragSystem.DragEnd();
    }

    // 드래그 동안 영역 내의 콜라이더 수집
    public void DraggingCollectCollider(float minX, float maxX, float minY, float maxY)
    {
        Camera cam = Camera.main;
        if (!cam) return;

        // 0) 픽셀 사각형 정리(스왑 + 카메라 뷰포트로 클램프)
        if (minX > maxX) (minX, maxX) = (maxX, minX);
        if (minY > maxY) (minY, maxY) = (maxY, minY);
        Rect vp = cam.pixelRect;
        minX = Mathf.Clamp(minX, vp.xMin, vp.xMax);
        maxX = Mathf.Clamp(maxX, vp.xMin, vp.xMax);
        minY = Mathf.Clamp(minY, vp.yMin, vp.yMax);
        maxY = Mathf.Clamp(maxY, vp.yMin, vp.yMax);
        if (maxX <= minX || maxY <= minY) return; // 화면 밖이면 종료

        // 1) 드래그 사각형 4모서리를 near/far로 언프로젝션(월드 8점)
        float nearZ = Mathf.Max(cam.nearClipPlane, 0.03f);
        float farZ = 30f; // 필요에 맞게 조절; 검증 단계에선 cam.farClipPlane 권장

        Vector3 Near(float x, float y) => cam.ScreenToWorldPoint(new Vector3(x, y, nearZ));
        Vector3 Far(float x, float y) => cam.ScreenToWorldPoint(new Vector3(x, y, farZ));

        // 인덱스 규칙: 0=BL, 1=TL, 2=TR, 3=BR  (CalculateFrustumCorners와 동일 규칙로 맞춤)
        Vector3[] n = new Vector3[4];
        Vector3[] f = new Vector3[4];
        n[0] = Near(minX, minY); n[1] = Near(minX, maxY); n[2] = Near(maxX, maxY); n[3] = Near(maxX, minY);
        f[0] = Far(minX, minY); f[1] = Far(minX, maxY); f[2] = Far(maxX, maxY); f[3] = Far(maxX, minY);

        // 2) 8점 AABB (브로드페이즈 후보 수집용)
        Bounds aabb = new Bounds(n[0], Vector3.zero);
        aabb.Encapsulate(n[1]); aabb.Encapsulate(n[2]); aabb.Encapsulate(n[3]);
        aabb.Encapsulate(f[0]); aabb.Encapsulate(f[1]); aabb.Encapsulate(f[2]); aabb.Encapsulate(f[3]);

        // 3) 수동 평면 구성(프러스텀 '안쪽이 항상 +측'이 되도록 정렬)
        Vector3 centerNear = (n[0] + n[1] + n[2] + n[3]) * 0.25f;
        Vector3 centerFar = (f[0] + f[1] + f[2] + f[3]) * 0.25f;
        Vector3 insidePoint = (centerNear + centerFar) * 0.5f;

        Plane MakeOriented(Vector3 a, Vector3 b, Vector3 c)
        {
            var p = new Plane(a, b, c);              // 노멀 = (b-a) × (c-a)
            if (!p.GetSide(insidePoint)) p.Flip();   // insidePoint가 -측이면 뒤집어 +측으로
            return p;
        }

        Plane[] planes = new Plane[]
        {
        // Near / Far (TL→TR→BR / BR→TR→TL)
        MakeOriented(n[1], n[2], n[3]),
        MakeOriented(f[3], f[2], f[1]),

        // Left / Right / Top / Bottom
        MakeOriented(n[0], n[1], f[1]), // BL(n)→TL(n)→TL(f)
        MakeOriented(n[3], n[2], f[2]), // BR(n)→TR(n)→TR(f)
        MakeOriented(n[1], n[2], f[2]), // TL(n)→TR(n)→TR(f)
        MakeOriented(n[3], n[0], f[0]), // BR(n)→BL(n)→BL(f)
        };

        // 4) 후보 수집 (NonAlloc 권장: 재사용 버퍼 _dragSelectedColliders)
        int candidateCount = Physics.OverlapBoxNonAlloc(
            aabb.center, aabb.extents, _dragSelectedColliders,
            Quaternion.identity, _dragDetectLayer, QueryTriggerInteraction.Collide
        );

        // 5) 정밀 필터: 프러스텀 평면으로 통과만 in-place 압축
        _dragObjectCount = 0;
        for (int i = 0; i < candidateCount; i++)
        {
            var col = _dragSelectedColliders[i];
            if (!col) continue;
            if (GeometryUtility.TestPlanesAABB(planes, col.bounds))
                _dragSelectedColliders[_dragObjectCount++] = col;
        }
        for (int i = _dragObjectCount; i < _dragSelectedColliders.Length; i++) _dragSelectedColliders[i] = null;

        // 0) 현재 프레임에 감지된 드래그 가능한 오브젝트 집합 만들기
        //    (재사용 캐시로 두면 좋음: 클래스 필드로 HashSet<ICanDragObject> _currentDragSet; List<ICanDragObject> _toRemove;)
        var currentDragSet = new HashSet<ICanDragObject>();
        for (int i = 0; i < _dragObjectCount; i++)
        {
            var d = _dragSelectedColliders[i]?.GetComponent<ICanDragObject>();
            if (d != null) currentDragSet.Add(d);
        }

        // 1) 새로 들어온 것들 추가 + 콜백
        foreach (var d in currentDragSet)
        {
            if (DragObjectHash.Add(d)) // 새로 추가된 경우에만
                d.OnDragSelectedThisObject();
        }

        // 2) 나간 것들 모아서 제거 + 콜백
        var toRemove = new List<ICanDragObject>();
        foreach (var d in DragObjectHash)        // 여기서는 "읽기만" 하고
        {
            if (!currentDragSet.Contains(d))
                toRemove.Add(d);                  // 지울 목록에만 담아둠
        }
        foreach (var d in toRemove)               // 열거가 끝난 뒤 실제 제거
        {
            d.OnDragOverThisObject();
            DragObjectHash.Remove(d);
        }

        Debug.Log($"DragFrustum collect: candidates={candidateCount}, inside={_dragObjectCount}");

        // =====(옵션) 시각 디버그: 프러스텀 와이어 + AABB=====
        float dur = 0f; bool depthTest = true;
        void L(Vector3 a, Vector3 b, Color c) => Debug.DrawLine(a, b, c, dur, depthTest);

        // near/far 사각형
        L(n[0], n[1], Color.cyan); L(n[1], n[2], Color.cyan);
        L(n[2], n[3], Color.cyan); L(n[3], n[0], Color.cyan);
        L(f[0], f[1], Color.blue); L(f[1], f[2], Color.blue);
        L(f[2], f[3], Color.blue); L(f[3], f[0], Color.blue);
        // 엣지
        L(n[0], f[0], Color.green); L(n[1], f[1], Color.green);
        L(n[2], f[2], Color.green); L(n[3], f[3], Color.green);
        // AABB
        Vector3 c0 = aabb.center, e = aabb.extents;
        Vector3 V(float sx, float sy, float sz) => c0 + new Vector3(e.x * sx, e.y * sy, e.z * sz);
        var v000 = V(-1, -1, -1); var v100 = V(1, -1, -1); var v110 = V(1, 1, -1); var v010 = V(-1, 1, -1);
        var v001 = V(-1, -1, 1); var v101 = V(1, -1, 1); var v111 = V(1, 1, 1); var v011 = V(-1, 1, 1);
        L(v000, v100, Color.yellow); L(v100, v110, Color.yellow); L(v110, v010, Color.yellow); L(v010, v000, Color.yellow);
        L(v001, v101, Color.yellow); L(v101, v111, Color.yellow); L(v111, v011, Color.yellow); L(v011, v001, Color.yellow);
        L(v000, v001, Color.yellow); L(v100, v101, Color.yellow); L(v110, v111, Color.yellow); L(v010, v011, Color.yellow);
    }

    // 드래그 오브젝트들의 확정 함수 호출
    public void DragObjectsCompleteCall()
    {
        foreach(var dragObj in DragObjectHash)
        {
            dragObj.OnDragCompleteThisObject();
        }
    }

    #endregion

    #region 월드 오브젝트 상호작용

    // 공격 타워 선택 함수
    public void AttackTowerSelected(AttackTower tower)
    {
        _selectedAttackTower.Add(tower);
    }

    // 공격 타워 선택 해쉬를 초기화하는 함수
    public void ResetAttackTowerHashSet()
    {
        _selectedAttackTower.Clear();
    }

    #endregion

    #region 연구실 로직

    public void OpenLaboratory(bool isOpening)
    {
        IsOpeningLaboratory = isOpening;
    }

    #endregion

    #region 카메라 제어

    // 시네머신 카메라 참조 할당
    public void SetCinemachineRef(CinemachineCamera cineCam)
    {
        _cineCam = cineCam;
        _camZoomDistance = 0f;
        _zoomTargetDistance = 0f;
        _zoomVel = 0f;
    }

    // 자기 자신이 빌더라면 마우스로 화면 제어
    public void BuilderCamMove()
    {
        float border = _cameraBorderThickness;         // 화면 끝 감지 영역(픽셀)
        float moveSpeed = _cameraMoveSpeed;      // 카메라 이동 속도(유닛/초)
        Vector3 moveDir = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 왼쪽
        if (mousePos.x < border)
            moveDir += Vector3.left;
        // 오른쪽
        if (mousePos.x > screenWidth - border)
            moveDir += Vector3.right;
        // 아래쪽
        if (mousePos.y < border)
            moveDir += Vector3.back; // 월드 Z-축이 앞으로라면 back
        // 위쪽
        if (mousePos.y > screenHeight - border)
            moveDir += Vector3.forward; // 월드 Z-축이 앞으로라면 forward

        if (moveDir != Vector3.zero)
        {
            // 월드 공간에서 카메라 이동 (XZ 플레인 기준)
            Vector3 move = moveDir.normalized * moveSpeed * Time.deltaTime;
            _cineCam.transform.position += new Vector3(move.x, 0f, move.z);
        }

        // 전방 줌: 휠은 "목표 거리"만 바꾸고, 이동은 부드럽게 보간
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        // 현재 전방과 줌 원점(현재값 기준)
        var fwd = _cineCam.transform.forward.normalized;
        Vector3 zoomOrigin = _cineCam.transform.position - fwd * _camZoomDistance;

        // 휠 한 칸당 전방으로 이동할 목표 거리 갱신 (deltaTime 곱하지 않음)
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            _zoomTargetDistance += scroll * _cameraWheelPerZoomDistance;
            _zoomTargetDistance = Mathf.Clamp(_zoomTargetDistance, 0f, _cameraMaxZoomDistance); // 최소/최대 제한
        }

        // 현재값 → 목표값 부드럽게 근사
        _camZoomDistance = Mathf.SmoothDamp(
            _camZoomDistance,
            _zoomTargetDistance,
            ref _zoomVel,
            _zoomSmoothTime
        );

        // 최종 위치 = 원점 + 전방 * 현재 줌 거리
        _cineCam.transform.position = zoomOrigin + fwd * _camZoomDistance;
    }

    #endregion
}
