using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Launcher
{
    public class NetworkGameTest : LauncherBase
    {
        [SerializeField] NetworkRunner networkRunnerPrefab;
        [SerializeField] Camera mainCamera;
        public NetworkRunner HostRunner;
        public NetworkRunner ClientRunner;

        protected override void OnLauncherInitialized()
        {
            NetworkGameTestGlobal.CurrentLauncher = this;
        }

        protected override void OnLauncherStarted()
        {
            StartAsBuilder();
        }

        async void StartAsBuilder()
        {
            await InitializeHostNetworkRunner();
            await InitializeClientNetworkRunner();
        }

        async Task InitializeHostNetworkRunner()
        {
            HostRunner = Instantiate(networkRunnerPrefab);
            HostRunner.name = "Host Network Runner";

            HostRunner.GetComponent<NetworkEvents>().OnSceneLoadDone.AddListener((runner) =>
            {
                Debug.Log("Scene load");
            });

            var result = await HostRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = "TestSession",
                SceneManager = HostRunner.GetComponent<NetworkSceneManagerDefault>(),
            });

            if (!result.Ok) { Debug.LogError("Failed to start Host Runner."); return; }

            var loadSceneResult = HostRunner.LoadScene("NetworkGameScene", loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
            await loadSceneResult;
            if (loadSceneResult.IsDone)
            {
                Debug.Log("Host Scene loaded Successfully");
            }
        }

        async Task InitializeClientNetworkRunner()
        {
            ClientRunner = Instantiate(networkRunnerPrefab);
            ClientRunner.name = "Client Network Runner";
            var result = await ClientRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = "TestSession",
                SceneManager = ClientRunner.GetComponent<NetworkSceneManagerDefault>(),
                EnableClientSessionCreation = false,
            });

            if (!result.Ok) { Debug.LogError("Failed to start Client Runner."); return; }
        }
    }
}