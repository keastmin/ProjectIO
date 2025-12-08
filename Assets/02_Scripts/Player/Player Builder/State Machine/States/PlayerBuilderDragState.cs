using UnityEngine;

public class PlayerBuilderDragState : IPlayerState
{
    PlayerBuilder _player;

    public PlayerBuilderDragState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Drag State 진입");
        _player.DragStart();
    }

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _player.SetClickValue(false);
        }

        _player.SetCurrentMousePoint(Input.mousePosition);
        _player.Dragging();

        TransitionTo();
    }

    public void LateUpdate()
    {
        
    }

    public void NetworkFixedUpdate()
    {
        
    }

    public void Render()
    {
        
    }

    public void Exit()
    {
        _player.DragEnd();
    }

    private void TransitionTo()
    {
        if (!_player.IsClick)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }
}
