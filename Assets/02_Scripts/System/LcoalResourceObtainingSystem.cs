using TMPro;
using UnityEngine;

public class LocalResourceObtainingSystem : MonoBehaviour
{
    [SerializeField] LocalTerritoryExpandingSystem territoryExpandingSystem;
    [SerializeField] LocalRunner runner;
    [SerializeField] Transform fieldTransform;
    [SerializeField] TextMeshProUGUI mineralText;
    [SerializeField] TextMeshProUGUI gasText;

    public void TryObtainResources(LocalTerritory territory)
    {
        foreach (var resource in fieldTransform.GetComponentsInChildren<Resource>())
        {
            var xzPosition = new Vector2(resource.transform.position.x, resource.transform.position.z);
            if (territory.IsPointInPolygon(xzPosition))
            {
                if (resource.Type == ResourceType.Mineral)
                {
                    runner.MineralAmount += resource.Amount;
                    Destroy(resource.gameObject);
                    mineralText.text = $"Minerals: {runner.MineralAmount}";
                }
                else if (resource.Type == ResourceType.Gas)
                {
                    runner.GasAmount += resource.Amount;
                    Destroy(resource.gameObject);
                    gasText.text = $"Gas: {runner.GasAmount}";
                }
            }
        }
    }
}