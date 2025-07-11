using System;
using System.IO;
using UnityEngine;

public class Utils
{
    private static string m_SavePath;
    private static string m_DataPath;

    static Utils()
    {
        m_SavePath = Application.persistentDataPath;
        m_DataPath = Application.dataPath;
        if (m_SavePath[m_SavePath.Length - 1] != '/')
        {
            m_SavePath += "/";
        }
        if (m_DataPath[m_DataPath.Length - 1] != '/')
        {
            m_DataPath += "/";
        }
    }

    public static string SavePath()
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) &&
            TesterSaveManager.Instance != null && TesterSaveManager.Instance.allowTesterSaves)
        {
            // Only on Android/iOS, redirect to alternate folder
            string basePath = Application.persistentDataPath;

            string parentDir = Directory.GetParent(basePath).FullName;
            string altPath = System.IO.Path.Combine(parentDir, "COMZ2_Testers");

            if (!Directory.Exists(altPath))
                Directory.CreateDirectory(altPath);

            return altPath.EndsWith("/") ? altPath : altPath + "/";
        }

        // Default path for PC, or if not using tester saves
        return m_SavePath;
    }

    public static string DataPath()
    {
        return m_DataPath;
    }

    public static string LoadResourcesFileForText(string filename)
    {
        TextAsset textAsset = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
        if (null != textAsset)
        {
            return textAsset.text;
        }
        return string.Empty;
    }

    public static void FileWriteString(string FileName, string WriteString)
    {
        try
        {
            string encryptedData = EncryptUtils.EncryptData(WriteString);
            using (FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(encryptedData);
                streamWriter.Flush();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("File write error: " + FileName + " Exception: " + ex.Message);
        }
    }

    public static bool FileReadString(string FileName, ref string content)
    {
        if (!File.Exists(FileName))
        {
            Debug.LogWarning("file: " + FileName + " does not exist!");
            return false;
        }

        try
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                content = streamReader.ReadToEnd();
            }

            // Try to decrypt the content
            string decryptedContent = EncryptUtils.DecryptData(content);
            if (string.IsNullOrEmpty(decryptedContent))
            {
                // If decryption fails, delete the file and return false
                Debug.LogWarning("Decryption failed. Deleting the file and creating a new one.");
                File.Delete(FileName);
                return false;
            }

            content = decryptedContent;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading or decrypting file: " + FileName + " Exception: " + ex.Message);
            return false;
        }

        return true;
    }
}
