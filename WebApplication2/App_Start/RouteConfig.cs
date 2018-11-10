using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "WaterLevel",
                url: "{controller}/{action}/{deviceID}/{waterLevel}",
                defaults: new { controller = "Home", action = "Index", deviceID = UrlParameter.Optional, waterLevel = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Admin", // Route name
                url: "{controller}/{action}/{deviceID}", // URL with parameters
                defaults: new { controller = "Home", action = "Index", deviceID = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DeviceIp", // Route name
                url: "{controller}/{action}/{deviceID}/{status}/{deviceIp}", // URL with parameters
                defaults: new { controller = "Home", action = "GetDeviceIp", deviceID = UrlParameter.Optional, deviceIp = UrlParameter.Optional }
            );


        }
    }
}
