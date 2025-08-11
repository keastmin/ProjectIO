using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionName;
    [SerializeField] private LobbyPlayerSlot _playerSlot1;
    [SerializeField] private LobbyPlayerSlot _playerSlot2;

    [SerializeField] private Button _startButton;

    private void OnDisable()
    {
        ClearLobby();
        _startButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        ClearLobby();

        // PlayerRegistry가 아직 스폰되지 않으면 작동하지 않음
        var playerRegistry = PlayerRegistry.Instance;
        if (playerRegistry)
        {
            // PlayerRegistry가 스폰되면 반복 시작
            foreach (var player in PlayerRegistry.Instance.RefToPosition)
            {
                // 플레이어의 역할군이 변경될 때마다 직접 클라이언트에서 각 역할군 UI Text 변경
                PlayerPositionUpdate(player);
            }

            SetLobby();
            StartButtonActivation();
        }
    }

    private void StartButtonActivation()
    {
        var runner = MatchMaker.Instance.Runner;
        if (runner)
        {
            if (runner.IsServer)
            {
                if (!_startButton.gameObject.activeSelf)
                    _startButton.gameObject.SetActive(true);

                _startButton.interactable = IsPlayersReady();
            }
            else
            {
                if (_startButton.gameObject.activeSelf)
                    _startButton.gameObject.SetActive(false);
            }
        }
    }

    private void PlayerPositionUpdate(KeyValuePair<PlayerRef, PlayerPosition> playerInfos)
    {
        if (MatchMaker.Instance.Runner.LocalPlayer == playerInfos.Key)
        {
            if (MatchMaker.Instance.Runner.IsServer)
            {
                PlayerSlotUpdate(playerInfos.Value, 0);
            }
            else
            {
                PlayerSlotUpdate(playerInfos.Value, 1);
            }
        }
        else
        {
            if (MatchMaker.Instance.Runner.IsServer)
            {
                PlayerSlotUpdate(playerInfos.Value, 1);
            }
            else
            {
                PlayerSlotUpdate(playerInfos.Value, 0);
            }
        }
    }

    private void PlayerSlotUpdate(PlayerPosition position, int slotNumber)
    {
        switch (slotNumber)
        {
            case 0:
                _playerSlot1.Position = position;
                break;
            case 1:
                _playerSlot2.Position = position;
                break;
        }
    }

    private void SetLobby()
    {
        _sessionName.text = MatchMaker.Instance.Runner.SessionInfo.Name;
    }

    public void OnClickPositionChange()
    {
        if (PlayerRegistry.Instance == null) return;
        var key = MatchMaker.Instance.Runner.LocalPlayer;
        PlayerRegistry.Instance.RPC_ChangeRole(key);
    }

    public bool IsPlayersReady()
    {
        if (PlayerRegistry.Instance != null)
        {
            PlayerPosition firstPlayerPosition = PlayerPosition.None;
            int count = 0;
            foreach (var player in PlayerRegistry.Instance.RefToPosition)
            {
                if (count == 0) 
                {
                    firstPlayerPosition = player.Value;
                    count++;
                }
                else
                {
                    if (player.Value != PlayerPosition.None && firstPlayerPosition != player.Value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void ClearLobby()
    {
        _sessionName.text = string.Empty;
        ClearPlayerSlot();
    }

    public void ClearPlayerSlot()
    {
        _playerSlot1.Position = _playerSlot2.Position = PlayerPosition.None;
    }
}
