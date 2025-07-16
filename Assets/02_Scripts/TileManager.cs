using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] TileContainer tileContainer;
    [SerializeField] Vector2Int mapSize = new Vector2Int(10, 10);
    [SerializeField] float tileSize = 1.0f;

    TileMapGenerator tileMapGenerator;

    void Start()
    {
        tileMapGenerator = new();
        GenerateTileMap(mapSize);
    }

    void GenerateTileMap(Vector2Int mapSize)
    {
        var tileMapGenerationData = tileMapGenerator.Generate(mapSize);

        float width = tileSize;
        float height = tileSize * Mathf.Sqrt(3) / 2f;

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int i = y * mapSize.x + x;
                float xPos = x * width * 0.75f;
                float zPos = y * height + (x % 2 == 0 ? 0 : height / 2f);

                var position = new Vector3(xPos, 0, zPos);
                var tileType = tileMapGenerationData.TileMap[i];

                tileContainer.CreateTile(position, tileType);
            }
        }
    }

    void Update()
    {
        // R키를 누르면 맵을 재생성
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateTileMap();
        }
    }

    void RegenerateTileMap()
    {
        tileContainer.ClearAllTiles();
        GenerateTileMap(mapSize);
    }

    public void UpdateTilePositions(int currentTileX, int currentTileY)
    {
        tileContainer.SetTileColor(new Vector2Int(currentTileX, currentTileY), Color.red);
    }
}