using System.Net.Http.Json;
using SatNguApp.Mobile.Models;

namespace SatNguApp.Mobile.Services
{
    public class BackendService
    {
        private readonly HttpClient _httpClient;
        
        // Live Railway Deployment URL
        private string BaseUrl = "https://satnguapp-backend-production.up.railway.app";

        public BackendService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ComboRecommendationResponse> GetRecommendationAsync(double lat, double lon)
        {
            try
            {
#if WINDOWS
                return new ComboRecommendationResponse
                {
                    Recommendation = new Recommendation { Fish_Target = "Cá Tra / Cá Lăng", Gear = "Cần Echo bạo lực, Thẻo dù 0.4", Bait = "Cám cá tanh + Cốt dừa" },
                    Context = new WeatherContext { Temperature = 30, Weather_Condition = "Nắng nhẹ", Tide_Condition = "Nước đang lớn" },
                    Message = "DỮ LIỆU DEMO (WINDOWS)"
                };
#else
                var response = await _httpClient.GetFromJsonAsync<ComboRecommendationResponse>($"{BaseUrl}/recommendation/?lat={lat}&lon={lon}");
                return response;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recommendation: {ex.Message}");
                return new();
            }
        }
        
        public async Task<BiorhythmResponse> GetBiorhythmAsync(int fishId)
        {
            try
            {
#if WINDOWS
                return new BiorhythmResponse
                {
                    Fish_Id = 1,
                    Advice = "Khung giờ vàng: Cá cắn mạnh nhất từ 07:00 - 09:00 sáng. Nên đánh tầng đáy.",
                    Hourly_Activity = new Dictionary<string, double> { { "07:00", 0.9 }, { "08:00", 1.0 }, { "09:00", 0.8 } }
                };
#else
                var response = await _httpClient.GetFromJsonAsync<BiorhythmResponse>($"{BaseUrl}/biorhythm/?fish_id={fishId}");
                return response;
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching biorhythm: {ex.Message}");
                return new();
            }
        }
    }
}
