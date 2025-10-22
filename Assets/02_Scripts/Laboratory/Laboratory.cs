using Fusion;
using UnityEngine;

public class Laboratory : NetworkBehaviour, IInteractableObject
{

    #region IInteractableObject

    // 빌더의 연구소를 통한 강화 UI 띄우기
    public void OnClickThisObject()
    {
        var manager = StageManager.Instance;
        if (StageManager.Instance != null)
        {
            StageManager.Instance.UIController.BuilderUI.OnClickLaboratoryButton(true);
        }
    }

    public void OnDragThisObject()
    {

    }

    #endregion
}
