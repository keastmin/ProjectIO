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
    }

    // 타워 선택 UI를 띄우는 함수
    public void OnClickTowerSelectButton()
    {
        _towerListUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
