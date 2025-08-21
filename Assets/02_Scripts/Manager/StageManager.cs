using Fusion;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    [Networked] public PlayerRunner PlayerRunner { get; set; }
    [Networked] public PlayerBuilder PlayerBuilder { get; set; }

    public static StageManager Instance { get; private set; }

    [Space(10)]

    [Header("Player")]
    [SerializeField] private PlayerRunner playerRunnerPrefab; // 러너 플레이어 프리팹
    [SerializeField] private PlayerBuilder playerBuilderPrefab; // 빌더 플레이어 프리팹

    [Space(10)]

    [Header("Network Systems")]
    [SerializeField] private NetworkInputSystem networkInputSystemPrefab;
    [SerializeField] private NetworkSystemBase[] systems;
    public ResourceSystem ResourceSystem;

    [Space(10)]

    [Header("Local Systems")]
    [SerializeField] private CinemachineSystem cinemachineSystemPrefab; // 시네머신 시스템 프리팸
    [SerializeField] private StageUIController _stageUIController; // 이 게임 스테이지의 UI 컨트롤러

    [Space(10)]

    [Header("Territory")]
    public TerritoryView TerritoryView;
    public TrackView TrackView;

    public override void Spawned()
    {
        Instance = this;

        if (HasStateAuthority)
        {
            SpawnPlayer();
            SpawnNetworkInputSystem();
        }

        foreach (var system in systems)
        {
            system.SetUp();
        }

        // 로컬 시스템 - 시네머신 초기화
        InitCinemachineSystem();

        // 로컬 시스템 - UI 초기화
        InitUIController();
    }

    void SpawnPlayer()
    {
        var runnerPlayer = PlayerRegistry.Instance.GetPlayerRefFromPosition(PlayerPosition.Runner);
        PlayerRunner = Runner.Spawn(ResourceManager.Instance.PlayerRunnerPrefab, Vector3.zero, Quaternion.identity, runnerPlayer);
        PlayerRunner.name = $"{Runner.name} - Player Runner";

        var builderPlayer = PlayerRegistry.Instance.GetPlayerRefFromPosition(PlayerPosition.Builder);
        PlayerBuilder = Runner.Spawn(ResourceManager.Instance.PlayerBuilderPrefab, Vector3.zero, Quaternion.identity, builderPlayer);
        PlayerBuilder.name = $"{Runner.name} - Player Builder";

        Debug.Log($"{Runner.name} - Player spawned");
    }

    void SpawnNetworkInputSystem()
    {
        var instance = Runner.Spawn(networkInputSystemPrefab, Vector3.zero, Quaternion.identity);
        instance.name = $"{Runner.name} - NetworkInputSystem";
        Debug.Log($"{Runner.name} - NetworkInputSystem spawned");
    }

    // 정해진 역할군에 따라 시네머신 카메라를 초기화
    void InitCinemachineSystem()
    {
        var instance = Instantiate(cinemachineSystemPrefab); // 시네머신 시스템 인스턴스 생성
        instance.name = $"{Runner.name} - CinemachineSystem";
        Debug.Log($"{Runner.name} - CinemachineSystem spawned");

        instance.SetRunnerCameraTarget(PlayerRunner.transform); // 플레이어 러너 오브젝트를 러너 카메라의 타겟으로 설정

        var currentPlayerPosition = PlayerRegistry.Instance.RefToPosition[Runner.LocalPlayer]; // 현재 자신의 역할군을 가져옴
        instance.SetCinemachinePriority(currentPlayerPosition); // 자신의 역할군에 따라 시네머신 카메라의 우선순위를 설정
    }

    // 정해진 역할군에 따라 표현되는 UI를 초기화, 타워 슬롯 버튼을 설정
    private void InitUIController()
    {
        var playerPosition = PlayerRegistry.Instance.RefToPosition[Runner.LocalPlayer]; // 현재 자신의 역할군을 가져옴
        _stageUIController.SetPlayerUI(playerPosition); // 자신의 역할군에 따라 UI를 설정
    }
}