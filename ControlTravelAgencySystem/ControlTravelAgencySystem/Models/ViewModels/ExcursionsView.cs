using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class ExcursionsView
    {
        public class ExcursionsViewItem
        {
            public bool IsChecked { get; set; }
            public int ExcursionId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Duration { get; set; }
        }

        public List<ExcursionsViewItem> ExcursionsViewItems { get; set; }

        public ExcursionsView()
        {
            ExcursionsViewItems = new List<ExcursionsViewItem>();
        }
    }
}