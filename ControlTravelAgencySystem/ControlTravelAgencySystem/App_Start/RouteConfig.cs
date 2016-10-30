﻿using System.Web.Mvc;
using System.Web.Routing;

namespace ControlTravelAgencySystem
{
    public class RouteConfig
    {
        /// <summary>
        /// Настройка URL маршрутизации
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // По умолчанию
            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Домашняя страница
            routes.MapRoute(
                name: "Home",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // ajax: Список отелей тура
            routes.MapRoute(
                name: "GetHotelsList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Hotels", action = "GetHotelsList", id = UrlParameter.Optional }
            );

            // ajax: Список номеров
            routes.MapRoute(
                name: "GetRoomsList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "GetRoomsList", id = UrlParameter.Optional }
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