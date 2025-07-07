using System;
using System.Collections.Generic;

namespace Prime31.WinPhoneStore
{
	public class LicenseInformation
	{
		public DateTimeOffset expirationDate { get; set; }

		public bool isActive { get; set; }

		public bool isTrial { get; set; }

		public Dictionary<string, ProductLicense> productLicenses { get; set; }

		public new string ToString()
		{
			return null;
		}
	}
}
