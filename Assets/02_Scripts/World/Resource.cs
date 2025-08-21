using Fusion;
using UnityEngine;

public class Resource : NetworkBehaviour
{
    public ResourceType Type;
    public int Amount = 5;

    public void OnTerritoryExpanded(Territory territory, TerritorySystem territorySystem)
    {
        var xzPosition = new Vector3(transform.position.x, transform.position.z);
        if (territory.IsPointInPolygon(xzPosition))
        {
            territorySystem.OnTerritoryExpandedEvent -= OnTerritoryExpanded;
            switch (Type)
            {
                case ResourceType.Mineral:
                    StageManager.Instance.ResourceSystem.Mineral += Amount;
                    break;
                case ResourceType.Gas:
                    StageManager.Instance.ResourceSystem.Gas += Amount;
                    break;
            }
            Debug.Log($"Obtained {Amount} resources from {gameObject.name}");
            Runner.Despawn(Object);
        }
    }
}