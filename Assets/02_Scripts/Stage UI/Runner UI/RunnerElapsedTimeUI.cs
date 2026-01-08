using TMPro;
using UnityEngine;

public class RunnerElapsedTimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _elapsedTimeText;

    public void SetElapsedTimeText(string timeText)
        => _elapsedTimeText.text = timeText;
}