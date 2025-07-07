using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("TUI/Control/Font")]
[ExecuteInEditMode]
public class TUIFont : MonoBehaviour
{
	public enum Alignment
	{
		Left,
		Center,
		Right
	}

	public TextAsset fontTxt;

	public Material material;

	public int mSpacingX;

	public int mSpacingY;

	public bool m_reload;

	[SerializeField]
	protected Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[SerializeField]
	protected BMFont mFont = new BMFont();

	protected List<Color> mColors = new List<Color>();

	public bool test;

	public BMFont bmFont
	{
		get
		{
			return mFont;
		}
	}

	public int texWidth
	{
		get
		{
			if (mFont == null)
			{
				return 1;
			}
			return mFont.texWidth;
		}
	}

	public int texHeight
	{
		get
		{
			if (mFont == null)
			{
				return 1;
			}
			return mFont.texHeight;
		}
	}

	public Texture2D texture
	{
		get
		{
			if (null == material)
			{
				return null;
			}
			return material.mainTexture as Texture2D;
		}
	}

	public int horizontalSpacing
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
			}
		}
	}

	public int verticalSpacing
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
			}
		}
	}

	public int size
	{
		get
		{
			return mFont.charSize;
		}
	}

	public void Print(string text, Color color, List<Vector3> verts, List<int> triangles, List<Vector2> uvs, List<Color> cols, bool encoding, Alignment alignment, int lineWidth)
	{
		Print(text, color, verts, triangles, uvs, cols, 1f, encoding, alignment, lineWidth);
	}

	public void Print(string text, Color color, List<Vector3> verts, List<int> triangles, List<Vector2> uvs, List<Color> cols, float scale, bool encoding, Alignment alignment, int lineWidth)
	{
		lineWidth = ((alignment != 0) ? ((lineWidth <= 0) ? Mathf.RoundToInt(CalculatePrintedSize(text, scale, encoding).x) : Mathf.RoundToInt((float)lineWidth / scale)) : 0);
		if (mFont == null || text == null || !mFont.isValid)
		{
			return;
		}
		mColors.Clear();
		mColors.Add(color);
		Vector2 vector = Vector2.one * scale;
		int count = verts.Count;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = mFont.charSize + mSpacingY;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector2 zero3 = Vector2.zero;
		Vector2 zero4 = Vector2.zero;
		float num6 = mUVRect.width / (float)mFont.texWidth;
		float num7 = mUVRect.height / (float)mFont.texHeight;
		int num8 = 0;
		int length = text.Length;
		while (num8 < length)
		{
			char c = text[num8];
			if (c == '\n')
			{
				if (num2 > num)
				{
					num = num2;
				}
				if (alignment != 0)
				{
					Align(verts, count, alignment, (int)((float)num2 * scale), lineWidth);
					count = verts.Count;
				}
				num2 = 0;
				num3 += num5;
				num4 = 0;
			}
			else if (c < ' ')
			{
				num4 = 0;
			}
			else
			{
				if (encoding && c == '{')
				{
					int num9 = TUITool.ParseSymbol(text, num8, mColors);
					if (num9 > 0)
					{
						color = mColors[mColors.Count - 1];
						num8 += num9;
						continue;
					}
				}
				BMGlyph glyph = mFont.GetGlyph(c);
				if (glyph != null)
				{
					if (num4 != 0)
					{
						num2 += glyph.GetKerning(num4);
					}
					zero.x = vector.x * (float)(num2 + glyph.offsetX);
					zero.y = (0f - vector.y) * (float)(num3 + glyph.offsetY);
					zero2.x = zero.x + vector.x * (float)glyph.width;
					zero2.y = zero.y - vector.y * (float)glyph.height;
					zero3.x = mUVRect.xMin + num6 * (float)glyph.x;
					zero3.y = mUVRect.yMax - num7 * (float)glyph.y;
					zero4.x = zero3.x + num6 * (float)glyph.width;
					zero4.y = zero3.y - num7 * (float)glyph.height;
					triangles.Add(verts.Count);
					triangles.Add(verts.Count + 1);
					triangles.Add(verts.Count + 2);
					triangles.Add(verts.Count + 2);
					triangles.Add(verts.Count + 3);
					triangles.Add(verts.Count);
					verts.Add(new Vector3(zero2.x, zero.y));
					verts.Add(new Vector3(zero2.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero2.y));
					verts.Add(new Vector3(zero.x, zero.y));
					uvs.Add(new Vector2(zero4.x, zero3.y));
					uvs.Add(new Vector2(zero4.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero4.y));
					uvs.Add(new Vector2(zero3.x, zero3.y));
					cols.Add(color);
					cols.Add(color);
					cols.Add(color);
					cols.Add(color);
					num2 += mSpacingX + glyph.advance;
					num4 = c;
				}
			}
			num8++;
		}
		if (alignment != 0 && count < verts.Count)
		{
			Align(verts, count, alignment, (int)((float)num2 * scale), lineWidth);
			count = verts.Count;
		}
	}

	public string WrapText(string text, float maxWidth, bool multiline, bool encoding)
	{
		if (encoding)
		{
			text = TUITool.StripSymbols(text, false);
		}
		int num = Mathf.RoundToInt(maxWidth);
		if (num < 1)
		{
			return text;
		}
		StringBuilder s = new StringBuilder();
		int length = text.Length;
		int num2 = num;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		bool flag = true;
		while (num5 < length)
		{
			char c = text[num5];
			if (c == '\n')
			{
				if (!multiline)
				{
					break;
				}
				num2 = num;
				if (num4 < num5)
				{
					s.Append(text.Substring(num4, num5 - num4 + 1));
				}
				else
				{
					s.Append(c);
				}
				flag = true;
				num4 = num5 + 1;
				num3 = 0;
			}
			else
			{
				if (c == ' ' && num3 != 32 && num4 < num5)
				{
					s.Append(text.Substring(num4, num5 - num4 + 1));
					flag = false;
					num4 = num5 + 1;
					num3 = c;
				}
				if (encoding && c == '{' && num5 + "{color}".Length - 1 < length)
				{
					if (text.Substring(num5, "{color}".Length).Equals("{color}"))
					{
						num5 += "{color}".Length;
						continue;
					}
					if (num5 + "{color:".Length - 1 + 9 < length && text.Substring(num5, "{color:".Length).Equals("{color:") && text[num5 + "{color:".Length - 1 + 9] == '}')
					{
						num5 += "{color:".Length + 9;
						continue;
					}
				}
				BMGlyph glyph = mFont.GetGlyph(c);
				if (glyph != null)
				{
					int num6 = mSpacingX + ((num3 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num3)));
					num2 -= num6;
					if (num2 < 0)
					{
						if (flag || !multiline)
						{
							s.Append(text.Substring(num4, Mathf.Max(0, num5 - num4)));
							if (!multiline)
							{
								num4 = num5;
								break;
							}
							EndLine(ref s);
							flag = true;
							num4 = ((c != ' ') ? num5 : (num5 + 1));
							num2 = num;
							num3 = 0;
						}
						else
						{
							flag = true;
							num5 = num4 - 1;
							num2 = num;
							num3 = 0;
							if (!multiline)
							{
								break;
							}
							EndLine(ref s);
						}
					}
					else
					{
						num3 = c;
					}
				}
			}
			num5++;
		}
		if (num4 < num5)
		{
			s.Append(text.Substring(num4, num5 - num4));
		}
		return s.ToString();
	}

	public Vector2 CalculatePrintedSize(string text, float scale, bool encoding)
	{
		Vector2 zero = Vector2.zero;
		if (mFont != null && mFont.isValid && !string.IsNullOrEmpty(text))
		{
			if (encoding)
			{
				text = TUITool.StripSymbols(text, true);
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = mFont.charSize + mSpacingY;
			int i = 0;
			for (int length = text.Length; i < length; i++)
			{
				char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
					num3 += num5;
					num4 = 0;
				}
				else if (c < ' ')
				{
					num4 = 0;
				}
				else
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph != null)
					{
						num2 += mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
						num4 = c;
					}
				}
			}
			zero.x = ((num2 <= num) ? ((float)num) : ((float)num2)) / scale;
			zero.y = (float)(num3 + num5) / scale;
		}
		return zero;
	}

	protected static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && s[num] == ' ')
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	protected void Align(List<Vector3> verts, int indexOffset, Alignment alignment, int x, int lineWidth)
	{
		if (alignment != 0 && mFont.charSize > 0)
		{
			float num = ((alignment != Alignment.Right) ? ((float)(lineWidth - x) * 0.5f) : ((float)(lineWidth - x)));
			if (num < 0f)
			{
				num = 0f;
			}
			for (int i = indexOffset; i < verts.Count; i++)
			{
				Vector3 value = verts[i];
				value.x += num;
				verts[i] = value;
			}
		}
	}

	public void LoadBMFont()
	{
		if (!(null == fontTxt))
		{
			BMFont.Reader.Load(mFont, base.name, fontTxt.bytes);
		}
	}

	private void Update()
	{
		if (m_reload)
		{
			LoadBMFont();
			m_reload = false;
		}
	}
}
