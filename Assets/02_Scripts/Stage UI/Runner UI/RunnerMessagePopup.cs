using TMPro;
using UnityEngine;

public class RunnerMessagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;

    public void ShowMessage(string message)
        => _messageText.text = message;
}