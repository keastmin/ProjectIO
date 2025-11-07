using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuilderLaboratoryState : IPlayerState
{
    private PlayerBuilder _player;

    public PlayerBuilderLaboratoryState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter() 
    {
        Debug.Log("Laboratory State");
    }

    public void Update() 
    {
        _player.BuilderCamMove();
        TransitionTo();
    }

    public void LateUpdate()
    {

    }

    public void Render() 
    { 

    }

    public void NetworkFixedUpdate() 
    {

    }

    public void Exit() 
    {
        var manager = StageManager.Instance;
        if(manager!= null)
        {
            manager.UIController.BuilderUI.ActivationLaboratoryUI(false);
        }
    }

    private void TransitionTo()
    {
        if(Input.GetMouseButtonDown(1))
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }
}
