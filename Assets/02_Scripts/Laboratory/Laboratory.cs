using Fusion;
using UnityEngine;

public class Laboratory : NetworkBehaviour, ICanClickObject
{

    #region IInteractableObject

    public void OnLeftMouseDownThisObject()
    {

    }

    // 빌더의 연구소를 통한 강화 UI 띄우기
    public void OnLeftMouseUpThisObject()
    {
        var manager = StageManager.Instance;
        if (StageManager.Instance != null)
        {
            StageManager.Instance.UIController.BuilderUI.OnClickLaboratoryButton(true);
        }
    }

    public void OnCancelClickThisObject()
    {

    }

    #endregion
}
