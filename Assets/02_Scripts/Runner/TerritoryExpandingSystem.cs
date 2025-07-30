using System;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryExpandingSystem : MonoBehaviour
{
    [SerializeField] ResourceObtainingSystem resourceObtainingSystem;
    [SerializeField] LineRenderer lineRenderer;
    public RunnerTestPlayer player;
    public int circlePointCount;
    public float circleRadius;

    [Header("Debug")]
    public bool isExpanding;
    public Vector2 previousPosition;
    public List<Vector2> playerPath;

    public Territory territory;

    public event Action<Territory, TerritoryExpandingSystem> OnTerritoryExpandedEvent;

    void Awake()
    {
        player.OnPositionChanged += HandlePlayerPositionChanged;
    }

    void HandlePlayerPositionChanged(RunnerTestPlayer player)
    {
        if (territory == null) { return; }

        var currentPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        if (territory.IsPointInPolygon(currentPosition))
        {
            if (isExpanding)
            {
                if (playerPath.Count > 1)
                {
                    playerPath.Add(currentPosition);
                    Debug.Log("확장됨");
                    territory.Expand(playerPath);
                    if (resourceObtainingSystem != null)
                    {
                        resourceObtainingSystem.TryObtainResources(territory);
                    }
                    OnTerritoryExpandedEvent?.Invoke(territory, this);
                }
                playerPath.Clear();
                lineRenderer.positionCount = 0;
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
                lineRenderer.positionCount = playerPath.Count;
                lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
            }

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                playerPath.Add(currentPosition);
                previousPosition = currentPosition;
                lineRenderer.positionCount = playerPath.Count;
                lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
            }
        }
    }

    public void GenerateInitialTerritory()
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
}