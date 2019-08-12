using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Location_Based_Search
{
	public class Search
	{
		public Search(string postcode)
		{
			Location origin = new Location(postcode);
			BoundingBox boundingBox = GetBoundingBox(origin, 10);
			List<Location> locations = GetEnvironmentVarables.GenerateLocations();

			locations.ForEach(x => Console.WriteLine(x.ToString() + ", Distance: [" + Distance(origin, x) + "]"));

			//locations = locations.Where(x =>
			//	x.Latitude < boundingBox.MaxLatitude &&
			//	x.Latitude > boundingBox.MinLatitude &&
			//	x.Longitude < boundingBox.MaxLongitude &&
			//	x.Longitude > boundingBox.MinLongitude)
			//	.ToList();


		}

		private BoundingBox GetBoundingBox(Location origin, double distance, bool isMetric = false)
		{
			double radius = isMetric ? 6371 : 3958.8;

			return new BoundingBox
			{
				MinLongitude = origin.Longitude - Rad2Deg(distance / radius / Math.Cos(Deg2Rad(origin.Latitude))),
				MaxLatitude = origin.Latitude + Rad2Deg(distance / radius),
				MaxLongitude = origin.Longitude + Rad2Deg(distance / radius / Math.Cos(Deg2Rad(origin.Latitude))),
				MinLatitude = origin.Latitude - Rad2Deg(distance / radius)
			};
		}

		private double Distance(Location origin, Location comparative, bool isMetric = false)
		{
			// convert latitude/longitude degrees for both coordinates
			// to radians: radian = degree * π / 180
			origin.Latitude = Deg2Rad(origin.Latitude);
			origin.Longitude = Deg2Rad(origin.Longitude);
			comparative.Latitude = Deg2Rad(comparative.Latitude);
			comparative.Longitude = Deg2Rad(comparative.Longitude);

			//$distance = acos(sin($lat1) * sin($lat2) + cos($lat1) * cos($lat2) * cos($lng1 - $lng2));

			// calculate great-circle distance
			//double distance = Math.Acos(
			//	Math.Sin(origin.Latitude) *
			//	Math.Sin(comparative.Latitude) +
			//	Math.Cos(origin.Latitude) *
			//	Math.Cos(comparative.Latitude) *
			//	Math.Cos(origin.Longitude - comparative.Longitude));

			var dLat = comparative.Latitude - origin.Latitude;
			var dLon = comparative.Longitude - origin.Longitude;
			var a =
			  Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			  Math.Cos(origin.Latitude) * Math.Cos(comparative.Latitude) *
			  Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
			  ;
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			var R = isMetric ? 6371 : 3958.8;
			return c * R;
		}

		private double Rad2Deg(double radions)
		{
			return 180 * radions / Math.PI;
		}

		private double Deg2Rad(double degrees)
		{
			return Math.PI * degrees / 180;
		}
	}

	public class BoundingBox
	{
		public double MinLongitude { get; set; }
		public double MaxLatitude { get; set; }
		public double MaxLongitude { get; set; }
		public double MinLatitude { get; set; }
	}

	public class Location
	{
		public string Postcode { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }

		public Location(string postcode)
		{
			SetCoordinatesFromPostcode(postcode);
		}

		private void SetCoordinatesFromPostcode(string postcode)
		{
			if (!IsValidPostcode(postcode)) throw new ArgumentException($"The postcode [{postcode}] was invalid.");

			string uri = "https://maps.googleapis.com/maps/api/geocode/json?components=postal_code:" + postcode + "&key=" + GetEnvironmentVarables.GetGoogleApiKey();
			string responceString = Web.Get(uri);
			JObject responseObject = JObject.Parse(responceString);

			this.Postcode = postcode;
			this.Longitude = (double)responseObject.SelectToken("results[0].geometry.location.lng");
			this.Latitude = (double)responseObject.SelectToken("results[0].geometry.location.lat");
		}

		private bool IsValidPostcode(string postcode)
		{
			return true;
		}

		public override string ToString()
		{
			return $"Postcode: [{Postcode}], Longitude: [{Longitude}], Latitude: [{Latitude}]";
		}
	}
}