using System;
using UnityEngine;

public class UIMapMissionDailyPanelController : UIShopPanelController
{
	public TUILabel Description;

	public Transform Reward;

	public TUIButtonSelect DailyEasyMode;

	public TUIButtonSelect DailyHardMode;

	public GameObject DailyUnlock;

	public TUIButtonClick CDTimerButton;

	public TUIMeshSprite UnlockCrystal;

	public TUILabel UnlockValue;

	public TUIButtonClick Start;

	private GameMsgBoxController msgBox;

	private int missionPrice;

	private int cdPrice;

	private double curCDTimer;

	private double preCDTimer;

	private bool isUpdateTimer;

	public QuestInfo QuestInfo { get; set; }

	private void Update()
	{
		if (!isUpdateTimer)
		{
			return;
		}
		if (curCDTimer > 0.0)
		{
			curCDTimer -= Time.deltaTime;
			if (preCDTimer - curCDTimer >= 1.0)
			{
				preCDTimer = curCDTimer;
				UpdateCDTimer();
			}
		}
		else
		{
			Hide(true);
			UIMapSceneController.Instance.ConnectServer();
			isUpdateTimer = false;
		}
	}

	public override void Show()
	{
		base.Show();
		DailyEasyMode.SetSelected(!GameData.Instance.is_crazy_daily);
		UpdateMissionInfo();
		UpdateButtonInfo();
	}

    private void UpdateMissionInfo()
    {
        for (int i = 0; i < Reward.childCount; i++)
        {
            UnityEngine.Object.Destroy(Reward.GetChild(i).gameObject);
        }

        int num = 0;

        int missionRewardCash = GameData.Instance.GetMissionRewardCash(QuestInfo.mission_type, QuestInfo.mission_day_type, (!GameData.Instance.is_crazy_daily) ? 1 : 2);
        if (missionRewardCash > 0)
        {
            CreateMoneyReward(missionRewardCash, "money_cash", num);
            num++;
        }

        int missionRewardVoucher = GameData.Instance.GetMissionRewardVoucher(QuestInfo.mission_type, QuestInfo.mission_day_type, (!GameData.Instance.is_crazy_daily) ? 1 : 2);
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
            foreach (string text in array)
            {
                Debug.Log(text);
                CreateDebrisReward(text, num);
                num++;
            }
        }

        // Mission description
        Description.Text = (!GameData.Instance.is_crazy_daily)
            ? "Lock and load.. It's time for epic zombie-slaying action!"
            : "Get 5x rewards while difficulty level is quadrupled.";
    }


    private void UpdateButtonInfo()
	{
		switch (GameData.Instance.EnableDailyMission())
		{
		case GameData.DailyMissionStatus.Free:
			DailyUnlock.SetActive(false);
			Start.gameObject.SetActive(true);
			break;
		case GameData.DailyMissionStatus.CrystalEnable:
			DailyUnlock.SetActive(true);
			Start.gameObject.SetActive(false);
			CDTimerButton.Disable(false);
			curCDTimer = (GameData.Instance.next_cd_date - GameData.Instance.last_checked_date_now).TotalSeconds;
			preCDTimer = curCDTimer;
			isUpdateTimer = true;
			UpdateCDTimer();
			cdPrice = GameData.Instance.GetDailyCDPrice();
			if (GameData.Instance.is_crazy_daily)
			{
				cdPrice += GameConfig.Instance.daily_price_hard2;
			}
			UnlockCrystal.gameObject.SetActive(true);
			UnlockValue.gameObject.SetActive(true);
			UnlockValue.Text = cdPrice.ToString("G");
			break;
		case GameData.DailyMissionStatus.Disenable:
		case GameData.DailyMissionStatus.CrystalDisenable:
			DailyUnlock.SetActive(true);
			Start.gameObject.SetActive(false);
			CDTimerButton.Disable(true);
			curCDTimer = (GameData.Instance.next_cd_date - GameData.Instance.last_checked_date_now).TotalSeconds;
			preCDTimer = curCDTimer;
			isUpdateTimer = true;
			UpdateCDTimer();
			UnlockCrystal.gameObject.SetActive(false);
			UnlockValue.gameObject.SetActive(false);
			break;
		}
	}

	private void UpdateCDTimer()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((int)curCDTimer);
		CDTimerButton.m_NormalLabelObj.GetComponent<TUILabel>().Text = timeSpan.ToString();
		CDTimerButton.m_PressLabelObj.GetComponent<TUILabel>().Text = timeSpan.ToString();
		CDTimerButton.m_DisableLabelObj.GetComponent<TUILabel>().Text = timeSpan.ToString();
	}

	private void CreateMoneyReward(int amount, string image, int index)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/reward_money")) as GameObject;
		gameObject.transform.parent = Reward;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
		gameObject.transform.Find("money").GetComponent<TUIMeshSprite>().texture = image;
		gameObject.transform.Find("value").GetComponent<TUILabel>().Text = amount.ToString("G");
	}

	private void CreateDebrisReward(string image, int index)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/reward_debris")) as GameObject;
		gameObject.transform.parent = Reward;
		gameObject.transform.localPosition = new Vector3(-90 + index * 45, -86f, 0f);
		gameObject.transform.Find("image").GetComponent<TUIMeshSprite>().texture = image;
	}

	private void OnStartGameEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			HandleStartButton();
		}
	}

	private void OnUnlockMissionEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 3)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			string content = "Spend " + cdPrice + " tCrystals to skip cooldown and proceed with this mission?";
			msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.DoubleButton, base.transform.gameObject, content, OnUnlockMission, OnMsgboxCancel);
		}
	}

	private void OnSelectEasyEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 1)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			GameData.Instance.is_crazy_daily = false;
			UpdateMissionInfo();
			UpdateButtonInfo();
		}
	}

	private void OnSelectHardEvent(TUIControl control, int eventType, float lparam, float wparam, object data)
	{
		if (eventType == 1)
		{
			UISceneController.Instance.SceneAudio.PlayAudio("UI_click");
			GameData.Instance.is_crazy_daily = true;
			UpdateMissionInfo();
			UpdateButtonInfo();
		}
	}

	public void HandleStartButton()
	{
		if (GameData.Instance.is_crazy_daily)
		{
			missionPrice = GameData.Instance.GetDailyMissionPrice(GameData.Instance.is_crazy_daily);
			string content = "Spend " + missionPrice + " tCrystals to proceed with this challenge?";
			msgBox = GameMsgBoxController.ShowMsgBox(GameMsgBoxController.MsgBoxType.DoubleButton, base.transform.gameObject, content, OnPurchaseMission, OnMsgboxCancel);
		}
		else
		{
			MenuAudioController.DestroyGameMenuAudio();
			GameData.Instance.MapSceneQuestInfoWrite(QuestInfo);
			Application.LoadLevel("Loading");
		}
	}

	private void OnPurchaseMission()
	{
		UnityEngine.Object.Destroy(msgBox.gameObject);
		if (GameData.Instance.total_crystal >= missionPrice)
		{
			MenuAudioController.DestroyGameMenuAudio();
			GameData.Instance.MapSceneQuestInfoWrite(QuestInfo);
			Application.LoadLevel("Loading");
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
		else
		{
			//UIMapSceneController.Instance.MoneyController.IapPanel.Show();
		}
	}

	private void OnUnlockMission()
	{
		UnityEngine.Object.Destroy(msgBox.gameObject);
		if (GameData.Instance.total_crystal >= cdPrice)
		{
			GameData.Instance.ResetDailyCD();
			MenuAudioController.DestroyGameMenuAudio();
			GameData.Instance.MapSceneQuestInfoWrite(QuestInfo);
			Application.LoadLevel("Loading");
			UISceneController.Instance.MoneyController.UpdateInfo();
		}
		else
		{
			//UIMapSceneController.Instance.MoneyController.IapPanel.Show();
		}
	}

	private void OnMsgboxCancel()
	{
		UnityEngine.Object.Destroy(msgBox.gameObject);
	}
}
