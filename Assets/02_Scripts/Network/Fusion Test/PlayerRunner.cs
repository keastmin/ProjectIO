using Fusion;
using UnityEngine;

public class PlayerRunner : Player
{
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _cc.maxSpeed = 10f;
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(10f * data.direction * Runner.DeltaTime);
        }
    }
}
