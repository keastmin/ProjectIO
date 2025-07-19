using UnityEngine;

public class HexaTileSnapSystem : MonoBehaviour
{
    [SerializeField] float hexSize = 1.0f; // Set your desired hex size
    [SerializeField] bool isFlatTop = true; // Set to true for flat-top

    public HexaTileMap hexaTileMap;

    [Header("Debug")]
    public Vector2 mousePositionInWorld;
    public Vector2 snappedPosition;

    Ray ray;
    Vector3 hitPoint;

    void Update()
    {
        var mousePositionInScreen = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(mousePositionInScreen);
        var xzPlane = new Plane(Vector3.up, Vector3.zero); // y=0 평면
        if (xzPlane.Raycast(ray, out var enter))
        {
            hitPoint = ray.GetPoint(enter);
            mousePositionInWorld = new Vector2(hitPoint.x, hitPoint.z);
        }
        snappedPosition = hexaTileMap.SnapToHexTile(mousePositionInWorld);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 100f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(hitPoint, 0.1f);
    }

    public void GenerateInitialHexaTileMap()
    {
        hexaTileMap = new HexaTileMap
        {
            HexSize = hexSize, // Set your desired hex size
            IsFlatTop = isFlatTop // Set to true for flat-top hexes, false for pointy-top
        };
    }
}