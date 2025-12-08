using UnityEngine;

public class PlayerBuilderStateMachine
{
    private IPlayerState _currentState;

    public PlayerBuilderOriginState OriginState;
    public PlayerBuilderDragState DragState;
    public PlayerBuilderTowerBuildState TowerBuildState;
    public PlayerBuilderLaboratoryState LaboratoryState;
    public PlayerBuilderTowerSelectState TowerSelectState;
    public PlayerBuilderTowerDragState TowerDragState;

    public PlayerBuilderStateMachine(PlayerBuilder player)
    {
        OriginState = new PlayerBuilderOriginState(player);
        DragState = new PlayerBuilderDragState(player);
        TowerBuildState = new PlayerBuilderTowerBuildState(player);
        LaboratoryState = new PlayerBuilderLaboratoryState(player);
        TowerSelectState = new PlayerBuilderTowerSelectState(player);
        TowerDragState = new PlayerBuilderTowerDragState(player);
    }

    public void InitStateMachine()
    {
        _currentState = OriginState;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState.Update();
    }

    public void Render()
    {
        _currentState.Render();
    }

    public void NetworkFixedUpdate()
    {
        _currentState.NetworkFixedUpdate();
    }

    public void LateUpdate()
    {
        _currentState.LateUpdate();
    }

    public void TransitionToState(IPlayerState next)
    {
        _currentState.Exit();
        _currentState = next;
        _currentState.Enter();
    }
}
