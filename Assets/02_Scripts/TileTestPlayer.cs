using UnityEngine;

public class TileTestPlayer : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    // 인스펙터에 표시할 현재 타일 좌표
    [Header("현재 헥사 타일 좌표")]
    public int currentTileX;
    public int currentTileY;

    // 헥사 타일맵 정보 (타일 크기와 오프셋)
    [Header("헥사 타일맵 설정")]
    public float tileSize = 1.0f;
    public Vector3 tileMapOrigin = Vector3.zero;

    [SerializeField] TileManager tileManager;

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = moveSpeed * Time.deltaTime * new Vector3(horizontal, 0, vertical);
        transform.Translate(movement, Space.World);

        // 플레이어가 이동할 때마다 타일의 위치를 업데이트
        UpdateTilePositions();

        tileManager.UpdateTilePositions(currentTileX, currentTileY);
    }

    void UpdateTilePositions()
    {
        // 헥사 타일맵(오프셋 odd-q) 기준으로 플레이어가 위치한 타일 좌표 계산
        Vector3 pos = transform.position - tileMapOrigin;
        float width = tileSize;
        float height = tileSize * Mathf.Sqrt(3) / 2f;

        // odd-q 오프셋 좌표 변환 공식
        int q = Mathf.RoundToInt(pos.x / (width * 0.75f));
        float xOffset = (q % 2 == 0) ? 0 : height / 2f;
        int r = Mathf.RoundToInt((pos.z - xOffset) / height);

        currentTileX = q;
        currentTileY = r;

        Debug.Log($"플레이어 위치: {transform.position}, 타일 좌표: ({currentTileX}, {currentTileY})");
    }
}