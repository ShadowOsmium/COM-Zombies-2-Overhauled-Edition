using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[ExecuteInEditMode]
public class TUITextureManager : MonoBehaviour
{
	[Serializable]
	public struct OutputParam
	{
		[Serializable]
		public struct FrameInfo
		{
			public string frameName;

			public int x;

			public int y;

			public int width;

			public int height;
		}

		[Serializable]
		public struct TextureInfo
		{
			public string textureFile;

			public FrameInfo[] frames;
		}

		public TextureInfo[] textureInfo;

		public TextureInfo[] textureInfoHD;
	}

	public TextAsset textureXml;

	public string m_materialPath;

	public bool autoLoadWhenAwake;

	private Dictionary<string, TUITextureInfo> m_TextureCenter = new Dictionary<string, TUITextureInfo>();

	private Dictionary<string, TUITextureInfo> m_TextureHDCenter = new Dictionary<string, TUITextureInfo>();

	public void Initialize(TextAsset xml)
	{
		if (null == xml)
		{
			if (null != textureXml)
			{
				Initialize(textureXml.text);
			}
		}
		else
		{
			Initialize(xml.text);
		}
	}

	public void Initialize(string content)
	{
		if (content.Length == 0)
		{
			return;
		}
		StringReader textReader = new StringReader(content);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(OutputParam));
		OutputParam outputParam = (OutputParam)xmlSerializer.Deserialize(textReader);
		m_TextureCenter.Clear();
		m_TextureHDCenter.Clear();
		for (int i = 0; i < outputParam.textureInfo.Length; i++)
		{
			OutputParam.TextureInfo textureInfo = outputParam.textureInfo[i];
			GameObject gameObject = GetGameObject<TUIMaterialInfo>(textureInfo.textureFile);
			TUIMaterialInfo component = gameObject.GetComponent<TUIMaterialInfo>();
			component.material = Resources.Load(m_materialPath + "/" + textureInfo.textureFile) as Material;
			for (int j = 0; j < textureInfo.frames.Length; j++)
			{
				OutputParam.FrameInfo frameInfo = textureInfo.frames[j];
				TUITextureInfo module = GetModule<TUITextureInfo>(gameObject, frameInfo.frameName);
				module.rect = new Rect(frameInfo.x, frameInfo.y, frameInfo.width, frameInfo.height);
				m_TextureCenter.Add(frameInfo.frameName, module);
			}
		}
		for (int k = 0; k < outputParam.textureInfoHD.Length; k++)
		{
			OutputParam.TextureInfo textureInfo2 = outputParam.textureInfoHD[k];
			GameObject gameObject2 = GetGameObject<TUIMaterialInfo>(textureInfo2.textureFile);
			TUIMaterialInfo component2 = gameObject2.GetComponent<TUIMaterialInfo>();
			component2.material = Resources.Load(m_materialPath + "/" + textureInfo2.textureFile) as Material;
			for (int l = 0; l < textureInfo2.frames.Length; l++)
			{
				OutputParam.FrameInfo frameInfo2 = textureInfo2.frames[l];
				TUITextureInfo module2 = GetModule<TUITextureInfo>(gameObject2, frameInfo2.frameName);
				module2.rect = new Rect(frameInfo2.x, frameInfo2.y, frameInfo2.width, frameInfo2.height);
				m_TextureHDCenter.Add(frameInfo2.frameName, module2);
			}
		}
	}

	public TUITextureInfo GetTextureInfo(string name, bool isRetina)
	{
		if (isRetina)
		{
			if (m_TextureHDCenter.ContainsKey(name))
			{
				return m_TextureHDCenter[name];
			}
		}
		else if (m_TextureCenter.ContainsKey(name))
		{
			return m_TextureCenter[name];
		}
		return null;
	}

	public TUITextureInfo GetTextureInfo(string name)
	{
		return GetTextureInfo(name, TUI.IsRetina());
	}

	private GameObject GetGameObject<T>(string name) where T : MonoBehaviour
	{
		GameObject gameObject = null;
		Transform transform = base.transform.Find(name);
		if ((bool)transform)
		{
			gameObject = transform.gameObject;
		}
		else
		{
			gameObject = new GameObject(name);
			gameObject.transform.parent = base.transform;
		}
		T component = gameObject.GetComponent<T>();
		if (!(UnityEngine.Object)component)
		{
			component = gameObject.AddComponent<T>();
		}
		return gameObject;
	}

	private T GetModule<T>(GameObject obj, string name) where T : MonoBehaviour
	{
		GameObject gameObject = null;
		Transform transform = obj.transform.Find(name);
		if ((bool)transform)
		{
			gameObject = transform.gameObject;
		}
		else
		{
			gameObject = new GameObject(name);
			gameObject.transform.parent = obj.transform;
		}
		T val = gameObject.GetComponent<T>();
		if (!(UnityEngine.Object)val)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}

	private void Awake()
	{
		if (autoLoadWhenAwake)
		{
			Initialize(textureXml);
		}
	}
}
