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
                var response = await _httpClient.GetFromJsonAsync<ComboRecommendationResponse>($"{BaseUrl}/recommendation/?lat={lat}&lon={lon}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recommendation: {ex.Message}");
                return null;
            }
        }
        
        public async Task<BiorhythmResponse> GetBiorhythmAsync(int fishId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<BiorhythmResponse>($"{BaseUrl}/biorhythm/?fish_id={fishId}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching biorhythm: {ex.Message}");
                return null;
            }
        }
    }
}
