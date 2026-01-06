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
    [SerializeField] private LayerMask _dragDetectLayer;

    [Header("Reference")]
    [SerializeField] private PlayerBuilderUI _builderUI; // UI 참조
    [SerializeField] private DragSystem _dragSystem; // 드래그 시스템 참조
    [SerializeField] private PlayerBuilderMover _builderMover; // 빌더 무버 참조
    [SerializeField] private HexagonGrid _hexagonGrid; // 그리드 참조
    [SerializeField] private Laboratory _laboratory; // 연구실 참조

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

    // 선택된 타워 정보
    private HashSet<Tower> _selectedTowers = new();
    public HashSet<Tower> SelectedTowers => _selectedTowers;
    public int SelectedTowersCount => SelectedTowers.Count;

    // UI와의 상호작용 변수
    public bool IsOpeningLaboratory { get; set; }

    // 상태머신
    public PlayerBuilderStateMachine StateMachine;

    #region 플레이어 빌더 컴포넌트

    private PlayerBuilderTowerBuild _builderTowerBuild; // 타워 건설 도움 컴포넌트
    private PlayerBuilderTowerSell _builderTowerSell; // 타워 판매 도움 컴포넌트
    private PlayerBuilderTowerMove _builderTowerMove; // 타워 이전 도움 컴포넌트

    #endregion

    #region 프로퍼티

    #region 컴포넌트 프로퍼티

    public PlayerBuilderTowerBuild BuilderTowerBuild => _builderTowerBuild;
    public PlayerBuilderTowerMove BuilderTowerMove => _builderTowerMove;
    public HexagonGrid Grid => _hexagonGrid;
    public PlayerBuilderUI BuilderUI => _builderUI;

    #endregion

    #region 필드 프로퍼티

    public bool IsClick => _isClick;
    public Vector2 StartMousePoint => _startMousePoint;
    public Vector2 CurrentMousePoint => _currentMousePoint;
    public float DragThresholdPixel => _dragThresholdPixel;
    public LayerMask DragDetectLayer => _dragDetectLayer;

    #endregion

    #endregion

    #region 월드 상호작용 오브젝트

    public ICanClickObject ClickObject;

    #region 드래그
    public HashSet<ICanDragObject> DragObjectHash = new();
    public HashSet<ICanDragObject> CurrentFrameDetectDragObjectHash = new();
    public List<ICanDragObject> CurrentFrameRemoveDragObjectList = new();
    public Collider[] DragSelectedColliders;
    #endregion

    #endregion

    private void Awake()
    {
        DragSelectedColliders = new Collider[100];
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
    public void PlayerBuilderReferenceInjection(PlayerBuilderUI builderUI, HexagonGrid hexagonGrid, Laboratory laboratory)
    {
        _builderUI = builderUI;  
        _dragSystem = builderUI.DragSystem;
        _hexagonGrid = hexagonGrid;
        _laboratory = laboratory;

        // 연구소 관련 액션 연결
        _builderUI.OnClickLaboratoryButtonAction += OpenLaboratory;
        _laboratory.OnClickLaboratoryObjectAction += OpenLaboratory;

        // 타워 판매 액션 연결
        TryGetComponent(out _builderTowerSell);
        _builderUI.OnClickSellTowerButtonAction += TowerSell;

        // 타워 이전 액션 연결
        TryGetComponent(out _builderTowerMove);
        _builderUI.OnClickMoveTowerButtonAction += ActiveTowerMoveState;

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
        if (DragObjectHash.Count > 0)
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

    // 타워 판매 함수
    public void TowerSell()
    {
        ClickObject = null;
        DragObjectHash.Clear();
        _builderTowerSell.SellTower(_hexagonGrid, SelectedTowers);
    }

    // 공격 타워 선택 함수
    public void AttackTowerSelected(AttackTower tower)
    {
        _selectedTowers.Add(tower);
    }

    // 공격 타워 선택 해쉬를 초기화하는 함수
    public void ResetAttackTowerHashSet()
    {
        _selectedTowers.Clear();
    }

    #endregion

    #region 연구실 로직

    public void OpenLaboratory() => IsOpeningLaboratory = true;

    public void CloseLaboratory() => IsOpeningLaboratory = false;

    #endregion

    #region 상태

    private void ActiveTowerMoveState() => StateMachine.TransitionToState(StateMachine.TowerMoveState);

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
