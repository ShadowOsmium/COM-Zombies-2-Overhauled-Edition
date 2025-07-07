using UnityEngine;

[AddComponentMenu("TUI/Control/Fade")]
[RequireComponent(typeof(TUIMeshSprite))]
public class TUIFade : TUIControlImpl
{
	private enum State
	{
		Idle,
		FadeIn,
		FadeOut
	}

	public VOID_FUNCTION_0PARAM OnFadeInCallback;

	public VOID_FUNCTION_0PARAM OnFadeOutCallback;

	public bool autoFadeInWhenStart = true;

	public Color fadeInColorBegin = new Color(0f, 0f, 0f, 1f);

	public Color fadeInColorEnd = new Color(0f, 0f, 0f, 0f);

	public float fadeInTime = 3f;

	public Color fadeOutColorBegin = new Color(0f, 0f, 0f, 0f);

	public Color fadeOutColorEnd = new Color(0f, 0f, 0f, 1f);

	public float fadeOutTime = 3f;

	private TUIMeshSprite m_Sprite;

	private State state;

	private float time;

	private string fadeOutScene = string.Empty;

	public void FadeIn()
	{
		state = State.FadeIn;
		time = 0f;
		if (m_Sprite != null)
		{
			m_Sprite.color = fadeInColorBegin;
		}
	}

	public void FadeOut()
	{
		FadeOut(string.Empty);
	}

	public void FadeOut(string scene)
	{
		state = State.FadeOut;
		time = 0f;
		fadeOutScene = scene;
		if (m_Sprite != null)
		{
			m_Sprite.color = fadeOutColorBegin;
		}
	}

	public void Awake()
	{
		base.transform.localScale = new Vector3(size.x, size.y, 1f);
		m_Sprite = base.gameObject.GetComponent<TUIMeshSprite>();
	}

	public void Start()
	{
		if (autoFadeInWhenStart)
		{
			FadeIn();
		}
	}

	public void Update()
	{
		if (state != 0)
		{
			if (state == State.FadeIn)
			{
				UpdateFadeIn(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
			else if (state == State.FadeOut)
			{
				UpdateFadeOut(Mathf.Clamp(Time.deltaTime, 0f, 0.05f));
			}
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (state == State.Idle)
		{
			return false;
		}
		if (PtInControl(input.position))
		{
			return true;
		}
		return false;
	}

	private void UpdateFadeIn(float deltaTime)
	{
		time += deltaTime;
		if (m_Sprite != null)
		{
			float t = Mathf.Clamp(time / fadeInTime, 0f, 1f);
			m_Sprite.color = Color.Lerp(fadeInColorBegin, fadeInColorEnd, t);
		}
		if (time > fadeInTime)
		{
			state = State.Idle;
			if (OnFadeInCallback != null)
			{
				OnFadeInCallback();
			}
		}
	}

	private void UpdateFadeOut(float deltaTime)
	{
		time += deltaTime;
		if (m_Sprite != null)
		{
			float t = Mathf.Clamp(time / fadeOutTime, 0f, 1f);
			m_Sprite.color = Color.Lerp(fadeOutColorBegin, fadeOutColorEnd, t);
		}
		if (!(time > fadeOutTime))
		{
			return;
		}
		state = State.Idle;
		if (fadeOutScene.Length > 0)
		{
			if (OnFadeOutCallback != null)
			{
				OnFadeOutCallback();
			}
			Application.LoadLevel(fadeOutScene);
		}
	}
}
