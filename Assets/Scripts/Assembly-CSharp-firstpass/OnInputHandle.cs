using UnityEngine;

[AddComponentMenu("TUI/Control/TUIEventRect")]
public delegate void OnInputHandle(TUIControl control, int eventType, float wparam, float lparam, object data);
