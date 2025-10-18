using Fusion;
using UnityEngine;

public class Laboratory : NetworkBehaviour, IInteractableObejct
{

    #region IInteractableObject

    // 빌더의 연구소를 통한 강화 UI 띄우기
    public void OnClickThisObject()
    {
        StageManager.Instance.UIController.BuilderUI.OpenLaboratoryUpgradeUI();
    }

    #endregion
}
