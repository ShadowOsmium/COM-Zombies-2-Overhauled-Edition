namespace TNetSdk
{
	public class RoomDragListCmd : RoomCmd
	{
		public enum ListType
		{
			all,
			not_full,
			not_full_not_game
		}

		public RoomDragListCmd(ushort group_id, ushort page, ushort page_split, ListType list_type)
		{
			PushUInt16(group_id);
			PushUInt16(page);
			PushUInt16(page_split);
			PushUInt16((ushort)list_type);
		}

		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_heartbeat);
		}
	}
}
