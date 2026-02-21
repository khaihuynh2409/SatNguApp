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
                    Recommendation = new Recommendation 
                    { 
                        Fish_Target = "Cá chép / Cá trắm (Hồ tự nhiên)", 
                        Gear = "Cần tay 5H - 5m4, Phao cỏ ngọn nhỏ",
                        Axis_Line = "Trục nylon 2.0",
                        Leader = "Thẻo Fluoro 1.2",
                        Hook = "Isego size 6, không ngạnh",
                        Bait = "Mồi xả tơi + Mồi vuốt (vị tanh dâu)" 
                    },
                    Context = new WeatherContext { Temperature = 30, Weather_Condition = "Nắng nhẹ", Tide_Condition = "Nước đang lớn" },
                    Message = "DỮ LIỆU DEMO NÂNG CẤP (WINDOWS)"
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

        public async Task<ChatResponse> SendChatMessageAsync(string message, string imageBase64 = null)
        {
            try
            {
#if WINDOWS
                return new ChatResponse { Reply = "🤖 (DEMO WINDOWS) Bác đã gửi tin nhắn. Trong bản Demo Windows không gọi Internet để tránh lỗi mạng. Trục thẻo thế nào bác cứ hỏi nhé!" };
#else
                var request = new ChatRequest { Message = message, Image_Base64 = imageBase64 };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/chat/", request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result ?? new ChatResponse { Reply = "Có lỗi xảy ra khi kết nối máy chủ AI." };
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending chat: {ex.Message}");
                return new ChatResponse { Reply = "Lỗi kết nối mạng: " + ex.Message };
            }
        }
    }
}
