using UnityEngine;

public class WorldMonsterSpawnSystem : MonoBehaviour
{
    [SerializeField] LocalTerritoryExpandingSystem territoryExpandingSystem;
    [SerializeField] Transform monsterParentTransform;
    [SerializeField] Monster monsterPrefab;
    [SerializeField] int spawnCount;
    [SerializeField] int spawnRadius;
    [SerializeField] Transform playerTransform;

    public void SpawnMonsters()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var randomSpawnPosition = monsterParentTransform.position + Random.insideUnitSphere * spawnRadius;
            randomSpawnPosition.y = 0; // y축 고정
            if (territoryExpandingSystem.territory.IsPointInPolygon(randomSpawnPosition))
            {
                i--;
                continue;
            }
            var monster = Instantiate(monsterPrefab, randomSpawnPosition, Quaternion.identity, monsterParentTransform);
            monster.name = $"Monster_{i}";
            monster.PlayerTransform = playerTransform;
            territoryExpandingSystem.OnTerritoryExpandedEvent += monster.OnTerritoryExpanded;
            monster.SetPatrolPivotPosition(randomSpawnPosition);
            monster.SetPatrolRadius(Random.Range(5f, 10f));
            monster.Territory = territoryExpandingSystem.territory;
        }
    }
}