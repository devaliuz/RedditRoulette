using System.ComponentModel;
using System.Windows.Input;
using RedditRoulette.Services;

namespace RedditRoulette.ViewModel
{
    public class ApiKeyEntryViewModel : INotifyPropertyChanged
    {
        private readonly FileService _fileService;
        private string _apiKey;

        public string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                OnPropertyChanged(nameof(ApiKey));
            }
        }

        public ICommand SaveApiKeyCommand { get; }

        public ApiKeyEntryViewModel(FileService fileService)
        {
            _fileService = fileService;
            SaveApiKeyCommand = new Command(async () => await SaveApiKey());
            LoadApiKey();
        }

        private async void LoadApiKey()
        {
            ApiKey = await _fileService.LoadApiKeyAsync();
        }

        private async Task SaveApiKey()
        {
            await _fileService.SaveApiKeyAsync(ApiKey);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
