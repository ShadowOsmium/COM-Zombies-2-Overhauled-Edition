using System.Collections.Generic;

namespace Prime31.WinPhoneStore
{
	public class ListingInformation
	{
		public string description { get; set; }

		public string formattedPrice { get; set; }

		public string name { get; set; }

		public Dictionary<string, ProductListing> productListings { get; set; }

		public new string ToString()
		{
			return null;
		}
	}
}
