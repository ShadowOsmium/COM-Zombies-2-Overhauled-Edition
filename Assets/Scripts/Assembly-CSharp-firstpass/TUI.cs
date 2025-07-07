using UnityEngine;

[ExecuteInEditMode]
public class TUI : TUIInputHandleModel
{
	public bool drawGizmos = true;

	public int layer = 8;

	public int depth;

	public TextAsset textureXml;

	private TUICamera tuiCamera;

	private TUIControlManager tuiControlMgr;

	private TUITextureManager[] tuiTextureMgrs;

	public TUICamera Camera
	{
		get
		{
			if (null == tuiCamera)
			{
				tuiCamera = GetModule<TUICamera>("TUICamera");
			}
			return tuiCamera;
		}
	}

	private TUIControlManager ControlManager
	{
		get
		{
			if (null == tuiControlMgr)
			{
				tuiControlMgr = GetModule<TUIControlManager>("TUIControls");
			}
			return tuiControlMgr;
		}
	}

	private TUITextureManager[] TextureManagers
	{
		get
		{
			if (tuiTextureMgrs == null)
			{
				tuiTextureMgrs = GetComponentsInChildren<TUITextureManager>();
			}
			return tuiTextureMgrs;
		}
	}

	public void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			float num = 240f;
			float num2 = 160f;
			Vector3[] array = new Vector3[4]
			{
				base.transform.TransformPoint(0f - num, num2, 0f),
				base.transform.TransformPoint(num, num2, 0f),
				base.transform.TransformPoint(num, 0f - num2, 0f),
				base.transform.TransformPoint(0f - num, 0f - num2, 0f)
			};
			Gizmos.color = Color.white;
			Gizmos.DrawLine(array[0], array[1]);
			Gizmos.DrawLine(array[1], array[2]);
			Gizmos.DrawLine(array[2], array[3]);
			Gizmos.DrawLine(array[3], array[0]);
			Gizmos.DrawLine(array[0], array[2]);
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (drawGizmos)
		{
			float num = 240f;
			float num2 = 160f;
			Vector3[] array = new Vector3[4]
			{
				base.transform.TransformPoint(0f - num, num2, 0f),
				base.transform.TransformPoint(num, num2, 0f),
				base.transform.TransformPoint(num, 0f - num2, 0f),
				base.transform.TransformPoint(0f - num, 0f - num2, 0f)
			};
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(array[0], array[1]);
			Gizmos.DrawLine(array[1], array[2]);
			Gizmos.DrawLine(array[2], array[3]);
			Gizmos.DrawLine(array[3], array[0]);
			Gizmos.DrawLine(array[0], array[2]);
		}
	}

	private void Awake()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		Camera.Initialize(layer, depth);
		ControlManager.Initialize(layer);
		GetModule<TUITextureManager>("TUITexture").Initialize(textureXml);
	}

	public static bool IsRetina()
	{
		return Mathf.Min(Screen.width, Screen.height) >= 640;
	}

	public static bool IsDoubleHD()
	{
		if ((Screen.width >= 1920 && Screen.height >= 1280) || (Screen.width >= 1280 && Screen.height >= 1920))
		{
			return true;
		}
		return false;
	}

	public TUITextureInfo GetTextureInfo(string name, bool isRetina)
	{
		if (TextureManagers == null)
		{
			return null;
		}
		TUITextureManager[] textureManagers = TextureManagers;
		foreach (TUITextureManager tUITextureManager in textureManagers)
		{
			if (tUITextureManager.transform.GetChildCount() != 0)
			{
				TUITextureInfo textureInfo = tUITextureManager.GetTextureInfo(name, isRetina);
				if (null != textureInfo)
				{
					return textureInfo;
				}
			}
		}
		return null;
	}

	public TUITextureInfo GetTextureInfo(string name)
	{
		if (TextureManagers == null)
		{
			return null;
		}
		TUITextureManager[] textureManagers = TextureManagers;
		foreach (TUITextureManager tUITextureManager in textureManagers)
		{
			if (tUITextureManager.transform.GetChildCount() != 0)
			{
				TUITextureInfo textureInfo = tUITextureManager.GetTextureInfo(name);
				if (null != textureInfo)
				{
					return textureInfo;
				}
			}
		}
		return null;
	}

	public void SetHandler(TUIHandler handler)
	{
		ControlManager.SetHandler(handler);
	}

	public override bool HandleInput(TUIInput input)
	{
		Vector3 position = new Vector3(input.position.x, input.position.y, tuiCamera.GetComponent<Camera>().nearClipPlane);
		Vector3 vector = tuiCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
		input.position.x = vector.x;
		input.position.y = vector.y;
		return ControlManager.HandleInput(input);
	}

	private T GetModule<T>(string name) where T : MonoBehaviour
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
		T val = gameObject.GetComponent<T>();
		if (!(Object)val)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}
}
