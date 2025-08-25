using Fusion;
using UnityEngine;

public class Tower : NetworkBehaviour
{
    [SerializeField] private int _cost;

    public int Cost => _cost;
}
