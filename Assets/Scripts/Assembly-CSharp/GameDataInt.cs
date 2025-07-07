using UnityEngine;

public class GameDataInt
{
    public string data = string.Empty;

    private int value;

    public GameDataInt(int val, GameDataIntPurpose purpose = GameDataIntPurpose.Generic)
    {
        bool punish = false;

        switch (purpose)
        {
            case GameDataIntPurpose.Cash:
                if (val > 2500000) punish = true;
                else if (val > 1500000) val = 1500000;
                break;

            case GameDataIntPurpose.Crystal:
                if (val > 1350) punish = true;
                else if (val > 1000) val = 1000;
                break;

            case GameDataIntPurpose.Voucher:
                if (val > 1650) punish = true;
                else if (val > 1250) val = 1250;
                break;
        }

        if (punish)
        {
            Debug.LogWarning("Suspicious " + purpose + " value detected: " + val);
            val = 0;

            if (GameData.Instance != null)
            {
                GameData.Instance.blackname = true;
                GameData.Instance.SaveData();
            }
        }

        string data_encipher = val.ToString();
        data = Encipher(data_encipher);
    }

    private string Decrypt()
    {
        return Encipher(data);
    }

    public void SetIntVal(int val, GameDataIntPurpose purpose = GameDataIntPurpose.Generic)
    {
        bool punish = false;

        switch (purpose)
        {
            case GameDataIntPurpose.Cash:
                if (val > 2500000) punish = true;
                else if (val > 1500000) val = 1500000;
                break;
            case GameDataIntPurpose.Crystal:
                if (val > 1350) punish = true;
                else if (val > 1000) val = 1000;
                break;
            case GameDataIntPurpose.Voucher:
                if (val > 1650) punish = true;
                else if (val > 1250) val = 1250;
                break;
            case GameDataIntPurpose.FreeSpin:
                if (val > 8) punish = true;
                else if (val > 5) val = 5;
                break;
        }

        if (punish)
        {
            Debug.LogWarning("Suspicious " + purpose + " value detected: " + val);
            val = 0;

            if (GameData.Instance != null)
            {
                GameData.Instance.blackname = true;
                GameData.Instance.SaveData();
            }
        }

        string data_encipher = val.ToString();
        data = Encipher(data_encipher);
        value = val;
    }

    private string Encipher(string data_encipher)
    {
        int num = 71;
        char[] array = data_encipher.ToCharArray();
        char[] array2 = data_encipher.ToCharArray();
        char[] array3 = new char[2] { '\0', '\0' };
        for (int i = 0; i < array.Length; i++)
        {
            char c = (array3[0] = array[i]);
            string s = new string(array3);
            int num2 = char.ConvertToUtf32(s, 0);
            num2 ^= num;
            array2[i] = char.ConvertFromUtf32(num2)[0];
        }
        return new string(array2);
    }

    public override string ToString()
    {
        return Decrypt();
    }

    public int GetIntVal()
    {
        return int.Parse(Decrypt());
    }

    public static GameDataInt operator +(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return new GameDataInt(c1 + num2);
    }

    public static GameDataInt operator -(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return new GameDataInt(c1 - num2);
    }

    public static GameDataInt operator -(GameDataInt c1, int c2)
    {
        int num = int.Parse(c1.Decrypt());
        return new GameDataInt(num - c2);
    }

    public static bool operator >=(GameDataInt c1, int c2)
    {
        int num = int.Parse(c1.Decrypt());
        return num >= c2;
    }

    public static bool operator <=(GameDataInt c1, int c2)
    {
        int num = int.Parse(c1.Decrypt());
        return num <= c2;
    }

    public static bool operator >(GameDataInt c1, int c2)
    {
        int num = int.Parse(c1.Decrypt());
        return num > c2;
    }

    public static bool operator <(GameDataInt c1, int c2)
    {
        int num = int.Parse(c1.Decrypt());
        return num < c2;
    }

    public static GameDataInt operator +(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return new GameDataInt(num + num2);
    }

    public static GameDataInt operator -(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return new GameDataInt(num - num2);
    }

    public static bool operator >=(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return num >= num2;
    }

    public static bool operator <=(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return num <= num2;
    }

    public static bool operator >(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return num > num2;
    }

    public static bool operator >=(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return c1 >= num2;
    }

    public static bool operator <=(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return c1 <= num2;
    }

    public static bool operator >(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return c1 > num2;
    }

    public static bool operator <(int c1, GameDataInt c2)
    {
        int num2 = int.Parse(c2.Decrypt());
        return c1 < num2;
    }

    public static bool operator <(GameDataInt c1, GameDataInt c2)
    {
        int num = int.Parse(c1.Decrypt());
        int num2 = int.Parse(c2.Decrypt());
        return num < num2;
    }
}
