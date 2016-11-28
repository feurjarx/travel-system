using System;
using System.Collections.Generic;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class FavotiteListView
    {
        public class FavotiteListViewItem
        {
            public class FlightsItem
            {
                public bool IsChecked { get; set; }
                public int? FlightId { get; set; }
                public string Code { get; set; }
                public airport FromAirport { get; set; }
                public airport ToAirport { get; set; }
                public DateTime? FlightAt { get; set; }
                public int? Duration { get; set; }
                public string AirlineName { get; set; }
            }

            public room SelectedRoom { get; set; }
            public int? FlightId { get; set; }

            public List<FlightsItem> FlightsItems { get; set; }

            public FavotiteListViewItem()
            {
                FlightsItems = new List<FlightsItem>();
            }
        }

        public List<FavotiteListViewItem> FavotiteListViewItems { get; set; }

        public FavotiteListView()
        {
            FavotiteListViewItems = new List<FavotiteListViewItem>();
        }

        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}