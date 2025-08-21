using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TerritorySystem : NetworkSystemBase
{
    [Header("Initial Territory")]
    [SerializeField] int circlePointCount;
    [SerializeField] float circleRadius;

    [Header("Expanding")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] bool isExpanding;
    [SerializeField] Vector2 previousPosition;
    [SerializeField] List<Vector2> playerPath;

    public Territory Territory;
    public TerritoryView TerritoryView;

    public event Action<Territory, TerritorySystem> OnTerritoryExpandedEvent;

    void OnDrawGizmos()
    {
        if (Territory != null)
        {
            Gizmos.color = Color.green;
            foreach (var point in Territory.Vertices)
            { // vector2(x, y) -> vector3(x, y, 0)
                Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.5f);
            }
        }
    }

    public override void SetUp()
    {
        TerritoryView = StageManager.Instance.TerritoryView;

        GenerateInitialTerritory();

        if (Object.HasStateAuthority)
        {
            StageManager.Instance.PlayerRunner.OnPositionChanged += OnPlayerPositionChanged;
        }
    }

    void GenerateInitialTerritory()
    {
        var vertices = GenerateCircleTerritory();

        CreateTerritory(vertices);
        TerritoryView.name = $"{Runner.name} - Territory";
        TerritoryView.SetTerritory(vertices);
    }

    void CreateTerritory(List<Vector2> vertices)
    {
        Territory = new() { Vertices = vertices };
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

        if (Territory.IsPointInPolygon(currentPosition))
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

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_StartExpanding()
    {
        isExpanding = true;
        playerPath.Clear();
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_StopExpanding()
    {
        playerPath.Clear();
        lineRenderer.positionCount = 0;
        isExpanding = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_AddExpandingPathPoint(Vector2 point)
    {
        playerPath.Add(point);
        lineRenderer.positionCount = playerPath.Count;
        lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_ExpandTerritory()
    {
        Debug.Log($"{Runner.name} - Expanding territory with path: { playerPath.Count}");

        Territory.Expand(playerPath);
        TerritoryView.SetTerritory(Territory.Vertices);

        if (Object.HasStateAuthority)
        {
            OnTerritoryExpandedEvent?.Invoke(Territory, this); // 호스트만
        }
    }
}