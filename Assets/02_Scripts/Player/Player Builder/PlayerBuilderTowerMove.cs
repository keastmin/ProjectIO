using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderTowerMove : NetworkBehaviour
{
    private List<TowerGhost> _ghosts;
    private Dictionary<TowerGhost, Tower> _ghostToTowerDic; // 타워 고스트로 타워 찾기
    private Dictionary<Tower, Vector3> _towerToVecDic; // 타워로 위치 찾기

    #region API

    public void TowerMoveSet(HashSet<Tower> towers)
    {
        // 각 타워의 타워 고스트 생성
        TowerGhostInstantiate(towers);

        // 타워 그룹의 중심 찾기
        Vector3 pivot = GetPivot(towers);

        // 각 타워가 피벗에서 떨어져있는 벡터 계산
        TowerDistanceVectorCalc(towers, pivot);
    }

    public void TowerGhostSnapShot(HexagonGrid grid, Vector3 mousePos)
    {
        foreach(var ghost in _ghosts)
        {
            Tower targetTower = _ghostToTowerDic[ghost];
            Vector3 diff = _towerToVecDic[targetTower];
            ghost.transform.position = mousePos + diff;

            ghost.EnableTower();
        }
    }

    public void TowerMoveClear()
    {
        for(int i = 0; i < _ghosts.Count; i++)
        {
            Destroy(_ghosts[i].gameObject);
        }
        _ghosts.Clear();
        _ghostToTowerDic.Clear();
        _towerToVecDic.Clear();
    }

    #endregion

    #region Core

    private void TowerGhostInstantiate(HashSet<Tower> towers)
    {
        _ghosts = new List<TowerGhost>();
        _ghostToTowerDic = new Dictionary<TowerGhost, Tower>();
        foreach(var tower in towers)
        {
            var ghost = Instantiate(tower.Ghost);
            _ghosts.Add(ghost);
            _ghostToTowerDic.Add(ghost, tower);
        }
    }

    // 타워 집합의 중심 피벗 찾기
    private Vector3 GetPivot(HashSet<Tower> towers)
    {
        if (towers == null || towers.Count == 0)
            return Vector3.zero;

        bool inited = false;
        Vector3 min = Vector3.zero, max = Vector3.zero;

        foreach(var t in towers)
        {
            if (!t) continue;

            Vector3 p = t.transform.position;

            if (!inited)
            {
                min = max = p;
                inited = true;
            }
            else
            {
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        }

        if (!inited) return Vector3.zero;
        return (min + max) * 0.5f;
    }

    private void TowerDistanceVectorCalc(HashSet<Tower> towers, Vector3 pivot)
    {
        _towerToVecDic = new Dictionary<Tower, Vector3>();

        foreach(var tower in towers)
        {
            Vector3 p = tower.transform.position;
            Vector3 diff = p - pivot;
            _towerToVecDic.Add(tower, diff);
        }
    }

    #endregion
}
