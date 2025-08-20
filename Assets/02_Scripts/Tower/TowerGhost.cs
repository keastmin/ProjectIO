using UnityEngine;

public class TowerGhost : MonoBehaviour
{
    [SerializeField] private GameObject _enableTowerObject;
    [SerializeField] private GameObject _disableTowerObject;

    public void Start()
    {
        _enableTowerObject.SetActive(false);
        _disableTowerObject.SetActive(false);
    }

    public void EnableTower()
    {
        _disableTowerObject.SetActive(false);
        _enableTowerObject.SetActive(true);
    }

    public void DisableTower()
    {
        _enableTowerObject.SetActive(false);
        _disableTowerObject.SetActive(true);
    }
}
