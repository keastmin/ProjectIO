using UnityEngine;

public class HexaTileSnapSystem : MonoBehaviour
{
    [SerializeField] TerritoryExpandingSystem territoryExpandingSystem;
    [SerializeField] float hexSize = 1.0f; // Set your desired hex size
    [SerializeField] bool isFlatTop = true; // Set to true for flat-top

    public HexaTileMap hexaTileMap;

    [Header("Debug")]
    public Vector2 mousePositionInWorld;
    public Vector2 snappedPosition;

    GameObject ball;

    void Start()
    {
        hexaTileMap = new HexaTileMap
        {
            HexSize = hexSize, // Set your desired hex size
            IsFlatTop = isFlatTop // Set to true for flat-top hexes, false for pointy-top
        };

        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    void Update()
    {
        var mousePositionInScreen = Input.mousePosition;
        mousePositionInScreen.z = -Camera.main.transform.position.z;
        mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePositionInScreen);
        snappedPosition = hexaTileMap.SnapToHexTile(mousePositionInWorld);
        ball.transform.position = (Vector3)snappedPosition;

        var territory = territoryExpandingSystem.territory;
        var isInTerritory = territory.IsPointInPolygon(snappedPosition);
        if (isInTerritory)
        {
            ball.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            ball.GetComponent<Renderer>().material.color = Color.red;
        }

        if (Input.GetMouseButtonDown(0) && isInTerritory)
        {
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = snappedPosition;
        }
    }
}