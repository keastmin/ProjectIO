using UnityEngine;
using UnityEngine.UI;

public class TowerSlotUI : MonoBehaviour
{
    public Button[] TowerSlots;

    public void OnClickTowerSelectButton(TowerData towerData)
    {
        if (StageManager.Instance)
        {
            var builder = StageManager.Instance.PlayerBuilder;
            if (builder)
            {
                builder.SetTowerData(towerData);
            }
        }
    }
}
