using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class RoomsView
    {
        public class RoomViewItem
        {
            public bool IsChecked { get; set; }
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public int CostPerDay { get; set; }
            public string Class { get; set; }
            public int SeatsNumber { get; set; }
            public int RoomSize { get; set; }
            public string Description { get; set; }
        }

        public List<RoomViewItem> RoomViewItems { get; set; }

        public RoomsView()
        {
            RoomViewItems = new List<RoomViewItem>();
        }
    }
}