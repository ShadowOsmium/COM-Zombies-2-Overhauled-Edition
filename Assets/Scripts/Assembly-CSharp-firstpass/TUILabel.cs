using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("TUI/Control/Label")]
[RequireComponent(typeof(TUIDrawSprite))]
public class TUILabel : MonoBehaviour
{
	public enum TUIPivot
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}

	[SerializeField]
	protected TUIFont fontHD;

	[SerializeField]
	protected float scale = 1f;

	[SerializeField]
	protected TUIFont.Alignment alignment;

	[SerializeField]
	protected TUIPivot pivot;

	[SerializeField]
	protected string textID = string.Empty;

	[SerializeField]
	protected string text = string.Empty;

	[SerializeField]
	protected int maxLineWidth;

	[SerializeField]
	protected bool encoding = true;

	[SerializeField]
	protected bool multiline = true;

	[SerializeField]
	protected bool password;

	[SerializeField]
	protected bool showLastChar;

	[SerializeField]
	protected int lineWidth;

	public Color color = Color.white;

	public Color colorBK = Color.black;

	protected bool shouldBeProcessed = true;

	protected string processedText;

	protected string lastTextID = string.Empty;

	protected string lastText = string.Empty;

	protected int lastWidth;

	protected bool lastEncoding = true;

	protected bool lastMulti = true;

	protected bool lastPass;

	protected bool lastShow;

	protected TUIPivot lastPivot;

	protected TUIFont.Alignment lastAlignment;

	protected Color lastColor = Color.white;

	protected Color lastColorBK = Color.black;

	protected float lastScale = 1f;

	protected TUIFont currentFont;

	protected TUIDrawSprite drawSprite;

	protected Material material;

	protected TUIGeometry geometry = new TUIGeometry();

	protected TUIGeometry geometryTemp = new TUIGeometry();

	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			if (scale != value)
			{
				scale = value;
			}
		}
	}

	public TUIPivot Pivot
	{
		get
		{
			return pivot;
		}
		set
		{
			pivot = value;
			HasChanged = true;
		}
	}

	public string TextID
	{
		get
		{
			return textID;
		}
		set
		{
			textID = value;
			Text = TUITextManager.Instance().GetString(textID);
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (value != null && text != value)
			{
				text = value;
				HasChanged = true;
			}
		}
	}

	public bool SupportEncoding
	{
		get
		{
			return encoding;
		}
		set
		{
			if (encoding != value)
			{
				encoding = value;
				HasChanged = true;
				if (value)
				{
					password = false;
				}
			}
		}
	}

	public int LineWidth
	{
		get
		{
			return maxLineWidth;
		}
		set
		{
			if (maxLineWidth != value)
			{
				maxLineWidth = value;
				HasChanged = true;
			}
		}
	}

	public bool MultiLine
	{
		get
		{
			return multiline;
		}
		set
		{
			if (multiline != value)
			{
				multiline = value;
				HasChanged = true;
				if (value)
				{
					password = false;
				}
			}
		}
	}

	public bool Password
	{
		get
		{
			return password;
		}
		set
		{
			if (password != value)
			{
				password = value;
				multiline = false;
				encoding = false;
				HasChanged = true;
			}
		}
	}

	public bool ShowLastPasswordChar
	{
		get
		{
			return showLastChar;
		}
		set
		{
			if (showLastChar != value)
			{
				showLastChar = value;
				HasChanged = true;
			}
		}
	}

	public string ProcessedText
	{
		get
		{
			if (shouldBeProcessed)
			{
				if (!string.IsNullOrEmpty(textID))
				{
					TextID = textID;
				}
				processedText = fontHD.WrapText(text, (float)(maxLineWidth * 2) / scale, multiline, encoding);
				shouldBeProcessed = false;
			}
			return processedText;
		}
	}

	public Bounds Bounds
	{
		get
		{
			return geometry.Bounds;
		}
	}

	private bool HasChanged
	{
		get
		{
			return shouldBeProcessed || (lastText != text && string.IsNullOrEmpty(textID)) || lastWidth != maxLineWidth || lastEncoding != encoding || lastMulti != multiline || lastPass != password || lastShow != showLastChar || lastPivot != pivot || lastAlignment != alignment || lastColor != color || lastColorBK != colorBK || lastTextID != textID || lastScale != scale;
		}
		set
		{
			if (value)
			{
				shouldBeProcessed = true;
				return;
			}
			shouldBeProcessed = false;
			lastTextID = textID;
			lastText = text;
			lastWidth = maxLineWidth;
			lastEncoding = encoding;
			lastMulti = multiline;
			lastPass = password;
			lastShow = showLastChar;
			lastPivot = pivot;
			lastAlignment = alignment;
			lastColor = color;
			lastColorBK = colorBK;
			lastScale = scale;
		}
	}

	public void SetFormatText(string textId, params object[] parms)
	{
		Text = TUITool.StringFormat(TUITextManager.Instance().GetString(textId), parms);
	}

	private void Layout(TUIGeometry geometry)
	{
		geometry.RecalculateBounds();
		Bounds bounds = geometry.Bounds;
		Matrix4x4 identity = Matrix4x4.identity;
		Vector3 zero = Vector3.zero;
		switch (Pivot)
		{
		case TUIPivot.Center:
			zero.x = bounds.center.x * -1f;
			zero.y = bounds.center.y * -1f;
			break;
		case TUIPivot.Left:
			zero.x = bounds.min.x * -1f;
			zero.y = bounds.center.y * -1f;
			break;
		case TUIPivot.Right:
			zero.x = bounds.max.x * -1f;
			zero.y = bounds.center.y * -1f;
			break;
		case TUIPivot.Top:
			zero.x = bounds.center.x * -1f;
			zero.y = bounds.max.y * -1f;
			break;
		case TUIPivot.Bottom:
			zero.x = bounds.center.x * -1f;
			zero.y = bounds.min.y * -1f;
			break;
		case TUIPivot.BottomLeft:
			zero.x = bounds.min.x * -1f;
			zero.y = bounds.min.y * -1f;
			break;
		case TUIPivot.TopLeft:
			zero.x = bounds.min.x * -1f;
			zero.y = bounds.max.y * -1f;
			break;
		case TUIPivot.TopRight:
			zero.x = bounds.max.x * -1f;
			zero.y = bounds.max.y * -1f;
			break;
		case TUIPivot.BottomRight:
			zero.x = bounds.max.x * -1f;
			zero.y = bounds.min.y * -1f;
			break;
		}
		identity.SetTRS(zero, Quaternion.identity, Vector3.one);
		geometry.TRS(identity);
	}

	private void MakePositionPerfect()
	{
		Vector3 position = base.transform.position;
		position.x = Mathf.RoundToInt(position.x);
		position.y = Mathf.RoundToInt(position.y);
		Vector3 size = geometry.Bounds.size;
		int num = Mathf.RoundToInt(size.x * scale);
		int num2 = Mathf.RoundToInt(size.y * scale);
		if (num % 2 == 1 && (pivot == TUIPivot.Top || pivot == TUIPivot.Center || pivot == TUIPivot.Bottom))
		{
			position.x += 0.5f;
		}
		if (num2 % 2 == 1 && (pivot == TUIPivot.Left || pivot == TUIPivot.Center || pivot == TUIPivot.Right))
		{
			position.y -= 0.5f;
		}
		base.transform.position = position;
	}

	private void Draw()
	{
		if (null != fontHD)
		{
			material = fontHD.material;
			drawSprite.material = material;
			geometry.Clear();
			fontHD.Print(ProcessedText, color, geometry.Vertices, geometry.Triangles, geometry.Uv, geometry.Colors, 0.5f * scale, encoding, alignment, lineWidth);
			Layout(geometry);
			drawSprite.Draw(geometry.Vertices, geometry.Triangles, geometry.Uv, geometry.Colors);
			drawSprite.SetClippingRect();
		}
	}

	private void Start()
	{
		drawSprite = GetComponent<TUIDrawSprite>();
	}

	public Bounds CalculateBounds(string str)
	{
		geometryTemp.Clear();
		fontHD.Print(fontHD.WrapText(str, (float)(maxLineWidth * 2) / scale, multiline, encoding), color, geometryTemp.Vertices, geometryTemp.Triangles, geometryTemp.Uv, geometryTemp.Colors, 0.5f * scale, encoding, alignment, lineWidth);
		Layout(geometryTemp);
		geometryTemp.RecalculateBounds();
		return geometryTemp.Bounds;
	}

	private void TryToDraw()
	{
		if (HasChanged)
		{
			shouldBeProcessed = true;
			Draw();
			HasChanged = false;
		}
	}

	private void LateUpdate()
	{
		TryToDraw();
	}

	public void OnDrawGizmos()
	{
		Vector3[] array = new Vector3[4]
		{
			base.transform.TransformPoint(Bounds.min.x, Bounds.max.y, 0f),
			base.transform.TransformPoint(Bounds.max.x, Bounds.max.y, 0f),
			base.transform.TransformPoint(Bounds.max.x, Bounds.min.y, 0f),
			base.transform.TransformPoint(Bounds.min.x, Bounds.min.y, 0f)
		};
		Gizmos.color = Color.white;
		Gizmos.DrawLine(array[0], array[1]);
		Gizmos.DrawLine(array[1], array[2]);
		Gizmos.DrawLine(array[2], array[3]);
		Gizmos.DrawLine(array[3], array[0]);
		Gizmos.DrawLine(array[0], array[2]);
	}
}
