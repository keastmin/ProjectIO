using UnityEngine;

public class RunnerTestManager : MonoBehaviour
{
    [SerializeField] TerritoryExpandingSystem territoryExpandingSystem;
    [SerializeField] HexaTileSnapSystem hexaTileSnapSystem;
    [SerializeField] ResourceSpawnSystem resourceSpawnSystem;

    void Start()
    {
        // 영역 생성
        territoryExpandingSystem.GenerateInitialTerritory();

        // 육각 타일 맵 생성
        hexaTileSnapSystem.GenerateInitialHexaTileMap();

        // 자원 생성
        resourceSpawnSystem.GenerateResources();
    }
}