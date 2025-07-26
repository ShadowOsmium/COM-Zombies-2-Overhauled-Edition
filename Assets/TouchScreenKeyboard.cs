#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEngine;

public class TouchScreenKeyboard : MonoBehaviour
{
    static TouchScreenKeyboard instance;

    public static bool hideInput;
    public static bool visible;
    public int maxLength = 20;
    private bool allSelected = false;
    private int selectionStart = 0;
    private int selectionEnd = 0;

    public static Rect area
    {
        get { return Rect.zero; }
    }

    string _text;

    public string text
    {
        get { return _text; }
        set
        {
            if (value.Length > maxLength)
                _text = value.Substring(0, maxLength);
            else
                _text = value;
        }
    }

    string placeholderText;

    TouchScreenKeyboardType type;
    bool autocorrection;
    bool multiline;
    bool secure;
    bool allert;

    public bool done;

    public bool active;
    public bool wasCanceled;
    private float initialBackspaceDelay = 0.2f;
    private float repeatRate = 0.03f;
    static GUIStyle style;

    float backspaceTime;

    [RuntimeInitializeOnLoadMethod]
    static void OnGameStart()
    {
        style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontSize = 24
        };
    }

    private Texture2D highlightTexture;

    void Awake()
    {
        highlightTexture = new Texture2D(1, 1);
        highlightTexture.SetPixel(0, 0, new Color(0f, 0.5f, 1f, 0.5f));
        highlightTexture.Apply();
    }

    public static TouchScreenKeyboard Open(string text = "", TouchScreenKeyboardType type = TouchScreenKeyboardType.Default, bool autocorrection = true, bool multiline = true, bool secure = false, bool allert = false, string placeholderText = "")
    {
        if (instance == null)
            instance = new GameObject("TouchScreenKeyboard (Emulator)").AddComponent<TouchScreenKeyboard>();

        instance.text = text;
        instance.multiline = multiline;
        instance.secure = secure;
        instance.allert = allert;
        instance.placeholderText = placeholderText;

        instance.active = true;
        instance.wasCanceled = false;
        instance.done = false;

        return instance;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            active = false;
            done = true;
            wasCanceled = true;
            return;
        }

        // Ctrl + A (Select All)
        if (Input.GetKeyDown(KeyCode.A) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            selectionStart = 0;
            selectionEnd = text.Length;
            allSelected = true;
            Debug.Log("All text selected.");
            return;
        }

        // Ctrl + C (Copy)
        if (Input.GetKeyDown(KeyCode.C) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            if (selectionStart != selectionEnd)
            {
                int start = Mathf.Min(selectionStart, selectionEnd);
                int end = Mathf.Max(selectionStart, selectionEnd);
                GUIUtility.systemCopyBuffer = text.Substring(start, end - start);
                Debug.Log("Copied: " + GUIUtility.systemCopyBuffer);
            }
            return;
        }

        // Ctrl + V (Paste)
        if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            string pasteText = GUIUtility.systemCopyBuffer;
            if (!string.IsNullOrEmpty(pasteText))
            {
                // Remove selected text if any
                if (selectionStart != selectionEnd)
                {
                    int start = Mathf.Min(selectionStart, selectionEnd);
                    int end = Mathf.Max(selectionStart, selectionEnd);
                    text = text.Substring(0, start) + pasteText + text.Substring(end);
                    selectionStart = start + pasteText.Length;
                }
                else
                {
                    // Insert pasteText at cursor position
                    int insertPos = selectionStart;
                    text = text.Substring(0, insertPos) + pasteText + text.Substring(insertPos);
                    selectionStart = insertPos + pasteText.Length;
                }
                if (text.Length > maxLength)
                {
                    text = text.Substring(0, maxLength);
                    selectionStart = Mathf.Min(selectionStart, maxLength);
                }
                selectionEnd = selectionStart;
                allSelected = false;
            }
            return;
        }

        // Ctrl + X (Cut)
        if (Input.GetKeyDown(KeyCode.X) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            if (selectionStart != selectionEnd)
            {
                int start = Mathf.Min(selectionStart, selectionEnd);
                int end = Mathf.Max(selectionStart, selectionEnd);
                GUIUtility.systemCopyBuffer = text.Substring(start, end - start);
                text = text.Substring(0, start) + text.Substring(end);
                selectionStart = start;
                selectionEnd = start;
                allSelected = false;
                Debug.Log("Cut: " + GUIUtility.systemCopyBuffer);
            }
            return;
        }


        if (Input.GetKeyDown(KeyCode.Return) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            done = true;
            active = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            done = true;
            active = false;
            wasCanceled = true;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (selectionStart != selectionEnd)
            {
                int start = Mathf.Min(selectionStart, selectionEnd);
                int end = Mathf.Max(selectionStart, selectionEnd);
                text = text.Substring(0, start) + text.Substring(end);
                selectionStart = start;
                selectionEnd = start;
                allSelected = false;
            }
            else if (!string.IsNullOrEmpty(text) && text.Length > 0)
            {
                int deletePos = Mathf.Max(0, selectionStart - 1);
                text = text.Remove(deletePos, 1);
                selectionStart = deletePos;
                selectionEnd = deletePos;
            }
            backspaceTime = initialBackspaceDelay;
            return;
        }

        if (Input.GetKey(KeyCode.Backspace))
        {
            if (!string.IsNullOrEmpty(text))
            {
                backspaceTime -= Time.unscaledDeltaTime;

                if (backspaceTime <= 0f)
                {
                    backspaceTime = repeatRate;

                    if (selectionStart != selectionEnd)
                    {
                        text = text.Substring(0, selectionStart) + text.Substring(selectionEnd);
                        selectionEnd = selectionStart;
                        allSelected = false;
                    }
                    else if (text.Length > 0)
                    {
                        int deletePos = Mathf.Max(0, selectionStart - 1);
                        text = text.Remove(deletePos, 1);
                        selectionStart = deletePos;
                        selectionEnd = deletePos;
                    }
                }
            }
            return;
        }

        if (!string.IsNullOrEmpty(Input.inputString))
        {
            if (allSelected)
            {
                text = Input.inputString;
                allSelected = false;
                selectionStart = Input.inputString.Length;
                selectionEnd = selectionStart;
            }
            else if (text.Length < maxLength)
            {
                text = text.Substring(0, selectionStart) + Input.inputString + text.Substring(selectionStart);
                selectionStart += Input.inputString.Length;
                selectionEnd = selectionStart;
            }
        }
    }

    void OnGUI()
    {
        if (!active || TouchScreenKeyboard.hideInput)
            return;

        /*GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        float labelHeight = 50f;
        float labelY = Screen.height * 0.4f;

        Vector2 fullTextSize = style.CalcSize(new GUIContent(text));
        float startX = (Screen.width - fullTextSize.x) / 2f;

        selectionStart = Mathf.Clamp(selectionStart, 0, text.Length);
        selectionEnd = Mathf.Clamp(selectionEnd, 0, text.Length);

        if (selectionStart > selectionEnd)
        {
            int tmp = selectionStart;
            selectionStart = selectionEnd;
            selectionEnd = tmp;
        }

        string beforeSelection = text.Substring(0, selectionStart);
        string selectedText = text.Substring(selectionStart, selectionEnd - selectionStart);
        string afterSelection = text.Substring(selectionEnd);

        Vector2 beforeSize = style.CalcSize(new GUIContent(beforeSelection));
        Vector2 selectedSize = style.CalcSize(new GUIContent(selectedText));

        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(startX, labelY, beforeSize.x, labelHeight), beforeSelection, style);

        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(startX + beforeSize.x, labelY, selectedSize.x, labelHeight), highlightTexture);

        GUI.Label(new Rect(startX + beforeSize.x, labelY, selectedSize.x, labelHeight), selectedText, style);

        Vector2 afterSize = style.CalcSize(new GUIContent(afterSelection));
        GUI.Label(new Rect(startX + beforeSize.x + selectedSize.x, labelY, afterSize.x, labelHeight), afterSelection, style);

        GUI.color = Color.white;
    }*/
    }
}
#endif
