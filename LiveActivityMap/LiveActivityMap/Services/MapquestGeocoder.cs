using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace LiveActivityMap.Services
{
    public class MapquestGeocoder : IGeocoder
    {
        private readonly string _apikey;

        public MapquestGeocoder(string apikey)
        {
            _apikey = apikey;
        }

        public LatLong Geocode(string city, string stateprovince, string postalCode, string country)
        {
            const string urlFormat = "http://www.mapquestapi.com/geocoding/v1/address?key={0}&location={1}&maxResults=1&thumbMaps=false";

            var location = postalCode;
            var strongPostalCode = (country == "United States") || (country == "Canada") || (country == "UK");
            if (string.IsNullOrEmpty(location) || !strongPostalCode)
                location = string.Format("{0}, {1}", city, country);

            var payload = new
                {
                    maxResults = 1,
                    thumbMaps = false
                };
            var jsonSerializer = new JavaScriptSerializer();
            var jsonPayload = jsonSerializer.Serialize(payload);

            var http = (HttpWebRequest)WebRequest.Create(new Uri(string.Format(urlFormat, _apikey, location)));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(jsonPayload);

            var newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var jsonContent = sr.ReadToEnd();

            try
            {
                // I don't trust this code
                var content = jsonSerializer.DeserializeObject(jsonContent) as Dictionary<string, object>;

                var firstLatLng =
                    ((Dictionary<string, object>)
                     ((Dictionary<string, object>)
                      ((object[])((Dictionary<string, object>)((object[])content["results"])[0])["locations"])[0])[
                          "latLng"]);

                var latlng = new LatLong() { latitude = (decimal)firstLatLng["lat"], longitude = (decimal)firstLatLng["lng"] };

                return latlng;
            }
            catch (Exception e)
            {
                return new LatLong() { latitude = 30.3874m, longitude = -97.7054m }; // 78758
            }
        }
    }
}