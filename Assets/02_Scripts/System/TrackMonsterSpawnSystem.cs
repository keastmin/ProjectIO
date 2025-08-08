using System.Collections;
using UnityEngine;

public class TrackMonsterSpawnSystem : NetworkSystemBase
{
    [SerializeField] TrackSystem trackSystem;
    [SerializeField] Transform monsterParentTransform;
    [SerializeField] Monster monsterPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] int spawnCount;

    public override void SetUp()
    {
        if (Object.HasStateAuthority)
        {
            SpawnMonsters(trackSystem.Track);
        }
    }

    public void SpawnMonsters(Track track)
    {
        if (Object.HasStateAuthority)
        {
            StartCoroutine(MonsterSpawnRoutine(track));
        }
    }

    IEnumerator MonsterSpawnRoutine(Track track)
    {
        var startPosition = track.Vertices[0];

        for (int i = 0; i < spawnCount; i++)
        {
            var monster = Runner.Spawn(monsterPrefab, startPosition, Quaternion.identity);
            monster.name = $"Monster_{i}";
            monster.SetTrack(track);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}