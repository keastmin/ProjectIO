using UnityEngine;

public class TileMapGenerator
{
    public TileMapGenerationData Generate(Vector2Int size)
    {
        int[] tileMap = new int[size.x * size.y];
        for (int i = 0; i < tileMap.Length; i++)
        {
            tileMap[i] = Random.Range(0, 2); // Randomly assign 0 or 1
        }
        return new TileMapGenerationData(size, tileMap);
    }
}