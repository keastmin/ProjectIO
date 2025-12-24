using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionName;
    [SerializeField] private TextMeshProUGUI[] _slots;

    [SerializeField] private Button _startButton;

    private void Update()
    {
        // PlayerRegistry가 아직 스폰되지 않으면 작동하지 않음
        var playerRegistry = NetworkManager.Instance.Registry;
        if (playerRegistry)
        {
            int myIndex = GetMySlotIndex(playerRegistry); // 내 인덱스 저장
            int otherIndex = myIndex == 0 ? 1 : 0; // 나와 다른 인덱스 저장

            SetSlotPositionText(playerRegistry, myIndex, otherIndex);

            StartButtonActivation();
        }
    }

    // 내 슬롯 번호를 반환하는 함수
    private int GetMySlotIndex(PlayerRegistry registry)
    {
        return registry.Runner.IsServer ? 0 : 1;
    }

    // 슬롯의 역할군 표시
    private void SetSlotPositionText(PlayerRegistry registry, int myIndex, int otherIndex)
    {
        // 슬롯 초기화
        _slots[myIndex].text = "None";
        _slots[otherIndex].text = "None";

        // 플레이어 정보 순회
        foreach(var player in registry.RefToPosition)
        {
            var position = player.Value;

            if(player.Key == registry.Runner.LocalPlayer)
            {
                _slots[myIndex].text = (position == PlayerPosition.Builder) ? "Builder" : "Runner";
            }
            else
            {
                _slots[otherIndex].text = (position == PlayerPosition.Builder) ? "Builder" : "Runner";
            }
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


    public void OnClickPositionChange()
    {
        if (NetworkManager.Instance.Registry == null) return;
        var key = MatchMaker.Instance.Runner.LocalPlayer;
        NetworkManager.Instance.Registry.RPC_ChangeRole(key);
    }

    public bool IsPlayersReady()
    {
        if (NetworkManager.Instance.Registry != null)
        {
            PlayerPosition firstPlayerPosition = PlayerPosition.Builder;
            int count = 0;
            foreach (var player in NetworkManager.Instance.Registry.RefToPosition)
            {
                if (count == 0) 
                {
                    firstPlayerPosition = player.Value;
                    count++;
                }
                else
                {
                    if (player.Value != PlayerPosition.Builder && firstPlayerPosition != player.Value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
