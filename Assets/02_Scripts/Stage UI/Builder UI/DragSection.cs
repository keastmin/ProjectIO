using UnityEngine;

public class DragSection : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Camera _cam;

    private Vector3 _startWorldPos;

    private void Awake()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_cam == null) _cam = Camera.main;
    }

    public void DrawStart(Vector3 worldPos)
    {
        gameObject.SetActive(true);
        _startWorldPos = worldPos;
        Drawing(worldPos);
    }

    public void Drawing(Vector3 currentWorldPos)
    {
        var screenStart = _cam.WorldToScreenPoint(_startWorldPos);
        var screenCurrent = _cam.WorldToScreenPoint(currentWorldPos);
        Debug.Log($"ScreenStart: {screenStart}, ScreenCurrent: {screenCurrent}");
    }

    public void DrawEnd()
    {
        gameObject.SetActive(false);
    }
}
