using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class RouteView
    {
        public class RouteViewItem
        {
            public bool IsChecked { get; set; }
            public int RouteId { get; set; }
            public string Type { get; set; }
            public string FromAirport { get; set; }
            public string ToAirport { get; set; }
            public string StartingAddress { get; set; }
            public DateTime StartingTime { get; set; }
            public string FinalAddress { get; set; }
            public int Duration { get; set; }
            public int TotalSeats { get; set; }
            public int Distance { get; set; }
            public int Cost { get; set; }
        }

        public List<RouteViewItem> RouteViewItems { get; set; }

        public RouteView()
        {
            RouteViewItems = new List<RouteViewItem>();
        }
    }
}