﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using ControlTravelAgencySystem.Common;
using System.Data.Entity;

namespace ControlTravelAgencySystem.Controllers
{
    /**
     * Контроллер доступа к панели администратора
     */ 
    public class AdminController : Controller
    {
        public static int EXPIRE_COOKIE = 1; // время жизни cookie в часах 

        // Словарь соответствий англ - рус слов
        public static Dictionary<string, string> CALLOUT_ORDER_STATUS_RUSSIFIERS = new Dictionary<string, string>
        {
            { "paid", "оплачено" },
            { "canceled", "отменено" },
            { "expired", "просрочено" },
            { "pending", "в ожидание" }
        };

        private readonly TravelSystemEntities _dbContext;
        public AdminController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            string section = id;

            // объект для взаимодействия с html
            AdminPanelPageView model = new AdminPanelPageView();

            // параметр доступа
            ViewBag.access = false;
            // параметр наличия ошибки
            ViewBag.error = null;
            
            model.user = new employee();
            // проверка наличия cookie, если пользователь уже авторизован
            if (HttpContext.Request.Cookies["session"] != null)
            {
                string sessionFromCookie = HttpContext.Request.Cookies["session"].Value;
                if (sessionFromCookie != null)
                {
                    // проверка подлинности пользователя, представившегося по cookie
                    employee user = _dbContext.employees.Where(e => e.session == sessionFromCookie).FirstOrDefault();
                    if (user != null)
                    {
                        // Аутентификация успешно пройдена
                        ViewBag.access = true;

                        model.user = user;
                        if (section != null)
                        {
                            model.section = section;
                        }
                        // подготовка данных для авторизованного пользователя 
                        prepareModel(model);
                    }
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AdminPanelPageView model)
        {
            ViewBag.error = null;
            ViewBag.access = false;

            ErrorView error = new ErrorView();
            employee user = model.user;

            // проверка наличия данных с полей логин и пароль после нажатия войти
            if (user.email != null && user.password != null)
            {
                string hash = Utils.md5(user.password);

                // Проверка подлинности пользователя
                employee userDb = _dbContext.employees
                    .Where(u => u.email == user.email && u.password == hash)
                    .FirstOrDefault();

                if (userDb != null)
                {
                    // Успешно
                    ViewBag.access = true;

                    string session = Utils.md5(userDb.id.ToString() + userDb.password + DateTime.UtcNow.ToString());
                    userDb.session = session;
                    _dbContext.SaveChangesAsync();

                    HttpCookie cookie = new HttpCookie("session");
                    cookie.Value = session;
                    cookie.Expires = DateTime.Now.AddHours(AdminController.EXPIRE_COOKIE);
                    HttpContext.Response.Cookies.Add(cookie);

                    model.user = userDb;
                    prepareModel(model);
                }
                else
                {
                    error.type = "danger";
                    error.message = new string[] { "Ошибка!", "Доступ запрещен" };
                }
            }
            else
            {
                error.type = "warning";
                error.message = new string[] { "Внимание!", "Некорректно заполнены данные" };
            }

            ViewBag.error = error;

            return View(model);
        }

        private void prepareModel(AdminPanelPageView model)
        {
            switch (model.section)
            {
                case "hotel_services":

                    ViewBag.Title = "Управление платными услугами";

                    model.hotel_services = _dbContext.hotel_service
                        .OrderByDescending(hs => hs.id)
                        .ToList();

                    model.hotels = _dbContext.hotels.ToList();

                    break;


                case "excursions":

                    ViewBag.Title = "Управление экскурсиями";

                    model.excursions = _dbContext.excursions
                        .OrderByDescending(e => e.id)
                        .ToList();

                    model.cities = _dbContext.cities.ToList();

                    break;

                case "routes":

                    ViewBag.Title = "Управление трансферными маршрутами";

                    model.routes = _dbContext.routes
                        .OrderByDescending(r => r.id)
                        .ToList();

                    model.airports = _dbContext.airports.ToList();

                    break;

                case "flights":

                    ViewBag.Title = "Управление авиарейсами";

                    model.flights = _dbContext.flights
                        .OrderByDescending(f => f.id)
                        .ToList();

                    model.airports = _dbContext.airports.ToList();
                    model.airlines = _dbContext.airlines.ToList();

                    break;

                case "employees":

                    ViewBag.Title = "Управление сотрудниками";

                    model.employees = _dbContext.employees
                        .Include("person")
                        .OrderByDescending(e => e.created_at)
                        .DefaultIfEmpty();

                    break;

                case "tours":

                    ViewBag.Title = "Управление турами";

                    model.tours = _dbContext
                        .tours
                        .OrderByDescending(t => t.id)
                        .ToList();

                    model.countries = _dbContext.countries.ToList();

                    break;

                case "hotels":

                    ViewBag.Title = "Управление отелями";

                    model.hotels = _dbContext
                        .hotels
                        .OrderByDescending(h => h.id)
                        //.ToList();
                        //.Take(1)
                        .ToList();

                    model.foods = _dbContext.foods.ToList();
                    model.cities = _dbContext.cities.ToList();
                    model.tours = _dbContext.tours.ToList();

                    break;

                case "rooms":

                    ViewBag.Title = "Управление номерами";

                    model.rooms = _dbContext
                        .rooms
                        .OrderByDescending(r => r.id)
                        .ToList();

                    model.hotels = _dbContext.hotels.Include("tour").ToList();

                    break;

                default:

                    ViewBag.Title = "Панель управления";

                    string calloutOrderStatusCriteria = Request.Params.Get("callout_order_status");
                    if (calloutOrderStatusCriteria != null && calloutOrderStatusCriteria != "")
                    {
                        // запрос на получение всех заявок по статусу их заказов (вкладка Заявки)
                        string status = CALLOUT_ORDER_STATUS_RUSSIFIERS[calloutOrderStatusCriteria];

                        if (calloutOrderStatusCriteria == "pending")
                        {
                            model.callouts = (
                                from c in _dbContext.callouts
                                join co in _dbContext.callout_order on c.id equals co.callout_id into join_scope
                                from co in join_scope.DefaultIfEmpty()
                                where co.id == null || co.status == status
                                select c
                            )
                            .OrderByDescending(c => c.created_at)
                            .ToList();
                        }
                        else
                        {
                            model.callouts = (

                                from c in _dbContext.callouts
                                join co in _dbContext.callout_order on c.id equals co.callout_id
                                where co.status == status
                                select c
                            )
                            .OrderByDescending(c => c.created_at)
                            .ToList();
                        }
                    }
                    else
                    {
                        model.callouts = _dbContext.callouts
                            .OrderByDescending(c => c.created_at)
                            .ToList();
                    }

                    string calloutIsPredefinedCriteria = Request.Params.Get("is_predefined");
                    if (calloutIsPredefinedCriteria != null && calloutIsPredefinedCriteria == "1")
                    {
                        model.callouts = model.callouts.Where(c => c.is_predefined == 1).ToList();
                    }


                    model.rooms = _dbContext.rooms.ToList();
                    model.flights = _dbContext.flights.ToList();
                    model.routes = _dbContext.routes.ToList();
                    model.excursions = _dbContext.excursions.ToList();
                    model.hotel_services = _dbContext.hotel_service.ToList();

                    break;
            }
        }

        public ActionResult Logout()
        {
            HttpCookie sessionFromCookie = HttpContext.Request.Cookies["session"];

            employee currentUser = _dbContext.employees
                .Where(e => e.session == sessionFromCookie.Value)
                .FirstOrDefault();

            if (currentUser != null)
            {
                // сброс данных в Cookie
                currentUser.session = null;
                _dbContext.SaveChangesAsync();
                
                sessionFromCookie.Expires = DateTime.Now.AddHours(-1 * AdminController.EXPIRE_COOKIE);
                HttpContext.Response.SetCookie(sessionFromCookie);
            }
            // перенаправление на страницу авторизации
            return Redirect("/admin/");
        }
    }
}