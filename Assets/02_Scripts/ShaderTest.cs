using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    private MaterialPropertyBlock mpb;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition; // x, y 좌표
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 worldPos = hitInfo.point;
            UpdateMousePosition(worldPos);
        }
    }

    private void UpdateMousePosition(Vector3 worldPos)
    {
        _renderer.GetPropertyBlock(mpb);
        mpb.SetVector("_MousePos", new Vector2(worldPos.x, worldPos.z));
        _renderer.SetPropertyBlock(mpb);
    }
}