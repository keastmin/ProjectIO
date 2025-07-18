using System.Collections.Generic;
using UnityEngine;

public class TerritoryExpandingSystem : MonoBehaviour
{
    public TerritoryTestPlayer player;
    public int circlePointCount;
    public float circleRadius;

    [Header("Debug")]
    public bool isExpanding;
    public Vector2 previousPosition;
    public List<Vector2> playerPath;

    public Territory territory;

    void Awake()
    {
        player.OnPositionChanged += HandlePlayerPositionChanged;
    }

    void Start()
    {
        var polygonPoints = new List<Vector2>();
        for (int i = 0; i < circlePointCount; i++)
        {
            float angle = (circlePointCount - 1 - i) * Mathf.PI * 2 / circlePointCount;
            var point = new Vector2(circleRadius * Mathf.Cos(angle), circleRadius * Mathf.Sin(angle));
            polygonPoints.Add(point);
        }

        territory = Territory.CreatePolygonMesh(polygonPoints.ToArray());
    }

    void HandlePlayerPositionChanged(TerritoryTestPlayer player)
    {
        if (territory == null) { return; }

        var currentPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        if (territory.IsPointInPolygon(currentPosition))
        {
            if (isExpanding)
            {
                if (playerPath.Count > 1)
                {
                    playerPath.Add(currentPosition);
                    Debug.Log("확장 시작");
                    territory.Expand(playerPath);
                }
                playerPath.Clear();
                isExpanding = false;
                Debug.Log("다시 들어옴");
            }
            previousPosition = currentPosition;
        }
        else
        {
            if (!isExpanding)
            {
                isExpanding = true;
                Debug.Log("나감");
                playerPath.Clear();
                playerPath.Add(previousPosition);
                playerPath.Add(currentPosition);
            }

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                playerPath.Add(currentPosition);
                previousPosition = currentPosition;
            }
        }
    }
}