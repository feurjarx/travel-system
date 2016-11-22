using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class HotelServicesView
    {
        public class HotelServicesViewItem
        {
            public bool IsChecked { get; set; }
            public int HotelServiceId { get; set; }
            public string Description { get; set; }
            public int CostPerMin { get; set; }
            public TimeSpan? StartingTime { get; set; }
        }

        public List<HotelServicesViewItem> HotelServicesViewItems { get; set; }

        public HotelServicesView()
        {
            HotelServicesViewItems = new List<HotelServicesViewItem>();
        }
    }
}