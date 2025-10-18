using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBuilderOriginState : IPlayerState
{
    private PlayerBuilder _player;

    public PlayerBuilderOriginState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Origin State");
    }

    public void Update()
    {
        if (_player.HasInputAuthority)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _player.OnClickInteractableObject();
            }
            _player.BuilderCamMove();
        }
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
        if(stageManager != null && stageManager.UIController.BuilderUI.IsLaboratoryUIActive)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.LaboratoryState);
        }
        else if(_player.IsStandByTowerBuild)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.TowerBuildState);
        }
    }
}
