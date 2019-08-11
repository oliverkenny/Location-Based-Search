using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Location_Based_Search
{
	public class Search
	{
		private int SearchRadius { get; set; }

		public Search(int searchRadius = 10)
		{
			this.SearchRadius = searchRadius;
		}

		public void SetSearchRadius(int searchRadius)
		{
			this.SearchRadius = searchRadius;
		}

		public void NewSearch()
		{
			List<string> postcodes = GetPostcodes();
			postcodes.ForEach(x => Console.WriteLine(GetLocationFromPostcode(x).ToString()));
			
		}

		private List<string> GetPostcodes()
		{
			List<string> postcodes = new List<string>
			{
				"LE8 5XT",
				"LE1 6GB",
				"LE2 9PL",
				"LE2 7PT"
			};

			return postcodes;
		}

		private Location GetLocationFromPostcode(string postcode)
		{
			if (!IsValidPostcode(postcode)) throw new ArgumentException();

			string uri = "http://api.postcodes.io/postcodes/" + postcode;
			string responceString = Get(uri);
			JObject responseObject = JObject.Parse(responceString);

			return new Location
			{
				Longitude = (double)responseObject.SelectToken("result.longitude"),
				Latitude = (double)responseObject.SelectToken("result.latitude")
			};
		}

		private string Get(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}

		private bool IsValidPostcode(string postcode)
		{
			return true;
		}
	}

	public class Location
	{
		public double Longitude { get; set; }
		public double Latitude { get; set; }

		public override string ToString() {
			return $"Longitude: [{Longitude}], Latitude: [{Latitude}]";
		}
	}
}
