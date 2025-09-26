using UnityEngine;

public class BuilderMainUI : MonoBehaviour
{
    [SerializeField] private GameObject _towerListUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickTowerSelectButton();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            OnClickLaboratoryButton();
        }
    }

    // 타워 선택 UI를 띄우는 함수
    public void OnClickTowerSelectButton()
    {
        _towerListUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    // 연구소 UI를 띄우는 함수
    public void OnClickLaboratoryButton()
    {
        StageManager.Instance.UIController.BuilderUI.OpenLaboratoryUpgradeUI();
    }
}
