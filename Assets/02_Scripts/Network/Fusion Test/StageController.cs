using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class StageController : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    [Networked, Capacity(2)]
    public NetworkDictionary<PlayerRef, Player> Players { get; }

    private PlayerBuilder _playerBuilder;
    private PlayerRunner _playerRunner;

    private int count = 0;

    public override void Spawned()
    {
        Debug.Log("스폰됌");
        Runner.AddCallbacks(this);
        SpawnPlayer();
    }

    public override void Render()
    {
        SetCamera();
    }

    private void SpawnPlayer()
    {
        var playerRegistry = PlayerRegistry.Instance;
        if (playerRegistry && Runner.IsServer)
        {
            foreach (var player in playerRegistry.PlayerInfos)
            {
                var playerPosition = player.Value;
                if (playerPosition == PlayerPosition.Builder)
                {
                    SpawnPlayerBuilder(player.Key);
                }
                else if (playerPosition == PlayerPosition.Runner)
                {
                    SpawnPlayerRunner(player.Key);
                }
            }
        }
    }

    private void SpawnPlayerBuilder(PlayerRef player)
    {
        _playerBuilder = Runner.Spawn(ResourceManager.Instance.PlayerBuilderPrefab, Vector3.zero, Quaternion.identity, player);
        Players.Add(player, _playerBuilder);
    }

    private void SpawnPlayerRunner(PlayerRef player)
    {
        _playerRunner = Runner.Spawn(ResourceManager.Instance.PlayerRunnerPrefab, Vector3.zero, Quaternion.identity, player);
        Players.Add(player, _playerRunner);
    }

    private void Update()
    {
        // 테스트용 권한 변경
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePlayerAuthority();
        }
    }

    private void ChangePlayerAuthority()
    {
        foreach(var player in PlayerRegistry.Instance.PlayerInfos)
        {
            PlayerRegistry.Instance.RPC_ChangeRole(player.Key);
        }
        RPC_ChangeAuthority();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeAuthority()
    {
        var builderObject = _playerBuilder.Object;
        var runnerObject = _playerRunner.Object;

        var builderRef = builderObject.InputAuthority;
        var runnerRef = runnerObject.InputAuthority;

        builderObject.AssignInputAuthority(runnerRef);
        runnerObject.AssignInputAuthority(builderRef);

        Players.Set(builderRef, _playerRunner);
        Players.Set(runnerRef, _playerBuilder);
    }

    public void SetCamera()
    {
        if(count < 3)
        {
            count++;
            Debug.Log("카메라 설정 중");
        }

        var playerRegistry = PlayerRegistry.Instance;
        if (playerRegistry && _cinemachineCamera && Runner)
        {
            var position = playerRegistry.PlayerInfos[Runner.LocalPlayer];
            if (position == PlayerPosition.Runner)
            {
                _cinemachineCamera.Target.TrackingTarget = Players[Runner.LocalPlayer].transform;
            }
            else
            {
                _cinemachineCamera.Target.TrackingTarget = null;
            }
        }
    }

    #region INetworkRunnerCallbacks

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    #endregion
}