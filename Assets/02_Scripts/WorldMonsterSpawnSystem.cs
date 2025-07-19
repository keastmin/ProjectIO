using UnityEngine;

public class WorldMonsterSpawnSystem : MonoBehaviour
{
    [SerializeField] Transform monsterParentTransform;
    [SerializeField] Monster monsterPrefab;
    [SerializeField] int spawnCount;
    [SerializeField] int spawnRadius;

    public void SpawnMonsters()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPosition = monsterParentTransform.position + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0; // y축 고정
            var monster = Instantiate(monsterPrefab, randomPosition, Quaternion.identity, monsterParentTransform);
            monster.name = $"Monster_{i}";
            monster.SetPatrolPivotPosition(randomPosition);
            monster.SetPatrolRadius(Random.Range(5f, 10f));
        }
    }
}