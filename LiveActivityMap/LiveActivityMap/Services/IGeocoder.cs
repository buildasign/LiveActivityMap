using System.Linq;
using System.Web;

namespace LiveActivityMap.Services
{
    public class LatLong
    {
        public decimal latitude;
        public decimal longitude;
    }

    public interface IGeocoder
    {
        LatLong Geocode(string city, string stateprovince, string postalCode, string country);
    }
}