using LiveActivityMap.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.ServiceLocation;

namespace LiveActivityMap.Hubs
{
    public class LiveActivity : Hub
    {
        public void SendCity(string id, string city, string state, string postalCode, string country, string label, string iconUrl,
                             int size, string type)
        {
            var geocoder = ServiceLocator.Current.GetInstance<IGeocoder>();
            var latlong = geocoder.Geocode(city, state, postalCode, country);
            Send(id, latlong.latitude, latlong.longitude, label, iconUrl, size, type);
        }

        public void Send(string id, decimal latitude, decimal longitude, string label, string iconUrl, int size, string type)
        {
            Clients.All.send(id, latitude, longitude, label, iconUrl, size, type);
        }
    }
}