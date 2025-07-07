using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class ProjectFixer
{
	public static void Fix()
	{
		foreach (GameObject gobj in Resources.FindObjectsOfTypeAll<GameObject>())
		{
			if (gobj.GetComponent<MeshFilter>() && gobj.GetComponent<MeshCollider>())
			{
				gobj.GetComponent<MeshFilter>().sharedMesh = gobj.GetComponent<MeshCollider>().sharedMesh;
			}
		}
	}
    public static void FixAnims()
    {
        string str = Application.dataPath + "/AnimationClip";
		string str2 = str + "/";
        Debug.LogError(str);
		int amount = PlayerPrefs.GetInt("fixedAnimsAmount");
		foreach (string file in Directory.GetFiles(str))
		{
			if (file.EndsWith("_" + amount + ".anim"))
			{
				amount++;
			}
		}
        for (int i = 0; i < amount; i++)
        {
            Directory.CreateDirectory(str2 + i);
            foreach (string file in Directory.GetFiles(str))
            {
                if (file.EndsWith("_" + i + ".anim"))
                {
                    File.Move(file, str2 + i + "/" + GetBitBefore(file.Substring(str.Length), "_" + i + ".anim") + ".anim");
                }
                if (file.EndsWith("_" + i + ".anim.meta"))
                {
                    File.Move(file, str2 + i + "/" + GetBitBefore(file.Substring(str.Length), "_" + i + ".anim.meta") + ".anim.meta");
                }
            }
        }
        PlayerPrefs.SetInt("fixedAnimsAmount", PlayerPrefs.GetInt("fixedAnimsAmount") + 100);
    }

    public static void FixAutoAnims()
    {
        foreach (Animation anim in Resources.FindObjectsOfTypeAll<Animation>())
        {
            if (anim.GetClipCount() == 1 && anim.playAutomatically && anim.clip == null)
            {
                foreach (AnimationState state in anim)
                {
                    anim.clip = state.clip;
                    break;
                }
            }
        }
    }

    public static string GetBitBefore(string text, string end) 
    {
          var index = text.IndexOf(end);
          if (index == -1) return text;

           return text.Substring(0, index);
    }
}
