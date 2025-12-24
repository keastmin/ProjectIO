using Fusion;
using System;
using TMPro;
using UnityEngine;

public class ResourceSystem : NetworkBehaviour
{
    public static ResourceSystem Instance;

    [SerializeField] private TextMeshProUGUI _mineralText;
    [SerializeField] private TextMeshProUGUI _gasText;

    [Networked, OnChangedRender(nameof(OnChangedMineralCount))] public int Mineral { get; set; }
    [Networked, OnChangedRender(nameof(OnChangedGasCount))] public int Gas { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 자신이 빌더라면 작동
        if (Input.GetKeyDown(KeyCode.Space) && NetworkManager.Instance.Registry.RefToPosition[Runner.LocalPlayer] == PlayerPosition.Builder)
        {
            RPC_GetMineral(5);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GetMineral(int mineral)
    {
        Mineral += mineral;
    }

    public void OnChangedMineralCount()
    {
        _mineralText.text = $"Mineral: {Mineral}";
    }

    public void OnChangedGasCount()
    {
        _gasText.text = $"Gas: {Gas}";
    }
}
