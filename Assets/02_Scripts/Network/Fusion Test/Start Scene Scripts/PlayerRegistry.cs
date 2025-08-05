using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static PlayerRegistry Instance { get; private set; }

    [Networked, Capacity(2)]
    public NetworkDictionary<PlayerRef, PlayerPosition> PlayerInfos { get; }

    private void Awake()
    {
        Debug.Log("스폰됨");

        // 싱글턴 인스턴스 설정
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public override void Spawned()
    {
        Runner.AddCallbacks(this);
        DontDestroyOnLoad(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Instance = null;
        runner.RemoveCallbacks(this);
    }

    // 플레이어 딕셔너리에 추가
    public void AddPlayerInfo(PlayerRef player)
    {
        Debug.Log("플레이어 추가");
        if (!PlayerInfos.ContainsKey(player))
        {
            PlayerInfos.Add(player, PlayerPosition.Builder);
        }
    }

    // 플레이어 딕셔너리에서 제거
    public void RemovePlayerInfo(PlayerRef player)
    {
        if (PlayerInfos.ContainsKey(player))
        {
            PlayerInfos.Remove(player);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangeRole(PlayerRef player)
    {
        if (Object.HasStateAuthority && PlayerInfos.ContainsKey(player))
        {
            var currentRole = PlayerInfos.Get(player);
            var newRole = currentRole == PlayerPosition.Builder ? PlayerPosition.Runner : PlayerPosition.Builder;
            PlayerInfos.Set(player, newRole);
        }
    }

    #region INetworkRunnerCallbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerJoined called for: {player} by runner: {runner.name} (isServer: {runner.IsServer}, isLocalPlayer: {player == runner.LocalPlayer})");
        if (runner.IsServer)
        {
            AddPlayerInfo(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            RemovePlayerInfo(player);
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
