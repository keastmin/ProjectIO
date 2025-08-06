using Fusion;
using UnityEngine;

public class NetworkGameTestStarter : MonoBehaviour
{
    [SerializeField] NetworkRunner runnerPrefab;
    [SerializeField] bool isHost;
    [SerializeField] NetworkRunner hostRunner;
    [SerializeField] NetworkRunner clientRunner;
    [SerializeField] StageManager stageManager;

    void Start()
    {
        InstantiateRunner();
    }

    async void InstantiateRunner()
    {
        hostRunner = Instantiate(runnerPrefab);
        clientRunner = Instantiate(runnerPrefab);

        // hostRunner.GetComponent<NetworkEvents>().PlayerJoined.AddListener((runner, player) =>
        // {
        //     Debug.Log($"Player {player.PlayerId} joined the game.");
        //     hostRunner.Spawn(stageManager, Vector3.zero, Quaternion.identity);
        // });

        var result = await hostRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host, // 호스트 모드로 시작
            SceneManager = hostRunner.GetComponent<NetworkSceneManagerDefault>() // 기본 씬 매니저 사용
        });

        // 룸 생성에 성공하면 룸 생성 이벤트 호출
        if (!result.Ok)
        {
            return;
        }

        var loadSceneResult = hostRunner.LoadScene("NetworkGameScene", loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
        await loadSceneResult;
        if (loadSceneResult.IsDone)
        {
            Debug.Log("Scene loaded Successfully");
        }

        result = await clientRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client, // 클라이언트 모드로 시작
            // SessionName = InterfaceManager.Instance.JoinSession.RoomCode, // 참여할 룸 코드를 받아옴
            SceneManager = clientRunner.GetComponent<NetworkSceneManagerDefault>(), // 기본 씬 매니저 사용
            EnableClientSessionCreation = false // 클라이언트 세션 생성 비활성화
        });

        // 룸 참여에 성공하면 룸 참여 이벤트 호출
        if (result.Ok)
        {
            Debug.Log("방 참여 성공");
        }
    }
}