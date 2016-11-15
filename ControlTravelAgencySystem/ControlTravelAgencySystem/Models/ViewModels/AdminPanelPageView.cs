using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class AdminPanelPageView
    {
        public employee user { get; set; }

        public IQueryable<callout> callouts { get; set; }
        public IQueryable<employee> employees { get; set; }

        public List<tour> tours { get; set; }

        public List<hotel> hotels { get; set; }

        public List<room> rooms { get; set; }

        public List<country> countries { get; set; }

        public List<city> cities { get; set; }

        public List<food> foods { get; set; }

        public string section = null;
    }
}