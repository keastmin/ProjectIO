using Fusion;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] PlayerRunner playerRunnerPrefab;
    [SerializeField] NetworkTerritory territoryPrefab;
    [SerializeField] CinemachineSystem cinemachineSystemPrefab;
    [SerializeField] NetworkInputSystem networkInputSystemPrefab;
    [SerializeField] NetworkTerritorySystem territorySystem;
    [SerializeField] NetworkSystemBase[] systems;

    CinemachineSystem cinemachineSystem;
    [Networked] public PlayerRunner Player { get; set; }
    [Networked] public NetworkTerritory TerritoryView { get; set; }

    public override void Spawned()
    {
        Instance = this;

        SpawnCinemachineSystem();
        if (Object.HasStateAuthority)
        {
            SpawnPlayer();
            SpawnTerritory();
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

    void SpawnPlayer()
    {
        Player = Runner.Spawn(playerRunnerPrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);

        Player.name = $"{Runner.name} - Player";
        Debug.Log($"{Runner.name} - Player spawned");
    }

    void SpawnTerritory()
    {
        TerritoryView = Runner.Spawn(territoryPrefab, Vector3.zero, Quaternion.identity);
        TerritoryView.name = $"{Runner.name} - Territory";
        Debug.Log($"{Runner.name} - Territory spawned");
    }

    void SpawnCinemachineSystem()
    {
        var instance = Instantiate(cinemachineSystemPrefab);
        // var instance = Runner.Spawn(cinemachineSystemPrefab, Vector3.zero, Quaternion.identity);
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