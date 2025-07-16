using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int tileType;
    [SerializeField] private Vector3 gridPosition;
    [SerializeField] private Renderer tileRenderer;
    [SerializeField] private Color[] tileColors = { Color.green, Color.gray }; // 타일 타입별 색상

    void Awake()
    {
        // Renderer가 지정되지 않았다면 자동으로 찾기
        if (tileRenderer == null)
            tileRenderer = GetComponent<Renderer>();
    }

    public void Initialize(int type, Vector3 position)
    {
        tileType = type;
        gridPosition = position;
        
        // 타일 타입에 따라 색상 변경
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (tileRenderer != null && tileColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(tileType, 0, tileColors.Length - 1);
            tileRenderer.material.color = tileColors[colorIndex];
        }
    }

    // 마우스 클릭 시 타일 정보 출력
    void OnMouseDown()
    {
        Debug.Log($"타일 클릭됨 - 타입: {tileType}, 위치: {gridPosition}");
    }

    // 공개 프로퍼티들
    public int TileType => tileType;
    public Vector3 GridPosition => gridPosition;
}