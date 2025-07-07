using System;
using System.Net;

namespace WPPlugin
{
	public class DnsAdaptor
	{
		public static event Action<bool, string> onGetHostAddress;

		public static void DnsLookup(string hostname)
		{
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
				string arg = hostAddresses[0].ToString();
				if (DnsAdaptor.onGetHostAddress != null)
				{
					DnsAdaptor.onGetHostAddress(true, arg);
				}
			}
			catch (Exception)
			{
				if (DnsAdaptor.onGetHostAddress != null)
				{
					DnsAdaptor.onGetHostAddress(false, "");
				}
			}
		}
	}
}
