using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
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

            var hotels = _dbContext.hotels
                .Include("food")
                .Include("city")
                .Include("tour")
                .Where(x => x.tour_id == id);

            foreach (var hotel in hotels)
            {
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
                        CityName = hotel.city?.name
                    });
            }

            return viewModel;
        }
    }
}