using System;
using System.Security.Cryptography;
using System.Text;

public class MD5Sample
{
	public static string GetMd5String(string fileData)
	{
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(fileData));
		string text = BitConverter.ToString(array);
		return text.Replace("-", string.Empty).ToLower();
	}

	public static string GetFileMD5(byte[] buffer)
	{
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] array = mD.ComputeHash(buffer);
		string text = BitConverter.ToString(array);
		return text.Replace("-", string.Empty).ToLower();
	}
}
