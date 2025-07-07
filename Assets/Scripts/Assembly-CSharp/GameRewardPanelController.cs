using System.Collections.Generic;
using UnityEngine;

public class GameRewardPanelController : UIPanelController
{
	public GameObject reward_item;

	public GameObject reward_scoller_root;

	public TUIScrollerEx reward_scoller;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ResetGameReward(List<GameReward> reward_list)
	{
		float num = -90f;
		int num2 = 0;
		foreach (GameReward item in reward_list)
		{
			GameObject gameObject = Object.Instantiate(reward_item) as GameObject;
			if (item.reward_frame == "jinbi" || item.reward_frame == "daibi" || item.reward_frame == "shuijing")
			{
				gameObject.GetComponent<GameRewardItem>().SetReward(item.reward_frame, item.reward_count, true);
			}
			else
			{
				gameObject.GetComponent<GameRewardItem>().SetReward(item.reward_frame, item.reward_count);
			}
			gameObject.transform.parent = reward_scoller_root.transform;
			gameObject.transform.localPosition = new Vector3(num, -8f, -1f);
			num += 45f;
			num2++;
		}
	}
}
