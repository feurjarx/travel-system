using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlTravelAgencySystem.Common
{
    public class Constants
    {
        public static string ddMMMyyyyHmmss = "dd MMM yyyy (H:mm:ss)";
        public static string hhmmss = @"hh\:mm\:ss";
        public static string ddMMMyyyy = "dd MMM yyyy";
    }

    public class Scheme
    {
        public static List<object> predefinedCallout = new List<object>()
        {
            "id",
            "details",
            "created_at",
            new
            {
                key = "airtickets",
                type = "array",
                properties = new List<object>()
                {
                    "id",
                    "departure_at",
                    "payment",
                    new
                    {
                        key = "flight",
                        properties = new List<object>
                        {
                            "id",
                            "code",
                            "flight_at",
                            "duration",
                            new
                            {
                                key = "airline",
                                properties = new List<object>
                                {
                                    "name"
                                }
                            },
                            new
                            {
                                key = "airport",
                                properties = new List<object>
                                {
                                    new
                                    {
                                        key = "city",
                                        properties = new List<object>
                                        {
                                            "name"
                                        }
                                    }
                                }
                            },
                            new
                            {
                                key = "airport1",
                                properties = new List<object>
                                {
                                    new
                                    {
                                        key = "city",
                                        properties = new List<object>
                                        {
                                            "name"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new
            {
                key = "transfers",
                type = "array",
                properties = new List<object>()
                {
                    new
                    {
                        key = "route",
                        properties = new List<object>
                        {
                            "type",
                            "starting_time",
                            "final_address"
                        }
                    }
                }
            },
            new
            {
                key = "callout_room",
                type = "array",
                properties = new List<object>()
                {
                    "start_living_at",
                    "duration",
                    new
                    {
                        key = "room",
                        properties = new List<object>
                        {
                            "type",
                            new
                            {
                                key = "hotel",
                                properties = new List<object>
                                {
                                    "name",
                                    "stars_number",
                                    "distance_to_beach",
                                    new
                                    {
                                        key = "city",
                                        properties = new List<object>
                                        {
                                            "name"
                                        }
                                    },
                                    new
                                    {
                                        key = "food",
                                        properties = new List<object>
                                        {
                                            "type",
                                            "how_many_in_day"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new
            {
                key = "excursion_order",
                type = "array",
                properties = new List<object>()
                {
                    "starting_address",
                    "starting_at",
                    new
                    {
                        key = "excursion",
                        properties = new List<object>()
                        {
                            "name",
                            "starting_time",
                            "duration"
                        }
                    }
                }
            },
            new
            {
                key = "hotel_service_order",
                type = "array",
                properties = new List<object>()
                {
                    new
                    {
                        key = "hotel_service",
                        properties = new List<object>()
                        {
                            "description"
                        }
                    }
                }
            }
        };
    }
}