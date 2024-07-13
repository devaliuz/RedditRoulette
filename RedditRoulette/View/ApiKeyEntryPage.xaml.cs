using RedditRoulette.Services;
using RedditRoulette.ViewModel;

namespace RedditRoulette.View
{
	public partial class ApiKeyEntryPage : ContentPage
	{

		private readonly FileService _fileService;


		public ApiKeyEntryPage()
		{
			InitializeComponent();
            BindingContext = new ApiKeyEntryViewModel(new FileService());

        }
    }
}