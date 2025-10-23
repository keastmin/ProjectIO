using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBuilder : Player
{
    [Header("Camera")]
    [SerializeField] private float _cameraBorderThickness = 10f;
    [SerializeField] private float _cameraMoveSpeed = 10f;
    [SerializeField] private float _cameraMaxZoomDistance = 11f;
    [SerializeField] private float _cameraWheelPerZoomDistance = 10f;
    [SerializeField] private float _zoomSmoothTime = 0.12f; // 작을수록 더 빠르게 붙음

    [SerializeField] private CinemachineCamera _cineCam;
    [SerializeField] private LayerMask _environmentalLayer;

    public LayerMask EnvironmentalLayer => _environmentalLayer;

    // 카메라 줌인/줌아웃 한계 변수
    private float _camZoomDistance = 0f; // 0=초기 위치, + = 전방으로 이동한 누적 거리
    private float _zoomTargetDistance = 0f;                  // 목표 전방 이동량(0~_cameraMaxZoomDistance)
    private float _zoomVel = 0f;                             // SmoothDamp용 내부 속도 캐시

    // 설치할 타워
    private TowerData _pTowerData;
    public TowerData PTowerData => _pTowerData;

    // 선택된 타워 정보
    private AttackTower _selectedAttackTower;
    public AttackTower SelectedAttackTower => _selectedAttackTower;

    // UI와의 상호작용 변수
    public bool IsStandByTowerBuild { get; set; }
    public bool IsOpeningLaboratory { get; set; }

    // 상태머신
    public PlayerBuilderStateMachine StateMachine;
    public bool IsSelectTower { get; set; }

    public void Awake()
    {
        StateMachine = new PlayerBuilderStateMachine(this);
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

    #region 타워 설치 로직

    // 타워 설치 대기 상태로 돌입
    public void StandByTowerBuild(TowerData towerData)
    {
        IsStandByTowerBuild = true;
        _pTowerData = towerData;
    }

    // 호스트에게 코스트만큼 소유한 자원을 감소 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_TowerBuild(NetworkPrefabRef towerRef, Vector3 buildPos, int cost)
    {
        StageManager.Instance.ResourceSystem.Mineral -= cost;
        Runner.Spawn(towerRef, buildPos, Quaternion.identity);
    }

    #endregion

    #region 연구실 로직

    public void OpenLaboratory(bool isOpening)
    {
        IsOpeningLaboratory = isOpening;
    }

    #endregion


    #region 월드 오브젝트 상호작용

    public void OnClickInteractableObject()
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hit, 5000))
        {
            var inter = hit.collider.GetComponentInParent<IInteractableObject>();
            if (inter != null)
            {
                inter.OnClickThisObject();
            }
        }
    }

    // 빌더의 타워 선택 상태 설정
    public void BuilderSelectTowerSetting(bool isSelectTower, AttackTower tower)
    {
        IsSelectTower = isSelectTower;
        _selectedAttackTower = tower;
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
