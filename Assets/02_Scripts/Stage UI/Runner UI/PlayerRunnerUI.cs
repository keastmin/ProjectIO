using UnityEngine;

public class PlayerRunnerUI : MonoBehaviour
{
    public RunnerDisplay Display;
    public RunnerPopup Popup;

    [SerializeField] ItemSlotUI[] itemSlotViews;

    int selectedItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (var itemSlotView in itemSlotViews)
            {
                itemSlotView.SetActiveSlot(false);
            }
            itemSlotViews[0].SetActiveSlot(true);
            selectedItem = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (var itemSlotView in itemSlotViews)
            {
                itemSlotView.SetActiveSlot(false);
            }
            itemSlotViews[1].SetActiveSlot(true);
            selectedItem = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (var itemSlotView in itemSlotViews)
            {
                itemSlotView.SetActiveSlot(false);
            }
            itemSlotViews[2].SetActiveSlot(true);
            selectedItem = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (var itemSlotView in itemSlotViews)
            {
                itemSlotView.SetActiveSlot(false);
            }
            itemSlotViews[3].SetActiveSlot(true);
            selectedItem = 3;
        }
    }
}