using Xamarin.Forms;

namespace EmoSong
{
	public partial class App : Application
	{
		public static RESTManager RManager { get; set; }
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
			RManager = new RESTManager();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
