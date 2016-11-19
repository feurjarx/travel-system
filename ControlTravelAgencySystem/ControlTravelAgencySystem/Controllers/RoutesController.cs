using ControlTravelAgencySystem.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class RoutesController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public RoutesController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                route route = new route();
                serializeToModel(ref route);
                _dbContext.routes.Add(route);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Edit(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                route route = _dbContext.routes.Find(id);
                serializeToModel(ref route);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref route route)
        {
            if (Request.Form["from_airport_id"] == "")
            {
                route.from_airport_id = null;
            }
            else
            {
                route.from_airport_id = int.Parse(Request.Form["from_airport_id"]);
            }

            if (Request.Form["to_airport_id"] == "")
            {
                route.to_airport_id = null;
            }
            else
            {
                route.to_airport_id = int.Parse(Request.Form["to_airport_id"]);
            }

            if (Request.Form["starting_address"] == "")
            {
                route.starting_address = null;
            }
            else
            {
                route.starting_address = Request.Form["starting_address"];
            }

            if (Request.Form["distance"] == "")
            {
                route.distance = null;
            }
            else
            {
                route.distance = int.Parse(Request.Form["distance"]);
            }

            route.type = Request.Form["type"];
            route.starting_time = TimeSpan.Parse(Request.Form["starting_time"]);
            route.final_address = Request.Form["final_address"];
            route.duration = int.Parse(Request.Form["duration"]);
            route.total_seats = int.Parse(Request.Form["total_seats"]);
            route.cost = int.Parse(Request.Form["cost"]);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                route route = _dbContext.routes.Find(id);
                if (route != null)
                {
                    _dbContext.routes.Remove(route);
                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Маршрут не найден");
                }
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}