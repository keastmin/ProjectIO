using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float _longPressDuration = 1.3f;
    public UnityEvent SuccessEvent;

    private bool _isPressed = false;
    private float _pressTime = 0f;

    private void Update()
    {
        if (_isPressed)
        {
            Debug.Log(_pressTime);

            if(_pressTime >= _longPressDuration)
            {
                SuccessEvent.Invoke();
                _isPressed = false;
                _pressTime = 0f;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        _pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _pressTime = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPressed = false;
        _pressTime = 0f;
    }
}
