using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using RedditRoulette.ViewModel;
using RedditRoulette.Services;
using RedditRoulette.Model;

namespace RedditRoulette
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(new RedditApiService(), new FileService());
            BindingContext = _viewModel;
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            _viewModel.SelectedSubreddit = null;
            _viewModel.CurrentPost = null;
        }

        private async void OnOpenPostClicked(object sender, EventArgs e)
        {
            if (_viewModel.CurrentPost != null)
            {
                await Launcher.OpenAsync(_viewModel.CurrentPost.Url);
            }
        }

        private void OnSubredditEntryFocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.SubredditInput))
            {
                _viewModel.SubredditInput = string.Empty;
            }
        }

        private void OnSubredditEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.SubredditInput))
            {
                _viewModel.SubredditInput = string.Empty;
            }
        }
    }
}
