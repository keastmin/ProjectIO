using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerListUI;
    [SerializeField] private GameObject _laboratoryUI;

    private void Awake()
    {
        _builderMainUI.SetActive(true);
        _towerListUI.SetActive(false);
        _laboratoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _builderMainUI.SetActive(true);
            _towerListUI.SetActive(false);
            _laboratoryUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenTowerListUI();
        }
    }

    public void OpenTowerListUI()
    {
        bool isActive = _towerListUI.activeSelf;

        _builderMainUI.SetActive(isActive);
        _towerListUI.SetActive(!isActive);

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
