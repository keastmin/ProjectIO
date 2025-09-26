using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerListUI;
    [SerializeField] private GameObject _laboratoryUI;

    private void OnEnable()
    {
        _builderMainUI.SetActive(true);
        _towerListUI.SetActive(false);
        _laboratoryUI.SetActive(false);
    }

    public void OpenLaboratoryUpgradeUI()
    {
        bool isActive = _laboratoryUI.activeSelf;

        _builderMainUI.SetActive(isActive);
        _laboratoryUI.SetActive(!isActive);

        _towerListUI.SetActive(false);
    }
}
