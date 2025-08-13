using System.Collections.Generic;
using UnityEngine;

public class Centipede : LocalMonster
{
    public Transform head;
    public GameObject segmentPrefab;
    [SerializeField] protected int segmentCount = 5;
    [SerializeField] protected float segmentDistance = 0.5f;
    [SerializeField] protected float moveCountThreshold = 10f;
    List<Transform> segments = new List<Transform>();
    List<Vector3> originalSegmentPositions = new List<Vector3>();
    List<Vector3> segmentPositions = new List<Vector3>();
    int moveCount = 0;

    protected void Start()
    {
        InitializeSegments();
    }

    void InitializeSegments()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform);
            segment.name = $"Segment_{i}";
            Transform segmentTransform = segment.transform;
            segmentTransform.position = head.position;
            segments.Add(segmentTransform);
            originalSegmentPositions.Add(segmentTransform.position);
            segmentPositions.Add(segmentTransform.position);
        }
    }

    protected override void Update()
    {
        base.Update();
        FollowSegments();
    }

    protected virtual void FollowSegments()
    {
        moveCount++;
        if (moveCount > moveCountThreshold)
        {
            moveCount = 0;
            Vector3 targetPosition = head.position;
            for (int i = 0; i < segmentCount; i++)
            {
                originalSegmentPositions[i] = segmentPositions[i];
            }
            for (int i = segmentCount - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    segmentPositions[i] = targetPosition;
                }
                else
                {
                    segmentPositions[i] = segmentPositions[i - 1];
                }
            }
        }
        for (int i = 0; i < segmentCount; i++)
        {
            Transform segment = segments[i];
            Vector3 targetPosition = segmentPositions[i];
            segment.position = Vector3.Lerp(originalSegmentPositions[i], targetPosition, moveCount / moveCountThreshold);
        }
    }
}