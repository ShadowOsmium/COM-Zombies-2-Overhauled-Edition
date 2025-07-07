using System;

namespace Prime31.WinPhoneStore
{
	public static class Store
	{
		public static void loadTestingLicenseXml(string xml)
		{
		}

		public static void loadListingInformation(Action<ListingInformation> completionHandler)
		{
		}

		public static LicenseInformation getLicenseInformation()
		{
			return null;
		}

		public static void showMarketplaceDetailPage()
		{
		}

		public static void clearCache()
		{
		}

		public static void requestProductPurchase(string productId, Action<string, Exception> completionHandler)
		{
		}

		public static void reportProductFulfillment(string productId)
		{
		}

		public static ProductLicense getProductLicense(string productId)
		{
			return null;
		}

		public static string getProductReceipt(string productId)
		{
			return null;
		}
	}
}
