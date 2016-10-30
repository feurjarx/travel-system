using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
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

            var tours = _dbContext.tours
                .Include("country");

            foreach (var tour in tours)
                viewModel.TourViewItems.Add(
                    new ToursView.TourViewItem
                    {
                        TourId = tour.id,
                        TourName = tour.name,
                        CountryName = tour.country?.name
                    });

            return viewModel;
        }
    }
}