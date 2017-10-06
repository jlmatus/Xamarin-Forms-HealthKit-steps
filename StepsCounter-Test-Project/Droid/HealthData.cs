using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Result;
using Android.OS;
using Android.Util;
using StepsCounterApp;
using StepsCounterTestProject.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(HealthData))]
namespace StepsCounterTestProject.Droid
{
	public class HealthData : Java.Lang.Object, IHealthData, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
	{
		GoogleApiClient mGoogleApiClient;
		bool authInProgress;
		const int REQUEST_OAUTH = 1;
		Action<bool> AuthorizationCallBack;
		private Action<int, Result, Intent> _resultCallback;

		public void FetchActiveEnergyBurned(Action<double> completionHandler)
		{
			throw new NotImplementedException();
		}

		public void FetchActiveMinutes(Action<double> completionHandler)
		{
			throw new NotImplementedException();
		}

		public void FetchMetersWalked(Action<double> completionHandler)
		{
			throw new NotImplementedException();
		}

		public void FetchSteps(Action<double> completionHandler)
		{
			throw new NotImplementedException();
		}

		public void GetHealthPermissionAsync(Action<bool> completion)
		{

			AuthorizationCallBack = completion;

			mGoogleApiClient = new GoogleApiClient.Builder(Forms.Context)
				.AddConnectionCallbacks(this)
				.AddApi(FitnessClass.HISTORY_API)
				.AddScope(new Scope(Scopes.FitnessActivityReadWrite))
				.AddOnConnectionFailedListener(result =>
				{
					Log.Info("apiClient", "Connection failed. Cause: " + result);
					if (!result.HasResolution)
					{
						// Show the localized error dialog
						GooglePlayServicesUtil.GetErrorDialog(result.ErrorCode, (Activity)Forms.Context, 0).Show();
						return;
					}
					// The failure has a resolution. Resolve it.
					// Called typically when the app is not yet authorized, and an
					// authorization dialog is displayed to the user.
					if (!authInProgress)
					{
						try
						{
							Log.Info("apiClient", "Attempting to resolve failed connection");
							authInProgress = true;
							result.StartResolutionForResult((Activity)Forms.Context, REQUEST_OAUTH);
						}
						catch (Exception e)
						{
							Log.Error("apiClient", "Exception while starting resolution activity", e);
						}
					}
				}).Build();
			//mGoogleApiClient.Connect();
			((MainActivity)Forms.Context).ActivityResult += HandleActivityResult;
			mGoogleApiClient.Connect();



		}

		private void HandleActivityResult(object sender, ActivityResultEventArgs e)
		{
			((MainActivity)Forms.Context).ActivityResult -= HandleActivityResult;
			if (e.RequestCode == REQUEST_OAUTH)
			{
				authInProgress = false;
				if (e.ResultCode == Result.Ok)
				{
					// Make sure the app is not already connected or attempting to connect
					if (!mGoogleApiClient.IsConnecting && !mGoogleApiClient.IsConnected)
					{
						mGoogleApiClient.Connect();
					}
				}
				else
				{
					AuthorizationCallBack(false);
				}
			}
		}

		public void OnConnected(Bundle bundle)
		{
			// This method is called when we connect to the LocationClient. We can start location updated directly form
			// here if desired, or we can do it in a lifecycle method, as shown above 

			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
			Log.Info("LocationClient", "Now connected to client");
			AuthorizationCallBack(true);

		}

		public void OnDisconnected()
		{
			// This method is called when we disconnect from the LocationClient.

			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
			Log.Info("LocationClient", "Now disconnected from client");
		}

		public void OnConnectionFailed(ConnectionResult bundle)
		{
			// This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
			// You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

			// You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
			Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
		}

		public void OnConnectionSuspended(int i)
		{

		}

	}
}
