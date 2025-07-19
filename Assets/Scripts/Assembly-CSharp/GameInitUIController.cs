using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using CoMZ2;
using System;

public class GameInitUIController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public RenderTexture renderTexture;
    public string defaultNextScene = "GameCover";

    public VideoClip promotionClip;
    public VideoClip introClip;

    private bool videoSkippedOrEnded = false;
    private bool videoPausedDueToFocusLoss = false;

    private int currentVideoIndex = 0; // 0 = promotion, 1 = intro

    private Coroutine checkVideoCoroutine;
    private Coroutine forceSkipCoroutine;
    public TesterSaveManager testerSaveManager;

    private void Awake()
    {
        GameConfig.CheckGameConfig();
        GameData.CheckGameData();
    }

    private IEnumerator Start()
    {
        GameConfig.CheckGameConfig();

        bool loaded = GameData.Instance.LoadData();
        Debug.Log("Save loaded: " + loaded);
        Debug.Log("Loaded game version: " + GameData.Instance.game_version);
        Debug.Log("Current app version: " + Application.version);

        if (!loaded)
        {
            Debug.LogWarning("Save failed to load. Initializing new save.");
            GameData.Instance.Init();
            GameData.Instance.game_version = Application.version;
            GameData.Instance.needsUpdate = false;
            GameData.Instance.SaveData(); // Save your fresh init
        }
        else if (GameData.Instance.game_version != Application.version)
        {
            Debug.LogWarning("Save version mismatch. Patching save for new version.");
            GameData.Instance.needsUpdate = true;
            GameData.Instance.game_version = Application.version;
            GameData.Instance.SaveData(); // Patch save to current version
        }

        if (GameData.Instance != null)
        {
            GameData.Instance.lastLoginDate = DateTime.Today;
            GameData.Instance.SaveData();
            Debug.Log("[Init] Updated lastLoginDate to: " + GameData.Instance.lastLoginDate);
        }

        OpenClikPlugin.Initialize("A36F6C65-C1E3-47D4-AD07-AA8A6C90132C");

        // This method only exists on real Android devices (not Editor or PC builds)
#if UNITY_ANDROID && !UNITY_EDITOR
        // Android native player path
        yield return PlayAndroidVideosSequentially();
#else
        // VideoPlayer path
        if (videoPlayer == null || videoImage == null || renderTexture == null)
        {
            Debug.LogError("Missing components: VideoPlayer, RawImage, or RenderTexture.");
            yield break;
        }

        videoPlayer.targetTexture = renderTexture;
        videoImage.texture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        videoPlayer.SetTargetAudioSource(0, audioSource);

        yield return PlayVideoClip(promotionClip);
#endif
    }

    // This method only exists on real Android devices (not Editor or PC builds)
#if UNITY_ANDROID && !UNITY_EDITOR
private IEnumerator PlayAndroidVideosSequentially()
{
    string promotionPath = System.IO.Path.Combine(Application.streamingAssetsPath, "InfinityPromotion.mp4");
    string introPath = System.IO.Path.Combine(Application.streamingAssetsPath, "GameStory.mp4");

    yield return new WaitForEndOfFrame(); // prevent black screen on some devices

    yield return Handheld.PlayFullScreenMovie(promotionPath, Color.black, FullScreenMovieControlMode.CancelOnInput);
    yield return Handheld.PlayFullScreenMovie(introPath, Color.black, FullScreenMovieControlMode.CancelOnInput);

    LoadNextScene();
}
#endif


    private IEnumerator PlayVideoClip(VideoClip clip)
    {
        videoSkippedOrEnded = false;

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Prepare();

        Debug.Log("Preparing video: " + clip.name);

        while (!videoPlayer.isPrepared)
            yield return null;

        Debug.Log("Video is prepared. Playing: " + clip.name);
        videoPlayer.Play();

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;

        if (checkVideoCoroutine != null)
            StopCoroutine(checkVideoCoroutine);
        checkVideoCoroutine = StartCoroutine(CheckIfVideoDone());

        if (forceSkipCoroutine != null)
            StopCoroutine(forceSkipCoroutine);
        float timeout = (float)(clip.length > 0 ? clip.length : 65f);
        forceSkipCoroutine = StartCoroutine(ForceSkipAfterTimeout(timeout));
    }

    private void Update()
    {
        if (videoSkippedOrEnded)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            SkipVideo("Mouse click (PC)");
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            SkipVideo("Touch input (Mobile)");
        }
#endif
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (videoSkippedOrEnded)
            return;

        Debug.Log("Video finished event: " + vp.clip.name);

        if (currentVideoIndex == 0)
        {
            currentVideoIndex = 1;
            StartCoroutine(PlayVideoClip(introClip));
        }
        else
        {
            SkipVideo("All videos finished");
        }
    }

    private IEnumerator CheckIfVideoDone()
    {
        yield return new WaitForSeconds(0.1f);

        while (!videoSkippedOrEnded)
        {
            if (videoPlayer.frameCount > 0 && videoPlayer.isPlaying)
            {
                if (videoPlayer.frame >= (long)(videoPlayer.frameCount - 1))
                {
                    SkipVideo("Video finished (manual check)");
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator ForceSkipAfterTimeout(float timeout)
    {
        float watchedTime = 0f;

        while (!videoSkippedOrEnded)
        {
            if (!videoPausedDueToFocusLoss)
            {
                watchedTime += Time.deltaTime;
            }

            if (watchedTime >= timeout)
            {
                Debug.LogWarning("Force skipping video: timeout reached.");
                SkipVideo("Forced timeout");
                yield break;
            }

            yield return null;
        }
    }

    private void SkipVideo(string reason)
    {
        if (videoSkippedOrEnded)
            return;

        Debug.Log("Skipping video: " + reason);

        videoSkippedOrEnded = true;
        videoPlayer.Stop();

        if (checkVideoCoroutine != null)
            StopCoroutine(checkVideoCoroutine);
        if (forceSkipCoroutine != null)
            StopCoroutine(forceSkipCoroutine);

        if (currentVideoIndex == 0)
        {
            currentVideoIndex = 1;
            videoSkippedOrEnded = false;
            StartCoroutine(PlayVideoClip(introClip));
            return;
        }

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        PushNotification.ReSetNotifications();

        if (GameData.Instance.needsUpdate)
        {
            Debug.Log("[Init] Save requires update. Forcing GameCover load.");
            SceneManager.LoadScene("GameCover");
            return;
        }

        if (GameData.Instance.is_enter_tutorial)
        {
            if (GameData.Instance.cur_quest_info == null)
                GameData.Instance.cur_quest_info = new QuestInfo();

            GameData.Instance.cur_quest_info.mission_type = MissionType.Tutorial;
            GameData.Instance.cur_quest_info.mission_day_type = MissionDayType.Tutorial;
            GameData.Instance.loading_to_scene = "GameTutorial";
            SceneManager.LoadScene("Loading");
        }
        else
        {
            SceneManager.LoadScene(defaultNextScene);
        }
    }


    private void OnApplicationFocus(bool hasFocus)
    {
        if (videoPlayer == null)
            return;

        if (hasFocus)
        {
            if (!videoPlayer.isPlaying && videoPlayer.isPrepared)
            {
                videoPlayer.Play();
                videoPausedDueToFocusLoss = false;
                Debug.Log("Resuming video on focus.");
            }
        }
        else
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                videoPausedDueToFocusLoss = true;
                Debug.Log("Pausing video on focus lost.");
            }
        }
    }
}