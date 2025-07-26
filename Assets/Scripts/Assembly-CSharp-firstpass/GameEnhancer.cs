using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoMZ2;

public class GameEnhancer : MonoBehaviour
{
    private const float MAX_ALLOWED_TIMESCALE = 1f;
    private const float MIN_ALLOWED_TIMESCALE = 0f;
    private const float MAX_SPEED_MULTIPLIER = 1.5f;

    private const float ROLLBACK_TIME_LIMIT = 2f;
    private const int MAX_ROLLBACKS_ALLOWED = 3;

    private const int CASH_SUSPICIOUS_THRESHOLD = 2500000;
    private const int CRYSTAL_SUSPICIOUS_THRESHOLD = 1250;
    private const int VOUCHER_SUSPICIOUS_THRESHOLD = 1650;
    private const int BULLET_SUSPICIOUS_THRESHOLD = 19999;

    private const int CASH_JUMP_THRESHOLD = 800000;
    private const int CRYSTAL_JUMP_THRESHOLD = 125;
    private const int VOUCHER_JUMP_THRESHOLD = 300;

    private const int MAX_REASONABLE_DAY_LEVEL = 9999;
    private const int MAX_DAY_LEVEL_JUMP = 2;

    private const int SUSPICIOUS_COUNT_THRESHOLD = 5;
    private const float SUSPICIOUS_TIME_WINDOW = 5f;

    private const int MAX_FREE_SPINS = 3;
    private const float PLAYER_USAGE_GRACE_PERIOD = 120f;

    private const float TOGGLE_TIME_WINDOW = 5f;
    private const int MAX_TOGGLES_ALLOWED = 5;

    private const int SpeedCheckSampleSize = 8;
    private readonly float[] speedRatios = new float[SpeedCheckSampleSize];
    private int speedRatioIndex = 0;
    private bool speedRatioFilled = false;

    private int speedHackDetectionCount = 0;
    private const int SPEED_HACK_DETECTION_THRESHOLD = 3;
    private float lastSpeedHackDetectionTime = 0f;
    private const float SPEED_HACK_DETECTION_COOLDOWN = 30f;

    private float lastRealTime = 0f;
    private float lastGameTime = 0f;

    private int lastCash = 0;
    private int lastCrystal = 0;
    private int lastVoucher = 0;

    private int suspiciousCashValueCount = 0;
    private float suspiciousCashValueTimer = 0f;
    private int suspiciousCrystalValueCount = 0;
    private float suspiciousCrystalValueTimer = 0f;
    private int suspiciousVoucherValueCount = 0;
    private float suspiciousVoucherValueTimer = 0f;
    private int suspiciousBulletValueCount = 0;
    private float suspiciousBulletValueTimer = 0f;

    private int suspiciousCashDeltaCount = 0;
    private float suspiciousCashDeltaTimer = 0f;
    private int suspiciousCrystalDeltaCount = 0;
    private float suspiciousCrystalDeltaTimer = 0f;
    private int suspiciousVoucherDeltaCount = 0;
    private float suspiciousVoucherDeltaTimer = 0f;

    private int suspiciousBulletCount = 0;
    private float suspiciousBulletTimer = 0f;

    private int lastValidDayLevel = -1;

    private int lastFreeLotterySpins = -1;
    private bool rollbackDetected = false;
    private float rollbackTimer = 0f;
    private int rollbackCount = 0;
    private float lastLegitRollbackTime = 0f;

    private bool playerUsedSpinRecently = false;
    private float lastSpinActivityTime = 0f;

    private bool lastRewardSafeMode = false;
    private float rewardSafeModeWindowStart = 0f;
    private int rewardSafeModeToggleCount = 0;

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
            lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();
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
            lastCash = GameData.Instance.total_cash.GetIntVal();
            lastCrystal = GameData.Instance.total_crystal.GetIntVal();
            lastVoucher = GameData.Instance.total_voucher.GetIntVal();
            lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();
        }

        if (IsRelevantScene(SceneManager.GetActiveScene().name))
        {
            ClampBullets();
        }

        lastRealTime = Time.realtimeSinceStartup;
        lastGameTime = Time.unscaledTime;

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
            lastGameTime = Time.unscaledTime;
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

        CheckSuspiciousValues();
        ClampFreeSpins();
    }

    private void HandleRewardSafeModeToggle()
    {
        if (GameData.Instance == null) return;

        bool current = GameData.Instance.rewardSafeMode;
        if (current != lastRewardSafeMode)
        {
            Debug.LogWarning("[GameEnhancer] rewardSafeMode changed: " + lastRewardSafeMode + " -> " + current + " at time: " + Time.time);

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

    private void CheckSpeedHack()
    {
        if (GameSceneController.Instance == null || GameSceneController.Instance.GamePlayingState != PlayingState.Gaming)
        {
            speedRatioIndex = 0;
            speedRatioFilled = false;
            speedHackDetectionCount = 0; // reset count if not playing
            return;
        }

        float scaledDelta = Time.deltaTime;
        float unscaledDelta = Time.unscaledDeltaTime;

        if (unscaledDelta < 0.0001f) return;

        float ratio = scaledDelta / unscaledDelta;

        speedRatios[speedRatioIndex] = ratio;
        speedRatioIndex = (speedRatioIndex + 1) % SpeedCheckSampleSize;
        if (speedRatioIndex == 0) speedRatioFilled = true;

        if (speedRatioFilled)
        {
            float sum = 0f;
            for (int i = 0; i < SpeedCheckSampleSize; i++) sum += speedRatios[i];
            float averageRatio = sum / SpeedCheckSampleSize;

            if (averageRatio > MAX_SPEED_MULTIPLIER)
            {
                float timeSinceLast = Time.time - lastSpeedHackDetectionTime;
                if (timeSinceLast > SPEED_HACK_DETECTION_COOLDOWN)
                {
                    speedHackDetectionCount = 0;
                }

                speedHackDetectionCount++;
                lastSpeedHackDetectionTime = Time.time;

                Debug.LogWarning(string.Format("[GameEnhancer] Speed hack detected! Count: {0}, Average timescale: {1:F3}", speedHackDetectionCount, averageRatio));

                if (speedHackDetectionCount >= SPEED_HACK_DETECTION_THRESHOLD)
                {
                    BlacklistPlayer("[GameEnhancer] Speed hack detected multiple times, blacklisting player.");
                    speedHackDetectionCount = 0;
                }
            }
            else
            {
                speedHackDetectionCount = 0;
            }
        }
    }

    private void CheckDayLevelCheat()
    {
        if (GameData.Instance == null || GameData.Instance.cur_quest_info == null) return;

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
            BlacklistPlayer(string.Format("[AntiCheat] day_level jump too large: {0} → {1} (+{2})", lastValidDayLevel, current, delta));
        }
        lastValidDayLevel = current;
    }

    private bool skipRollbackCheckAfterLoad = true;
    private float skipRollbackDuration = 5f;
    private float loadTime;

    public void OnLoadComplete()
    {
        skipRollbackCheckAfterLoad = true;
        loadTime = Time.time;
    }

    public void CheckCurrencyRollback()
    {
        if (skipRollbackCheckAfterLoad)
        {
            if (Time.time - loadTime < skipRollbackDuration)
            {
                return;
            }
            else
            {
                skipRollbackCheckAfterLoad = false;
            }
        }

        if (GameData.Instance == null) return;

        int currentCash = GameData.Instance.total_cash.GetIntVal();
        int currentCrystal = GameData.Instance.total_crystal.GetIntVal();
        int currentVoucher = GameData.Instance.total_voucher.GetIntVal();

        Debug.Log("[AntiCheat] Checking rollback:");
        Debug.Log("CurrentCash: " + currentCash + " LastSavedCash: " + GameData.Instance.lastSavedCash);
        Debug.Log("CurrentCrystal: " + currentCrystal + " LastSavedCrystal: " + GameData.Instance.lastSavedCrystal);
        Debug.Log("CurrentVoucher: " + currentVoucher + " LastSavedVoucher: " + GameData.Instance.lastSavedVoucher);

        if (currentCash < GameData.Instance.lastSavedCash)
        {
            Debug.LogWarning("[AntiCheat] Cash rollback detected.");
            BlacklistPlayer("Cash rollback after spending");
        }
        if (currentCrystal < GameData.Instance.lastSavedCrystal)
        {
            Debug.LogWarning("[AntiCheat] Crystal rollback detected.");
            BlacklistPlayer("Crystal rollback after spending");
        }
        if (currentVoucher < GameData.Instance.lastSavedVoucher)
        {
            Debug.LogWarning("[AntiCheat] Voucher rollback detected.");
            BlacklistPlayer("Voucher rollback after spending");
        }
    }

    private void ResetSuspiciousCounters()
    {
        suspiciousCashValueCount = suspiciousCrystalValueCount = suspiciousVoucherValueCount = suspiciousBulletValueCount = 0;
        suspiciousCashValueTimer = suspiciousCrystalValueTimer = suspiciousVoucherValueTimer = suspiciousBulletValueTimer = 0f;

        suspiciousCashDeltaCount = suspiciousCrystalDeltaCount = suspiciousVoucherDeltaCount = 0;
        suspiciousCashDeltaTimer = suspiciousCrystalDeltaTimer = suspiciousVoucherDeltaTimer = 0f;
    }

    private void FreezeCurrencies()
    {
        if (GameData.Instance == null) return;

        GameData.Instance.total_cash.SetIntVal(0, GameDataIntPurpose.Cash);
        GameData.Instance.total_crystal.SetIntVal(0, GameDataIntPurpose.Crystal);
        GameData.Instance.total_voucher.SetIntVal(0, GameDataIntPurpose.Voucher);
    }

    private void ClampFreeSpins()
    {
        if (GameData.Instance == null) return;

        int spins = GameData.Instance.free_lottery_spins.GetIntVal();
        if (spins > MAX_FREE_SPINS)
        {
            Debug.LogWarning("[GameEnhancer] Clamping free spins from " + spins + " to max " + MAX_FREE_SPINS);
            GameData.Instance.free_lottery_spins.SetIntVal(MAX_FREE_SPINS, GameDataIntPurpose.FreeSpin);
        }
    }

    private void UpdateSuspiciousCounter(int value, int threshold, ref int count, ref float timer)
    {
        if (value >= threshold)
        {
            if (count == 0)
                timer = Time.time;

            count++;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsRelevantScene(scene.name))
        {
            ClampBullets();
            if (GameData.Instance != null)
            {
                lastFreeLotterySpins = GameData.Instance.free_lottery_spins.GetIntVal();

                suspiciousCashValueCount = suspiciousCrystalValueCount = suspiciousVoucherValueCount = suspiciousBulletValueCount = 0;
                suspiciousCashValueTimer = suspiciousCrystalValueTimer = suspiciousVoucherValueTimer = suspiciousBulletValueTimer = 0f;

                suspiciousCashDeltaCount = suspiciousCrystalDeltaCount = suspiciousVoucherDeltaCount = 0;
                suspiciousCashDeltaTimer = suspiciousCrystalDeltaTimer = suspiciousVoucherDeltaTimer = 0f;
            }
        }
        ResetRapidSaveDetection();
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

    public void OnSuspiciousDateRollback()
    {
        if (GameData.Instance == null) return;

        DateTime today = DateTime.Today;
        DateTime lastRollback = GameData.Instance.lastRollbackDate.Date;

        TimeSpan timeSinceLast = today - lastRollback;

        if (timeSinceLast.TotalDays > 60)
        {
            Debug.Log("[AntiCheat] Date rollback skipped — player returning after long break.");
            GameData.Instance.lastRollbackDate = today;
            GameData.Instance.SaveData();
            return;
        }

        if (lastRollback != today)
        {
            Debug.LogWarning("[AntiCheat] Detected system clock rollback — warning issued.");
            GameData.Instance.lastRollbackDate = today;
            GameData.Instance.SaveData();
        }
        else
        {
            Debug.LogWarning("[AntiCheat] Multiple rollbacks in same day — blacklisting.");
            BlacklistPlayer("[AntiCheat] Multiple rollbacks in same day.");
        }
    }


    private void ResetRapidSaveDetection()
    {
        GameData.Instance.suspiciousSaveCount = 0;
    }

    private bool CheckAndUpdateSuspiciousCounter(int currentValue, int threshold, ref int count, ref float timer)
    {
        if (currentValue >= threshold)
        {
            if (count == 0)
                timer = Time.time;

            count++;
        }
        else if ((Time.time - timer) > SUSPICIOUS_TIME_WINDOW)
        {
            count = 0;
            timer = 0f;
        }
        else
        {
        }

        return (count >= SUSPICIOUS_COUNT_THRESHOLD) && (Time.time - timer <= SUSPICIOUS_TIME_WINDOW);
    }

    private void ClampBullets()
    {
        if (WeaponData.Instance == null || WeaponData.Instance.playerWeapons == null) return;

        foreach (var weapon in WeaponData.Instance.playerWeapons)
        {
            if (weapon.Value.total_bullet_count > BULLET_SUSPICIOUS_THRESHOLD)
            {
                Debug.LogWarning("[AntiCheat] Clamping bullet count on " + weapon.Key);
                weapon.Value.total_bullet_count = BULLET_SUSPICIOUS_THRESHOLD;
            }
        }
    }

    private void CheckSuspiciousValues()
    {
        if (GameData.Instance == null) return;

        if (GameData.Instance.rewardSafeMode)
        {
            ResetSuspiciousCounters();
            return;
        }

        int currentCash = GameData.Instance.total_cash.GetIntVal();
        int currentCrystal = GameData.Instance.total_crystal.GetIntVal();
        int currentVoucher = GameData.Instance.total_voucher.GetIntVal();

        int deltaCash = Math.Abs(currentCash - lastCash);
        int deltaCrystal = Math.Abs(currentCrystal - lastCrystal);
        int deltaVoucher = Math.Abs(currentVoucher - lastVoucher);

        bool suspiciousValues =
            CheckAndUpdateSuspiciousCounter(currentCash, CASH_SUSPICIOUS_THRESHOLD, ref suspiciousCashValueCount, ref suspiciousCashValueTimer) &&
            CheckAndUpdateSuspiciousCounter(currentCrystal, CRYSTAL_SUSPICIOUS_THRESHOLD, ref suspiciousCrystalValueCount, ref suspiciousCrystalValueTimer) &&
            CheckAndUpdateSuspiciousCounter(currentVoucher, VOUCHER_SUSPICIOUS_THRESHOLD, ref suspiciousVoucherValueCount, ref suspiciousVoucherValueTimer);

        bool suspiciousDeltas =
            CheckAndUpdateSuspiciousCounter(deltaCash, CASH_JUMP_THRESHOLD, ref suspiciousCashDeltaCount, ref suspiciousCashDeltaTimer) &&
            CheckAndUpdateSuspiciousCounter(deltaCrystal, CRYSTAL_JUMP_THRESHOLD, ref suspiciousCrystalDeltaCount, ref suspiciousCrystalDeltaTimer) &&
            CheckAndUpdateSuspiciousCounter(deltaVoucher, VOUCHER_JUMP_THRESHOLD, ref suspiciousVoucherDeltaCount, ref suspiciousVoucherDeltaTimer);

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
                    bulletsSuspicious = true;
                    break;
                }
            }
        }

        if (bulletsSuspicious)
            CheckAndUpdateSuspiciousCounter(1, 0, ref suspiciousBulletValueCount, ref suspiciousBulletValueTimer);
        else
        {
            suspiciousBulletValueCount = 0;
            suspiciousBulletValueTimer = 0f;
        }

        if (suspiciousValues)
            BlacklistPlayer("[GameEnhancer] AntiCheat triggered: Multiple suspicious values detected.");

        if (suspiciousDeltas)
            BlacklistPlayer("[GameEnhancer] AntiCheat triggered: Multiple suspicious deltas detected.");
    }

    private const float SafeSaveCooldown = 1f;

    private float lastSafeSaveTime = -SafeSaveCooldown;

    public bool TryMarkSafeSave()
    {
        if (Time.time - lastSafeSaveTime >= SafeSaveCooldown)
        {
            lastSafeSaveTime = Time.time;
            return true;
        }
        return false;
    }

    public void BlacklistPlayer(string reason)
    {
        if (GameData.Instance == null)
        {
            return;
        }

        if (!GameData.Instance.blackname)
        {
            GameData.Instance.blackname = true;
            FreezeCurrencies();
            ClampBullets();
            GameData.Instance.SaveData();
            Debug.LogWarning(reason + " Player marked blackname.");
            ResetSuspiciousCounters();
        }
        else
        {
            Debug.Log("[Blacklist] Player already blacklisted; ignoring duplicate call.");
        }
    }
}