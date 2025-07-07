using TNetSdk;
using UnityEngine;

public class CoopRoomUserItem : MonoBehaviour
{
	public TNetUser cur_user;

	protected TUILabel user_name;

	protected TUILabel user_combat_val;

	protected TUIMeshSprite user_icon;

	protected TUIButtonClick kick_button;

	protected TUIMeshSprite bk_img;

	private void Awake()
	{
		user_name = base.transform.Find("User_Name").GetComponent<TUILabel>();
		user_combat_val = base.transform.Find("User_CombatVal").GetComponent<TUILabel>();
		user_icon = base.transform.Find("User_Icon").GetComponent<TUIMeshSprite>();
		kick_button = base.transform.Find("KickButton").GetComponent<TUIButtonClick>();
		bk_img = base.transform.Find("BK").GetComponent<TUIMeshSprite>();
	}

	public bool ResetUserItem(TNetUser user, bool show_kick_button)
	{
		base.gameObject.SetActive(true);
		//cur_user = user;
		//if (cur_user == null)
		//{
		//	base.gameObject.SetActive(false);
		//	return false;
		//}
		//if (cur_user.ContainsVariable(TNetUserVarType.RoomState))
		//{
		//	SFSObject sFSObject = cur_user.GetVariable(TNetUserVarType.RoomState).GetSFSObject("data") as SFSObject;
		//	if (!sFSObject.GetBool("InRoom"))
		//	{
		//		cur_user = null;
		//		base.gameObject.SetActive(false);
		//		return false;
		//	}
			user_name.Text = GameConfig.Instance.AvatarConfig_Set[GameData.Instance.cur_avatar].show_name;
			int @int = (int)GameData.Instance.AvatarData_Set[GameData.Instance.cur_avatar].avatar_state;
			user_icon.texture = GameConfig.Instance.AvatarConfig_Set[GameData.Instance.cur_avatar].avatar_name + "_0" + @int + "_icon";
			user_combat_val.Text = "Day:" + GameData.Instance.day_level;
			//if (cur_user.IsItMe)
			//{
				bk_img.texture = "shuxin_dikuang";
			//}
			//else
			//{
			//	bk_img.texture = "shuxin_dikuang_lv";
			//}
		//}
		//if (TNetConnection.IsServer)
		//{
		//	if (cur_user.Id == TNetConnection.Connection.Myself.Id)
		//	{
		//		kick_button.gameObject.SetActive(false);
		//	}
		//	else if (show_kick_button)
		//	{
		//		kick_button.gameObject.SetActive(true);
		//	}
		//	else
		//	{
		//		kick_button.gameObject.SetActive(false);
		//	}
		//}
		//else
		//{
		//	kick_button.gameObject.SetActive(false);
		//}
		return true;
	}
}
