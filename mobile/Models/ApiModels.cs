using System;
using System.Collections.Generic;

namespace SatNguApp.Mobile.Models
{
    public class ComboRecommendationResponse
    {
        public WeatherContext Context { get; set; }
        public Recommendation Recommendation { get; set; }
        public string Message { get; set; }
    }

    public class WeatherContext
    {
        public int Temperature { get; set; }
        public string Weather_Condition { get; set; }
        public string Tide_Condition { get; set; }
    }

    public class Recommendation
    {
        public string Fish_Target { get; set; }
        public string Gear { get; set; }
        public string Bait { get; set; }
    }

    public class BiorhythmResponse
    {
        public int Fish_Id { get; set; }
        public Dictionary<string, double> Hourly_Activity { get; set; }
        public string Advice { get; set; }
    }
}
