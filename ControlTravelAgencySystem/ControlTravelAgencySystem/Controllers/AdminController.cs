using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;

namespace ControlTravelAgencySystem.Controllers
{
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
            ViewBag.callouts = _dbContext.callouts.ToList();
            

            return View();
        }
    }
}