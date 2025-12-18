using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBuilderTowerSelectState : IPlayerState
{
    private PlayerBuilder _player;

    public PlayerBuilderTowerSelectState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Tower Select 상태 진입");

        var manager = StageManager.Instance;
        if (manager != null)
            manager.UIController.BuilderUI.ActivationTowerSelectUI(true);
    }

    public void Render()
    {
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            _player.ClickLeftMouseDownOnWorld();
            _player.SetClickValue(true);
        }

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
        var manager = StageManager.Instance;
        if (manager != null)
            manager.UIController.BuilderUI.ActivationTowerSelectUI(false);
    }

    private void TransitionTo()
    {
        if (_player.SelectedAttackTowerCount <= 0)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.OriginState);
        }
    }
}
