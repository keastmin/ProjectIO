using UnityEngine;

public class PlayerBuilderTowerSelectState : IPlayerState
{
    private PlayerBuilder _player;

    public PlayerBuilderTowerSelectState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        var manager = StageManager.Instance;
        if (manager != null)
            manager.UIController.BuilderUI.ActivationTowerSelectUI(true);
    }

    public void Render()
    {
        
    }

    public void Update()
    {


        TransitionTo();
    }

    public void LateUpdate()
    {

    }

    public void NetworkFixedUpdate()
    {

    }

    public void Exit()
    {
        _player.BuilderSelectTowerSetting(false, null);

        var manager = StageManager.Instance;
        if (manager != null)
            manager.UIController.BuilderUI.ActivationTowerSelectUI(false);
    }

    private void TransitionTo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }
}
