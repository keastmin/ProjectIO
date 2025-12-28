using UnityEngine;

public sealed class GridCell
{
    // 이 셀의 위치
    private Vector3 _position;

    // 이 셀의 상태
    private CellState _state;

    // 지어진 타워
    private Tower _builtTower;

    #region 프로퍼티

    public Vector3 Position => _position;
    public CellState State => _state;
    public Tower BuiltTower => _builtTower;

    #endregion

    public GridCell(Vector3 pos, CellState state = CellState.None)
    {
        _position = pos;
        _state = state;
        _builtTower = null;
    }

    /// <summary>
    /// 상태를 설정하는 함수
    /// </summary>
    /// <param name="state">설정할 상태</param>
    public void SetState(CellState state)
    {
        _state = state;
    }
}
