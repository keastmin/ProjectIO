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
    bool isIntersected = false;

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

        StageManager.Instance.PlayerRunner.OnPositionChanged += OnPlayerPositionChanged;
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

    Vector2 toward;
    public void OnPlayerPositionChanged(PlayerRunner playerRunner) // 러너만
    {
        var currentPosition = new Vector2(playerRunner.transform.position.x, playerRunner.transform.position.z);

        if (Territory.IsPointInPolygon(currentPosition))
        {
            if (!Object.HasStateAuthority) { return; }
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
            if (Object.HasStateAuthority && !isExpanding)
            {
                Debug.Log("나감");
                RPC_StartExpanding();
                RPC_AddExpandingPathPoint(previousPosition);
                RPC_AddExpandingPathPoint(currentPosition);
            }

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                if (Object.HasStateAuthority)
                {
                    if (playerPath.Count >= 2)
                    {
                        toward = (currentPosition - previousPosition).normalized;
                        var dir = Vector2.Dot(toward, (currentPosition - playerPath[^1]).normalized);
                        if (dir < 1 - 0.01f)
                        {
                            RPC_AddExpandingPathPoint(previousPosition);
                        }
                    }

                    // 러너가 자신이 지나온 길을 다시 밟으면 게임 오버
                    CheckPlayerRunnerCrossedOwnPath(currentPosition);
                }

                if (lineRenderer.positionCount > 0)
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(currentPosition.x, 0, currentPosition.y));
                }
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
        lineRenderer.positionCount = playerPath.Count + 1;
        var converted = playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y));
        converted.Add(new Vector3(point.x, 0, point.y));
        lineRenderer.SetPositions(converted.ToArray());
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    public void RPC_ExpandTerritory()
    {
        Debug.Log($"{Runner.name} - Expanding territory with path: {playerPath.Count}");

        Territory.Expand(playerPath);
        TerritoryView.SetTerritory(Territory.Vertices);

        if (Object.HasStateAuthority)
        {
            OnTerritoryExpandedEvent?.Invoke(Territory, this); // 호스트만
        }
    }

    // 플레이어 러너가 이전 경로를 밟았는지 확인하고 밟았다면 게임 오버 처리
    private void CheckPlayerRunnerCrossedOwnPath(Vector2 currPos)
    {
        if (isIntersected) { return; }
        if (CheckCurrPathCrossPrevPath(currPos))
        {
            // GameOver
            Debug.Log("Game Over! Player crossed own path.");
            isIntersected = true;
        }
    }

    // 플레이어 러너의 현재 경로가 이전 경로와 교차했는지 확인
    private bool CheckCurrPathCrossPrevPath(Vector2 currPos)
    {
        int count = playerPath.Count;

        Vector2 prevPos = playerPath[count - 1];

        for (int i = 0; i < count - 2; i++) // 마지막 두 점은 현재 경로이므로 제외
        {
            Vector2 pos1 = playerPath[i];
            Vector2 pos2 = playerPath[i + 1];

            if (Geometry.SegmentIntersection(currPos, prevPos, pos1, pos2, true, out Vector2 intersection))
            {
                Debug.Log($"Intersection at: {intersection}");
                return true;
            }
        }

        // 마지막 선분
        {
            Vector2 pos1 = playerPath[^2];
            Vector2 pos2 = playerPath[^1];
            if (Geometry.SegmentIntersection(currPos, prevPos, pos1, pos2, false, out Vector2 intersection))
            {
                Debug.Log($"Intersection at: {intersection}");
                return true;
            }
        }

        return false;
    }
}