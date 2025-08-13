using UnityEngine;

public class HexagonGridSystem : MonoBehaviour
{
    [Header("Cell Size")]
    [SerializeField] [Min(0.1f)] private float _length = 1.5f; // 정육각형의 한 변의 길이

    [Header("Grid Size")]
    [SerializeField] [Min(1)] private int _cellCountX = 10; // X 인덱스의 최대 값
    [SerializeField] [Min(1)] private int _cellCountY = 10; // Y 인덱스의 최대 값

    [Header("Gizmos")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private bool _drawCenters = true;
    [SerializeField] private float _centerRadius = 0.08f;

    private HexagonCell[,] _grid; // 그리드

    // 편의용 캐시: size/간격
    float width => (0.866f * _length);
    float xStep => width * 2;
    float zStep => (1.5f * _length);

    private void OnValidate()
    {
        InitGrid(_cellCountX, _cellCountY);
    }

    private void Awake()
    {
        OnValidate();
    }

    private void Start()
    {
        
    }

    private void InitGrid(int countX, int countY)
    {
        _grid = new HexagonCell[countX, countY];

        Vector3 origin = transform.position;

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
