using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using Plugin.LocalNotification;
using SatNguApp.Mobile.Models;
using SatNguApp.Mobile.Services;

namespace SatNguApp.Mobile.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly BackendService _backendService;
        private ComboRecommendationResponse _recommendation;
        private bool _isLoading;
        private string _locationText = "Đang tìm vị trí...";

        public MainViewModel()
        {
            _backendService = new BackendService();
            LoadRecommendationCommand = new Command(async () => await LoadDataAsync());
            
            // Auto load on init
            Task.Run(async () => await LoadDataAsync());
        }

        public ICommand LoadRecommendationCommand { get; }

        public ComboRecommendationResponse Recommendation
        {
            get => _recommendation;
            set
            {
                _recommendation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasData));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        
        public bool HasData => Recommendation != null;

        public string LocationText
        {
            get => _locationText;
            set
            {
                _locationText = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            LocationText = "Đang lấy tọa độ GPS...";

            try
            {
                // Request notification permission
                var isGranted = await LocalNotificationCenter.Current.AreNotificationsEnabled();
                if (!isGranted)
                {
                    await LocalNotificationCenter.Current.RequestNotificationPermission();
                }

                // 1. Get Location
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    LocationText = $"Vị trí: {location.Latitude:F4}, {location.Longitude:F4}";
                    
                    // 2. Fetch Recommendation
                    Recommendation = await _backendService.GetRecommendationAsync(location.Latitude, location.Longitude);

                    if (Recommendation != null && Recommendation.Recommendation != null)
                    {
                        ScheduleDailyNotification(Recommendation.Recommendation);
                    }
                }
                else
                {
                    LocationText = "Không thể lấy vị trí GPS.";
                }
            }
            catch (FeatureNotSupportedException)
            {
                LocationText = "Thiết bị không hỗ trợ GPS.";
            }
            catch (FeatureNotEnabledException)
            {
                LocationText = "Vui lòng bật GPS trên thiết bị.";
            }
            catch (PermissionException)
            {
                LocationText = "Ứng dụng chưa được cấp quyền vị trí.";
            }
            catch (Exception ex)
            {
                LocationText = $"Lỗi: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ScheduleDailyNotification(Recommendation rec)
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "🎣 Thời điểm cực tốt để đi câu!",
                Description = $"Hôm nay cá {rec.Fish_Target} cắn mạnh. Bộ món: {rec.Bait}.",
                ReturningData = "BaitData", 
                Schedule = new NotificationRequestSchedule
                {
                    // Hẹn giờ thông báo mỗi ngày vào lúc 6h sáng (06:00:00)
                    NotifyTime = DateTime.Today.AddDays(1).AddHours(6),
                    RepeatType = NotificationRepeat.Daily
                }
            };

            LocalNotificationCenter.Current.Show(notification);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
