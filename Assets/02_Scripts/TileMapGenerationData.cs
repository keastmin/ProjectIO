using UnityEngine;

public class TileMapGenerationData
{
    public Vector2Int MapSize { get; set; }
    public int[] TileMap { get; set; }

    public TileMapGenerationData(Vector2Int mapSize, int[] tileMap)
    {
        MapSize = mapSize;
        TileMap = tileMap;
    }
}