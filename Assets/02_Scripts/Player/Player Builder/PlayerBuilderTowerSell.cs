using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderTowerSell : NetworkBehaviour
{
    #region API

    /// <summary>
    /// 타워를 판매하는 함수
    /// </summary>
    /// <param name="grid">육각 그리드</param>
    /// <param name="towers">판매할 타워들</param>
    public void SellTower(HexagonGrid grid, HashSet<Tower> towers)
    {
        if (towers == null || towers.Count == 0) return;

        NetworkId[] ids = new NetworkId[towers.Count];
        Cost[] costs = new Cost[towers.Count];
        int n = 0;

        foreach(var t in towers)
        {
            if (t == null) continue;

            CellStateChange(grid, t); // 셀 상태 변경
            NetworkObject no = t.Object;
            if (no == null) continue;

            costs[n] = t.Cost;
            ids[n++] = no.Id;
        }

        if (n == 0) return;
        if (n != ids.Length) Array.Resize(ref ids, n);

        towers.Clear();
        RPC_SellTower(ids, costs);      
    }

    #endregion

    #region Core

    // 타워가 있던 셀의 상태를 None으로 변경
    private void CellStateChange(HexagonGrid grid, Tower tower)
    {
        Vector2Int index = grid.GetNearIndex(tower.transform.position); // 타워 근처 인덱스 찾기
        grid.ChangeCellState(index, CellState.None); // 셀 상태 변경
    }

    // 타워를 Despawn하는 것을 요청하고 금액 환불
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SellTower(NetworkId[] towerIds, Cost[] costs)
    {
        // Despawn
        foreach(var id in towerIds)
        {
            if (!Runner.TryFindObject(id, out NetworkObject obj))
                continue;

            Runner.Despawn(obj);
        }

        // 환불
        foreach(var cost in costs)
        {
            ResourceSystem.Instance.Mineral += cost.Mineral;
            ResourceSystem.Instance.Gas += cost.Gas;
        }
    }

    #endregion
}
