using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerListUI;

    private void Awake()
    {
        _builderMainUI.SetActive(true);
        _towerListUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickTowerButton();
        }
    }

    public void OnClickTowerButton()
    {
        bool isActive = _towerListUI.activeSelf;

        _builderMainUI.SetActive(isActive);
        _towerListUI.SetActive(!isActive);
    }
}
