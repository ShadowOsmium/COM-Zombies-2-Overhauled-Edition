using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILotterManager : MonoBehaviour
{
    public List<UILotterSeat> lotter_seat_set = new List<UILotterSeat>();

    private int cur_seat_index;
    private int tar_seat_index;
    private float seat_change_interval = 0.05f;
    private bool isSpinning;
    private int spin_cycles;

    private string weapon_combine = string.Empty;
    private bool isFreeSpin;

    private static UILotterManager _instance;
    private GameEnhancer gameEnhancer;

    public static UILotterManager Instance
    {
        get { return _instance; }
    }

    public bool IsSpinning
    {
        get { return isSpinning; }
    }

    void Awake()
    {
        _instance = this;
        gameEnhancer = GameEnhancer.Instance;
    }

    private void Start()
    {
        foreach (var seat in lotter_seat_set)
        {
            seat.SetLightState(false);
        }
        InitLotterSeat();
    }

    private IEnumerator SpinRoutine()
    {
        while (isSpinning)
        {
            cur_seat_index = (cur_seat_index + 1) % lotter_seat_set.Count;
            ResetSeatlightState();
            spin_cycles++;

            if (spin_cycles > lotter_seat_set.Count * 3 && cur_seat_index == tar_seat_index)
            {
                isSpinning = false;
                OnLotteryAward();
                yield break;
            }

            yield return new WaitForSeconds(seat_change_interval);
        }
    }

    public void StartLottery(bool freeSpin)
    {
        if (isSpinning)
        {
            Debug.LogWarning("StartLottery was called while already spinning.");
            return;
        }

        if (freeSpin)
        {
            if (GameData.Instance.free_lottery_spins <= 0)
            {
                Debug.LogWarning("[Lottery] No free spins left, aborting StartLottery.");
                return;
            }

            GameData.Instance.SaveData();

            if (UILotteryController.Instance != null)
            {
                UILotteryController.Instance.UpdateFreeSpinsLabel();
                UILotteryController.Instance.UpdateSpinButtons();
            }
            else
            {
                Debug.LogWarning("[Lottery] Tried to start free spin but no free spins left.");
                return;
            }
        }

        if (gameEnhancer != null)
        {
            gameEnhancer.OnPlayerUsedFreeSpin();
        }

        cur_seat_index = UnityEngine.Random.Range(0, lotter_seat_set.Count);
        tar_seat_index = GetWeightedRandomIndex();
        spin_cycles = 0;
        isSpinning = true;

        if (!GameData.Instance.hasEnteredLottery)
        {
            GameData.Instance.hasEnteredLottery = true;
            GameData.Instance.suspiciousSaveCount = 0;
        }

        GameData.Instance.lottery_count++;
        GameData.Instance.SaveData();

        UILotteryController.Instance.EnableBlock(true);

        // Start spinning coroutine instead of Update loop
        StartCoroutine(SpinRoutine());
    }

    public void ResetSeatLevel(bool is_crystal)
    {
        while (GameData.Instance.lottery_seat_state.Count < lotter_seat_set.Count)
        {
            GameData.Instance.lottery_seat_state.Add("null");
        }
        while (GameData.Instance.lottery_seat_state.Count > lotter_seat_set.Count)
        {
            GameData.Instance.lottery_seat_state.RemoveAt(GameData.Instance.lottery_seat_state.Count - 1);
        }

        int totalSeatsNeeded = 0;
        foreach (int key in GameConfig.Instance.Lottery_Seat_Count_Set.Keys)
        {
            totalSeatsNeeded += GameConfig.Instance.Lottery_Seat_Count_Set[key];
        }
        if (totalSeatsNeeded > lotter_seat_set.Count)
        {
            Debug.LogWarning("Lottery_Seat_Count_Set total (" + totalSeatsNeeded + ") exceeds seats count (" + lotter_seat_set.Count + "). Adjusting...");
            totalSeatsNeeded = lotter_seat_set.Count;
        }

        List<UILotterSeat> list = RandomSortList(lotter_seat_set);

        int num2 = 0;
        foreach (int key in GameConfig.Instance.Lottery_Seat_Count_Set.Keys)
        {
            int countForKey = GameConfig.Instance.Lottery_Seat_Count_Set[key];
            for (int i = 0; i < countForKey && num2 < totalSeatsNeeded; i++)
            {
                list[num2].award_level = key;
                list[num2].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[key];
                num2++;
            }
        }

        List<int> availableLevels = new List<int>(GameConfig.Instance.Lottery_Seat_Count_Set.Keys);
        availableLevels.Sort();
        int leftoverIndex = 0;
        while (num2 < lotter_seat_set.Count)
        {
            int levelToAssign = availableLevels[leftoverIndex % availableLevels.Count];
            list[num2].award_level = levelToAssign;
            list[num2].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[levelToAssign];
            num2++;
            leftoverIndex++;
        }

        int chance = is_crystal ? GameConfig.Instance.lottery_crystal_percent : GameConfig.Instance.lottery_free_percent;
        if (UnityEngine.Random.Range(0, 100) < chance)
        {
            for (int i = 0; i < lotter_seat_set.Count; i++)
            {
                if (lotter_seat_set[i].award_level == 2)
                {
                    lotter_seat_set[i].award_level = 7;
                    lotter_seat_set[i].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[7];
                    break;
                }
            }
        }

        Dictionary<int, List<GameAwardItem>> dictionary = new Dictionary<int, List<GameAwardItem>>();
        for (int level = 1; level < 8; level++)
        {
            dictionary[level] = GameConfig.Instance.GetLotteryAwardWithLevel(level);
        }

        num2 = 0;

        Dictionary<int, HashSet<GameAwardItem.AwardType>> assignedTypesPerLevel = new Dictionary<int, HashSet<GameAwardItem.AwardType>>();
        for (int level = 1; level < 8; level++)
        {
            assignedTypesPerLevel[level] = new HashSet<GameAwardItem.AwardType>();
        }

        foreach (UILotterSeat seat in lotter_seat_set)
        {
            Debug.Log("ResetSeatLevel: seat #" + num2 + " award_level=" + seat.award_level);

            if (!dictionary.ContainsKey(seat.award_level))
            {
                Debug.LogWarning("ResetSeatLevel: dictionary missing award_level " + seat.award_level);
                GameData.Instance.lottery_seat_state[num2] = "null";
                num2++;
                continue;
            }

            List<GameAwardItem> awardList = dictionary[seat.award_level];
            if (awardList == null || awardList.Count == 0)
            {
                Debug.LogWarning("ResetSeatLevel: awardList for level " + seat.award_level + " is empty.");
                GameData.Instance.lottery_seat_state[num2] = "null";
                num2++;
                continue;
            }

            List<GameAwardItem> filteredList = awardList.FindAll(item =>
                !assignedTypesPerLevel[seat.award_level].Contains(item.award_type));

            GameAwardItem randomAward = null;

            if (filteredList.Count > 0)
            {
                randomAward = filteredList[UnityEngine.Random.Range(0, filteredList.Count)];
            }
            else
            {
                randomAward = awardList[UnityEngine.Random.Range(0, awardList.Count)];
            }

            seat.SetLotteryAward(randomAward);
            GameData.Instance.lottery_seat_state[num2] = randomAward.award_name;

            assignedTypesPerLevel[seat.award_level].Add(randomAward.award_type);

            num2++;
        }

        GameData.Instance.lottery_reset_count++;
        Debug.Log("ResetSeatLevel called - lottery_reset_count incremented to: " + GameData.Instance.lottery_reset_count);
        GameData.Instance.SaveData();
    }

    public List<T> RandomSortList<T>(List<T> ListT)
    {
        System.Random random = new System.Random();
        List<T> list = new List<T>();
        foreach (T item in ListT)
        {
            list.Insert(random.Next(list.Count + 1), item);
        }
        return list;
    }

    private int GetWeightedRandomIndex()
    {
        int totalWeight = 0;
        foreach (var seat in lotter_seat_set)
        {
            totalWeight += seat.seat_lottery_weight;
        }

        int rand = UnityEngine.Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < lotter_seat_set.Count; i++)
        {
            cumulative += lotter_seat_set[i].seat_lottery_weight;
            if (rand < cumulative) return i;
        }
        return 0;
    }

    private void ResetSeatlightState()
    {
        for (int i = 0; i < lotter_seat_set.Count; i++)
        {
            lotter_seat_set[i].SetLightState(i == cur_seat_index);
        }
    }

    public void InitLotterSeat()
    {
        for (int i = 0; i < GameData.Instance.lottery_seat_state.Count; i++)
        {
            string id = GameData.Instance.lottery_seat_state[i];
            if (!string.IsNullOrEmpty(id) && GameConfig.Instance.Lottery_AwardItem_Set.ContainsKey(id))
            {
                lotter_seat_set[i].SetLotteryAward(GameConfig.Instance.Lottery_AwardItem_Set[id]);
                int level = GameConfig.Instance.Lottery_AwardItem_Set[id].award_level;
                lotter_seat_set[i].award_level = level;
                lotter_seat_set[i].seat_lottery_weight = GameConfig.Instance.Lottery_Seat_Weight_Set[level];
            }
        }
    }

    public void OnLotteryAward()
    {
        UILotteryController.Instance.SceneAudio.PlayAudio("UI_craft");
        var award = lotter_seat_set[tar_seat_index].lottery_award;
        var awardType = award.award_type;
        bool isDuplicate = false;
        bool isFragmentCombined = false;
        bool showBK = false;
        int cashValue = 0;
        int count = award.award_count;

        if (awardType == GameAwardItem.AwardType.Weapon)
        {
            var weapon = GameData.Instance.WeaponData_Set[award.award_name];
            if (weapon.exist_state == WeaponExistState.Owned) isDuplicate = true;
            else weapon.LotteryReward(true);
            count = 1;
            cashValue = GameConfig.Instance.WeaponConfig_Set[award.award_name].sell_price.GetIntVal();
        }
        else if (awardType == GameAwardItem.AwardType.WeaponFragment)
        {
            var cfg = (WeaponFragmentProbsCfg)GameConfig.Instance.ProbsConfig_Set[award.award_name];
            var state = GameData.Instance.WeaponData_Set[cfg.weapon_name].exist_state;
            if (state != WeaponExistState.Locked) isDuplicate = true;
            else
            {
                if (GameData.Instance.WeaponFragmentProbs_Set.ContainsKey(award.award_name))
                {
                    if (GameData.Instance.WeaponFragmentProbs_Set[award.award_name].count >= 1)
                    {
                        isDuplicate = true;
                    }
                    else
                    {
                        GameData.Instance.WeaponFragmentProbs_Set[award.award_name].count = 1;
                    }
                }
                else
                {
                    GameProb gameProb = new GameProb();
                    gameProb.prob_cfg = GameConfig.Instance.ProbsConfig_Set[award.award_name];
                    gameProb.count = 1;
                    GameData.Instance.WeaponFragmentProbs_Set.Add(award.award_name, gameProb);
                }

                if (!isDuplicate && GameData.Instance.CheckFragmentProbCombine(cfg.weapon_name) && GameData.Instance.WeaponData_Set[cfg.weapon_name].Unlock())
                {
                    isFragmentCombined = true;
                    weapon_combine = cfg.weapon_name;
                }
            }
            count = 1;
            cashValue = cfg.sell_price.GetIntVal();
            showBK = true;
        }
        else
        {
            if (awardType == GameAwardItem.AwardType.Cash)
                GameData.Instance.total_cash.SetIntVal(GameData.Instance.total_cash.GetIntVal() + count, GameDataIntPurpose.Cash);
            else if (awardType == GameAwardItem.AwardType.Crystal)
                GameData.Instance.total_crystal.SetIntVal(GameData.Instance.total_crystal.GetIntVal() + count, GameDataIntPurpose.Crystal);
            else if (awardType == GameAwardItem.AwardType.Voucher)
                GameData.Instance.total_voucher.SetIntVal(GameData.Instance.total_voucher.GetIntVal() + count, GameDataIntPurpose.Voucher);
        }

        if (isDuplicate)
        {
            AwardChangePanel.ShowAwardChangePanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, award.award_name, "Cash_s", cashValue, showBK);
            GameData.Instance.total_cash.SetIntVal(GameData.Instance.total_cash.GetIntVal() + cashValue, GameDataIntPurpose.Cash);
        }
        else if (isFragmentCombined)
        {
            AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnFragmentCombine, award.award_name, 1, showBK);
        }
        else
        {
            AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, award.award_name, count, showBK);
        }

        GameData.Instance.SaveData();
        UILotteryController.Instance.EnableBlock(false);
        UISceneController.Instance.MoneyController.UpdateInfo();
        UILotteryController.Instance.UpdateLotteryBar();
    }

    private void OnAwardOk()
    {
        if (UILotteryController.Instance.lotter_count_award)
        {
            AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, null, "tCrystal_Cent499", GameConfig.Instance.lottery_count_award.GetIntVal());
            GameData.Instance.total_crystal.SetIntVal(GameData.Instance.total_crystal.GetIntVal() + GameConfig.Instance.lottery_count_award.GetIntVal(), GameDataIntPurpose.Crystal);
            UILotteryController.Instance.lotter_count_award = false;
            UISceneController.Instance.MoneyController.UpdateInfo();
        }
        GameData.Instance.rewardSafeMode = false;
    }

    private void OnFragmentCombine()
    {
        AwardGetPanel.ShowAwardGetPanel(UILotteryController.Instance.TUIControls.gameObject, OnAwardOk, weapon_combine, 1);
        GameData.Instance.rewardSafeMode = false;
    }
}
