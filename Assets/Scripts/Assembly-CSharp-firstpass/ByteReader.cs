using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
	private byte[] mBuffer;

	private int mOffset;

	public bool canRead
	{
		get
		{
			return mBuffer != null && mOffset < mBuffer.Length;
		}
	}

	public ByteReader(byte[] bytes)
	{
		mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		mBuffer = asset.bytes;
	}

	public string ReadLine()
	{
		int num = mBuffer.Length;
		while (mOffset < num && mBuffer[mOffset] < 32)
		{
			mOffset++;
		}
		int num2 = mOffset;
		if (num2 < num)
		{
			int num3 = 0;
			do
			{
				if (num2 < num)
				{
					num3 = mBuffer[num2++];
					continue;
				}
				num2++;
				break;
			}
			while (num3 != 10 && num3 != 13);
			string @string = Encoding.UTF8.GetString(mBuffer, mOffset, num2 - mOffset - 1);
			mOffset = num2;
			return @string;
		}
		mOffset = num;
		return null;
	}

	public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] separator = new char[1] { '=' };
		while (canRead)
		{
			string text = ReadLine();
			if (text == null)
			{
				break;
			}
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				dictionary.Add(array[0].Trim(), array[1].Trim());
			}
		}
		return dictionary;
	}
}
