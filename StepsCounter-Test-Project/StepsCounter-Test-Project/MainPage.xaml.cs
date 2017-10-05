using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StepsCounterApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FetchHealthData();

            // iterate over a collection adding tasks to the dispatch group

        }

        void FetchHealthData()
        {
            
            List<Task> tasks = new List<Task>();
            DependencyService.Get<IHealthData>().GetHealthPermissionAsync((result) =>
            {
                var a = result;
                if (result)
                {
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

                }
            });

        }
    }
}
