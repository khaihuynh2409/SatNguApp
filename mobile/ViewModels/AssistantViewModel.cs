using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SatNguApp.Mobile.Models;
using SatNguApp.Mobile.Services;

namespace SatNguApp.Mobile.ViewModels
{
    public class ChatMessageModel
    {
        public string Text { get; set; } = string.Empty;
        public bool IsIncoming { get; set; }
        public bool HasImage => !string.IsNullOrEmpty(ImagePath);
        public string ImagePath { get; set; } = string.Empty;
        public string BackgroundColor => IsIncoming ? "#1C2541" : "#5BC0BE";
        public string TextColor => IsIncoming ? "White" : "#0B132B";
        public LayoutOptions Alignment => IsIncoming ? LayoutOptions.Start : LayoutOptions.End;
    }

    public class AssistantViewModel : INotifyPropertyChanged
    {
        private readonly BackendService _backendService;
        public ObservableCollection<ChatMessageModel> Messages { get; } = new();

        private string _inputText = string.Empty;
        public string InputText
        {
            get => _inputText;
            set { _inputText = value; OnPropertyChanged(); }
        }

        private string _selectedImagePath = string.Empty;
        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set { _selectedImagePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSelectedImage)); }
        }

        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePath);

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public Command SendCommand { get; }
        public Command PickImageCommand { get; }
        public Command RemoveImageCommand { get; }

        public AssistantViewModel()
        {
            _backendService = new BackendService();
            SendCommand = new Command(async () => await SendMessageAsync(), () => !IsBusy && (!string.IsNullOrWhiteSpace(InputText) || HasSelectedImage));
            PickImageCommand = new Command(async () => await PickImageAsync());
            RemoveImageCommand = new Command(() => SelectedImagePath = string.Empty);
            
            // Add initial welcome message
            Messages.Add(new ChatMessageModel 
            { 
                Text = "🤖 Chào bác! Em là Trợ Lý AI. Bác cần hỏi gì về mồi, trục thẻo hay muốn em kiểm tra mồi bằng hình ảnh thì nhắn em nhé!", 
                IsIncoming = true 
            });
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(InputText) && !HasSelectedImage) return;

            IsBusy = true;
            string textToSend = InputText;
            string imgPath = SelectedImagePath;
            string base64Image = null;

            // Add user message to UI
            Messages.Add(new ChatMessageModel { Text = textToSend, IsIncoming = false, ImagePath = imgPath });
            
            InputText = string.Empty;
            SelectedImagePath = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(imgPath))
                {
                    byte[] imageBytes = await File.ReadAllBytesAsync(imgPath);
                    base64Image = Convert.ToBase64String(imageBytes);
                }

                var response = await _backendService.SendChatMessageAsync(textToSend, base64Image);

                Messages.Add(new ChatMessageModel { Text = response.Reply, IsIncoming = true });
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessageModel { Text = $"[Lỗi]: {ex.Message}", IsIncoming = true });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Chọn ảnh mồi câu/địa hình"
                });

                if (result != null)
                {
                    SelectedImagePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image pick error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(InputText) || propertyName == nameof(SelectedImagePath) || propertyName == nameof(IsBusy))
            {
                SendCommand.ChangeCanExecute();
            }
        }
    }
}
