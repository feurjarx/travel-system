using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
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

            foreach (var tour in tours)
            {
                var citiesId = new List<int>();

                foreach (var hotel in hotels)
                    if (hotel.tour_id == tour.id)
                    {
                        var id = hotel.city.id;

                        if (!citiesId.Any(x => x == id))
                            citiesId.Add(id);
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
                        Cities = cities
                    });
            }

            return viewModel;
        }
    }
}