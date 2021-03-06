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

            // ajax: Список номеров
            routes.MapRoute(
                name: "GetRoutesList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "GetRoutesList", id = UrlParameter.Optional }
            );

            // ajax: Список экскурсий
            routes.MapRoute(
                name: "GetExcursionsList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "GetExcursionsList", id = UrlParameter.Optional }
            );

            // ajax: Список доп. услуг
            routes.MapRoute(
                name: "GetHotelServicesList",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "GetHotelServicesList", id = UrlParameter.Optional }
            );

            // ajax: Checked
            routes.MapRoute(
                name: "HotelServiceChecked",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "HotelServiceChecked", id = UrlParameter.Optional }
            );

            // ajax: Checked
            routes.MapRoute(
                name: "ExcursionChecked",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "ExcursionChecked", id = UrlParameter.Optional }
            );

            // ajax: Checked
            routes.MapRoute(
                name: "RoomChecked",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "RoomChecked", id = UrlParameter.Optional }
            );

            // ajax: Checked
            routes.MapRoute(
                name: "FlightChecked",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "FlightChecked", id = UrlParameter.Optional }
            );

            // ajax: Checked
            routes.MapRoute(
                name: "RouteChecked",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Rooms", action = "RouteChecked", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
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

            routes.MapRoute(
                name: "Tours",
                url: "{controller}/{action}",
                defaults: new { controller = "Tours", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Hotels",
                url: "{controller}/{action}",
                defaults: new { controller = "Hotels", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Rooms",
                url: "{controller}/{action}",
                defaults: new { controller = "Rooms", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Flights",
                url: "{controller}/{action}",
                defaults: new { controller = "Flights", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Routes",
                url: "{controller}/{action}",
                defaults: new { controller = "Routes", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Excursions",
                url: "{controller}/{action}",
                defaults: new { controller = "Excursions", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HotelServices",
                url: "{controller}/{action}",
                defaults: new { controller = "HotelServices", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Suggestions",
                url: "{controller}/{action}",
                defaults: new { controller = "HotelServices", action = "Create", id = UrlParameter.Optional, type = UrlParameter.Optional }
            );
        }
    }
}