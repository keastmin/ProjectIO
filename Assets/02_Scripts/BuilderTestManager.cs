using UnityEngine;

public class BuilderTestManager : MonoBehaviour
{
    [SerializeField] LocalTerritorySystem territoryExpandingSystem;
    [SerializeField] HexaTileSnapSystem hexaTileSnapSystem;
    [SerializeField] LocalTrackGenerationSystem trackGenerationSystem;
    [SerializeField] TrackMonsterSpawnSystem monsterGenerationSystem;

    void Start()
    {
        // 영역 생성
        territoryExpandingSystem.SetUp();

        // 육각 타일 맵 생성
        hexaTileSnapSystem.GenerateInitialHexaTileMap();

        // 트랙 생성
        trackGenerationSystem.GenerateTrack();

        // 몬스터 생성
        monsterGenerationSystem.SpawnMonsters(trackGenerationSystem.Track);
    }
}