using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoMZ2; // For PlayingState enum

public class GameEnhancer : MonoBehaviour
{
    private const float MAX_ALLOWED_TIMESCALE = 1f;
    private const float MIN_ALLOWED_TIMESCALE = 0f;

    private const float SPEED_CHECK_INTERVAL = 1f;
    private const float MAX_SPEED_MULTIPLIER = 1.5f;

    private const float REWARD_SAFE_MODE_COOLDOWN = 3f;

    private const int CASH_SUSPICIOUS_THRESHOLD = 2500000;
    private const int CRYSTAL_SUSPICIOUS_THRESHOLD = 1250;
    private const int VOUCHER_SUSPICIOUS_THRESHOLD = 1650;
    private const int BULLET_SUSPICIOUS_THRESHOLD = 19999;

    private const int CASH_JUMP_THRESHOLD = 800000;
    private const int CRYSTAL_JUMP_THRESHOLD = 125;
    private const int VOUCHER_JUMP_THRESHOLD = 300;

    private const int MAX_REASONABLE_DAY_LEVEL = 9999;
    private const int MAX_DAY_LEVEL_JUMP = 2;

    private const int MAX_ROLLBACKS_ALLOWED = 3;
    private const float ROLLBACK_TIME_LIMIT = 2f;

    private const int SUSPICIOUS_COUNT_THRESHOLD = 5;
    private const float SUSPICIOUS_TIME_WINDOW = 5f;

    private const int MAX_FREE_SPINS = 5;
    private const float PLAYER_USAGE_GRACE_PERIOD = 120f;

    private float elapsedSpeedCheckTime = 0f;
    private float lastRealTime = 0f;
    private float lastGameTime = 0f;

    private int lastCash = 0;
    private int lastCrystal = 0;
    private int lastVoucher = 0;

    private int suspiciousCashCount = 0;
    private float suspiciousCashTimer = 0f;
    private int suspiciousCrystalCount = 0;
    private float suspiciousCrystalTimer = 0f;
    private int suspiciousVoucherCount = 0;
    private float suspiciousVoucherTimer = 0f;
    private int suspiciousBulletCount = 0;
    private float suspiciousBulletTimer = 0f;

    private int lastValidDayLevel = -1;

    private int lastFreeLotterySpins = -1;
    private int suspiciousFreeSpinCount = 0;
    private float suspiciousFreeSpinTimer = 0f;

    private bool rollbackDetected = false;
    private float rollbackTimer = 0f;
    private int rollbackCount = 0;
    private float lastLegitRollbackTime = 0f;

    private bool playerUsedSpinRecently = false;
    private float lastSpinActivityTime = 0f;

    private int rewardSafeModeToggleCount = 0;
    private float rewardSafeModeWindowStart = 0f;
    private bool lastRewardSafeMode = false;
    private const float TOGGLE_TIME_WINDOW = 5f;
    private const int MAX_TOGGLES_ALLOWED = 5;

    private static GameEnhancer _instance;
    public static GameEnhancer Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (Application.isPlaying && GameData.Instance != null)
        {
            lastCash = GameData.Instance.total_cash.GetIntVal();
            lastCrystal = GameData.Instance.total_crystal.GetIntVal();
            lastVoucher = GameData.Instance.total_voucher.GetIntVal();
        }
    }

    void OnEnable()
    {
        if (!Application.isPlaying) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (!Application.isPlaying) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (GameData.Instance != null)
        {
            lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();
            lastCash = GameData.Instance.total_cash.GetIntVal();
            lastCrystal = GameData.Instance.total_crystal.GetIntVal();
            lastVoucher = GameData.Instance.total_voucher.GetIntVal();
        }

        if (IsRelevantScene(SceneManager.GetActiveScene().name))
        {
            ClampBullets();
        }

        lastRealTime = Time.realtimeSinceStartup;
        lastGameTime = Time.time;

        StartCoroutine(ClampBulletsLoop());
        StartCoroutine(ClampTimeScaleLoop());
    }

    void Update()
    {
        if (!Application.isPlaying) return;

        string scene = SceneManager.GetActiveScene().name;
        if (scene == "InitScene" || scene == "Loading") return;
        if (UILotterManager.Instance != null && UILotterManager.Instance.IsSpinning) return;

        HandleRewardSafeModeToggle();
        if (lastRealTime == 0f)
        {
            lastRealTime = Time.realtimeSinceStartup;
            lastGameTime = Time.time;
            return;
        }

        CheckSpeedHack();
        CheckDayLevelCheat();

        if (GameData.Instance.blackname)
        {
            FreezeCurrencies();
            ClampBullets();
            return;
        }

        CheckFreeSpinOscillation();
        CheckSuspiciousFreeSpins();
        CheckSuspiciousValues();
        ClampFreeSpins();
    }

    private void HandleRewardSafeModeToggle()
    {
        bool current = GameData.Instance.rewardSafeMode;
        if (current != lastRewardSafeMode)
        {
            Debug.LogWarning("[Debug] rewardSafeMode changed: " + lastRewardSafeMode + " -> " + current + " at time: " + Time.time);

            if (Time.time - rewardSafeModeWindowStart > TOGGLE_TIME_WINDOW)
            {
                rewardSafeModeWindowStart = Time.time;
                rewardSafeModeToggleCount = 1;
            }
            else
            {
                rewardSafeModeToggleCount++;
                if (rewardSafeModeToggleCount > MAX_TOGGLES_ALLOWED)
                {
                    Debug.LogWarning("[GameEnhancer] rewardSafeMode toggled too frequently, possible cheat.");
                    ResetSuspiciousCounters();
                }
            }

            lastRewardSafeMode = current;
        }
    }

    public void CheckFreeSpinUsage()
    {
        if (GameData.Instance == null) return;

        int freeSpins = GameData.Instance.free_lottery_spins.GetIntVal();
        if (freeSpins < 0 || freeSpins > MAX_FREE_SPINS)
        {
            Debug.LogWarning("[GameEnhancer] Suspicious free spin count detected: " + freeSpins);
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
        }
    }

    private void CheckSpeedHack()
    {
        if (GameSceneController.Instance == null || GameSceneController.Instance.GamePlayingState != PlayingState.Gaming)
        {
            lastRealTime = Time.realtimeSinceStartup;
            lastGameTime = Time.time;
            elapsedSpeedCheckTime = 0f;
            return;
        }

        float currentRealTime = Time.realtimeSinceStartup;
        float currentGameTime = Time.time;

        float realDelta = currentRealTime - lastRealTime;
        float gameDelta = currentGameTime - lastGameTime;

        elapsedSpeedCheckTime += realDelta;

        if (elapsedSpeedCheckTime >= SPEED_CHECK_INTERVAL)
        {
            float multiplier = gameDelta / realDelta;
            if (multiplier > MAX_SPEED_MULTIPLIER)
            {
                Debug.LogWarning("[GameEnhancer] Speed hack detected! Multiplier: " + multiplier);
                if (!GameData.Instance.blackname)
                {
                    GameData.Instance.blackname = true;
                    FreezeCurrencies();
                    ClampBullets();
                    GameData.Instance.SaveData();
                    Debug.LogWarning("[GameEnhancer] Player marked blackname due to speed hack.");
                }
            }
            elapsedSpeedCheckTime = 0f;
        }

        lastRealTime = currentRealTime;
        lastGameTime = currentGameTime;
    }

    private void CheckDayLevelCheat()
    {
        if (GameData.Instance == null || GameData.Instance.cur_quest_info == null)
            return;

        if (GameData.Instance.cur_quest_info.mission_day_type == MissionDayType.Endless ||
            GameData.Instance.cur_quest_info.mission_type == MissionType.Endless)
            return;

        int current = GameData.Instance.day_level;
        if (current > MAX_REASONABLE_DAY_LEVEL)
        {
            Debug.LogWarning("[AntiCheat] day_level exceeds max allowed: " + current);
            GameData.Instance.suspiciousSaveCount++;
            GameData.Instance.day_level = MAX_REASONABLE_DAY_LEVEL;
            return;
        }

        if (lastValidDayLevel == -1)
        {
            lastValidDayLevel = current;
            return;
        }

        int delta = current - lastValidDayLevel;
        if (delta > MAX_DAY_LEVEL_JUMP)
        {
            Debug.LogWarning("[AntiCheat] day_level jump too large: " + lastValidDayLevel + " → " + current + " (+" + delta + ")");
            GameData.Instance.suspiciousSaveCount++;
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
        }
        lastValidDayLevel = current;
    }

    private void CheckSuspiciousValues()
    {
        if (GameData.Instance.rewardSafeMode)
        {
            ResetSuspiciousCounters();
            return;
        }

        int currentCash = GameData.Instance.total_cash.GetIntVal();
        int currentCrystal = GameData.Instance.total_crystal.GetIntVal();
        int currentVoucher = GameData.Instance.total_voucher.GetIntVal();

        UpdateSuspiciousCounter(currentCash, CASH_SUSPICIOUS_THRESHOLD, ref suspiciousCashCount, ref suspiciousCashTimer);
        UpdateSuspiciousCounter(currentCrystal, CRYSTAL_SUSPICIOUS_THRESHOLD, ref suspiciousCrystalCount, ref suspiciousCrystalTimer);
        UpdateSuspiciousCounter(currentVoucher, VOUCHER_SUSPICIOUS_THRESHOLD, ref suspiciousVoucherCount, ref suspiciousVoucherTimer);

        UpdateSuspiciousCounter(Math.Abs(currentCash - lastCash), CASH_JUMP_THRESHOLD, ref suspiciousCashCount, ref suspiciousCashTimer);
        UpdateSuspiciousCounter(Math.Abs(currentCrystal - lastCrystal), CRYSTAL_JUMP_THRESHOLD, ref suspiciousCrystalCount, ref suspiciousCrystalTimer);
        UpdateSuspiciousCounter(Math.Abs(currentVoucher - lastVoucher), VOUCHER_JUMP_THRESHOLD, ref suspiciousVoucherCount, ref suspiciousVoucherTimer);

        lastCash = currentCash;
        lastCrystal = currentCrystal;
        lastVoucher = currentVoucher;

        bool bulletsSuspicious = false;
        if (WeaponData.Instance != null && WeaponData.Instance.playerWeapons != null)
        {
            foreach (var w in WeaponData.Instance.playerWeapons)
            {
                if (w.Value.total_bullet_count > BULLET_SUSPICIOUS_THRESHOLD)
                {
                    Debug.LogWarning("[AntiCheat] Clamped suspicious bullet count on " + w.Key);
                    w.Value.total_bullet_count = 0;
                    bulletsSuspicious = true;
                    break;
                }
            }
        }

        if (bulletsSuspicious)
            UpdateSuspiciousCounter(1, 0, ref suspiciousBulletCount, ref suspiciousBulletTimer);
        else
        {
            suspiciousBulletCount = 0;
            suspiciousBulletTimer = 0f;
        }

        if (IsSuspicious(ref suspiciousCashCount, ref suspiciousCashTimer) &&
            IsSuspicious(ref suspiciousCrystalCount, ref suspiciousCrystalTimer) &&
            IsSuspicious(ref suspiciousVoucherCount, ref suspiciousVoucherTimer) &&
            IsSuspicious(ref suspiciousBulletCount, ref suspiciousBulletTimer))
        {
            Debug.LogWarning("[GameEnhancer] AntiCheat triggered: Multiple suspicious values detected.");
            FreezeCurrencies();
            ClampBullets();

            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();

            ResetSuspiciousCounters();
        }
    }

    private void CheckSuspiciousFreeSpins()
    {
        if (GameData.Instance.rewardSafeMode)
        {
            suspiciousFreeSpinCount = 0;
            suspiciousFreeSpinTimer = 0f;
            return;
        }

        int currentSpins = GameData.Instance.free_lottery_spins.GetIntVal();

        if (currentSpins < 0 || currentSpins > MAX_FREE_SPINS)
        {
            Debug.LogWarning("[AntiCheat] Free spins out of allowed range: " + currentSpins);
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
            return;
        }

        int delta = currentSpins - lastFreeLotterySpins;
        if (delta > 3)
        {
            if (playerUsedSpinRecently && Time.time - lastSpinActivityTime <= PLAYER_USAGE_GRACE_PERIOD)
            {
                Debug.Log("[AntiCheat] Skipping suspicious spin delta check due to recent player use.");
            }
            else
            {
                suspiciousFreeSpinCount++;
                if (suspiciousFreeSpinTimer == 0f)
                    suspiciousFreeSpinTimer = Time.time;
            }
        }

        if (suspiciousFreeSpinCount >= 3 && Time.time - suspiciousFreeSpinTimer <= 5f)
        {
            Debug.LogWarning("[AntiCheat] Rapid suspicious increase in free spins.");
            GameData.Instance.blackname = true;
            GameData.Instance.SaveData();
            suspiciousFreeSpinCount = 0;
            suspiciousFreeSpinTimer = 0f;
        }

        lastFreeLotterySpins = currentSpins;
    }

    private void CheckFreeSpinOscillation()
    {
        if (GameData.Instance.rewardSafeMode)
        {
            rollbackDetected = false;
            rollbackCount = 0;
            rollbackTimer = 0f;
            lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();
            return;
        }

        int currentFreeSpins = GameData.Instance.free_lottery_spins.GetIntVal();

        if (lastFreeLotterySpins == -1)
        {
            lastFreeLotterySpins = currentFreeSpins;
            return;
        }

        if (currentFreeSpins > lastFreeLotterySpins)
        {
            if (playerUsedSpinRecently && Time.time - lastSpinActivityTime <= PLAYER_USAGE_GRACE_PERIOD)
            {
                Debug.Log("[GameEnhancer] Legitimate spin gain detected after player activity.");
            }
            else
            {
                rollbackDetected = true;
                rollbackTimer = 0f;
                rollbackCount++;
                Debug.LogWarning("[CheckFreeSpinOscillation] rollbackCount=" + rollbackCount + " after " + lastFreeLotterySpins + " → " + currentFreeSpins);
            }
        }

        if (rollbackDetected)
        {
            if (Time.time - lastLegitRollbackTime > 120f)
            {
                lastLegitRollbackTime = Time.time;
                rollbackDetected = false;
                rollbackCount = 0;
                rollbackTimer = 0f;
                Debug.Log("[GameEnhancer] Allowed one-time rollback due to possible restart.");
                return;
            }

            rollbackTimer += Time.deltaTime;
            if (rollbackTimer > ROLLBACK_TIME_LIMIT)
            {
                rollbackDetected = false;
                rollbackCount = 0;
                rollbackTimer = 0f;
            }
            else if (rollbackCount >= MAX_ROLLBACKS_ALLOWED)
            {
                Debug.LogWarning("[GameEnhancer] Free spin rollback threshold exceeded. Marking blackname.");
                GameData.Instance.blackname = true;
                GameData.Instance.SaveData();
                rollbackDetected = false;
                rollbackCount = 0;
                rollbackTimer = 0f;
            }
        }

        lastFreeLotterySpins = currentFreeSpins;
    }

    public void OnPlayerUsedFreeSpin()
    {
        playerUsedSpinRecently = true;
        lastSpinActivityTime = Time.time;
        lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();

        Debug.Log("[GameEnhancer] Player used a free spin. Freeze detection reset.");
    }

    private void ResetSuspiciousCounters()
    {
        suspiciousCashCount = suspiciousCrystalCount = suspiciousVoucherCount = suspiciousBulletCount = 0;
        suspiciousCashTimer = suspiciousCrystalTimer = suspiciousVoucherTimer = suspiciousBulletTimer = 0f;
    }

    private void FreezeCurrencies()
    {
        GameData.Instance.total_cash.SetIntVal(0, GameDataIntPurpose.Cash);
        GameData.Instance.total_crystal.SetIntVal(0, GameDataIntPurpose.Crystal);
        GameData.Instance.total_voucher.SetIntVal(0, GameDataIntPurpose.Voucher);
    }

    private void ClampBullets()
    {
        if (WeaponData.Instance == null || WeaponData.Instance.playerWeapons == null) return;

        foreach (var weapon in WeaponData.Instance.playerWeapons)
        {
            if (weapon.Value.total_bullet_count > BULLET_SUSPICIOUS_THRESHOLD)
            {
                Debug.LogWarning("[AntiCheat] Clamped bullet count on " + weapon.Key);
                weapon.Value.total_bullet_count = 0;
            }
        }
    }

    private void ClampFreeSpins()
    {
        int spins = GameData.Instance.free_lottery_spins.GetIntVal();
        if (spins > MAX_FREE_SPINS)
        {
            Debug.LogWarning("[GameEnhancer] Clamping free spins from " + spins + " to max " + MAX_FREE_SPINS);
            GameData.Instance.free_lottery_spins.SetIntVal(MAX_FREE_SPINS, GameDataIntPurpose.FreeSpin);
        }
    }

    private void UpdateSuspiciousCounter(int value, int threshold, ref int count, ref float timer)
    {
        if (value > threshold)
        {
            count++;
            if (timer == 0f)
                timer = Time.time;
        }
        else
        {
            count = 0;
            timer = 0f;
        }
    }

    private bool IsSuspicious(ref int count, ref float timer)
    {
        if (count >= SUSPICIOUS_COUNT_THRESHOLD && (Time.time - timer) <= SUSPICIOUS_TIME_WINDOW)
            return true;

        if ((Time.time - timer) > SUSPICIOUS_TIME_WINDOW)
        {
            count = 0;
            timer = 0f;
        }
        return false;
    }

    private bool IsRelevantScene(string sceneName)
    {
        return sceneName == "UIMap" || sceneName == "UIShop" || sceneName == "UILottery";
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsRelevantScene(scene.name))
        {
            ClampBullets();
            lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();
        }
    }

    private IEnumerator ClampBulletsLoop()
    {
        while (true)
        {
            ClampBullets();
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator ClampTimeScaleLoop()
    {
        while (true)
        {
            float timeScale = Time.timeScale;
            if (timeScale > MAX_ALLOWED_TIMESCALE || timeScale < MIN_ALLOWED_TIMESCALE)
            {
                Debug.LogWarning("[GameEnhancer] Time.timeScale out of range: " + timeScale + ", resetting to 1.0");
                Time.timeScale = 1f;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
