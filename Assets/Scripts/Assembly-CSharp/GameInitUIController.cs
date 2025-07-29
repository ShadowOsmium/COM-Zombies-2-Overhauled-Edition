using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using CoMZ2;
using System;
using UnityEngine.Networking;

public class GameInitUIController : MonoBehaviour
{
    [Serializable]
    private class GitHubRelease
    {
        public string tag_name;
    }

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

    private string remoteVersion; // To store the fetched remote version

    private IEnumerator Start()
    {
        GameConfig.CheckGameConfig();

        bool loaded = GameData.Instance.LoadData();
        Debug.Log("Save loaded: " + loaded);

        bool isNewSave = false;

        // 1. Init save if load failed
        if (!loaded)
        {
            Debug.LogWarning("Save failed to load. Initializing new save.");
            GameData.Instance.Init();

            string localVersion = LoadLocalVersionFromResources();
            GameData.Instance.game_version = localVersion ?? "1.3.3";
            GameData.Instance.needsUpdate = false;

            GameData.Instance.SaveData();

            isNewSave = true; // mark this as a new save
        }

        // 2. Attempt to get remote version
        yield return StartCoroutine(GetRemoteGameVersion());

        bool hasRemote = !string.IsNullOrEmpty(remoteVersion);
        Debug.Log(string.Format("Remote Version: {0}, Local Version: {1}", remoteVersion, GameData.Instance.game_version));

        if (hasRemote)
        {
            if (!VersionsMatch(GameData.Instance.game_version, remoteVersion))
            {
                Debug.LogWarning("Version mismatch detected. Forcing update.");

                GameData.Instance.needsUpdate = true;
                GameData.Instance.game_version = remoteVersion;
                GameData.Instance.SaveData();

                SceneManager.LoadScene("GameCover");
                yield break;
            }
            else
            {
                GameData.Instance.needsUpdate = false;
                GameData.Instance.SaveData();
            }
        }
        else
        {
            // Offline check — only block if update is actually required
            // AND this is NOT a new save
            if (GameData.Instance.needsUpdate && !isNewSave)
            {
                Debug.LogWarning("Offline but update flagged. Blocking access.");
                SceneManager.LoadScene("GameCover");
                yield break;
            }
            else
            {
                Debug.Log("[Offline] No update needed or new save, allowing play.");
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            // Fallback to version.txt (editor/pc only)
            string localVersion = LoadLocalVersionFromResources();
            GameData.Instance.game_version = localVersion ?? "DEV-PC";
            Debug.Log("[Editor] Loaded local version: " + GameData.Instance.game_version);
#endif
        }

        // 3. Update login date
        if (GameData.Instance != null)
        {
            GameData.Instance.lastLoginDate = DateTime.Today;
            GameData.Instance.SaveData();
            Debug.Log("[Init] Updated lastLoginDate to: " + GameData.Instance.lastLoginDate);
        }

        // 4. Continue initialization (ads, video, etc.)
        OpenClikPlugin.Initialize("A36F6C65-C1E3-47D4-AD07-AA8A6C90132C");

#if UNITY_ANDROID && !UNITY_EDITOR
    yield return PlayAndroidVideosSequentially();
#else
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
            audioSource = gameObject.AddComponent<AudioSource>();

        videoPlayer.SetTargetAudioSource(0, audioSource);
        yield return PlayVideoClip(promotionClip);
#endif
    }

    private string LoadLocalVersionFromResources()
    {
        TextAsset versionAsset = Resources.Load<TextAsset>("version");
        if (versionAsset != null)
        {
            return versionAsset.text.Trim();
        }
        Debug.LogWarning("Could not load version.txt from Resources.");
        return null;
    }

    private IEnumerator GetRemoteGameVersion()
    {
        // Replace with your actual GitHub release URL or API endpoint
        string url = "https://api.github.com/repos/ShadowOsmium/COM-Zombies-2-Overhauled-Edition/releases/latest";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error fetching remote version: " + webRequest.error);
            }
            else
            {
                // Parse the response to get the version
                string jsonResponse = webRequest.downloadHandler.text;

                try
                {
                    GitHubRelease release = JsonUtility.FromJson<GitHubRelease>(jsonResponse);
                    remoteVersion = release.tag_name;

                    if (!string.IsNullOrEmpty(remoteVersion))
                    {
                        remoteVersion = remoteVersion.Trim();
                        if (remoteVersion.StartsWith("v"))
                        {
                            remoteVersion = remoteVersion.Substring(1);
                        }
                    }

                    Debug.Log("Cleaned remote version: " + remoteVersion);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse GitHub version: " + e);
                }
            }
        }
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

        Debug.Log("[LoadNextScene] needsUpdate: " + GameData.Instance.needsUpdate + ", is_enter_tutorial: " + GameData.Instance.is_enter_tutorial);

        if (GameData.Instance.needsUpdate)
        {
            Debug.Log("[Init] Save requires update. Forcing GameCover load.");
            SceneManager.LoadScene("GameCover");
            return;
        }

        if (GameData.Instance.is_enter_tutorial)
        {
            Debug.Log("[Init] Tutorial NOT completed, loading tutorial scene.");

            if (GameData.Instance.cur_quest_info == null)
                GameData.Instance.cur_quest_info = new QuestInfo();

            GameData.Instance.cur_quest_info.mission_type = MissionType.Tutorial;
            GameData.Instance.cur_quest_info.mission_day_type = MissionDayType.Tutorial;
            GameData.Instance.loading_to_scene = "GameTutorial";

            SceneManager.LoadScene("Loading");
            return;  // Important to return here to avoid falling through
        }
        else
        {
            Debug.Log("[Init] Tutorial completed or skipped, loading default scene.");
            // Tutorial done, go to normal game scene
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

    private bool VersionsMatch(string local, string remote)
    {
        return local.Trim().ToLower() == remote.Trim().ToLower();
    }
}