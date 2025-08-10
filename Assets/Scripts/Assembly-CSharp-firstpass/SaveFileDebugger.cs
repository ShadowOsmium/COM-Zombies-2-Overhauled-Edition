/*using UnityEngine;
using System.Collections.Generic;
using CoMZ2;

public class SaveFileDebugger : MonoBehaviour
{
    void Update()
    {
        // When you press 'D', dump the save data
        if (Input.GetKeyDown(KeyCode.D))
        {
            DumpSaveFile();
        }
    }

    void DumpSaveFile()
    {
        string savePath = Utils.SavePath() + MD5Sample.GetMd5String("CoMZ2") + ".bytes";

        string encryptedData = null;
        if (!Utils.FileReadString(savePath, ref encryptedData))
        {
            Debug.LogError("Save file not found at: " + savePath);
            return;
        }

        string decryptedData = GameData.DataDecrypt(encryptedData); // or wherever your decrypt method is

        Configure configure = new Configure();
        configure.Load(decryptedData);

        // You have to know the section(s) you want to read
        string section = "Save";  // Or the actual section name your save uses

        Debug.Log("==== SAVE FILE DUMP ====");
        Debug.Log("Section: " + section);

        List<string> keys = configure.GetAllKeysInSection(section);

        Debug.LogFormat("  Keys ({0}): {1}", keys.Count, string.Join(", ", keys.ToArray()));

        if (keys.Contains("WeaponsDataCount"))
        {
            int count = int.Parse(configure.GetSingle(section, "WeaponsDataCount"));
            Debug.Log("  WeaponsDataCount = " + count);
            for (int i = 0; i < Mathf.Min(3, count); i++)
            {
                string weaponName = configure.GetArray2(section, "WeaponsData", i, 0);
                Debug.Log("    Weapon[" + i + "]: " + weaponName);
            }
        }

        if (keys.Contains("AvatarsDataCount"))
        {
            int count = int.Parse(configure.GetSingle(section, "AvatarsDataCount"));
            Debug.Log("  AvatarsDataCount = " + count);
            for (int i = 0; i < Mathf.Min(3, count); i++)
            {
                string avatarName = configure.GetArray2(section, "AvatarsData", i, 0);
                Debug.Log("    Avatar[" + i + "]: " + avatarName);
            }
        }

        if (keys.Contains("SkillAvatarDataCount"))
        {
            int count = int.Parse(configure.GetSingle(section, "SkillAvatarDataCount"));
            Debug.Log("  SkillAvatarDataCount = " + count);
            for (int i = 0; i < Mathf.Min(3, count); i++)
            {
                string skillName = configure.GetArray2(section, "SkillAvatarData", i, 0);
                Debug.Log("    Skill[" + i + "]: " + skillName);
            }
        }
        Debug.Log("==== END SAVE FILE DUMP ====");
    }
}*/