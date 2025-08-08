using UnityEngine;

public class PlayerBuilder : Player
{
    [SerializeField] private Tower _tower;

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            bool mouseButton0 = data.MouseButton0.IsSet(NetworkInputData.MOUSEBUTTON0);
            if (HasStateAuthority && mouseButton0)
            {
                Runner.Spawn(_tower, data.MousePosition, Quaternion.identity);
            }
        }
    }
}
