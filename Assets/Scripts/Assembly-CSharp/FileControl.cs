using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

public class FileControl
{
	protected Hashtable hashtable;

	public void Add(string key, object value)
	{
		if (hashtable == null)
		{
			hashtable = new Hashtable();
		}
		if (!hashtable.ContainsKey(key))
		{
			hashtable.Add(key, value);
		}
		else
		{
			hashtable[key] = value;
		}
	}

	public object Get(string key)
	{
		return hashtable[key];
	}

	protected void Remove(string key)
	{
		if (hashtable != null && hashtable.ContainsKey(key))
		{
			hashtable.Remove(key);
		}
	}

	public void Save(string path)
	{
		string path2 = path.Substring(0, path.LastIndexOf("/"));
		if (!Directory.Exists(path2))
		{
			Directory.CreateDirectory(path2);
		}
		FileStream fileStream = File.Open(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(ToXml(hashtable).OuterXml);
		binaryWriter.Close();
		fileStream.Close();
		Debug.Log(path + "-Save Success!");
	}

	public void Load(string path)
	{
		FileStream fileStream = File.Open(path, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		string xml = binaryReader.ReadString();
		binaryReader.Close();
		fileStream.Close();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		hashtable = ToHashTable(xmlDocument);
		Debug.Log(path + "-Load Success!");
	}

	private static Hashtable ToHashTable(XmlDocument xmlDoc)
	{
		Hashtable hashtable = new Hashtable();
		XmlNode documentElement = xmlDoc.DocumentElement;
		foreach (XmlNode childNode in documentElement.ChildNodes)
		{
			hashtable.Add(childNode.Name, childNode.InnerText);
		}
		return hashtable;
	}

	private static XmlDocument ToXml(Hashtable hashtable)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateElement("Config");
		xmlDocument.AppendChild(xmlNode);
		foreach (string key in hashtable.Keys)
		{
			XmlElement xmlElement = xmlDocument.CreateElement(key);
			xmlElement.InnerText = hashtable[key].ToString();
			xmlNode.AppendChild(xmlElement);
		}
		return xmlDocument;
	}

	public static string ColorToString(Color color)
	{
		return color.r + ":" + color.g + ":" + color.b + ":" + color.a;
	}

	public static Color StringToColor(string str)
	{
		string[] array = str.Split(':');
		return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	public static string Vector3ToString(Vector3 vec)
	{
		return vec.x + ":" + vec.y + ":" + vec.z;
	}

	public static Vector3 StringToVector3(string str)
	{
		string[] array = str.Split(':');
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}
}
