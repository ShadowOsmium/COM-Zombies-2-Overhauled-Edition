using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameRewardCoopPanelController : UIPanelController
{
	public GameObject reward_item_ref;

	public GameObject reward_root;

	public TUIMeshSprite title_bk;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ResetGameReward(List<GameRewardCoop> reward_list)
	{
		float num = 50f;
		foreach (GameRewardCoop item in reward_list)
		{
			Debug.Log(string.Concat("game_reward:", item.avatar_type, " damage:", item.damage, " money:", item.money_count));
			GameObject gameObject = Object.Instantiate(reward_item_ref) as GameObject;
			gameObject.GetComponent<GameRewardCoopItem>().Init(item);
			gameObject.transform.parent = reward_root.transform;
			gameObject.transform.localPosition = new Vector3(0f, num, -3f);
			num -= 45f;
		}
	}
}
