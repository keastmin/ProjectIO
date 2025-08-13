using UnityEngine;

public class LocalWorldMonsterSpawnSystem : MonoBehaviour
{
    [SerializeField] LocalTerritorySystem territorySystem;
    [SerializeField] Transform monsterParentTransform;
    [SerializeField] LocalMonster monsterPrefab;
    [SerializeField] int spawnCount;
    [SerializeField] int spawnRadius;
    [SerializeField] Transform playerTransform;

    public void SpawnMonsters()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var randomSpawnPosition = monsterParentTransform.position + Random.insideUnitSphere * spawnRadius;
            randomSpawnPosition.y = 0; // y축 고정
            if (territorySystem.Territory.IsPointInPolygon(randomSpawnPosition))
            {
                i--;
                continue;
            }
            var monster = Instantiate(monsterPrefab, randomSpawnPosition, Quaternion.identity, monsterParentTransform);
            monster.name = $"Monster_{i}";
            monster.PlayerTransform = playerTransform;
            territorySystem.OnTerritoryExpandedEvent += monster.OnTerritoryExpanded;
            monster.SetPatrolPivotPosition(randomSpawnPosition);
            monster.SetPatrolRadius(Random.Range(5f, 10f));
            monster.Territory = territorySystem.Territory;
        }
    }
}