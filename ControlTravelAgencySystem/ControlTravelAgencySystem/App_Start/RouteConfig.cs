using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ControlTravelAgencySystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HotelsList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Hotels", action = "List", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HotelRoomsList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "HotelRooms", action = "List", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin",
                url: "{controller}/{action}",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                name: "Callout",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Callout", action = "Suggestions" }
            );

            routes.MapRoute(
                name: "Employee",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Employee", action = "Create", id = UrlParameter.Optional }
            );
        }
    }
}