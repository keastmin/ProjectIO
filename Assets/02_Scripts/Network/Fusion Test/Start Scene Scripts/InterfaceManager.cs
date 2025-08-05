using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance { get; private set; }

    [SerializeField] private GameObject _prevFocusUI;
    [SerializeField] private GameObject _currFocusUI;

    public JoinSessionPanel JoinSession;
    public LobbyUI Lobby;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        FocusNextUI(_currFocusUI);
    }

    public void FocusNextUI(GameObject nextUI)
    {
        if(_currFocusUI != null)
        {
            _prevFocusUI = _currFocusUI;
            _currFocusUI.SetActive(false);
        }

        _currFocusUI = nextUI;
        _currFocusUI.SetActive(true);
    }

    public void ClearUI()
    {
        if(_currFocusUI != null)
        {
            _prevFocusUI = _currFocusUI;
            _currFocusUI.SetActive(false);
        }
    }
}
