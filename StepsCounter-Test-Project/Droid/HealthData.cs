using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Android.OS;
using Android.Util;
using Java.Util;
using Java.Util.Concurrent;
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
		const string DATE_FORMAT = "yyyy.MM.dd HH:mm:ss";
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
				.AddScope(new Scope(Scopes.FitnessLocationRead))
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

		void HandleActivityResult(object sender, ActivityResultEventArgs e)
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
			FetchGoogleFitSteps();
			FetchGoogleFitCalories();
			FetchGoogleFitDistance();
			FetchGoogleFitActiveMinutes();
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

		public async void FetchGoogleFitSteps()
		{
			DataReadRequest readRequest = queryStepsData();

			var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(mGoogleApiClient, readRequest);
			var steps = 0.0;
			if (dataReadResult.Buckets.Count > 0)
			{
				foreach (Bucket bucket in dataReadResult.Buckets)
				{

					IList<DataSet> dataSets = bucket.DataSets;
					foreach (DataSet dataSet in dataSets)
					{
						steps = steps + GetDataSetValuesSum(dataSet);
					}

				}
			}

			PrintData(dataReadResult);
		}

		public async void FetchGoogleFitDistance()
		{
			DataReadRequest readRequest = queryDistance();

			var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(mGoogleApiClient, readRequest);
			var distance = 0.0;
			if (dataReadResult.Buckets.Count > 0)
			{
				foreach (Bucket bucket in dataReadResult.Buckets)
				{

					IList<DataSet> dataSets = bucket.DataSets;
					foreach (DataSet dataSet in dataSets)
					{
						distance = distance + GetDataSetValuesSum(dataSet);
					}

				}
			}

			PrintData(dataReadResult);
		}

		public async Task<double> FetchGoogleFitCalories()
		{
			DataReadRequest readRequest = queryActiveEnergy();

			var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(mGoogleApiClient, readRequest);
			var calories = 0.0;
			if (dataReadResult.Buckets.Count > 0)
			{
				foreach (Bucket bucket in dataReadResult.Buckets)
				{
					if (bucket.Activity.Contains(FitnessActivities.Walking) || bucket.Activity.Contains(FitnessActivities.Running))
					{
						IList<DataSet> dataSets = bucket.DataSets;
						foreach (DataSet dataSet in dataSets)
						{
							calories = calories + GetDataSetValuesSum(dataSet);
						}
					}
				}
			}
			PrintData(dataReadResult);
			return calories;
		}

		public async Task<double> FetchGoogleFitActiveMinutes()
		{
			DataReadRequest readRequest = queryActiveEnergy();

			var dataReadResult = await FitnessClass.HistoryApi.ReadDataAsync(mGoogleApiClient, readRequest);
			var totalActiveTime = 0.0;
			if (dataReadResult.Buckets.Count > 0)
			{
				foreach (Bucket bucket in dataReadResult.Buckets)
				{
					if (bucket.Activity.Contains(FitnessActivities.Walking) || bucket.Activity.Contains(FitnessActivities.Running))
					{
						long activeTime = bucket.GetEndTime(TimeUnit.Minutes) - bucket.GetStartTime(TimeUnit.Minutes);
						totalActiveTime = totalActiveTime + activeTime;

					}
				}
			}
			PrintData(dataReadResult);
			return totalActiveTime;
		}

		static double GetDataSetValuesSum(DataSet dataSet)
		{
			var dataSetSum = 0.0;
			foreach (DataPoint dp in dataSet.DataPoints)
			{
				foreach (Field field in dp.DataType.Fields)
				{

					try
					{
						dataSetSum = dataSetSum + Convert.ToDouble(dp.GetValue(field).ToString());
					}
					catch (Exception e) { }
				}
			}
			return dataSetSum;
		}

		static DataReadRequest queryStepsData()
		{
			DateTime endTime = DateTime.Now;
			//DateTime startTime = endTime.Subtract(TimeSpan.FromDays(7));
			DateTime startTime = DateTime.Today;
			long endTimeElapsed = GetMsSinceEpochAsLong(endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong(startTime);

			Log.Info("googleFitFetch", "Range Start: " + startTime.ToString(DATE_FORMAT));
			Log.Info("googleFitFetch", "Range End: " + endTime.ToString(DATE_FORMAT));
			DataSource source = new DataSource.Builder()
											  .SetAppPackageName("com.google.android.gms")
											  .SetDataType(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta)
											  .SetType(DataSource.TypeDerived)
											  .SetStreamName("estimated_steps")
											  .Build();

			var readRequest = new DataReadRequest.Builder()
				.Aggregate(source, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
				.BucketByTime(1, TimeUnit.Days)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.Build();

			return readRequest;
		}

		static DataReadRequest queryActiveEnergy()
		{
			DateTime endTime = DateTime.Now;
			//DateTime startTime = endTime.Subtract(TimeSpan.FromDays(7));
			DateTime startTime = DateTime.Today;
			long endTimeElapsed = GetMsSinceEpochAsLong(endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong(startTime);

			Log.Info("googleFitFetch", "Range Start: " + startTime.ToString(DATE_FORMAT));
			Log.Info("googleFitFetch", "Range End: " + endTime.ToString(DATE_FORMAT));


			var readRequest = new DataReadRequest.Builder()
				.Aggregate(Android.Gms.Fitness.Data.DataType.TypeCaloriesExpended, Android.Gms.Fitness.Data.DataType.AggregateCaloriesExpended)
				.BucketByActivitySegment(1, TimeUnit.Milliseconds)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.Build();

			return readRequest;
		}

		static DataReadRequest queryDistance()
		{
			DateTime endTime = DateTime.Now;
			//DateTime startTime = endTime.Subtract(TimeSpan.FromDays(7));
			DateTime startTime = DateTime.Today;
			long endTimeElapsed = GetMsSinceEpochAsLong(endTime);
			long startTimeElapsed = GetMsSinceEpochAsLong(startTime);

			Log.Info("googleFitFetch", "Range Start: " + startTime.ToString(DATE_FORMAT));
			Log.Info("googleFitFetch", "Range End: " + endTime.ToString(DATE_FORMAT));


			var readRequest = new DataReadRequest.Builder()
				.Aggregate(Android.Gms.Fitness.Data.DataType.TypeDistanceDelta, Android.Gms.Fitness.Data.DataType.AggregateDistanceDelta)
				.BucketByTime(1, TimeUnit.Days)
				.SetTimeRange(startTimeElapsed, endTimeElapsed, TimeUnit.Milliseconds)
				.Build();

			return readRequest;
		}


		static long GetMsSinceEpochAsLong(DateTime dateTime)
		{
			return (long)dateTime.ToUniversalTime()
				.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
				.TotalMilliseconds;
		}


		void PrintData(DataReadResult dataReadResult)
		{
			if (dataReadResult.Buckets.Count > 0)
			{
				Log.Info("TAG", "Number of returned buckets of DataSets is: "
				+ dataReadResult.Buckets.Count);

				foreach (Bucket bucket in dataReadResult.Buckets)
				{
					IList<DataSet> dataSets = bucket.DataSets;
					Log.Info("TAG", "bucket is: "
				+ bucket.Activity);
					foreach (DataSet dataSet in dataSets)
					{
						DumpDataSet(dataSet);
					}
				}
			}
			else if (dataReadResult.DataSets.Count > 0)
			{
				Log.Info("TAG", "Number of returned DataSets is: "
				+ dataReadResult.DataSets.Count);

				foreach (DataSet dataSet in dataReadResult.DataSets)
				{
					DumpDataSet(dataSet);
				}
			}
		}

		void DumpDataSet(DataSet dataSet)
		{
			Log.Info("TAG", "Data returned for Data type: " + dataSet.DataType.Name);
			foreach (DataPoint dp in dataSet.DataPoints)
			{
				Log.Info("TAG", "Data point:");
				Log.Info("TAG", "\tType: " + dp.DataType.Name);
				Log.Info("TAG", "\tStart: " + new DateTime(1970, 1, 1).AddMilliseconds(
					dp.GetStartTime(TimeUnit.Milliseconds)).ToString(DATE_FORMAT));
				Log.Info("TAG", "\tEnd: " + new DateTime(1970, 1, 1).AddMilliseconds(
					dp.GetEndTime(TimeUnit.Milliseconds)).ToString(DATE_FORMAT));
				foreach (Field field in dp.DataType.Fields)
				{
					Log.Info("TAG", "\tField: " + field.Name +
					" Value: " + dp.GetValue(field));
				}
			}
		}



	}
}
