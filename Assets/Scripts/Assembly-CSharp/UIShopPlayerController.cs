using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class UIShopPlayerController : MonoBehaviour
{
	public string ANI_IDLE = "Idle01";

	public string ANI_IDLE_AFTER = "Idle02";

	public string ANI_ONCLICK = "Idle03";

	public string ANI_ONCLICK_AFTER = "Idle04";

	public UIShopPlayerState IdleState;

	public UIShopPlayerState IdleAfterState;

	public UIShopPlayerState OnclickState;

	public UIShopPlayerState OnclickAfterState;

	public AvatarType AvatarType = AvatarType.None;

	public AvatarData.AvatarState avatar_state = AvatarData.AvatarState.Normal;

	protected AvatarData avatar_data;

	public Transform CameraViewTrans;

	public List<Renderer> RenderList = new List<Renderer>(4);

	protected UIShopPlayerState playerState;

	protected UIShopPlayerStateType playerStateType;

	public GameObject SubAnimationObj;

	public Vector3 shop_pos = Vector3.zero;

	public Quaternion shop_rot = Quaternion.identity;

	public Vector3 shop_pos_hide = Vector3.zero;

	public Quaternion shop_rot_hide = Quaternion.identity;

	public AvatarData avatarData
	{
		get
		{
			return avatar_data;
		}
		set
		{
			avatar_data = value;
		}
	}

	public UIShopPlayerState PlayerState
	{
		get
		{
			return playerState;
		}
	}

	private void Awake()
	{
		shop_pos = base.transform.localPosition;
		shop_rot = base.transform.localRotation;
		shop_pos_hide += Vector3.down * 5f;
	}

	private void Start()
	{
		IdleState = UIShopPlayerState.Create(UIShopPlayerStateType.Idle, this);
		IdleAfterState = UIShopPlayerState.Create(UIShopPlayerStateType.IdleAfter, this);
		OnclickState = UIShopPlayerState.Create(UIShopPlayerStateType.OnClick, this);
		OnclickAfterState = UIShopPlayerState.Create(UIShopPlayerStateType.OnCLickAfter, this);
		OnBackFromClicked();
	}

	private void Update()
	{
		if (playerState != null)
		{
			playerState.DoStateLogic(Time.deltaTime);
		}
	}

	public void SetPlayerState(UIShopPlayerState state)
	{
		if (playerState == null)
		{
			playerState = state;
			playerState.OnEnterState();
			playerStateType = state.GetStateType();
		}
		else if (playerState != null && playerState.GetStateType() != state.GetStateType())
		{
			playerState.OnExitState();
			playerState = state;
			playerState.OnEnterState();
			playerStateType = state.GetStateType();
		}
	}

	public void ChangeAvatarShader(bool isChosen)
	{
		foreach (Renderer render in RenderList)
		{
			Color color = Color.clear;
			Color color2 = Color.white;
			if (avatar_data.exist_state == AvatarExistState.Owned)
			{
				color = ((!isChosen) ? new Color(1f, 1f, 1f, 0f) : new Color(0.3f, 1f, 0f, 0.6f));
				color2 = ((!isChosen) ? new Color(0.6f, 0.6f, 0.6f, 1f) : Color.white);
			}
			else if (avatar_data.exist_state == AvatarExistState.Unlocked)
			{
				color = new Color(1f, 1f, 1f, 0f);
				color2 = new Color(0.6f, 0.6f, 0.6f, 1f);
			}
			else if (avatar_data.exist_state == AvatarExistState.Locked)
			{
				color = new Color(1f, 1f, 1f, 0f);
				color2 = new Color(0.1f, 0.1f, 0.1f, 1f);
			}
			render.material.SetColor("_OutlineColor", color);
			render.material.SetColor("_Color", color2);
		}
	}

	public void OnBackFromClicked()
	{
		SetPlayerState(IdleState);
		if (AvatarType == AvatarType.Swat)
		{
			AnimationUtil.Stop(SubAnimationObj, ANI_ONCLICK);
			Transform transform = SubAnimationObj.transform.Find("Dummy_Swat02_All/Dummy_Swat02_Root/Bone_Swat02_C");
			transform.Rotate(0f, 0f, -82f);
		}
	}

	private void OnAnimateSwatCap()
	{
		AnimationUtil.PlayAnimate(SubAnimationObj, ANI_ONCLICK, WrapMode.Once);
	}

	private void ShowInShop()
	{
		base.transform.localPosition = shop_pos;
		base.transform.localRotation = shop_rot;
	}

	private void HideInShop()
	{
		base.transform.localPosition = shop_pos_hide;
		base.transform.localRotation = shop_rot_hide;
	}

	public bool CheckAvatarPosInShop()
	{
		if (avatar_state == avatar_data.avatar_state)
		{
			ShowInShop();
			return true;
		}
		HideInShop();
		return false;
	}
}
