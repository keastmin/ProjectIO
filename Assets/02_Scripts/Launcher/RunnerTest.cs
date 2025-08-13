using UnityEngine;

namespace Launcher
{
    public class RunnerTest : LauncherBase
    {
        [SerializeField] LocalTerritorySystem territorySystem;
        [SerializeField] HexaTileSnapSystem hexaTileSnapSystem;
        [SerializeField] LocalResourceSpawnSystem resourceSpawnSystem;
        [SerializeField] LocalWorldMonsterSpawnSystem worldMonsterSpawnSystem;

        protected override void OnLauncherStarted()
        {
            // 영역 생성
            territorySystem.SetUp();

            // 육각 타일 맵 생성
            hexaTileSnapSystem.GenerateInitialHexaTileMap();

            // 자원 생성
            resourceSpawnSystem.GenerateResources();

            // 테스트용 몬스터 스폰
            worldMonsterSpawnSystem.SpawnMonsters();
        }
    }
}