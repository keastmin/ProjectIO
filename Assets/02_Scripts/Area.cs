using UnityEngine;

public class Area
{
    /// <summary>
    /// 주어진 점 집합으로 폴리곤 메쉬를 생성하고, GameObject에 MeshFilter, MeshRenderer를 추가하여 씬에 표시합니다.
    /// </summary>
    /// <param name="points">시계방향 또는 반시계방향으로 정렬된 2D 평면상의 점 집합</param>
    /// <param name="material">메쉬에 적용할 머티리얼 (null이면 기본 머티리얼 사용)</param>
    public static GameObject CreatePolygonMesh(Vector2[] points, Material material = null)
    {
        if (points == null || points.Length < 3)
        {
            Debug.LogError("점이 3개 이상 필요합니다.");
            return null;
        }

        // 3D로 변환 (z=0)
        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);

        // 삼각분할 (Triangulator 사용 또는 간단한 팬 방식)
        int[] triangles = new int[(points.Length - 2) * 3];
        for (int i = 0; i < points.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        Mesh mesh = new Mesh();
        mesh.name = "PolygonMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject go = new GameObject("PolygonMesh");
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        if (material != null)
            meshRenderer.material = material;
        else
            meshRenderer.material = new Material(Shader.Find("Standard"));

        return go;
    }

    /// <summary>
    /// 주어진 2D 점이 MeshFilter의 폴리곤 메쉬 내부에 있는지 Raycasting(홀수/짝수 교차) 기법으로 판정합니다.
    /// (z=0 평면 기준)
    /// </summary>
    /// <param name="point">테스트할 2D 점 (월드 좌표계)</param>
    /// <param name="meshFilter">MeshFilter (GameObject의 Transform 포함)</param>
    /// <returns>내부면 true, 외부면 false</returns>
    public static bool IsPointInPolygonMesh(Vector2 point, MeshFilter meshFilter)
    {
        if (meshFilter == null || meshFilter.sharedMesh == null)
            return false;

        var mesh = meshFilter.sharedMesh;
        var vertices = mesh.vertices;
        var tf = meshFilter.transform;

        int n = vertices.Length;
        int crossings = 0;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            // 월드 좌표로 변환
            Vector3 vi = tf.TransformPoint(vertices[i]);
            Vector3 vj = tf.TransformPoint(vertices[j]);
            Vector2 pi = new Vector2(vi.x, vi.y);
            Vector2 pj = new Vector2(vj.x, vj.y);

            // Raycast: point.y가 에지의 y범위 내에 있고, point.x가 에지와의 교차점보다 작은지
            if (((pi.y > point.y) != (pj.y > point.y)))
            {
                float atX = (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y + Mathf.Epsilon) + pi.x;
                if (point.x < atX)
                    crossings++;
            }
        }
        // 홀수면 내부, 짝수면 외부
        return (crossings % 2) == 1;
    }
}