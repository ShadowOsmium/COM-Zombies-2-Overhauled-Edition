using System.Security.Cryptography;
using System.Text;

public static class HashUtils
{
    /// <summary>
    /// Calculates a SHA256 hash and returns a lowercase hex string.
    /// </summary>
    public static string GetSha256String(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return BytesToHexString(hash);
        }
    }

    /// <summary>
    /// Calculates an MD5 hash and returns a lowercase hex string.
    /// </summary>
    public static string GetMd5String(string input)
    {
        using (var md5 = MD5.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(bytes);
            return BytesToHexString(hash);
        }
    }

    /// <summary>
    /// Converts a byte array to a lowercase hex string without dashes.
    /// </summary>
    private static string BytesToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var b in bytes)
            sb.Append(b.ToString("x2")); // lowercase hex
        return sb.ToString();
    }
}
