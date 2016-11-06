using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class ToursView
    {
        public class TourViewItem
        {
            public int TourId { get; set; }
            public string TourName { get; set; }
            public string CountryName { get; set; }
            public string Cities { get; set; }
            public string Description { get; set; }
        }

        public List<TourViewItem> TourViewItems { get; set; }

        public ToursView()
        {
            TourViewItems = new List<TourViewItem>();
        }
    }
}