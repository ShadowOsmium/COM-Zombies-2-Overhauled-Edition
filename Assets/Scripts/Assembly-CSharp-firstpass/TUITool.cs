using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TUITool
{
	public const string colorBeginFlag = "{color:";

	public const string colorEndFlag = "{color}";

	public static Bounds CalculateAbsoluteControlBounds(Transform trans, bool includeInactive)
	{
		TUIControlImpl[] componentsInChildren = trans.GetComponentsInChildren<TUIControlImpl>();
		Bounds result = new Bounds(trans.transform.position, Vector3.zero);
		bool flag = true;
		TUIControlImpl[] array = componentsInChildren;
		foreach (TUIControlImpl tUIControlImpl in array)
		{
			Vector2 size = tUIControlImpl.size;
			Vector2 zero = Vector2.zero;
			float num = (zero.x + 0.5f) * size.x;
			float num2 = (zero.y - 0.5f) * size.y;
			size *= 0.5f;
			Transform transform = tUIControlImpl.transform;
			Vector3 vector = transform.TransformPoint(new Vector3(num - size.x, num2 - size.y, 0f));
			if (flag)
			{
				flag = false;
				result = new Bounds(vector, Vector3.zero);
			}
			else
			{
				result.Encapsulate(vector);
			}
			result.Encapsulate(transform.TransformPoint(new Vector3(num - size.x, num2 + size.y, 0f)));
			result.Encapsulate(transform.TransformPoint(new Vector3(num + size.x, num2 - size.y, 0f)));
			result.Encapsulate(transform.TransformPoint(new Vector3(num + size.x, num2 + size.y, 0f)));
		}
		return result;
	}

	public static Bounds CalculateRelativeControlBounds(Transform root, Transform child, bool includeInactive)
	{
		TUIControlImpl[] componentsInChildren = child.GetComponentsInChildren<TUIControlImpl>();
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		bool flag = true;
		TUIControlImpl[] array = componentsInChildren;
		foreach (TUIControlImpl tUIControlImpl in array)
		{
			Vector2 size = tUIControlImpl.size;
			Vector2 zero = Vector2.zero;
			float num = (zero.x + 0.5f) * size.x;
			float num2 = (zero.y - 0.5f) * size.y;
			num = 0f;
			num2 = 0f;
			size *= 0.5f;
			Transform transform = tUIControlImpl.transform;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(transform.TransformPoint(new Vector3(num - size.x, num2 - size.y, 0f)));
			if (flag)
			{
				flag = false;
				result = new Bounds(vector, Vector3.zero);
			}
			else
			{
				result.Encapsulate(vector);
			}
			result.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(transform.TransformPoint(new Vector3(num - size.x, num2 + size.y, 0f))));
			result.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(transform.TransformPoint(new Vector3(num + size.x, num2 - size.y, 0f))));
			result.Encapsulate(worldToLocalMatrix.MultiplyPoint3x4(transform.TransformPoint(new Vector3(num + size.x, num2 + size.y, 0f))));
		}
		return result;
	}

	public static Bounds CalculateRelativeControlBounds(Transform trans, bool includeInactive)
	{
		return CalculateRelativeControlBounds(trans, trans, includeInactive);
	}

	public static Color ParseColor(string text, int offset)
	{
		int num = (TUIMath.HexToDecimal(text[offset]) << 4) | TUIMath.HexToDecimal(text[offset + 1]);
		int num2 = (TUIMath.HexToDecimal(text[offset + 2]) << 4) | TUIMath.HexToDecimal(text[offset + 3]);
		int num3 = (TUIMath.HexToDecimal(text[offset + 4]) << 4) | TUIMath.HexToDecimal(text[offset + 5]);
		int num4 = (TUIMath.HexToDecimal(text[offset + 6]) << 4) | TUIMath.HexToDecimal(text[offset + 7]);
		float num5 = 0.003921569f;
		return new Color(num5 * (float)num, num5 * (float)num2, num5 * (float)num3, num5 * (float)num4);
	}

	public static string EncodeColor(Color c)
	{
		return TUIMath.ColorToInt(c).ToString("X8");
	}

	private static string ColorToStringBeginFlag(Color c)
	{
		return "{color:" + EncodeColor(c) + "}";
	}

	public static string ColorString(string str, Color c)
	{
		return ColorToStringBeginFlag(c) + str + "{color}";
	}

	public static int ParseSymbol(string text, int index, List<Color> colors)
	{
		int length = text.Length;
		if (index + "{color}".Length - 1 < length)
		{
			if (text.Substring(index, "{color}".Length).Equals("{color}"))
			{
				if (colors != null && colors.Count > 1)
				{
					colors.RemoveAt(colors.Count - 1);
				}
				return "{color}".Length;
			}
			if (index + "{color:".Length - 1 + 9 < length && text.Substring(index, "{color:".Length).Equals("{color:") && text[index + "{color:".Length - 1 + 9] == '}')
			{
				if (colors != null)
				{
					Color item = ParseColor(text, index + "{color:".Length);
					item.a = colors[colors.Count - 1].a;
					colors.Add(item);
				}
				return "{color:".Length + 9;
			}
		}
		return 0;
	}

	public static string StripSymbols(string text, bool romveColor)
	{
		if (text != null)
		{
			text = text.Replace("\\n", "\n");
			if (romveColor)
			{
				int num = 0;
				int length = text.Length;
				while (num < length)
				{
					char c = text[num];
					if (c == '{')
					{
						int num2 = ParseSymbol(text, num, null);
						if (num2 > 0)
						{
							text = text.Remove(num, num2);
							length = text.Length;
							continue;
						}
					}
					num++;
				}
			}
		}
		return text;
	}

	public static string GetHierarchy(GameObject obj)
	{
		string text = obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = obj.name + "/" + text;
		}
		return "\"" + text + "\"";
	}

	public static void AmendFieldNullString(object obj)
	{
		Type type = obj.GetType();
		List<FieldInfo> list = new List<FieldInfo>(type.GetFields());
		list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
		foreach (FieldInfo item in list)
		{
			if (item.FieldType == typeof(string) && item.GetValue(obj) == null)
			{
				item.SetValue(obj, string.Empty);
			}
		}
	}

	public static int[] UintToNumberArray(uint value)
	{
		string text = value.ToString();
		int[] array = new int[text.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Convert.ToInt32(text[text.Length - 1 - i].ToString());
		}
		return array;
	}

	public static void ObjectFunctionInvoke(object obj, string functionName, params object[] @params)
	{
		try
		{
			MethodInfo method = obj.GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.Invoke(obj, @params);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
			Debug.LogWarning("functionName:" + functionName);
		}
	}

	public static void TUIControlEventInvoke(TUIControl control, params object[] @params)
	{
		try
		{
			Component component = control.invokeObject.GetComponent(control.componentName);
			ObjectFunctionInvoke(component, control.invokeFunctionName, @params);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
		}
	}

	public static string StringFormat(string formatString, params object[] args)
	{
		string text = formatString;
		if (args == null)
		{
			throw new ArgumentNullException();
		}
		for (int i = 0; i < args.Length; i++)
		{
			text = text.Replace("{" + i + "}", args[i].ToString());
		}
		return text;
	}

	public static Material CreateUITextureMaterial()
	{
		return new Material(Shader.Find("Triniti/Sprite"));
	}
}
