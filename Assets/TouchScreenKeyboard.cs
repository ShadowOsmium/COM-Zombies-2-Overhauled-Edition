#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEngine;

public class TouchScreenKeyboard : MonoBehaviour
{
    static TouchScreenKeyboard instance;

    public static bool hideInput;
    public static bool visible;

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
            //string val = value;

            //if (!multiline) val.Replace("\n", "");

            //_text = val;
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

    static GUIStyle style;

    float backspaceTime;

    [RuntimeInitializeOnLoadMethod]
    static void OnGameStart()
    {
        style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontSize = 46
        };
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
            text = text.Substring(0, text.Length - 1);

            backspaceTime = 0.5f;

            return;
        }

        if (Input.GetKey(KeyCode.Backspace) && !string.IsNullOrEmpty(text))
        {
            backspaceTime -= Time.unscaledDeltaTime;

            if (backspaceTime > 0) return;

            backspaceTime = 0.075f;
            text = text.Substring(0, text.Length - 1);

            return;
        }

        text += Input.inputString;
    }

    void OnGUI()
    {
        if (!active || hideInput) return;

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        float labelWidth = Screen.width * 0.8f;
        float labelHeight = 50f;
        float labelX = (Screen.width - labelWidth) / 2f;
        float labelY = Screen.height * 0.4f;

        if (string.IsNullOrEmpty(text))
        {
            style.normal.textColor = Color.grey;
            GUI.Label(new Rect(labelX, labelY, labelWidth, labelHeight), placeholderText, style);
        }
        else
        {
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(labelX, labelY, labelWidth, labelHeight), text, style);
        }
    }
}
#endif
