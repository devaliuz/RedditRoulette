using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using RedditRoulette.Services;
using RedditRoulette.Model;


namespace RedditRoulette.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly RedditApiService _redditApiService;
        private readonly FileService _fileService;
        public ObservableCollection<string> Subreddits { get; } = new ObservableCollection<string>();
        private RedditPost _currentPost;
        private string _selectedSubreddit;
        private string _subredditInput;
        private double _headerFontSizeProperty;
        private double _listFontSizeProperty;
        private double _labelFontSizeProperty;

        public RedditPost CurrentPost
        {
            get => _currentPost;
            set
            {
                _currentPost = value;
                OnPropertyChanged(nameof(CurrentPost));
            }
        }

        public string SelectedSubreddit
        {
            get => _selectedSubreddit;
            set
            {
                if (_selectedSubreddit != value)
                {
                    _selectedSubreddit = value;
                    OnPropertyChanged(nameof(SelectedSubreddit));
                }
            }
        }

        public string SubredditInput
        {
            get => _subredditInput;
            set
            {
                _subredditInput = value;
                OnPropertyChanged(nameof(SubredditInput));
            }
        }

        public double HeaderFontSizeProperty
        {
            get => _headerFontSizeProperty;
            set
            {
                _headerFontSizeProperty = value;
                OnPropertyChanged(nameof(HeaderFontSizeProperty));
            }
        }
        public double ListFontSizeProperty
        {
            get => _listFontSizeProperty;
            set
            {
                _listFontSizeProperty = value;
                OnPropertyChanged(nameof(ListFontSizeProperty));
            }
        }
        public double LabelFontSizeProperty
        {
            get => _labelFontSizeProperty;
            set
            {
                _labelFontSizeProperty = value;
                OnPropertyChanged(nameof(LabelFontSizeProperty));
            }
        }

        public ICommand AddSubredditCommand { get; }
        public ICommand GetRandomPostCommand { get; }
        public ICommand DeleteSubredditCommand { get; }
        public ICommand ClearSelectionCommand { get; }
        public ICommand OpenPostCommand { get; }




        public MainViewModel(RedditApiService redditApiService, FileService fileService)
        {
            _redditApiService = redditApiService;
            _fileService = fileService;
            AddSubredditCommand = new Command(AddSubreddit);
            GetRandomPostCommand = new Command(async () => await GetRandomPost(SelectedSubreddit));
            DeleteSubredditCommand = new Command<string>(DeleteSubreddit);
            ClearSelectionCommand = new Command(ClearSelection);
            OpenPostCommand = new Command(OpenPost);

            HeaderFontSizeProperty = 28;
            ListFontSizeProperty = 15;
            LabelFontSizeProperty = 20;

            LoadSubreddits();
        }

        private async void LoadSubreddits()
        {
            var profile = await _fileService.LoadProfileAsync();
            foreach (var subreddit in profile.Subreddits)
            {
                Subreddits.Add(subreddit);
            }
        }

        private async void AddSubreddit()
        {
            string subredditName = SubredditInput?.Trim();
            if (!string.IsNullOrWhiteSpace(subredditName))
            {
                // Remove "r/" if the user accidentally included it
                subredditName = subredditName.TrimStart('r', '/');

                string fullSubredditName = $"r/{subredditName}";
                if (!Subreddits.Contains(fullSubredditName))
                {
                    Subreddits.Add(fullSubredditName);
                    await SaveSubreddits();
                }
            }
            SubredditInput = string.Empty; // Clear the input field
        }

        private async void DeleteSubreddit(string subreddit)
        {
            if (Subreddits.Contains(subreddit))
            {
                Subreddits.Remove(subreddit);
                await SaveSubreddits();
            }
        }

        private async Task SaveSubreddits()
        {
            var profile = new Profile { Subreddits = Subreddits.ToList() };
            await _fileService.SaveProfileAsync(profile);
        }


        private async Task GetRandomPost(string subreddit)
        {
            if (string.IsNullOrEmpty(subreddit))
            {
                return;
            }

            try
            {
                SelectedSubreddit = subreddit;  // Set the selected subreddit
                CurrentPost = await _redditApiService.GetRandomPost(subreddit);
            }
            catch (Exception ex)
            {
                // Handle error (e.g., show an alert)
            }
        }

        private async void OpenPost()
        {
            if (CurrentPost != null)
            {
                await Browser.OpenAsync(CurrentPost.Url, BrowserLaunchMode.SystemPreferred);
            }
        }

        private void ClearSelection()
        {
            SelectedSubreddit = null;
            CurrentPost = null;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
