using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float _longPressDuration = 1.3f;
    [SerializeField] private Slider _slider;
    public UnityEvent SuccessEvent;

    private bool _isPressed = false;
    private float _pressTime = 0f;
    public float PressTime
    {
        get
        {
            return _pressTime;
        }
        set
        {
            _pressTime = value;
            if(_slider != null)
            {
                _slider.value = _pressTime / _longPressDuration;
            }
        }
    }

    private void OnEnable()
    {
        SetPressState(false);
    }

    private void OnDisable()
    {
        SetPressState(false);
    }

    private void Update()
    {
        if (_isPressed)
        {
            PressTime += Time.deltaTime;

            Debug.Log(PressTime);

            if(PressTime >= _longPressDuration)
            {
                SuccessEvent.Invoke();
                SetPressState(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPressState(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetPressState(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPressState(false);
    }

    private void SetPressState(bool pressed)
    {
        _isPressed = pressed;
        PressTime = 0f;
    }
}
