using UnityEngine;

public class LocalTrackGenerationSystem : MonoBehaviour
{
    [SerializeField] Transform positionParentTransform;
    [SerializeField] float horizontalRadius;
    [SerializeField] float verticalRadius;
    [SerializeField] float noise;
    [SerializeField] int pointCount;
    [SerializeField] LineRenderer lineRenderer;

    public Track Track;

    public void GenerateTrack()
    {
        for (int i = positionParentTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(positionParentTransform.GetChild(i).gameObject);
        }

        Track = new Track
        {
            Vertices = new Vector3[pointCount],
        };

        for (int i = 0; i < pointCount; i++)
        {
            float angle = Mathf.PI + 2 * Mathf.PI * i / pointCount;
            float x = Mathf.Cos(angle) * horizontalRadius;
            float z = Mathf.Sin(angle) * verticalRadius;
            // 노이즈 적용
            x += Random.Range(-noise, noise);
            z += Random.Range(-noise, noise);

            var point = new Vector3(x, 0, z);
            Track.Vertices[i] = point;
            GameObject pointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointObject.transform.SetParent(positionParentTransform, false);
            pointObject.transform.localPosition = point;
            pointObject.transform.localScale = Vector3.one * 0.5f; // 크기 조정
            pointObject.name = $"TrackPoint_{i}";
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(Track.Vertices);
        lineRenderer.loop = true; // 선을 닫아 원형 트랙을 만듭니다.
    }
}