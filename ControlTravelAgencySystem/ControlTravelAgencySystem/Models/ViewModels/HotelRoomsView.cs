using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class HotelRoomsView
    {
        public class HotelRoomViewItem
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public int CostPerDay { get; set; }
            public string Class { get; set; }
            public int SeatsNumber { get; set; }
            public int RoomSize { get; set; }
            public string Description { get; set; }
        }

        public string HotelName { get; set; }

        public List<HotelRoomViewItem> HotelRoomViewItems { get; set; }

        public HotelRoomsView()
        {
            HotelRoomViewItems = new List<HotelRoomViewItem>();
        }
    }
}