using ControlTravelAgencySystem.Common;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    /// <summary>
    /// Контроллер номеров
    /// </summary>
    public class RoomsController : Controller
    {
        // Хранилище данных БД
        private readonly TravelSystemEntities _dbContext;

        /// <summary>
        /// Инициализация
        /// </summary>
        public RoomsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public JsonResult RoomChecked(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("fail");

            var parId = int.Parse(id);

            if (Session["selected-check"] == null)
                Session["selected-check"] = new List<int>();

            var list = Session["selected-check"] as List<int>;

            if (list.Any(x => x == parId))
            {
                list.Remove(parId);
            }
            else
            {
                list.Add(parId);
            }

            return Json("ok");
        }

        [HttpPost]
        public JsonResult FlightChecked(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("fail");

            var parId = int.Parse(id);

            if (Session["flight-check"] == null)
                Session["flight-check"] = new List<int>();

            var list = Session["flight-check"] as List<int>;

            if (list.Any(x => x == parId))
                list.Remove(parId);
            else
                list.Add(parId);

            return Json("ok");
        }

        [HttpPost]
        public JsonResult RouteChecked(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json("fail");

            var parId = int.Parse(id);

            if (Session["route-check"] == null)
                Session["route-check"] = new List<int>();

            var list = Session["route-check"] as List<int>;

            if (list.Any(x => x == parId))
                list.Remove(parId);
            else
                list.Add(parId);

            return Json("ok");
        }

        /// <summary>
        /// GET: /
        /// </summary>
        public ActionResult FavoriteList()
        {
            var list = Session["selected-check"] as List<int>;
            var list2 = Session["flight-check"] as List<int>;

            var viewModel = new FavotiteListView();

            if (list == null || list?.Count == 0)
                return Redirect("/Home/Index");

            foreach (var rId in list)
            {
                var room = _dbContext.rooms
                    .First(x => x.id == rId);

                var city = room.hotel.city;

                var flight = _dbContext.flights
                    .Include("airport")
                    .FirstOrDefault(x => x.airport.city_id == city.id);

                var isChecked = false;

                // переписать говнокод
                if (list2 != null)
                    if (list2.Any(x => x == flight.id))
                        isChecked = true;

                viewModel.FavotiteListViewItems.Add(new FavotiteListView.FavotiteListViewItem
                {
                    IsChecked = isChecked,
                    SelectedRoom = room,
                    FlightId = flight.id,
                    Code = flight.code,
                    FromAirport = _dbContext.airports.FirstOrDefault(x => x.id == flight.from_airport_id),
                    ToAirport = _dbContext.airports.FirstOrDefault(x => x.id == flight.to_airport_id),
                    FlightAt = Utils.tsToDateTime(flight.flight_at),
                    Duration = flight.duration,
                    AirlineName = flight.airline.name
                });
            }

            return View(viewModel);
        }

        public PartialViewResult GetRoutesList(int id)
        {
            var viewModel = new RoutesView();

            var routes = _dbContext.routes
                .Include("airport1");

            var list = Session["route-check"] as List<int>;

            foreach (var route in routes)
            {
                var isChecked = false;

                // переписать говнокод
                if (list != null)
                    if (list.Any(x => x == route.id))
                        isChecked = true;

                viewModel.RoutesViewItems.Add(
                    new RoutesView.RoutesViewItem
                    {
                        IsChecked = isChecked,
                        RouteId = route.id,
                        Type = route.type,
                        FromAirport = route.airport?.name,
                        ToAirport = route.airport1?.name,
                        StartingAddress = route.starting_address,
                        StartingTime = route.starting_time,
                        FinalAddress = route.final_address,
                        Duration = route.duration,
                        TotalSeats = route.total_seats,
                        Distance = route.distance ?? 0,
                        Cost = route.cost
                    });
            }

            return PartialView(viewModel);
        }


        /// <summary>
        /// Представление списка номеров выбранного отеля
        /// </summary>
        /// <param name="id">ID отеля</param>
        public PartialViewResult GetRoomsList(int id)
        {
            var viewModel = GetRoomsViewModel(id);
            return PartialView(viewModel);
        }

        /// <summary>
        /// Возвращает модель-представление списка номеров
        /// </summary>
        public RoomsView GetRoomsViewModel(int id)
        {
            var viewModel = new RoomsView();

            var rooms = _dbContext.rooms
                .Include("hotel")
                .Where(x => x.hotel_id == id);

            var list = Session["selected-check"] as List<int>;

            foreach (var room in rooms)
            {
                var isChecked = false;

                // переписать говнокод
                if (list != null)
                    if (list.Any(x => x == room.id))
                        isChecked = true;

                viewModel.RoomViewItems.Add(
                    new RoomsView.RoomViewItem
                    {
                        IsChecked = isChecked,
                        RoomId = room.id,
                        RoomNumber = room.number,
                        CostPerDay = room.cost_per_day,
                        Class = room.type,
                        SeatsNumber = room.seats_number,
                        RoomSize = room.room_size ?? 0,
                        Description = room.description
                    });
            }

            return viewModel;
        }

        [HttpPost]
        public ActionResult CalloutCreate(FavotiteListView favListView)
        {
            _dbContext.callouts.Add(
                new callout
                {
                    fullname = favListView.Fullname,
                    email = favListView.Email,
                    phone = favListView.Phone,
                    created_at = Utils.dtToTimestamp(DateTime.Now)
                });

            _dbContext.SaveChanges();

            var callout = _dbContext.callouts.ToList().Last();
            
            var rooms = _dbContext.rooms;
            var list = Session["selected-check"] as List<int>;

            if (list != null)
                foreach (var item in list)
                {
                    _dbContext.callout_room.Add(
                        new callout_room
                        {
                            callout_id = callout.id,
                            room_id = item,
                            created_at = Utils.dtToTimestamp(DateTime.Now)
                        });

                    _dbContext.SaveChanges();
                }

            Session["selected-check"] = null;

            var list2 = Session["flight-check"] as List<int>;

            if (list2 != null)
                foreach (var item in list2)
                {
                    _dbContext.airtickets.Add(
                        new airticket
                        {
                            callout_id = callout.id,
                            flight_id = item,
                            created_at = Utils.dtToTimestamp(DateTime.Now)
                        });

                    _dbContext.SaveChanges();
                }

            Session["flight-check"] = null;

            var list3 = Session["route-check"] as List<int>;

            if (list3 != null)
                foreach (var item in list3)
                {
                    _dbContext.transfers.Add(
                        new transfer
                        {
                            callout_id = callout.id,
                            route_id = item,
                            created_at = Utils.dtToTimestamp(DateTime.Now)
                        });

                    _dbContext.SaveChanges();
                }

            Session["route-check"] = null;

            _dbContext.SaveChanges();

            return View();
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                room room = new room();
                serializeToModel(ref room);
                _dbContext.rooms.Add(room);
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
                room room = _dbContext.rooms.Find(id);
                serializeToModel(ref room);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref room room)
        {
            room.number = Request.Form["number"];
            room.hotel_id = int.Parse(Request.Form["hotel_id"]);
            room.cost_per_day = int.Parse(Request.Form["cost_per_day"]);
            room.type = Request.Form["type"];
            room.seats_number = int.Parse(Request.Form["seats_number"]);

            if (Request.Form["room_size"] == "")
            {
                room.room_size = null;
            }
            else
            {
                room.room_size = int.Parse(Request.Form["room_size"]);
            }

            if (Request.Form["description"] == "")
            {
                room.description = null;
            }
            else
            {
                room.description = Request.Form["description"];
            }
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                room room = _dbContext.rooms.Find(id);
                if (room != null)
                {
                    if (room.callout_room.Count() > 0)
                    {
                        int calloutRoomsBlockedCounter = 0;
                        List<callout_room> calloutRoomList = room.callout_room.ToList();
                        foreach (callout_room cr in calloutRoomList)
                        {
                            if (Utils.dtToTimestamp(DateTime.Now) > (cr.start_living_at + cr.duration))
                            {
                                _dbContext.callout_room.Remove(cr);
                            }
                            else
                            {
                                calloutRoomsBlockedCounter++;
                            }
                        }

                        if (calloutRoomsBlockedCounter == 0)
                        {
                            _dbContext.rooms.Remove(room);
                            
                        }
                        else
                        {
                            result.Add("error", "На данный момент номер забронирован. Удаление недопустимо");
                        }
                        
                    }
                    else
                    {
                        _dbContext.rooms.Remove(room);
                    }

                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Номер не найден");
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