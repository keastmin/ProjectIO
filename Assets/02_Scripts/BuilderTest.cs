using UnityEngine;

public class BuilderTest : MonoBehaviour
{
    [SerializeField] TrackGenerationSystem trackGenerationSystem;
    [SerializeField] MonsterSpawnSystem monsterGenerationSystem;

    void Start()
    {
        // 트랙 생성
        trackGenerationSystem.GenerateTrack();

        // 몬스터 생성
        monsterGenerationSystem.SpawnMonsters(trackGenerationSystem.Track);
    }
}