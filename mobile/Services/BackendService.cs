using System.Net.Http.Json;
using SatNguApp.Mobile.Models;
using System.Globalization;

namespace SatNguApp.Mobile.Services
{
    public class BackendService
    {
        private readonly HttpClient _httpClient;
        
        // Live Railway Deployment URL
        private string BaseUrl = "https://satnguapp-backend-production.up.railway.app";

        public BackendService()
        {
#if ANDROID
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            _httpClient = new HttpClient(handler);
#else
            _httpClient = new HttpClient();
#endif
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ComboRecommendationResponse> GetRecommendationAsync(double lat, double lon)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync($"{BaseUrl}/recommendation/?lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}", ApiJsonContext.Default.ComboRecommendationResponse);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recommendation: {ex.Message}");
                return new ComboRecommendationResponse 
                { 
                    Message = ex.ToString(),
                    Recommendation = new Recommendation { Fish_Target = "ERR: " + ex.Message }
                };
            }
        }
        
        public async Task<BiorhythmResponse> GetBiorhythmAsync(int fishId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync($"{BaseUrl}/biorhythm/?fish_id={fishId}", ApiJsonContext.Default.BiorhythmResponse);
                return response;
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
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/chat/", request, ApiJsonContext.Default.ChatRequest);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync(ApiJsonContext.Default.ChatResponse);
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
