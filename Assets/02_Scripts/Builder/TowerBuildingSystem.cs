using UnityEngine;

public class TowerBuildingSystem : MonoBehaviour
{
    [SerializeField] LocalTerritorySystem territorySystem;
    [SerializeField] HexaTileSnapSystem hexaTileSnapSystem;
    [SerializeField] Transform towerContainerTransform;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private HexaTileMap hexaTileMap;

    GameObject ball;

    bool isActive;

    void Start()
    {
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    void Update()
    {
        var snappedPosition = hexaTileSnapSystem.snappedPosition;

        var ballPosition = new Vector3(snappedPosition.x, 0, snappedPosition.y);
        ball.transform.position = ballPosition;

        var territory = territorySystem.Territory;
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
            BuildTower(ballPosition);
        }
    }

    public void SetBuildingModeActive(bool active)
    {
        isActive = active;

        if (isActive)
        {
            hexaTileSnapSystem.enabled = true;
            // Optionally, you can enable some UI or visual feedback here
            Debug.Log("Tower Building Mode Activated");
        }
        else
        {
            // Optionally, disable UI or visual feedback here
            Debug.Log("Tower Building Mode Deactivated");
        }
    }

    public void BuildTower(Vector3 position)
    {
        Instantiate(towerPrefab, position, Quaternion.identity, towerContainerTransform);
    }
}