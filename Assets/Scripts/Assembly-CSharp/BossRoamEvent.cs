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

    private Coroutine cgEndCoroutine;

    public static BossRoamEvent Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple BossRoamEvent instances detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameSceneController.Instance.is_play_cg)
        {
            GameSceneController.Instance.is_skip_cg = true;

            CancelInvoke("OnCamerShakeBegin");
            CancelInvoke("OnCamerShakeOver");

            if (boss_view_obj != null)
            {
                var anim = boss_view_obj.GetComponent<Animation>();
                if (anim != null && anim.isPlaying)
                {
                    anim.Stop();
                }
            }

            if (cgEndCoroutine != null)
            {
                StopCoroutine(cgEndCoroutine);
                cgEndCoroutine = null;
            }

            FinishCutsceneCleanup();

            OnBossSpawn();
        }
    }

    public void StartCutsceneEnd(float fadeDuration)
    {
        fade_time = fadeDuration;
        cgEndCoroutine = StartCoroutine(OnGameCgEnd());
    }

    private IEnumerator OnGameCgEnd()
    {
        yield return new WaitForSeconds(fade_time);

        FinishCutsceneCleanup();
    }

    private void OnGameCgEndImmediate()
    {
        FinishCutsceneCleanup();
    }

    private void FinishCutsceneCleanup()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameSceneController.Instance.is_play_cg = false;
        GameSceneController.Instance.is_skip_cg = false;
        GameSceneController.Instance.OnGameCgEnd();
        CameraFade.Clear();
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
			return;
		}
		if (GetComponent<CameraFadeEvent>() != null)
		{
			CameraFadeEvent component = GetComponent<CameraFadeEvent>();
			if (component.isFadeOut)
			{
				component.on_fadeout_end = OnBossSpawn;
			}
		}
		else
		{
			OnBossSpawn();
		}
	}

    public bool hasSpawnedBoss = false;
    private void OnBossSpawn()
	{
        if (hasSpawnedBoss) return;
        hasSpawnedBoss = true;

        GameSceneController.Instance.is_play_cg = true;
        int num = Random.Range(0, 100) % 2;
		if (GameData.Instance.cur_quest_info.boss_type == EnemyType.E_FATCOOK || GameData.Instance.cur_quest_info.boss_type == EnemyType.E_FATCOOK_E)
		{
			if (num == 0)
			{
				GameSceneController.Instance.enable_spawn_ani = "Boss_FatCook_Camera_Show01";
				cur_show_pos = fat_show1_pos;
			}
			else
			{
				GameSceneController.Instance.enable_spawn_ani = "Boss_FatCook_Camera_Show02";
				cur_show_pos = fat_show2_pos;
			}
		}
        else if (GameData.Instance.cur_quest_info.boss_type == EnemyType.E_HAOKE_A || GameData.Instance.cur_quest_info.boss_type == EnemyType.E_HAOKE_B)
		{
			if (num == 0)
			{
				GameSceneController.Instance.enable_spawn_ani = "Haoke_Camera_Show01";
				cur_show_pos = haoke_show1_pos;
			}
			else
			{
				GameSceneController.Instance.enable_spawn_ani = "Haoke_Camera_Show02";
				cur_show_pos = haoke_show2_pos;
			}
		}
		else if (GameData.Instance.cur_quest_info.boss_type == EnemyType.E_WRESTLER || GameData.Instance.cur_quest_info.boss_type == EnemyType.E_WRESTLER_E)
		{
			if (num == 0)
			{
				GameSceneController.Instance.enable_spawn_ani = "Wrestler_Camera_Show01";
				cur_show_pos = wrestler_show1_pos;
			}
			else
			{
				GameSceneController.Instance.enable_spawn_ani = "Wrestler_Camera_Show02";
				cur_show_pos = wrestler_show2_pos;
			}
		}
		else if (GameData.Instance.cur_quest_info.boss_type == EnemyType.E_HALLOWEEN || GameData.Instance.cur_quest_info.boss_type == EnemyType.E_HALLOWEEN_E)
		{
			if (num == 0)
			{
				GameSceneController.Instance.enable_spawn_ani = "Hook_Demon_Camera_Show01";
				cur_show_pos = halloween_show1_pos;
			}
			else
			{
				GameSceneController.Instance.enable_spawn_ani = "Hook_Demon_Camera_Show02";
				cur_show_pos = halloween_show2_pos;
			}
		}
		else if (GameData.Instance.cur_quest_info.boss_type == EnemyType.E_SHARK || GameData.Instance.cur_quest_info.boss_type == EnemyType.E_SHARK_E)
		{
			if (num == 0)
			{
				GameSceneController.Instance.enable_spawn_ani = "Zombie_Guter_Tennung_Camera_Show01";
				cur_show_pos = shark_show1_pos;
			}
			else
			{
				GameSceneController.Instance.enable_spawn_ani = "Zombie_Guter_Tennung_Camera_Show02";
				cur_show_pos = shark_show2_pos;
			}
		}
        AnimationUtil.PlayAnimate(boss_view_obj, GameSceneController.Instance.enable_spawn_ani, WrapMode.Once);
        GameSceneController.Instance.enable_boss_spawn = true;
        Invoke("OnCamerShakeBegin", 0.05f);
        Invoke("OnCamerShakeOver", 3f);
    }

    private void OnCamerShakeOver()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameSceneController.Instance.OnGameCgEnd();
        GameSceneController.Instance.is_play_cg = false;
    }

    private void OnCamerShakeBegin()
	{
        Camera.main.transform.parent = cur_show_pos.transform;
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;
	}
}
