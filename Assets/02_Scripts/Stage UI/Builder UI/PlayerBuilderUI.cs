using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerListUI;
    [SerializeField] private GameObject _attackTowerListUI;
    [SerializeField] private GameObject _centerTowerListUI;
    [SerializeField] private GameObject _supportTowerListUI;
    [SerializeField] private GameObject _laboratoryUI;

    private List<GameObject> _uiList;
    private GameObject _lastUI;

    private void Awake()
    {
        _uiList = new List<GameObject>() { _builderMainUI, _towerListUI, _attackTowerListUI,
        _centerTowerListUI, _supportTowerListUI, _laboratoryUI};
        InitUIState();
    }

    // UI 상태 초기화
    private void InitUIState()
    {
        _builderMainUI.SetActive(true);
        _towerListUI.SetActive(false);
        _attackTowerListUI.SetActive(false);
        _centerTowerListUI.SetActive(false);
        _supportTowerListUI.SetActive(false);
        _laboratoryUI.SetActive(false);
    }

    // 연구소 UI를 여는 로직
    public void OpenLaboratoryUpgradeUI()
    {
        bool isActive = _laboratoryUI.activeSelf;

        _laboratoryUI.SetActive(!isActive);
        if (isActive)
        {
            _lastUI.SetActive(true);
            _lastUI = null;
        }
        else
        {
            for (int i = 0; i < _uiList.Count; i++)
            {
                if (_uiList[i].activeSelf)
                {
                    _lastUI = _uiList[i];
                    _uiList[i].SetActive(false);
                    break;
                }
            }
        }
    }
}
