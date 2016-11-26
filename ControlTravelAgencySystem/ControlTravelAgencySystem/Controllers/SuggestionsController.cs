using ControlTravelAgencySystem.Common;
using ControlTravelAgencySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public SuggestionsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                switch(Request["entity"])
                {
                    case "callout_room":

                        callout_room calloutRoom = new callout_room();
                        serializeToCalloutRoomModel(ref calloutRoom);
                        _dbContext.callout_room.Add(calloutRoom);
                        _dbContext.SaveChanges();
                        break;

                    case "airticket":

                        airticket airticket = new airticket();
                        serializeToAirticketModel(ref airticket);
                        _dbContext.airtickets.Add(airticket);
                        _dbContext.SaveChanges();
                        break;

                    case "transfer":

                        transfer transfer = new transfer();
                        serializeToTransferModel(ref transfer);
                        _dbContext.transfers.Add(transfer);
                        _dbContext.SaveChanges();
                        break;

                    case "excursion_order":

                        excursion_order excursionOrder = new excursion_order();
                        serializeToExcursionOrderModel(ref excursionOrder);
                        _dbContext.excursion_order.Add(excursionOrder);
                        _dbContext.SaveChanges();
                        break;

                    case "hotel_service_order":

                        hotel_service_order hotelServiceOrder = new hotel_service_order();
                        serializeToHotelServiceOrderModel(ref hotelServiceOrder);
                        _dbContext.hotel_service_order.Add(hotelServiceOrder);
                        _dbContext.SaveChanges();
                        break;

                    default:
                        throw new Exception("Error! Unexcepted entity");
                }
                
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
                switch (Request["entity"])
                {
                    case "callout_room":

                        callout_room calloutRoom = _dbContext.callout_room.Find(id);
                        serializeToCalloutRoomModel(ref calloutRoom);
                        _dbContext.SaveChanges();
                        break;

                    case "airticket":

                        airticket airticket = _dbContext.airtickets.Find(id);
                        serializeToAirticketModel(ref airticket);
                        _dbContext.SaveChanges();
                        break;

                    case "transfer":

                        transfer transfer = _dbContext.transfers.Find(id);
                        serializeToTransferModel(ref transfer);
                        _dbContext.SaveChanges();
                        break;

                    case "excursion_order":

                        excursion_order excursionOrder = _dbContext.excursion_order.Find(id);
                        serializeToExcursionOrderModel(ref excursionOrder);
                        _dbContext.SaveChanges();
                        break;

                    case "hotel_service_order":

                        hotel_service_order hotelServiceOrder = _dbContext.hotel_service_order.Find(id);
                        serializeToHotelServiceOrderModel(ref hotelServiceOrder);
                        _dbContext.SaveChanges();
                        break;

                    default:
                        throw new Exception("Error! Unexcepted entity");
                }

                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        
        private void serializeToCalloutRoomModel(ref callout_room calloutRoom)
        {
            calloutRoom.room_id = int.Parse(Request.Form["room_id"]);
            calloutRoom.start_living_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["start_living_at"]));
            calloutRoom.duration = int.Parse(Request.Form["duration"]);
            calloutRoom.created_at = Utils.dtToTimestamp(DateTime.Now);
            calloutRoom.callout_id = int.Parse(Request.Form["callout_id"]);

            room room = _dbContext.rooms.Find(calloutRoom.room_id);
            calloutRoom.payment = room.cost_per_day * (calloutRoom.duration / 24);
        }

        private void serializeToAirticketModel(ref airticket airticket)
        {
            flight flight = null;
            if (Request.Form["flight_id"] != "")
            {
                airticket.flight_id = int.Parse(Request.Form["flight_id"]);
                flight = _dbContext.flights.Find(airticket.flight_id);
            }
            else
            {
                airticket.flight_id = null;
            }

            airticket.departure_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["departure_at"]));
            airticket.created_at = Utils.dtToTimestamp(DateTime.Now);

            airticket.callout_id = int.Parse(Request.Form["callout_id"]);

            airticket.payment = 0;
            if (flight != null)
            {
                airticket.payment = flight.cost;
            }
        }

        private void serializeToTransferModel(ref transfer transfer)
        {
            route route = null;
            if (Request.Form["route_id"] != "")
            {
                transfer.route_id = int.Parse(Request.Form["route_id"]);
                route = _dbContext.routes.Find(transfer.route_id);
            }
            else
            {
                transfer.route_id = null;
            }

            transfer.starting_date = DateTime.Parse(Request.Form["starting_date"]);
            transfer.created_at = Utils.dtToTimestamp(DateTime.Now);
            
            transfer.callout_id = int.Parse(Request.Form["callout_id"]);

            transfer.payment = 0;
            if (route != null)
            {
                transfer.payment = route.cost;
            }
        }

        private void serializeToExcursionOrderModel(ref excursion_order excursionOrder)
        {
            excursionOrder.excursion_id = int.Parse(Request.Form["excursion_id"]);
            excursionOrder.starting_address = Request.Form["starting_address"];
            excursionOrder.starting_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["starting_at"]));
            excursionOrder.created_at = Utils.dtToTimestamp(DateTime.Now);
            excursionOrder.callout_id = int.Parse(Request.Form["callout_id"]);
            
            excursion excursion = _dbContext.excursions.Find(excursionOrder.excursion_id);
            excursionOrder.payment = excursion.cost;
        }

        private void serializeToHotelServiceOrderModel(ref hotel_service_order hotelServiceOrder)
        {
            hotelServiceOrder.hotel_service_id = int.Parse(Request.Form["hotel_service_id"]);
            hotelServiceOrder.duration = int.Parse(Request.Form["duration"]);
            hotelServiceOrder.provision_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["provision_at"]));
            hotelServiceOrder.created_at = Utils.dtToTimestamp(DateTime.Now);
            hotelServiceOrder.callout_id = int.Parse(Request.Form["callout_id"]);

            hotel_service hotelService = _dbContext.hotel_service.Find(hotelServiceOrder.hotel_service_id);
            hotelServiceOrder.payment = hotelService.cost_per_min * hotelServiceOrder.duration;
        }

        [HttpDelete]
        public JsonResult Delete(int id, string type)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                switch (type)
                {
                    case "callout_room":

                        callout_room calloutRoom = _dbContext.callout_room.Find(id);
                        _dbContext.callout_room.Remove(calloutRoom);
                        _dbContext.SaveChanges();

                        break;

                    case "airticket":

                        airticket airticket = _dbContext.airtickets.Find(id);
                        _dbContext.airtickets.Remove(airticket);
                        _dbContext.SaveChanges();
                        break;

                    case "transfer":

                        transfer transfer = _dbContext.transfers.Find(id);
                        _dbContext.transfers.Remove(transfer);
                        _dbContext.SaveChanges();
                        break;

                    case "excursion_order":

                        excursion_order excursionOrder = _dbContext.excursion_order.Find(id);
                        _dbContext.excursion_order.Remove(excursionOrder);
                        _dbContext.SaveChanges();
                        break;

                    case "hotel_service_order":

                        hotel_service_order hotelServiceOrder = _dbContext.hotel_service_order.Find(id);
                        _dbContext.hotel_service_order.Remove(hotelServiceOrder);
                        _dbContext.SaveChanges();
                        break;

                    default:
                        throw new Exception("Error! Unexcepted entity");
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