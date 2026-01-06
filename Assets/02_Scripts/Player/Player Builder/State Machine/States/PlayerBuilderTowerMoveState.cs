using UnityEngine;

public class PlayerBuilderTowerMoveState : IPlayerState
{
    private PlayerBuilder _player;

    public PlayerBuilderTowerMoveState(PlayerBuilder player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.BuilderUI.ActivationTowerBuildUI(true, "Left Mouse: Complete, RightMouse: Cancel");
        _player.BuilderTowerMove.TowerMoveSet(_player.SelectedTowers);
    }

    public void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos(_player.Grid);
        _player.BuilderTowerMove.TowerGhostSnapShot(_player.Grid, mouseWorldPos);
        TransitionTo();
    }

    public void LateUpdate()
    {
        
    }

    public void Exit()
    {
        _player.BuilderTowerMove.TowerMoveClear();
        _player.BuilderUI.ActivationTowerBuildUI(false);
    }

    private void TransitionTo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.StateMachine.TransitionToState(_player.StateMachine.TowerSelectState);
        }
    }

    private Vector3 GetMouseWorldPos(HexagonGrid grid)
    {
        Vector3 pos = Vector3.zero;
        float height = grid.GridHeight;
        Plane plane = new Plane(Vector3.up, height);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            pos = ray.GetPoint(enter);
        }
        return pos;
    }
}
