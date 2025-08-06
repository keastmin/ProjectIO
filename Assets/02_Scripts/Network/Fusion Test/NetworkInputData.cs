using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte DASH_INPUT = 2; // 러너의 대쉬

    public NetworkButtons buttons;
    public Vector3 mousePosition;

    // 러너 입력
    public NetworkButtons DashInput;
    public Vector3 PlayerRunnerDirection;
}
