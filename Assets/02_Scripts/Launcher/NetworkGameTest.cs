using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Launcher
{
    public class NetworkGameTest : LauncherBase
    {
        [Header("Network")]
        [SerializeField] NetworkRunner networkRunnerPrefab;

        [Header("Host Role")]
        [SerializeField] PlayerPosition hostPlayerPosition;

        NetworkRunner hostRunner;
        NetworkRunner clientRunner;

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
            var sessionUUID = System.Guid.NewGuid().ToString();
            await InitializeHostNetworkRunner(sessionUUID);
            await InitializeClientNetworkRunner(sessionUUID);
            await LoadScene();
        }

        async Task InitializeHostNetworkRunner(string sessionId)
        {
            hostRunner = Instantiate(networkRunnerPrefab);
            hostRunner.name = "Host Network Runner";

            var result = await hostRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = sessionId,
                SceneManager = hostRunner.GetComponent<NetworkSceneManagerDefault>(),
                SessionProperties = new Dictionary<string, SessionProperty>
                {
                    { "IsHostBuilder", hostPlayerPosition == PlayerPosition.Builder },
                }
            });

            if (!result.Ok) { Debug.LogError("Failed to start Host Runner."); return; }
        }

        async Task InitializeClientNetworkRunner(string sessionId)
        {
            clientRunner = Instantiate(networkRunnerPrefab);
            clientRunner.name = "Client Network Runner";

            var result = await clientRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = sessionId,
                SceneManager = clientRunner.GetComponent<NetworkSceneManagerDefault>(),
                EnableClientSessionCreation = false,
            });

            if (!result.Ok) { Debug.LogError("Failed to start Client Runner."); return; }
        }

        async Task LoadScene()
        {
            await hostRunner.LoadScene("NetworkGameScene", loadSceneMode: LoadSceneMode.Additive);
        }
    }
}