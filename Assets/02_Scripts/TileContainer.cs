using UnityEngine;

public class TileContainer : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    public void CreateTile(Vector3 position)
    {
        var tileObject = Instantiate(tilePrefab, position, Quaternion.identity);
        tileObject.transform.SetParent(transform);
    }
}