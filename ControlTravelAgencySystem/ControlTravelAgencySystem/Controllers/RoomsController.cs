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
    }
}