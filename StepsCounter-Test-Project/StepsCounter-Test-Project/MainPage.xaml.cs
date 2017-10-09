using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth.XamarinForms;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Text;

namespace StepsCounterApp
{
    public partial class MainContentPage : ContentPage
    {

        bool launched = false;
        protected Xamarin.Auth.WebAuthenticator authenticator = null;
        public MainContentPage()
        {
            //InitializeComponent();
            initializeAuthenticator();
            // iterate over a collection adding tasks to the dispatch group


        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
            //FetchHealthData();
            if (launched == false)
            {
                AuthenticationState.Authenticator = authenticator;
                PageHelpers.PresentUILoginScreen(this,authenticator);
                //PresentUILoginScreen(authenticator);
                launched = true;
            }
        }

        void initializeAuthenticator()
        {
            authenticator = new OAuth2Authenticator(
                 "228N3T",
                 null,
                 "profile activity heartrate weight  location",
                 new Uri("https://www.fitbit.com/oauth2/authorize"),
                 new Uri("StepsCounter://"),
                 new Uri("https://api.fitbit.com/oauth2/token"),
                 null,
                 true);
            authenticator.Completed += OnAuthCompleted;

			authenticator.Error +=
				(s, ea) =>
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Error = ").AppendLine($"{ea.Message}");

					DisplayAlert
							(
								"Authentication Error",
								sb.ToString(),
								"OK"
							);
					return;
				};
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var a = e;
        }

        void FetchHealthData()
        {

            List<Task> tasks = new List<Task>();
            DependencyService.Get<IHealthData>().GetHealthPermissionAsync((result) =>
            {
                var a = result;
                if (result)
                { /*
                    DependencyService.Get<IHealthData>().FetchSteps((totalSteps) =>
                    {
						Device.BeginInvokeOnMainThread(() =>
						{
                            Label label = new Label
                            {
                                Text = "Total steps today: " + Math.Floor(totalSteps).ToString(),
                            };
                            ScrollView scroll = new ScrollView();
                            StackLayout newStack = new StackLayout();

                            this.myStack.Children.Add(label);
                            //this.   .Add(label);
						});
                    });

                    DependencyService.Get<IHealthData>().FetchMetersWalked((metersWalked) =>
                    {
						Device.BeginInvokeOnMainThread(() =>
						{
							Label label = new Label
							{
								Text = "Total meters walked today: " + Math.Floor(metersWalked).ToString(),
							};
							ScrollView scroll = new ScrollView();
							StackLayout newStack = new StackLayout();

							this.myStack.Children.Add(label);
						});
						
                    });

                    DependencyService.Get<IHealthData>().FetchActiveMinutes((activeMinutes) =>
                    {
						Device.BeginInvokeOnMainThread(() =>
						{
							Label label = new Label
							{
								Text = "Total excersice minutes today: " + Math.Floor(activeMinutes).ToString(),
							};
							ScrollView scroll = new ScrollView();
							StackLayout newStack = new StackLayout();

							this.myStack.Children.Add(label);
						});
						
                    });

					DependencyService.Get<IHealthData>().FetchActiveEnergyBurned((caloriesBurned) =>
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Label label = new Label
							{
								Text = "Total active calories burned today: " + Math.Floor(caloriesBurned).ToString(),
							};
							ScrollView scroll = new ScrollView();
							StackLayout newStack = new StackLayout();

							this.myStack.Children.Add(label);
						});

					});

                    // wait for them all to finish
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //this.Label1.Text = "Total steps today: " + Math.Floor(steps).ToString() + " Meters Walked " + Math.Floor(meters).ToString() + " Active minutes " + Math.Floor(minutes).ToString();
                    });
					*/
                }
            });

        }       
    }
}
