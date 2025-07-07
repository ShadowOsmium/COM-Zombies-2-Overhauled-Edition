#if UNITY_STANDALONE || UNITY_EDITOR

using System;
using System.Collections;
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

        coopController = FindObjectOfType<GameSceneCoopController>();
        regularController = FindObjectOfType<GameSceneController>();

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        endlessController = FindObjectOfType<EndlessMissionController>();
        coopController = FindObjectOfType<GameSceneCoopController>();
        regularController = FindObjectOfType<GameSceneController>();
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
        else if (regularController != null)
        {
            regularController.OnGamePause();
        }
        else if (coopController != null)
        {
            coopController.OnGamePause();
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
        else if (regularController != null)
        {
            regularController.OnGameResume();
        }
        else if (coopController != null)
        {
            coopController.OnGameResume();
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
        {
            pausePanel.SetActive(false);
        }

        UnlockCursor();

        if (regularController != null)
        {
            regularController.OnGameQuit();
        }
        else if (coopController != null)
        {
            coopController.OnGameQuit();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

#endif