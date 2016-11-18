using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    /// <summary>
    /// Контроллер отелей
    /// </summary>
    public class HotelsController : Controller
    {
        // Хранилище данных БД
        private readonly TravelSystemEntities _dbContext;

        /// <summary>
        /// Инициализация
        /// </summary>
        public HotelsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Представление списка отелей выбранного тура
        /// </summary>
        /// <param name="id">ID тура</param>
        public PartialViewResult GetHotelsList(int? id)
        {
            var viewModel = GetHotelsViewModel(id);
            return PartialView(viewModel);
        }

        /// <summary>
        /// Возвращает модель-представление списка отелей
        /// </summary>
        private HotelsView GetHotelsViewModel(int? id)
        {
            var viewModel = new HotelsView();

            var rooms = _dbContext.rooms
                .ToList();

            var hotels = _dbContext.hotels
                .Include("food")
                .Include("city")
                .Include("tour")
                .Where(x => x.tour_id == id);

            foreach (var hotel in hotels)
            {
                var averagePrice = 0;
                var count = 0;

                foreach (var room in rooms)
                    if (room.hotel_id == hotel.id)
                    {
                        averagePrice += room.cost_per_day;
                        count++;
                    }

                averagePrice /= count;

                viewModel.HotelViewItems.Add(
                    new HotelsView.HotelViewItem
                    {
                        HotelId = hotel.id,
                        HotelName = hotel.name,
                        HotelAddress = hotel.address,
                        StarsNumber = hotel.stars_number,
                        DistanceToBeach = hotel.distance_to_beach ?? 0,
                        FoodType = hotel.food?.type,
                        FoodDescription = hotel.food?.description,
                        HowManyInDay = hotel.food?.how_many_in_day ?? 0,
                        CityName = hotel.city?.name,
                        AveragePrice = averagePrice
                    });
            }

            return viewModel;
        }

        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                hotel hotel = new hotel();
                serializeToModel(ref hotel);
                _dbContext.hotels.Add(hotel);
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
                hotel hotel = _dbContext.hotels.Find(id);
                serializeToModel(ref hotel);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref hotel hotel)
        {
            hotel.name = Request.Form["name"];

            if (Request.Form["tour_id"] != "")
            {
                hotel.tour_id = int.Parse(Request.Form["tour_id"]);
            }

            hotel.city_id = int.Parse(Request.Form["city_id"]);
            hotel.address = Request.Form["address"];
            hotel.stars_number = int.Parse(Request.Form["stars_number"]);

            if (Request.Form["distance_to_beach"] != "")
            {
                hotel.distance_to_beach = int.Parse(Request.Form["distance_to_beach"]);
            }

            if (Request.Form["food_id"] != "")
            {
                hotel.food_id = int.Parse(Request.Form["food_id"]);
            }
        }
    }
}