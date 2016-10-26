using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    public class HotelRoomsController : Controller
    {
        private readonly TravelSystemEntities _dbContext;

        public HotelRoomsController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Hotels
        public ActionResult List(int id)
        {
            var viewModel = new HotelRoomsView();

            var rooms = _dbContext.rooms
                .Include("hotel")
                .Where(x => x.hotel_id == id);

            foreach (var room in rooms)
            {
                viewModel.HotelRoomViewItems.Add(
                    new HotelRoomsView.HotelRoomViewItem
                    {
                        RoomId = room.id,
                        RoomNumber = room.number,
                        CostPerDay = room.cost_per_day,
                        Class = room.@class,
                        SeatsNumber = room.seats_number,
                        RoomSize = room.room_size ?? 0,
                        Description = room.description
                    });
            }

            var hotelName = "";
            var r = rooms.FirstOrDefault();

            if (r != null)
                hotelName = r.hotel.name;

            viewModel.HotelName = hotelName;

            return View(viewModel);
        }
    }
}