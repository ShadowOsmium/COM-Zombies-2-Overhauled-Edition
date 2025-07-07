using CoMZ2;
using UnityEngine;

public class DayLevelManager : MonoBehaviour
{
    public static DayLevelManager Instance;

    private float timer = 0f;
    private int interval = 30; // seconds
    private int incrementAmount = 5;
    private bool initialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize day level system for endless mode.
    /// </summary>
    public void Initialize(int startValue, int incrementAmount, int interval)
    {
        GameData.Instance.day_level = startValue;
        this.incrementAmount = incrementAmount;
        this.interval = interval;
        this.timer = 0f;
        this.initialized = true;

        GameSceneController.Instance.UpdateEndlessEnemyStandardStats(GameData.Instance.day_level);
        Debug.Log("[DayLevelManager] Initialized: start=" + startValue + ", inc=" + incrementAmount + ", int=" + interval);
    }

    /// <summary>
    /// Update function to be called by EndlessMissionController.
    /// </summary>
    public void UpdateDayLevel(float deltaTime)
    {
        if (!initialized)
            return;

        if (GameSceneController.Instance == null ||
            GameSceneController.Instance.GamePlayingState != PlayingState.Gaming)
            return;

        if (GameData.Instance.cur_quest_info == null ||
            GameData.Instance.cur_quest_info.mission_day_type != MissionDayType.Endless)
            return;

        timer += deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            GameData.Instance.day_level += incrementAmount;
            GameSceneController.Instance.UpdateEndlessEnemyStandardStats(GameData.Instance.day_level);
            Debug.Log("[DayLevelManager] Day increased to " + GameData.Instance.day_level);
        }
    }

    public void ResetManager()
    {
        initialized = false;
        timer = 0f;
    }
}