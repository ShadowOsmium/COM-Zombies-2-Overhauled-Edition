using System.Security.Cryptography;
using System.Text;

public static class SHA256Sample
{
    public static string GetSha256String(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2")); // two-character lowercase hex

            return sb.ToString();
        }
    }
}
