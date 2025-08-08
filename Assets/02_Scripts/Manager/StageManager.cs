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
    [SerializeField] TerritorySystem territorySystem;
    [SerializeField] NetworkSystemBase[] systems;

    [Networked] public PlayerRole PlayerRole { get; set; }
    [Networked] public PlayerRunner Player { get; set; }
    public TerritoryView TerritoryView;
    public TrackView TrackView;

    CinemachineSystem cinemachineSystem;

    public override void Spawned()
    {
        Instance = this;

        SpawnCinemachineSystem();

        if (Object.HasStateAuthority)
        {
            SpawnPlayerRole();
            SpawnPlayer();
            SpawnNetworkInputSystem();

            Player.OnPositionChanged += territorySystem.OnPlayerPositionChanged;
            // territorySystem.OnTerritoryExpandedEvent += resourceObtainingSystem.OnTerritoryExpanded;
        }

        foreach (var system in systems)
        {
            system.SetUp();
        }

        cinemachineSystem.SetRunnerCameraTarget(Player.transform);
    }

    void SpawnPlayerRole()
    {
        PlayerRole = Runner.Spawn(playerRolePrefab, Vector3.zero, Quaternion.identity);
        PlayerRole.name = $"{Runner.name} - PlayerRole";
    }

    void SpawnPlayer()
    {
        var runnerPlayer = PlayerRole.Instance.Role[PlayerPosition.Runner];
        Player = Runner.Spawn(playerRunnerPrefab, Vector3.zero, Quaternion.identity, runnerPlayer);
        Player.name = $"{Runner.name} - Player";

        var builderPlayer = PlayerRole.Instance.Role[PlayerPosition.Builder];
        Runner.Spawn(playerBuilderPrefab, Vector3.zero, Quaternion.identity, builderPlayer);
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