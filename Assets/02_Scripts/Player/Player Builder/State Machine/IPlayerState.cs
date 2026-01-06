using UnityEngine;

public interface IPlayerState
{
    public void Enter();
    public void Update();
    public void LateUpdate();
    public void Exit();
}
