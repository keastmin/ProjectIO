using Fusion;
using UnityEngine;

public sealed class PlayerBuilderTowerBuild : NetworkBehaviour
{
    [SerializeField] private Tower _tower;
    [SerializeField] private TowerGhost _towerGhost;
    [SerializeField] private Cost _buildCost;
    [SerializeField] private NetworkPrefabRef _towerRef;
    [SerializeField] private bool _isStandByBuild = false;

    public TowerGhost TowerGhost => _towerGhost;
    public Cost BuildCost => _buildCost;
    public bool IsStandByBuild => _isStandByBuild;

    #region API

    /// <summary>
    /// 외부에서 호출해야하는 초기화 함수
    /// </summary>
    /// <param name="builderUI">빌더의 UI 컴포넌트</param>
    public void Init(PlayerBuilderUI builderUI)
    {
        _isStandByBuild = false;
        LinkBuildTowerAction(builderUI);
    }

    /// <summary>
    /// 건설 준비중인 타워를 설치하는 함수
    /// </summary>
    /// <param name="position">타워를 설치할 위치</param>
    public void BuildTower(Vector3 position)
    {
        if(_towerRef != default && _tower != null)
        {
            RPC_BuildTower(_towerRef, _buildCost, position);
        }
        RevertStandBy();
    }

    /// <summary>
    /// 타워 건설 준비중을 원래대로 되돌리는 함수
    /// </summary>
    public void RevertStandBy()
    {
        _tower = null;
        _towerGhost = null;
        _towerRef = default;
        _isStandByBuild = false;
    }

    #endregion

    #region Core

    /// <summary>
    /// 빌더 UI의 타워 건설 버튼을 누르면 작동할 액션 연결
    /// </summary>
    /// <param name="builderUI">빌더의 UI 컴포넌트</param>
    private void LinkBuildTowerAction(PlayerBuilderUI builderUI)
    {
        if(builderUI != null)
        {
            builderUI.OnClickTowerBuildButtonAction += InjectionTowerData;
        }
    }

    // 설치할 타워 데이터 주입
    private void InjectionTowerData(TowerData data)
    {
        if(data != null)
        {
            _tower = data.Tower;
            _towerGhost = data.TowerGhost;
            _towerRef = data.TowerPrefabRef;

            if(_tower != null)
            {
                _buildCost = _tower.Cost;
            }

            // 타워 설치 준비
            if (_tower != null && _towerGhost != null && _towerRef != null)
                _isStandByBuild = true;
        }
    }

    // Host에게 자원 차감과 타워 스폰 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_BuildTower(NetworkPrefabRef towerRef, Cost cost, Vector3 position)
    {
        ResourceSystem.Instance.Mineral -= cost.Mineral;
        Runner.Spawn(towerRef, position, Quaternion.identity);
    }

    #endregion
}
