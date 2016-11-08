using System;
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
        public ActionResult Index()
        {
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
            // параметр сортировки заявок по статусам их заказов
            string calloutOrderStatusCriteria = Request.Params.Get("callout_order_status");
            
            if (calloutOrderStatusCriteria != null && calloutOrderStatusCriteria != "")
            {
                // запрос на получение всех заявок по статусу их заказов (вкладка Заявки)
                string status = CALLOUT_ORDER_STATUS_RUSSIFIERS[calloutOrderStatusCriteria];
                
                if (calloutOrderStatusCriteria == "pending")
                {
                    model.callouts =
                        from c in _dbContext.callouts
                        join co in _dbContext.callout_order on c.id equals co.callout_id into join_scope
                        from co in join_scope.DefaultIfEmpty()
                        where co.id == null || co.status == status
                        select c;
                }
                else
                {
                    model.callouts =
                        from c in _dbContext.callouts
                        join co in _dbContext.callout_order on c.id equals co.callout_id
                        where co.status == status
                        select c;
                }
            }
            else
            {
                model.callouts = _dbContext.callouts;
            }

            model.callouts = model.callouts.Include("callout_order");

            // получение всех сотрудников (вкладка Сотрудники)
            model.employees = _dbContext.employees
                .Include("person")
                .DefaultIfEmpty();

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