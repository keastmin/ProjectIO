using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image frameImage;

    public void SetActiveSlot(bool isActive)
    {
        frameImage.color = isActive ? Color.red : Color.white;
    }
}