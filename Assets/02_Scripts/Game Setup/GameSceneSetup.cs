using Fusion;
using UnityEngine;

public class GameSceneSetup : MonoBehaviour
{
    private TestModeGameSceneSetup _testModeSetup;

    private void Awake()
    {
        _testModeSetup = GetComponent<TestModeGameSceneSetup>();
    }

    private void Start()
    {
        // 플레이를 시작한 종류에 따라 셋업 진행
        ModeSetup();
    }

    /// <summary>
    /// 테스트 모드로 게임을 시작했는지, 로비 절차를 거쳐 게임을 시작했는지에 따라 셋업 방식을 다르게 하는 함수
    /// </summary>
    private void ModeSetup()
    {
        // 네트워크 매니저 존재 여부 확인
        bool isExist = CheckExistNetworkManager();

        // 존재한다면 정식 게임 모드, 존재하지 않는다면 테스트 모드
        if (isExist)
        {           
            Debug.Log("정식 게임 모드로 게임 셋업");
        }
        else
        {
            Debug.Log("테스트 모드로 게임 셋업");
            StartTestMode();
        }
    }

    /// <summary>
    /// 네트워크 매니저가 존재하는지 확인하는 함수
    /// </summary>
    /// <returns>네트워크 매니저가 존재하면 true, 아니라면 false</returns>
    private bool CheckExistNetworkManager()
    {
        return (NetworkManager.Instance != null);
    }

    /// <summary>
    /// 테스트 모드로 게임 환경 구축 시작
    /// </summary>
    private void StartTestMode()
    {
        _testModeSetup.BuildTestModeEnvironment();
    }
}
