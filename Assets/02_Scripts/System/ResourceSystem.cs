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
    [Networked] public int Gas { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
}
