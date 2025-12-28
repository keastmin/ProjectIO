using Fusion;
using System;

[Serializable]
public struct Cost : INetworkStruct
{
    public int Mineral;
    public int Gas;
}
