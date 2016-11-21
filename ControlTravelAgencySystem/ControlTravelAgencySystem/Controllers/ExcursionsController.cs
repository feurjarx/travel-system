using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Common;

namespace ControlTravelAgencySystem.Controllers
{
    public class ExcursionsController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public ExcursionsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                excursion excursion = new excursion();
                serializeToModel(ref excursion);
                _dbContext.excursions.Add(excursion);
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
                excursion excursion = _dbContext.excursions.Find(id);
                serializeToModel(ref excursion);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref excursion excursion)
        {
            if (Request.Form["starting_time"] == "")
            {
                excursion.starting_time = null;
            }
            else
            {
                excursion.starting_time = TimeSpan.Parse(Request.Form["starting_time"]);
            }

            if (Request.Form["city_id"] == "")
            {
                excursion.city_id = null;
            }
            else
            {
                excursion.city_id = int.Parse(Request.Form["city_id"]);
            }

            excursion.name = Request.Form["name"];
            excursion.cost = int.Parse(Request.Form["cost"]);
            excursion.description = Request.Form["description"];
            excursion.duration = int.Parse(Request.Form["duration"]);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                excursion excursion = _dbContext.excursions.Find(id);
                if (excursion != null)
                {

                    if (excursion.excursion_order.Count() > 0)
                    {
                        int excursionOrdersBlockedCounter = 0;
                        List<excursion_order> excursionOrderList = excursion.excursion_order.ToList();
                        foreach (excursion_order eo in excursionOrderList)
                        {
                            if (Utils.dtToTimestamp(DateTime.Now) > eo.starting_at)
                            {
                                _dbContext.excursion_order.Remove(eo);
                            }
                            else
                            {
                                excursionOrdersBlockedCounter++;
                            }
                        }

                        if (excursionOrdersBlockedCounter == 0)
                        {
                            _dbContext.excursions.Remove(excursion);
                        }
                        else
                        {
                            result.Add("error", "На данный момент экскурсия заказана. Удаление недопустимо");
                        }
                    }
                    else
                    {
                        _dbContext.excursions.Remove(excursion);
                    }

                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Экскурсия не найдена");
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