using System;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using StepsCounterApp;
using StepsCounterTestProject.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(HealthData))]
namespace StepsCounterTestProject.Droid
{
    public class HealthData : IHealthData, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        GoogleApiClient mGoogleApiClient;

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

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                                             .RequestProfile()
                                                             .Build();

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
            mGoogleApiClient.Connect();

            throw new NotImplementedException();
        }

    }
}
