using System.Collections.Generic;
using UnityEngine;

public class UIShortcut : MonoBehaviour
{
    [SerializeField] private UIShortcutPart _mainUI;

    private Stack<UIShortcutPart> _uiShortcutPartStack;

    private void Awake()
    {
        _uiShortcutPartStack = new Stack<UIShortcutPart>();
        _uiShortcutPartStack.Push(_mainUI);
    }

    private void Update()
    {
        var top = _uiShortcutPartStack.Peek();
        if (top != null)
        {
            if (_uiShortcutPartStack.Count > 1 && Input.GetMouseButtonDown(1)) // 이전으로
            {
                SetPreviousPart();
            }
            else // 숏컷 확인
            {
                DetectShortcutInput(top);
            }
        }
    }

    // 이전 파트로 돌아가기
    public void SetPreviousPart()
    {
        _uiShortcutPartStack.Pop().DeactiveThisPart();

        var previousTop = _uiShortcutPartStack.Peek();
        previousTop.ActiveThisPart();
    }

    public void DetectShortcutInput(UIShortcutPart currentTop)
    {
        foreach (var info in currentTop.Shortcuts)
        {
            // 단축키가 눌렸는지 확인
            if (Input.GetKeyDown(info.ShortcutKey))
            {
                // 단축키에 해당하는 버튼 클릭 이벤트 호출
                info.ShortcutButton.onClick.Invoke();
            }
        }
    }

    // 다음 파트로 이동하는 버튼 클릭 이벤트
    public void OnClickNextPartButton(UIShortcutPart nextPart)
    {
        var currentTop = _uiShortcutPartStack.Peek();
        currentTop.DeactiveThisPart();
        _uiShortcutPartStack.Push(nextPart);
        nextPart.ActiveThisPart();
    }
}
