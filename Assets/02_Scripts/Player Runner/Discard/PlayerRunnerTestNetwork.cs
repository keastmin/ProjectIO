using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRunnerTestNetwork : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("네트워크 러너")]
    [SerializeField] private NetworkRunner _runnerPrefab;

    [Header("시스템")]
    [SerializeField] private NetworkInputSystem _inputSystemPrefab;
    [SerializeField] private CinemachineSystem _cinemachineSystemPrefab;

    [Header("플레이어 러너")]
    [SerializeField] private PlayerRunner _playerRunnerPrefab;

    private NetworkRunner _runner;

    private void Start()
    {
        StartNetwork();
    }

    async void StartNetwork()
    {
        if (_runner == null)
        {
            _runner = Instantiate(_runnerPrefab);
            _runner.GetComponent<NetworkEvents>().PlayerJoined.AddListener((runner, player) =>
            {
                if (runner.IsServer && player == runner.LocalPlayer)
                {
                    Debug.Log("Input System 스폰 시작");
                    runner.Spawn(_inputSystemPrefab);

                    Debug.Log("Cinemachine System 스폰 시작");
                    // runner.Spawn(_cinemachineSystemPrefab);
                    Instantiate(_cinemachineSystemPrefab);
                }
            });
        }

        _runner.AddCallbacks(this);

        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = _runner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("세션 진입");
        }
    }

    #region INetworkRunnerCallbacks

    // 플레이어가 룸에 입장할 때 호출되는 콜백
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("플레이어 입장");
        if (runner.IsServer)
        {
            // 플레이어 러너 생성
            _runner.Spawn(_playerRunnerPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    #endregion
}
