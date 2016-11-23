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

        /// <summary>
        /// Получение данных о желаниях
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Suggestions(int id)
        {
            // С вкладки Заявки Админки при клике на заявку сюда приходит id выбранной заявки
            callout callout = _dbContext.callouts.Where(c => c.id == id).FirstOrDefault();

            // Сбор данных о авиабилетах
            List<object> airticketsList = new List<object>();
            foreach (airticket airticket in callout.airtickets)
            {
                flight flight = airticket.flight;

                double payment = flight.cost;
                if (airticket.is_baggage == 1)
                {
                    payment += payment * 0.1;
                }

                if (airticket.is_baby == 1)
                {
                    payment = 0;
                }

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

                    payment = airticket.payment == 0 ? payment : airticket.payment,
                    is_baggage = airticket.is_baggage,
                    is_baby = airticket.is_baby
                });
            }

            // Сбор данных о трансферах
            List<object> transfersList = new List<object>();
            foreach (transfer transfer in callout.transfers)
            {
                route route = transfer.route;

                airport fromAirport = null;
                airport toAirport = null;
                if (route != null)
                {
                    fromAirport = route.airport;
                    toAirport = route.airport1;
                }

                double payment = route.cost;
                if (transfer.is_baggage == 1)
                {
                    payment += payment * 0.1;
                }

                if (transfer.is_baby == 1)
                {
                    payment = 0;
                }

                transfersList.Add(new
                {

                    id = transfer.id,
                    starting_date = transfer.starting_date.ToString(Constants.ddMMMyyyy),
                    created_datetime = Utils.tsToDateTime(transfer.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    is_baggage = transfer.is_baggage,
                    is_baby = transfer.is_baby,

                    payment = transfer.payment == 0 ? payment : transfer.payment,

                    route = new
                    {
                        id = route.id,
                        type = route.type,

                        from_airport = fromAirport != null ? new
                        {

                            id = fromAirport.id,
                            name = fromAirport.name,
                            city_name = fromAirport.city.name

                        } : null,

                        to_airport = toAirport != null ? new
                        {

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

            // Сбор данных о желаемых комнатах
            List<object> roomsList = new List<object>();
            foreach (callout_room calloutRoom in callout.callout_room)
            {
                room room = calloutRoom.room;
                hotel hotel = room.hotel;
                food food = hotel.food;
                city city = hotel.city;
                country country = city.country;

                int numberNights = calloutRoom.duration / 24;
                double payment = calloutRoom.room.cost_per_day * numberNights;

                roomsList.Add(new
                {

                    created_datetime = Utils.tsToDateTime(calloutRoom.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    start_living_datetime = Utils.tsToDateTime(calloutRoom.start_living_at).ToString(Constants.ddMMMyyyyHmmss),
                    duration = calloutRoom.duration,
                    payment = calloutRoom.payment == 0 ? payment : calloutRoom.payment,

                    room = new
                    {

                        id = room.id,
                        number = room.number,
                        @class = room.type,
                        seats_number = room.seats_number,
                        room_size = room.room_size,
                        description = room.description,
                        hotel = new
                        {

                            id = hotel.id,
                            name = hotel.name,
                            stars_number = hotel.stars_number,
                            distance_to_beach = hotel.distance_to_beach,

                            food = food != null ? new
                            {

                                id = food.id,
                                type = food.type,
                                description = food.description

                            } : null,

                            city = new
                            {

                                id = city.id,
                                name = city.name,
                                country = new
                                {

                                    id = country.id,
                                    name = country.name
                                }
                            }
                        }
                    }
                });
            }

            // Сбор данных об экскурсиях в заявке
            List<object> excursionOrdersList = new List<object>();
            foreach (excursion_order excursionOrder in callout.excursion_order)
            {
                excursion excursion = excursionOrder.excursion;

                double payment = excursion.cost;

                if (excursionOrder.is_privilege == 1)
                {
                    payment -= payment * 0.5;
                }

                if (excursionOrder.is_baby == 1)
                {
                    payment = 0;
                }

                excursionOrdersList.Add(new
                {

                    id = excursionOrder.id,
                    created_datetime = Utils.tsToDateTime(excursionOrder.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    starting_address = excursionOrder.starting_address,
                    starting_datetime = Utils.tsToDateTime(excursionOrder.starting_at).ToString(Constants.ddMMMyyyy),
                    is_baby = excursionOrder.is_baby,
                    is_privilege = excursionOrder.is_privilege,
                    is_custom = excursionOrder.is_custom,
                    bus_place_number = excursionOrder.bus_place_number,
                    payment = excursionOrder.payment == 0 ? payment : excursionOrder.payment,

                    excursion = new
                    {

                        id = excursion.id,
                        name = excursion.name,
                        starting_time = excursion.starting_time != null ? ((TimeSpan)excursion.starting_time).ToString(Constants.hhmmss) : null,
                        duration = excursion.duration,
                        city_name = excursion.city != null ? excursion.city.name : null,
                        description = excursion.description
                    }
                });
            }

            // Сбор данных о платных услугах в заявке
            List<object> hotelServiceOrdersList = new List<object>();
            foreach (hotel_service_order hotelServiceOrder in callout.hotel_service_order)
            {
                hotel_service service = hotelServiceOrder.hotel_service;

                double payment = service.cost_per_min * hotelServiceOrder.duration;

                hotelServiceOrdersList.Add(new
                {

                    id = hotelServiceOrder.id,
                    created_datetime = Utils.tsToDateTime(hotelServiceOrder.created_at).ToString(Constants.ddMMMyyyyHmmss),
                    provision_datetime = Utils.tsToDateTime(hotelServiceOrder.provision_at).ToString(Constants.ddMMMyyyyHmmss),
                    duration = hotelServiceOrder.duration,
                    payment = hotelServiceOrder.payment == 0 ? payment : hotelServiceOrder.payment,

                    room = hotelServiceOrder.room != null ? new
                    {

                        id = hotelServiceOrder.room.id,
                        number = hotelServiceOrder.room.number

                    } : null,

                    hotel_service = new
                    {

                        id = service.id,
                        hotel_name = service.hotel.name,
                        description = service.description,
                        starting_time = service.starting_time != null ? ((TimeSpan)service.starting_time).ToString(Constants.hhmmss) : null
                    }
                });
            }

            // возврат данных на фронтэнд (клиенту)
            return Json(new
            {

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


        /// <summary>
        /// Удаление выбранной заявки (вкладка заявки)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult Delete()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            // Получение всех присланных id заявок для удаления
            string calloutsIdsLine = Request.Params.Get("callouts_ids[]");
            List<int> calloutIdsList = calloutsIdsLine.Split(',').Select(int.Parse).ToList();

            try
            {
                // Жадная подгрузка всех заявок, айди которых есть в листе
                List<callout> callouts = _dbContext.callouts
                    .Include("callout_order")
                    .Include("airtickets")
                    .Include("transfers")
                    .Include("hotel_service_order")
                    .Include("excursion_order")
                    .Include("callout_room")
                    .Where(c => calloutIdsList.Contains(c.id))
                    .ToList<callout>();

                // проверка на актуальность присланных идентификаторов
                if (callouts.Count() == calloutIdsList.Count())
                {
                    // перебор удаляемых заявко в цикле
                    foreach (callout callout in callouts)
                    {
                        // удаление всех заказов текущей заявки (т.к. связь заказ - заявка RESTRICT)
                        List<callout_order> orders = callout.callout_order.ToList<callout_order>();
                        foreach (callout_order order in orders)
                        {
                            _dbContext.callout_order.Remove(order);
                        }

                        // удаление всех авиабилетов текущей заявки (т.к. связь авиабилет - заявка RESTRICT)
                        List<airticket> airtickets = callout.airtickets.ToList<airticket>();
                        foreach (airticket ticket in airtickets)
                        {
                            _dbContext.airtickets.Remove(ticket);
                        }

                        // удаление всех трансферов текущей заявки (т.к. связь транфсер - заявка RESTRICT)
                        List<transfer> transfers = callout.transfers.ToList<transfer>();
                        foreach (transfer ticket in transfers)
                        {
                            _dbContext.transfers.Remove(ticket);
                        }

                        // остальные связи автоматически удаляться (т.к. остальные связи с заявкой CASKADE)
                        // после удаление крепких связей ранее удаление самой заявки 
                        _dbContext.callouts.Remove(callout);
                    }
                    // осуществление транзакции 
                    _dbContext.SaveChanges();
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

            // возврат результат операции
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        
        [HttpGet]
        public JsonResult PredefinedCallouts()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            IQueryable<callout> qb = _dbContext.callouts
                .Where(c => c.is_predefined == 1);
            
            if (Request["need"] == "total")
            {
                result.Add("total", qb.Count());
            }
            else
            {
                result.Add("callouts", new List<object>());

                int pageSize = int.Parse(Request["pageSize"]);
                int pageNumber = int.Parse(Request["pageNumber"]);
                int offset = (pageNumber - 1) * pageSize;
                
                List<callout> callouts = qb
                    .OrderByDescending(c => c.created_at)
                    .Skip(offset)
                    .ToList();

                foreach (callout c in callouts)
                {
                    ((List<object>)result["callouts"]).Add(Utils.toJsonByCustomProperties(c, Scheme.predefinedCallout));
                }
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}