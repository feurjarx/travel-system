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
        private readonly TravelSystemEntities _dbContext;
        public AdminController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Admin
        public ActionResult Index()
        {
            // параметр доступа
            ViewBag.access = false;
            // параметр наличия ошибки
            ViewBag.error = null;
            
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
                        return View(user);
                    }
                }
            }
            
            // Пользователь не авторизован
            return View(new employee());
        }

        [HttpPost]
        public ActionResult Index(employee user)
        {
            ViewBag.error = null;
            ViewBag.access = false;

            ErrorView error = new ErrorView();

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
                    cookie.Expires = DateTime.Now.AddHours(1);
                    HttpContext.Response.Cookies.Add(cookie);
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
            return View(user);
        }
    }
}