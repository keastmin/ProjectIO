using UnityEngine;

[System.Serializable]
public class ResourceSpawnGroup
{
    public GameObject resourcePrefab;
    public int resourceCount;
    public float spawnRadius;
}

public class LocalResourceSpawnSystem : MonoBehaviour
{
    [SerializeField] LocalTerritorySystem territorySystem;
    [SerializeField] Transform fieldTransform;
    [SerializeField] ResourceSpawnGroup[] resourceSpawnGroups;

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
            Vector3 randomPosition = fieldTransform.position + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0; // y축 고정
            if (territorySystem.Territory.IsPointInPolygon(new Vector2(randomPosition.x, randomPosition.z)))
            {
                i--;
                continue;
            }
            Instantiate(resourcePrefab, randomPosition, Quaternion.identity, fieldTransform);
        }
    }
}