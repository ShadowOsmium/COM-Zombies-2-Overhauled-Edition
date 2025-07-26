using CoMZ2;
using System.Collections;
using UnityEngine;

public class BossRoamEvent : MonoBehaviour, IRoamEvent
{
    private float fade_time;

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

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameSceneController.Instance.is_play_cg && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[BossRoamEvent] Cutscene skip triggered!");
            CancelInvoke();
            Animation anim = boss_view_obj.GetComponent<Animation>();
            if (anim != null && anim.isPlaying)
            {
                anim.Stop();
            }
            OnBossSpawn();
            OnCamerShakeBegin();
            OnCamerShakeOver();
            GameSceneController.Instance.is_play_cg = false;
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
            return;

        if (GetComponent<CameraFadeEvent>() != null)
        {
            var fade = GetComponent<CameraFadeEvent>();
            if (fade.isFadeOut)
                fade.on_fadeout_end = OnBossSpawn;
        }
        else
        {
            OnBossSpawn();
        }
    }

    private bool manualSkipDone = false;

    public void SkipCutsceneManually()
    {
        CancelInvoke();
        StopAllCoroutines();

        Debug.Log("[BossRoamEvent] SkipCutsceneManually called.");

        OnBossSpawn();
        OnCamerShakeBegin();
        OnCamerShakeOver();
        GameSceneController.Instance.is_play_cg = false;
    }

    private void OnBossSpawn()
    {
        int num = Random.Range(0, 2);

        var bossType = GameData.Instance.cur_quest_info.boss_type;

        if (bossType == EnemyType.E_FATCOOK || bossType == EnemyType.E_FATCOOK_E)
        {
            GameSceneController.Instance.enable_spawn_ani = num == 0 ? "Boss_FatCook_Camera_Show01" : "Boss_FatCook_Camera_Show02";
            cur_show_pos = num == 0 ? fat_show1_pos : fat_show2_pos;
        }
        else if (bossType == EnemyType.E_HAOKE_A || bossType == EnemyType.E_HAOKE_B)
        {
            GameSceneController.Instance.enable_spawn_ani = num == 0 ? "Haoke_Camera_Show01" : "Haoke_Camera_Show02";
            cur_show_pos = num == 0 ? haoke_show1_pos : haoke_show2_pos;
        }
        else if (bossType == EnemyType.E_WRESTLER || bossType == EnemyType.E_WRESTLER_E)
        {
            GameSceneController.Instance.enable_spawn_ani = num == 0 ? "Wrestler_Camera_Show01" : "Wrestler_Camera_Show02";
            cur_show_pos = num == 0 ? wrestler_show1_pos : wrestler_show2_pos;
        }
        else if (bossType == EnemyType.E_HALLOWEEN || bossType == EnemyType.E_HALLOWEEN_E)
        {
            GameSceneController.Instance.enable_spawn_ani = num == 0 ? "Hook_Demon_Camera_Show01" : "Hook_Demon_Camera_Show02";
            cur_show_pos = num == 0 ? halloween_show1_pos : halloween_show2_pos;
        }
        else if (bossType == EnemyType.E_SHARK || bossType == EnemyType.E_SHARK_E)
        {
            GameSceneController.Instance.enable_spawn_ani = num == 0 ? "Zombie_Guter_Tennung_Camera_Show01" : "Zombie_Guter_Tennung_Camera_Show02";
            cur_show_pos = num == 0 ? shark_show1_pos : shark_show2_pos;
        }

        AnimationUtil.PlayAnimate(boss_view_obj, GameSceneController.Instance.enable_spawn_ani, WrapMode.Once);
        GameSceneController.Instance.enable_boss_spawn = true;

        Camera.main.transform.parent = cur_show_pos.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;

        float aniLength = boss_view_obj.GetComponent<Animation>()[GameSceneController.Instance.enable_spawn_ani].length;
        cgCoroutine = StartCoroutine(EndCutsceneAfter(aniLength));
    }

    private void OnCamerShakeBegin()
    {
        if (Camera.main != null && cur_show_pos != null)
        {
            Camera.main.transform.parent = cur_show_pos.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }

        GameSceneController.Instance.is_play_cg = true;
    }

    private bool hasSkipped = false;

    public void SkipCutscene()
    {
        if (hasSkipped || !GameSceneController.Instance.is_play_cg) return;

        hasSkipped = true;
        if (!GameSceneController.Instance.is_play_cg) return;

        Debug.Log("[BossRoamEvent] Cutscene skip triggered via button!");

        CancelInvoke();

        Animation anim = boss_view_obj.GetComponent<Animation>();
        if (anim != null && anim.isPlaying)
        {
            anim.Stop();
        }

        OnBossSpawn();
        OnCamerShakeBegin();
        OnCamerShakeOver();

        GameSceneController.Instance.is_play_cg = false;
    }

    private void OnCamerShakeOver()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameSceneController.Instance.OnGameCgEnd();
    }

    private IEnumerator EndCutsceneAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        Camera.main.transform.parent = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameSceneController.Instance.is_play_cg = false;
        GameSceneController.Instance.GamePlayingState = PlayingState.Gaming;
        GameSceneController.Instance.OnGameCgEnd();
        CameraFade.Clear();
    }
}
