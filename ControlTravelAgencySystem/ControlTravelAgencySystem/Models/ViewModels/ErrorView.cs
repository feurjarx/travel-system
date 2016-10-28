using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Models.ViewModels
{
    public class ErrorView
    {
        public string type { get; set; }
        public string[] message { get; set; }
    }
}