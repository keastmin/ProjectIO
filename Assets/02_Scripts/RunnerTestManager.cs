using UnityEngine;

public class RunnerTestManager : MonoBehaviour
{
    [SerializeField] LocalTerritoryExpandingSystem territoryExpandingSystem;
    [SerializeField] HexaTileSnapSystem hexaTileSnapSystem;
    [SerializeField] ResourceSpawnSystem resourceSpawnSystem;
    [SerializeField] WorldMonsterSpawnSystem worldMonsterSpawnSystem;

    void Start()
    {
        // 영역 생성
        territoryExpandingSystem.GenerateInitialTerritory(); // 여기

        // 육각 타일 맵 생성
        hexaTileSnapSystem.GenerateInitialHexaTileMap();

        // 자원 생성
        resourceSpawnSystem.GenerateResources();

        // 테스트용 몬스터 스폰
        worldMonsterSpawnSystem.SpawnMonsters();
    }
}