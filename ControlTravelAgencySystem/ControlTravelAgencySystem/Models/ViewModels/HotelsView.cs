using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class HotelsView
    {
        public class HotelViewItem
        {
            public int HotelId { get; set; }
            public string HotelName { get; set; }
            public string HotelAddress { get; set; }
            public int StarsNumber { get; set; }
            public int DistanceToBeach { get; set; }
            public string FoodType { get; set; }
            public string FoodDescription { get; set; }
            public int HowManyInDay { get; set; }
            public string CityName { get; set; }
        }

        public List<HotelViewItem> HotelViewItems { get; set; }

        public HotelsView()
        {
            HotelViewItems = new List<HotelViewItem>();
        }
    }
}