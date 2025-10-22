using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct UIShortcutInfo
{
    public KeyCode ShortcutKey;
    public Button ShortcutButton;
    public bool IsLongPress;
    public float LongPressDuration;
    public float PressTime { get; set; }
}
