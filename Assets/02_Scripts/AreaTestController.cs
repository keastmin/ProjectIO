using System.Collections.Generic;
using UnityEngine;

public class AreaTestController : MonoBehaviour
{
    public bool isCircularMeshGeneration;
    public List<Vector2> polygonPoints;
    public Vector3 mousePosition;
    public Vector3 mousePositionInWorld;
    public bool isMouseInsidePolygon;
    public GameObject go;
    MeshRenderer meshRenderer;

    void Start()
    {
        if (isCircularMeshGeneration)
        {
            polygonPoints.Clear();

            var pointCount = 10;
            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * Mathf.PI * 2 / pointCount;
                var point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                polygonPoints.Add(point);
            }
        }

        RefreshPolygonPoints();
    }

    void Update()
    {
        if (go != null && go.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            var point = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePositionInWorld = point;
            var isInside = Area.IsPointInPolygonMesh(new Vector2(point.x, point.y), meshFilter);
            isMouseInsidePolygon = isInside;
            if (isInside)
            {
                meshRenderer.material.color = Color.green;
            }
            else
            {
                meshRenderer.material.color = Color.red;
            }
        }
    }

    public bool IsInside(Vector2 point)
    {
        if (go != null && go.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            return Area.IsPointInPolygonMesh(point, meshFilter);
        }
        return false;
    }

    public void MemorizePosition(Vector2 position)
    {
        polygonPoints.Add(position);
    }

    public void RefreshPolygonPoints()
    {
        go = Area.CreatePolygonMesh(polygonPoints.ToArray());
        meshRenderer = go.GetComponent<MeshRenderer>();
    }
}