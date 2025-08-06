using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkTerritoryExpandingSystem : NetworkBehaviour
{
    [SerializeField] ResourceObtainingSystem resourceObtainingSystem;
    [SerializeField] LineRenderer lineRenderer;
    public RunnerTestPlayer player; // Spawn으로 생성되어야함, 실행 순서를 생각해서 구현해야함 -> Manager가 게임을 초기화: Spawn할 때 연결, Spawn은 호스트만 할 수 있음
    public int circlePointCount;
    public float circleRadius;

    [Header("Debug")]
    public bool isExpanding;
    public Vector2 previousPosition;
    public List<Vector2> playerPath;

    public LocalTerritory territory;

    public event Action<LocalTerritory, NetworkTerritoryExpandingSystem> OnTerritoryExpandedEvent;

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