#if UNITY_STANDALONE || UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PC : MonoBehaviour
{
    private GameSceneCoopController coopController;
    private GameSceneController regularController;
    private EndlessMissionController endlessController;

    public bool isPaused = false;
    public static bool ForceUnlockCursor = false;
    private bool can_buy_ammo = false;

    [SerializeField] public GameObject pausePanel;
    [SerializeField] public Button quitButton;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        RefreshControllers();

        UnlockCursor();

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshControllers();
    }

    void RefreshControllers()
    {
        endlessController = FindObjectOfType<EndlessMissionController>();
        coopController = FindObjectOfType<GameSceneCoopController>();
        regularController = coopController != null ? null : FindObjectOfType<GameSceneController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (can_buy_ammo && Input.GetKeyDown(KeyCode.R))
        {
            GameSceneController.Instance.OnAddBulletButton();
        }
    }

    public void PauseGame(bool fromFocusLoss = false)
    {
        if (!IsGamePlaying() || isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (endlessController != null)
        {
            endlessController.PauseEndlessGame();
        }
        else
        {
            GameSceneController baseController = coopController ?? regularController;
            if (baseController != null) baseController.OnGamePause();
        }

        if (pausePanel != null)
            pausePanel.SetActive(true);

        UnlockCursor();
    }

    void UnpauseGame()
    {
        if (!IsGamePlaying() || !isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (endlessController != null)
        {
            endlessController.ResumeEndlessGame();
        }
        else if (coopController != null)
        {
            coopController.OnGameResume(); // ✅ correct for coop
        }
        else if (regularController != null)
        {
            regularController.OnGameResume(); // ✅ normal mode
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsGamePlaying()
    {
        if (coopController != null && coopController.GamePlayingState == CoMZ2.PlayingState.Gaming)
            return true;

        if (regularController != null && regularController.GamePlayingState == CoMZ2.PlayingState.Gaming)
            return true;

        return false;
    }

    public void OnQuitButtonClicked()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        UnlockCursor();

        if (coopController != null)
            coopController.OnGameQuit();
        else if (regularController != null)
            regularController.OnGameQuit();
    }
}

#endif
