using Fusion;
using System;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    [SerializeField] private Cost _cost;

    public Cost Cost => _cost;
}
