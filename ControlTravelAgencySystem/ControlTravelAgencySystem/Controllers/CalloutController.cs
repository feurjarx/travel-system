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
            callout callout =  _dbContext.callouts.Where(c => c.id == id).FirstOrDefault();
            
            List<object> airticketsList = new List<object>();
            foreach (airticket airticket in callout.airtickets)
            {
                flight flight = airticket.flight;

                airticketsList.Add(new {

                    id = airticket.id,
                    created_datetime = Utils.tsToDateTime(airticket.created_at).ToString(Constants.ddMMMyyyyHmmss),

                    flight = new {

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
            foreach (transfer transfer in callout.transfers)
            {
                route route = transfer.route;
                airport fromAirport = route.airport;
                airport toAirport = route.airport1;
                
                transfersList.Add(new {

                    id = transfer.id,
                    starting_date = transfer.starting_date.ToString(Constants.ddMMMyyyy),
                    created_datetime = Utils.tsToDateTime(transfer.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    payment = transfer.payment,
                    is_baggage = transfer.is_baggage,
                    is_baby = transfer.is_baby,

                    route = new {

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

            List<object> roomsList = new List<object>();
            foreach (callout_room calloutRoom in callout.callout_room)
            {
                room room = calloutRoom.room;
                hotel hotel = room.hotel;
                food food = hotel.food;
                city city = hotel.city;
                country country = city.country;

                roomsList.Add(new {

                    created_datetime = Utils.tsToDateTime(calloutRoom.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    start_living_datetime = Utils.tsToDateTime(calloutRoom.start_living_at).ToString(Constants.ddMMMyyyyHmmss),
                    duration = calloutRoom.duration,
                    payment = calloutRoom.payment,

                    room = new {

                        id = room.id,   
                        number = room.number,
                        @class = room.@class,
                        seats_number = room.seats_number,
                        room_size =  room.room_size,
                        description = room.description,
                        hotel = new {

                            id = hotel.id,
                            name = hotel.name,
                            stars_number = hotel.stars_number,
                            distance_to_beach = hotel.distance_to_beach,

                            food = food != null ? new {

                                id = food.id,
                                type = food.type,
                                description = food.description

                            } : null,

                            city = new {

                                id = city.id,
                                name = city.name,
                                country = new {

                                    id = country.id,
                                    name = country.name
                                }
                            }
                        }
                    }
                });
            }

            List<object> excursionOrdersList = new List<object>();
            foreach (excursion_order excursionOrder in callout.excursion_order)
            {
                excursion excursion = excursionOrder.excursion;
                excursionOrdersList.Add(new {

                    id = excursionOrder.id,
                    created_datetime = Utils.tsToDateTime(excursionOrder.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    payment = excursionOrder.payment,
                    starting_address = excursionOrder.starting_address,
                    starting_datetime = Utils.tsToDateTime(excursionOrder.starting_at).ToString(Constants.ddMMMyyyy),
                    is_baby = excursionOrder.is_baby,
                    is_privilege = excursionOrder.is_privilege,
                    is_custom = excursionOrder.is_custom,
                    bus_place_number = excursionOrder.bus_place_number,

                    excursion = new {

                        id = excursion.id,
                        name = excursion.name,
                        starting_time = excursion.starting_time != null ? ((TimeSpan)excursion.starting_time).ToString(Constants.hhmmss) : null,
                        duration = excursion.duration,
                        city_name = excursion.city != null ? excursion.city.name : null,
                        description = excursion.description
                    }
                });
            }

            List<object> hotelServiceOrdersList = new List<object>();
            foreach (hotel_service_order hotelServiceOrder in callout.hotel_service_order)
            {
                hotel_service service = hotelServiceOrder.hotel_service;
                hotelServiceOrdersList.Add(new {

                    id = hotelServiceOrder.id,
                    created_datetime = Utils.tsToDateTime(hotelServiceOrder.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    provision_datetime = Utils.tsToDateTime(hotelServiceOrder.provision_at).ToString(Constants.ddMMMyyyyHmmss),
                    payment = hotelServiceOrder.payment,
                    duration = hotelServiceOrder.duration,

                    room = hotelServiceOrder.room != null ? new {

                        id = hotelServiceOrder.room.id,
                        number = hotelServiceOrder.room.number

                    } : null,

                    hotel_service = new {

                        id = service.id,
                        hotel_name = service.hotel.name,
                        description = service.description,
                        starting_time = service.starting_time != null ? ((TimeSpan)service.starting_time).ToString(Constants.hhmmss) : null
                    }
                });
            }
            
            return Json(new {
                
                airtickets = airticketsList,
                transfers = transfersList,
                rooms = roomsList,
                excursions = excursionOrdersList,
                hotel_services = hotelServiceOrdersList,

                // далее трубопровод =D P.S. веселые приключения программиста
                is_services = 
                    airticketsList.Count() > 0
                    ||
                    transfersList.Count() > 0
                    ||
                    roomsList.Count() > 0
                    ||
                    excursionOrdersList.Count() > 0
                    ||
                    hotelServiceOrdersList.Count() > 0

            }, JsonRequestBehavior.DenyGet);
        }


        [HttpDelete]
        public JsonResult Delete()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            string calloutsIdsLine = Request.Params.Get("callouts_ids[]");
            List<int> calloutIdsList = calloutsIdsLine.Split(',').Select(int.Parse).ToList();

            try
            {
                IQueryable<callout> callouts = _dbContext.callouts
                    .Include("callout_order")
                    .Include("airtickets")
                    .Include("transfers")
                    .Include("hotel_service_order")
                    .Include("excursion_order")
                    .Include("callout_room")
                    .Where(c => calloutIdsList.Contains(c.id));

                if (callouts.Count() == calloutIdsList.Count())
                {
                    foreach (callout callout in callouts)
                    {
                        foreach (callout_order order in callout.callout_order)
                        {
                            _dbContext.callout_order.Attach(order);
                            _dbContext.callout_order.Remove(order);
                        }


                        foreach (airticket ticket in callout.airtickets)
                        {
                            _dbContext.airtickets.Attach(ticket);
                            _dbContext.airtickets.Remove(ticket);
                        }

                        foreach (transfer ticket in callout.transfers)
                        {
                            _dbContext.transfers.Attach(ticket);
                            _dbContext.transfers.Remove(ticket);
                        }
                        
                        
                        _dbContext.callouts.Attach(callout);
                        _dbContext.callouts.Remove(callout);
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    result.Add("error", "Ошибка при удалении (несоответствие)");
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