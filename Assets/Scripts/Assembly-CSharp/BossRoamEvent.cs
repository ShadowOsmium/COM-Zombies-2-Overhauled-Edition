using CoMZ2;
using System.Collections;
using UnityEngine;

public class BossRoamEvent : MonoBehaviour, IRoamEvent
{
    private float fade_time = 2f;

    public GameObject boss_view_obj;

    public GameObject haoke_show1_pos;
    public GameObject haoke_show2_pos;
    public GameObject fat_show1_pos;
    public GameObject fat_show2_pos;
    public GameObject wrestler_show1_pos;
    public GameObject wrestler_show2_pos;
    public GameObject halloween_show1_pos;
    public GameObject halloween_show2_pos;
    public GameObject shark_show1_pos;
    public GameObject shark_show2_pos;

    private GameObject cur_show_pos;

    private Coroutine cgCoroutine;

    public static BossRoamEvent Instance { get; private set; }

    private bool hasSkipped = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Skip cutscene on Space key press if cutscene is playing
        if (GameSceneController.Instance.is_play_cg && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[BossRoamEvent] Cutscene skip triggered!");
            SkipCutscene();
        }
    }

    public void OnRoamTrigger()
    {
        GameSceneController.Instance.is_play_cg = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRoamStop()
    {
        if (GameSceneController.Instance.IsSkipCg)
        {
            // Skip logic already handled
            return;
        }

        CameraFadeEvent fade = GetComponent<CameraFadeEvent>();
        if (fade != null && fade.isFadeOut)
        {
            fade.on_fadeout_end = OnBossSpawn;
        }
        else
        {
            OnBossSpawn();
        }
    }

    public void SkipCutscene()
    {
        if (hasSkipped || !GameSceneController.Instance.is_play_cg)
            return;

        hasSkipped = true;

        // Stop any ongoing animation on boss_view_obj
        Animation anim = boss_view_obj.GetComponent<Animation>();
        if (anim != null && anim.isPlaying)
        {
            anim.Stop();
        }

        // Immediately spawn boss and do camera shake effects
        OnBossSpawn();
        OnCameraShakeBegin();
        OnCameraShakeOver();

        GameSceneController.Instance.is_play_cg = false;
        GameSceneController.Instance.StopCameraRoam();
    }

    private void OnBossSpawn()
    {
        int num = Random.Range(0, 2);
        EnemyType bossType = GameData.Instance.cur_quest_info.boss_type;

        switch (bossType)
        {
            case EnemyType.E_FATCOOK:
            case EnemyType.E_FATCOOK_E:
                GameSceneController.Instance.enable_spawn_ani = (num == 0) ? "Boss_FatCook_Camera_Show01" : "Boss_FatCook_Camera_Show02";
                cur_show_pos = (num == 0) ? fat_show1_pos : fat_show2_pos;
                break;

            case EnemyType.E_HAOKE_A:
            case EnemyType.E_HAOKE_B:
                GameSceneController.Instance.enable_spawn_ani = (num == 0) ? "Haoke_Camera_Show01" : "Haoke_Camera_Show02";
                cur_show_pos = (num == 0) ? haoke_show1_pos : haoke_show2_pos;
                break;

            case EnemyType.E_WRESTLER:
            case EnemyType.E_WRESTLER_E:
                GameSceneController.Instance.enable_spawn_ani = (num == 0) ? "Wrestler_Camera_Show01" : "Wrestler_Camera_Show02";
                cur_show_pos = (num == 0) ? wrestler_show1_pos : wrestler_show2_pos;
                break;

            case EnemyType.E_HALLOWEEN:
            case EnemyType.E_HALLOWEEN_E:
                GameSceneController.Instance.enable_spawn_ani = (num == 0) ? "Hook_Demon_Camera_Show01" : "Hook_Demon_Camera_Show02";
                cur_show_pos = (num == 0) ? halloween_show1_pos : halloween_show2_pos;
                break;

            case EnemyType.E_SHARK:
            case EnemyType.E_SHARK_E:
                GameSceneController.Instance.enable_spawn_ani = (num == 0) ? "Zombie_Guter_Tennung_Camera_Show01" : "Zombie_Guter_Tennung_Camera_Show02";
                cur_show_pos = (num == 0) ? shark_show1_pos : shark_show2_pos;
                break;

            default:
                Debug.LogWarning("[BossRoamEvent] Unknown boss type: " + bossType);
                return;
        }

        AnimationUtil.PlayAnimate(boss_view_obj, GameSceneController.Instance.enable_spawn_ani, WrapMode.Once);
        GameSceneController.Instance.enable_boss_spawn = true;

        if (Camera.main != null && cur_show_pos != null)
        {
            Camera.main.transform.parent = cur_show_pos.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }

        float aniLength = boss_view_obj.GetComponent<Animation>()[GameSceneController.Instance.enable_spawn_ani].length;
        cgCoroutine = StartCoroutine(EndCutsceneAfter(aniLength));
    }

    private IEnumerator EndCutsceneAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        OnCameraShakeOver();
    }

    private void OnCameraShakeBegin()
    {
        // You might want to do actual camera shake here
        Debug.Log("[BossRoamEvent] Camera shake begins");
    }

    private void OnCameraShakeOver()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameSceneController.Instance.is_play_cg = false;
        GameSceneController.Instance.OnGameCgEnd();

        CameraFade.Clear();

        if (Camera.main != null)
        {
            Camera.main.transform.parent = null;
        }
    }

    public void SkipCutsceneManually()
    {
        Debug.Log("[SceneRoamEvent] SkipCutsceneManually called");

        GameSceneController.Instance.is_skip_cg = true;
        GameSceneController.Instance.is_play_cg = false;

        CancelInvoke();
        StopAllCoroutines();
    }
}