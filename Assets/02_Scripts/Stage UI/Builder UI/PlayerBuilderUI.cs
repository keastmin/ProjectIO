using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject _builderMainUI;
    [SerializeField] private GameObject _towerBuildUI;
    [SerializeField] private GameObject _laboratoryUI;
    [SerializeField] private GameObject _towerSelectUI;
    [SerializeField] private DragSystem _dragSystem;

    [SerializeField] private TextMeshProUGUI _buildUIText;

    public bool IsLaboratoryUIActive => _laboratoryUI.activeSelf;

    #region Action

    public event Action<TowerData> OnClickTowerBuildButtonAction; // 타워 건설 버튼을 눌렀을 때의 액션
    public event Action OnClickLaboratoryButtonAction; // 실험실 버튼을 눌렀을 떄의 액션
    public event Action OnClickSellTowerButtonAction; // 타워 판매 버튼을 눌렀을 때의 액션
    public event Action OnClickMoveTowerButtonAction; // 타워 움직임 버튼을 눌렀을 때의 액션
    public event Action OnClickUpgradeTowerButtonAction; // 타워 속성 부여 버튼을 눌렀을 때의 액션

    #endregion

    #region 프로퍼티

    public DragSystem DragSystem => _dragSystem;

    #endregion

    private void Awake()
    {
        _builderMainUI.SetActive(true);
        _towerBuildUI.SetActive(false);
        _laboratoryUI.SetActive(false);
        _towerSelectUI.SetActive(false);
    }

    #region 클릭 이벤트 메서드

    // 실험실 버튼 클릭 이벤트
    public void OnClickLaboratoryButton(bool isActive)
    {
        OnClickLaboratoryButtonAction?.Invoke();
    }

    // 타워 선택 버튼 클릭 이벤트
    public void OnClickTowerButton(TowerData data)
    {
        OnClickTowerBuildButtonAction?.Invoke(data);
    }

    // 타워 판매 버튼 클릭 이벤트
    public void OnClickTowerSellButton()
    {
        OnClickSellTowerButtonAction?.Invoke();
    }

    // 타워 이동 버튼 클릭 이벤트
    public void OnClickTowerMoveButton()
    {
        OnClickMoveTowerButtonAction?.Invoke();
    }

    // 타워 속성 부여 버튼 클릭 이벤트
    public void OnClickTowerUpgradeButton()
    {
        OnClickUpgradeTowerButtonAction?.Invoke();
    }

    #endregion

    #region UI 활성화/비활성화 메서드

    // 실험실 UI 활성화/비활성화
    public void ActivationLaboratoryUI(bool isActive)
    {
        _builderMainUI.SetActive(!isActive);
        _laboratoryUI.SetActive(isActive);
    }

    // 타워 건설 UI 활성화/비활성화
    public void ActivationTowerBuildUI(bool isActive, string towerBuildInfo = "")
    {
        if(_buildUIText != null)
        {
            _buildUIText.text = towerBuildInfo;
        }

        _builderMainUI.SetActive(!isActive);
        _towerBuildUI.SetActive(isActive);
    }

    // 타워 선택 UI 활성화/비활성화
    public void ActivationTowerSelectUI(bool isActive)
    {
        _builderMainUI.SetActive(!isActive);
        _towerSelectUI.SetActive(isActive);
    }

    #endregion

    // 플레이어 필더가 존재하는지 확인
    private bool IsPlayerBuilderExist(out PlayerBuilder builder)
    {
        builder = null;
        var manager = StageManager.Instance;
        if(manager != null)
        {
            builder = manager.PlayerBuilder;
            if(builder != null)
            {
                return true;
            }
        }
        return false;
    }
}
