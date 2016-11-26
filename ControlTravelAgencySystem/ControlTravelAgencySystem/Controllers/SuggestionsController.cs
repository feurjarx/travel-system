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