using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopSceneController : UISceneController
{
	private UIShopPlayerController currentAvatar;

	public UIShopCamera MainCamera;

	public UIShopInputController InputController;

	public Transform CameraOrigin;

	public UIShopPanelController AvatarPanel;

	public UIShopPanelController PrimaryWeaponPanel;

	public UIShopPanelController SecondaryWeaponPanel;

	public UIPromotionPanelController PromotionPanel;

	public UIShopPanelController SkillDetalPanel;

	private List<UIShopPlayerController> avatarSet = new List<UIShopPlayerController>();

	public ParticleSystem level_up_eff;

	public new static UIShopSceneController Instance
	{
		get
		{
			return (UIShopSceneController)UISceneController.instance;
		}
	}

	public UIShopPlayerController CurrentAvatar
	{
		get
		{
			return currentAvatar;
		}
	}

	private void Awake()
	{
		UISceneController.instance = this;
		GameConfig.CheckGameConfig();
		GameData.CheckGameData();
		MenuAudioController.CheckGameMenuAudio();
		GameObject gameObject = base.transform.Find("AvatarSet").gameObject;
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			UIShopPlayerController component = gameObject2.GetComponent<UIShopPlayerController>();
			component.avatarData = GameData.Instance.AvatarData_Set[component.AvatarType];
			if (component.AvatarType == GameData.Instance.cur_avatar)
			{
				component.ChangeAvatarShader(true);
			}
			else
			{
				component.ChangeAvatarShader(false);
			}
			avatarSet.Add(component);
		}
	}

	private IEnumerator Start()
	{
		yield return 1;
		CheckAvatarPosInShop();
		GameObject.Find("LabelDay").GetComponent<TUILabel>().Text = "Day " + GameData.Instance.day_level;
		while (!GameConfig.Instance.Load_finished)
		{
			yield return 1;
		}
		HidePanels();
		LoadingUIController.FinishedLoading();
		if (GameData.Instance.show_ui_tutorial)
		{
			GameData.Instance.show_ui_tutorial = false;
		}
		Invoke("CheckRateGame", 1f);
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = OnTapJoyPointsAdd;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameData.Instance.UnlockList.Clear();
		if (TapJoyScript.Instance != null)
		{
			TapJoyScript.Instance.points_add_call_back = null;
		}
	}

	public void HidePanels()
	{
		foreach (UIShopPanelController item in PanelStack)
		{
			item.Hide(true);
		}
	}

	public void ShowAvatarPanel(UIShopPlayerController avatar)
	{
		currentAvatar = avatar;
		OnAvatarOnClick(currentAvatar.avatarData.avatar_type);
		if (currentAvatar.avatarData.exist_state == AvatarExistState.Owned && GameData.Instance.cur_avatar != currentAvatar.AvatarType)
		{
			UpdateAvatarShaders();
		}
		MainCamera.SmoothCameraTo(currentAvatar.CameraViewTrans);
		AvatarPanel.Show();
	}

	public void UpdateAvatarShaders()
	{
		GameData.Instance.cur_avatar = currentAvatar.AvatarType;
		foreach (UIShopPlayerController item in avatarSet)
		{
			if (item.avatarData.exist_state == AvatarExistState.Owned)
			{
				if (item.AvatarType == currentAvatar.AvatarType)
				{
					item.ChangeAvatarShader(true);
				}
				else
				{
					item.ChangeAvatarShader(false);
				}
			}
		}
	}

	public bool CheckShopCameraAtOrigin()
	{
		return Vector3.Distance(MainCamera.transform.position, CameraOrigin.position) < 1f;
	}

    private void CheckRateGame()
    {
        if (GameData.Instance.enter_shop_count < 3)
        {
            GameData.Instance.enter_shop_count++;

            if (GameData.Instance.enter_shop_count == 3)
            {
                int num = MiscPlugin.ShowMessageBox2(string.Empty, "Having fun? Join my discord server!", "Sure", "No, thanks.");

                Transform parentTransform = base.transform.parent != null ? base.transform.parent.parent : null;

                if (parentTransform != null)
                {
                    GameMsgBoxController.ShowMsgBox(
                        GameMsgBoxController.MsgBoxType.DoubleButton,
                        parentTransform.gameObject,
                        "Having fun? Join my discord server!",
                        HaveFunBox,
                        null
                    );
                }
                else
                {
                    Debug.LogWarning("Parent hierarchy is missing for message box target!");
                }
            }

            GameData.Instance.SaveData();
        }
    }

    private void HaveFunBox()
	{
		Application.OpenURL("https://discord.gg/X2j3szhMzt");
	}

	public void CheckAvatarPosInShop()
	{
		foreach (UIShopPlayerController item in avatarSet)
		{
			if (item.CheckAvatarPosInShop() && CurrentAvatar != null && item.avatar_state != CurrentAvatar.avatar_state && item.avatarData.avatar_type == CurrentAvatar.avatarData.avatar_type)
			{
				currentAvatar = item;
				OnAvatarSkinChanged();
			}
		}
	}

	public void OnAvatarOnClick(AvatarType avatar_type)
	{
		foreach (UIShopPlayerController item in avatarSet)
		{
			if (item.avatarData.avatar_type == avatar_type)
			{
				item.SetPlayerState(item.OnclickState);
			}
		}
	}

	private void OnAvatarSkinChanged()
	{
		Debug.Log("Avatar Skin Changed, avatar:" + currentAvatar.avatarData.avatar_name);
		level_up_eff.transform.position = currentAvatar.transform.position;
		level_up_eff.Play();
	}

	private void OnTapJoyPointsAdd()
	{
		MoneyController.UpdateInfo();
	}
}
