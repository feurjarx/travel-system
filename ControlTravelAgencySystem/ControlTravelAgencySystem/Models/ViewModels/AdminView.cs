using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class AdminView
    {
        public class AdminViewItem
        {
            public string test { get; set; }
            
        }

        public List<AdminViewItem> AdminViewItems { get; set; }

        public AdminView()
        {
            AdminViewItems = new List<AdminViewItem>();
        }
    }
}