using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveActivityMap.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/

        private string GetWebAppRoot(HttpRequestBase request)
        {
            string host = (request.Url.IsDefaultPort) ?
                request.Url.Host :
                request.Url.Authority;
            host = String.Format("{0}://{1}", request.Url.Scheme, host);
            if (request.ApplicationPath == "/")
                return host;
            else
                return host + request.ApplicationPath;
        }

        public ActionResult Index()
        {
            ViewBag.AppBaseUrl = GetWebAppRoot(Request) + "/";
            ViewBag.GoogleEarthZoomOutRange = ConfigurationManager.AppSettings["GoogleEarthZoomOutRange"];
            ViewBag.GoogleEarthZoomInRange = ConfigurationManager.AppSettings["GoogleEarthZoomInRange"];
            ViewBag.GoogleEarthFlyToSpeed = ConfigurationManager.AppSettings["GoogleEarthFlyToSpeed"];
	        ViewBag.RemoveTimeout = ConfigurationManager.AppSettings["RemoveTimeout"];

            return View();
        }
    }
}
