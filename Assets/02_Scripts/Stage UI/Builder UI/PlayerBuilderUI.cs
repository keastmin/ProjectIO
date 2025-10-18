using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerBuildUI;
    [SerializeField] private GameObject _laboratoryUI;

    public bool IsLaboratoryUIActive => _laboratoryUI.activeSelf;

    private void Update()
    {
        if(IsLaboratoryUIActive && Input.GetMouseButtonDown(1))
        {
            OnClickLaboratoryButton(false);
        }
    }

    // 타워 선택 버튼 클릭 이벤트
    public void OnClickTowerButton(TowerData data)
    {
        var manager = StageManager.Instance;
        if(manager != null)
        {
            var builder = manager.PlayerBuilder;
            if(builder != null)
            {
                builder.StandByTowerBuild(data);
            }
        }
    }

    // 실험실 버튼 클릭 이벤트
    public void OnClickLaboratoryButton(bool isActive)
    {
        _builderMainUI.SetActive(!isActive);
        _laboratoryUI.SetActive(isActive);
    }

    // 타워 건설 UI 활성화/비활성화
    public void ActivationTowerBuildUI(bool isActive)
    {
        _builderMainUI.SetActive(!isActive);
        _towerBuildUI.SetActive(isActive);
    }
}
