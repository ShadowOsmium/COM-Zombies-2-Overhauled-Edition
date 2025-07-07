using System;
using System.Collections.Generic;

public class PushNotification
{
	public static void ReSetNotifications()
	{
		LocalNotificationWrapper.CancelAll();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list.Add("The zombie horde is descending, hurry up and fight them off!");
		list.Add("Your chainsaw thirsts for blood!");
		list.Add("A huge zombie is on the warpath, grab your gun and start shooting!");
		Random random = new Random();
		foreach (string item in list)
		{
			int index = random.Next(list2.Count + 1);
			list2.Insert(index, item);
		}
		int num = 1;
		foreach (string item2 in list2)
		{
			LocalNotificationWrapper.Schedule(item2, 259200 * num);
			num++;
		}
	}
}
