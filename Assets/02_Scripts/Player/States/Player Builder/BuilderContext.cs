using Fusion;
using UnityEngine;

public sealed class BuilderContext
{
    public PlayerBuilder OwnerBuilder;

    public NetworkPrefabRef TowerRef => OwnerBuilder.SelcetTowerRef;
    public Tower Tower => OwnerBuilder.SelectTower;
    public TowerGhost TowerGhost => OwnerBuilder.SelectTowerGhost;

    public LayerMask EnvironmentalLayer => OwnerBuilder.EnvironmentalLayer;
}
