using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TowerSlotUI : MonoBehaviour
{
    public void OnClickTowerSelectButton(TowerData towerData)
    {
        if (StageManager.Instance)
        {
            var builder = StageManager.Instance.PlayerBuilder;
            if (builder)
            {
                builder.ClickTowerSelectButton(towerData);
            }
        }
    }
}
