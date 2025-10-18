using UnityEngine;

public class BuilderLaboratoryUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
        {
            StageManager.Instance.UIController.BuilderUI.OpenLaboratoryUpgradeUI();
        }
    }
}
