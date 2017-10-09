using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using StepsCounterApp;

namespace StepsCounterTestProject.Droid
{

	public class ActivityResultEventArgs : EventArgs
	{
		public int RequestCode { get; set; }
		public Result ResultCode { get; set; }
		public Intent Data { get; set; }

	}

	[Activity(Label = "StepsCounter-Test-Project.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		public event EventHandler<ActivityResultEventArgs> ActivityResult = delegate { };

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);

			LoadApplication(new App());
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			ActivityResult(this, new ActivityResultEventArgs
			{
				RequestCode = requestCode,
				ResultCode = resultCode,
				Data = data
			});
		}
	}

}
