using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class EHelper
{
    private const string SecretKey = "Ia2ZKta4n1RN4J2rBB8NAw";

    public static string DataEncipher(string data)
    {
        if (!GameConfig.IsEditorMode())
        {
            var blowfish = new TNetSdk.BlowFish(SecretKey);
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);

            int paddedLength = ((plainBytes.Length + 7) / 8) * 8;
            byte[] padded = new byte[paddedLength];
            Array.Copy(plainBytes, padded, plainBytes.Length);

            for (int i = 0; i < padded.Length; i += 8)
            {
                ulong block = BitConverter.ToUInt64(padded, i);
                blowfish.Encrypt(ref block);
                Array.Copy(BitConverter.GetBytes(block), 0, padded, i, 8);
            }

            return Convert.ToBase64String(padded);
        }
        return data;
    }

    public static string DataDecrypt(string data)
    {
        if (!GameConfig.IsEditorMode())
        {
            var blowfish = new TNetSdk.BlowFish(SecretKey);
            byte[] encrypted = Convert.FromBase64String(data);

            for (int i = 0; i < encrypted.Length; i += 8)
            {
                ulong block = BitConverter.ToUInt64(encrypted, i);
                blowfish.Decrypt(ref block);
                Array.Copy(BitConverter.GetBytes(block), 0, encrypted, i, 8);
            }

            return Encoding.UTF8.GetString(encrypted).TrimEnd('\0');
        }
        return data;
    }
}
