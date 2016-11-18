using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    /// <summary>
    /// Контроллер курортов
    /// </summary>
    public class ToursController : Controller
    {
        // Хранилище данных БД
        private readonly TravelSystemEntities _dbContext;

        /// <summary>
        /// Инициализация
        /// </summary>
        public ToursController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Представление списка туров
        /// </summary>
        public PartialViewResult GetToursList()
        {
            var viewModel = GetToursViewModel();
            return PartialView(viewModel);
        }

        /// <summary>
        /// Возвращает модель-представление списка туров
        /// </summary>
        private ToursView GetToursViewModel()
        {
            var viewModel = new ToursView();

            var citiesList = _dbContext.cities
                .ToList();

            var hotels = _dbContext.hotels
                .Include("city")
                .Include("tour")
                .ToList();

            var tours = _dbContext.tours
                .Include("country");

            var rooms = _dbContext.rooms
                .ToList();

            foreach (var tour in tours)
            {
                var citiesId = new List<int>();
                var min = 0;

                foreach (var hotel in hotels)
                    if (hotel.tour_id == tour.id)
                    {
                        var id = hotel.city.id;

                        if (!citiesId.Any(x => x == id))
                            citiesId.Add(id);

                        foreach (var room in rooms)
                            if (room.hotel_id == hotel.id)
                            {
                                if (min == 0)
                                    min = room.cost_per_day;
                                else
                                    if (min > room.cost_per_day)
                                        min = room.cost_per_day;
                            }
                    }

                var cities = "";
                var first = true;

                foreach (var id in citiesId)
                {
                    if (first)
                        first = false;
                    else
                        cities += ", ";

                    cities += citiesList.First(x => x.id == id).name;
                }

                viewModel.TourViewItems.Add(
                    new ToursView.TourViewItem
                    {
                        TourId = tour.id,
                        TourName = tour.name,
                        CountryName = tour.country?.name,
                        Cities = cities,
                        Description = "летний",
                        MinCost = 5 * min
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
                tour tour = new tour();
                serializeToModel(ref tour);
                _dbContext.tours.Add(tour);
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
                tour tour = _dbContext.tours.Find(id);
                serializeToModel(ref tour);
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private void serializeToModel(ref tour tour)
        {
            tour.country_id = int.Parse(Request.Form["country_id"]);
            tour.name = Request.Form["name"];
        }
    }
}