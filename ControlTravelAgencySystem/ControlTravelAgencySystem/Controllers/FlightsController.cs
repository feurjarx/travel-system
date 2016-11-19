using ControlTravelAgencySystem.Common;
using ControlTravelAgencySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class FlightsController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public FlightsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                flight flight = new flight();
                serializeToModel(ref flight);
                _dbContext.flights.Add(flight);
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
                flight flight = _dbContext.flights.Find(id);
                serializeToModel(ref flight);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref flight flight)
        {
            if (Request.Form["airline_id"] == "")
            {
                flight.airline_id = null;
            }
            else
            {
                flight.airline_id = int.Parse(Request.Form["airline_id"]);
            }

            flight.code = Request.Form["code"];
            flight.from_airport_id = int.Parse(Request.Form["from_airport_id"]);
            flight.to_airport_id = int.Parse(Request.Form["to_airport_id"]);
            flight.flight_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["flight_at"]));
            flight.duration = int.Parse(Request.Form["duration"]);
            flight.cost = int.Parse(Request.Form["cost"]);
            flight.total_seats = int.Parse(Request.Form["total_seats"]);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                flight flight = _dbContext.flights.Find(id);
                if (flight != null)
                {
                    _dbContext.flights.Remove(flight);
                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Авиарейс не найден");
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