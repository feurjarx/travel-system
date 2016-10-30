using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using ControlTravelAgencySystem.Common;

namespace ControlTravelAgencySystem.Controllers
{
    /**
     * Контроллер доступа к панели администратора
     */ 
    public class AdminController : Controller
    {
        public static int EXPIRE_COOKIE = 1; // время жизни cookie в часах 

        private readonly TravelSystemEntities _dbContext;
        public AdminController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Admin
        public ActionResult Index()
        {
            AdminPanelPageView model = new AdminPanelPageView();

            // параметр доступа
            ViewBag.access = false;
            // параметр наличия ошибки
            ViewBag.error = null;
            
            model.user = new employee();

            if (HttpContext.Request.Cookies["session"] != null)
            {
                string sessionFromCookie = HttpContext.Request.Cookies["session"].Value;
                if (sessionFromCookie != null)
                {
                    employee user = _dbContext.employees.Where(e => e.session == sessionFromCookie).FirstOrDefault();
                    if (user != null)
                    {
                        // Аутентификация успешно пройдена
                        ViewBag.access = true;

                        model.user = user;
                        model.callouts = _dbContext.callouts.DefaultIfEmpty();
                        model.employees = _dbContext.employees
                            .Include("person")
                            .DefaultIfEmpty();
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

            if (user.email != null && user.password != null)
            {
                string hash = Utils.md5(user.password);
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
                    model.callouts = _dbContext.callouts.DefaultIfEmpty();
                    model.employees = _dbContext.employees
                        .Include("person")
                        .DefaultIfEmpty();
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

        public ActionResult Logout()
        {
            HttpCookie sessionFromCookie = HttpContext.Request.Cookies["session"];

            employee currentUser = _dbContext.employees
                .Where(e => e.session == sessionFromCookie.Value)
                .FirstOrDefault();

            if (currentUser != null)
            {
                currentUser.session = null;
                _dbContext.SaveChangesAsync();

                sessionFromCookie.Expires = DateTime.Now.AddHours(-1 * AdminController.EXPIRE_COOKIE);
                HttpContext.Response.SetCookie(sessionFromCookie);
            }
            
            return Redirect("/admin/");
        }
    }
}