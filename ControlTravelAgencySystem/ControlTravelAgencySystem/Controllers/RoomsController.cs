using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
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

        /// <summary>
        /// GET: /
        /// </summary>
        public ActionResult FavoriteList()
        {
            var list = Session["selected-check"] as List<int>;

            var viewModel = new FavotiteListView();

            foreach (var rId in list)
            {
                var room = _dbContext.rooms
                    .First(x => x.id == rId);
                var city = room.hotel.city;
                var flight = _dbContext.flights
                    .Include("airport")
                    .FirstOrDefault(x => x.airport.city_id == city.id);

                viewModel.FavotiteListViewItems.Add(new FavotiteListView.FavotiteListViewItem
                {
                    SelectedRoom = room,
                    FlightId = flight.id,
                    Code = flight.code,
                    FromAirport = _dbContext.airports.FirstOrDefault(x => x.id == flight.from_airport_id),
                    ToAirport = _dbContext.airports.FirstOrDefault(x => x.id == flight.to_airport_id),
                    FlightAt = flight.flight_at,
                    Duration = flight.duration
                });
            }

            return View(viewModel);
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
                        Class = room.@class,
                        SeatsNumber = room.seats_number,
                        RoomSize = room.room_size ?? 0,
                        Description = room.description
                    });
            }

            return viewModel;
        }

        [HttpPost]
        public ActionResult CalloutCreate(CalloutForm calloutForm)
        {
            _dbContext.callouts.Add(
                new callout
                {
                    fullname = calloutForm.Fullname,
                    email = calloutForm.Email,
                    phone = calloutForm.Phone
                });

            _dbContext.SaveChanges();

            var callout = _dbContext.callouts.Last();

            var rooms = _dbContext.rooms;
            var list = Session["selected-check"] as List<int>;

            foreach (var item in list)
            {
                _dbContext.callout_room.Add(
                    new callout_room
                    {
                        callout_id = callout.id,
                        room_id = item
                    });

                _dbContext.SaveChanges();
            }

            Session["selected-check"] = null;

            return View();
        }

        public class CalloutForm
        {
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }
    }
}