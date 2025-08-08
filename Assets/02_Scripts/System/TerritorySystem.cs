using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TerritorySystem : NetworkSystemBase
{
    [SerializeField] LocalResourceObtainingSystem resourceObtainingSystem;

    [Header("Initial Territory")]
    [SerializeField] int circlePointCount;
    [SerializeField] float circleRadius;

    [Header("Expanding")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] bool isExpanding;
    [SerializeField] Vector2 previousPosition;
    [SerializeField] List<Vector2> playerPath;

    Territory territory;
    TerritoryView territoryView;

    public event Action<Territory, TerritorySystem> OnTerritoryExpandedEvent;

    public override void SetUp()
    {
        territoryView = StageManager.Instance.TerritoryView;

        GenerateInitialTerritory();
    }

    void GenerateInitialTerritory()
    {
        var vertices = GenerateCircleTerritory();

        CreateTerritory(vertices);
        territoryView.name = $"{Runner.name} - Territory";
        territoryView.SetTerritory(vertices);
    }

    void CreateTerritory(List<Vector2> vertices)
    {
        territory = new() { Vertices = vertices };
    }

    List<Vector2> GenerateCircleTerritory()
    {
        var polygonPoints = new List<Vector2>();

        var twoPI = Mathf.PI * 2;
        var partOfAngle = twoPI / circlePointCount;

        for (int i = 0; i < circlePointCount; i++)
        {
            var angle = (circlePointCount - 1 - i) * partOfAngle;
            var x = circleRadius * Mathf.Cos(angle);
            var y = circleRadius * Mathf.Sin(angle);
            var point = new Vector2(x, y);
            polygonPoints.Add(point);
        }

        return polygonPoints;
    }

    public void OnPlayerPositionChanged(PlayerRunner playerRunner) // 러너만
    {
        var currentPosition = new Vector2(playerRunner.transform.position.x, playerRunner.transform.position.z);

        if (territory.IsPointInPolygon(currentPosition))
        {
            if (isExpanding)
            {
                if (playerPath.Count > 1)
                {
                    RPC_AddExpandingPathPoint(currentPosition);
                    RPC_ExpandTerritory();
                }
                RPC_StopExpanding();
                Debug.Log("다시 들어옴");
            }
            previousPosition = currentPosition;
        }
        else
        {
            if (!isExpanding)
            {
                Debug.Log("나감");
                RPC_StartExpanding();
                RPC_AddExpandingPathPoint(previousPosition);
                RPC_AddExpandingPathPoint(currentPosition);
            }

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                RPC_AddExpandingPathPoint(currentPosition);
                previousPosition = currentPosition;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartExpanding()
    {
        isExpanding = true;
        playerPath.Clear();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StopExpanding()
    {
        playerPath.Clear();
        lineRenderer.positionCount = 0;
        isExpanding = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddExpandingPathPoint(Vector2 point)
    {
        playerPath.Add(point);
        lineRenderer.positionCount = playerPath.Count;
        lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ExpandTerritory()
    {
        Debug.Log($"{Runner.name} - Expanding territory with path: { playerPath.Count}");

        territory.Expand(playerPath);
        territoryView.SetTerritory(territory.Vertices);

        // if (resourceObtainingSystem != null)
        // {
        //     resourceObtainingSystem.TryObtainResources(territoryView);
        // }

        if (Object.HasStateAuthority)
        {
            OnTerritoryExpandedEvent?.Invoke(territory, this); // 호스트만
        }
    }
}