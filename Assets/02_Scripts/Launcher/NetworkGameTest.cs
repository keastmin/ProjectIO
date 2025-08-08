using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Launcher
{
    public class NetworkGameTest : LauncherBase
    {
        [SerializeField] NetworkRunner networkRunnerPrefab;
        [SerializeField] PlayerPosition hostPlayerPosition;
        public NetworkRunner HostRunner;
        public NetworkRunner ClientRunner;

        string sessionUUID;

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
            sessionUUID = System.Guid.NewGuid().ToString();
            await InitializeHostNetworkRunner();
            await InitializeClientNetworkRunner();
            await LoadScene();
        }

        async Task InitializeHostNetworkRunner()
        {
            HostRunner = Instantiate(networkRunnerPrefab);
            HostRunner.name = "Host Network Runner";

            var result = await HostRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = sessionUUID,
                SceneManager = HostRunner.GetComponent<NetworkSceneManagerDefault>(),
                SessionProperties = new Dictionary<string, SessionProperty>
                {
                    { "IsHostBuilder", hostPlayerPosition == PlayerPosition.Builder },
                }
            });

            if (!result.Ok) { Debug.LogError("Failed to start Host Runner."); return; }
        }

        async Task InitializeClientNetworkRunner()
        {
            ClientRunner = Instantiate(networkRunnerPrefab);
            ClientRunner.name = "Client Network Runner";

            var result = await ClientRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = sessionUUID,
                SceneManager = ClientRunner.GetComponent<NetworkSceneManagerDefault>(),
                EnableClientSessionCreation = false,
            });

            if (!result.Ok) { Debug.LogError("Failed to start Client Runner."); return; }
        }

        async Task LoadScene()
        {
            var loadSceneResult = HostRunner.LoadScene("NetworkGameScene", loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
            await loadSceneResult;
            if (loadSceneResult.IsDone)
            {
                Debug.Log("Host Scene loaded Successfully");
            }
        }
    }
}