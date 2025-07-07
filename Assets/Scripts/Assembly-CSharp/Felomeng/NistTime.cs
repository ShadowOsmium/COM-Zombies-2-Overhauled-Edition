using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Felomeng
{
	public class NistTime
	{
		private static char[] SeparatorArray = new char[1] { ' ' };

		public DateTime GetNistTime()
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("132.163.4.101"), 13);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.ReceiveTimeout = 10000;
			Socket socket2 = socket;
			socket2.Connect(remoteEP);
			byte[] array = new byte[1024];
			StringBuilder stringBuilder = new StringBuilder();
			int count;
			while ((count = socket2.Receive(array, 0, 1024, SocketFlags.None)) > 0)
			{
				stringBuilder.Append(Encoding.UTF8.GetString(array, 0, count));
			}
			DateTime result = ParseDateTimeFromString(stringBuilder.ToString());
			socket2.Close();
			return result;
		}

		private static DateTime ParseDateTimeFromString(string daytimeString)
		{
			daytimeString = daytimeString.Replace("\n", string.Empty).Replace("\0", string.Empty);
			string[] array = daytimeString.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries);
			if (array[7] != "UTC(NIST)" || array[8] != "*")
			{
				throw new Exception(string.Format("Invalid RFC-867 daytime protocol string: '{0}'", daytimeString));
			}
			int num = int.Parse(array[0]);
			DateTime dateTime = new DateTime(1858, 11, 17).AddDays(num);
			string[] array2 = array[2].Split(':');
			int num2 = int.Parse(array2[0]);
			int num3 = int.Parse(array2[1]);
			int num4 = int.Parse(array2[2]);
			double num5 = double.Parse(array[6], new CultureInfo("en-US"));
			return dateTime.AddHours(num2).AddMinutes(num3).AddSeconds(num4)
				.AddMilliseconds(0.0 - num5);
		}
	}
}
