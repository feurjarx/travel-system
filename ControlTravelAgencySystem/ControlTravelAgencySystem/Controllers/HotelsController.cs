using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class HotelsController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public HotelsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Hotels
        public ActionResult List(int id)
        {
            var viewModel = new HotelsView();

            var hotels = _dbContext.hotels
                .Include("food")
                .Include("city")
                .Include("tour")
                .Where(x => x.tour_id == id);

            foreach (var hotel in hotels)
            {
                var foodDescription = "";
                var foodType = "";
                var howManyInDay = 0;
                var cityName = "";

                if (hotel.food != null)
                {
                    foodDescription = hotel.food.description;
                    howManyInDay = hotel.food.how_many_in_day;
                    foodType = hotel.food.type;
                }

                if (hotel.city != null)
                    cityName = hotel.city.name;

                viewModel.HotelViewItems.Add(
                    new HotelsView.HotelViewItem
                    {
                        HotelId = hotel.id,
                        HotelName = hotel.name,
                        HotelAddress = hotel.address,
                        StarsNumber = hotel.stars_number,
                        DistanceToBeach = hotel.distance_to_beach ?? 0,
                        FoodType = foodType,
                        FoodDescription = foodDescription,
                        HowManyInDay = howManyInDay,
                        CityName = cityName
                    });
            }

            var tourName = "";
            var h = hotels.FirstOrDefault();

            if (h != null)
                tourName = h.name;

            ViewBag.TourName = tourName;

            return View(viewModel);
        }
    }
}