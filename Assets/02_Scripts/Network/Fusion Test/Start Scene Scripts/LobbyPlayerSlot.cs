using Fusion;
using TMPro;
using UnityEngine;

public class LobbyPlayerSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerPosition;

    private PlayerPosition _position;
    public PlayerPosition Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            SetPlayerPositionText(_position);
        }
    }

    private void Awake()
    {
        Position = PlayerPosition.Builder;
    }

    public void InitSlot()
    {
        Position = PlayerPosition.Builder;
    }

    public void SetPlayerPositionText(PlayerPosition position)
    {
        if(position == PlayerPosition.Builder)
        {
            _playerPosition.text = "Builder";
        }
        else if(position == PlayerPosition.Runner)
        {
            _playerPosition.text = "Runner";
        }
    }
}
