using UnityEngine;

public class BuilderAttackTowerListUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderTowerUI;
    [SerializeField] private TowerData _sentryGunData;
    [SerializeField] private TowerData _laserData;
    [SerializeField] private TowerData _missileData;
    [SerializeField] private TowerData _bladeData;

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
            OnClickTowerButton(_sentryGunData);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            OnClickTowerButton(_laserData);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickTowerButton(_missileData);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            OnClickTowerButton(_bladeData);
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
