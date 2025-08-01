using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class TestMonsterSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject _monsterPrefab; // 에디터에서 프리팹 할당
    [SerializeField] private int _spawnCount = 10; // 스폰할 몬스터 수
    [SerializeField] private Vector2 _mapMin = new Vector2(-20f, -20f); // 맵 XZ 최소 좌표
    [SerializeField] private Vector2 _mapMax = new Vector2(20f, 20f);   // 맵 XZ 최대 좌표

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SpawnMonsters();
        }
    }

    private void SpawnMonsters()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPos = GetRandomPosition();
            Runner.Spawn(_monsterPrefab, spawnPos, Quaternion.identity, null); // InputAuthority 없음
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(_mapMin.x, _mapMax.x);
        float z = Random.Range(_mapMin.y, _mapMax.y);
        float y = 0f; // 지상
        return new Vector3(x, y, z);
    }
}
