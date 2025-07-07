using System;

namespace WPPlugin
{
	public class MsgBoxAdaptor
	{
		public static event Action<bool> onButtonOKCancel;

		public static void Show(string content, string name, bool okCancel)
		{
		}
	}
}
