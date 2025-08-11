using Fusion;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] PlayerRole playerRolePrefab;
    [SerializeField] PlayerRunner playerRunnerPrefab;
    [SerializeField] PlayerBuilder playerBuilderPrefab;
    [SerializeField] CinemachineSystem cinemachineSystemPrefab;
    [SerializeField] NetworkInputSystem networkInputSystemPrefab;
    [SerializeField] NetworkSystemBase[] systems;

    //[Networked] public PlayerRole PlayerRole { get; set; }
    [Networked] public PlayerRunner PlayerRunner { get; set; }
    public TerritoryView TerritoryView;
    public TrackView TrackView;

    CinemachineSystem cinemachineSystem;

    public override void Spawned()
    {
        Instance = this;

        SpawnCinemachineSystem();

        if (HasStateAuthority)
        {
            //SpawnPlayerRole();
            SpawnPlayer();
            SpawnNetworkInputSystem();

            // territorySystem.OnTerritoryExpandedEvent += resourceObtainingSystem.OnTerritoryExpanded;
        }

        foreach (var system in systems)
        {
            system.SetUp();
        }

        cinemachineSystem.SetRunnerCameraTarget(PlayerRunner.transform);

        var currentPlayerPosition = PlayerRegistry.Instance.RefToPosition[Runner.LocalPlayer];
        cinemachineSystem.SetCinemachinePriority(currentPlayerPosition);
    }

    //void SpawnPlayerRole()
    //{
    //    PlayerRole = Runner.Spawn(playerRolePrefab, Vector3.zero, Quaternion.identity);
    //    PlayerRole.name = $"{Runner.name} - PlayerRole";
    //}

    void SpawnPlayer()
    {
        var runnerPlayer = PlayerRegistry.Instance.GetPlayerRefFromPosition(PlayerPosition.Runner);
        PlayerRunner = Runner.Spawn(ResourceManager.Instance.PlayerRunnerPrefab, Vector3.zero, Quaternion.identity, runnerPlayer);
        PlayerRunner.name = $"{Runner.name} - Player Runner";

        var builderPlayer = PlayerRegistry.Instance.GetPlayerRefFromPosition(PlayerPosition.Builder);
        var playerBuilder = Runner.Spawn(ResourceManager.Instance.PlayerBuilderPrefab, Vector3.zero, Quaternion.identity, builderPlayer);
        playerBuilder.name = $"{Runner.name} - Player Builder";

        Debug.Log($"{Runner.name} - Player spawned");
    }

    void SpawnCinemachineSystem()
    {
        var instance = Instantiate(cinemachineSystemPrefab);
        instance.name = $"{Runner.name} - CinemachineSystem";
        cinemachineSystem = instance;
        Debug.Log($"{Runner.name} - CinemachineSystem spawned");
    }

    void SpawnNetworkInputSystem()
    {
        var instance = Runner.Spawn(networkInputSystemPrefab, Vector3.zero, Quaternion.identity);
        instance.name = $"{Runner.name} - NetworkInputSystem";
        Debug.Log($"{Runner.name} - NetworkInputSystem spawned");
    }
}