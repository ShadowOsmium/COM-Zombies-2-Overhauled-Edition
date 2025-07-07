using UnityEngine;

public class GameRewardItem : MonoBehaviour
{
	public TUIMeshSprite reward_frame;

	public TUILabel reward_count;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetReward(string frame, int count, bool show_count = false)
	{
		switch (frame)
		{
		case "jinbi":
		case "daibi":
		case "shuijing":
			reward_frame.transform.localPosition = new Vector3(0f, 5f, -1f);
			break;
		default:
			reward_frame.transform.localPosition = new Vector3(0f, 0f, -1f);
			break;
		}
		reward_frame.texture = frame;
		if (show_count || count > 1)
		{
			reward_count.Text = count.ToString();
		}
		else
		{
			reward_count.Text = string.Empty;
		}
	}
}
