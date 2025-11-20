using UnityEngine;

public class DragSection : MonoBehaviour
{
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Camera _cam;

    private Vector3 _startWorldPos;
    public Vector3 StartWorldPos => _startWorldPos;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _cam = Camera.main;
    }

    public void DrawStart(Vector3 worldPos, Vector2 mousePos)
    {
        _startWorldPos = worldPos;
        Drawing(mousePos);
    }

    public void Drawing(Vector2 mousePos)
    {
        Vector2 startAnchor = default;
        Vector2 endAnchor = default;
        float width = 0f;
        float height = 0f;

        if (WorldToAnchoredPosition(_rectTransform, _startWorldPos, _canvas, _cam, out startAnchor))
        {
            if(ScreenToAnchoredPosition(_rectTransform, mousePos, _canvas, out endAnchor))
            {
                width = Mathf.Abs(endAnchor.x - startAnchor.x);
                height = Mathf.Abs(endAnchor.y - startAnchor.y);
                Vector2 center = (startAnchor + endAnchor) * 0.5f;
                _rectTransform.anchoredPosition = center;
                _rectTransform.sizeDelta = new Vector2(width, height);
            }
        }
    }

    private bool WorldToAnchoredPosition(RectTransform target, Vector3 worldPosition, Canvas canvas, Camera sceneCam, out Vector2 anchored)
    {
        anchored = default;

        if (target == null) return false;
        var parent = target.parent as RectTransform;
        if (parent == null) return false;

        Camera uiCam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(sceneCam ?? uiCam, worldPosition);

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, uiCam, out var parentLocal))
            return false;

        var pr = parent.rect;
        Vector2 anchorCenterInParent =
            new Vector2(
                Mathf.Lerp(pr.xMin, pr.xMax, (target.anchorMin.x + target.anchorMax.x) * 0.5f),
                Mathf.Lerp(pr.yMin, pr.yMax, (target.anchorMin.y + target.anchorMax.y) * 0.5f)
                );

        anchored = parentLocal - anchorCenterInParent;
        return true;
    }

    private bool ScreenToAnchoredPosition(RectTransform target, Vector2 screenPosition, Canvas canvas, out Vector2 anchored)
    {
        anchored = default;
        if (!target) return false;

        var parent = target.parent as RectTransform;
        if (!parent) return false;

        // Overlay는 null, 그 외(Camera/World Space)는 canvas.worldCamera 사용
        Camera uiCam = (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;

        // 1) 스크린 → 부모 로컬(부모 pivot 기준)
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, uiCam, out var parentLocal))
            return false;

        // 2) 부모 로컬에서 내 앵커 사각형의 "중심" 좌표 계산
        var pr = parent.rect;
        var anchorCenter = new Vector2(
            Mathf.Lerp(pr.xMin, pr.xMax, (target.anchorMin.x + target.anchorMax.x) * 0.5f),
            Mathf.Lerp(pr.yMin, pr.yMax, (target.anchorMin.y + target.anchorMax.y) * 0.5f)
        );

        // anchoredPosition = 부모로컬 - 앵커중심
        anchored = parentLocal - anchorCenter;
        return true;
    }
}
