using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBuilderOriginState : IPlayerState
{
    private PlayerBuilder _player;

    private bool _isDragging = false;

    public PlayerBuilderOriginState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Origin State");

        _player.SetClickValue(false); // 클릭 여부 초기화
    }

    public void Update()
    {
        if (_player.HasInputAuthority)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {               
                _player.SetClickValue(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                _player.OnClickInteractableObject();
                _player.SetClickValue(false);
            }

            if (_player.IsClick)
            {
                _player.SetCurrentMousePoint(Input.mousePosition);
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
        else if (_player.IsSelectTower)
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.TowerSelectState);
        }
        else if(_player.IsClick && (Vector2.Distance(_player.StartMousePoint, _player.CurrentMousePoint) >= _player.DragThresholdPixel))
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.DragState);
        }
    }
}
