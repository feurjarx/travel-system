using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public HomeController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public ActionResult Index()
        {
            var viewModel = new ToursView();            
            var tours = _dbContext.tours.Include("country");

            foreach (var tour in tours)
            {
                var tourId = tour.id;
                var tourName = tour.name;
                var countryName = "";

                if (tour.country != null)
                    countryName = tour.country.name;

                viewModel.TourViewItems.Add(
                    new ToursView.TourViewItem
                    {
                        TourId = tourId,
                        TourName = tourName,
                        CountryName = countryName
                    });
            }

             return View(viewModel);
        }
    }
}
