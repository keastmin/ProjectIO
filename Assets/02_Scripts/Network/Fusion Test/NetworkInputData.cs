using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1; // 빌더의 타워 설치
    public const byte DASH_INPUT = 2; // 러너의 대쉬

    // 러너 입력
    public NetworkButtons DashInput;
    public Vector3 PlayerRunnerDirection;

    // 빌더 입력
    public NetworkButtons MouseButton0; // 빌더의 타워 설치에 필요한 좌클릭 
    public Vector3 MousePosition; // 타워를 설치할 위치에 해당하는 마우스 위치
}
