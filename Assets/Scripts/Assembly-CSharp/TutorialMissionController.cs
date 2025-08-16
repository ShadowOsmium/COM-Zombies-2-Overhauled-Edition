using System.Collections;
using System.Collections.Generic;
using CoMZ2;
using UnityEngine;

public class TutorialMissionController : MissionController
{
    private GuideController guideController;

    public override List<EnemyType> GetMissionEnemyTypeList()
    {
        List<EnemyType> list = new List<EnemyType>();
        list.Add(EnemyType.E_ZOMBIE);
        return list;
    }

    public override void Update()
    {
    }

    public override IEnumerator Start()
    {
        GameSceneController.Instance.game_main_panel.gameObject.SetActive(false);

        if (zombie_nest_array == null)
        {
            InitMissionController();
        }

        mission_type = MissionType.Tutorial;

        yield return null;

        PlayerController player = GameSceneController.Instance.player_controller;
        while (player == null)
        {
            yield return null;
            player = GameSceneController.Instance.player_controller;
        }

        yield return new WaitForSeconds(1f);

        GameObject guideObj = Object.Instantiate(Resources.Load("Prefab/GuideUI")) as GameObject;
        guideObj.transform.parent = GameObject.Find("TUI/TUIControls").transform;
        guideObj.transform.localPosition = new Vector3(0f, 0f, -100f);
        guideController = guideObj.GetComponent<GuideController>();

        guideController.Show(new MoveGuide(guideController));

        yield return null;

        for (int i = 0; i < 3; i++)
        {
            SpwanZombiesFromNest(EnemyType.E_ZOMBIE, zombie_nest_array[0]);
        }

        yield return new WaitForSeconds(3f);

        Vector3 arrowPosition = Vector3.zero;

        while (!EnemyNearPlayer(ref arrowPosition))
        {
            yield return null;
        }

        guideController.Show(new KillZombieGuide(guideController, arrowPosition));
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked after KillZombieGuide.");

        while (!CouldHuntBox())
        {
            yield return null;
        }

        guideController.Show(new DestroyBoxGuide(guideController));

        while (!GameSceneController.Instance.tutorial_ui_over)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        MissionFinished();
    }

        private bool EnemyNearPlayer(ref Vector3 arrowPos)
    {
        if (GameSceneController.Instance.Enemy_Set.Count > 0)
        {
            foreach (int key in GameSceneController.Instance.Enemy_Set.Keys)
            {
                if (GameSceneController.Instance.Enemy_Set[key].SqrDistanceFromPlayer < 300f)
                {
                    Vector3 position = GameSceneController.Instance.Enemy_Set[key].transform.position;
                    Vector3 position2 = GameSceneController.Instance.main_camera.GetComponent<Camera>().WorldToScreenPoint(position);
                    Vector3 vector = GameSceneController.Instance.tui_camera.GetComponent<Camera>().ScreenToWorldPoint(position2);
                    arrowPos = new Vector3(vector.x, vector.y + 50f, -5f);
                    return true;
                }
            }
        }
        return false;
    }

    private bool CouldHuntBox()
    {
        return GameSceneController.Instance.Enemy_Set.Count == 0;
    }
}