using Fusion;
using System;
using UnityEngine;

[Serializable]
public struct Cost
{
    public int Mineral;
    public int Gas;
}

public class Tower : NetworkBehaviour
{
    [SerializeField] private Cost _cost;

    public Cost Cost => _cost;
}
