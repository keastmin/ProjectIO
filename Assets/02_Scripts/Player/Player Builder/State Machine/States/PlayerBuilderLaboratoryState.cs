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

    }

    private void TransitionTo()
    {
        var stageManager = StageManager.Instance;
        if(stageManager != null && !stageManager.UIController.BuilderUI.IsLaboratoryUIActive)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }
}
