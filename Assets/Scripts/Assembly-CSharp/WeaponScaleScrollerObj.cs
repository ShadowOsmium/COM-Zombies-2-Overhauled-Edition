using System;
using CoMZ2;
using UnityEngine;

public class WeaponScaleScrollerObj : MonoBehaviour
{
	public TUIMeshSprite WeaponImage;

	public TUIMeshSprite BackgroundImage;

	public TUIMeshSprite ExistStateImage;

	public GameObject CraftEffect;

	public TUILabel Level;

	public OnAnimateCraftEffectEnd OnAnimateCraftEffectEnd;

	private Vector3 oriPositon;

	private float scaleFactor;

	private float alphaFactor;

	private float depthFactor;

	private bool isAnimateCraftEffect;

	private GameObject newLabel;

	private bool isNew;

	public WeaponData WeaponData { get; private set; }

	private void Update()
	{
		if (isAnimateCraftEffect && AnimationUtil.IsAnimationPlayedPercentage(CraftEffect, "guang", 0.9f))
		{
			CraftEffect.SetActive(false);
			isAnimateCraftEffect = false;
			UpdateInfo();
			if (OnAnimateCraftEffectEnd != null)
			{
				OnAnimateCraftEffectEnd();
			}
		}
	}

	public void Init(WeaponData data)
	{
		WeaponData = data;
		oriPositon = Vector3.zero;
		scaleFactor = 0.00160000008f;
		alphaFactor = 0.00222222228f;
		depthFactor = UIShopWeaponPanelController.ScrollerPageDepthDistance / UIShopWeaponPanelController.ScrollerPageDistance;
		WeaponImage.texture = "weapon_" + data.weapon_name;
		WeaponImage.GrayStyle = data.exist_state != WeaponExistState.Owned;
		string texture = ((data.exist_state != 0) ? string.Empty : "tag_locked");
		if (data.weapon_name == UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment)
		{
			texture = "tag_equipped";
		}
		ExistStateImage.texture = texture;
		CraftEffect.SetActive(false);
		if (data.is_secondary)
		{
			Level.gameObject.SetActive(true);
			Level.Text = "LV" + data.level.ToString("G");
		}
		foreach (UnlockInGame unlock in GameData.Instance.UnlockList)
		{
			if (unlock.Type == UnlockInGame.UnlockType.Weapon && unlock.Name == data.weapon_name)
			{
				newLabel = UnityEngine.Object.Instantiate(Resources.Load("Prefab/NewLabel")) as GameObject;
				newLabel.transform.parent = base.transform;
				newLabel.transform.localPosition = new Vector3(-95f, 37f, -1f);
				isNew = true;
				break;
			}
		}
	}

	public void UpdateScaleScrollerView()
	{
		float num = base.transform.localPosition.x + base.transform.parent.localPosition.x;
		float num2 = Mathf.Abs(oriPositon.x - num);
		if (isNew)
		{
			if (Math.Abs(num2) < 0.0001f)
			{
				IsNotNew();
			}
		}
		else if (newLabel != null && Mathf.Abs(num2) > 50f)
		{
			UnityEngine.Object.Destroy(newLabel);
		}
		float num3 = 1f - num2 * scaleFactor;
		num3 = ((!(num3 < 0f)) ? num3 : 0f);
		base.transform.localScale = new Vector3(num3, num3, 1f);
		float z = num2 * depthFactor;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, z);
		float num4 = 0.8f - num2 * alphaFactor;
		if (WeaponData.exist_state == WeaponExistState.Owned && num2 < UIShopWeaponPanelController.ScrollerPageDistance * 0.6f)
		{
			num4 += 0.2f;
		}
		num4 = ((!(num4 < 0.1f)) ? num4 : 0.1f);
		BackgroundImage.color = new Color(1f, 1f, 1f, num4);
		if (!WeaponImage.GrayStyle)
		{
			WeaponImage.color = new Color(1f, 1f, 1f, num4);
		}
		else
		{
			WeaponImage.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, num4));
		}
	}

	public void AnimateCraftEffect()
	{
		CraftEffect.SetActive(true);
		isAnimateCraftEffect = true;
		AnimationUtil.PlayAnimate(CraftEffect, "guang", WrapMode.Once);
		AnimationUtil.PlayAnimate(WeaponImage.gameObject, "weaponOwnedEffect", WrapMode.Once);
		UISceneController.Instance.SceneAudio.PlayAudio("UI_craft");
	}

	private void UpdateInfo()
	{
		WeaponImage.GrayStyle = WeaponData.exist_state != WeaponExistState.Owned;
		string texture = ((WeaponData.exist_state != 0) ? string.Empty : "tag_locked");
		if (WeaponData.weapon_name == UIShopSceneController.Instance.CurrentAvatar.avatarData.primary_equipment)
		{
			texture = "tag_equipped";
		}
		ExistStateImage.texture = texture;
	}

	public void UpdateEquipState(bool equipped)
	{
		if (equipped)
		{
			if (ExistStateImage.m_texture != "tag_equipped")
			{
				ExistStateImage.texture = "tag_equipped";
			}
		}
		else if (ExistStateImage.m_texture == "tag_equipped")
		{
			ExistStateImage.texture = string.Empty;
		}
	}

	public void LevelUp()
	{
		if (WeaponData.is_secondary)
		{
			Level.Text = "LV" + WeaponData.level.ToString("G");
		}
	}

	private void IsNotNew()
	{
		foreach (UnlockInGame unlock in GameData.Instance.UnlockList)
		{
			if (unlock.Type == UnlockInGame.UnlockType.Weapon && unlock.Name == WeaponData.weapon_name)
			{
				GameData.Instance.UnlockList.Remove(unlock);
				isNew = false;
				break;
			}
		}
	}
}
