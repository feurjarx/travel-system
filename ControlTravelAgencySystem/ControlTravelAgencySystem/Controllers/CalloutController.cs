using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Common;

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

            foreach (airticket airticket in airtickets)
            {
                flight flight = airticket.flight;

                airticketsList.Add(new
                {
                    id = airticket.id,
                    created_datetime = Utils.tsToDateTime(airticket.created_at).ToString(Constants.ddMMMyyyyHmmss),
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


            List<object> transfersList = new List<object>();
            IQueryable<transfer> transfers = _dbContext.transfers
                .Include("route.airport.city")
                .Include("route.airport1.city")
                .Where(t => t.callout_id == id)
                ;
        
            foreach (transfer transfer in transfers)
            {
                route route = transfer.route;
                airport fromAirport = route.airport;
                airport toAirport = route.airport1;
                
                transfersList.Add(new
                {
                    id = transfer.id,
                    starting_date = transfer.starting_date.ToString(Constants.ddMMMyyyy),
                    created_datetime = Utils.tsToDateTime(transfer.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    payment = transfer.payment,
                    is_baggage = transfer.is_baggage,
                    is_baby = transfer.is_baby,
                    route = new
                    {
                        id = route.id,
                        type = route.type,
                        from_airport = fromAirport != null ? new {

                            id = fromAirport.id,
                            name = fromAirport.name,
                            city_name = fromAirport.city.name

                        } : null,
                        to_airport = toAirport != null ? new {

                            id = toAirport.id,
                            name = toAirport.name,
                            city_name = toAirport.city.name 

                        } : null,
                        starting_address = route.starting_address,
                        starting_time = route.starting_time.ToString(Constants.hhmmss),
                        final_address = route.final_address,
                        duration = route.duration,
                        total_seats = route.total_seats,
                        distance = route.distance,
                        cost = route.cost,
                    }
                });
            }
            
            return Json(new {
                
                airtickets = airticketsList,
                transfers = transfersList,

                is_services = 
                    airticketsList.Count() > 0
                    ||
                    transfersList.Count() > 0

            }, JsonRequestBehavior.DenyGet);
        }
    }
}