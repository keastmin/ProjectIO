using System.Collections.Generic;
using UnityEngine;

public class BuilderTowerUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _attackTowerListUI;
    [SerializeField] private GameObject _centerTowerListUI;
    [SerializeField] private GameObject _supportTowerListUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnClickAttackTowerListButton();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            OnClickCenterTowerListButton();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnClickSupportTowerListButton();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnMouseButtonCancel();
        }
    }

    public void OnClickAttackTowerListButton()
    {
        _attackTowerListUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnClickCenterTowerListButton()
    {
        _centerTowerListUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnClickSupportTowerListButton()
    {
        _supportTowerListUI.SetActive(true);
        gameObject.SetActive(false);
    }

    // 현재 UI 취소
    private void OnMouseButtonCancel()
    {
        _builderMainUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
