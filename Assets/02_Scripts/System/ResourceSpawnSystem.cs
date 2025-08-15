using Fusion;
using UnityEngine;

public class ResourceSpawnSystem : NetworkSystemBase
{
    [SerializeField] TerritorySystem territorySystem;
    [SerializeField] Transform resourceContainerTransform;
    [SerializeField] ResourceSpawnGroup[] resourceSpawnGroups;

    public override void SetUp()
    {
        if (Object.HasStateAuthority)
        {
            GenerateResources();
        }

        base.SetUp();
    }

    public void GenerateResources()
    {
        foreach (var group in resourceSpawnGroups)
        {
            SpawnResources(group.resourcePrefab, group.resourceCount, group.spawnRadius);
        }
    }

    void SpawnResources(GameObject resourcePrefab, int resourceCount, float spawnRadius)
    {
        for (int i = 0; i < resourceCount; i++)
        {
            Vector3 randomPosition = resourceContainerTransform.position + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0; // y축 고정
            if (territorySystem.Territory.IsPointInPolygon(new Vector2(randomPosition.x, randomPosition.z)))
            {
                i--;
                continue;
            }
            var resourceObject = Runner.Spawn(resourcePrefab, randomPosition, Quaternion.identity, PlayerRef.None, (runner, obj) =>
            {
                obj.transform.SetParent(resourceContainerTransform);
            });
        }
    }
}