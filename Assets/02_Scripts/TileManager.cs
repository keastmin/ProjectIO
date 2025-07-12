using UnityEngine;

public class TileManager : MonoBehaviour
{
    // [SerializeField] TileContainer tileContainer;
    TileMapGenerator tileMapGenerator;

    void Start()
    {
        tileMapGenerator = new();

        var mapSize = new Vector2Int(10, 10);
        GenerateTileMap(mapSize);
    }

    void GenerateTileMap(Vector2Int mapSize)
    {
        var tileMapGenerationData = tileMapGenerator.Generate(mapSize);

        for (int i = 0; i < tileMapGenerationData.TileMap.Length; i++)
        {
            var x = i % mapSize.x;
            var y = i / mapSize.x;

            // tileContainer.CreateTile(new Vector2Int(x, y), tileMap[i]);
        }
    }
}