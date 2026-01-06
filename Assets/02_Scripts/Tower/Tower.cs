using Fusion;
using System;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    [SerializeField] private Cost _cost;
    [SerializeField] private TowerGhost _ghost;

    public Cost Cost => _cost;
    public TowerGhost Ghost => _ghost;
}
