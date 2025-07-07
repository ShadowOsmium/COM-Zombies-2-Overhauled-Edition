using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptUtils
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1f8Xv9bKd0zR7LmP2uE4cYtNq6sHgWj");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("Z9xC7vB3mQpL5nKf");

    public static string EncryptData(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Close();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string DecryptData(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            byte[] buffer = Convert.FromBase64String(cipherText);
            using (MemoryStream msDecrypt = new MemoryStream(buffer))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
