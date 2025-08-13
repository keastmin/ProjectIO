using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalTerritorySystem : SystemBase
{
    [Header("Initial Territory")]
    [SerializeField] int circlePointCount;
    [SerializeField] float circleRadius;

    [Header("Expanding")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] bool isExpanding;
    [SerializeField] Vector2 previousPosition;
    [SerializeField] List<Vector2> playerPath;
    [SerializeField] UnityEvent<Territory, LocalTerritorySystem> onTerritoryExpandedEvent;

    public Territory Territory;
    public TerritoryView TerritoryView;

    public event Action<Territory, LocalTerritorySystem> OnTerritoryExpandedEvent;

    public override void SetUp()
    {
        GenerateInitialTerritory();
    }

    void GenerateInitialTerritory()
    {
        var vertices = GenerateCircleTerritory();

        CreateTerritory(vertices);
        TerritoryView.name = $"Local Territory";
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

    public void OnPlayerPositionChanged(LocalPlayerRunner playerRunner) // 러너만
    {
        var currentPosition = new Vector2(playerRunner.transform.position.x, playerRunner.transform.position.z);

        if (Territory.IsPointInPolygon(currentPosition))
        {
            if (isExpanding)
            {
                if (playerPath.Count > 1)
                {
                    AddExpandingPathPoint(currentPosition);
                    ExpandTerritory();
                }
                StopExpanding();
                Debug.Log("다시 들어옴");
            }
            previousPosition = currentPosition;
        }
        else
        {
            if (!isExpanding)
            {
                Debug.Log("나감");
                StartExpanding();
                AddExpandingPathPoint(previousPosition);
                AddExpandingPathPoint(currentPosition);
            }

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                AddExpandingPathPoint(currentPosition);
                previousPosition = currentPosition;
            }
        }
    }

    public void StartExpanding()
    {
        isExpanding = true;
        playerPath.Clear();
    }

    public void StopExpanding()
    {
        playerPath.Clear();
        lineRenderer.positionCount = 0;
        isExpanding = false;
    }

    public void AddExpandingPathPoint(Vector2 point)
    {
        playerPath.Add(point);
        lineRenderer.positionCount = playerPath.Count;
        lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
    }

    public void ExpandTerritory()
    {
        Debug.Log($"Local - Expanding territory with path: {playerPath.Count}");

        Territory.Expand(playerPath);
        TerritoryView.SetTerritory(Territory.Vertices);

        onTerritoryExpandedEvent?.Invoke(Territory, this);
        OnTerritoryExpandedEvent?.Invoke(Territory, this);
    }

    // void Awake()
    // {
    //     player.OnPositionChanged += HandlePlayerPositionChanged;
    // }

    // void HandlePlayerPositionChanged(RunnerTestPlayer player)
    // {
    //     if (localTerritory == null) { return; }

    //     var currentPosition = new Vector2(player.transform.position.x, player.transform.position.z);
    //     if (localTerritory.IsPointInPolygon(currentPosition))
    //     {
    //         if (isExpanding)
    //         {
    //             if (playerPath.Count > 1)
    //             {
    //                 playerPath.Add(currentPosition);
    //                 Debug.Log("확장됨");
    //                 localTerritory.Expand(playerPath);
    //                 if (resourceObtainingSystem != null)
    //                 {
    //                     resourceObtainingSystem.TryObtainResources(localTerritory);
    //                 }
    //                 OnTerritoryExpandedEvent?.Invoke(localTerritory, this);
    //             }
    //             playerPath.Clear();
    //             lineRenderer.positionCount = 0;
    //             isExpanding = false;
    //             Debug.Log("다시 들어옴");
    //         }
    //         previousPosition = currentPosition;
    //     }
    //     else
    //     {
    //         if (!isExpanding)
    //         {
    //             isExpanding = true;
    //             Debug.Log("나감");
    //             playerPath.Clear();
    //             playerPath.Add(previousPosition);
    //             playerPath.Add(currentPosition);
    //             lineRenderer.positionCount = playerPath.Count;
    //             lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
    //         }

    //         if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
    //         {
    //             playerPath.Add(currentPosition);
    //             previousPosition = currentPosition;
    //             lineRenderer.positionCount = playerPath.Count;
    //             lineRenderer.SetPositions(playerPath.ConvertAll(p => new Vector3(p.x, 0, p.y)).ToArray());
    //         }
    //     }
    // }

    // public void GenerateInitialTerritory()
    // {
    //     var polygonPoints = new List<Vector2>();
    //     for (int i = 0; i < circlePointCount; i++)
    //     {
    //         float angle = (circlePointCount - 1 - i) * Mathf.PI * 2 / circlePointCount;
    //         var point = new Vector2(circleRadius * Mathf.Cos(angle), circleRadius * Mathf.Sin(angle));
    //         polygonPoints.Add(point);
    //     }

    //     localTerritory = Territory.CreatePolygonMesh(polygonPoints.ToArray());
    // }
}