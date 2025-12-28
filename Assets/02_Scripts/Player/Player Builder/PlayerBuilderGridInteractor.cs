using UnityEngine;
using UnityEngine.UIElements;

public sealed class PlayerBuilderGridInteractor : MonoBehaviour
{
    [SerializeField] private HexagonGrid _hexagonGrid;

    #region API

    /// <summary>
    /// 외부에서 호출해야 하는 초기화 함수
    /// </summary>
    /// <param name="grid">Hexagon Grid 컴포넌트</param>
    public void Init(HexagonGrid grid)
    {
        InjectionGrid(grid);
    }

    #endregion

    #region Core

    // 그리드의 참조를 주입하는 함수
    private void InjectionGrid(HexagonGrid grid)
    {
        _hexagonGrid = grid;
    }

    #endregion
}
