using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using RedditRoulette.Services;
using RedditRoulette.Model;
using System.Diagnostics;

namespace RedditRoulette.ViewModel
{
    /// <summary>
    /// ViewModel for the main page of the Reddit Roulette application.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private readonly RedditApiService _redditApiService;
        private readonly FileService _fileService;
        private ObservableCollection<string> _subredditSuggestions = new ObservableCollection<string>();
        private RedditChildData _currentPost;
        private string _selectedSubreddit;
        private string _subredditInput;
        private double _headerFontSizeProperty;
        private double _listFontSizeProperty;
        private double _labelFontSizeProperty;
        private bool _canAddSubreddit = true;
        private string _addButtonMessage = "";
        private CancellationTokenSource _suggestionCts;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the collection of subreddits added by the user.
        /// </summary>
        public ObservableCollection<string> Subreddits { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the current Reddit post being displayed.
        /// </summary>
        public RedditChildData CurrentPost
        {
            get => _currentPost;
            set
            {
                _currentPost = value;
                OnPropertyChanged(nameof(CurrentPost));
            }
        }

        /// <summary>
        /// Gets or sets the currently selected subreddit.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the user's input for a new subreddit.
        /// </summary>
        public string SubredditInput
        {
            get => _subredditInput;
            set
            {
                if (_subredditInput != value)
                {
                    _subredditInput = value;
                    OnPropertyChanged(nameof(SubredditInput));
                    UpdateSubredditSuggestions(value);
                    UpdateAddButtonState();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font size for header text.
        /// </summary>
        public double HeaderFontSizeProperty
        {
            get => _headerFontSizeProperty;
            set
            {
                _headerFontSizeProperty = value;
                OnPropertyChanged(nameof(HeaderFontSizeProperty));
            }
        }

        /// <summary>
        /// Gets or sets the font size for list items.
        /// </summary>
        public double ListFontSizeProperty
        {
            get => _listFontSizeProperty;
            set
            {
                _listFontSizeProperty = value;
                OnPropertyChanged(nameof(ListFontSizeProperty));
            }
        }

        /// <summary>
        /// Gets or sets the font size for labels.
        /// </summary>
        public double LabelFontSizeProperty
        {
            get => _labelFontSizeProperty;
            set
            {
                _labelFontSizeProperty = value;
                OnPropertyChanged(nameof(LabelFontSizeProperty));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Add Subreddit button can be clicked.
        /// </summary>
        public bool CanAddSubreddit
        {
            get => _canAddSubreddit;
            set
            {
                if (_canAddSubreddit != value)
                {
                    _canAddSubreddit = value;
                    OnPropertyChanged(nameof(CanAddSubreddit));
                }
            }
        }

        /// <summary>
        /// Gets or sets the message displayed near the Add Subreddit button.
        /// </summary>
        public string AddButtonMessage
        {
            get => _addButtonMessage;
            set
            {
                if (_addButtonMessage != value)
                {
                    _addButtonMessage = value;
                    OnPropertyChanged(nameof(AddButtonMessage));
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of subreddit suggestions based on user input.
        /// </summary>
        public ObservableCollection<string> SubredditSuggestions
        {
            get => _subredditSuggestions;
            set
            {
                if (_subredditSuggestions != value)
                {
                    _subredditSuggestions = value;
                    OnPropertyChanged(nameof(SubredditSuggestions));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command for adding a new subreddit.
        /// </summary>
        public ICommand AddSubredditCommand { get; }

        /// <summary>
        /// Gets the command for fetching a random post from the selected subreddit.
        /// </summary>
        public ICommand GetRandomPostCommand { get; }

        /// <summary>
        /// Gets the command for deleting a subreddit from the list.
        /// </summary>
        public ICommand DeleteSubredditCommand { get; }

        /// <summary>
        /// Gets the command for clearing the current selection.
        /// </summary>
        public ICommand ClearSelectionCommand { get; }

        /// <summary>
        /// Gets the command for opening the current post in a web browser.
        /// </summary>
        public ICommand OpenPostCommand { get; }

        /// <summary>
        /// Gets the command for selecting a subreddit suggestion.
        /// </summary>
        public ICommand SelectSuggestionCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="redditApiService">The service for interacting with the Reddit API.</param>
        /// <param name="fileService">The service for file operations.</param>
        public MainViewModel(RedditApiService redditApiService, FileService fileService)
        {
            _redditApiService = redditApiService;
            _fileService = fileService;

            // Initialize commands
            AddSubredditCommand = new Command(AddSubreddit);
            GetRandomPostCommand = new Command(async () => await GetRandomPost(SelectedSubreddit));
            DeleteSubredditCommand = new Command<string>(DeleteSubreddit);
            ClearSelectionCommand = new Command(ClearSelection);
            OpenPostCommand = new Command(OpenPost);
            SelectSuggestionCommand = new Command<string>(SelectSuggestion);

            // Set default font sizes
            HeaderFontSizeProperty = 28;
            ListFontSizeProperty = 15;
            LabelFontSizeProperty = 20;

            LoadSubreddits();
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Loads saved subreddits from the profile.
        /// </summary>
        private async void LoadSubreddits()
        {
            var profile = await _fileService.LoadProfileAsync();
            foreach (var subreddit in profile.Subreddits)
            {
                Subreddits.Add(subreddit);
            }
        }

        /// <summary>
        /// Saves the current list of subreddits to the profile.
        /// </summary>
        private async Task SaveSubreddits()
        {
            var profile = new Profile { Subreddits = Subreddits.ToList() };
            await _fileService.SaveProfileAsync(profile);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Adds a new subreddit to the list.
        /// </summary>
        private async void AddSubreddit()
        {
            string subredditName = SubredditInput?.Trim();
            if (!string.IsNullOrWhiteSpace(subredditName))
            {
                subredditName = subredditName.TrimStart('r', '/');
                string fullSubredditName = $"r/{subredditName}";
                if (!Subreddits.Contains(fullSubredditName))
                {
                    Subreddits.Add(fullSubredditName);
                    await SaveSubreddits();
                }
            }
            SubredditInput = string.Empty;
            UpdateAddButtonState();
            ClearSuggestions();
        }

        /// <summary>
        /// Deletes a subreddit from the list.
        /// </summary>
        /// <param name="subreddit">The name of the subreddit to delete.</param>
        private async void DeleteSubreddit(string subreddit)
        {
            if (Subreddits.Contains(subreddit))
            {
                Subreddits.Remove(subreddit);
                await SaveSubreddits();
            }
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        private void ClearSelection()
        {
            SelectedSubreddit = null;
            CurrentPost = null;
        }

        /// <summary>
        /// Opens the current post in the default browser.
        /// </summary>
        private async void OpenPost()
        {
            if (CurrentPost != null && !string.IsNullOrEmpty(CurrentPost.Url))
            {
                await Browser.OpenAsync(CurrentPost.Url, BrowserLaunchMode.SystemPreferred);
            }
        }

        /// <summary>
        /// Selects a suggestion from the list.
        /// </summary>
        /// <param name="suggestion">The suggested subreddit name to select.</param>
        private void SelectSuggestion(string suggestion)
        {
            SubredditInput = suggestion.TrimStart('r', '/');
            SubredditSuggestions.Clear();
            OnPropertyChanged(nameof(SubredditSuggestions));
        }

        #endregion

        #region API Tasks

        /// <summary>
        /// Fetches a random post from the selected subreddit.
        /// </summary>
        /// <param name="subreddit">The name of the subreddit to fetch a post from.</param>
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
                // TODO: Handle error (e.g., show an alert)
                Debug.WriteLine($"Error fetching random post: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the subreddit suggestions based on user input.
        /// </summary>
        /// <param name="query">The user's input to base suggestions on.</param>
        private async void UpdateSubredditSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                ClearSuggestions();
                return;
            }

            _suggestionCts?.Cancel();
            _suggestionCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(1000, _suggestionCts.Token);
                var suggestions = await _redditApiService.GetSubreddits(query);

                if (!_suggestionCts.IsCancellationRequested)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        SubredditSuggestions.Clear();
                        // Filter out already saved subreddits and take only the top 4
                        var filteredSuggestions = suggestions
                            .Where(s => !Subreddits.Contains(s) && !Subreddits.Contains($"r/{s.TrimStart('r', '/')}"))
                            .Take(4);
                        foreach (var suggestion in filteredSuggestions)
                        {
                            SubredditSuggestions.Add(suggestion);
                        }
                        OnPropertyChanged(nameof(SubredditSuggestions));
                        UpdateAddButtonState();
                    });

                    Debug.WriteLine($"SubredditSuggestions now contains {SubredditSuggestions.Count} items");
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle error (e.g., show an alert)
                Debug.WriteLine($"Error updating subreddit suggestions: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the state of the Add Subreddit button and its message.
        /// </summary>
        private void UpdateAddButtonState()
        {
            bool isInputEmpty = string.IsNullOrWhiteSpace(SubredditInput);
            string normalizedInput = isInputEmpty ? "" : $"r/{SubredditInput.TrimStart('r', '/')}";
            bool subredditExists = !isInputEmpty && Subreddits.Any(subreddit => subreddit.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase));
            bool hasSuggestions = SubredditSuggestions.Count >= 1;

            CanAddSubreddit = !isInputEmpty && !subredditExists && hasSuggestions;

            if (isInputEmpty)
            {
                AddButtonMessage = "";
            }
            else if (subredditExists)
            {
                AddButtonMessage = "Subreddit already added!";
            }
            else if (!hasSuggestions)
            {
                AddButtonMessage = "Subreddit not found!";
            }
            else
            {
                AddButtonMessage = "";
            }
        }

        /// <summary>
        /// Clears the current subreddit suggestions.
        /// </summary>
        private void ClearSuggestions()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SubredditSuggestions.Clear();
                OnPropertyChanged(nameof(SubredditSuggestions));
            });
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}