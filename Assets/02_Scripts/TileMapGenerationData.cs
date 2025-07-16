using UnityEngine;

[System.Serializable]
public class TileMapGenerationData
{
    public Vector2Int MapSize { get; set; }
    public int[] TileMap { get; set; }

    public TileMapGenerationData(Vector2Int mapSize, int[] tileMap)
    {
        MapSize = mapSize;
        TileMap = tileMap;
    }

    // 특정 위치의 타일 타입 가져오기
    public int GetTileType(int x, int y)
    {
        if (x < 0 || x >= MapSize.x || y < 0 || y >= MapSize.y)
            return -1; // 범위를 벗어나면 -1 반환

        return TileMap[y * MapSize.x + x];
    }

    // 특정 위치의 타일 타입 설정하기
    public void SetTileType(int x, int y, int tileType)
    {
        if (x < 0 || x >= MapSize.x || y < 0 || y >= MapSize.y)
            return;

        TileMap[y * MapSize.x + x] = tileType;
    }

    // Vector2Int 위치로 타일 타입 가져오기
    public int GetTileType(Vector2Int position)
    {
        return GetTileType(position.x, position.y);
    }

    // Vector2Int 위치로 타일 타입 설정하기
    public void SetTileType(Vector2Int position, int tileType)
    {
        SetTileType(position.x, position.y, tileType);
    }
}