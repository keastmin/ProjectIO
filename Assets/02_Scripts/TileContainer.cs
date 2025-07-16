using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour
{
    [SerializeField] GameObject[] tilePrefabs; // 여러 종류의 타일 프리팹
    [SerializeField] Transform tileParent; // 타일들의 부모 오브젝트

    private List<GameObject> spawnedTiles = new List<GameObject>();

    void Awake()
    {
        // 타일 부모가 지정되지 않았다면 현재 오브젝트를 부모로 설정
        if (tileParent == null)
            tileParent = transform;
    }

    public void CreateTile(Vector3 position, int tileType)
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogWarning("타일 프리팹이 설정되지 않았습니다!");
            return;
        }

        // 타일 타입이 범위를 벗어나면 기본 타일(0번) 사용
        int safeTileType = Mathf.Clamp(tileType, 0, tilePrefabs.Length - 1);

        var tileObject = Instantiate(tilePrefabs[safeTileType], position, Quaternion.identity);
        tileObject.transform.SetParent(tileParent);

        // 타일에 Tile 컴포넌트가 있다면 초기화
        var tile = tileObject.GetComponent<Tile>();
        if (tile != null)
        {
            tile.Initialize(tileType, position);
        }

        spawnedTiles.Add(tileObject);
    }

    public void ClearAllTiles()
    {
        foreach (var tile in spawnedTiles)
        {
            if (tile != null)
                DestroyImmediate(tile);
        }
        spawnedTiles.Clear();
    }

    // 구버전 호환성을 위한 메서드
    public void CreateTile(Vector3 position)
    {
        CreateTile(position, 0);
    }

    public void SetTileColor(Vector2Int location, Color color)
    {
        // 위치에 해당하는 타일을 찾기
        foreach (var tile in spawnedTiles)
        {
            if (tile != null && PositionToTileCoordinates(tile.transform.position) == location)
            {
                Renderer renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
                return;
            }
        }

        Debug.LogWarning($"위치 {location}에 해당하는 타일을 찾을 수 없습니다.");
    }

    Vector2Int PositionToTileCoordinates(Vector3 position)
    {
        // 타일의 크기를 고려하여 좌표 계산
        float tileSize = 1.0f; // 타일 크기 (필요에 따라 조정)
        int x = Mathf.FloorToInt(position.x / (tileSize * 0.75f));
        int y = Mathf.FloorToInt(position.z / (tileSize * Mathf.Sqrt(3) / 2f));

        // int q = Mathf.RoundToInt(pos.x / (width * 0.75f));
        // float xOffset = (q % 2 == 0) ? 0 : height / 2f;
        // int r = Mathf.RoundToInt((pos.z - xOffset) / height);
        return new Vector2Int(x, y);
    }
}