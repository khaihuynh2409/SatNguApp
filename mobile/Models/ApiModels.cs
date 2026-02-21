using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SatNguApp.Mobile.Models
{
    public class ComboRecommendationResponse
    {
        public WeatherContext Context { get; set; } = new();
        public Recommendation Recommendation { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class WeatherContext
    {
        public int Temperature { get; set; }
        public string Weather_Condition { get; set; } = string.Empty;
        public string Tide_Condition { get; set; } = string.Empty;
    }

    public class Recommendation
    {
        public string Fish_Target { get; set; } = string.Empty;
        public string Gear { get; set; } = string.Empty;
        public string Axis_Line { get; set; } = string.Empty;
        public string Leader { get; set; } = string.Empty;
        public string Hook { get; set; } = string.Empty;
        public string Bait { get; set; } = string.Empty;
    }

    public class BiorhythmResponse
    {
        public int Fish_Id { get; set; }
        public Dictionary<string, double> Hourly_Activity { get; set; } = new();
        public string Advice { get; set; } = string.Empty;
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? Image_Base64 { get; set; }
    }

    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
    }

    [JsonSerializable(typeof(ComboRecommendationResponse))]
    [JsonSerializable(typeof(WeatherContext))]
    [JsonSerializable(typeof(Recommendation))]
    [JsonSerializable(typeof(BiorhythmResponse))]
    [JsonSerializable(typeof(ChatRequest))]
    [JsonSerializable(typeof(ChatResponse))]
    [JsonSerializable(typeof(Dictionary<string, double>))]
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    public partial class ApiJsonContext : JsonSerializerContext
    {
    }
}
