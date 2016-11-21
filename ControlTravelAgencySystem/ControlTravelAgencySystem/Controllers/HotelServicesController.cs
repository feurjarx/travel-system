using ControlTravelAgencySystem.Common;
using ControlTravelAgencySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class HotelServicesController : Controller
    {
        private readonly TravelSystemEntities _dbContext;

        public HotelServicesController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                hotel_service hotelService = new hotel_service();
                serializeToModel(ref hotelService);
                _dbContext.hotel_service.Add(hotelService);
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
                hotel_service hotelService = _dbContext.hotel_service.Find(id);
                serializeToModel(ref hotelService);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref hotel_service hotelService)
        {
            if (Request.Form["hotel_id"] == "")
            {
                hotelService.hotel_id = null;
            }
            else
            {
                hotelService.hotel_id = int.Parse(Request.Form["hotel_id"]);
            }

            if (Request.Form["starting_time"] == "")
            {
                hotelService.starting_time = null;
            }
            else
            {
                hotelService.starting_time = TimeSpan.Parse(Request.Form["starting_time"]);
            }
            
            hotelService.description = Request.Form["description"];
            hotelService.cost_per_min = int.Parse(Request.Form["cost_per_min"]);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                hotel_service hotelService = _dbContext.hotel_service.Find(id);
                if (hotelService != null)
                {

                    if (hotelService.hotel_service_order.Count() > 0)
                    {
                        int hotelServiceOrdersBlockedCounter = 0;
                        List<hotel_service_order> hotelServiceOrderList = hotelService.hotel_service_order.ToList();
                        foreach (hotel_service_order hso in hotelServiceOrderList)
                        {
                            if (Utils.dtToTimestamp(DateTime.Now) > hso.provision_at)
                            {
                                _dbContext.hotel_service_order.Remove(hso);
                            }
                            else
                            {
                                hotelServiceOrdersBlockedCounter++;
                            }
                        }

                        if (hotelServiceOrdersBlockedCounter == 0)
                        {
                            _dbContext.hotel_service.Remove(hotelService);
                        }
                        else
                        {
                            result.Add("error", "На данный момент услуга заказана. Удаление недопустимо");
                        }
                    }
                    else
                    {
                        _dbContext.hotel_service.Remove(hotelService);
                    }
                    
                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Услуга не найдена");
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