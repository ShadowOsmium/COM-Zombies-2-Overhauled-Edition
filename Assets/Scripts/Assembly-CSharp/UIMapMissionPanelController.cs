using CoMZ2;
using UnityEngine;

public class UIMapMissionPanelController : UIShopPanelController
{
	public TUIMeshSprite SceneImage;

	public TUILabel Description;

	public Transform Reward;

	public TUIMeshSprite weapon_award;

	public QuestInfo QuestInfo { get; set; }

    public override void Show()
    {
        base.Show();
        if (QuestInfo.scene_name.StartsWith("COM2"))
        {
            SceneImage.texture = "panel_scene_COM2";
        }
        else if (QuestInfo.scene_name.StartsWith("Street"))
        {
            SceneImage.texture = "panel_scene_Street";
        }
        else if (QuestInfo.scene_name.StartsWith("Lab"))
        {
            SceneImage.texture = "panel_scene_Lab";
        }
        else
        {
            SceneImage.texture = "panel_scene_" + QuestInfo.scene_name;
        }
        Description.Text = QuestInfo.mission_tag;

        if (QuestInfo.mission_day_type == MissionDayType.Endless)
        {
            Reward.gameObject.SetActive(true);
            CreateMysteryReward(0);

            weapon_award.gameObject.SetActive(false);
            return;
        }

        Reward.gameObject.SetActive(true);

        int num = 0;

        int missionRewardCash = GameData.Instance.GetMissionRewardCash(QuestInfo.mission_type, QuestInfo.mission_day_type, GameData.Instance.is_crazy_daily ? 1 : 2);
        if (missionRewardCash > 0)
        {
            CreateMoneyReward(missionRewardCash, "money_cash", num);
            num++;
        }

        int missionRewardVoucher = GameData.Instance.GetMissionRewardVoucher(QuestInfo.mission_type, QuestInfo.mission_day_type, GameData.Instance.is_crazy_daily ? 1 : 2);
        if (missionRewardVoucher > 0)
        {
            CreateMoneyReward(missionRewardVoucher, "money_voucher", num);
            num++;
        }

        int missionRewardCrystal = GameData.Instance.GetMissionRewardCrystal(QuestInfo.mission_type, QuestInfo.mission_day_type);
        if (missionRewardCrystal > 0)
        {
            CreateMoneyReward(missionRewardCrystal, "money_crystal", num);
            num++;
        }

        if (QuestInfo.reward_prob != string.Empty)
        {
            string[] array = QuestInfo.reward_prob.Split('|');
            foreach (string image in array)
            {
                CreateDebrisReward(image, num);
                num++;
            }
        }

        if (QuestInfo.avatar != AvatarType.None)
        {
            CreateAvatarReward(QuestInfo.avatar.ToString(), num);
        }

        if (QuestInfo.reward_weapon != string.Empty)
        {
            weapon_award.gameObject.SetActive(true);
            weapon_award.texture = "Gameui_" + QuestInfo.reward_weapon;
        }
        else
        {
            weapon_award.gameObject.SetActive(false);
        }
    }

    public override void Hide(bool isPopFromStack)
	{
		base.Hide(isPopFromStack);
		for (int i = 0; i < Reward.childCount; i++)
		{
			Object.Destroy(Reward.GetChild(i).gameObject);
		}
	}

	private void CreateMoneyReward(int amount, string image, int index)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_money")) as GameObject;
		gameObject.transform.parent = Reward;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
		gameObject.transform.Find("money").GetComponent<TUIMeshSprite>().texture = image;
		gameObject.transform.Find("value").GetComponent<TUILabel>().Text = amount.ToString("G");
	}

	private void CreateDebrisReward(string image, int index)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
		gameObject.transform.parent = Reward;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
		gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = image;
	}

	private void CreateAvatarReward(string image, int index)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
		gameObject.transform.parent = Reward;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
		gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = "reward_" + image;
	}

    private void CreateMysteryReward(int index)
    {
        GameObject gameObject = Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
        gameObject.transform.parent = Reward;
        gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
        gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = "reward_unknown";
    }

    private void OnStartGameEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
    {
        if (eventType == 3)
        {
            UISceneController.Instance.SceneAudio.PlayAudio("UI_click");

            if (QuestInfo.mission_type == MissionType.Endless && GameData.Instance.day_level < 30)
            {
                GameMsgBoxController.ShowMsgBox(
                    GameMsgBoxController.MsgBoxType.SingleButton,
                    gameObject,
                    "Reach Day 30 to unlock Endless Mode!",
                    () => GameMsgBoxController.Instance.Hide(),
                    null,
                    true
                );
                return;
            }

            MenuAudioController.DestroyGameMenuAudio();
            GameData.Instance.MapSceneQuestInfoWrite(QuestInfo);
            Application.LoadLevel("Loading");
        }
    }
}
