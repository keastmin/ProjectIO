using Fusion;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    [SerializeField] GameObject territoryPrefab;

    NetworkTerritory territory;

    public override void Spawned()
    {
        // runner = Runner;
        if (Object.HasStateAuthority)
        {
            SpawnTerritory();
        }
    }

    public void SpawnTerritory()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(territoryPrefab, Vector3.zero, Quaternion.identity, onBeforeSpawned: OnBeforeSpawned);
        }
        // 여기에 Territory를 스폰하는 로직을 추가합니다.
        // 예: Runner.Spawn(territoryPrefab, position, rotation);
        Debug.Log("Territory spawned");
    }

    void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj)
    {
        // 여기에 네트워크 오브젝트가 스폰되기 전에 실행할 로직을 추가합니다.
        // 예: networkObject.GetComponent<NetworkTerritory>().Initialize();
    }
}