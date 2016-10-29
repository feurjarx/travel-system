using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;

namespace ControlTravelAgencySystem.Controllers
{
    public class CalloutController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public CalloutController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpPost]
        public JsonResult Suggestions(int id)
        {
            List<object> airticketsList = new List<object>();
            
            IQueryable<airticket> airtickets = _dbContext.airtickets
                .Include("flight.airline")
                .Where(a => a.callout_id == id)
                ;

            if (airtickets.Count() > 0)
            {
                foreach (airticket airticket in airtickets)
                {
                    flight flight = airticket.flight;

                    airticketsList.Add(new
                    {
                        id = airticket.id,
                        flight = new
                        {
                            id = flight.id,
                            airline_name = flight.airline.name,
                            code = flight.code,
                            duration = flight.duration,
                            cost = flight.cost,
                            total_seats = flight.total_seats
                        },
                        payment = airticket.payment,
                        is_baggage = airticket.is_baggage,
                        is_baby = airticket.is_baby
                    });
                }
            }
            
            return Json(new {

                airtickets = airticketsList

            }, JsonRequestBehavior.DenyGet);
        }
    }
}