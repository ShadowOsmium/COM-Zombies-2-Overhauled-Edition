using System;

namespace Prime31.WinPhoneStore
{
	public class ProductLicense
	{
		public DateTimeOffset expirationDate { get; set; }

		public bool isActive { get; set; }

		public string productId { get; set; }

		public new string ToString()
		{
			return null;
		}
	}
}
