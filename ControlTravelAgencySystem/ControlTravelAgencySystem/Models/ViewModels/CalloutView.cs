using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class CalloutView
    {
        public class CalloutViewItem
        {
            public int id { get; set; }
            public int createdAt { get; set; }

            public int cancellationAt { get; set; }

            public string fullname { get; set; }

            public string email { get; set; }

            public string phone { get; set; }
        }

        public List<CalloutViewItem> CalloutViewItems { get; set; }

        public CalloutView()
        {
            CalloutViewItems = new List<CalloutViewItem>();
        }
    }
}