using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TowerListUpUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _towerList;

    // 타워 리스트를 표출시키는 버튼 클릭 시 호출되는 메서드
    public void OnClickTowerListUpButton(int index)
    {
        if(index < 0 || index >= _towerList.Length)
        {
            Debug.Log("Index out of range");
            return;
        }

        // 선택한 타워 리스트만 활성화 하고 나머지는 비활성화
        for(int i = 0; i < _towerList.Length; i++)
        {
            if (i == index) _towerList[i].SetActive(true);
            else _towerList[i].SetActive(false);
        }
    }

    // 타워 리스트에서 타워 선택 버튼 클릭 시 호출되는 메서드
    public void OnClickTowerSelectButton(TowerData towerData)
    {
        // StageManager의 PlayerBuilder를 통해 빌더에게 타워 데이터 전달
        if (StageManager.Instance)
        {
            var builder = StageManager.Instance.PlayerBuilder;
            if (builder)
            {
                builder.ClickTowerSelectButton(towerData);
            }
        }
    }
}
