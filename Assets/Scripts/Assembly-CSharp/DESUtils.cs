using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DESUtils
{
	public static string Encrypt(string pToEncrypt, string sKey)
	{
		//Discarded unreachable code: IL_008d
		using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
		{
			byte[] bytes = Encoding.UTF8.GetBytes(pToEncrypt);
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
			}
			string result = Convert.ToBase64String(memoryStream.ToArray());
			memoryStream.Close();
			return result;
		}
	}

	public static string Decrypt(string pToDecrypt, string sKey)
	{
		//Discarded unreachable code: IL_008d
		byte[] array = Convert.FromBase64String(pToDecrypt);
		using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
		{
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();
			}
			string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
			memoryStream.Close();
			return @string;
		}
	}
}
