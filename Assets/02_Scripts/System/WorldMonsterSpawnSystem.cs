using Fusion;
using UnityEngine;

public class WorldMonsterSpawnSystem : NetworkSystemBase
{
    [SerializeField] TerritorySystem territorySystem;
    [SerializeField] Transform monsterParentTransform;
    [SerializeField] Monster monsterPrefab;
    [SerializeField] int spawnCount;
    [SerializeField] int spawnRadius;
    public Transform playerTransform;

    public override void SetUp()
    {
        if (Object.HasStateAuthority)
        {
            playerTransform = StageManager.Instance.PlayerRunner.transform;
            SpawnMonsters();
        }

        base.SetUp();
    }

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
            var monster = Runner.Spawn(monsterPrefab, randomSpawnPosition, Quaternion.identity, PlayerRef.None, (runner, obj) =>
            {
                obj.name = $"Monster_{i}";
                obj.transform.SetParent(monsterParentTransform);
            });

            monster.PlayerTransform = playerTransform;
            territorySystem.OnTerritoryExpandedEvent += monster.OnTerritoryExpanded;
            monster.SetPatrolPivotPosition(randomSpawnPosition);
            monster.SetPatrolRadius(Random.Range(5f, 10f));
            monster.Territory = territorySystem.Territory;
        }
    }
}