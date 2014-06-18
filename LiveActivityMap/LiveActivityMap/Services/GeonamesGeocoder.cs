using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;

namespace LiveActivityMap.Services
{
	public class GeonamesGeocoder : IGeocoder
	{
		private readonly string _username;

		private IDictionary<string, string> _iso3166Dictionary = new Dictionary<string, string> {{"United States", "US"}, {"UK", "UK"}, {"Canada", "CA"}}; 

		public GeonamesGeocoder(string username)
		{
			_username = username;
		}

		public LatLong Geocode(string city, string stateprovince, string postalCode, string country)
		{
			const string urlFormat = "http://api.geonames.org/postalCodeSearchJSON?style=SHORT&postalcode={0}&placename={1}&countryBias={2}&username={3}";

			var iso3166Country = _iso3166Dictionary[country] ?? "US";

			var zipcode = (iso3166Country == "US") ? postalCode.Substring(0, 5) : postalCode;

			var url = string.Format(urlFormat, zipcode, city, iso3166Country, _username);
			var client = new WebClient();
			var jsonContent = client.DownloadString(url);

			var jsonSerializer = new JavaScriptSerializer();
			try
			{
				// I don't trust this code
				var content = jsonSerializer.DeserializeObject(jsonContent) as Dictionary<string, object>;

				var latlng = new LatLong();

				latlng.latitude = (decimal)((Dictionary<string, object>)((object[])((Dictionary<string, object>)content)["postalCodes"])[0])["lat"];
				latlng.longitude = (decimal)((Dictionary<string, object>)((object[])((Dictionary<string, object>)content)["postalCodes"])[0])["lng"];

				return latlng;
			}
			catch (Exception e)
			{
				return new LatLong() { latitude = 30.3874m, longitude = -97.7054m }; // 78758
			}

		}
	}
}