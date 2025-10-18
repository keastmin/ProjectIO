using UnityEngine;

public class BuilderCenterTowerListUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderTowerUI;
    [SerializeField] private TowerData _centerTowerData;

    private void Update()
    {
        bool isBuilderStandByTowerBuild = StageManager.Instance.PlayerBuilder.IsStandByTowerBuild;
        if (Input.GetMouseButtonDown(1) && !isBuilderStandByTowerBuild)
        {
            // 타워 설치 대기중이 아닐 때에만 작동
            OnMouseButtonCancel();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            OnClickTowerButton(_centerTowerData);
        }
    }

    public void OnClickTowerButton(TowerData towerData)
    {
        StageManager.Instance.PlayerBuilder.StandByTowerBuild(towerData);
    }

    private void OnMouseButtonCancel()
    {
        _builderTowerUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
