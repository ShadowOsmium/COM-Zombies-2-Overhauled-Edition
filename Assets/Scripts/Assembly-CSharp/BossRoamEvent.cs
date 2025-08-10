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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameSceneController.Instance.is_play_cg)
        {
            GameSceneController.Instance.StopCameraRoam();
            OnGameCgEnd();
        }
    }

    public void OnRoamTrigger()
    {
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

    private void OnBossSpawn()
    {
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
        Invoke("OnCamerShakeOver", boss_view_obj.GetComponent<Animation>()[GameSceneController.Instance.enable_spawn_ani].length);
    }

    private IEnumerator OnGameCgEnd()
    {
        yield return new WaitForSeconds(fade_time);
        GameSceneController.Instance.OnGameCgEnd();
        yield return 1;
        CameraFade.Clear();
    }

    private void OnCamerShakeOver()
    {
        GameSceneController.Instance.OnGameCgEnd();
    }

    private void OnCamerShakeBegin()
    {
        Camera.main.transform.parent = cur_show_pos.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
}