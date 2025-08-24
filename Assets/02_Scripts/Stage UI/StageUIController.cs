using UnityEngine;
using UnityEngine.Events;

public class StageUIController : MonoBehaviour
{
    [SerializeField] private GameObject _builderUI; // 플레이어 빌더가 보게 될 UI 오브젝트
    [SerializeField] private GameObject _runnerUI; // 플레이어 러너가 보게 될 UI 오브젝트

    private void Awake()
    {
        SetDisableAllUI(); // 시작할 때 모든 UI 비활성화
    }

    /// <summary>
    /// 자신의 역할군에 따라 맞는 UI를 활성화
    /// </summary>
    /// <param name="playerPosition">자신의 역할군</param>
    public void SetPlayerUI(PlayerPosition playerPosition)
    {
        switch (playerPosition)
        {
            case PlayerPosition.Builder:
                _builderUI.SetActive(true);
                break;
            case PlayerPosition.Runner:
                _runnerUI.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 모든 UI를 비활성화
    /// </summary>
    public void SetDisableAllUI()
    {
        _builderUI.SetActive(false);
        _runnerUI.SetActive(false);
    }
}
