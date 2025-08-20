using UnityEngine;

public class HexagonGridSystem : MonoBehaviour
{
    public static HexagonGridSystem Instance;

    [Header("Cell Size")]
    [SerializeField] [Min(0.1f)] private float _length = 1.5f; // 정육각형의 한 변의 길이

    [Header("Grid Size")]
    [SerializeField] [Min(1)] private int _cellCountX = 10; // X 인덱스의 최대 값
    [SerializeField] [Min(1)] private int _cellCountY = 10; // Y 인덱스의 최대 값

    [Header("Territory")]
    [SerializeField] private TerritorySystem _territorySystem;

    private MeshCollider _collider;

    private HexagonCell[,] _grid; // 그리드

    // 편의용 캐시: size/간격 (Pointy-Top)
    float width => (0.8660254f * _length); // √3/2 * s
    float xStep => width * 2f;             // √3 * s
    float zStep => (1.5f * _length);       // 1.5 * s
    const float SQRT3 = 1.7320508075688772f;

    private void OnValidate()
    {
        InitGrid(_cellCountX, _cellCountY);
    }

    private void Awake()
    {
        Instance = this;
        OnValidate();
    }

    private void InitGrid(int countX, int countY)
    {
        _grid = new HexagonCell[countX, countY];

        TryGetComponent(out _collider);
      
        Vector3 origin = _collider.bounds.min;

        for (int y = 0; y < countY; y++)
        {
            float rowOffsetX = (y & 1) == 1 ? width : 0; 

            for (int x = 0; x < countX; x++)
            {
                float posX = origin.x + (x * xStep) + rowOffsetX;
                float posZ = origin.z + (y * zStep);

                _grid[x, y] = new HexagonCell(x, y, posX, posZ);
            }
        }
    }

    // 현재 마우스 위치로부터 가까운 셀의 중심 위치를 반환
    public Vector3 GetNearGridPosition(Vector3 mousePosition)
    {
        // 1) 월드XZ -> 그리드 원점 기준 상대좌표
        Vector3 origin = _collider.bounds.min;
        Vector2 originXZ = new Vector2(origin.x, origin.z);
        Vector2 wp = new Vector2(mousePosition.x, mousePosition.z) - originXZ;

        // 2) 월드XZ -> 축좌표(q, r) (Pointy-Top)
        float s = _length;
        float r = (2f / 3f) * (wp.y / s);
        float q = (wp.x / (s * SQRT3)) - (wp.y / (3f * s));

        // 3) 축좌표를 가장 가까운 정수 육각으로 반올림 (cube-rounding)
        //    cube: (x=q, y=-q-r, z=r), x+y+z=0을 만족하도록 가장 큰 오차 성분을 조정
        float cx = q;
        float cz = r;
        float cy = -cx - cz;

        int rx = Mathf.RoundToInt(cx);
        int ry = Mathf.RoundToInt(cy);
        int rz = Mathf.RoundToInt(cz);

        float dx = Mathf.Abs(rx - cx);
        float dy = Mathf.Abs(ry - cy);
        float dz = Mathf.Abs(rz - cz);

        if (dx > dy && dx > dz)
            rx = -ry - rz;
        else if (dy > dz)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        // 4) 정수 축좌표(qi=rx, ri=rz) -> Odd-R 오프셋 인덱스(col, row)
        int qi = rx;
        int ri = rz;
        int row = ri;
        int col = qi + ((ri - (ri & 1)) / 2);

        // 5) 그리드 경계 클램프
        col = Mathf.Clamp(col, 0, _cellCountX - 1);
        row = Mathf.Clamp(row, 0, _cellCountY - 1);

        // 6) 월드 중심 좌표 반환
        return _grid[col, row].WorldPosition;
    }

    public bool IsPointToTowerCraftValid(Vector3 mousePoint)
    {
        bool isValid = false;
        Vector2 point = new Vector2(mousePoint.x, mousePoint.z);

        if (_territorySystem != null)
            isValid = _territorySystem.Territory.IsPointInPolygon(point);

        Debug.Log($"point: {point}, isValid: {isValid}");

        return isValid;
    }

    private void OnDrawGizmos()
    {
        if(_grid != null)
        {
            Gizmos.color = Color.red;
            foreach(var cell in _grid)
            {
                Gizmos.DrawSphere(cell.WorldPosition, 0.3f);
            }
        }
    }
}
